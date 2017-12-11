using System;
using System.Threading.Tasks;
using Api.Models;
using Functional.Maybe;

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
	    public async Task<Maybe<long>> Apply(IApiClient2 client)
	    {
		    var product = await client.Add(Product, RequestId);
		    return product.Select(p => p.Id);
	    }
    }
}
