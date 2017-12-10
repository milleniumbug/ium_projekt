using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;

namespace ApiClientLib
{
	public class MockApiClient : IApiClient
	{
		private List<Product> l;

		private int idCounter = 42;

		private MockApiClient(IEnumerable<Product> products)
		{
			l = new List<Product>(products);
		}

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
			if(p == null)
				throw new ElementNotFound();
			p.Amount += howMuch;
			return Task.FromResult(new Product(p));
		}

		/// <inheritdoc />
		public Task<Product> DecreaseAmount(Product product, int howMuch)
		{
			var p = l.FirstOrDefault(pr => pr.Id == product.Id);
			if(p == null)
				throw new ElementNotFound();
			p.Amount -= howMuch;
			return Task.FromResult(new Product(p));
		}

		public static Task<IApiClient> Create(ConnectionSettings conn)
		{
			return Create(conn, Enumerable.Empty<Product>());
		}

		public static Task<IApiClient> Create(ConnectionSettings conn, IEnumerable<Product> products)
		{
			IApiClient r = new MockApiClient(products);
			return Task.FromResult(r);
		}

		/// <inheritdoc />
		public void Dispose()
		{
		}
	}
}