using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using ApiClientLib;
using NUnit.Framework;

static internal class ApiClientAssert
{
	internal static async Task AssertEqualContent(IApiClient expectedClient, IApiClient actualClient)
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