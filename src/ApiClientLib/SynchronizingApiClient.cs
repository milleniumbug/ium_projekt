using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Models;
using ApiClientLib.Deltas;
using Functional.Maybe;
using Newtonsoft.Json;

namespace ApiClientLib
{
	public class SynchronizingApiClient : IApiClient
	{
		private class UserIdentity
		{
			public string Login { get; set; }
		}

		private enum ServerState
		{
			Exists,
			ExistsButModifiedLocally,
			NonExisting
		}

		private class ClientProduct
		{
			public ServerState State { get; set; }

			public Product Product { get; set; }
		}

		private long localId;

		private Func<Task<IApiClient2>> createOnlineClient;

		private Dictionary<long, ClientProduct> products;

		private Queue<IDelta> deltas;

		private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto
		};

		private static readonly string deltaFilenameComponent = "deltas.json";
		private static readonly string productsFilenameComponent = "products.json";
		public string offlineStoragePathBase;
		private static readonly string userIdentityFilenameComponent = "user.json";

		private SynchronizingApiClient()
		{

		}

		/// <inheritdoc />
		public Task<IEnumerable<Product>> GetAll()
		{
			IEnumerable<Product> Impl()
			{
				foreach(var product in products)
				{
					yield return product.Value.Product;
				}
			}

			return Task.FromResult(Impl());
		}

		/// <inheritdoc />
		public Task<Product> Add(Product product)
		{
			product = new Product(product) { Id = localId-- };
			products.Add(product.Id, new ClientProduct
			{
				Product = product,
				State = ServerState.NonExisting
			});
			deltas.Enqueue(new DeltaAdd(product));
			return Task.FromResult(product);
		}

		/// <inheritdoc />
		public Task Delete(Product product)
		{
			products.Remove(product.Id);
			deltas.Enqueue(new DeltaDelete(product));
			return Task.FromResult(0);
		}

		/// <inheritdoc />
		public Task<Product> IncreaseAmount(Product product, int howMuch)
		{
			return ChangeAmount(product, howMuch);
		}

		/// <inheritdoc />
		public Task<Product> DecreaseAmount(Product product, int howMuch)
		{
			return ChangeAmount(product, -howMuch);
		}

		private Task<Product> ChangeAmount(Product product, int howMuch)
		{
			var delta = new DeltaAmountChange(product, howMuch);
			product = new Product(product);
			product.Amount += howMuch;
			deltas.Enqueue(delta);
			return Task.FromResult(product);
		}

		// synchronizes the state with the server
		// updates the local state
		// invalidates all returned Products
		public async Task Synchronize()
		{
			var idMappings = new Dictionary<long, long>();

			void UpdateDelta(IDelta delta)
			{
				long oldId = delta.Product.Id;
				if(idMappings.TryGetValue(oldId, out var newId))
				{
					delta.RebindId(newId);
				}
			}

			using(var onlineClient = await createOnlineClient())
			{
				try
				{
					while(deltas.Count != 0)
					{
						var delta = deltas.Peek();
						try
						{
							UpdateDelta(delta);
							long oldId = delta.Product.Id;
							Maybe<long> maybeNewId = await delta.Apply(onlineClient);
							maybeNewId.Do(newId =>
							{
								if(oldId != newId)
									idMappings.Add(oldId, newId);
							});
						}
						catch(ElementNotFound)
						{
							// the element we are patching was removed
						}
						deltas.Dequeue();
					}
				}
				finally
				{
					foreach(var delta in deltas)
					{
						UpdateDelta(delta);
					}
				}
				products = (await onlineClient.GetAll()).Select(p => new ClientProduct
				{
					Product = p,
					State = ServerState.Exists
				}).ToDictionary(clientProduct => clientProduct.Product.Id);
			}
		}

		public void Save()
		{
			File.WriteAllText(
				Path.Combine(offlineStoragePathBase, deltaFilenameComponent),
				JsonConvert.SerializeObject(deltas, jsonSerializerSettings),
				Encoding.UTF8);
			File.WriteAllText(
				Path.Combine(offlineStoragePathBase, productsFilenameComponent),
				JsonConvert.SerializeObject(products.Select(kvp => kvp.Value), jsonSerializerSettings),
				Encoding.UTF8);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			Save();
		}

		public static Task<SynchronizingApiClient> Create(
			string offlineStoragePathBase,
			ConnectionSettings conn)
		{
			return Create(offlineStoragePathBase, conn, () => ApiClient.Create(conn));
		}

		public static Task<SynchronizingApiClient> Create(string offlineStoragePathBase, ConnectionSettings conn, Func<Task<IApiClient2>> onlineClientFactory)
		{
			if(!IsLocalStorageValid(offlineStoragePathBase, conn))
			{
				InvalidateLocalStorage(offlineStoragePathBase);
			}
			var client = new SynchronizingApiClient
			{
				offlineStoragePathBase = offlineStoragePathBase,
				createOnlineClient = onlineClientFactory,
				deltas = LoadDeltas(offlineStoragePathBase),
				products = LoadProducts(offlineStoragePathBase)
			};
			var id = client.products.Select(p => p.Value.Product.Id).DefaultIfEmpty(0).Min() - 1;
			client.localId = Math.Min(id, -1);
			File.WriteAllText(
				Path.Combine(offlineStoragePathBase, userIdentityFilenameComponent),
				JsonConvert.SerializeObject(new UserIdentity{Login = conn.Login}, jsonSerializerSettings),
				Encoding.UTF8);

			return Task.FromResult(client);
		}

		private static bool IsLocalStorageValid(string offlineStoragePathBase, ConnectionSettings conn)
		{
			try
			{
				var userIdentity = JsonConvert.DeserializeObject<UserIdentity>(
					File.ReadAllText(Path.Combine(offlineStoragePathBase, userIdentityFilenameComponent)));
				return userIdentity.Login == conn.Login;
			}
			catch(FileNotFoundException)
			{
				return false;
			}
		}

		public static void InvalidateLocalStorage(string offlineStoragePathBase)
		{
			File.Delete(Path.Combine(offlineStoragePathBase, productsFilenameComponent));
			File.Delete(Path.Combine(offlineStoragePathBase, deltaFilenameComponent));
			File.Delete(Path.Combine(offlineStoragePathBase, userIdentityFilenameComponent));
		}

		private static Dictionary<long, ClientProduct> LoadProducts(string offlineStoragePathBase)
		{
			try
			{
				return JsonConvert.DeserializeObject<IEnumerable<ClientProduct>>(
					File.ReadAllText(Path.Combine(offlineStoragePathBase, productsFilenameComponent)),
					jsonSerializerSettings).ToDictionary(clientProduct => clientProduct.Product.Id);
			}
			catch(FileNotFoundException)
			{
				return new Dictionary<long, ClientProduct>();
			}
		}

		private static Queue<IDelta> LoadDeltas(string offlineStoragePathBase)
		{
			try
			{
				return new Queue<IDelta>(JsonConvert.DeserializeObject<IEnumerable<IDelta>>(
					File.ReadAllText(Path.Combine(offlineStoragePathBase, deltaFilenameComponent)),
					jsonSerializerSettings));
			}
			catch(FileNotFoundException)
			{
				return new Queue<IDelta>();
			}
		}
	}
}