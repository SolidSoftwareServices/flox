using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.Gas;
using EI.RP.DomainServices.InternalShared.ContractSales;
using EI.RP.DomainServices.InternalShared.MeterReading;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.Queries.MovingHouse.IsPrnNewAcquisition;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using EI.RP.DomainServices.InternalShared.PointOfDelivery;
using Ei.Rp.DomainModels.MappingValues;
using System.Collections.Generic;

namespace EI.RP.DomainServices.UnitTests.Commands.Contracts.AddAdditionalAccount.Gas
{
	internal class AddGasAccountCommandHandlerTest : CommandHandlerTest<AddGasAccountCommandHandler, AddGasAccountCommand>
	{
		internal class CaseModel
		{
			public bool IsPODNewAcquisition { get; set; }
			public bool HasInstallations { get; set; }
			public bool IsInstallationDeregStatusDeregistered { get; set; }
			public PaymentSetUpType PaymentSetupType { get; set; }
			public string CaseName { get; set; }
		}

		private static IEnumerable<TestCaseData> CanResolveCases()
		{
			var cases = new[]
			{
				new CaseModel { IsPODNewAcquisition = true, HasInstallations = true, IsInstallationDeregStatusDeregistered = false, PaymentSetupType = PaymentSetUpType.SetUpNewDirectDebit, CaseName = "PODIsNewAcquisition" },
				new CaseModel { IsPODNewAcquisition = false, HasInstallations = false, IsInstallationDeregStatusDeregistered = false, PaymentSetupType = PaymentSetUpType.SetUpNewDirectDebit, CaseName = "NoInstallationDevices" },
				new CaseModel { IsPODNewAcquisition = false, HasInstallations = true, IsInstallationDeregStatusDeregistered = true, PaymentSetupType = PaymentSetUpType.SetUpNewDirectDebit, CaseName = "InstallationDeregStatusDeregistered" },
				new CaseModel { IsPODNewAcquisition = false, HasInstallations = true, IsInstallationDeregStatusDeregistered = false, PaymentSetupType = PaymentSetUpType.SetUpNewDirectDebit, CaseName = "WithPaymentSetUpNewDirectDebit" },
				new CaseModel { IsPODNewAcquisition = false, HasInstallations = true, IsInstallationDeregStatusDeregistered = false, PaymentSetupType = PaymentSetUpType.UseExistingDirectDebit, CaseName = "WithPaymentUseExistingDirectDebit" },
				new CaseModel { IsPODNewAcquisition = false, HasInstallations = true, IsInstallationDeregStatusDeregistered = false, PaymentSetupType = PaymentSetUpType.Manual, CaseName = "WithPaymentManual" },
			};

			foreach (var caseItem in cases)
			{
				yield return new TestCaseData(caseItem)
					.SetName(caseItem.CaseName);
			}
		}

