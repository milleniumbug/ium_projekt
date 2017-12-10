// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
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
			using(var client = await ApiClientLib.OfflineApiClient.Create(Environment.CurrentDirectory, "brockallen@gmail.com", "Pass123$"))
			{
				var p1 = await client.Add(new Product
				{
					Amount = 0,
					Name = "Mleko",
					ShopName = "Kerfur",
					Price = 2.5M
				});

				var p2 = await client.Add(new Product
				{
					Amount = 0,
					Name = "Kartofle",
					ShopName = "Biedronka",
					Price = 1.5M
				});
				await client.IncreaseAmount(p2, 4);
				await client.Delete(p1);
				var products = await client.GetAll();
				foreach(var p in products)
				{
					Console.WriteLine(p);
				}
				await client.Synchronize();
			}
		}
	}
}