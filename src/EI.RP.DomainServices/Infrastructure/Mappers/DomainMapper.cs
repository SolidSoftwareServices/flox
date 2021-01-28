using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Platform;
using EI.RP.CoreServices.System.DependencyInjection;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Banking;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.DomainServices.Infrastructure.Settings;
using EI.RP.DomainServices.Queries.Metering.Premises;
using AddressInfoDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.AddressInfoDto;
using BankAccountDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.BankAccountDto;
using PremiseDto = EI.RP.DataModels.Sap.ErpUmc.Dtos.PremiseDto;

namespace EI.RP.DomainServices.Infrastructure.Mappers
{

	class DomainMapper :  IDomainMapper

		, IDomainMapper<BusinessAgreementDto, BusinessAgreement>
		, IDomainMapper<BusinessAgreementDto, BankAccountInfo>
		, IDomainMapper<AddressInfoDto, AddressInfo>
		, IDomainMapper<BankAccountDto, BankAccountInfo>
		, IDomainMapper<EI.RP.DataModels.Sap.ErpUmc.Dtos.AddressInfoDto, AddressInfo>
		, IDomainMapper<ContractItemDto, ContractItem>
		, IDomainMapper<PremiseDto, DataModels.Sap.CrmUmc.Dtos.PremiseDto, Premise>
		, IDomainMapper<InstallationDto, InstallationInfo>
		, IDomainMapper<DeviceDto, InstallationDto, DeviceInfo>
		, IDomainMapper<RegisterToReadDto, DeviceRegisterInfo>
		, IDomainMapper<PointOfDeliveryDto,  PointOfDeliveryInfo>
		, IDomainMapper<MeterReadingResultDto, MeterReadingInfo>
		, IDomainMapper<SmartActivationPlanDataModel,  SmartPlan>
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IServiceProvider _serviceProvider;
		private readonly IDomainSettings _domainSettings;
		public async Task<TDomainModel> Map<TDataModel, TDomainModel>(TDataModel dataModel) where TDomainModel : IQueryResult
		{
			var mapper = _serviceProvider.Resolve<IDomainMapper<TDataModel, TDomainModel>>();
			return await mapper.Map(dataModel);
		}

		public async Task<TDomainModel> Map<TDataModel1, TDataModel2, TDomainModel>(TDataModel1 dataModel1, TDataModel2 dataModel2) where TDomainModel : IQueryResult
		{
			var mapper = _serviceProvider.Resolve<IDomainMapper<TDataModel1,TDataModel2, TDomainModel>>();
			return await mapper.Map(dataModel1,dataModel2);
		}

		public DomainMapper(IDomainQueryResolver queryResolver, IServiceProvider serviceProvider, IDomainSettings domainSettings)
		{
			_queryResolver = queryResolver;
			_serviceProvider = serviceProvider;
			_domainSettings = domainSettings;
		}

