using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EI.RP.WebApp.Flows.AppFlows.Agent.Components.AgentFooter
{
    [ViewComponent(Name = Name)]
    public class Component : FlowStepComponent<InputModel, ViewModel>
    {
        private readonly IComponentViewModelBuilder<InputModel, ViewModel> _builder;
        public const string Name = "AgentFooter";

        public Component(IComponentViewModelBuilder<InputModel, ViewModel> builder) : base(Name)
        {
            _builder = builder;
        }

        protected override async Task<ViewModel> ResolveComponentDataAsync(InputModel input, UiFlowScreenModel screenModel)
        {
            return await _builder.Resolve(input, screenModel);
        }
    }
}
