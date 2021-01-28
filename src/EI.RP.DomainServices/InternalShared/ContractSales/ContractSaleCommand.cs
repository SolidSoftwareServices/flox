using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Banking;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.DomainServices.Infrastructure.Mappers;
using EI.RP.DomainServices.InternalShared.Products;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.Queries.Register;
using BankAccountDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.BankAccountDto;
using PremiseDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.PremiseDto;
using AccountDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.AccountDto;
using EI.RP.DomainServices.Queries.MovingHouse.IsPrnNewAcquisition;
using EI.RP.DomainServices.Queries.Contracts.Contract;
using EI.RP.DomainServices.InternalShared.PointOfDelivery;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;

namespace EI.RP.DomainServices.InternalShared.ContractSales
{
	class ContractSaleCommand:IContractSaleCommand
	{
		private readonly IProductProposalResolver _productProposalResolver;
		private readonly ISapRepositoryOfErpUmc _erpUmc;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IDomainCommandDispatcher _commandDispatcher;
		private readonly IDomainMapper _domainMapper;
		private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;
		private readonly IPointOfDeliveryCommand _pointOfDeliveryBuilder;

		public ContractSaleCommand( IDomainQueryResolver queryResolver, IProductProposalResolver productProposalResolver,
			ISapRepositoryOfCrmUmc crmUmcRepository, ISapRepositoryOfErpUmc erpUmc, IDomainCommandDispatcher commandDispatcher,IDomainMapper domainMapper,
			IPointOfDeliveryCommand pointOfDeliveryBuilder)
		{
			_queryResolver = queryResolver;
			_productProposalResolver = productProposalResolver;
			_crmUmcRepository = crmUmcRepository;
			_erpUmc = erpUmc;
			_commandDispatcher = commandDispatcher;
			_domainMapper = domainMapper;
			_pointOfDeliveryBuilder = pointOfDeliveryBuilder;
		}
		private async Task<PointOfDeliveryInfo> GetOrAddPointOfDelivery(PointReferenceNumber prn, string addAddresFromPremiseIWhenMissing)
		{
			var pointOfDelivery =
				await _queryResolver.GetPointOfDeliveryInfoByPrn(prn, true);

			if (pointOfDelivery == null && !string.IsNullOrEmpty(addAddresFromPremiseIWhenMissing))
			{
				pointOfDelivery = await _pointOfDeliveryBuilder.AddPointOfDelivery(prn,
														ResolveAccountType(),
														usePremisesAddressFromPremiseId: addAddresFromPremiseIWhenMissing);
			}

            return pointOfDelivery;

            ClientAccountType ResolveAccountType()
            {
	            if (prn.Type == PointReferenceNumberType.Gprn)
		            return ClientAccountType.Gas;
	            if (prn.Type == PointReferenceNumberType.Mprn)
		            return ClientAccountType.Electricity;
	            throw new NotSupportedException();
            }
		}
		public async Task<ContractSaleDto> ExecuteContractSale(MoveHouse commandData)
		{
			return await _Execute(commandData, false);
		}
		public async Task<ContractSaleDto> ResolveContractSaleChecks(MoveHouse command, bool autobatch = true)
		{
			return await _Execute(command, true, autobatch);
		}
		private async Task<ContractSaleDto> _Execute(MoveHouse command, bool checkOnly, bool autobatch = true)
        {
            var getProductProposal = _productProposalResolver.GetMoveHouseProductProposal(command);
            var sale = await PrepareSaleDetails(command, checkOnly);
            sale.ProductProposalResult = await getProductProposal;

            sale = await _crmUmcRepository.AddThenGet(sale, autobatch);
			return sale;
        }

