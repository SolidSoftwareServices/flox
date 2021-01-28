using System.Collections.Generic;
using AutoFixture.Dsl;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts
{
	public class ElectricityAccountConfigurator : CommonElectricityAndGasAccountConfigurator
	{
		private IEnumerable<DateTimeRange> _smartPeriods;
		private IEnumerable<DateTimeRange> _nonSmartPeriods;
		public ElectricityAccountConfigurator(DomainFacade domainFacade) : base(domainFacade, ClientAccountType.Electricity)
		{
		}

		public ElectricityAccountConfigurator WithElectricity24HrsDevices(RegisterConfigType configType = null,CommsTechnicallyFeasibleValue ctfValue=null)
		{
			return (ElectricityAccountConfigurator)WithDeviceSet(ConfigurableDeviceSet.Electricity24H,configType, ctfValue);
		}

		public ElectricityAccountConfigurator WithElectricityDayAndNightDevices(RegisterConfigType configType = null, CommsTechnicallyFeasibleValue ctfValue = null)
		{
			return (ElectricityAccountConfigurator)WithDeviceSet(ConfigurableDeviceSet.ElectricityDayAndNightMeters, configType, ctfValue);
		}

		public ElectricityAccountConfigurator WithElectricityNightStorageHeaterDevice(RegisterConfigType configType = null, CommsTechnicallyFeasibleValue ctfValue = null)
		{
			return (ElectricityAccountConfigurator) WithDeviceSet(ConfigurableDeviceSet.ElectricityNightStorageHeater, configType, ctfValue);
		}
		public virtual CommonElectricityAndGasAccountConfigurator WithSmartPeriods(IEnumerable<DateTimeRange> smartPeriods)
		{
			_smartPeriods = smartPeriods ?? new DateTimeRange[0];
			return this;
		}
		public virtual CommonElectricityAndGasAccountConfigurator WithNonSmartPeriods(DateTimeRange[] nonSmartPeriods)
		{
			_nonSmartPeriods = nonSmartPeriods ?? new DateTimeRange[0];
			return this;
		}
		protected override IPostprocessComposer<AccountInfo> ConfigureSpecificTypeAccountInfo(IPostprocessComposer<AccountInfo> composer)
		{
			composer= base.ConfigureSpecificTypeAccountInfo(composer);
			AddPeriods();

			return composer;
			void AddPeriods()
			{
				if (_smartPeriods != null)
				{
					composer = composer.With(x => x.SmartPeriods, _smartPeriods);
				}
				else
				{
					composer = composer.With(x => x.SmartPeriods,
						new DateTimeRange[0]);
				}

				if (_nonSmartPeriods != null)
				{
					composer = composer.With(x => x.NonSmartPeriods, _nonSmartPeriods);
				}
				else
				{
					composer = composer.With(x => x.NonSmartPeriods,
						new DateTimeRange[0]);
				}
			}
		}
		
	}
}