using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.Components.PlanDetails
{
    /// <summary>
    /// This class defines the the component PlanDetails
    /// </summary>
    /// <example>
    /// This sample shows how to include the component
    ///<code>
    ///<![CDATA[ <vc:plan-details screen-model = "@Model" input =  @( new  InputModel {SampleInputValue="This is a sample value" } )/>]] >
    /// </code>
    /// </example>
    [ViewComponent(Name = Name)]
    public class Component : FlowStepComponent<InputModel, ViewModel>
    {
        private readonly IComponentViewModelBuilder<InputModel, ViewModel> _builder;
        public const string Name = "PlanDetails";
        public Component(IComponentViewModelBuilder<InputModel, ViewModel> builder): base(Name)
        {
            _builder = builder;
        }

        protected override async Task<ViewModel> ResolveComponentDataAsync(InputModel input, UiFlowScreenModel screenModel)
        {
            return await _builder.Resolve(input, screenModel);
        }
    }
}