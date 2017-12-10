using System.Threading.Tasks;
using Api.Models;

namespace ApiClientLib.Deltas
{
	internal class DeltaDelete : IDelta
	{
		public DeltaDelete(Product product)
		{
			this.Product = new Product(product);
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
			await client.Delete(Product);
			return Product.Id;
		}
	}
}