using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.PromotionEntry.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Settings;

namespace EI.RP.WebApp.Flows.AppFlows.PromotionEntry.Steps
{
	public class PromotionEntry : PromotionEntryScreen
	{
		private readonly IUiAppSettings _uiAppSettings;
		public override ScreenName ScreenStep => PromotionEntryStep.PromotionEntry;

		public PromotionEntry(IUiAppSettings uiAppSettings)
		{
			_uiAppSettings = uiAppSettings;
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var screenModel = new ScreenModel
			{
				PromotionHeading = _uiAppSettings.PromotionHeading,
				PromotionDescription1 = _uiAppSettings.PromotionDescription1,
				PromotionDescription2 = _uiAppSettings.PromotionDescription2,
				PromotionLinkText = _uiAppSettings.PromotionLinkText,
				PromotionLinkURL = _uiAppSettings.PromotionLinkURL,
				PromotionPageTitle = _uiAppSettings.PromotionPageTitle,
				PromotionDescription3 = _uiAppSettings.PromotionDescription3,
				PromotionDescription4 = _uiAppSettings.PromotionDescription4,
				PromotionTermsConditionsLinkText = _uiAppSettings.PromotionTermsConditionsLinkText,
				PromotionTermsConditionsLinkURL = _uiAppSettings.PromotionTermsConditionsLinkURL,
				PromotionImageDesktop = _uiAppSettings.PromotionImageDesktop,
				PromotionImageMobile = _uiAppSettings.PromotionImageMobile,
				PromotionImageHeader = _uiAppSettings.PromotionImageHeader
			};

			SetTitle(Title, screenModel);

			return screenModel;
		}

		public class ScreenModel : UiFlowScreenModel
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == PromotionEntryStep.PromotionEntry;
			}

			public string PromotionHeading { get; set; }
			public string PromotionDescription1 { get; set; }
			public string PromotionDescription2 { get; set; }
			public string PromotionLinkText { get; set; }
			public string PromotionLinkURL { get; set; }
			public string PromotionPageTitle { get; set; }
			public string PromotionDescription3 { get; set; }
			public string PromotionDescription4 { get; set; }
			public string PromotionTermsConditionsLinkText { get; set; }
			public string PromotionTermsConditionsLinkURL { get; set; }
			public string PromotionImageDesktop { get; set; }
			public string PromotionImageMobile { get; set; }
			public string PromotionImageHeader { get; set; }
		}
	}
}
