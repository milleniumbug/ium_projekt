using System.Threading.Tasks;
using Api.Models;
using Newtonsoft.Json.Linq;

namespace ApiClientLib.Deltas
{
    internal class DeltaAdd : IDelta
    {
	    public DeltaAdd(Product product)
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
		    var p = await client.Add(Product);
		    return p.Id;
	    }
    }
}
