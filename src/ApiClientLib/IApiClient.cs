using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models;
using Functional.Maybe;

namespace ApiClientLib
{
	public interface IApiClient : IDisposable
	{
		Task<IEnumerable<Product>> GetAll();
		Task<Product> Add(Product product);
		Task Delete(Product product);
		Task<Product> IncreaseAmount(Product product, int howMuch);
		Task<Product> DecreaseAmount(Product product, int howMuch);
	}

	public interface IApiClient2 : IApiClient
	{
		Task<Maybe<Product>> Add(Product product, Guid requestId);
		Task Delete(Product product, Guid requestId);
		Task<Maybe<Product>> IncreaseAmount(Product product, int howMuch, Guid requestId);
		Task<Maybe<Product>> DecreaseAmount(Product product, int howMuch, Guid requestId);
	}
}