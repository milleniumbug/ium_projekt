﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Models;
using ApiClientLib;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class SynchronizingClient
	{
		private static readonly Product p1 = new Product
		{
			Amount = 1,
			Name = "Kartofle",
			Price = 1.25m,
			ShopName = "Biedra"
		};

		private IApiClient onlineClient;

		private SynchronizingApiClient offlineClient;

		private SynchronizingApiClient offlineClient2;

		[SetUp]
		public async Task SetUp()
		{
			var rootPath = Environment.CurrentDirectory;
			var first = Directory.CreateDirectory(Path.Combine(rootPath, "first")).FullName;
			var second = Directory.CreateDirectory(Path.Combine(rootPath, "second")).FullName;
			var conn = new ConnectionSettings("dummy", "dummy");
			SynchronizingApiClient.InvalidateLocalStorage(first);
			SynchronizingApiClient.InvalidateLocalStorage(second);
			onlineClient = await MockApiClient.Create(conn);
			offlineClient = await SynchronizingApiClient.Create(
				first,
				conn,
				() => Task.FromResult(onlineClient));
			offlineClient2 = await SynchronizingApiClient.Create(
				second,
				conn,
				() => Task.FromResult(onlineClient));
		}

		[TearDown]
		public void TearDown()
		{
			onlineClient.Dispose();
			offlineClient.Dispose();
			offlineClient2.Dispose();
		}

		[Test]
		public async Task BasicNoClientState()
		{
			await onlineClient.Add(p1);
			await offlineClient.Synchronize();
			await AssertEqualContent(onlineClient, offlineClient);
		}

		private async Task AssertEqualContent(IApiClient expectedClient, IApiClient actualClient)
		{
			CollectionAssert.AreEqual(
				(await expectedClient.GetAll()).OrderBy(p => p.Id),
				(await actualClient.GetAll()).OrderBy(p => p.Id),
				Comparer<Product>.Create((left, right) =>
				{
					return 256 * Math.Sign(Comparer.Default.Compare(left.Id, right.Id)) +
						64 * Math.Sign(Comparer.Default.Compare(left.Amount, right.Amount)) +
						16 * Math.Sign(Comparer.Default.Compare(left.Name, right.Name)) +
						4 * Math.Sign(Comparer.Default.Compare(left.Price, right.Price)) +
						1 * Math.Sign(Comparer.Default.Compare(left.ShopName, right.ShopName));
				}));
		}
	}
}