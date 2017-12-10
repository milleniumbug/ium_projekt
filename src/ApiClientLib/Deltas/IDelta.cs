using System.Threading.Tasks;
using Api.Models;

namespace ApiClientLib.Deltas
{
	internal interface IDelta
	{
		void RebindId(long id);
		Product Product { get; }
		Task<long> Apply(IApiClient client);
	}
}