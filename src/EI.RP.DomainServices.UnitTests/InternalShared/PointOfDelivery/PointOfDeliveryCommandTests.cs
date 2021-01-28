using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.TestServices;
using EI.RP.DomainServices.InternalShared.PointOfDelivery;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataServices;
using EI.RP.DomainServices.Infrastructure.Mappers;
using Ei.Rp.DomainModels.Metering;
using NUnit.Framework;
using Moq;
using Moq.AutoMock;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;

namespace EI.RP.DomainServices.UnitTests.InternalShared.PointOfDelivery
{
	internal class PointOfDeliveryCommandTests : UnitTestFixture<PointOfDeliveryCommandTests.TestContext, PointOfDeliveryCommand>
	{
		internal class CaseModel
		{
			public ClientAccountType AccountType { get; set; }
			public string CaseName { get; set; }
		}

		private static IEnumerable<TestCaseData> CanResolveCases(string methodName)
		{
			var cases = new[]
			{
				new CaseModel { AccountType = ClientAccountType.Gas, CaseName = "NewPodForGas" },
				new CaseModel { AccountType = ClientAccountType.Electricity, CaseName = "NewPodForElecticity" },
			};

			foreach (var caseItem in cases)
			{
				yield return new TestCaseData(caseItem)
					.SetName($"{methodName} - {caseItem.CaseName}");
			}
		}

		public class TestContext : UnitTestContext<PointOfDeliveryCommand>
		{
			public string PremiseID { get; private set; }
			public PointReferenceNumber NewPrn { get; private set; }
			public string AccountNumber { get; private set; }
			private ClientAccountType _accountType;


			public TestContext WithClientAccountType(ClientAccountType accountType)
			{
				_accountType = accountType;
				return this;
			}

			protected override PointOfDeliveryCommand BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				domainFacade.SetUpMocker(autoMocker);
				IFixture fixure = new Fixture().CustomizeDomainTypeBuilders();

				var gprn = fixure.Create<GasPointReferenceNumber>();
				var mprn = fixure.Create<ElectricityPointReferenceNumber>();
				NewPrn = _accountType.Equals(ClientAccountType.Gas) ? (PointReferenceNumber)gprn : (PointReferenceNumber)mprn;
				PremiseID = fixure.Create<long>().ToString();

				var cfg = new AppUserConfigurator(domainFacade);
				cfg.AddElectricityAccount();
				cfg.AddGasAccount();
				cfg.Execute();

				var electricityAccount = cfg.ElectricityAccount()?.Model;
				var electricityAccountNumber = electricityAccount.AccountNumber;
				var gasAccount = cfg.GasAccount()?.Model;
				var gasAccountNumber = gasAccount.AccountNumber;

				AccountNumber = _accountType == ClientAccountType.Gas ? electricityAccountNumber : gasAccountNumber;

				var contractItemDto = fixure.Build<ContractItemDto>()
								.Create();

				var businessAgreementDto = fixure
					.Build<BusinessAgreementDto>()
					.With(x => x.ContractItems, contractItemDto.ToOneItemArray().ToList())
					.Create();

				var newPointOfDeliveryDto = fixure
					.Build<PointOfDeliveryDto>()
					.With(x => x.ExternalID, (string)NewPrn)
					.With(x => x.DivisionID, _accountType.ToDivisionType())
					.Create();

				newPointOfDeliveryDto.SetAddAsOdata(false);

				var newPointOfDelivery = fixure
					.Build<PointOfDeliveryInfo>()
					.With(x => x.Prn, (string)NewPrn)
					.With(x => x.PointOfDeliveryId, newPointOfDeliveryDto.PointOfDeliveryID)
					.With(x => x.PremiseId, PremiseID)
					.Create();

				var premiseDto = fixure
					.Build<PremiseDto>()
					.With(x => x.PremiseID, PremiseID)
					.With(x => x.ContractItems, contractItemDto.ToOneItemArray().ToList())
					.Create();