        private async Task<ContractSaleDto> PrepareSaleDetails(MoveHouse command, bool checkOnly)
        {
            var context = command.Context;
            var contractAccount = context.AllAccounts().First(x => x.IsContractAccount);
            var sale = new ContractSaleDto
            {
                SalesOrderID = string.Empty,
                ConsumerID = SapConsumerId.MoveHomeConsumer,
                AccountID = contractAccount.Account.Partner,
                CheckModeOnly = checkOnly,
                ContractStartDate = command.Context.MoveInDetails.MovingInDate.Value,
                MoveOutDate = command.Context.MoveOutDetails.MovingOutDate
            };

            var electricitySale = await BuildElectricitySaleDetail();
            if (electricitySale != null)
            {
                sale.SaleDetails.Add(electricitySale);
            }

            var gasSale = await BuildGasSaleDetail();
            if (gasSale != null)
            {
                sale.SaleDetails.Add(gasSale);
            }

            return sale;

            async Task<ContractSaleDetailDto> BuildElectricitySaleDetail()
            {
                var saleDetail = await InitializeElectricityDetail();
                saleDetail = await AddMoveOutItems(saleDetail);
                saleDetail = await AddMoveInItems(saleDetail);

                if ((context.Electricity != null || context.HasNewMprn()) && !checkOnly)
                {
					saleDetail = await SetUpPaymentDetails(command.Context.AllAccounts().First().Account.Partner,
														   saleDetail,
														   command.CommandsToExecute.SingleOrDefault(x => x.AccountType == ClientAccountType.Electricity));
				}
                return saleDetail;

                async Task<ContractSaleDetailDto> InitializeElectricityDetail()
                {
                    if (context.Electricity == null && !context.HasNewMprn()) return null;

                    var result = new ContractSaleDetailDto
                    {
                        DivisionID = DivisionType.Electricity,
                        PointOfDeliveryGUID = context.NewPointsOfDelivery.ContainsKey(ClientAccountType.Electricity) ? context.NewPointsOfDelivery[ClientAccountType.Electricity].PointOfDeliveryId : string.Empty
					};

                    if (context.Electricity == null || context.Electricity.Account == null) return result;

                    result.ContractID = context.Electricity.Account.ContractId;
                    result.BusinessAgreementID = context.Electricity.Account.BusinessAgreement?.BusinessAgreementId;
                    return result;
                }

                async Task<ContractSaleDetailDto> AddMoveOutItems(ContractSaleDetailDto s)
                {
                    ContractSaleDetailDto result = s;
                    if (context.Electricity != null)
                    {
                        var getDevices = GetDevices(context.Electricity.Account);
                        foreach (var device in await getDevices)
                        {
                            var registerReads = device.RegistersToRead.ToArray();

                            foreach (var registerRead in registerReads.Where(x => ((MeterType)x.RegisterType.Description)
                                .IsOneOf(MeterType.Electricity24h, MeterType.ElectricityDay, MeterType.ElectricityNight, MeterType.ElectricityNightStorageHeater)))
                            {
                                result.MeterReadings.Add(new ContractSaleMeterReadingDto
                                {
                                    DivisionID = device.DivisionID,
                                    ContractID = context.Electricity.Account.ContractId,
                                    DeviceID = device.DeviceID,
                                    SerialNumber = device.SerialNumber,
                                    RegisterID = registerRead.RegisterID,
                                    ReadingDateTime = context.MoveOutDetails.MovingOutDate,
                                    ReadingResult = (registerRead.RegisterType.Description == MeterType.Electricity24h ||
                                                     registerRead.RegisterType.Description == MeterType.ElectricityDay
                                        ? context.MoveOutDetails.ElectricityMeterReadingDayOr24HrsValue
                                        : context.MoveOutDetails.ElectricityMeterReadingNightOrNshValue).ToString(),
                                    MeterReadingReasonID = MeterReadingReason.MeterReadingAtMoveOut,
                                    MeterReadingCategoryID = MeterReadingCategoryType.Customer,
                                });
                            }
                        }
                    }

                    return result;
                }

                async Task<ContractSaleDetailDto> AddMoveInItems(ContractSaleDetailDto s)
                {
                    ContractSaleDetailDto result = s;
                    if (context.HasNewMprn())
                    {
						var addAddresFromPremiseIdWhenMissing = context.NewPointsOfDelivery.ContainsKey(ClientAccountType.Gas) ?
							 context.NewPointsOfDelivery[ClientAccountType.Gas].PremiseId : string.Empty;

						var pointOfDelivery = await GetOrAddPointOfDelivery(context.NewPrns.NewMprn, addAddresFromPremiseIdWhenMissing);
                        result.PointOfDeliveryGUID = pointOfDelivery?.PointOfDeliveryId ?? string.Empty;
                        var premise = await _queryResolver.GetPremiseByPrn(context.NewPrns.NewMprn, true);
                        var devices = premise.Installations.FirstOrDefault()?.Devices ?? new DeviceInfo[0];
                        foreach (var device in devices)
                        {
                            var registerReads = device.Registers.ToArray();

                            foreach (var registerRead in registerReads.Where(x => ((MeterType)x.MeterType)
                                .IsOneOf(MeterType.Electricity24h, MeterType.ElectricityDay, MeterType.ElectricityNight, MeterType.ElectricityNightStorageHeater)))
                            {
                                var getNewAcquisitionRegister = _queryResolver.GetRegisterInfoByPrn(context.NewPrns.NewMprn);
                                result.MeterReadings.Add(new ContractSaleMeterReadingDto
                                {
                                    DivisionID = device.DivisionId,
                                    ContractID = string.Empty,
                                    DeviceID = device.DeviceId,
                                    SerialNumber = !pointOfDelivery.IsNewAcquisition
                                        ? device.SerialNum
                                        : (await getNewAcquisitionRegister).First().RegisterId,
                                    RegisterID = !pointOfDelivery.IsNewAcquisition ? registerRead.RegisterId : (await getNewAcquisitionRegister).First().RegisterId,
                                    ReadingDateTime = context.MoveInDetails.MovingInDate.Value,
                                    ReadingResult = (registerRead.MeterType.IsOneOf(MeterType.Electricity24h, MeterType.ElectricityDay)
                                        ? context.MoveInDetails.ElectricityMeterReadingDayOr24HrsValue
                                        : context.MoveInDetails.ElectricityMeterReadingNightOrNshValue).ToString(),
                                    MeterReadingReasonID = MeterReadingReason.MeterReadingAtMoveIn,
                                    MeterReadingCategoryID = MeterReadingCategoryType.Customer,
                                    TimeSlot = pointOfDelivery.IsNewAcquisition ? (await getNewAcquisitionRegister).First().TimeofUsePeriod : null

                                });
                            }
                        }
                    }

                    return result;
                }
            }
            async Task<ContractSaleDetailDto> BuildGasSaleDetail()
            {
                var saleDetail = await InitializeGasDetail();
                saleDetail = await AddMoveOutItems(saleDetail);
                saleDetail = await AddMoveInItems(saleDetail);
                if ((context.Gas != null || context.HasNewGprn()) && !checkOnly)
                {
					saleDetail = await SetUpPaymentDetails(command.Context.AllAccounts().First().Account.Partner,
														   saleDetail,
														   command.CommandsToExecute.SingleOrDefault(x => x.AccountType == ClientAccountType.Gas));
				}
                return saleDetail;

                async Task<ContractSaleDetailDto> InitializeGasDetail()
                {
                    if (context.Gas == null && !context.HasNewGprn()) return null;

                    var result = new ContractSaleDetailDto
                    {
                        DivisionID = DivisionType.Gas,
                        PointOfDeliveryGUID = context.NewPointsOfDelivery.ContainsKey(ClientAccountType.Gas) ? context.NewPointsOfDelivery[ClientAccountType.Gas].PointOfDeliveryId : string.Empty
					};

                    if (context.Gas == null || context.Gas.Account == null) return result;

                    result.ContractID = context.Gas.Account.ContractId;
                    result.BusinessAgreementID = context.Gas.Account.BusinessAgreement?.BusinessAgreementId;
                    return result;
                }

                async Task<ContractSaleDetailDto> AddMoveOutItems(ContractSaleDetailDto s)
                {
                    ContractSaleDetailDto result = s;
                    if (context.Gas != null)
                    {
                        var getDevices = GetDevices(context.Gas.Account);
                        foreach (var device in await getDevices)
                        {
                            var registerReads = device.RegistersToRead.ToArray();

                            foreach (var registerRead in registerReads.Where(x => (MeterReadingRegisterType)x.RegisterID == MeterReadingRegisterType.ActiveEnergyRegisterType))
                            {
                                result.MeterReadings.Add(new ContractSaleMeterReadingDto
                                {
                                    DivisionID = device.DivisionID,
                                    ContractID = context.Gas.Account.ContractId,
                                    DeviceID = device.DeviceID,
                                    SerialNumber = device.SerialNumber,
                                    RegisterID = registerRead.RegisterID,
                                    ReadingDateTime = context.MoveOutDetails.MovingOutDate,
                                    ReadingResult = context.MoveOutDetails.GasMeterReadingValue.ToString(),
                                    MeterReadingReasonID = MeterReadingReason.MeterReadingAtMoveOut,
                                    MeterReadingCategoryID = MeterReadingCategoryType.Customer,
                                });
                            }
                        }
                    }

                    return result;
                }

                async Task<ContractSaleDetailDto> AddMoveInItems(ContractSaleDetailDto s)
                {
                    var result = s;
					if (context.HasNewGprn())
                    {
						var addAddresFromPremiseIdWhenMissing = context.NewPointsOfDelivery.ContainsKey(ClientAccountType.Electricity) ?
							 context.NewPointsOfDelivery[ClientAccountType.Electricity].PremiseId : string.Empty;

						var pointOfDelivery = await GetOrAddPointOfDelivery(context.NewPrns.NewGprn, addAddresFromPremiseIdWhenMissing);
                        result.PointOfDeliveryGUID = pointOfDelivery?.PointOfDeliveryId ?? string.Empty;

                        var premise = await _queryResolver.GetPremiseByPrn(context.NewPrns.NewGprn, true);
                        var devices = premise.Installations.FirstOrDefault()?.Devices.ToArray() ?? new DeviceInfo[0];

						if (devices.Any())
						{
							foreach (var device in devices)
							{
								var registerReads = device.Registers.ToArray();

								foreach (var registerRead in registerReads.Where(x => x.RegisterId == MeterReadingRegisterType.ActiveEnergyRegisterType))
								{
									result.MeterReadings.Add(new ContractSaleMeterReadingDto
									{
										DivisionID = device.DivisionId,
										ContractID = string.Empty,
										DeviceID = device.DeviceId,
										SerialNumber = !pointOfDelivery.IsNewAcquisition
											? device.SerialNum
											: "DEVICE",
										RegisterID = MeterReadingRegisterType.ActiveEnergyRegisterType,
										ReadingDateTime = context.MoveInDetails.MovingInDate.Value,
										ReadingResult = context.MoveInDetails.GasMeterReadingValue.ToString(),
										MeterReadingReasonID = MeterReadingReason.MeterReadingAtMoveIn,
										MeterReadingCategoryID = MeterReadingCategoryType.Customer,
									});
								}
							}
						}
						else
						{
							result.MeterReadings.Add(new ContractSaleMeterReadingDto
							{
								DivisionID = DivisionType.Gas,
								ContractID = string.Empty,
								DeviceID = null,
								SerialNumber = "DEVICE",
								RegisterID = MeterReadingRegisterType.ActiveEnergyRegisterType,
								ReadingDateTime = context.MoveInDetails.MovingInDate.Value,
								ReadingResult = context.MoveInDetails.GasMeterReadingValue.ToString(),
								MeterReadingReasonID = MeterReadingReason.MeterReadingAtMoveIn,
								MeterReadingCategoryID = MeterReadingCategoryType.Customer,
							});
						}
                    }

                    return result;
                }
            }
        }

