using System;
using System.Collections.Generic;
using AutoFixture.Dsl;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts
{
	public class GasAccountConfigurator : CommonElectricityAndGasAccountConfigurator
	{
		public GasAccountConfigurator(DomainFacade domainFacade) : base(domainFacade, ClientAccountType.Gas)
		{
		}
		public GasAccountConfigurator WithGasDevice()
		{
			return (GasAccountConfigurator)WithDeviceSet(ConfigurableDeviceSet.Gas);
		}


		protected override IPostprocessComposer<AccountInfo> ConfigureSpecificTypeAccountInfo(IPostprocessComposer<AccountInfo> composer)
		{
			composer= base.ConfigureSpecificTypeAccountInfo(composer)
				.With(x=>x.SmartActivationStatus,SmartActivationStatus.SmartNotAvailable);
			ConfigureSmartPeriods();
			return composer;

			void ConfigureSmartPeriods()
			{
				composer = composer
					.With(x => x.SmartPeriods, new DateTimeRange[0])
					.With(x => x.NonSmartPeriods, new DateTimeRange[0]);
			}
		}
	}
}