		public async Task< MeterReadingInfo> Map(MeterReadingResultDto dataModel)
		{
			if (dataModel == null)
				return null;
			var result = new MeterReadingInfo();
			result.MeterReadingReasonID = dataModel.MeterReadingReasonID;
			result.ReadingDate = dataModel.ReadingDateTime;
			result.MeterType =(MeterType) dataModel?.Device?.RegistersToRead
				?.FirstOrDefault(x => x.RegisterID == dataModel.RegisterID)?.RegisterType.Description;
			result.MeterReadingReason = (MeterReadingReason)dataModel.MeterReadingReasonID;
			result.MeterReadingStatus = (MeterReadingStatus)dataModel.MeterReadingStatusID;
			result.Reading = Convert.ToDecimal(dataModel.ReadingResult);
			result.SerialNumber = dataModel.SerialNumber;
			result.RegisterId = dataModel.RegisterID;
			result.IsPendingReadingVerification = dataModel.MeterReadingStatusID == MeterReadingStatus.AutomaticallyLocked ||
			                                      dataModel.MeterReadingStatusID == MeterReadingStatus.LockedByAgent;
			result.IsEstimate = ((MeterReadingCategoryType)dataModel.MeterReadingCategoryID).IsEstimation;
			result.MeterReadingCategory = MeterReadingCategoryType.From(dataModel.MeterReadingCategoryID);
			result.ReadingUnit = (MeterUnit)dataModel.ReadingUnit;
			result.Consumption = dataModel.ConsumptionAsDecimal() ?? 0M;
			result.MeasurementUnitForConsumption = dataModel.ReadingUnit;
			result.ReadingDateTime = dataModel.ReadingDateTime.Value;
			result.Lcpe = dataModel.Lcpe;
			result.Vertrag = dataModel.Vertrag;
			result.Vkont = dataModel.Vkont;
			return result;
		}
		public async Task<InstallationInfo> Map(InstallationDto dataModel)
		{
			if(dataModel == null)
			{
				return null;
			}
			var validTo = SapDateTimes.SapDateTimeMax;
			return new InstallationInfo()
			{
				InstallationId = dataModel.InstallationID,
				Devices = await Task.WhenAll(dataModel.Devices.Where(x => x != null).Select(y => Map(y, dataModel)).ToArray()),
				DiscStatus = (InstallationDiscStatusType)dataModel.DiscStatus,
				DeregStatus = (DeregStatusType)dataModel.DeregStatus,
				HasFreeElectricityAllowance = dataModel.InstallationFacts?.Any(x => x.Operand == OperandType.FreeElectricityAllowance),
				HasFirstStaffDiscount = dataModel.InstallationFacts?.Any(x => x.Operand == OperandType.FirstStaffDiscount) ?? false,
				ElectricityDuosGroup = dataModel.InstallationFacts?.SingleOrDefault(x => x.Operand == OperandType.DuosGroup && x.ValidTo == validTo)?.OperandValue,
				ElectricityRegisterConfiguration=(RegisterConfigType)dataModel.InstallationFacts?.SingleOrDefault(x => x.Operand == OperandType.SmartMeterConfiguration && x.ValidTo==validTo)?.OperandValue,
				ElectricityCtfValue = (CommsTechnicallyFeasibleValue)dataModel.InstallationFacts?.SingleOrDefault(x => x.Operand == OperandType.CTFValue && x.ValidTo == validTo)?.OperandValue,
				GasBand = dataModel.InstallationFacts?.SingleOrDefault(x => x.Operand == OperandType.Band && x.ValidTo == validTo)?.OperandValue,
			};
		}

		public async Task<PointOfDeliveryInfo> Map(PointOfDeliveryDto src)
		{
			if (src == null) return null;
			return new PointOfDeliveryInfo
			{
				Prn = src.ExternalID,
				PointOfDeliveryId = src.PointOfDeliveryID,
				PremiseId = src.PremiseID,
				AddressInfo = await Map(src.Premise.AddressInfo),
				IsNewAcquisition = false
			};
		}

		public async Task<BusinessAgreement> Map(BusinessAgreementDto dataModel)
		{
			var addressTask = Map(dataModel.BillToAccountAddress.AddressInfo);
			var contractsTask = Task.WhenAll(dataModel.ContractItems.Where(x => x != null).Select(Map));

			var businessAgreement = new BusinessAgreement();
			businessAgreement.IncomingPaymentMethodType = (PaymentMethodType)dataModel.IncomingPaymentMethodID;
			businessAgreement.CollectiveParentId = dataModel.CollectiveParentID;
			businessAgreement.AlternativePayerId = dataModel.AlternativePayerID;
			businessAgreement.AlternativePayerCA = dataModel.AlternativePayerCA;
			businessAgreement.BusinessAgreementId = dataModel.BusinessAgreementID;
			businessAgreement.Language = dataModel.Language;

			
			
			businessAgreement.Description = dataModel.Description;
			businessAgreement.AccountCategory = dataModel.AccountCategory;
			businessAgreement.AccountDeterminationID = dataModel.AccountDeterminationID;
			businessAgreement.FixedBillingDateDay = !string.IsNullOrWhiteSpace(dataModel.FixedBillingDate)?int.Parse(dataModel.FixedBillingDate):(int?)null;
			businessAgreement.IsEBiller = dataModel.EBiller == SapBooleanFlag.Yes;

			businessAgreement.BillToAccountAddress = await addressTask;
			businessAgreement.Contracts = (await contractsTask).ToArray();

			return businessAgreement;
		}