		private async Task<ContractSaleDetailDto> SetUpPaymentDetails(string partner,
															  ContractSaleDetailDto saleDetail,
															  SetUpDirectDebitDomainCommand payment)
		{
			if (payment != null)
			{
				saleDetail.IncomingPaymentMethodID = payment.PaymentMethodType;

				if (payment.PaymentMethodType == PaymentMethodType.DirectDebit)
				{
					var bankAccount = await EnsureBankAccountExists(payment.NewIBAN,
																	payment.NameOnBankAccount,
																	partner);

					saleDetail.BankAccountID = bankAccount.BankAccountId;
				}

				saleDetail.SEPAFlag = payment.PaymentMethodType == PaymentMethodType.DirectDebit;
			}

			return saleDetail;
		}

		private async Task<BankAccountInfo> EnsureBankAccountExists(string iban, 
																	string nameOnBankAccount, 
																	string accountId)
		{
			var query = _crmUmcRepository.NewQuery<AccountDto>()
										 .Key(accountId)
										 .Expand(x => x.BankAccounts);

			var partnerWithBankAccounts = await _crmUmcRepository.GetOne(query);

			var bankAccount = partnerWithBankAccounts.BankAccounts
													 .SingleOrDefault(x => x.IBAN == iban.ToUpperInvariant());

			var result = bankAccount!=null ? await _domainMapper.Map<BankAccountDto, BankAccountInfo>(bankAccount) : null;

			if (result == null)
			{
				result = await CreateNewBankAccount(iban,
										nameOnBankAccount,
										accountId);
			}
			return result;
		}

