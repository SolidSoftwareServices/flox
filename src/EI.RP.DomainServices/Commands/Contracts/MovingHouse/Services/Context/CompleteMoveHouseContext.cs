using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;

using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.Queries.MovingHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Progress;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.Context
{
	internal class CompleteMoveHouseContext : ICompleteMovingHouseContextFactory
	{
		private readonly IDomainQueryResolver _queryResolver;

		public CompleteMoveHouseContext(IDomainQueryResolver queryResolver)

		{
			_queryResolver = queryResolver;
		}


		public MovingHouseInProgressMovingInInfo MoveInDetails { get; private set; }

		public MovingHouseInProgressNewPRNsInfo NewPrns { get; private set; }

		public MovingHouseInProgressMovingOutInfo MoveOutDetails { get; private set; }


		public ExpandedAccount Gas { get; private set; }

		public ExpandedAccount Electricity { get; private set; }

		public ChecksPointInfo CheckPoints { get; } = new ChecksPointInfo();

		public async Task<CompleteMoveHouseContext> Resolve(MoveHouse commandData)
		{
			var electricityAccountTask = !string.IsNullOrEmpty(commandData.ElectricityAccount)
				? _queryResolver.GetAccountInfoByAccountNumber(commandData.ElectricityAccount, true)
				: Task.FromResult((AccountInfo)null);
			var gasAccountTask = !string.IsNullOrEmpty(commandData.GasAccount)
				? _queryResolver.GetAccountInfoByAccountNumber(commandData.GasAccount, true)
				: Task.FromResult((AccountInfo)null);

			var electricityDevicesTask = !string.IsNullOrEmpty(commandData.ElectricityAccount)
				? _queryResolver.GetDevicesByAccount(commandData.ElectricityAccount, byPassPipeline:true)
				: Task.FromResult((IEnumerable<DeviceInfo>) new DeviceInfo[0]);
			var gasDevicesTask = !string.IsNullOrEmpty(commandData.GasAccount)
				? _queryResolver.GetDevicesByAccount(commandData.GasAccount, byPassPipeline:true)
				: Task.FromResult((IEnumerable<DeviceInfo>) new DeviceInfo[0]);
		

			if (commandData.ElectricityAccount != null)
			{
				Electricity = new ExpandedAccount(commandData.ContractAccountType == ClientAccountType.Electricity,
					await electricityAccountTask, await electricityDevicesTask);

			}
			if (commandData.GasAccount != null)
			{
				Gas = new ExpandedAccount(commandData.ContractAccountType == ClientAccountType.Gas,
					await gasAccountTask, await gasDevicesTask);

			}

			var account1 = await electricityAccountTask ?? await gasAccountTask;
			var account2 = account1 == await electricityAccountTask ? await gasAccountTask : null;

			var moveOutInfo = _queryResolver.GetMovingHouseProgressMoveOutInfo(account1,account2,
				commandData.MoveType, true);
			var movingHouseProgressNewPrnsInfo =
				_queryResolver.GetMovingHouseProgressNewPrnsInfo(account1, account2,
					commandData.MoveType, true);
			var movingInInfo = _queryResolver.GetMovingHouseProgressMoveInInfo(account1, account2, commandData.MoveType, true);

			NewPrns = await movingHouseProgressNewPrnsInfo;
			MoveOutDetails = await moveOutInfo;
			MoveInDetails = await movingInInfo;

			var moveInElectricityPointOfDelivery = NewPrns.NewMprn != null
				? await _queryResolver.GetPointOfDeliveryInfoByPrn(NewPrns.NewMprn, true) : null;
			if (moveInElectricityPointOfDelivery != null)
				NewPointsOfDelivery.Add(ClientAccountType.Electricity, moveInElectricityPointOfDelivery);

			var moveInGasPointOfDelivery = NewPrns.NewGprn != null
				? await _queryResolver.GetPointOfDeliveryInfoByPrn(NewPrns.NewGprn, true) : null;
			if (moveInGasPointOfDelivery != null)
				NewPointsOfDelivery.Add(ClientAccountType.Gas, moveInGasPointOfDelivery);

			return this;
		}
		public Dictionary<ClientAccountType,PointOfDeliveryInfo> NewPointsOfDelivery { get; }=new Dictionary<ClientAccountType, PointOfDeliveryInfo>();

		public IEnumerable<ExpandedAccount> AllAccounts()
		{
			return new[] {Electricity, Gas}.Where(x => x != null);
		}

		public class ExpandedAccount
		{
			public ExpandedAccount(bool isContractAccount, AccountInfo account
				, IEnumerable<DeviceInfo> moveOutDevices
				)
			{
				IsContractAccount = isContractAccount;
				Account = account;
				MoveOutDevices = moveOutDevices;
			}

			public bool IsContractAccount { get; }
			public AccountInfo Account { get; }

			public IEnumerable<DeviceInfo> MoveOutDevices { get; }
		}

		public class ChecksPointInfo
		{
			
			public bool StoreNewIncommingOccupantCompleted_1 { get; set; }
			public bool SubmitMoveOutMeterReadCompleted_2 { get; set; }
			public bool SubmitMoveInMeterReadCompleted_3 { get; set; }
			public bool SetUpNewDirectDebitsCompleted_4 { get; set; }
			public bool SubmitNewContractCompleted_5 { get; set; }
		}


		public bool HasNewMprn() => NewPrns.NewMprn != null && NewPrns.NewMprn.HasValue;
		public bool HasNewGprn() => NewPrns.NewGprn!= null && NewPrns.NewGprn.HasValue;

		
	}
}