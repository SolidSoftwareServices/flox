using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainServices.Commands.Users.Session.CreateUserSession;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users
{
	public class AppUserConfigurator : AppDomainConfigurator
	{
		public IEnumerable<CommonElectricityAndGasAccountConfigurator> ElectricityAndGasAccountConfigurators =>
			_electricityAndGasAccountConfigurators;

		public IEnumerable<EnergyServicesAccountConfigurator> EnergyServicesAccountConfigurators =>
			_energyServicesConfigurators;

		private readonly List<CommonElectricityAndGasAccountConfigurator> _electricityAndGasAccountConfigurators = new List<CommonElectricityAndGasAccountConfigurator>();
		private readonly List<EnergyServicesAccountConfigurator> _energyServicesConfigurators = new List<EnergyServicesAccountConfigurator>();
		public IEnumerable<ElectricityAccountConfigurator> ElectricityAccounts() =>
			ElectricityAndGasAccountConfigurators.Where(x => x.AccountType== ClientAccountType.Electricity)
				.Cast<ElectricityAccountConfigurator>().ToArray();

		public IEnumerable<GasAccountConfigurator> GasAccounts() =>
			ElectricityAndGasAccountConfigurators.Where(x => x.AccountType == ClientAccountType.Gas)
				.Cast<GasAccountConfigurator>().ToArray();


		public ElectricityAccountConfigurator ElectricityAccount() => ElectricityAccounts().SingleOrDefault();
		public GasAccountConfigurator GasAccount() => GasAccounts().SingleOrDefault();

		public AppUserConfigurator() : this(new DomainFacade())
		{
		}
		public AppUserConfigurator(DomainFacade domainFacade):this(domainFacade,"any.user.is@fine.ie","AnyPasswordIsFine",ResidentialPortalUserRole.OnlineUserRoi) 
		{
		}
		public AppUserConfigurator(ResidentialPortalUserRole role) : this(new DomainFacade(), "any.user.is@fine.ie", "AnyPasswordIsFine", role)
		{
		}
		public AppUserConfigurator( string userName, string userPassword, ResidentialPortalUserRole userRole) : this(new DomainFacade(),userName,userPassword, userRole)
		{

		}
		public AppUserConfigurator(DomainFacade domainFacade, string userName, string userPassword, ResidentialPortalUserRole userRole) : base(domainFacade)
		{
			UserName = userName;
			Password = userPassword;
			Role = userRole;

		}

		public IEnumerable<AccountInfo> Accounts => ElectricityAndGasAccountConfigurators.Select(x => x.Model)
			.Union(EnergyServicesAccountConfigurators.Select(x => x.Model));

		public string UserName { get; }
		public ResidentialPortalUserRole Role { get; }
		public string Password { get; }


		public ElectricityAccountConfigurator AddElectricityAccount(
			bool opened = true,
			bool withPaperBill = true,
			bool isContractPending = false,
			PaymentMethodType paymentType = null,
			bool canEstimateLatestBill = false,
			bool hasAccountCredit = false,
			bool canBeRefunded = false,
			bool canAddNewAccount = false,
			DateTime[] withEqualizerSetupDates = null,
			CommonElectricityAndGasAccountConfigurator duelFuelSisterAccount = null,
			bool isPRNDeRegistered = false,
			bool configureDefaultDevice = true,
			bool newPrnAddressExists = false,
			bool isMoveOutOk = true,
			bool hasExitFee = false,
			bool isNewMPRNAddressInSwitch = true,
			bool hasStaffDiscount = false,
			bool hasFreeElectricityAllowance = false,
			string planName=null,
			decimal discount=0,
			bool isSmart=false,
			bool hasQuotationsInProgress=false,
			bool canMoveToStandardPlan=false,
			bool switchToSmartPlanDismissed=false,
			bool isEbiller=false)
		{
			if (withEqualizerSetupDates == null)
			{
				withEqualizerSetupDates = new DateTime[0];
			}

			paymentType = paymentType ?? PaymentMethodType.Manual;
			var result = (ElectricityAccountConfigurator) new ElectricityAccountConfigurator(DomainFacade)
				.WithOpenedStatus(opened)
				.WithBillsOnPaper(withPaperBill)
				.WithIsContractPending(isContractPending)
				.WithPaymentMethodType(paymentType)
				.WithLatestBill(canEstimateLatestBill)
				.WithAccountCredit(hasAccountCredit)
				.WithRefundLinkAvailable(canBeRefunded)
				.HavingDuelFuelWith(duelFuelSisterAccount)
                .WithMoveOutOk(isMoveOutOk)
                .WithExitFee(hasExitFee)
                .WithNewMPRNAddressInSwitch(isNewMPRNAddressInSwitch)
				.WithStaffDiscount(hasStaffDiscount)
				.WithFreeElectricityAllowance(hasFreeElectricityAllowance)
				.WithPlan(planName,discount)
				.WithQuotationsInProgress(hasQuotationsInProgress)
				.WithCanMoveToStandardPlan(canMoveToStandardPlan)
				.WithSwitchToSmartPlanDismissed(switchToSmartPlanDismissed)
				.WithEBiller(isEbiller);
			if (isSmart)
			{
				result.WithSmartPeriods(new[]{new DateTimeRange(DateTime.Today.AddDays(-1),DateTime.Today.AddDays(2))});
				result.WithNonSmartPeriods(new[]{new DateTimeRange(DateTime.Today.AddDays(-2),DateTime.Today.AddDays(-1))});
			}
			else
			{
				result.WithNonSmartPeriods(new[]{new DateTimeRange(DateTime.Today.AddDays(-1),DateTime.Today.AddDays(2))});
				result.WithSmartPeriods(new[]{new DateTimeRange(DateTime.Today.AddDays(-2),DateTime.Today.AddDays(-1))});
			}
			if (canAddNewAccount)
			{
				result.WithAddNewAccountAvailable(newPrnAddressExists, isPRNDeRegistered);
			}

			if (configureDefaultDevice)
			{
				result.WithElectricity24HrsDevices();
			}

			if (withEqualizerSetupDates.Any())
			{
				result.WithEqualMonthlyPayments(withEqualizerSetupDates);
			}

			_electricityAndGasAccountConfigurators.Add(result);
			return result;
		}

		public GasAccountConfigurator AddGasAccount(bool opened = true,
			bool isContractPending = false,
			bool withPaperBill = true,
			PaymentMethodType paymentType = null,
			CommonElectricityAndGasAccountConfigurator duelFuelSisterAccount = null,
			bool newPrnAddressExists = false, 
            bool isPrnDeregistered = false, 
            bool configureDefaultDevice = true,
			bool canAddNewAccount = false, 
            bool isMoveOutOk = true, 
            bool hasExitFee = false,
            bool hasInstalmentPlan = false,
			bool hasFreeElectricityAllowance = false,
			string planName = null,
			decimal discount = 0,
			bool isEbiller = false)
		{

			var result = (GasAccountConfigurator) new GasAccountConfigurator(DomainFacade)
				.WithOpenedStatus(opened)
				.WithBillsOnPaper(withPaperBill)
				.WithIsContractPending(isContractPending)
				.WithPaymentMethodType(paymentType)
				.HavingDuelFuelWith(duelFuelSisterAccount)
                .WithMoveOutOk(isMoveOutOk)
                .WithExitFee(hasExitFee)
                .WithInstalmentPlan(hasInstalmentPlan)
				.WithFreeElectricityAllowance(hasFreeElectricityAllowance)
				.WithPlan(planName, discount)
				.WithEBiller(isEbiller); 

			if (canAddNewAccount)
			{
				result.WithAddNewAccountAvailable(newPrnAddressExists, isPrnDeregistered);
			}

			if (configureDefaultDevice)
			{
				result = result.WithGasDevice();
			}

			_electricityAndGasAccountConfigurators.Add(result);
			return result;
		}

		public EnergyServicesAccountConfigurator AddEnergyServicesAccount(
			bool opened = true,
			PaymentMethodType paymentType = null)
		{
			paymentType = paymentType ?? PaymentMethodType.Manual;
			var result = new EnergyServicesAccountConfigurator(DomainFacade)
				.WithOpenedStatus(opened)
				.WithPaymentMethodType(paymentType);

			_energyServicesConfigurators.Add(result);
			return result;
		}

		public AppUserConfigurator Execute()
		{
			foreach (var configurator in ElectricityAndGasAccountConfigurators)
			{
				configurator.ConfigureDomainExpectations();
			}

			foreach (var configurator in EnergyServicesAccountConfigurators)
			{
				configurator.ConfigureDomainExpectations();
			}

			DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(new CreateUserSessionCommand(UserName, Password),
				cmd => { DomainFacade.SetValidSessionFor(UserName, Role); });
			var accountInfos = Accounts.ToArray();
			DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery(), accountInfos.Select(_=>(AccountOverview)_).ToArray());
			DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery(), accountInfos);
			
			DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery {Opened = true}, accountInfos);
			
			DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery {Opened = true}, accountInfos.Select(_=>(AccountOverview)_).ToArray());
			DomainFacade.QueryResolver.ExpectQuery(new AccountBillingActivityQuery(), Accounts);

            return this;
		}

		public AppUserConfigurator SetValidSession()
		{
			DomainFacade.SetValidSessionFor(this.UserName,Role);
			return this;
		}
	}
}