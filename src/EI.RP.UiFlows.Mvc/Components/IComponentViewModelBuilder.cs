using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;

namespace EI.RP.UiFlows.Mvc.Components
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