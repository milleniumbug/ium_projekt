// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Api.Models;
using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleClient
{
	public class Program
	{
		public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

		private static async Task MainAsync()
		{
			// discover endpoints from metadata
			var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
			if(disco.IsError)
			{
				Console.WriteLine(disco.Error);
				return;
			}

			// request token
			var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
			var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("brockallen@gmail.com", "Pass123$", "api1");

			if(tokenResponse.IsError)
			{
				Console.WriteLine(tokenResponse.Error);
				return;
			}

			Console.WriteLine(tokenResponse.Json);
			Console.WriteLine("\n\n");

			// call api
			var client = new HttpClient();
			client.SetBearerToken(tokenResponse.AccessToken);
			long? id = null;
			{
				var response = await client.PostAsync("http://localhost:5001/products", new StringContent(
					JsonConvert.SerializeObject(new Product
					{
						Amount = 0,
						Price = 42.4M,
						ShopName = "Kerfur",
						Name = "Mleko"
					}),
					Encoding.UTF8,
					"application/json"));
				if(!response.IsSuccessStatusCode)
				{
					Console.WriteLine(response.StatusCode);
				}
				else
				{
					var content = await response.Content.ReadAsStringAsync();
					var product = JsonConvert.DeserializeObject<Product>(content);
					id = product.Id;
				}
			}
			{
				var response = await client.PatchAsync($"http://localhost:5001/products/{id.Value}", new StringContent(
					JsonConvert.SerializeObject(new List<ProductPatch>
					{
						new ProductPatch
						{
							Operation = ProductPatch.OperationType.Increase,
							What = ProductPatch.TargetField.Amount,
							Value = 4
						}
					}),
					Encoding.UTF8,
					"application/json"));
				if(!response.IsSuccessStatusCode)
				{
					Console.WriteLine(response.StatusCode);
				}
				else
				{
					var content = await response.Content.ReadAsStringAsync();
					var product = JsonConvert.DeserializeObject<Product>(content);
				}
			}
			id = null;
			{
				var response = await client.PostAsync("http://localhost:5001/products", new StringContent(
					JsonConvert.SerializeObject(new Product
					{
						Amount = 0,
						Price = 1.4M,
						ShopName = "Biedronka",
						Name = "Kartofle"
					}),
					Encoding.UTF8,
					"application/json"));
				if(!response.IsSuccessStatusCode)
				{
					Console.WriteLine(response.StatusCode);
				}
				else
				{
					var content = await response.Content.ReadAsStringAsync();
					var product = JsonConvert.DeserializeObject<Product>(content);
					id = product.Id;
				}
			}
			{
				var response = await client.DeleteAsync($"http://localhost:5001/products/{id.Value}");
				if(!response.IsSuccessStatusCode)
				{
					Console.WriteLine(response.StatusCode);
				}
			}
			{
				var response = await client.GetAsync("http://localhost:5001/products");
				if(!response.IsSuccessStatusCode)
				{
					Console.WriteLine(response.StatusCode);
				}
				else
				{
					var content = await response.Content.ReadAsStringAsync();
					Console.WriteLine(JArray.Parse(content));
				}
			}
		}
	}
}