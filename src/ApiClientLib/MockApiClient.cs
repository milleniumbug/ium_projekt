using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;

namespace ApiClientLib
{
	public class MockApiClient : IApiClient
	{
		private List<Product> l = new List<Product>
		{
			new Product
			{
				Amount = 42,
				Id = 2,
				Name = "Snikers",
				Price = 1.60M,
				ShopName = "LIDL"
			},
			new Product
			{
				Amount = 0,
				Id = 3,
				Name = "Rzodkiewki",
				Price = 2.0M,
				ShopName = "Monopolowy za rogiem"
			}
		};

		private int idCounter = 42;

		/// <inheritdoc />
		public Task<IEnumerable<Product>> GetAll()
		{
			IEnumerable<Product> r = l.ToList();
			return Task.FromResult(r);
		}

		/// <inheritdoc />
		public Task<Product> Add(Product product)
		{
			var p = new Product(product);
			p.Id = idCounter++;
			l.Add(p);
			return Task.FromResult(p);
		}

		/// <inheritdoc />
		public Task Delete(Product product)
		{
			l.RemoveAll(p => p.Id == product.Id);
			return Task.FromResult(0);
		}

		/// <inheritdoc />
		public Task<Product> IncreaseAmount(Product product, int howMuch)
		{
			var p = l.FirstOrDefault(pr => pr.Id == product.Id);
			p.Amount += howMuch;
			return Task.FromResult(new Product(p));
		}

		/// <inheritdoc />
		public Task<Product> DecreaseAmount(Product product, int howMuch)
		{
			var p = l.FirstOrDefault(pr => pr.Id == product.Id);
			p.Amount -= howMuch;
			return Task.FromResult(new Product(p));
		}

		public static Task<IApiClient> Create(string login, string password)
		{
			IApiClient r = new MockApiClient();;
			return Task.FromResult(r);
		}

		public static Task<IApiClient> Create(string login, string password, string apiAddress, string openIdAddress)
		{
			IApiClient r = new MockApiClient();
			return Task.FromResult(r);
		}
	}
}