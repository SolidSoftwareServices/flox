using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.FixtureExtensions;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.Queries.Metering.Premises;
using NUnit.Framework;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Premises
{
	public class PremiseConfigurator
	{
		private readonly string _contractId;
		private DomainFacade Domain { get; }

		public PremiseConfigurator(DomainFacade domain, ClientAccountType accountType, string contractId)
		{
			_contractId = contractId;
			Domain = domain;
			var fixture = domain.ModelsBuilder;
			PremiseId = fixture.Create<long>().ToString();
			if (accountType == ClientAccountType.Gas)
			{
				GasPrn = fixture.Create<GasPointReferenceNumber>();
			}
			else if (accountType == ClientAccountType.Electricity)
			{
				ElectricityPrn = fixture.Create<ElectricityPointReferenceNumber>();
			}
		}

		public string PremiseId
		{
			get => _premiseId;
			set
			{
				ThrowIfAlreadyExecuted();
				_premiseId = value;
			}
		}

		public ElectricityPointReferenceNumber ElectricityPrn
		{
			get => _electricityPrn;
			set
			{
				ThrowIfAlreadyExecuted();
				_electricityPrn = value;
			}
		}

		public GasPointReferenceNumber GasPrn
		{
			get => _gasPrn;
			set
			{
				ThrowIfAlreadyExecuted();
				_gasPrn = value;
			}
		}


		public IEnumerable<DeviceInfo> Devices { get; private set; } = new DeviceInfo[0];

		public PremiseConfigurator SetDualPrn(PremiseConfigurator other)
		{
			if (ElectricityPrn == null)
			{
				_electricityPrn = other.ElectricityPrn;
				Assert.IsNotNull(_electricityPrn);
			}
			else
			{
				_gasPrn = other.GasPrn;
				Assert.IsNotNull(_gasPrn);
			}


			Devices = Devices.Union(other.Devices);
			ConfigurePointOfDeliveryQueries();
			ConfigureInstallation();
			return this;
		}

		public PointOfDeliveryInfo PointOfDeliveryInfo { get; private set; }

		public void Execute()
		{
			//ThrowIfAlreadyExecuted();
			ConfigurePointOfDelivery();
			
			ConfigureDevices();
			ConfigurePremise();
			ConfigureInstallation();
			_executed = true;
		}
		
		private void ConfigurePremise()
		{
			PremiseInfo = Domain.ModelsBuilder.Build<Premise>()
				.With(x => x.PremiseId, PremiseId)
				.With(x => x.Installations, Domain.ModelsBuilder.Build<InstallationInfo>()
					.With(x => x.Devices, Devices)
					.With(x => x.DiscStatus, InstallationDiscStatusType.New)
					.With(x => x.DeregStatus, DeregStatusType.Registered)
					.With(x=>x.HasFirstStaffDiscount, _hasStaffDiscount)
					.With(x=>x.HasFreeElectricityAllowance, _hasFreeElectricityAllowance)
					.Create().ToOneItemArray())
				.Create();

			Domain.QueryResolver.ExpectQuery(new PremisesQuery
			{
				PremiseId = PremiseId
			}, PremiseInfo.ToOneItemArray());

			if(GasPrn != null)
			{
				Domain.QueryResolver.ExpectQuery(new PremisesQuery
				{
					Prn = GasPrn
				}, PremiseInfo.ToOneItemArray());
			}

			if (ElectricityPrn != null)
			{
				Domain.QueryResolver.ExpectQuery(new PremisesQuery
				{
					Prn = ElectricityPrn
				}, PremiseInfo.ToOneItemArray());
			}
		}

		private void ConfigurePointOfDelivery()
		{
			if (DuelFuelSisterAccount != null)
			{
				PointOfDeliveryInfo = DuelFuelSisterAccount.Premise.PointOfDeliveryInfo;
			}
			else
			{
				PointOfDeliveryInfo = Domain.ModelsBuilder.Build<PointOfDeliveryInfo>()
					.With(x => x.PremiseId, PremiseId)
					.Create();

				
			}

			ConfigurePointOfDeliveryQueries();
		}

		private void ConfigurePointOfDeliveryQueries()
		{
			var pointOfDeliveryInfos = PointOfDeliveryInfo.ToOneItemArray();

			if (ElectricityPrn != null)
			{
				Domain.QueryResolver.ExpectQuery(new PointOfDeliveryQuery
				{
					Prn = ElectricityPrn,
				}, pointOfDeliveryInfos);
			}

			if (GasPrn != null)
			{
				Domain.QueryResolver.ExpectQuery(new PointOfDeliveryQuery
				{
					Prn = GasPrn,
				}, pointOfDeliveryInfos);
			}

			if (NewDuelFuelAccountConfigurator != null)
			{
				Domain.QueryResolver.ExpectQuery(new PointOfDeliveryQuery
				{
					Prn = NewDuelFuelAccountConfigurator.Prn,
				}, pointOfDeliveryInfos);
			}
		}

		private readonly List<ConfigurableDeviceInfo> _devicesConfigurations = new List<ConfigurableDeviceInfo>();

		public PremiseConfigurator WithDevicesForConfiguration(
			ConfigurableDeviceSet configurationSetting)
		{
			this.WithDevicesForConfiguration(new ConfigurableDeviceInfo
			{
				DeviceType = configurationSetting,
				RegisterConfigType =null
			});
			return this;
		}
		public PremiseConfigurator WithDevicesForConfiguration(
			ConfigurableDeviceInfo configurationSetting)
		{
			this._devicesConfigurations.Add(configurationSetting);
			return this;
		}

		public PremiseConfigurator WithStaffDiscount(
			bool hasStaffDiscount)
		{
			this._hasStaffDiscount = hasStaffDiscount;
			return this;
		}

		public PremiseConfigurator WithFreeElectricityAllowance(bool hasFreeElectricityAllowance)
		{
			_hasFreeElectricityAllowance = hasFreeElectricityAllowance;
			return this;
		}

		private void ConfigureInstallation()
		{

			ConfigurePremises();

			ConfigureInstallations();

			void ConfigurePremises()
			{
				Devices = Devices.Union(NewDuelFuelAccountConfigurator?.Devices ?? new DeviceInfo[0]).ToArray();
				
			}

			void ConfigureInstallations()
			{
				if (DuelFuelSisterAccount != null)
				{

					InstallationInfo = DuelFuelSisterAccount.Premise.PremiseInfo.Installations.Single();
					InstallationInfo.Devices = InstallationInfo.Devices.Union(Devices)
						.Union(NewDuelFuelAccountConfigurator?.Devices ?? new DeviceInfo[0]).ToArray();

					
				}
				else
				{
					InstallationInfo = PremiseInfo.Installations.Single();
					InstallationInfo.Devices = InstallationInfo.Devices.Union(Devices).ToArray();


				}
			}
		}
		private void ConfigureDevices()
		{
			foreach (var devicesConfiguration in _devicesConfigurations)
			{

				switch (devicesConfiguration.DeviceType)
				{

					case ConfigurableDeviceSet.Gas:
						AddDevice(devicesConfiguration,MeterType.Gas);
						break;
					case ConfigurableDeviceSet.Electricity24H:
						AddDevice(devicesConfiguration, MeterType.Electricity24h);
						break;
					case ConfigurableDeviceSet.ElectricityDayAndNightMeters:
						AddDevice(devicesConfiguration,MeterType.ElectricityDay, MeterType.ElectricityNight);
						break;
					case ConfigurableDeviceSet.ElectricityNightStorageHeater:
						AddDevice(devicesConfiguration,MeterType.ElectricityNightStorageHeater);
						break;
					case ConfigurableDeviceSet.Electricity24HAndNightStorageHeater:
						AddDevice(devicesConfiguration,MeterType.ElectricityNightStorageHeater);
						AddDevice(devicesConfiguration,MeterType.Electricity24h);
						break;
					case ConfigurableDeviceSet.None:
						AddDevice(devicesConfiguration);
						break;
					default:
						throw new NotSupportedException();

				}
			}

			void AddDevice(ConfigurableDeviceInfo info, params MeterType[] meterTypes)
			{
				

				var devices = (Devices ?? new DeviceInfo[0]).ToList();
				if (meterTypes.Any())
				{
					devices = devices.Where(x => x.Registers.Any()).ToList();
				}

				devices.Add(Domain.ModelsBuilder.CreateDevice(_contractId,info.IsSmart, info.CTF,info.RegisterConfigType, meterTypes));
				Devices = devices;
			}

		}
		public void SetAsCurrentForAccount(string accountNumber)
		{
			Domain.QueryResolver.ExpectQuery(new DevicesQuery
			{
				AccountNumber =accountNumber
			}, Devices.ToArray());
		}

		public void SetInstallationsAsDeregistered()
		{
			foreach (var installation in PremiseInfo.Installations)
			{
				installation.DeregStatus = DeregStatusType.Deregistered;
			}
		}

        public void SetupForNewAcquisitionsNoDevicesAttached()
        {
            foreach (var installation in PremiseInfo.Installations)
            {
				installation.DiscStatus = InstallationDiscStatusType.New;
				installation.DeregStatus = DeregStatusType.Deregistered;
                installation.Devices = new DeviceInfo[0];
            }
        }

        public Premise PremiseInfo { get; private set; }
		public AddNewDuelFuelAccountConfigurator NewDuelFuelAccountConfigurator { get; set; }
		public CommonElectricityAndGasAccountConfigurator DuelFuelSisterAccount { get; set; }
		public InstallationInfo InstallationInfo { get; private set; }

		private bool _executed = false;
		private string _premiseId;
		private ElectricityPointReferenceNumber _electricityPrn;
		private GasPointReferenceNumber _gasPrn;
		private bool _hasStaffDiscount;
		private bool _hasFreeElectricityAllowance;

		private void ThrowIfAlreadyExecuted()
		{
			if (_executed)
			{
				throw new InvalidOperationException("configurator already Executed ");
			}
		}

		public PointReferenceNumber GetPrn(ClientAccountType accountType)
		{
			if (accountType == ClientAccountType.Electricity)
			{
				return ElectricityPrn;
			}
			if (accountType == ClientAccountType.Gas)
			{
				return GasPrn;
			}

			return null;
		}

		public DeviceInfo ElectricityDevice()
		{
			return Devices.SingleOrDefault(x => x.MeterType.IsElectricity());
		}
		public DeviceInfo GasDevice()
		{
			return Devices.SingleOrDefault(x => x.MeterType.IsGas());
		}
	}
}
