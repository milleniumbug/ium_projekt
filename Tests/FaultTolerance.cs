using System;
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
	class FaultTolerance
	{
		private static readonly Product p1 = new Product
		{
			Amount = 1,
			Name = "Kartofle",
			Price = 1.25m,
			ShopName = "Biedra"
		};

		private async Task RetryUntilSucceeds(Func<Task> action)
		{
			while(true)
			{
				try
				{
					await action();
					return;
				}
				catch(ConnectionErrorException)
				{
					// ignored
				}
			}
		}

		private FailureInducingMockApiClient onlineClient;

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
			onlineClient = new FailureInducingMockApiClient(
				await MockApiClient.Create(conn),
				new Random(42));
			offlineClient = await SynchronizingApiClient.Create(
				first,
				conn,
				() => Task.FromResult<IApiClient2>(onlineClient));
			offlineClient2 = await SynchronizingApiClient.Create(
				second,
				conn,
				() => Task.FromResult<IApiClient2>(onlineClient));
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
			onlineClient.FaultProbability = 0.6;
			await RetryUntilSucceeds(async () => await offlineClient.Synchronize());
			onlineClient.FaultProbability = 0.0;
			await ApiClientAssert.AssertEqualContent(onlineClient, offlineClient);
		}

		[Test]
		public async Task BasicNoServerState()
		{
			await offlineClient.Add(p1);
			onlineClient.FaultProbability = 0.6;
			await RetryUntilSucceeds(async () => await offlineClient.Synchronize());
			onlineClient.FaultProbability = 0.0;
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
			onlineClient.FaultProbability = 0.6;
			await RetryUntilSucceeds(async () => await offlineClient.Synchronize());
			await RetryUntilSucceeds(async () => await offlineClient2.Synchronize());
			onlineClient.FaultProbability = 0.0;
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
			onlineClient.FaultProbability = 0.6;
			await RetryUntilSucceeds(async () => await offlineClient.Synchronize());
			await RetryUntilSucceeds(async () => await offlineClient2.Synchronize());
			onlineClient.FaultProbability = 0.0;
			await ApiClientAssert.AssertEqualContent(onlineClient, offlineClient);
			await ApiClientAssert.AssertEqualContent(onlineClient, offlineClient2);
			await ApiClientAssert.AssertEqualContent(offlineClient, offlineClient2);
			Assert.AreEqual(11, (await onlineClient.GetAll()).First().Amount);
		}
	}
}