		private async Task<BankAccountInfo> CreateNewBankAccount(string iban, string nameOnBankAccount, string accountID)
		{
			var newBankAccount = new BankAccountDto
			{
				BankAccountID = string.Empty,
				Bank = null,
				BankAccountNo = string.Empty,
				BankID = string.Empty,
				CollectionAuth = string.Empty,
				ControlKey = string.Empty,
				IBAN = iban,
				BankAccountName = nameOnBankAccount,
				AccountHolder = nameOnBankAccount,
				CountryID = CountryIdType.IE,
				AccountID = accountID
			};

			newBankAccount = await _crmUmcRepository.AddThenGet(newBankAccount);
			var result = await _domainMapper.Map<BankAccountDto, BankAccountInfo>(newBankAccount);

			return result;
		}

		async Task<IEnumerable<DeviceDto>> GetDevices(AccountInfo account)
		{
			return await _erpUmc.NewQuery<ContractDto>()
				.Key(account.ContractId)
				.NavigateTo<DeviceDto>(x => x.Devices)
				.Expand(x => x.RegistersToRead)
				.Expand(x => x.RegistersToRead[0].RegisterType)
				.Expand(x => x.MeterReadingResults)
				.GetMany();
		}


		public  async Task<ContractSaleDto> ExecuteAddGasContractSale(string electricityAccountNumber,
			PaymentSetUpType paymentSetup, GasPointReferenceNumber prn, decimal meterReading,
			Premise electricityPremise,
			string iban, string nameOnBankAccount, PointOfDeliveryInfo pointOfDelivery)
		{
			var getElectricityContract = _crmUmcRepository.NewQuery<BusinessAgreementDto>()
				.Key(electricityAccountNumber)
				.NavigateTo<ContractItemDto>()
				.Expand(x => x.Premise.PointOfDeliveries)
				.Expand(x => x.BusinessAgreement)
				.GetOne();

			var isNewAcquisition = await _queryResolver.IsPrnNewAcquisition(prn, pointOfDelivery.IsNewAcquisition);
			var account = await _queryResolver.GetAccountInfoByAccountNumber(electricityAccountNumber);
			var gasPremise = await _queryResolver.GetPremiseByPrn(prn);
			var getProductProposal = _productProposalResolver.GetAddGasProductProposal(electricityAccountNumber, paymentSetup, pointOfDelivery, prn, electricityPremise);

			var result = new ContractSaleDto
			{
				SalesOrderID = string.Empty,
				ConsumerID = SapConsumerId.AddGasConsumer,
				AccountID = account.Partner,
				CheckModeOnly = false,
				ContractStartDate = DateTime.Today,
				MoveOutDate = null,
				ProductProposalResult = await getProductProposal,				
			};

			var electricityContract = await getElectricityContract;
			var electricitySale = new ContractSaleDetailDto
			{
				ContractID = electricityContract.ContractID,
				DivisionID = DivisionType.Electricity,
				BusinessAgreementID = electricityContract.BusinessAgreementID,
			};

			var gasContractOnSameBusinessPartner = await GetPremiseExistingGasContractIfSameBusinessPartner(gasPremise, electricityPremise);

			var gasSale = new ContractSaleDetailDto
			{
				ContractID = gasContractOnSameBusinessPartner?.ContractID,
				DivisionID = DivisionType.Gas,
				BusinessAgreementID = gasContractOnSameBusinessPartner?.BusinessAgreementID,
				PointOfDeliveryGUID = gasContractOnSameBusinessPartner != null ? null : pointOfDelivery.PointOfDeliveryId,
			};

			if (paymentSetup == PaymentSetUpType.UseExistingDirectDebit)
			{
				gasSale.IncomingPaymentMethodID = PaymentMethodType.DirectDebit;
				gasSale.BankAccountID = account.IncomingBankAccount.BankAccountId;
				gasSale.SEPAFlag = SapBooleanFlag.Yes.ToBoolean();
			}

			if (paymentSetup == PaymentSetUpType.SetUpNewDirectDebit)
			{
				gasSale.IncomingPaymentMethodID = PaymentMethodType.DirectDebit;
				var bankAccount = await EnsureBankAccountExists(iban,
																nameOnBankAccount,
																account.Partner);

				gasSale.BankAccountID = bankAccount.BankAccountId;
				gasSale.SEPAFlag = SapBooleanFlag.Yes.ToBoolean();
			}

			var deviceId = isNewAcquisition ? null :
				gasPremise.Installations.SelectMany(x => x.Devices).Where(x => x.Registers.Any(r => r.MeterType.IsGas())).First().DeviceId;

			gasSale.MeterReadings.Add(new ContractSaleMeterReadingDto()
			{
				DivisionID = DivisionType.Gas,
				DeviceID = deviceId,
				SerialNumber = "DEVICE",
				RegisterID = "001",
				ReadingDateTime = DateTime.Today,
				ReadingResult = meterReading.ToString(),
				MeterReadingReasonID = MeterReadingReason.MeterReadingAtMoveIn,
				MeterReadingCategoryID = MeterReadingCategoryType.Customer
			});

			gasSale.NewConnection = false;
			result.SaleDetails.AddRange(new[] { electricitySale, gasSale });
			
			await _crmUmcRepository.Add(result);
			return result;
		}

