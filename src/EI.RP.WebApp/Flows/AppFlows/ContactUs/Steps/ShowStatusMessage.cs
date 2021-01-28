using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.ContactUs.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.ContactUs.Steps
{
	public class ShowStatusMessage : ContactUsScreen
	{
		public override ScreenName ScreenStep => ContactUsStep.ShowContactUsStatusMessage;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<ContactUs.ScreenModel>();

			var screenModel =  new ScreenModel
			{
				QueryType = rootData.SelectedQueryType, 
				Comments = rootData.CommentText, 
				Subject = rootData.Subject
			};

			SetTitle(Title, screenModel);

			return screenModel;
		}

		public class ScreenModel : UiFlowScreenModel
		{
			public string QueryType { get; set; }

			public string Subject { get; set; }

			public string Comments { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == ContactUsStep.ShowContactUsStatusMessage;
			}
		}
	}
}