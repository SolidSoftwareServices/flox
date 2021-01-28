using AutoFixture;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Platform;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.Stubs.CoreServices.Http.Session;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Components.SmartMeterActivationBanner;
using EI.RP.WebApp.Infrastructure.StringResources;
using EI.RP.WebApp.UnitTests.Infrastructure;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Infrastructure.Settings;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Accounts.Components.SmartMeterActivationBanner
{
	class SmartMeterActivationBannerTestContext : UnitTestContext<ViewModelBuilder>
	{
		private bool _hasSmartEligibleAccounts;
		private bool _isNotificationDismissed;
		private bool _isSmartActivationEnabled;
		private bool _isNotificationDismissedForSession;
		private ClientAccountType _accountType = ClientAccountType.Electricity;
		private bool _isOpen;
		private bool _isAccountAlreadySmart;
		private ScreenEvent _dismissEvent;
		private ScreenEvent _flowEvent;

		public DomainFacade DomainFacade { get; } = new DomainFacade();
		public IEnumerable<AccountInfo> ElectricityAccounts { get; private set; }

		public SmartMeterActivationBannerTestContext WithHasSmartEligibleAccounts(bool canOptToSmart)
		{
			_hasSmartEligibleAccounts = canOptToSmart;
			return this;
		}

		
		public SmartMeterActivationBannerTestContext WithEvents(ScreenEvent dismissEvent, ScreenEvent flowEvent)
		{
			_dismissEvent = dismissEvent;
			_flowEvent = flowEvent;
			return this;
		}

		public SmartMeterActivationBannerTestContext WithIsNotificationDismissed(bool isNotificationDismissed)
		{
			_isNotificationDismissed = isNotificationDismissed;
			return this;
		}

		public SmartMeterActivationBannerTestContext WithIsNotificationDismissedForSession(bool isNotificationDismissedForSession)
		{
			_isNotificationDismissedForSession = isNotificationDismissedForSession;
			return this;
		}

		public SmartMeterActivationBannerTestContext WithIsSmartActivationEnabled(bool isSmartActivationEnabled)
		{
			_isSmartActivationEnabled = isSmartActivationEnabled;
			return this;
		}

		public SmartMeterActivationBannerTestContext WithAccountType(ClientAccountType accountType)
		{
			_accountType = accountType;
			return this;
		}

		public SmartMeterActivationBannerTestContext WithIsAccountOpen(bool isOpen)
		{
			_isOpen = isOpen;
			return this;
		}

		public SmartMeterActivationBannerTestContext WithIsAccountAlreadySmart(bool alreadySmart)
		{
			_isAccountAlreadySmart = alreadySmart;
			return this;
		}

		public SmartMeterActivationBannerTestContext()
		{
			Fixture.CustomizeDomainTypeBuilders();
		}

		public InputModel BuildInput()
		{
			return new InputModel
			{
				DismissBannerEvent = _dismissEvent,
				ToSmartActivationEvent = _flowEvent
			};
		}

		protected override ViewModelBuilder BuildSut(AutoMocker autoMocker)
		{
			SetupMocks();
			return base.BuildSut(autoMocker);

			void SetupMocks()
			{
				var cfg=new AppUserConfigurator(DomainFacade);

				if (_accountType == ClientAccountType.Electricity)
				{
					var configurator = cfg.AddElectricityAccount(_isOpen, isSmart: _isAccountAlreadySmart,
						switchToSmartPlanDismissed: _isNotificationDismissed||!_isSmartActivationEnabled);

					if (_isAccountAlreadySmart)
					{
						configurator
							.WithElectricity24HrsDevices(RegisterConfigType.MCC12, CommsTechnicallyFeasibleValue.CTF3);
					}
					else
					{
						if (_hasSmartEligibleAccounts)
						{
							configurator
								.WithElectricity24HrsDevices(RegisterConfigType.MCC16,
									CommsTechnicallyFeasibleValue.CTF3);
						}
						else
						{
							configurator
								.WithElectricity24HrsDevices(RegisterConfigType.MCC01,
									CommsTechnicallyFeasibleValue.CTF1);
						}
					}
				}
				else if (_accountType==ClientAccountType.Gas)
				{
					cfg.AddGasAccount(_isOpen);
				}
				else
				{
					throw new NotSupportedException();
				}

				cfg.Execute();
				

				DomainFacade.SetUpMocker(autoMocker);
				

				var platformSettings = new Mock<IDomainSettings>();
				platformSettings.Setup(_ => _.IsSmartActivationEnabled).Returns(_isSmartActivationEnabled);

				autoMocker.Use(platformSettings);

				ElectricityAccounts = cfg.Accounts.Where(x => x.ClientAccountType == ClientAccountType.Electricity).ToArray();

			}
			
		}
	}
}
