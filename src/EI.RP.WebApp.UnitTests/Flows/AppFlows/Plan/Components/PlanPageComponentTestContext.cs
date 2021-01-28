using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Platform;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.TestServices;
using EI.RP.WebApp.UnitTests.Infrastructure;
using Moq;
using Moq.AutoMock;
using System;
using EI.RP.DomainServices.Infrastructure.Settings;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Plan.Components
{
	class PlanPageComponentTestContext<TViewModelBuilder> : UnitTestContext<TViewModelBuilder>
		where TViewModelBuilder : class
	{
		public string AccountNumber { get; private set; }
		private string _planName;
		private decimal _discount;
		private bool _hasElectricity = true;
		private bool _hasGas;
		private ClientAccountType _selectedAccount = ClientAccountType.Electricity;
		private bool _isAlreadySmart;
		private bool _canBeSmart;
		private bool _isElectricityOpen;
		private PaymentMethodType _paymentMethodType;
		private bool _hasPaperBill;
		private bool _isContractPending;
		private bool _isSmartActivationEnabled;

		public PlanPageComponentTestContext() : base(new Fixture().CustomizeDomainTypeBuilders())
		{
		}

		public DomainFacade DomainFacade { get; } = new DomainFacade();

		protected override TViewModelBuilder BuildSut(AutoMocker autoMocker)
		{
			Validate();

			var platformSettings = new Mock<IDomainSettings>();
			platformSettings.Setup(_ => _.IsSmartActivationEnabled).Returns(_isSmartActivationEnabled);
			autoMocker.Use(platformSettings);

			var cfg = AddAccounts();

			AccountNumber = ResolveSelectedAccount()
				.Model
				.AccountNumber;
			DomainFacade.SetUpMocker(autoMocker);
			
			return base.BuildSut(autoMocker);

			void Validate()
			{
				if (!_hasElectricity && _selectedAccount.IsElectricity())
				{
					throw new InvalidOperationException("Electricity configuration is not valid");
				}

				if (!_hasGas && _selectedAccount.IsGas())
				{
					throw new InvalidOperationException("gas configuration is not valid");
				}
			}

			CommonElectricityAndGasAccountConfigurator ResolveSelectedAccount()
			{
				if (_selectedAccount.IsElectricity())
					return cfg.Execute().ElectricityAccount();
				if (_selectedAccount.IsGas())
					return cfg.Execute().GasAccount();

				throw new InvalidOperationException();
			}

			AppUserConfigurator AddAccounts()
			{
				var result = new AppUserConfigurator(DomainFacade);
				ElectricityAccountConfigurator electricityCfg = null;
				GasAccountConfigurator gasCfg = null;

				if (_hasElectricity)
				{
					electricityCfg = result.AddElectricityAccount(opened: _isElectricityOpen, planName: _planName,
							discount: _discount,
							isSmart: _isAlreadySmart, paymentType: _paymentMethodType,withPaperBill:_hasPaperBill, isContractPending: _isContractPending)
						.WithElectricity24HrsDevices(
							_isAlreadySmart ? RegisterConfigType.MCC12 : RegisterConfigType.MCC01,
							_isSmartActivationEnabled && _canBeSmart ? CommsTechnicallyFeasibleValue.CTF3 : CommsTechnicallyFeasibleValue.CTF1);

				}

				if (_hasGas)
				{
					gasCfg = result.AddGasAccount(planName: _planName, discount: _discount,
						duelFuelSisterAccount: _hasElectricity ? electricityCfg : null,
						paymentType: _paymentMethodType, withPaperBill: _hasPaperBill);
				}

				return result;
			}
		}

		public PlanPageComponentTestContext<TViewModelBuilder> WithAccounts(bool hasElectricity, bool hasGas,
			bool isElectricityOpen)
		{
			_hasElectricity = hasElectricity;
			_hasGas = hasGas;
			_isElectricityOpen = isElectricityOpen;
			return this;
		}

		public PlanPageComponentTestContext<TViewModelBuilder> WithSelectedAccount(ClientAccountType selectedAccount)
		{
			_selectedAccount = selectedAccount;
			return this;
		}



		public PlanPageComponentTestContext<TViewModelBuilder> WithPlanName(string planName)
		{
			_planName = planName;
			return this;
		}

		public PlanPageComponentTestContext<TViewModelBuilder> WithDiscount(decimal discount)
		{
			_discount = discount;
			return this;
		}


		public PlanPageComponentTestContext<TViewModelBuilder> WithSmartConfiguration(bool isAlreadySmart,
			bool canBeSmart)
		{
			_isAlreadySmart = isAlreadySmart;
			_canBeSmart = canBeSmart;

			return this;
		}

		public PlanPageComponentTestContext<TViewModelBuilder> WithPaymentMethod(PaymentMethodType paymentMethod)
		{
			_paymentMethodType = paymentMethod;
			return this;
		}

		public PlanPageComponentTestContext<TViewModelBuilder> WithIsSmartActivationEnabled(bool isSmartActivationEnabled)
		{
			_isSmartActivationEnabled = isSmartActivationEnabled;
			return this;
		}

		public PlanPageComponentTestContext<TViewModelBuilder> WithPaperBill(bool hasPaperbill)
		{
			_hasPaperBill = hasPaperbill;
			return this;
		}

		public PlanPageComponentTestContext<TViewModelBuilder> WithIsContractPending(bool isContractPending)
		{
			_isContractPending = isContractPending;
			return this;
		}
	}
}