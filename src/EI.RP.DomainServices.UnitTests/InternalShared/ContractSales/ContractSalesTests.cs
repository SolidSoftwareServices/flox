using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using EI.RP.DomainServices.InternalShared.ContractSales;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.InternalShared.Products;
using EI.RP.DomainServices.Queries.Contracts.Contract;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.CoreServices.System;
using NUnit.Framework;
using Moq;

namespace EI.RP.DomainServices.UnitTests.InternalShared.MeterReading
{
	internal class ContractSaleCommandTests : UnitTestFixture<ContractSaleCommandTests.TestContext, ContractSaleCommand>
	{
		public class TestContext : UnitTestContext<ContractSaleCommand>
		{
			private string _electricityAccountNumber;
			private bool _isDuelFuel;

			public DomainFacade DomainFacade { get; } = new DomainFacade();

			public TestContext WithElectricityAccountNumber(string electricityAccountNumber)
			{
				_electricityAccountNumber = electricityAccountNumber;
				return this;
			}

			public TestContext WitDuelFuelAccount(bool isDuelFuel)
			{
				_isDuelFuel = isDuelFuel;
				return this;
			}

			public void SetupMocks()
			{
				DomainFacade.SetUpMocker(AutoMocker);
			}
		}

		private void SetupContactsMocksForAccount(string accountNumber)
		{
			var contractItem = Context.Fixture.Build<ContractItem>().Create();

			Context.DomainFacade.QueryResolver.ExpectQuery(new ContractInfoQuery
			{
				AccountNumber = accountNumber
			}, contractItem.ToOneItemArray().AsEnumerable());
		}

		private void SetupProductProposalMock(string electricityAccountNumber, 
											  string gasAccountNumber, 
											  ProductProposalResultDto expectedProductProposal)
		{
			var productProposalMock = Context.AutoMocker.GetMock<IProductProposalResolver>();
			productProposalMock.Setup(x => x.ChangeSmartToStandardProductProposal(electricityAccountNumber, gasAccountNumber))
							   .ReturnsAsync(expectedProductProposal);
		}

		[Test]
		[Theory]
		public async Task CanChangeSmartPlanToStandardContractSale(bool isDuelFuel, bool addGas)
		{
			Context.Fixture.CustomizeDomainTypeBuilders();
			var expectedProductPropsalForHomeElecticity = new ProductProposalResultDto { ElecProductID = "RE_HOME_ELEC" };
			var expectedProductPropsalForHomeDual = new ProductProposalResultDto { BundleID = "RDF_HOME_ELEC", ElecProductID = "RE_HOME_ELEC", GasProductID = "RG_GASPLAN" };

			var cfg = new AppUserConfigurator(Context.DomainFacade);
			cfg.AddElectricityAccount(isSmart: true);

			if (addGas || isDuelFuel)
			{
				cfg.AddGasAccount(
					duelFuelSisterAccount: isDuelFuel ? cfg.ElectricityAndGasAccountConfigurators.Single() : null);
			}

			cfg.Execute();

			var electricityAccount = cfg.ElectricityAccount()?.Model;
			if (!isDuelFuel) electricityAccount.BundleReference = null;			
			var electricityAccountNumber = electricityAccount.AccountNumber;

			SetupContactsMocksForAccount(electricityAccountNumber);

			if (isDuelFuel)
			{
				var gasAccount = cfg.GasAccount()?.Model;
				SetupContactsMocksForAccount(gasAccount.AccountNumber);
				SetupProductProposalMock(electricityAccountNumber, gasAccount.AccountNumber, expectedProductPropsalForHomeDual);
			} else
			{
				SetupProductProposalMock(electricityAccountNumber, null, expectedProductPropsalForHomeElecticity);
			}

			Context
				.WithElectricityAccountNumber(electricityAccountNumber)
				.WitDuelFuelAccount(isDuelFuel)
				.SetupMocks();

			var result = await Context
				.Sut
				.ExecuteChangeSmartPlanToStandardContractSale(electricityAccountNumber);

			var expectedResult = new ContractSaleDto()
			{
				SalesOrderID = string.Empty,
				ConsumerID = SapConsumerId.ExistingCustomer,
				AccountID = electricityAccount.Partner,
				CheckModeOnly = false,
				ContractStartDate = DateTime.Today,
				MoveOutDate = null,
				ProductProposalResult = isDuelFuel ? expectedProductPropsalForHomeDual : expectedProductPropsalForHomeElecticity,
			};

			Assert.IsNotNull(result);

			Assert.AreEqual(result.ConsumerID, expectedResult.ConsumerID);
			Assert.AreEqual(result.AccountID, expectedResult.AccountID);
			Assert.AreEqual(result.CheckModeOnly, expectedResult.CheckModeOnly);
			Assert.AreEqual(result.ContractStartDate, expectedResult.ContractStartDate);

			Assert.IsTrue(isDuelFuel ? result.SaleDetails.Count() == 2 : result.SaleDetails.Count() == 1);

			Assert.AreEqual(result.ProductProposalResult, expectedResult.ProductProposalResult);

			Assert.IsTrue(result.SaleDetails.Any(x=> x.DivisionID == DivisionType.Electricity));
			if (isDuelFuel)
			{
				Assert.IsTrue(result.SaleDetails.Any(x => x.DivisionID == DivisionType.Gas));
			} else
			{
				Assert.IsFalse(result.SaleDetails.Any(x => x.DivisionID == DivisionType.Gas));
			}
		}
	}
}
