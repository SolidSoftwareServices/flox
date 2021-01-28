using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Banking;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Premises;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts
{
	public abstract class AccountConfigurator<T> : AppDomainConfigurator
	where T:AccountConfigurator<T>
	{
		protected readonly string _accountNumber;
		protected readonly string _contractId;
		public ClientAccountType AccountType { get; }

		protected AccountConfigurator(DomainFacade domainFacade, ClientAccountType accountType) : base(domainFacade)
		{
			AccountType = accountType ?? throw new ArgumentNullException(nameof(accountType));
			_accountNumber = DomainFacade.ModelsBuilder.Create<long>().ToString();
			_contractId = DomainFacade.ModelsBuilder.Create<string>();

			Premise = new PremiseConfigurator(domainFacade, accountType, _contractId);
			NewPremise = new PremiseConfigurator(domainFacade, accountType, DomainFacade.ModelsBuilder.Create<string>());
		}


		public AccountInfo Model { get; private set; }
		public PremiseConfigurator Premise { get; protected set; }
		public PremiseConfigurator NewPremise { get; protected set; }

		public virtual T ConfigureDomainExpectations()
		{
			if (Model != null) throw new InvalidOperationException("Account Already configured");


			var accountInfoComposer = DomainFacade.ModelsBuilder.Build<AccountInfo>()
				.With(x => x.AccountNumber, _accountNumber)
				.With(x => x.Partner, DomainFacade.ModelsBuilder.Create<long>().ToString)
				.With(x => x.ClientAccountType, AccountType)
				.With(x => x.PointReferenceNumber,  Premise.GetPrn(AccountType))
				.With(x => x.BusinessAgreement, GetBusinessAgreement())
				.With(x=>x.IncomingBankAccount, GetIncomingBankAccount())
				;

			Model = ConfigureSpecificTypeAccountInfo(accountInfoComposer).Create();

			DomainFacade.QueryResolver.ConfigureAccountInfo(Model, Premise.GetPrn(AccountType));
			var prn = NewPremise.GetPrn(AccountType);
			DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				Prn = prn
			}, Model.ToOneItemArray());
			DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				Prn = prn
			}, Model.ToOneItemArray().Select(_=>(AccountOverview)_).ToArray());
			ConfigureDependenciesForSpecificAccountType(Model);


			return (T)this;

			BusinessAgreement GetBusinessAgreement()
			{
				return DomainFacade.ModelsBuilder.Build<BusinessAgreement>()
					.With(x => x.BusinessAgreementId, _accountNumber)
					.With(x=>x.IsEBiller, IsEBiller)
					.With(x => x.Contracts,
						DomainFacade.ModelsBuilder.Build<ContractItem>()
							.With(x => x.AccountID, _accountNumber)

							.CreateMany(1).ToArray())
					.Create();
			}

			BankAccountInfo GetIncomingBankAccount()
			{
				return  DomainFacade.ModelsBuilder.Build<BankAccountInfo>()
					.With(x => x.AccountNumber, _accountNumber)
					.With(x => x.PaymentMethod, PaymentType)
					.With(x => x.IBAN)
					.Create();
			}
		}

		protected PaymentMethodType PaymentType { get; set; } = PaymentMethodType.Manual;

		protected bool IsEBiller { get; set; } = false;

		protected virtual void ConfigureDependenciesForSpecificAccountType(AccountInfo accountInfoModel)
		{
		}

		protected virtual IPostprocessComposer<AccountInfo> ConfigureSpecificTypeAccountInfo(
			IPostprocessComposer<AccountInfo> accountInfoComposer)
		{
			return accountInfoComposer;
		}
	}
}