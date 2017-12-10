using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models;

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
}