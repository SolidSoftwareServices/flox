using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Mvc.Components
{
	public interface IComponentViewModelBuilder<in TComponentInput,TComponentModel> where TComponentModel: FlowComponentViewModel
	{
		/// <summary>
		/// It resolves the step data related to the component
		/// </summary>
		/// <param name="componentInput"></param>
		/// <param name="screenModelContainingTheComponent"></param>
		/// <returns></returns>
		Task<TComponentModel> Resolve(TComponentInput componentInput, UiFlowScreenModel screenModelContainingTheComponent=null);
	}
}