using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;
using AccountDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.AccountDto;
using BankAccountDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.BankAccountDto;

namespace EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit
{
	internal class SetUpDirectDebitCommandHandler : ICommandHandler<SetUpDirectDebitDomainCommand>
	{
		private readonly ICommandHandler<PublishBusinessActivityDomainCommand> _businessActivityPublisher;
		private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;
        private readonly ISapRepositoryOfErpUmc _erpUmcRepository;


        public SetUpDirectDebitCommandHandler(ISapRepositoryOfCrmUmc crmUmcRepository, ISapRepositoryOfErpUmc erpUmcRepository,
            ICommandHandler<PublishBusinessActivityDomainCommand> businessActivityPublisher)
		{
			_crmUmcRepository = crmUmcRepository;
            _erpUmcRepository = erpUmcRepository;
            _businessActivityPublisher = businessActivityPublisher;
		}

		public async Task ExecuteAsync(SetUpDirectDebitDomainCommand command)
		{
			var currentBankAccount = await GetCurrentBankAccount(command);
            await CreatePaymentSchemeWhenEqualizer(command);
			await SubmitDirectDebitSetup(command, currentBankAccount);
		}

        private async Task CreatePaymentSchemeWhenEqualizer(SetUpDirectDebitDomainCommand commandData)
        {
            if (commandData.IsNewBankAccount &&
                commandData.PaymentMethodType == PaymentMethodType.Equalizer)
            {
                var paymentScheme = new PaymentSchemeDto();
                paymentScheme.Frequency = PaymentSchemeFrequency.Monthly;
                paymentScheme.Category = PaymentSchemeCategoryType.MEQCategoryType;
                paymentScheme.StartDate = DateTime.SpecifyKind(commandData.StartDate.Value, DateTimeKind.Unspecified);
				paymentScheme.FirstDueDate = DateTime.SpecifyKind(commandData.FirstDueDate.Value, DateTimeKind.Unspecified);
				paymentScheme.ContractID = commandData.ContractId;
                paymentScheme.Status = string.Empty;
                paymentScheme.Currency = Currency.Euro;
                paymentScheme.PaymentSchemeID = string.Empty;
                paymentScheme.Amount = 0;
                paymentScheme.AlternativePayDate = DateTime.SpecifyKind(commandData.FirstDueDate.Value, DateTimeKind.Unspecified);
                paymentScheme.NextDueDate = DateTime.SpecifyKind(commandData.FirstDueDate.Value, DateTimeKind.Unspecified);
				await _erpUmcRepository.Add(paymentScheme);
            }
        }

        private async Task<BankAccountDto> GetCurrentBankAccount(SetUpDirectDebitDomainCommand commandData)
		{
			var query = _crmUmcRepository.NewQuery<AccountDto>().Key(commandData.BusinessPartner)
				.Expand(x => x.BankAccounts);

			var partnerWithBankAccounts = await _crmUmcRepository.GetOne(query);

			BankAccountDto currentBankAccount = null;
			if (partnerWithBankAccounts != null)
				currentBankAccount =
					partnerWithBankAccounts.BankAccounts.SingleOrDefault(x =>
						x.IBAN == commandData.NewIBAN.ToUpperInvariant());

			return currentBankAccount;
		}