		private async Task<ContractItemDto> GetPremiseExistingGasContractIfSameBusinessPartner(Premise gasPremise,
			Premise electricityPremise)
		{

			ContractItemDto result = null;

			var activeContractsEndTime = new DateTime(9999, 12, 31);
			var getElectricityContract = _crmUmcRepository
				.NewQuery<PremiseDto>()
				.Key(electricityPremise.PremiseId)
				.NavigateTo<ContractItemDto>(x => x.ContractItems)
				.GetMany();
			var gasContract = (await _crmUmcRepository
				.NewQuery<PremiseDto>()
				.Key(gasPremise.PremiseId)
				.NavigateTo<ContractItemDto>(x => x.ContractItems)
				.GetMany()).SingleOrDefault(x => x.ContractEndDate == activeContractsEndTime);
			if (gasContract != null)
			{
				var gasAccount = await _queryResolver.GetAccountInfoByAccountNumber(gasContract.BusinessAgreementID, true);
				var electricityContract = (await getElectricityContract).Single(x => x.ContractEndDate == activeContractsEndTime);
				var electricityAccount = await _queryResolver.GetAccountInfoByAccountNumber(electricityContract.BusinessAgreementID, true);

				if (gasAccount.Partner == electricityAccount.Partner)
				{
					result = gasContract;
				}

			}

			return result;
		}

