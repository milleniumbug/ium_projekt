using System;
using System.Threading.Tasks;
using Api.Models;
using Newtonsoft.Json.Linq;

namespace ApiClientLib.Deltas
{
    internal class DeltaAdd : IDelta
    {
	    public DeltaAdd(Product product, Guid? requestId = null)
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
		    var p = await client.Add(Product);
		    return p.Id;
	    }
    }
}
