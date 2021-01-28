using System.Linq;
using System.Threading.Tasks;
using Autofac;
using EI.RP.CoreServices.Serialization;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.ActivityOperations;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.ChangeBusinessAgreementOperations;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.Context;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.DirectDebitOperations;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.MoveOutOperationsHandler;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Commands.MovingHouse
{
	[Explicit]
	[TestFixture]
	public class MoveHouseCommandTests : DomainTests
	{

		private static async Task<MoveHouse> LoginAndGetCommand()
		{
			await Login();
			
			var cmd = new MoveHouse("904756590",
				"904756591",
				MovingHouseType.MoveElectricityAndGas,
				new []
				{
					new SetUpDirectDebitDomainCommand(
						"904756590",
						"",
						"IE29AIBK93115212345678",
						"IE30AIBK93338420784094",
						"1003658901",
						ClientAccountType.Electricity,
						PaymentMethodType.DirectDebit
					),
					new SetUpDirectDebitDomainCommand(
						"904756591",
						"",
						null,
						"IE25BOFI90135128358573",
						"1003658901",
						ClientAccountType.Gas,
						PaymentMethodType.DirectDebit
					)
				},
				ClientAccountType.Gas
			);
			cmd.Context = await AssemblySetUp.Container.Value.Resolve<ICompleteMovingHouseContextFactory>().Resolve(cmd);
			return cmd;
		}

		private static async Task Login()
		{
			//await LoginUser("ElecDD20@esb.ie", "Test3333");
			await LoginUser("MEG002@test.ie", "Test3333");

		}

		[Test]
		public async Task CanExecute()
		{
			var cmd = await LoginAndGetCommand();

			await DomainCommandDispatcher.ExecuteAsync(cmd, true);
		}
		//[Test]
		//public async Task CanExecuteMoveOutOperations1()
		//{
		//	var cmd = await LoginAndGetCommand();


		//	var sut = AssemblySetUp.Container.Resolve<IMoveHomeOutOperationsHandler>();

		//	await sut.StoreIncommingOccupant(cmd);
		//	var moveOut = await sut.SubmitMoveOutMeterReadings(cmd);

		//}
		//[Test]
		//public async Task CanExecuteMoveIn2()
		//{
		//	var cmd = await LoginAndGetCommand();

		//	var sut = AssemblySetUp.Container.Resolve<IMoveInOperationsHandler>();

		//	await sut.SubmitMoveInMeterReads(cmd);


		//}

		//[Test]
		//public async Task CanSubmitDirectDebitOperations3()
		//{
		//	var cmd = await LoginAndGetCommand();

		//	var sut = AssemblySetUp.Container.Resolve<IMoveHomeSubmitDirectDebitOperationsHandler>();

		//	await sut.Submit(cmd);


		//}

		//[Test]
		//public async Task CanChangeBusinessAgreement4()
		//{
		//	var cmd = await LoginAndGetCommand();

		//	var sut = AssemblySetUp.Container.Resolve<IMoveHomeChangeBusinessAgreementOperationsHandler>();

		//	await sut.SetNewAddressAndBusinessAgreementChanges(cmd);


		//}

		//[Test]
		//public async Task CanCreateNewContract5()
		//{
		//	var cmd = await LoginAndGetCommand();

		//	var sut = AssemblySetUp.Container.Resolve<IMoveHomeNewContractOperationsHandler>();

		//	await sut.CreateNewContract(cmd);


		//}

		//[Test]
		//public async Task QueryNewAccount6()
		//{
		//	await Login();

		//	var account1 =
		//		await DomainQueryProvider.GetAccountInfoByPrn((GasPointReferenceNumber)"1052119", true);

		//	var account2 =
		//		await DomainQueryProvider.GetAccountInfoByAccountNumber("950436366", true);

		//}
	}
}