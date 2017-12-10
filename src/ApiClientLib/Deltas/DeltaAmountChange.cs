using System.Threading.Tasks;
using Api.Models;

namespace ApiClientLib.Deltas
{
	internal class DeltaAmountChange : IDelta
	{
		public int HowMuch { get; }

		public DeltaAmountChange(Product product, int howMuch)
		{
			this.Product = new Product(product);
			this.HowMuch = howMuch;
		}

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
			if(HowMuch > 0)
				await client.IncreaseAmount(Product, HowMuch);
			else
				await client.DecreaseAmount(Product, -HowMuch);
			return Product.Id;
		}
	}
}