		private async Task SubmitDirectDebitSetup(SetUpDirectDebitDomainCommand commandData,
			BankAccountDto currentBankAccount)
		{
			var bankAccount = MapBankAccount(commandData);

			var businessAgreement = await GetBusinessAgreement();

			await EnsureSEPAFlagIsEmpty();

            businessAgreement.IncomingPaymentMethodID = commandData.PaymentMethodType==PaymentMethodType.Manual? PaymentMethodType.DirectDebit:commandData.PaymentMethodType;

			await UpdateBankDetails();

			businessAgreement.SEPAFlag = SapBooleanFlag.Yes;
            ApplyRulesWhenEqualiser();

            businessAgreement = await _crmUmcRepository.UpdateThenGet(businessAgreement);

			await SubmitBusinessActivity(commandData, businessAgreement);


            void ApplyRulesWhenEqualiser()
            {
                if (commandData.IsNewBankAccount && commandData.PaymentMethodType == PaymentMethodType.Equalizer)
                {
                    businessAgreement.InvoiceOutsortingCheckGroup = InvoiceOutsortingCheckgroupType.EQUAL;
                    businessAgreement.BudgetBillingProcedure = BudgetBillingProcedureType.EqualizerPaymentMonthlyPayment;
                }
            }

            async Task EnsureSEPAFlagIsEmpty()
            {
                if (businessAgreement.SEPAFlag != SapBooleanFlag.No)
                {
                    businessAgreement.SEPAFlag = SapBooleanFlag.No;
                    businessAgreement = await _crmUmcRepository.UpdateThenGet(businessAgreement);
                }
            }

            async Task UpdateBankDetails()
            {
                if (currentBankAccount == null)
                {
                    bankAccount = await _crmUmcRepository.AddThenGet(bankAccount);
                    businessAgreement.IncomingPaymentBankAccountID = bankAccount.BankAccountID;
                }
                else
                {
                    currentBankAccount.IBAN = commandData.NewIBAN;
                    currentBankAccount.BankAccountName = commandData.NameOnBankAccount;
                    currentBankAccount.AccountHolder = commandData.NameOnBankAccount;
                    currentBankAccount.BankAccountNo = string.Empty;

                    var a = await _crmUmcRepository.UpdateThenGet(currentBankAccount);
                    businessAgreement.IncomingPaymentBankAccountID = currentBankAccount.BankAccountID;
                }
            }

            async Task<BusinessAgreementDto> GetBusinessAgreement()
            {
	            var getBusinessAgreement = await _crmUmcRepository.GetOne(_crmUmcRepository.NewQuery<BusinessAgreementDto>()
		            .Key(commandData.AccountNumber));
	            if (getBusinessAgreement == null)
	            {
		            throw new DomainException(DomainError.GeneralValidation,
			            $"Could not find a Business agreement with key:{commandData.AccountNumber}");
	            }

	            return getBusinessAgreement;
            }
		}

       

        private static BankAccountDto MapBankAccount(SetUpDirectDebitDomainCommand commandData)
		{
			var bankAccount = new BankAccountDto();
			bankAccount.IBAN = commandData.NewIBAN.Any(x => x == '*')
				? commandData.ExistingIBAN
				: commandData.NewIBAN.ToUpperInvariant();
			bankAccount.AccountHolder = commandData.NameOnBankAccount;
			bankAccount.BankID
				= bankAccount.BankAccountID
					= bankAccount.BankAccountNo
						= bankAccount.ControlKey
							= bankAccount.CollectionAuth
								= string.Empty;
			bankAccount.BankAccountName = commandData.NameOnBankAccount;
			bankAccount.CountryID = "IE";

			bankAccount.AccountID = commandData.BusinessPartner;
			return bankAccount;
		}


		private async Task SubmitBusinessActivity(SetUpDirectDebitDomainCommand commandData,
			BusinessAgreementDto businessAgreementDto)
		{
			PublishBusinessActivityDomainCommand.BusinessActivityType businessActivityType;
            var description = string.Empty;

            if (commandData.IsNewBankAccount)
            {
                if (commandData.PaymentMethodType == PaymentMethodType.Equalizer)
                {
	                if (commandData.AccountType == ClientAccountType.Gas)
	                {
		                description =
			                "Equalizer created on online portal with first payment date GAS_PAYMENT_DATE for amount GAS_MONTHLY_AMOUNT";
	                }
					else if (commandData.AccountType == ClientAccountType.Electricity)
	                {
		                description =
			                "Equalizer created on online portal with first payment date ELEC_PAYMENT_DATE for amount ELEC_MONTHLY_AMOUNT";
					}
                }

                businessActivityType = commandData.PaymentMethodType == PaymentMethodType.Equalizer
                    ? PublishBusinessActivityDomainCommand.BusinessActivityType.EqualizerSetup
                    : PublishBusinessActivityDomainCommand.BusinessActivityType.SubmitDirectDebit;
            }
            else
            {
	            businessActivityType = PublishBusinessActivityDomainCommand.BusinessActivityType.EditDirectDebit;
            }

            await _businessActivityPublisher.ExecuteAsync(new PublishBusinessActivityDomainCommand(businessActivityType,
				commandData.BusinessPartner, commandData.AccountNumber, description: description));

			businessAgreementDto.SEPAFlag = string.Empty;
			await _crmUmcRepository.UpdateThenGet(businessAgreementDto);
		}
	}
}