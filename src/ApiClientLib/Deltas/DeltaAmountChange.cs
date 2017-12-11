using System;
using System.Threading.Tasks;
using Api.Models;
using Functional.Maybe;

namespace ApiClientLib.Deltas
{
	internal class DeltaAmountChange : IDelta
	{
		public int HowMuch { get; }

		public DeltaAmountChange(Product product, int howMuch, Guid? requestId = null)
		{
			this.Product = new Product(product);
			this.HowMuch = howMuch;
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
		public async Task<Maybe<long>> Apply(IApiClient2 client)
		{
			Maybe<Product> result;
			if(HowMuch > 0)
				result = await client.IncreaseAmount(Product, HowMuch, RequestId);
			else
				result = await client.DecreaseAmount(Product, -HowMuch, RequestId);
			return result.Select(p => p.Id);
		}
	}
}