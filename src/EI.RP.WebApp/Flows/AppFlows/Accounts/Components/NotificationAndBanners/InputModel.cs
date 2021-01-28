using System;
using System.Collections.Generic;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.NotificationAndBanners
{
	public class InputModel
    {
	    public string AccountNumber { get; set; }

		
	    public ScreenEvent ToSmartActivationEvent { get; set; }
	    public ScreenEvent ToCompetitionEvent { get; set; }
	    public ScreenEvent ToPromotionEvent { get; set; }
	    public ScreenEvent DismissSmartActivationBannerEvent { get; set; }
	    public ScreenEvent DismissCompetitionBannerEvent { get; set; }
	    public ScreenEvent DismissPromotionBannerEvent { get; set; }
	}
}
