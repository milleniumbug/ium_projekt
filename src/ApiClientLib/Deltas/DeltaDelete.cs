using System;
using System.Threading.Tasks;
using Api.Models;

namespace ApiClientLib.Deltas
{
	internal class DeltaDelete : IDelta
	{
		public DeltaDelete(Product product, Guid? requestId = null)
		{
			this.Product = new Product(product);
			if(requestId == null)
				requestId = Guid.NewGuid();
			RequestId = requestId.Value;
		}

		/// <inheritdoc />
		public Guid RequestId { get; }

		/// <inheritdoc />
		public void RebindId(long id)
		{
			Product.Id = id;
		}

		/// <inheritdoc />
		public Product Product { get; }

		/// <inheritdoc />
		public async Task<long> Apply(IApiClient client)
		{
			await client.Delete(Product);
			return Product.Id;
		}
	}
}