		public async Task<ContractSaleDto> ExecuteChangeSmartPlanToStandardContractSale(string electricityAccountNumber)
		{
			var electricityContract = await _queryResolver.GetContactByAccountNumber(electricityAccountNumber);
			var account = await _queryResolver.GetAccountInfoByAccountNumber(electricityAccountNumber);
			string gasAccountNumber = null;

			if (!string.IsNullOrEmpty(account.BundleReference))
			{
				var accountInfos = (await _queryResolver.GetDuelFuelAccountsByAccountNumber(electricityAccountNumber)).ToArray();
				gasAccountNumber = accountInfos.SingleOrDefault(x => x.ClientAccountType == ClientAccountType.Gas)?.AccountNumber;
			}

			var productProposal = await _productProposalResolver.ChangeSmartToStandardProductProposal(electricityAccountNumber, gasAccountNumber);

			var result = new ContractSaleDto
			{
				SalesOrderID = string.Empty,
				ConsumerID = SapConsumerId.ExistingCustomer,
				AccountID = account.Partner,
				CheckModeOnly = false,
				ContractStartDate = DateTime.Today,
				MoveOutDate = null,
				ProductProposalResult = productProposal,
			};

			var electricitySale = new ContractSaleDetailDto
			{
				ContractID = electricityContract.ContractID,
				DivisionID = DivisionType.Electricity,
				BusinessAgreementID = electricityContract.BusinessAgreementID,
			};

			if (string.IsNullOrEmpty(gasAccountNumber))
			{
				result.SaleDetails.AddRange(new[] { electricitySale });
			}
			else
			{
				var gasContract = await _queryResolver.GetContactByAccountNumber(gasAccountNumber);
				var gasSale = new ContractSaleDetailDto
				{
					ContractID = gasContract.ContractID,
					DivisionID = DivisionType.Gas,
					BusinessAgreementID = gasContract.BusinessAgreementID,
				};

				result.SaleDetails.AddRange(new[] { electricitySale, gasSale });
			}
			
			await _crmUmcRepository.Add(result);
			return result;
		}

