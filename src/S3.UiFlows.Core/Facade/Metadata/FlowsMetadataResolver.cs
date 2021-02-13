using System.Linq;
using System.Threading.Tasks;
using S3.UiFlows.Core.Flows;

namespace S3.UiFlows.Core.Facade.Metadata
{
	class FlowsMetadataResolver : IFlowsMetadataResolver
	{
		
		private readonly IAppUiFlowsCollection _flowsCollection;


		public FlowsMetadataResolver(IAppUiFlowsCollection flowsCollection)
		{
			_flowsCollection = flowsCollection;
		}


		public async Task<AppMetadata> GetMetadata()
		{
			var result = new AppMetadata();
			var flows=await _flowsCollection.GetAll();

			result.Flows = (await Task.WhenAll(flows.Select(x => x.GetMetadata()))).ToList();
			
			
			return result;
		}
	}
}