		[TestCaseSource(nameof(CanResolveCases))]
		public async Task AddGasCommandExecuteMethods(CaseModel caseModel)
		{
			var gprn = Context.Fixture.Create<GasPointReferenceNumber>();
			var electricityAccountNumber = Context.Fixture.Create<long>().ToString();
			var meterReading = Context.Fixture.Create<decimal>();
			var premise = Context
					.Fixture
					.Build<Premise>()
					.Without(x => x.Address)
					.Create();

			var pod = Context
					.Fixture
					.Build<PointOfDeliveryInfo>()
					.With(x => x.IsNewAcquisition, caseModel.IsPODNewAcquisition)
					.Without(x => x.AddressInfo)
					.Create();

			var cmd = ArrangeAndGetCommand();

			await Context.Sut.ExecuteAsync(cmd);

			Assert(cmd.IBAN, cmd.NameOnBankAccount);

			void Assert(string iban, string nameOnBankAccount)
			{
				var submiteMeterReadingService = Context.AutoMocker.GetMock<ISubmitMeterReadings>();
				var isNewAcquisition = caseModel.IsPODNewAcquisition || !caseModel.HasInstallations || caseModel.IsInstallationDeregStatusDeregistered;

				submiteMeterReadingService.Verify(
					x => x.SubmitGasMeterReading(electricityAccountNumber, isNewAcquisition, gprn, meterReading, true), Times.Once);
				submiteMeterReadingService.Verify(
					x => x.SubmitGasMeterReading(electricityAccountNumber, isNewAcquisition, gprn, meterReading, false), Times.Never);
				submiteMeterReadingService.VerifyNoOtherCalls();

				var contractSalesCommand = Context.AutoMocker.GetMock<IContractSaleCommand>();
				contractSalesCommand.Verify(x =>
						x.ExecuteAddGasContractSale(electricityAccountNumber, caseModel.PaymentSetupType, gprn, meterReading, premise, iban, nameOnBankAccount, pod),
					Times.Once);
				contractSalesCommand.VerifyNoOtherCalls();

				var newPoDCommand = Context.AutoMocker.GetMock<IPointOfDeliveryCommand>();
				if (caseModel.IsPODNewAcquisition)
				{
					newPoDCommand.Verify(x =>
						x.AddPointOfDelivery(gprn, ClientAccountType.Gas, electricityAccountNumber, null), Times.Once);
				}
				else
				{
					newPoDCommand.Verify(x =>
						x.AddPointOfDelivery(gprn, ClientAccountType.Gas, electricityAccountNumber, null), Times.Never);
				}

				newPoDCommand.VerifyNoOtherCalls();
			}

			AddGasAccountCommand ArrangeAndGetCommand()
			{
				var contractItemDto = Context.Fixture.Build<ContractItemDto>()
					.Without(c => c.ContractItemTimeSlices)
					.Without(c => c.Product)
					.Without(c => c.Division)
					.Without(c => c.ContractItemEXTAttrs)
					.Without(c => c.SmartConsents)
					.Without(c => c.Attributes)
					.Without(c => c.Dates)
					.With(c => c.PremiseID, premise.PremiseId)
					.With(c => c.Premise,
						Context.Fixture.Build<PremiseDto>()
							.With(p => p.PremiseID, premise.PremiseId)
							.Create()
						)
					.Create();

				var electricityContract = Context.Fixture
					.Build<BusinessAgreementDto>()
					.With(x => x.BusinessAgreementID, electricityAccountNumber)
					.With(x => x.ContractItems, contractItemDto.ToOneItemArray().ToList())
					.Without(x => x.IncomingAlternativePayerPaymentCard)
					.Without(x => x.IncomingPaymentMethod)
					.Without(x => x.OutgoingPaymentMethod5)
					.Without(x => x.AlternativePayer)
					.Without(x => x.OutgoingAlternativePayeePaymentCard)
					.Without(x => x.IncomingAlternativePayerBankAccount)
					.Without(x => x.OutgoingBankAccount)
					.Without(x => x.OutgoingPaymentMethod3)
					.Without(x => x.PaymentCard)
					.Without(x => x.Country)
					.Without(x => x.IncomingBankAccount)
					.Without(x => x.OutgoingAlternativePayeeBankAccount)
					.Without(x => x.OutgoingPaymentMethod4)
					.Without(x => x.OutgoingPaymentMethod2)
					.Without(x => x.IncomingPaymentCard)
					.Without(x => x.OutgoingPaymentCard)
					.Without(x => x.AlternativePayee)
					.Without(x => x.BillToAccountAddress)
					.Without(x => x.OutgoingPaymentMethod1)
					.Without(x => x.AccountAddress)
					.Without(x => x.CollectiveParent)
					.Without(x => x.AlternativePayerBuAg)
					.Create();

				Context.CrmUmcRepoMock.Value.MockQuery(contractItemDto.ToOneItemArray());
				Context.CrmUmcRepoMock.Value.MockQuery(contractItemDto.Premise.PointOfDeliveries);
				Context.CrmUmcRepoMock.Value.MockQuery(electricityContract.ToOneItemArray());
				Context.CrmUmcRepoMock.Value.WithNavigation<BusinessAgreementDto, ContractItemDto>();
				Context.CrmUmcRepoMock.Value.MockQuerySingle(contractItemDto);

				var contractQueryMock = Context.AutoMocker.GetMock<IFluentODataModelQuery<ContractItemDto>>();
				contractQueryMock.Setup(_ => _.GetOne()).ReturnsAsync(contractItemDto);

				var domainFacade = new DomainFacade();
				domainFacade.SetUpMocker(Context.AutoMocker);

				domainFacade.QueryResolver.ExpectQuery(new PremisesQuery
				{
					PremiseId = contractItemDto.Premise.PremiseID
				}, premise.ToOneItemArray().AsEnumerable());

				domainFacade.QueryResolver.ExpectQuery(new PointOfDeliveryQuery
				{
					Prn = gprn,
				}, caseModel.IsPODNewAcquisition ? Enumerable.Empty<PointOfDeliveryInfo>() :
					pod.ToOneItemArray().AsEnumerable());

				if (caseModel.IsPODNewAcquisition)
				{
					var newPoDMock = Context.AutoMocker.GetMock<IPointOfDeliveryCommand>();
					newPoDMock.Setup(x => x.AddPointOfDelivery(gprn, ClientAccountType.Gas, electricityAccountNumber, null))
									   .ReturnsAsync(pod);
				}

				var isNewAcquisition = caseModel.IsPODNewAcquisition || !caseModel.HasInstallations || caseModel.IsInstallationDeregStatusDeregistered;
				var isPrnNewAcquisitionRequestResult = new IsPrnNewAcquisitionRequestResult
				{
					IsNewAcquisition = isNewAcquisition
				};

				domainFacade.QueryResolver.ExpectQuery(new IsPrnNewAcquisitionQuery
				{
					Prn = gprn,
					IsPODNewAcquisition = caseModel.IsPODNewAcquisition
				}, isPrnNewAcquisitionRequestResult.ToOneItemArray().AsEnumerable());

				string iban = caseModel.PaymentSetupType == PaymentSetUpType.SetUpNewDirectDebit ? "IE29AIBK93115212345678" : null;
				string nameOnBankAccount = caseModel.PaymentSetupType == PaymentSetUpType.SetUpNewDirectDebit ? "Name Surename" : null;

				return new AddGasAccountCommand(gprn,
					electricityAccountNumber,
					meterReading,
					caseModel.PaymentSetupType, iban, nameOnBankAccount);
			}

		}

	}
}
