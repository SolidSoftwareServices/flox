using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.CrmUmc.Functions;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.Context;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.Queries.MovingHouse.IsPrnNewAcquisition;
using EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.DataModels.ResidentialPortal;

namespace EI.RP.DomainServices.InternalShared.Products
{
	class ProductProposalResolver : IProductProposalResolver
	{
		private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;
		private readonly IDomainQueryResolver _queryResolver;

		public ProductProposalResolver(ISapRepositoryOfCrmUmc crmUmcRepository, IDomainQueryResolver queryResolver)
		{
			_crmUmcRepository = crmUmcRepository;
			_queryResolver = queryResolver;
		}
		public async Task<ProductProposalResultDto> GetMoveHouseProductProposal(MoveHouse command)
		{
			CompleteMoveHouseContext context = command.Context;
			var contractAccount = context.AllAccounts().First(x=>x.IsContractAccount);

			var isElectricityNewPointsOfDeliveryNewAcquisition = context.HasNewMprn() ? context.NewPointsOfDelivery[ClientAccountType.Electricity].IsNewAcquisition : false;
			var isNewPremiseElectricityNewAcquisition = context.HasNewMprn() ? await _queryResolver.IsPrnNewAcquisition(context.NewPrns.NewMprn,
																				isElectricityNewPointsOfDeliveryNewAcquisition) : false;

			var isGasNewPointsOfDeliveryNewAcquisition = context.HasNewGprn() ? context.NewPointsOfDelivery[ClientAccountType.Gas].IsNewAcquisition : false;
			var isNewGasPremiseNewAcquisition = context.HasNewGprn() ? await _queryResolver.IsPrnNewAcquisition(context.NewPrns.NewGprn,
																				 isGasNewPointsOfDeliveryNewAcquisition) : false;

			var salesType = GetSalesType(context.HasNewMprn(), 
							   		     context.HasNewGprn(), 
										 isNewPremiseElectricityNewAcquisition, 
										 isNewGasPremiseNewAcquisition);

			var productProposalFunction = new ProductProposalFunction
			{
				Query =
				{
					SalesOrigin = "WEB",
					AccountID = contractAccount.Account.Partner,
					PaperlessBilling = !contractAccount.Account.PaperBillChoice
						.ToBoolean(),
					BonusAmount = isNewPremiseElectricityNewAcquisition || isNewGasPremiseNewAcquisition ? "0175" : "0000",
					ElecFlag = context.HasNewMprn(),
					ElecSalesType = salesType,

					ElecMeterType = "CR",
					ElecRegisterConfig = string.Empty,
					ElecCTF = String.Empty,
					ElecQuarterHourly = false,
					ElecAnnualConsumption = 0M,

                    GasFlag = context.HasNewGprn(),
					GasSalesType = salesType,
					GasMeterType = "CR",
					GasAnnualConsumption = 0M,
                    GasBand = "A"
                }
			};

			var query = productProposalFunction.Query;

			if (context.HasNewMprn())
			{
				var getNewMprnPremise =
					_queryResolver.GetPremiseByPrn(context.NewPrns.NewMprn, true);
				var newMprnPremise = await getNewMprnPremise;
				query.ElecDuOSGroup = newMprnPremise.ElectricityDuosGroup;
			}

			if (context.Electricity != null)
			{
				query.ElecBusinessAgreementID = context.Electricity.Account.BusinessAgreement.BusinessAgreementId;			
			}

			if (context.Electricity != null || context.HasNewMprn())
			{
				query.ElecDirectDebit = command.CommandsToExecute.Any(x =>
							x.AccountType == ClientAccountType.Electricity &&
							x.PaymentMethodType == PaymentMethodType.DirectDebit);
			}

			if (context.HasNewGprn())
			{
				var getNewGrprnPremise =
					_queryResolver.GetPremiseByPrn(context.NewPrns.NewGprn, true);
				var newGrpnPremise = await getNewGrprnPremise;

				if (newGrpnPremise.GasBand != null)
				{
					query.GasBand = newGrpnPremise.GasBand;
				}			
				else if (isNewGasPremiseNewAcquisition)
				{
					query.GasBand = "B";
				}
			}

			if (context.Gas != null)
			{
				query.GasBusinessAgreementID = context.Gas.Account.BusinessAgreement.BusinessAgreementId;
			}

			if (context.Gas != null || context.HasNewGprn())
			{
				query.GasDirectDebit = command.CommandsToExecute.Any(x =>
						   x.AccountType == ClientAccountType.Gas &&
						   x.PaymentMethodType == PaymentMethodType.DirectDebit);
			}

			var proposals = await _crmUmcRepository.ExecuteFunctionWithManyResults(productProposalFunction);

            var proposal = proposals.Single(x => IsValidFor(x, command.MoveType));
            ValidateResult(proposal);

            return proposal;
        }

