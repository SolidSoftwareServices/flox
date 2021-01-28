using System;
using System.Collections.Generic;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.SmartMeterActivationBanner
{
	public class InputModel
    {
	    public ScreenEvent ToSmartActivationEvent { get; set; }
	    public ScreenEvent DismissBannerEvent { get; set; }
	}
}