		public async Task<ContractItem> Map(ContractItemDto contractItemDto)
		{
			var getPremise = _queryResolver.GetPremise(contractItemDto.PremiseID, true);

			var map = new ContractItem();
			map.Description = contractItemDto.Description;
			map.Division = (DivisionType) contractItemDto.DivisionID;
			map.ProductType = (ProductType)contractItemDto.ProductID;
			map.AccountID = contractItemDto.AccountID;
			map.ContractStartDate = contractItemDto.ContractStartDate;
			map.ContractEndDate= contractItemDto.ContractEndDate;
			map.PointOfDeliveryGuid = contractItemDto.PointOfDeliveryGUID;
			map.IsBilledMonthly = contractItemDto.Attributes.Any(x => x.AttributeID == "MONTHLYBILL_FLAG" && x.Value == SapBooleanFlag.Yes) ;
			map.ContractID = contractItemDto.ContractID;
			map.BusinessAgreementID = contractItemDto.BusinessAgreementID;
			map.ContractStatus = contractItemDto.ContractItemEXTAttrs !=null ? (ContractStatusType)contractItemDto.ContractItemEXTAttrs.USER_STAT_DESC_SHORT : ContractStatusType.Unknown;
			map.Premise = await getPremise;

			return map;
		}

		public async Task<AddressInfo> Map(AddressInfoDto dataModel)
		{
			var map = new AddressInfo();
			map.PostalCode = dataModel.PostalCode;
			map.Street = dataModel.Street;
			map.CareOf = dataModel.CareOf;
			map.City = dataModel.City;
			map.HouseNo = dataModel.HouseNo;
			map.Region = dataModel.Region;
			map.ShortForm = dataModel.ShortForm;
			map.Country = dataModel.CountryName;
			map.District = dataModel.District;
			map.POBoxPostalCode = dataModel.POBoxPostalCode;
			map.POBox = dataModel.POBox;
			map.Building = dataModel.Building;
			map.Floor = dataModel.Floor;
			map.RoomNo = dataModel.RoomNo;
			map.CountryID = dataModel.CountryID;
			map.CountryName = dataModel.CountryName;
			map.RegionName = dataModel.RegionName;
			map.TimeZone = dataModel.TimeZone;
			map.TaxJurisdictionCode = dataModel.TaxJurisdictionCode;
			map.LanguageID = dataModel.LanguageID;
			map.HouseSupplement = dataModel.HouseSupplement;
			map.AddressLine1 = dataModel.AddressLine1;
			map.AddressLine2 = dataModel.AddressLine2;
			map.AddressLine4 = dataModel.AddressLine4;
			map.AddressLine5 = dataModel.AddressLine5;
			return map;
		}

		public async Task<AddressInfo> Map(EI.RP.DataModels.Sap.ErpUmc.Dtos.AddressInfoDto dataModel)
		{
			var map = new AddressInfo();
			map.PostalCode = dataModel.PostalCode;
			map.Street = dataModel.Street;
			map.City = dataModel.City;
			map.HouseNo = dataModel.HouseNo;
			map.Region = dataModel.Region;
			map.ShortForm = dataModel.ShortForm;
			map.Country = dataModel.CountryName;
			map.District = dataModel.District;
			map.POBoxPostalCode = dataModel.POBoxPostalCode;
			map.POBox = dataModel.POBox;
			map.Building = dataModel.Building;
			map.Floor = dataModel.Floor;
			map.RoomNo = dataModel.RoomNo;
			map.CountryID = dataModel.CountryID;
			map.CountryName = dataModel.CountryName;
			map.RegionName = dataModel.RegionName;
			map.TimeZone = dataModel.TimeZone;
			map.TaxJurisdictionCode = dataModel.TaxJurisdictionCode;
			map.LanguageID = dataModel.LanguageID;
			return map;
		}

