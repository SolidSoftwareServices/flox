using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.CrmUmc.Functions;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Services;
using EI.RP.DomainServices.Commands.Premises.IncommingOccupants;
using EI.RP.DomainServices.Queries.Contracts.CanCloseAccount;
using NLog;
using AccountAddressDependentEmailDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.AccountAddressDependentEmailDto;
using AccountAddressDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.AccountAddressDto;
using AddressInfoDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.AddressInfoDto;

namespace EI.RP.DomainServices.Commands.Contracts.CloseAccounts
{
	internal class CloseAccountCommandHandler : ICommandHandler<CloseAccountsCommand>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDomainCommandDispatcher _commandsDispatcher;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ISapRepositoryOfCrmUmc _crmUmc;
		private readonly ISapRepositoryOfErpUmc _erpUmc;

		private readonly ICloseAccountInfoProvider _infoProvider;

		public CloseAccountCommandHandler(ICloseAccountInfoProvider infoProvider,
			ISapRepositoryOfCrmUmc crmUmc, ISapRepositoryOfErpUmc erpUmc, IDomainCommandDispatcher commandsDispatcher,IDomainQueryResolver queryResolver)
		{
			_infoProvider = infoProvider;
			_crmUmc = crmUmc;
			_erpUmc = erpUmc;
			_commandsDispatcher = commandsDispatcher;
			_queryResolver = queryResolver;
		}

		public async Task ExecuteAsync(CloseAccountsCommand command)
		{
			command.Context = await _infoProvider.Resolve(command);
			
			await Task.WhenAll(SubmitOccupantDetails(command)
				, SubmitCloseMeterReads(command));
			await ExecuteMoveOut(command);
		}

		private async Task<AccountsCloseableInfo> ResolveCloseAccountsInfo(CloseAccountsCommand commandData)
		{
			var closeAccounts = await _queryResolver.CanCloseAccounts(
				commandData.MeterReadingElectricityAccount?.AccountNumber,
				commandData.MeterReadingGasAccount?.AccountNumber,
				commandData.MoveOutDate, 
				true);

			if (!closeAccounts.CanClose)
			{
				throw new DomainException(ResidentialDomainError.AccountCannotBeClosed, message:closeAccounts.ReasonCannotClose);
			}

			return closeAccounts;

		}

		private async Task ExecuteMoveOut(CloseAccountsCommand commandData)
		{

			var function = new ZMoveOutFunction
			{
				Query =
				{
					MoveOutDate = commandData.MoveOutDate
				}
			};

			var query = function.Query;

			var electricityAccount = commandData.Context.ElectricityAccount;
			if (electricityAccount != null)
			{
				query.ElecContractID = electricityAccount.ContractId;
				query.ElecMOMRReg1 = commandData.MeterReadingElectricityAccount.MeterReading24Hrs?? commandData.MeterReadingElectricityAccount.MeterReadingDay??0;
				query.ElecMOMRReg2 = commandData.MeterReadingElectricityAccount.MeterReadingNight??0;
				
			}

			var gasAccount = commandData.Context.GasAccount;
			if (gasAccount != null)
			{
				query.GasContractID = gasAccount.ContractId;
				query.GasMOMRReg1= commandData.MeterReadingGasAccount.MeterReading??0;
			}


			await _crmUmc.ExecuteFunctionWithManyResults(function);
		}