				var premiseQueryMock = AutoMocker.GetMock<IFluentODataModelQuery<PremiseDto>>();
				premiseQueryMock.Setup(x => x.Key(PremiseID)).Returns(premiseQueryMock.Object);
				premiseQueryMock.Setup(x => x.Expand(c => c.ContractItems)).Returns(premiseQueryMock.Object);
				premiseQueryMock.Setup(x => x.GetMany(It.IsAny<bool>())).ReturnsAsync(premiseDto.ToOneItemArray());

				var businessAgreementQueryMock = AutoMocker.GetMock<IFluentODataModelQuery<BusinessAgreementDto>>();
				businessAgreementQueryMock.Setup(x => x.Key(AccountNumber)).Returns(businessAgreementQueryMock.Object);
				businessAgreementQueryMock.Setup(x => x.Expand(c => c.ContractItems)).Returns(businessAgreementQueryMock.Object);
				businessAgreementQueryMock.Setup(x => x.Expand(c => c.ContractItems[0].Premise)).Returns(businessAgreementQueryMock.Object);
				businessAgreementQueryMock.Setup(x => x.GetOne()).ReturnsAsync(businessAgreementDto);

				var repoMock = AutoMocker.GetMock<ISapRepositoryOfCrmUmc>();
				repoMock.Setup(x => x.NewQuery<BusinessAgreementDto>()).Returns(businessAgreementQueryMock.Object);
				repoMock.Setup(x => x.NewQuery<PremiseDto>()).Returns(premiseQueryMock.Object);
				repoMock.Setup(x => x.AddThenGet(It.Is<PointOfDeliveryDto>(y => y.AddsAsOData() == false), It.IsAny<bool>())).ReturnsAsync(newPointOfDeliveryDto);

				var mockDomainMapper = AutoMocker.GetMock<IDomainMapper<PointOfDeliveryDto, PointOfDeliveryInfo>>();
				mockDomainMapper.Setup(x => x.Map(newPointOfDeliveryDto)).ReturnsAsync(newPointOfDelivery);
				AutoMocker.Use(mockDomainMapper);

				return base.BuildSut(autoMocker);
			}
		}

		[TestCaseSource(nameof(CanResolveCases), new object[] { nameof(CanAddPointOfDeliveryWhenPremisesAddressOfAccountNumber) })]
		public async Task CanAddPointOfDeliveryWhenPremisesAddressOfAccountNumber(CaseModel caseModel)
		{
			var result = await Context
				.WithClientAccountType(caseModel.AccountType)
				.Sut
				.AddPointOfDelivery(Context.NewPrn, caseModel.AccountType, Context.AccountNumber, null);

			AssertResult(result, Context.NewPrn, caseModel.AccountType);
		}

		[TestCaseSource(nameof(CanResolveCases), new object[] { nameof(CanAddPointOfDeliveryWhenUsePremisesAddressFromPremiseId) })]
		public async Task CanAddPointOfDeliveryWhenUsePremisesAddressFromPremiseId(CaseModel caseModel)
		{
			var result = await Context
				.WithClientAccountType(caseModel.AccountType)
				.Sut
				.AddPointOfDelivery(Context.NewPrn, caseModel.AccountType, null, Context.PremiseID);

			AssertResult(result, Context.NewPrn, caseModel.AccountType);
		}

		private void AssertResult(PointOfDeliveryInfo result, PointReferenceNumber newPrn, ClientAccountType AccountType)
		{
			Assert.NotNull(result);
			Assert.IsTrue(result.IsNewAcquisition);
			Assert.NotNull(result.AddressInfo);

			var cmd = Context.AutoMocker.GetMock<ISapRepositoryOfCrmUmc>();
			cmd.Verify(x => x.AddThenGet(It.Is<PointOfDeliveryDto>(y => y.AddsAsOData() == false &&
																		y.DivisionID == AccountType.ToDivisionType() &&
																		y.ExternalID == (string)newPrn),
																		It.IsAny<bool>()), Times.Once);
		}

	}
}