		private SalesType GetSalesType(bool hasNewMprn, 
									   bool hasNewGprn,
									   bool isNewPremiseElectricityNewAcquisition, 
									   bool isNewGasPremiseNewAcquisition)
		{
			var salesType = SalesType.AllExistingAcquisitions;

			if (hasNewMprn && hasNewGprn)
			{
				if (isNewPremiseElectricityNewAcquisition && isNewGasPremiseNewAcquisition)
					salesType = SalesType.AllNewAcquisitions;

				if (isNewPremiseElectricityNewAcquisition != isNewGasPremiseNewAcquisition)
					salesType = SalesType.Mix;

				return salesType;
			}

			if (hasNewMprn && !hasNewGprn)
			{
				if (isNewPremiseElectricityNewAcquisition)
					return SalesType.AllNewAcquisitions;
			}

			if (!hasNewMprn && hasNewGprn)
			{
				if (isNewGasPremiseNewAcquisition)
					return SalesType.AllNewAcquisitions;
			}

			return salesType;
		}

		private bool IsValidFor(ProductProposalResultDto proposal, MovingHouseType moveType)
		{
			if (moveType.IsOneOf(MovingHouseType.MoveElectricity, MovingHouseType.MoveElectricityAndCloseGas))
			{
				return proposal.ElecProductID == "RE_ELECTRICITYPLAN_WIN" ||
				       proposal.ElecProductID == "RE_ELECTRICITYPLAN";
			}
			if (moveType.IsOneOf(MovingHouseType.MoveGas))
			{
				return proposal.GasProductID == "RG_GASPLAN_WIN" ||
				       proposal.GasProductID == "RG_GASPLAN";
			}
			if (moveType.IsOneOf(MovingHouseType.MoveElectricityAndAddGas, MovingHouseType.MoveElectricityAndGas,MovingHouseType.MoveGasAndAddElectricity))
			{
				return proposal.BundleID == "RDF_DUALFUELPLAN_WIN" ||
				       proposal.BundleID == "RDF_DUALFUELPLAN";
			}
			throw new InvalidOperationException();
		}

		public async Task<ProductProposalResultDto> GetAddGasProductProposal(string electricityAccountNumber,
			PaymentSetUpType paymentSetup,
			PointOfDeliveryInfo pointOfDelivery, GasPointReferenceNumber newGprn,
			Premise electricityPremise)
		{
			var electricityContract = _crmUmcRepository.NewQuery<BusinessAgreementDto>()
				.Key(electricityAccountNumber)
				.NavigateTo<ContractItemDto>()
				.Expand(x => x.BusinessAgreement)
				.GetOne();
			
			var electricityAccount = await _queryResolver.GetAccountInfoByAccountNumber(electricityAccountNumber, true);
			var baseElectricityContractItem = await electricityContract;
			var isNewGasPremiseNewAcquisition = await _queryResolver.IsPrnNewAcquisition(newGprn, pointOfDelivery.IsNewAcquisition);
			var gasBand = "A";

			if (isNewGasPremiseNewAcquisition)
			{
				gasBand = "B";
			} else
			{
				var newGrpnPremise = await GetNewPremiseAndValidate(newGprn);
				if (newGrpnPremise.GasBand != null)
				{
					gasBand = newGrpnPremise.GasBand;
				}
			}

			var productProposalFunction = new ProductProposalFunction
			{
				Query =
				{
					SalesOrigin = "WEB",
					AccountID = electricityAccount.Partner,
					PaperlessBilling = !((PaperBillChoice) baseElectricityContractItem.BusinessAgreement.PaperBill)
						.ToBoolean(),
					BonusAmount = isNewGasPremiseNewAcquisition ? "0175" : "0000",
					ElecFlag = true,
					ElecBusinessAgreementID = baseElectricityContractItem.BusinessAgreementID,
					ElecSalesType = isNewGasPremiseNewAcquisition ? SalesType.Mix : SalesType.AllExistingAcquisitions,
					ElecDuOSGroup = electricityPremise.ElectricityDuosGroup,
					ElecDirectDebit = electricityAccount.PaymentMethod == PaymentMethodType.DirectDebit,
					ElecMeterType = "CR",
					ElecRegisterConfig = electricityPremise.RegisterConfiguration.IsSmartElectricity()
						? electricityPremise.RegisterConfiguration
						: string.Empty,
					ElecCTF = electricityPremise.ElectricityCTF,
					ElecQuarterHourly = false,
					ElecAnnualConsumption = 0M,
					GasFlag = true,
					GasSalesType = isNewGasPremiseNewAcquisition ? SalesType.Mix : SalesType.AllExistingAcquisitions,
					GasBand = gasBand,
					GasDirectDebit = paymentSetup.IsOneOf(PaymentSetUpType.SetUpNewDirectDebit,
						PaymentSetUpType.UseExistingDirectDebit),
					GasMeterType = "CR",
					GasAnnualConsumption = 0M

				}
			};
			var proposals = await _crmUmcRepository.ExecuteFunctionWithManyResults(productProposalFunction);

			var proposal = proposals.Single(x => x.IsNonSmartProduct());
			ValidateResult(proposal);

			return proposal;
		}

