using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using AutoFixture;
using Moq;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using System.Collections.Generic;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.DomainServices.Queries.SmartActivation.SmartActivationPlan;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Infrastructure
{
	class Step5SummaryDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;
		public ElectricityPointReferenceNumber Mprn { get; private set; }
		public List<SetUpDirectDebitDomainCommand> DebitDomainCommands { get; private set; }
		
		public Step5SummaryDataBuilder(AppUserConfigurator domainConfigurator)
		{
			_domainConfigurator = domainConfigurator;
		}
		public Step5Summary.ScreenModel LastCreated { get; private set; }
		public Step5Summary.ScreenModel Create(string step2SelectedPlanName)
		{
			IFixture fixture = new Fixture().CustomizeDomainTypeBuilders();
			var screenData = _domainConfigurator.DomainFacade.ModelsBuilder
				.Build<Step5Summary.ScreenModel>()
				.With(x=>x.SelectedPlanName,step2SelectedPlanName)
				.Create();	
			
			Mprn = fixture.Create<ElectricityPointReferenceNumber>();
			var accountInfoQueryResult = new AccountInfo { PointReferenceNumber = Mprn };

			_domainConfigurator.DomainFacade.QueryResolver.Current.Setup(
					x => x.FetchAsync<AccountInfoQuery, AccountInfo>(It.IsAny<AccountInfoQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(accountInfoQueryResult.ToOneItemArray().AsEnumerable()));

		

			DebitDomainCommands = new List<SetUpDirectDebitDomainCommand>();

			foreach (var item in screenData.PaymentInfo)
			{
				if (item.SelectedPaymentSetUpType.IsOneOf(PaymentSetUpType.SetUpNewDirectDebit, PaymentSetUpType.UseExistingDirectDebit))
				{
					var clientAccountType = item.Account == null ? item.TargetAccountType : item.Account.ClientAccountType;
					var command = new SetUpDirectDebitDomainCommand(item.Account?.AccountNumber,
						item.CommandToExecute.NameOnBankAccount,
						item.BankAccount?.IBAN,
						item.CommandToExecute.IBAN,
						item.Account?.Partner,
						clientAccountType,
						PaymentMethodType.DirectDebit,
						null,
						null,
						null);

					DebitDomainCommands.Add(command);
				}
			}

			return LastCreated = screenData;
		}

		
	}
}