		private async Task SubmitCloseMeterReads(CloseAccountsCommand commandData)
		{
			//TODO: MOVE CLOSER TO USAGE
			var getMoveOutInfo = ResolveCloseAccountsInfo(commandData);
			//TODO: to command
			var devices = await GetDevices();
			var moveOutInfo = await getMoveOutInfo;

			var tasks = devices.Select(SubmitDeviceMeterReads).ToArray();
			try
			{
				await Task.WhenAll(tasks);
			}
			catch (DomainException ex)
			{
				//second attempt from the user
				if (!ex.DomainError.Equals(ResidentialDomainError.DataAlreadyReleased)) throw;
			}

			MeterReadingResultDto MapNewMeterReading(DeviceDto device, RegisterToReadDto registerRead)
			{
				var newMr = new MeterReadingResultDto();

				newMr.DeviceID = registerRead.DeviceID;
				newMr.FmoRequired = string.Empty;
				var meterReadingResult = device.MeterReadingResults.FirstOrDefault(x =>
					x.SerialNumber == registerRead.SerialNumber
					&& x.DeviceID == registerRead.DeviceID
					&& x.RegisterID == registerRead.RegisterID);
				newMr.RegisterID = registerRead.RegisterID;
				newMr.MeterReadingNoteID = string.Empty;
				newMr.ReadingDateTime = device.DivisionID == DivisionType.Gas
					? moveOutInfo.GasEffectiveClosingDate
					: moveOutInfo.ElectricityEffectiveClosingDate;

				MapElectricity();

				MapGas();


				newMr.Vertrag = meterReadingResult != null ? meterReadingResult.Vertrag : string.Empty;

				return newMr;

				void MapGas()
				{
					if (device.DivisionID != DivisionType.Gas) return;
					if (registerRead.RegisterID == "001")
					{
						var read = Convert.ToDecimal(commandData.MeterReadingGasAccount.MeterReading);
						var lastRecordedRead = device.MeterReadingResults.Where(x =>
								x.RegisterID == "001" &&
								x.MeterReadingCategoryID != MeterReadingCategoryType.EstimatedA)
							.Select(x => x.ReadingResultAsDecimal()).LastOrDefault();

						newMr.SetReadingResult(read < lastRecordedRead ? lastRecordedRead + 1 : read);
					}
					else
					{
						newMr.SetReadingResult(0M);
					}

					newMr.Lcpe = moveOutInfo.UpfLcpe;
				}

				void MapElectricity()
				{
					if (device.DivisionID != DivisionType.Electricity) return;

					if (registerRead.RegisterType.Description == MeterType.Electricity24h)
						newMr.SetReadingResult( commandData.MeterReadingElectricityAccount.MeterReading24Hrs.Value);
					else if (registerRead.RegisterType.Description == MeterType.ElectricityDay)
						newMr.SetReadingResult(commandData.MeterReadingElectricityAccount.MeterReadingDay.Value);
					else if (registerRead.RegisterType.Description == MeterType.ElectricityNight)
						newMr.SetReadingResult(commandData.MeterReadingElectricityAccount.MeterReadingNight.Value);

					newMr.Lcpe = moveOutInfo.UpfLcpe;
				}
			}

			async Task<IEnumerable<DeviceDto>> GetDevices()
			{
				var getDevicesTasks = new List<Task<IEnumerable<DeviceDto>>>();
				foreach (var accountInfo in commandData.Context.AllAccounts())
					getDevicesTasks.Add(_erpUmc.NewQuery<ContractDto>()
						.Key(accountInfo.ContractId)
						.NavigateTo<DeviceDto>(x => x.Devices)
						.Expand(x => x.RegistersToRead)
						.Expand(x => x.RegistersToRead[0].RegisterType)
						.Expand(x => x.MeterReadingResults)
						.GetMany()
					);

				return (await Task.WhenAll(getDevicesTasks.ToArray())).SelectMany(x => x).ToArray();
			}

			Task SubmitDeviceMeterReads(DeviceDto device)
			{
				var registerReads = device.DivisionID == DivisionType.Gas
					? device.RegistersToRead.Where(x => x.RegisterID == "001").ToArray()
					: device.RegistersToRead.ToArray();

				MeterReadingResultDto dto = null;
				foreach (var registerRead in registerReads)
				{
					var newMeterReading = MapNewMeterReading(device, registerRead);

					if (dto == null)
						dto = newMeterReading;
					else
						dto.DependentMeterReadingResults.Add(newMeterReading);
				}


				return _erpUmc.Add(dto);
			}
		}

