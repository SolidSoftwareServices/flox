using System;
using Ei.Rp.Mvc.Core;
using Ei.Rp.Mvc.Core.Cryptography.Urls;

namespace EI.RP.WebApp.Infrastructure.Settings
{
	public interface IUiAppSettings:IUrlEncryptionSettings,IRequestPipelineCoreSettings, IGoogleSettings
	{
		bool ShowDevelopmentTools { get; }

		bool RequireCookiesPolicyCompliance { get; }
		bool IsPromotionsEnabled { get; }
        bool IsCompetitionEnabled { get; }
        string LogsRoot { get; }
		bool LogViewerEnabled { get; }

		bool FlowDebuggerIsEnabled { get; }
        string ShopBaseUrl { get; }
        string StoreBaseUrl { get; }
        string ElectricIrelandBaseUrl { get; }
        string CompetitionName { get; }
        string CompetitionHeading { get; }
        string CompetitionDescription { get; }
        string CompetitionDescription1 { get; }
        string CompetitionDescription2 { get; }
        string CompetitionDescription3 { get; }
        string CompetitionQuestionPart1 { get; }
        string CompetitionQuestionPart2 { get; }
        string CompetitionQuestionPart3 { get; }
        string CompetitionAnswerA { get; }
        string CompetitionAnswerB { get; }
        string CompetitionAnswerC { get; }
        string CompetitionTermAndConditionsUrl { get; }
        string CompetitionAlreadyEnteredMessage { get; }
		IImagesSetting CompetitionBannerImages { get; }
		IImagesSetting CompetitionPageImages { get; }
		string PromotionHeading { get; }
        string PromotionDescription1 { get; }
        string PromotionDescription2 { get; }
        string PromotionLinkText { get; }
        string PromotionLinkURL { get; }
        string PromotionPageTitle { get; }
        string PromotionDescription3 { get; }
        string PromotionDescription4 { get; }
        string PromotionTermsConditionsLinkText { get; }
        string PromotionTermsConditionsLinkURL { get; }
        string PromotionImageDesktop { get; }
        string PromotionImageMobile { get; }
        string PromotionImageHeader { get; }
	}
}