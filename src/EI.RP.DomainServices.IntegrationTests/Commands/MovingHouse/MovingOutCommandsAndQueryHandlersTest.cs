using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Progress;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Commands.MovingHouse
{
	[Explicit]
	[TestFixture]
	public class MovingOutCommandsAndQueryHandlersTest : DomainTests
	{
		private readonly IFixture _fixture = new Fixture().CustomizeDomainTypeBuilders();
		public static IEnumerable<TestCaseData> CanExecuteThenQueryCases()
		{
			yield return new TestCaseData(ClientAccountType.Electricity, ClientAccountType.Gas);
			yield return new TestCaseData(ClientAccountType.Gas, ClientAccountType.Electricity);
			yield return new TestCaseData(ClientAccountType.Electricity, null);
			yield return new TestCaseData(ClientAccountType.Gas, null);
		}
		[TestCaseSource(nameof(CanExecuteThenQueryCases))]

		public async Task CanExecuteThenQuery(ClientAccountType initiatorAccountType, ClientAccountType otherAccountType)
		{
			
			await LoginUser("DFDD19@esb.ie", "Test3333");
			var moveOut = await GetRecordMovingOutProgress(initiatorAccountType,otherAccountType);


			var first = await DomainQueryProvider.GetMovingHouseProgressMoveOutInfo(moveOut.InitiatedFromAccount,
				moveOut.OtherAccount, MovingHouseType.MoveElectricityAndGas);
			await TestMoveOut24H();

			await TestMoveOutDayNight();

			await SimulateStep2();

			await SimulateStep3();
			return;

			async Task SimulateStep2()
			{
				//initial load
				var stepRecord = await DomainQueryProvider.GetMovingHouseProgressNewPrnsInfo(moveOut.InitiatedFromAccount, moveOut.OtherAccount, MovingHouseType.MoveElectricityAndGas);
				Assert.IsNotNull(stepRecord);

				//on leaving step
				string newMprn = stepRecord.NewMprn==null? "10901555555" : (long.Parse(stepRecord.NewMprn.ToString())+1).ToString();
				string newGprn = stepRecord.NewGprn == null ? "15201" : (long.Parse(stepRecord.NewGprn.ToString()) + 1).ToString();
				var cmd=new RecordMovingHomePrns(MovingHouseType.MoveElectricityAndGas,moveOut.InitiatedFromAccount,moveOut.OtherAccount, newMprn,newGprn);
				await DomainCommandDispatcher.ExecuteAsync(cmd);

				//on reload
				stepRecord = await DomainQueryProvider.GetMovingHouseProgressNewPrnsInfo(moveOut.InitiatedFromAccount, moveOut.OtherAccount, MovingHouseType.MoveElectricityAndGas);
				Assert.IsNotNull(stepRecord);
				if (initiatorAccountType == ClientAccountType.Gas || otherAccountType == ClientAccountType.Gas)
				{
					Assert.AreEqual((GasPointReferenceNumber)newGprn,stepRecord.NewGprn);
				}

				if (initiatorAccountType == ClientAccountType.Electricity || otherAccountType == ClientAccountType.Electricity)
				{
					Assert.AreEqual((ElectricityPointReferenceNumber)newMprn, stepRecord.NewMprn);
				}

			}

			async Task SimulateStep3()
			{
				//initial load
				var stepRecord = await DomainQueryProvider.GetMovingHouseProgressMoveInInfo(moveOut.InitiatedFromAccount, moveOut.OtherAccount, MovingHouseType.MoveElectricityAndGas);
				Assert.IsNotNull(stepRecord);
				


				//on leaving step

				var cmd = await GetRecordMovingInProgress(initiatorAccountType, otherAccountType);
				await DomainCommandDispatcher.ExecuteAsync(cmd);

				//on reload
				stepRecord = await DomainQueryProvider.GetMovingHouseProgressMoveInInfo(moveOut.InitiatedFromAccount, moveOut.OtherAccount, MovingHouseType.MoveElectricityAndGas);
				Assert.IsNotNull(stepRecord);
				Assert.AreEqual(cmd.ContactNumber, stepRecord.ContactNumber);
				Assert.AreEqual(cmd.MovingInDate, stepRecord.MovingInDate);
				Assert.AreEqual(cmd.ElectricityMeterReading24HrsOrDayValue, stepRecord.ElectricityMeterReadingDayOr24HrsValue);
				Assert.AreEqual(cmd.ElectricityMeterReadingNightValueOrNshValue, stepRecord.ElectricityMeterReadingNightOrNshValue);
				Assert.AreEqual(cmd.GasMeterReadingValue, stepRecord.GasMeterReadingValue);

			}


		

			async Task TestMoveOut24H()
			{
				if (initiatorAccountType == ClientAccountType.Electricity ||
				    otherAccountType == ClientAccountType.Electricity)
				{
					moveOut =await  GetRecordMovingOutProgress(initiatorAccountType, otherAccountType,electricityMeterReadingNightValue:0);
				}

				await DomainCommandDispatcher.ExecuteAsync(moveOut);
				var actualMoveOut =await DomainQueryProvider.GetMovingHouseProgressMoveOutInfo(moveOut.InitiatedFromAccount,moveOut.OtherAccount, MovingHouseType.MoveElectricityAndGas);
				if (initiatorAccountType == ClientAccountType.Electricity ||
				    otherAccountType == ClientAccountType.Electricity)
				{
					Assert.AreEqual(moveOut.ElectricityMeterReading24HrsOrDayValue,
						actualMoveOut.ElectricityMeterReadingDayOr24HrsValue);
					Assert.AreEqual(0, actualMoveOut.ElectricityMeterReadingNightOrNshValue);
				}

				VerifyCommon(moveOut, actualMoveOut);
			}

			async Task TestMoveOutDayNight()
			{
				if (initiatorAccountType == ClientAccountType.Electricity ||
				    otherAccountType == ClientAccountType.Electricity)
				{
					moveOut=await GetRecordMovingOutProgress(initiatorAccountType, otherAccountType); 
				}

				await DomainCommandDispatcher.ExecuteAsync(moveOut);
				var actualMoveOut =
					await DomainQueryProvider.GetMovingHouseProgressMoveOutInfo(moveOut.InitiatedFromAccount,
						moveOut.OtherAccount, MovingHouseType.MoveElectricityAndGas);
				if (initiatorAccountType == ClientAccountType.Electricity ||
				    otherAccountType == ClientAccountType.Electricity)
				{
					Assert.AreEqual(moveOut.ElectricityMeterReading24HrsOrDayValue,
						actualMoveOut.ElectricityMeterReadingDayOr24HrsValue);
					Assert.AreEqual(moveOut.ElectricityMeterReadingNightOrNshValue,
						actualMoveOut.ElectricityMeterReadingNightOrNshValue);
				}

				VerifyCommon(moveOut, actualMoveOut);
			}
		}
		
		private  async Task<RecordMovingOutProgress> GetRecordMovingOutProgress(ClientAccountType initiatorAccountType, ClientAccountType otherAccountType, 
			int? electricityMeterReading24HrsOrDayValue=null,
			int? electricityMeterReadingNightValue=null,
			int? gasMeterReadingValue=null
			)
		{
			
			var electricityAccount = DomainQueryProvider.GetAccountInfoByAccountNumber("901571221");
			var gasAccount = DomainQueryProvider.GetAccountInfoByAccountNumber("903772573");

			var initiatedFromAccount= initiatorAccountType==ClientAccountType.Electricity? await electricityAccount:await gasAccount;

			if (initiatorAccountType == ClientAccountType.Gas)
				 initiatedFromAccount = await gasAccount;
			else if (initiatorAccountType == ClientAccountType.Electricity)
				initiatedFromAccount = await electricityAccount;
			else
				initiatedFromAccount = null;
			AccountInfo otherAccount;
			if (otherAccountType == ClientAccountType.Gas)
				 otherAccount = await gasAccount;
			else if (otherAccountType == ClientAccountType.Electricity)
				 otherAccount = await electricityAccount;
			else
				 otherAccount = null;

			 electricityMeterReading24HrsOrDayValue= electricityMeterReading24HrsOrDayValue??_fixture.Create<int>();
			electricityMeterReadingNightValue = electricityMeterReadingNightValue ?? _fixture.Create<int>();
			if (!(initiatedFromAccount?.IsElectricityAccount() ?? false) &&
			    !( otherAccount?.IsElectricityAccount() ?? false))
			{
				electricityMeterReading24HrsOrDayValue =  electricityMeterReadingNightValue = 0;
			}

			gasMeterReadingValue = gasMeterReadingValue??_fixture.Create<int>();
			if (!(initiatedFromAccount?.IsGasAccount() ?? false) &&
			    !(otherAccount?.IsGasAccount() ?? false))
			{
				gasMeterReadingValue = 0;
			}
			string lettingPhoneNumber = "0863689898";
			string lettingAgentName = "AgentName1";



			return new RecordMovingOutProgress(
				MovingHouseType.MoveElectricityAndGas,
				initiatedFromAccount,
				otherAccount,
				electricityMeterReading24HrsOrDayValue.Value,
				electricityMeterReadingNightValue.Value,
				gasMeterReadingValue.Value,
				_fixture.Create<bool>(),
				_fixture.Create<bool>(),
				_fixture.Create<bool>(),
				lettingAgentName, 
				lettingPhoneNumber,
				_fixture.Create<bool>(),
				_fixture.Create<DateTime>());
		}

		private static void VerifyCommon(RecordMovingOutProgress expected, MovingHouseInProgressMovingOutInfo actual)
		{
			Assert.AreEqual(expected.InitiatedFromAccount.AccountNumber, actual.InitiatedFromAccountNumber);

			Assert.AreEqual(expected.OtherAccount!=null?expected.OtherAccount.AccountNumber:null, actual.OtherAccountNumber);
			Assert.AreEqual(expected.GasMeterReadingValue, actual.GasMeterReadingValue);
			Assert.AreEqual(expected.IncomingOccupant, actual.IncomingOccupant);
			Assert.AreEqual(expected.InformationCollectionAuthorized, actual.InformationCollectionAuthorized);
			Assert.AreEqual(expected.LettingAgentName, actual.LettingAgentName);
			Assert.AreEqual(expected.LettingPhoneNumber, actual.LettingPhoneNumber);
			Assert.AreEqual(expected.MovingOutDate, actual.MovingOutDate);
			Assert.AreEqual(expected.OccupierDetailsAccepted, actual.OccupierDetailsAccepted);
			Assert.AreEqual(expected.TermsAndConditionsAccepted, actual.TermsAndConditionsAccepted);
		}

		private async Task<RecordMovingInProgress> GetRecordMovingInProgress(ClientAccountType initiatorAccountType, ClientAccountType otherAccountType,
		int? electricityMeterReading24HrsOrDayValue = null,
		int? electricityMeterReadingNightValue = null,
		int? gasMeterReadingValue = null
		)
		{

			var electricityAccount = DomainQueryProvider.GetAccountInfoByAccountNumber("901571221");
			var gasAccount = DomainQueryProvider.GetAccountInfoByAccountNumber("903772573");

			var initiatedFromAccount = initiatorAccountType == ClientAccountType.Electricity ? await electricityAccount : await gasAccount;

			if (initiatorAccountType == ClientAccountType.Gas)
				initiatedFromAccount = await gasAccount;
			else if (initiatorAccountType == ClientAccountType.Electricity)
				initiatedFromAccount = await electricityAccount;
			else
				initiatedFromAccount = null;
			AccountInfo otherAccount;
			if (otherAccountType == ClientAccountType.Gas)
				otherAccount = await gasAccount;
			else if (otherAccountType == ClientAccountType.Electricity)
				otherAccount = await electricityAccount;
			else
				otherAccount = null;

			electricityMeterReading24HrsOrDayValue = electricityMeterReading24HrsOrDayValue ?? _fixture.Create<int>();
			electricityMeterReadingNightValue = electricityMeterReadingNightValue ?? _fixture.Create<int>();
			if (!(initiatedFromAccount?.IsElectricityAccount() ?? false) &&
				!(otherAccount?.IsElectricityAccount() ?? false))
			{
				electricityMeterReading24HrsOrDayValue = electricityMeterReadingNightValue = 0;
			}

			gasMeterReadingValue = gasMeterReadingValue ?? _fixture.Create<int>();
			if (!(initiatedFromAccount?.IsGasAccount() ?? false) &&
				!(otherAccount?.IsGasAccount() ?? false))
			{
				gasMeterReadingValue = 0;
			}
		


			return new RecordMovingInProgress(MovingHouseType.MoveElectricityAndGas,initiatedFromAccount, otherAccount, electricityMeterReading24HrsOrDayValue.Value, electricityMeterReadingNightValue.Value, gasMeterReadingValue.Value
				, _fixture.Create<DateTime>(),"888999888",false,true);
			{

			};
		}
	}
}