		public async Task<ContractSaleDto> ActivateSmartMeter(ActivateSmartMeterCommand commandData)
		{
			var electricityContract = await _queryResolver.GetContactByAccountNumber(commandData.ElectricityAccountNumber);
			var account = await _queryResolver.GetAccountInfoByAccountNumber(commandData.ElectricityAccountNumber);
			string gasAccountNumber = null;

			if (!string.IsNullOrEmpty(account.BundleReference))
			{
				var accountInfos = (await _queryResolver.GetDuelFuelAccountsByAccountNumber(commandData.ElectricityAccountNumber)).ToArray();
				gasAccountNumber = accountInfos.SingleOrDefault(x => x.ClientAccountType == ClientAccountType.Gas)?.AccountNumber;
			}

			var productProposal = await _productProposalResolver.GetActivateSmartProductProposal(commandData, gasAccountNumber);

			var result = new ContractSaleDto
			{
				SalesOrderID = string.Empty,
				ConsumerID = SapConsumerId.ActivateSmart,
				AccountID = account.Partner,
				CheckModeOnly = false,
				ContractStartDate = DateTime.Today.AddDays(1),
				MoveOutDate = null,
				ProductProposalResult = productProposal,
			};

			var electricitySaleDetail = new ContractSaleDetailDto
			{
				ContractID = electricityContract.ContractID,
				DivisionID = DivisionType.Electricity,
				BusinessAgreementID = electricityContract.BusinessAgreementID,
				SmartConsentStatusID = SmartConsent.ConsentGiven,
				FixedBillingDate = commandData.MonthlyBilling ? commandData.MonthlyBillingSelectedDay.ToString("D2") : string.Empty
			};

			electricitySaleDetail = await SetUpPaymentDetails(account.Partner, 
															  electricitySaleDetail,
															  commandData.CommandsToExecute.SingleOrDefault(x => x.AccountType == ClientAccountType.Electricity));

			if (string.IsNullOrEmpty(gasAccountNumber))
			{
				result.SaleDetails.AddRange(new[] { electricitySaleDetail });
			}
			else
			{
				var gasContract = await _queryResolver.GetContactByAccountNumber(gasAccountNumber);
				var gasSaleDetail = new ContractSaleDetailDto
				{
					ContractID = gasContract.ContractID,
					DivisionID = DivisionType.Gas,
					BusinessAgreementID = gasContract.BusinessAgreementID,
				};

				gasSaleDetail = await SetUpPaymentDetails(account.Partner,
														  gasSaleDetail,
														  commandData.CommandsToExecute.SingleOrDefault(x => x.AccountType == ClientAccountType.Gas));

				result.SaleDetails.AddRange(new[] { electricitySaleDetail, gasSaleDetail });
			}

			await _crmUmcRepository.Add(result);
			return result;
		}
	}
}
