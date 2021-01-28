using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.TestServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.Context;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Queries.MovingHouse.Progress;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.Stubs.CoreServices.Http.Session;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.DataModels.Events;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using NUnit.Framework;
using AutoFixture;
using Moq.AutoMock;

namespace EI.RP.DomainServices.UnitTests.Commands.Contracts.MovingHouse
{
	internal class MoveHouseEventBuilderTest : UnitTestFixture<MoveHouseEventBuilderTest.TestContext,
		MoveHouseEventBuilder>
	{
		public class TestContext : UnitTestContext<MoveHouseEventBuilder>
		{			
			public DomainFacade DomainFacade { get; } = new DomainFacade();
			public string TestUserName = "myusername";
			public string ElectrictyAccountNumber { get; private set; }
			public string GasAccountNumber { get; private set; }
			public string PartnerNumber { get; private set; }
			public MovingHouseType MovingHouseType { get; private set; }

			public TestContext WithMovingHouseType(MovingHouseType movingHouseType)
			{
				MovingHouseType = movingHouseType;
				return this;
			}

			protected override MoveHouseEventBuilder BuildSut(AutoMocker autoMocker)
			{
				DomainFacade.SetUpMocker(autoMocker);
				IFixture fixture = new Fixture().CustomizeDomainTypeBuilders();

				var claims = new List<Claim>()
				{
					new Claim(ClaimTypes.Name, TestUserName),
					new Claim(ClaimTypes.NameIdentifier, "userId")
				};
				var identity = new ClaimsIdentity(claims, "TestAuthType");
				var claimsPrincipal = new ClaimsPrincipal(identity);

				var mockSession = autoMocker.GetMock<FakeSessionProviderStrategy>();
				mockSession.SetupGet(c => c.CurrentUserClaimsPrincipal)
					.Returns(claimsPrincipal);
				autoMocker.Use<IUserSessionProvider>(mockSession.Object);
		
				PartnerNumber = fixture.Create<long>().ToString();
				ElectrictyAccountNumber = fixture.Create<long>().ToString();
				GasAccountNumber = fixture.Create<long>().ToString();

				var electrictyAccount = fixture
					.Build<AccountInfo>()
					.With(x => x.AccountNumber, ElectrictyAccountNumber)
					.With(x => x.ClientAccountType, ClientAccountType.Electricity)
					.With(x => x.Partner, PartnerNumber)
					.Create();

				var gasAccount = fixture
					.Build<AccountInfo>()
					.With(x => x.AccountNumber, GasAccountNumber)
					.With(x => x.ClientAccountType, ClientAccountType.Gas)
					.With(x => x.Partner, PartnerNumber)
					.Create();

				var movingHouseInProgressMovingOutInfo = fixture
					.Build<MovingHouseInProgressMovingOutInfo>()
					.Create();

				var movingHouseInProgressMovingInInfo = fixture
					.Build<MovingHouseInProgressMovingInInfo>()
					.Create();

				var movingHouseInProgressNewPRNsInfo = fixture
					.Build<MovingHouseInProgressNewPRNsInfo>()
					.Create();

				DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery()
				{
					AccountNumber = ElectrictyAccountNumber,
				}, electrictyAccount.ToOneItemArray().AsEnumerable());

				DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery()
				{
					AccountNumber = GasAccountNumber,
				}, gasAccount.ToOneItemArray().AsEnumerable());

				DomainFacade.QueryResolver.ExpectQuery(new MoveHouseProgressQuery()
				{
					MoveType = MovingHouseType.MoveElectricityAndGas,
					InitiatedFromAccount = electrictyAccount,
					OtherAccount = gasAccount
				}, movingHouseInProgressMovingOutInfo.ToOneItemArray().AsEnumerable());

				DomainFacade.QueryResolver.ExpectQuery(new MoveHouseProgressQuery()
				{
					MoveType = MovingHouseType.MoveElectricityAndGas,
					InitiatedFromAccount = electrictyAccount,
					OtherAccount = gasAccount
				}, movingHouseInProgressMovingInInfo.ToOneItemArray().AsEnumerable());

				DomainFacade.QueryResolver.ExpectQuery(new MoveHouseProgressQuery()
				{
					MoveType = MovingHouseType.MoveElectricityAndGas,
					InitiatedFromAccount = electrictyAccount,
					OtherAccount = gasAccount
				}, movingHouseInProgressNewPRNsInfo.ToOneItemArray().AsEnumerable());

				return base.BuildSut(autoMocker);
			}
		}

		internal class CaseModel
		{
			public MovingHouseType MovingHouseType { get; set; }
			public int ErrorOnStepNumber { get; set; }
			public string CustomErrorEventDescription { get; set; }
			public string TestName { get; set; }
		}

		public static IEnumerable<TestCaseData> CanResolveCases()
		{
			var cases = new[]
			{
				new CaseModel { ErrorOnStepNumber = 1, MovingHouseType = MovingHouseType.MoveElectricityAndGas, CustomErrorEventDescription = null, TestName = "ErrorInStoreNewIncommingOccupantCompleted_1" },
				new CaseModel { ErrorOnStepNumber = 2, MovingHouseType = MovingHouseType.MoveElectricityAndGas, CustomErrorEventDescription = "Error submitting CMeter Read for close account", TestName = "ErrorSubmitMoveOutMeterReadCompleted_2" },
				new CaseModel { ErrorOnStepNumber = 3, MovingHouseType = MovingHouseType.MoveElectricityAndGas, CustomErrorEventDescription = "Error submitting CMeter Read for moveIn account", TestName = "ErrorSubmitMoveInMeterReadCompleted_3" },
				new CaseModel { ErrorOnStepNumber = 4, MovingHouseType = MovingHouseType.MoveElectricityAndGas, CustomErrorEventDescription = "Error setting up new direct debit, moving house", TestName = "ErrorSetUpNewDirectDebitsCompleted_4" },
				new CaseModel { ErrorOnStepNumber = 5, MovingHouseType = MovingHouseType.MoveElectricityAndGas, CustomErrorEventDescription = null, TestName = "ErrorSubmitNewContractCompleted_5" },
			};

			foreach (var caseItem in cases)
			{
				yield return new TestCaseData(caseItem).SetName(caseItem.TestName);
			}
		}

		[TestCaseSource(nameof(CanResolveCases))]
		public async Task TestBuildEventsOnError(CaseModel caseModel)
		{
			var sutContext = Context
								.WithMovingHouseType(caseModel.MovingHouseType)
								.Sut;

			var cmd = await ArrangeAndGetMoveHouseCommandAsync(caseModel.ErrorOnStepNumber);
			var ex = new AggregateException();

			var actuals =
				await sutContext
				.BuildEventsOnError(cmd, ex);

			foreach(EventApiEvent actual in actuals)
				AssertResult(actual, caseModel.CustomErrorEventDescription, EventAction.LastOperationFailed);
		}

		void AssertResult(EventApiEvent actual, string expectedDescription, long eventAction)
		{
			const int maxDescriptionLength = 50;

			Assert.IsNotNull(actual);
			Assert.IsTrue((actual.Description?.Length ?? 0) <= maxDescriptionLength);
			if (expectedDescription != null)
			{
				Assert.IsTrue(actual.Description.Equals(expectedDescription));
			}
			Assert.IsTrue(actual.ActionId.Equals(eventAction));
			Assert.IsTrue(actual.CategoryId.Equals(EventCategory.MovingHouse));
			Assert.IsTrue(actual.Username == Context.TestUserName);
			Assert.IsTrue(actual.Partner.Equals(long.Parse(Context.PartnerNumber)));
			
			Assert.IsTrue(actual.SubCategoryId.Equals(EventSubCategory.MovingHouseCloseElec) || actual.SubCategoryId.Equals(EventSubCategory.MovingHouseCloseGas));
			if (actual.SubCategoryId.Equals(EventSubCategory.MovingHouseCloseElec))
			{
				Assert.IsTrue(actual.ContractAccount.Equals(long.Parse(Context.ElectrictyAccountNumber)));
			}

			if (actual.SubCategoryId.Equals(EventSubCategory.MovingHouseCloseGas))
			{
				Assert.IsTrue(actual.ContractAccount.Equals(long.Parse(Context.GasAccountNumber)));
			}
		}

		[Test]
		public async Task TestBuildEventsOnSuccess()
		{
			var sutContext = Context
								.WithMovingHouseType(MovingHouseType.MoveElectricityAndGas)
								.Sut;

			var cmd = await ArrangeAndGetMoveHouseCommandAsync();

			var actual =
				await sutContext
				.BuildEventsOnSuccess(cmd);

			Assert.IsNotNull(actual);
			Assert.IsEmpty(actual);
		}

		async Task<MoveHouse> ArrangeAndGetMoveHouseCommandAsync(int errorOnStepNumber = 0)
		{
			var cmd = new MoveHouse(Context.ElectrictyAccountNumber, Context.GasAccountNumber, Context.MovingHouseType, null, ClientAccountType.Electricity);
			cmd.Context = new CompleteMoveHouseContext(Context.DomainFacade.QueryResolver.Current.Object);
			await cmd.Context.Resolve(cmd);

			cmd.Context.CheckPoints.StoreNewIncommingOccupantCompleted_1 = errorOnStepNumber != 1;
			cmd.Context.CheckPoints.SubmitMoveOutMeterReadCompleted_2 = errorOnStepNumber != 2;
			cmd.Context.CheckPoints.SubmitMoveInMeterReadCompleted_3 = errorOnStepNumber != 3;
			cmd.Context.CheckPoints.SetUpNewDirectDebitsCompleted_4 = errorOnStepNumber != 4;
			cmd.Context.CheckPoints.SubmitNewContractCompleted_5 = errorOnStepNumber != 5;

			return cmd;
		}
	}
}
