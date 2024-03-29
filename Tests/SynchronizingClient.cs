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

		private IApiClient2 onlineClient;

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
			await ApiClientAssert.AssertEqualContent(onlineClient, offlineClient);
		}

		[Test]
		public async Task BasicNoServerState()
		{
			await offlineClient.Add(p1);
			await offlineClient.Synchronize();
			await ApiClientAssert.AssertEqualContent(onlineClient, offlineClient);
		}

		[Test]
		public async Task RemovalOnOneAndUpdateOnAnother()
		{
			await onlineClient.Add(p1);
			await offlineClient.Synchronize();
			await offlineClient2.Synchronize();
			var pcl1 = (await offlineClient.GetAll()).First();
			await offlineClient.Delete(pcl1);
			var pcl2 = (await offlineClient2.GetAll()).First();
			await offlineClient2.IncreaseAmount(pcl2, 5);
			await offlineClient.Synchronize();
			await offlineClient2.Synchronize();
			await ApiClientAssert.AssertEqualContent(onlineClient, offlineClient);
			await ApiClientAssert.AssertEqualContent(onlineClient, offlineClient2);
			await ApiClientAssert.AssertEqualContent(offlineClient, offlineClient2);
		}

		[Test]
		public async Task UpdateOnBoth()
		{
			await onlineClient.Add(p1);
			await offlineClient.Synchronize();
			await offlineClient2.Synchronize();
			var pcl1 = (await offlineClient.GetAll()).First();
			await offlineClient.IncreaseAmount(pcl1, 5);
			var pcl2 = (await offlineClient2.GetAll()).First();
			await offlineClient2.IncreaseAmount(pcl2, 5);
			await offlineClient.Synchronize();
			await offlineClient2.Synchronize();
			await ApiClientAssert.AssertEqualContent(onlineClient, offlineClient);
			await ApiClientAssert.AssertEqualContent(onlineClient, offlineClient2);
			await ApiClientAssert.AssertEqualContent(offlineClient, offlineClient2);
			Assert.AreEqual(11, (await onlineClient.GetAll()).First().Amount);
		}
	}
}