		public async Task<DeviceInfo> Map(DeviceDto deviceDto, InstallationDto installationDto)
		{
			if (deviceDto == null)
			{
				return null;
			}

			var firstRegister = deviceDto.RegistersToRead.ToArray().FirstOrDefault();
			var result = new DeviceInfo();
			result.ContractId = deviceDto.ContractID;
			result.DeviceId = deviceDto.DeviceID;
			result.DivisionId = deviceDto.DivisionID;
			result.DeviceMaterial = deviceDto.DeviceMaterial;
			result.DeviceLocation = deviceDto.DeviceLocation;
			result.SerialNum = deviceDto.SerialNumber;
			result.DeviceDescription = deviceDto.DeviceDescription;
			result.IsSmart = deviceDto.FunctionClass == "1006";

			result.MeterReadingResults = await Task.WhenAll(deviceDto.MeterReadingResults.Where(x => x != null)
				.Select(Map).ToArray());
			result.Registers = await Task.WhenAll(deviceDto.RegistersToRead.Select(Map).Where(x => x != null)
				.ToArray());
			result.MeterType = (MeterType) firstRegister?.RegisterType.Description;
			result.MeterUnit = firstRegister?.ReadingUnit;
			result.PremiseId = installationDto?.PremiseID;

			await SetSmartInfo();
			

			return result;

			async Task SetSmartInfo()
			{
				if (result.DivisionId != DivisionType.Electricity)
				{
					return;
				}

				
				result.MCCConfiguration = (RegisterConfigType)installationDto.InstallationFacts
					.SingleOrDefault(x =>
						x.Operand == OperandType.SmartMeterConfiguration &&
						x.ValidTo == SapDateTimes.SapDateTimeMax)
					?.OperandValue;
				if (result.MCCConfiguration == RegisterConfigType.MCC12)
				{
					result.IsSmart = true;
				}

				result.CTF = (CommsTechnicallyFeasibleValue)installationDto.InstallationFacts
					?.SingleOrDefault(
						x => x.Operand == OperandType.CTFValue && x.ValidTo == SapDateTimes.SapDateTimeMax)
					?.OperandValue;

				ResolveSmartActivationStatus();

				void ResolveSmartActivationStatus()
				{
					result.SmartActivationStatus = SmartActivationStatus.SmartNotAvailable;
					if (_domainSettings.IsSmartActivationEnabled 
					    && result.IsSmart 
					    && result.DivisionId == DivisionType.Electricity
					    && result.MCCConfiguration != null)
					{
						if (result.MCCConfiguration.IsSmartConfigurationActive())
						{
							result.SmartActivationStatus = SmartActivationStatus.SmartActive;
						}
						else if ( result.MCCConfiguration.CanOptToSmartActive())
						{
							result.SmartActivationStatus = result.CTF.AllowsAllSmartFeatures() ? SmartActivationStatus.SmartAndEligible : SmartActivationStatus.SmartButNotEligible;
						}
					}
				}
			}
		}

		public async Task<DeviceRegisterInfo> Map(RegisterToReadDto source)
		{
			if (source==null|| !MeterType.AllValues.Any(x =>
				x.ToString().Equals(source.RegisterType.Description, StringComparison.InvariantCultureIgnoreCase)))
				return null;
			var result = new DeviceRegisterInfo();
			result.DeviceId = source.DeviceID;
			result.MeterType = (MeterType)source.RegisterType.Description;
			result.MeterUnit = (MeterUnit)source.ReadingUnit;
			result.MeterNumber = source.SerialNumber;
			result.RegisterId = (MeterReadingRegisterType)source.RegisterID;
			return result;
		}

		public async Task<Premise> Map(PremiseDto erp, DataModels.Sap.CrmUmc.Dtos.PremiseDto crm)
		{
				if (erp == null && crm == null) return null;

				var result = new Premise();
				if (erp != null)
				{
					result.PremiseId = erp.PremiseID;
					result.Address = await Map(erp.AddressInfo);
					result.Installations = await Task.WhenAll(erp.Installations.Select(Map));
					result.ElectricityDuosGroup= result.Installations
						.SingleOrDefault(x => !string.IsNullOrWhiteSpace(x.ElectricityDuosGroup))
						?.ElectricityDuosGroup;
					var registerConfiguration = (RegisterConfigType) result.Installations
						.SingleOrDefault(x => !string.IsNullOrWhiteSpace(x.ElectricityRegisterConfiguration))
						?.ElectricityRegisterConfiguration;
					result.RegisterConfiguration=registerConfiguration;
					result.ElectricityCTF = registerConfiguration!=null && registerConfiguration.IsSmartElectricity()
						? result.Installations
							.SingleOrDefault(x => !string.IsNullOrWhiteSpace(x.ElectricityCtfValue))
							?.ElectricityCtfValue
						: string.Empty;
					result.ElectricityCTFOperandValue = result.Installations
								.SingleOrDefault(x => !string.IsNullOrWhiteSpace(x.ElectricityCtfValue))
								?.ElectricityCtfValue;
				result.GasBand = result.Installations
						.SingleOrDefault(x => !string.IsNullOrWhiteSpace(x.GasBand))
						?.GasBand;
				};
				if (crm != null)
				{
					result.PointOfDeliveries = await Task.WhenAll(crm.PointOfDeliveries.Select(Map));
				}

				return result;
		}

