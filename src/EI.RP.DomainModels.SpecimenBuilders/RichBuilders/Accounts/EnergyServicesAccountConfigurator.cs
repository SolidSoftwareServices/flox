using System;
using System.Linq;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Others;
using Ei.Rp.DomainModels.User;
using EI.RP.DomainServices.Queries.Billing.LatestBill;
using EI.RP.DomainServices.Queries.User.UserContact;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts
{
	public class EnergyServicesAccountConfigurator : AccountConfigurator<EnergyServicesAccountConfigurator>
	{
		private bool _asOpened;
		private PaymentMethodType _paymentType;

		public EnergyServicesAccountConfigurator(DomainFacade domainFacade) : base(domainFacade, ClientAccountType.EnergyService)
		{
			FinancialActivitiesConfiguration = new FinancialActivitiesConfiguration(domainFacade);
		}
		public FinancialActivitiesConfiguration FinancialActivitiesConfiguration { get; }
		public EnergyServicesAccountConfigurator WithOpenedStatus(bool opened)
		{
			
			_asOpened = opened;
			return this;
		}
		public EnergyServicesAccountConfigurator WithPaymentMethodType(PaymentMethodType paymentType)
		{
			_paymentType = paymentType;
			return this;
		}

		public LatestBillInfo LatestBill { get; private set; }

       

        public UserContactDetails UserContactDetails { get; set; }

        protected override void ConfigureDependenciesForSpecificAccountType(AccountInfo accountInfoModel)
        {
	        accountInfoModel.ContractEndDate = _asOpened ? new DateTime(9999, 12, 31) : DateTime.Today.AddDays(-1);
            accountInfoModel.IsOpen = _asOpened;
			ConfigureLatestBill(accountInfoModel);
            ConfigureUserAccount();
			FinancialActivitiesConfiguration.WithInvoices(3).Execute(Model.AccountNumber,_paymentType);
        }

        private void ConfigureUserAccount()
        {
            UserContactDetails = DomainFacade.ModelsBuilder.Build<UserContactDetails>()
                .With(x => x.AccountNumber, _accountNumber)
                .With(x => x.ContactEMail, DomainFacade.ModelsBuilder.Create<EmailAddress>().ToString)
                .With(x => x.PrimaryPhoneNumber, "0892332458")
                .With(x => x.AlternativePhoneNumber, "0892332458")
                .With(x => x.CommunicationPreference, CommunicationPreferenceType.AllValues.Select(x => new CommunicationPreference
                {
                    Accepted = DomainFacade.ModelsBuilder.Create<bool>(),
                    PreferenceType = (CommunicationPreferenceType)x
                }))
                .Create();

            DomainFacade.QueryResolver.ExpectQuery(new UserContactDetailsQuery
            {
                AccountNumber = _accountNumber
            }, UserContactDetails.ToOneItemArray());
        }

        private void ConfigureLatestBill(AccountInfo accountInfoModel)
		{
			LatestBill = DomainFacade.ModelsBuilder.Build<LatestBillInfo>()
				.With(x => x.AccountNumber, _accountNumber)
				.With(x => x.AccountIsOpen, _asOpened)

				//TODO: CONFIGURATION VALUES???
				.With(x => x.MeterReadingType, MeterReadingCategoryType.Actual)
				.With(x => x.PaymentMethod, PaymentMethodType.DirectDebit)
				

				.Create();
			DomainFacade.QueryResolver
				.ExpectQuery(new LatestBillQuery
				{
					AccountNumber = _accountNumber,
				}, LatestBill.ToOneItemArray());
		}

        

    }
}