using System;
using System.Collections.Generic;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.FixtureExtensions;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts
{
	public class AddNewDuelFuelAccountConfigurator
	{
		private readonly DomainFacade _domainFacade;
		public ClientAccountType AccountType { get; }
		private readonly bool _newAccountAddressExists;

		public AddNewDuelFuelAccountConfigurator(DomainFacade domainFacade,ClientAccountType accountType, bool newAccountAddressExists, bool isPrnDeRegistered)
		{
			_fixture = domainFacade.ModelsBuilder;

			_domainFacade = domainFacade;
			_newAccountAddressExists = newAccountAddressExists;
			IsDeRegistered = isPrnDeRegistered;
			AccountType = accountType;
			AccountNumber = _fixture.Create<long>().ToString();
			Partner = _fixture.Create<long>().ToString();
		}

		public bool IsDeRegistered { get;  }

		public string Partner { get; }

		public string AccountNumber { get; }

		public PointReferenceNumber Prn => _executed?_prn:throw new InvalidOperationException("configurator Not Executed yet");

		private  PointReferenceNumber _prn;
		private readonly IFixture _fixture;
		private bool _executed = false;
		private void ThrowIfAlreadyExecuted()
		{
			if (_executed)
			{
				throw new InvalidOperationException("configurator already Executed ");
			}
		}

		public AccountInfo NewAccountOnceAdded { get; private set; }

	

		public string NewContractId { get; private set; }
		public IEnumerable<DeviceInfo> Devices { get; private set; }

		public void Execute()
		{
			ThrowIfAlreadyExecuted();

			NewContractId = _fixture.Create<long>().ToString();

			_prn = this.AccountType == ClientAccountType.Electricity
				? (PointReferenceNumber) _fixture.Create<ElectricityPointReferenceNumber>()
				: this.AccountType == ClientAccountType.Gas
					? _fixture.Create<GasPointReferenceNumber>()
					: throw new NotSupportedException();

			NewAccountOnceAdded = _fixture.Build<AccountInfo>()
				.With(x => x.AccountNumber, AccountNumber
				)
				.With(x => x.Partner, Partner)
				.With(x => x.ClientAccountType, this.AccountType)

				.With(x => x.IsOpen, true)
				.With(x => x.PaperBillChoice, PaperBillChoice.On)
				.With(x => x.PaymentMethod, PaymentMethodType.Manual)
				.With(x => x.PointReferenceNumber,
					AccountType == ClientAccountType.Electricity 
						? _fixture.Create<ElectricityPointReferenceNumber>() 
						: AccountType == ClientAccountType.Gas 
							? _fixture.Create<GasPointReferenceNumber>() 
							: (PointReferenceNumber) null)
				.Create();



			_domainFacade.QueryResolver.ConfigureAccountInfo(NewAccountOnceAdded, _prn);


			Devices = _fixture.CreateDevice(NewContractId,
					this.AccountType == ClientAccountType.Electricity ? MeterType.Electricity24h : MeterType.Gas)
				.ToOneItemArray();

			if (!_newAccountAddressExists)
			{
				_domainFacade.QueryResolver.ExpectQuery(new PointOfDeliveryQuery
				{

					Prn = _prn,
				}, new PointOfDeliveryInfo[]{null});

			}


			_executed = true;
		}

		
	}
}