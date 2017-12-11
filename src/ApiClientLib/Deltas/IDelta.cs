using System;
using System.Threading.Tasks;
using Api.Models;
using Functional.Maybe;

namespace ApiClientLib.Deltas
{
	internal interface IDelta
	{
		Guid RequestId { get; }
		void RebindId(long id);
		Product Product { get; }
		Task<Maybe<long>> Apply(IApiClient2 client);
	}
}