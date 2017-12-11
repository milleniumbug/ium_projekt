using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Api.Models;
using Functional.Maybe;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace ApiClientLib
{
	public class ApiClient : IApiClient2
	{
		private HttpClient client;

		private string openIdAddress;

		private string apiAddress;

		private ApiClient()
		{

		}

		public async Task<IEnumerable<Product>> GetAll()
		{
			var response = await client.GetAsync($"{apiAddress}/products");
			return JsonConvert.DeserializeObject<List<Product>>(await response.Content.ReadAsStringAsync());
		}

		public async Task<Product> Add(Product product)
		{
			var response = await client.PostAsync($"{apiAddress}/products", new StringContent(
				JsonConvert.SerializeObject(product),
				Encoding.UTF8,
				"application/json"));
			if(!response.IsSuccessStatusCode)
			{
				throw new ConnectionErrorException($"{response.StatusCode}");
			}
			else
			{
				var content = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Product>(content);
			}
		}

		public async Task Delete(Product product)
		{
			var response = await client.DeleteAsync($"{apiAddress}/products/{product.Id}");
			if(!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
			{
				throw new ConnectionErrorException($"{response.StatusCode}");
			}
		}

		public Task<Product> IncreaseAmount(Product product, int howMuch)
		{
			return ChangeAmount(product, howMuch, ProductPatch.OperationType.Increase);
		}

		public Task<Product> DecreaseAmount(Product product, int howMuch)
		{
			return ChangeAmount(product, howMuch, ProductPatch.OperationType.Decrease);
		}

		private async Task<Product> ChangeAmount(Product product, int howMuch, ProductPatch.OperationType type)
		{
			var response = await client.PatchAsync($"{apiAddress}/products/{product.Id}", new StringContent(
				JsonConvert.SerializeObject(new List<ProductPatch>
				{
					new ProductPatch
					{
						Operation = type,
						What = ProductPatch.TargetField.Amount,
						Value = howMuch
					}
				}),
				Encoding.UTF8,
				"application/json"));
			if(response.StatusCode == HttpStatusCode.NotFound)
			{
				throw new ElementNotFound($"{response.StatusCode}");
			}
			else if(!response.IsSuccessStatusCode)
			{
				throw new ConnectionErrorException($"{response.StatusCode}");
			}
			else
			{
				var content = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Product>(content);
			}
		}

		public static async Task<IApiClient2> Create(ConnectionSettings conn)
		{
			var client = new ApiClient();

			var discoveryClient = new DiscoveryClient(conn.OpenIdUrl);
			var disco = await discoveryClient.GetAsync();
			if(disco.IsError)
			{
				throw new ConnectionErrorException($"Error on OpenID discovery: {disco.Error}");
			}

			// request token
			var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
			var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(conn.Login, conn.Password, "api1");

			if(tokenResponse.IsError)
			{
				throw new ConnectionErrorException($"Error on OpenID login: {tokenResponse.Error}");
			}

			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

			client.client = httpClient;
			client.apiAddress = conn.ApiUrl;
			client.openIdAddress = conn.OpenIdUrl;
			return client;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			client?.Dispose();
		}

		/// <inheritdoc />
		public async Task<Maybe<Product>> Add(Product product, Guid requestId)
		{
			var response = await client.PostAsync($"{apiAddress}/products?deltaGuid={requestId}", new StringContent(
				JsonConvert.SerializeObject(product),
				Encoding.UTF8,
				"application/json"));
			if(response.StatusCode == HttpStatusCode.Conflict)
			{
				return Maybe<Product>.Nothing;
			}
			else if(!response.IsSuccessStatusCode)
			{
				throw new ConnectionErrorException($"{response.StatusCode}");
			}
			else
			{
				var content = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Product>(content).ToMaybe();
			}
		}

		/// <inheritdoc />
		public async Task Delete(Product product, Guid requestId)
		{
			var response = await client.DeleteAsync($"{apiAddress}/products/{product.Id}?deltaGuid={requestId}");
			if(!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
			{
				throw new ConnectionErrorException($"{response.StatusCode}");
			}
		}

		/// <inheritdoc />
		public Task<Maybe<Product>> IncreaseAmount(Product product, int howMuch, Guid requestId)
		{
			return ChangeAmount(product, howMuch, ProductPatch.OperationType.Increase, requestId);
		}

		/// <inheritdoc />
		public Task<Maybe<Product>> DecreaseAmount(Product product, int howMuch, Guid requestId)
		{
			return ChangeAmount(product, howMuch, ProductPatch.OperationType.Decrease, requestId);
		}

		private async Task<Maybe<Product>> ChangeAmount(Product product, int howMuch, ProductPatch.OperationType type, Guid requestId)
		{
			var response = await client.PatchAsync($"{apiAddress}/products/{product.Id}?deltaGuid={requestId}", new StringContent(
				JsonConvert.SerializeObject(new List<ProductPatch>
				{
					new ProductPatch
					{
						Operation = type,
						What = ProductPatch.TargetField.Amount,
						Value = howMuch
					}
				}),
				Encoding.UTF8,
				"application/json"));
			if(response.StatusCode == HttpStatusCode.Conflict)
			{
				return Maybe<Product>.Nothing;
			}
			else if(response.StatusCode == HttpStatusCode.NotFound)
			{
				throw new ElementNotFound($"{response.StatusCode}");
			}
			else if(!response.IsSuccessStatusCode)
			{
				throw new ConnectionErrorException($"{response.StatusCode}");
			}
			else
			{
				var content = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Product>(content).ToMaybe();
			}
		}
	}
}