		private void ValidateResult(ProductProposalResultDto p)
		{
			if (p == null)
			{
				throw new InvalidOperationException("Product proposal not found");
			}
		}

		private async Task<Premise> GetNewPremiseAndValidate(PointReferenceNumber prn)
		{
			var result = await _queryResolver.GetPremiseByPrn(prn, true);
			if (result.Installations.Any(x => x.DiscStatus != InstallationDiscStatusType.New))
			{
				throw new DomainException(ResidentialDomainError.InstallationDiscStatusIsNotNew);
			}

			return result;
		}

		public async Task<ProductProposalResultDto> ChangeSmartToStandardProductProposal(string electricityAccountNumber, string gasAccountNumber)
		{
			var electricityContract = _crmUmcRepository.NewQuery<BusinessAgreementDto>()
				.Key(electricityAccountNumber)
				.NavigateTo<ContractItemDto>()
				.Expand(x => x.BusinessAgreement)
				.GetOne();

			var electricityAccount = await _queryResolver.GetAccountInfoByAccountNumber(electricityAccountNumber, true);
			var electricityContractItem = await electricityContract;
			var getElectricityPremise = _queryResolver.GetPremise(electricityContractItem.PremiseID, true);
			var electricityPremise = await getElectricityPremise;

			var hasGas = !string.IsNullOrEmpty(gasAccountNumber);
			AccountInfo gasAccount = null;
			var gasBusinessAgreementID = string.Empty;
			var gasBand = "A";

			if (hasGas)
			{
				gasAccount = await _queryResolver.GetAccountInfoByAccountNumber(gasAccountNumber, true);
				var gasContract = await _crmUmcRepository.NewQuery<BusinessAgreementDto>()
					.Key(gasAccountNumber)
					.NavigateTo<ContractItemDto>()
					.Expand(x => x.BusinessAgreement)
					.GetOne();

				gasBusinessAgreementID = gasContract.BusinessAgreementID;

				var gasPremise = await _queryResolver.GetPremise(gasContract.PremiseID, true);
				if (gasPremise.GasBand != null)
				{
					gasBand = gasPremise.GasBand;
				}
			}


			var productProposalFunction = new ProductProposalFunction
			{
				Query =
				{
					SalesOrigin = "WEB",
					AccountID = electricityAccount.Partner,
					PaperlessBilling = !((PaperBillChoice) electricityContractItem.BusinessAgreement.PaperBill)
						.ToBoolean(),
					BonusAmount = "0000",
					ElecFlag = true,
					ElecBusinessAgreementID = electricityContractItem.BusinessAgreementID,
					ElecSalesType = SalesType.AllExistingAcquisitions,
					ElecDuOSGroup = electricityPremise.ElectricityDuosGroup,
					ElecDirectDebit = electricityAccount.PaymentMethod == PaymentMethodType.DirectDebit,
					ElecRegisterConfig = RegisterConfigType.MCC16.ToString(),
					ElecCTF = electricityPremise.ElectricityCTF,
					ElecQuarterHourly = false,
					ElecAnnualConsumption = 0M,
					ElecMeterType = "CR",
					GasFlag = hasGas,
					GasBusinessAgreementID = gasBusinessAgreementID,
					GasSalesType = hasGas ? SalesType.AllExistingAcquisitions : string.Empty,
					GasMeterType = hasGas ? "CR" : string.Empty,
					GasAnnualConsumption = 0M,
					GasBand = hasGas ? gasBand : string.Empty,
					GasDirectDebit = hasGas ? gasAccount.PaymentMethod == PaymentMethodType.DirectDebit : false,
				}
			};

			var proposals = await _crmUmcRepository.ExecuteFunctionWithManyResults(productProposalFunction);
			var proposal = proposals.Single(x => x.IsStandardProduct());
			ValidateResult(proposal);

			return proposal;
		}

