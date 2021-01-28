using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using Moq;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Commands.Banking.DirectDebit.SetUpDirectDebit
{
	internal class SetupDirectDebitCommandHandlerTests : CommandHandlerTest<SetUpDirectDebitCommandHandler, SetUpDirectDebitDomainCommand>
	{
		internal class CaseModel
		{
			public ClientAccountType ClientAccountType { get; set; }
			public string Description { get; set; }
			public string CaseName { get; set; }
		}

		private static IEnumerable<TestCaseData> CanResolveCases()
		{
			var cases = new[]
			{
				new CaseModel
				{
					ClientAccountType  = ClientAccountType.Electricity,
					Description = "Equalizer created on online portal with first payment date ELEC_PAYMENT_DATE for amount ELEC_MONTHLY_AMOUNT",
					CaseName = $"{nameof(EqualizerSetsTheCorrectBusinessActivityDescription)}_New_Equalizer_For_{ClientAccountType.Electricity}"
				},
				new CaseModel
				{
					ClientAccountType  = ClientAccountType.Gas,
					Description = "Equalizer created on online portal with first payment date GAS_PAYMENT_DATE for amount GAS_MONTHLY_AMOUNT",
					CaseName =$"{nameof(EqualizerSetsTheCorrectBusinessActivityDescription)}_New_Equalizer_For_{ClientAccountType.Gas}"
				}
			};

			foreach (var caseItem in cases)
			{
				yield return new TestCaseData(caseItem)
					.SetName(caseItem.CaseName);
			}
		}


		[TestCaseSource(nameof(CanResolveCases))]
		public async Task EqualizerSetsTheCorrectBusinessActivityDescription(CaseModel testCase)
		{
			var setUpDirectDebitDomainCommand = new SetUpDirectDebitDomainCommand(
				accountNumber: "12345",
				nameOnBankAccount: "nameOnBankAccount",
				existingIban: "",
				newIban: "IBANNew",
				businessPartner: "businessPartner",
				accountType: testCase.ClientAccountType,
				paymentMethodType: PaymentMethodType.Equalizer,
				startDate: DateTime.Now,
				firstDueDate: DateTime.Now);

			var publishBusinessActivityDomainCommand = new PublishBusinessActivityDomainCommand(
				activityType: PublishBusinessActivityDomainCommand.BusinessActivityType.EqualizerSetup,
				businessPartner: setUpDirectDebitDomainCommand.BusinessPartner,
				accountNumber: setUpDirectDebitDomainCommand.AccountNumber,
				documentStatus:"",
				processType:"",
				subject:"",
				description: testCase.Description);

			var businessAgreement = new BusinessAgreementDto()
			{
				SEPAFlag = SapBooleanFlag.No,
				IncomingPaymentMethod = new PaymentMethodDto() {Description = PaymentMethodType.DirectDebit}
			};

			var businessActivityPublisher = Context.AutoMocker.GetMock<ICommandHandler<PublishBusinessActivityDomainCommand>>();
			Context.AutoMocker.Use(businessActivityPublisher);

			var crmUmcRepo = Context.AutoMocker.GetMock<ISapRepositoryOfCrmUmc>();

			SetupBankAccountsQuery();
			SetupBusinessAgreementQuery();
			SetupAddBankAccount();
			SetupBusinessAgreementUpdate();

			Context.AutoMocker.Use(crmUmcRepo);
			Context.AutoMocker.Use(businessActivityPublisher);

			await Context.Sut.ExecuteAsync(setUpDirectDebitDomainCommand);

			businessActivityPublisher.Verify(x => x.ExecuteAsync(publishBusinessActivityDomainCommand), Times.Once);

			void SetupBankAccountsQuery()
			{
				var keyMock = new Mock<IFluentODataModelQuery<AccountDto>>();
				var expandsBankMock = new Mock<IFluentODataModelQuery<AccountDto>>();
				var mockQuery = new Mock<IFluentODataModelQuery<AccountDto>>();

				crmUmcRepo.Setup(x => x.NewQuery<AccountDto>()).Returns(keyMock.Object);
				keyMock.Setup(x => x.Key(setUpDirectDebitDomainCommand.BusinessPartner)).Returns(expandsBankMock.Object);
				expandsBankMock.Setup(x => x.Expand(y => y.BankAccounts)).Returns(mockQuery.Object);
			}

			void SetupAddBankAccount()
			{
				crmUmcRepo.Setup(x => x.AddThenGet(It.IsAny<BankAccountDto>(), It.IsAny<bool>())).Returns(Task.FromResult(new BankAccountDto()));
			}

			void SetupBusinessAgreementQuery()
			{
				var keyMock = new Mock<IFluentODataModelQuery<BusinessAgreementDto>>();
				var businessPartnerQueryMock = new Mock<IFluentODataModelQuery<BusinessAgreementDto>>();

				crmUmcRepo.Setup(x => x.NewQuery<BusinessAgreementDto>()).Returns(keyMock.Object);
				keyMock.Setup(x => x.Key(setUpDirectDebitDomainCommand.AccountNumber)).Returns(businessPartnerQueryMock.Object);

				crmUmcRepo.Setup(x => x.GetOne(It.IsAny<IFluentODataModelQuery<BusinessAgreementDto>>(), It.IsAny<bool>()))
					.Returns(Task.FromResult(businessAgreement));
			}

			void SetupBusinessAgreementUpdate()
			{
				crmUmcRepo.Setup(x => x.UpdateThenGet(businessAgreement, It.IsAny<bool>())).Returns(Task.FromResult(businessAgreement));
			}
		}
	}
}