		public async Task<BankAccountInfo> Map(BankAccountDto dataModel)
		{
			
			var result = new BankAccountInfo
			{
				BankAccountId = dataModel.BankAccountID,
				IBAN = dataModel.IBAN,
				BIC = dataModel.BankID,
				NameInBankAccount = dataModel.AccountHolder
			};

			return result;
		}

	

		async Task<BankAccountInfo> IDomainMapper<BusinessAgreementDto, BankAccountInfo>.Map(BusinessAgreementDto dataModel)
		{
			var bankAccount = dataModel.IncomingBankAccount;
			var result = new BankAccountInfo();
			result.PaymentMethod = !string.IsNullOrWhiteSpace(dataModel.AlternativePayerCA)
				? PaymentMethodType.DirectDebitNotAvailable
				: (PaymentMethodType)dataModel.IncomingPaymentMethodID;
			result.AccountNumber = dataModel.BusinessAgreementID;

			if (bankAccount != null)
			{
				result.BankAccountId = bankAccount.BankAccountID;
				if (result.PaymentMethod != PaymentMethodType.Manual && result.PaymentMethod != PaymentMethodType.DirectDebitNotAvailable)
				{
					result.BankAccountId = bankAccount.BankAccountID;
					result.IBAN = bankAccount.IBAN;
					result.BIC = bankAccount.BankID;
					result.NameInBankAccount = bankAccount.AccountHolder;
				}
			}

			return result;
		}

		public Task<SmartPlan> Map(SmartActivationPlanDataModel dataModel)
		{
			var lst = new List<PlanPrice>();
			if(dataModel.PriceDay!=null) lst.Add(new PlanPrice{Type=SmartPlanPriceType.ElectricityDay,Value=dataModel.PriceDay});
			if(dataModel.PriceElectricity24H!=null) lst.Add(new PlanPrice{Type=SmartPlanPriceType.Electricity24H,Value=dataModel.PriceElectricity24H});
			if(dataModel.PriceNight!=null) lst.Add(new PlanPrice{Type=SmartPlanPriceType.ElectricityNight,Value=dataModel.PriceNight});
			if (dataModel.PriceBoost != null) lst.Add(new PlanPrice { Type = SmartPlanPriceType.ElectricityBoost, Value = dataModel.PriceBoost });
			if (dataModel.PricePeak!=null) lst.Add(new PlanPrice{Type=SmartPlanPriceType.ElectricityPeak,Value=dataModel.PricePeak});
			if (dataModel.PriceGas24H != null) lst.Add(new PlanPrice { Type = SmartPlanPriceType.Gas24H, Value = dataModel.PriceGas24H });
			var result = new SmartPlan
			{
				ID = dataModel.ID,
				IsActive = dataModel.IsAvailable,
				PlanType = (SmartPlanGroup)dataModel.GroupName,
				BonusDescription = dataModel.BonusDescription,
				EABDescription = dataModel.EABDescription,
				EstimatedAnnualBill = dataModel.EstimatedAnnualBill,
				OrderIndex = dataModel.OrderIndex,
				PlanFeatures = dataModel.PlanFeatures.ToArray(),
				PlanName = dataModel.PlanName,
				FirstYearCostPerKwh=dataModel.FirstYearCostPerKwh,
				FreeDayOfElectricityChoice = dataModel.FreeDayOfElectricityChoice?.Select(x=>x.ToEnum<DayOfWeek>()).ToArray(),
				FreeDayOfElectricityDescription=dataModel.FreeDayOfElectricityDescription,
				PlanPrices=lst,
				GeneralTermsAndConditionsUrl = dataModel.GeneralTermsAndConditionsUrl,
				PricePlanTermsAndConditionsUrl = dataModel.PricePlanTermsAndConditionsUrl,
				ValidProductProposals = dataModel.ValidProductProposals,
				FullPricingInformation = dataModel.FullPricingInformation
			};

			return Task.FromResult(result);
		}
	}
}