		public async Task<ProductProposalResultDto> GetActivateSmartProductProposal(ActivateSmartMeterCommand command, string gasAccountNumber)
		{
			var electricityContract = _crmUmcRepository.NewQuery<BusinessAgreementDto>()
				.Key(command.ElectricityAccountNumber)
				.NavigateTo<ContractItemDto>()
				.Expand(x => x.BusinessAgreement)
				.GetOne();

			var electricityAccount = await _queryResolver.GetAccountInfoByAccountNumber(command.ElectricityAccountNumber, true);
			var electricityContractItem = await electricityContract;
			var getElectricityPremise = _queryResolver.GetPremise(electricityContractItem.PremiseID, true);
			var electricityPremise = await getElectricityPremise;

			var hasGas = !string.IsNullOrEmpty(gasAccountNumber);
			AccountInfo gasAccount = null;
			var gasBusinessAgreementID = string.Empty;
			var gasBand = "A";

			if (hasGas)
			{
				gasAccount = await _queryResolver.GetAccountInfoByAccountNumber(gasAccountNumber, true);
				var gasContract = await _crmUmcRepository.NewQuery<BusinessAgreementDto>()
					.Key(gasAccountNumber)
					.NavigateTo<ContractItemDto>()
					.Expand(x => x.BusinessAgreement)
					.GetOne();

				gasBusinessAgreementID = gasContract.BusinessAgreementID;

				var gasPremise = await _queryResolver.GetPremise(gasContract.PremiseID, true);
				if (gasPremise.GasBand != null)
				{
					gasBand = gasPremise.GasBand;
				}
			}

			var productProposalFunction = new ProductProposalFunction
			{
				Query =
				{
					SalesOrigin = "WEB",
					AccountID = electricityAccount.Partner,
					PaperlessBilling = true,
					BonusAmount = "0000",
					ElecFlag = true,
					ElecBusinessAgreementID = electricityContractItem.BusinessAgreementID,
					ElecSalesType = SalesType.AllExistingAcquisitions,
					ElecDuOSGroup = electricityPremise.ElectricityDuosGroup,
					ElecDirectDebit = command.CommandsToExecute.Any(x =>
							x.AccountType == ClientAccountType.Electricity &&
							x.PaymentMethodType == PaymentMethodType.DirectDebit),
					ElecRegisterConfig = RegisterConfigType.MCC12.ToString(),
					ElecCTF = electricityPremise.ElectricityCTFOperandValue,
					ElecQuarterHourly = false,
					ElecAnnualConsumption = 0M,
					ElecMeterType = "CR",
					GasFlag = hasGas,
					GasBusinessAgreementID = gasBusinessAgreementID,
					GasSalesType = hasGas ? SalesType.AllExistingAcquisitions : string.Empty,
					GasMeterType = hasGas ? "CR" : string.Empty,
					GasAnnualConsumption = 0M,
					GasBand = hasGas ? gasBand : string.Empty,
					GasDirectDebit = hasGas ? command.CommandsToExecute.Any(x =>
							x.AccountType == ClientAccountType.Gas &&
							x.PaymentMethodType == PaymentMethodType.DirectDebit) : false,
				}
			};

			var proposals = await _crmUmcRepository.ExecuteFunctionWithManyResults(productProposalFunction);
			var proposal = proposals.Single(x => IsValidFor(x, command.SelectedPlan, command.SelectedPlanFreeDay));
			ValidateResult(proposal);

			return proposal;
		}

		private bool IsValidFor(ProductProposalResultDto proposal, SmartPlan selectedPlan, DayOfWeek? selectedPlanFreeDay)
		{
			var hasDayVariants = selectedPlan.FreeDayOfElectricityChoice.Any();
			var smartActivationProductProposal = new SmartActivationProductProposal
			{
				FreeDayOfElectricityChoice = hasDayVariants ? selectedPlanFreeDay.ToString() : string.Empty,
				BundleID = proposal.BundleID,
				ElecProductID = proposal.ElecProductID,
				GasProductID = proposal.GasProductID,
			};

			var isValid = selectedPlan.ValidProductProposals.Any(x => x.Equals(smartActivationProductProposal));

			return isValid;
		}
	}
}
