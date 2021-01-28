using System.Linq;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Runtime;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.IoC;

namespace EI.RP.UiFlows.Core.Facade.Metadata
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