		private async Task SubmitOccupantDetails(CloseAccountsCommand commandData)
		{
			var setAddressToBlankTask = SetAccountAddressesToBlank();
			var submitAddressTask = SubmitAddress();
			var submitDependentEmailTask = SubmitAccountAddressDependentEmail();
			var setOccupantDetailsTask = SetOccupantDetails();

			await Task.WhenAll(setAddressToBlankTask, submitAddressTask, submitDependentEmailTask,
				setOccupantDetailsTask);

			async Task SetOccupantDetails()
			{
				if (commandData.MoveOutIncommingOccupant == null ||
				    !commandData.MoveOutIncommingOccupant.IncomingOccupant) return;
				var tasks = new List<Task>();
				foreach (var account in commandData.Context.AllAccounts())
					tasks.Add(_commandsDispatcher.ExecuteAsync(
						new NotifyNewIncommingOccupant(account.AccountNumber,
							commandData.MoveOutIncommingOccupant.LettingAgentName,
							commandData.MoveOutIncommingOccupant.LettingPhoneNumber)
						, true));

				await Task.WhenAll(tasks.ToArray());
			}

			async Task SetAccountAddressesToBlank()
			{
				var getContractTasks = new List<Task<ContractAccountDto>>();
				foreach (var account in commandData.Context.AllAccounts())
					getContractTasks.Add(_erpUmc.NewQuery<ContractAccountDto>()
						.Key(account.AccountNumber)
						.GetOne());

				var updateTasks = new List<Task>();
				foreach (var getContractTask in getContractTasks)
				{
					var contract = await getContractTask;
					contract.AccountAddressID = string.Empty;
					updateTasks.Add(_erpUmc.Update(contract));
					Logger.Debug(() => $"Address set to blank for account number:{contract.AccountID}");
				}

				await Task.WhenAll(updateTasks);
			}

			async Task SubmitAccountAddressDependentEmail()
			{
				try
				{
					var businessAgreement = (await _crmUmc.NewQuery<BusinessAgreementDto>()
						.Key(commandData.Context.ContractAccount.AccountNumber)
						.Expand(x => x.Account.AccountAddresses)
						.Expand(x => x.Account.StandardAccountAddress.AccountAddressDependentEmails)
						.GetMany()).FirstOrDefault();


					var email = new AccountAddressDependentEmailDto();
					email.AccountID = commandData.Context.ContractAccount.Partner;
					email.AddressID = businessAgreement?.Account.AccountAddresses.FirstOrDefault()?.AddressID ??
					                  string.Empty;
					email.HomeFlag = true;
					email.StandardFlag = true;
					email.Email = businessAgreement?.Account.StandardAccountAddress
						              .AccountAddressDependentEmails.FirstOrDefault(x => x.StandardFlag)?.Email ??
					              string.Empty;

					email.SequenceNo = string.Empty;

					await _crmUmc.Add(email);

					Logger.Debug("submitDependentEmail successfully executed ");
				}
				catch (Exception ex)
				{
					//this is as per the existing code
					Logger.Error(() => $"Error in submitDependentEmail with exception {ex}");
				}
			}

			async Task SubmitAddress()
			{
				var dto = MapAddress();
				await _crmUmc.AddThenGet(dto);
				Logger.Debug(() => "Forwarding Address Saved successfully.");

				AccountAddressDto MapAddress()
				{
					var address = commandData.AddressInfo;
					var accountAddress = new AccountAddressDto
					{
						AddressInfo = new AddressInfoDto()
					};
					accountAddress.AddressID = string.Empty;
					var addressInfo = accountAddress.AddressInfo;
					addressInfo.StandardFlag = AddressFlag.StandardFlag;
					
					addressInfo.Street =
						address.Street == null ? string.Empty : address.Street.ToUpper();
					addressInfo.City = address.City == null ? string.Empty : address.City.ToUpper();

					addressInfo.PostalCode =
						address.PostalCode == null ? string.Empty : address.PostalCode.ToUpper();

					addressInfo.CountryID = address.Country == null ? "IE" : address.Country.ToUpper();
					addressInfo.AddressLine1 =
						address.AddressLine1 == null ? string.Empty : address.AddressLine1.ToUpper();
					addressInfo.AddressLine2 =
						address.AddressLine2 == null ? string.Empty : address.AddressLine2.ToUpper();
					addressInfo.AddressLine4 =
						address.AddressLine4 == null ? string.Empty : address.AddressLine4.ToUpper();
					addressInfo.AddressLine5 =
						address.AddressLine5 == null ? string.Empty : address.AddressLine5.ToUpper();
					addressInfo.POBoxPostalCode = address.POBoxPostalCode == null
						? string.Empty
						: address.POBoxPostalCode.ToUpper();
					addressInfo.POBox = address.POBox == null ? string.Empty : address.POBox.ToUpper();

					addressInfo.HouseNo =
						address.HouseNo == null ? string.Empty : address.HouseNo.ToUpper();
					addressInfo.Region =
						address.Region == null ? string.Empty : address.Region.ToUpper();
					addressInfo.District =
						address.District == null ? string.Empty : address.District.ToUpper();
					addressInfo.Building = string.Empty;
					addressInfo.CareOf = string.Empty;
					addressInfo.CountryName = string.Empty;
					addressInfo.Floor = string.Empty;
					addressInfo.HouseSupplement = string.Empty;
					addressInfo.RegionName = string.Empty;
					addressInfo.TimeZone = string.Empty;
					addressInfo.TaxJurisdictionCode = string.Empty;
					addressInfo.ShortForm = string.Empty;
					addressInfo.LanguageID = string.Empty;
					addressInfo.RoomNo = string.Empty;

					accountAddress.AccountID = commandData.Context.ContractAccount.Partner;
					return accountAddress;
				}
			}
		}
	}
}