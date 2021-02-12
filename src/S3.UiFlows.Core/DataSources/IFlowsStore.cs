using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3.UiFlows.Core.DataSources
{
	public interface IFlowsStore
	{
		Task<IEnumerable<string>> GetValuesAsync();
		Task<string> GetAsync(string key);
		Task SetAsync(string key, string value);
		Task RemoveAsync(string key);
		Task ClearAsync();
	}
}
