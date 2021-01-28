using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Serialization;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.ActivityOperations
{
	internal class MoveHomeActivityPublisher : IMoveHomeActivityPublisher
	{
		private readonly IDomainCommandDispatcher _commandsDispatcher;

		public MoveHomeActivityPublisher(IDomainCommandDispatcher commandsDispatcher)
		{
			_commandsDispatcher = commandsDispatcher;
		}

		public async Task SubmitActivityError(MoveHouse commandData, DomainException ex)
		{
			var mainAccount = commandData.Context.Electricity?.Account ?? commandData.Context.Gas.Account;
			if (mainAccount == null)
			{
				throw new InvalidOperationException("Could not resolve the main account");
			}
			var activity = new PublishBusinessActivityDomainCommand(
				PublishBusinessActivityDomainCommand.BusinessActivityType.MoveHouse,
				mainAccount?.Partner??"-1",
				mainAccount.AccountNumber,
				description: await ResolveActivityNotes(commandData, ex)
			);
			await _commandsDispatcher.ExecuteAsync(activity, true);
		}

		private async Task<string> ResolveActivityNotes(MoveHouse commandData, DomainException domainException)
		{
			var context = commandData.Context;

			var model = new MoveHouseInteractionRecordModel();
			AddModelRootInfo();

			AddMoveOutReadings();
			AddLettingAgent();

			await Task.WhenAll(AddMoveInElectricityReading(), AddMoveInGasReading());

			AddPaymentMethodType();
			return model.ToJson().Replace(',', '\n');


			void AddModelRootInfo()
			{
				model.Process = $"Moving {commandData.MoveType}";
				model.Error_Reason = domainException.DomainError.ErrorMessage;
				model.Moveout_Date = context.MoveOutDetails.MovingOutDate.ToString("yyyy-MM-dd");
				model.Movein_Date = context.MoveInDetails.MovingInDate?.ToString("yyyy-MM-dd") ?? string.Empty;
				model.Phone_Number = string.IsNullOrEmpty(context.MoveInDetails.ContactNumber)
					? string.Empty
					: context.MoveInDetails.ContactNumber;

				model.Electricity_Contract_Account = context.Electricity?.Account.AccountNumber ?? string.Empty;
			}

			void AddLettingAgent()
			{
				var inocc = new IncomingOcc_Landlord_LettingAgent();
				inocc.Name = commandData.Context.MoveOutDetails.LettingAgentName ?? string.Empty;
				inocc.PhoneNo = commandData.Context.MoveOutDetails.LettingPhoneNumber ?? string.Empty;
				model.IncomingOcc_Landlord_LettingAgent = inocc;
			}

			void AddMoveOutReadings()
			{
				var deviceRegisterInfos = context.Electricity?.MoveOutDevices.SelectMany(x => x.Registers).ToArray() ??
				                          new DeviceRegisterInfo[0];
				if (deviceRegisterInfos.Any())
				{
					var moListe = new List<Electricity_MoveOut_Reading>();
					foreach (var registerInfo in deviceRegisterInfos)
					{
						var mr = new Electricity_MoveOut_Reading();
						mr.DeviceID = registerInfo.DeviceId;
						mr.RegisterNo = registerInfo.MeterType;
						mr.Reading = context.MoveOutDetails.ElectricityMeterReadingDayOr24HrsValue.ToString();
						moListe.Add(mr);
					}

					model.Electricity_MoveOut_Reading = moListe;
				}

				deviceRegisterInfos =
					context.Gas?.MoveOutDevices.SelectMany(x => x.Registers).ToArray() ?? new DeviceRegisterInfo[0];
				if (deviceRegisterInfos.Any())
				{
					var moListg = new List<Gas_MoveOut_Reading>();
					foreach (var registerInfo in deviceRegisterInfos)
					{
						var mr = new Gas_MoveOut_Reading();
						mr.DeviceID = registerInfo.DeviceId;

						mr.Reading = context.MoveOutDetails.GasMeterReadingValue.ToString();
						moListg.Add(mr);
					}

					model.Gas_MoveOut_Reading = moListg;
				}
			}

			async Task AddMoveInElectricityReading()
			{
				var newMprn = commandData.Context.NewPrns.NewMprn;
				model.MoveIn_MPRN = newMprn?.ToString() ?? string.Empty;
				if (newMprn != null && newMprn.HasValue)
				{
					var miListe = new List<Electricity_MoveIn_Reading>();
					//TODO:
					//var device = await _queryResolver.GetDeviceInfo(newMprn);
					//foreach (var register in device.Registers)
					//{

					//	var mr = new Electricity_MoveIn_Reading();
					//	mr.DeviceID = register.DeviceId;
					//	mr.RegisterNo = register.MeterType;
					//	mr.Reading = register.MeterType.IsOneOf(MeterType.Electricity24h, MeterType.ElectricityDay)
					//		? context.MoveOutDetails.ElectricityMeterReadingDayOr24HrsValue.ToString()
					//		: context.MoveOutDetails.ElectricityMeterReadingNightValue.ToString();

					//	miListe.Add(mr);

					//}

					model.Electricity_MoveIn_Reading = miListe;
				}
			}

			async Task AddMoveInGasReading()
			{
				var newGprn = commandData.Context.NewPrns.NewGprn;
				model.MoveIn_GPRN = newGprn?.ToString() ?? string.Empty;
				if (newGprn != null && newGprn.HasValue)
				{
					var miListg = new List<Gas_MoveIn_Reading>();
					//TODO:
					//var device = await _queryResolver.GetDeviceInfo(newGprn);
					//foreach (var register in device.Registers)
					//{
					//	var mr = new Gas_MoveIn_Reading();
					//	mr.DeviceID = register.DeviceId;
					//	mr.Reading = context.MoveOutDetails.GasMeterReadingValue.ToString();
					//	miListg.Add(mr);
					//}

					model.Gas_MoveIn_Reading = miListg;
				}
			}

			void AddPaymentMethodType()
			{
				model.PaymentMethod_ElectricityMI =
					commandData.CommandsToExecute.SingleOrDefault(x => x.AccountType == ClientAccountType.Electricity)
						?.PaymentMethodType ?? string.Empty;

				model.PaymentMethod_GasMI =
					commandData.CommandsToExecute.SingleOrDefault(x => x.AccountType == ClientAccountType.Gas)
						?.PaymentMethodType ?? string.Empty;
			}
		}

		public class MoveHouseInteractionRecordModel
		{
			public string Process { get; set; }
			public string Error_Reason { get; set; }
			public string Moveout_Date { get; set; }
			public string Movein_Date { get; set; }
			public string Phone_Number { get; set; }
			public string Electricity_Contract_Account { get; set; }
			public string Gas_Contract_Account { get; set; }
			public IncomingOcc_Landlord_LettingAgent IncomingOcc_Landlord_LettingAgent { get; set; }
			public List<Gas_MoveOut_Reading> Gas_MoveOut_Reading { get; set; }
			public List<Electricity_MoveOut_Reading> Electricity_MoveOut_Reading { get; set; }
			public List<Gas_MoveIn_Reading> Gas_MoveIn_Reading { get; set; }
			public List<Electricity_MoveIn_Reading> Electricity_MoveIn_Reading { get; set; }
			public string MoveIn_GPRN { get; set; }
			public string MoveIn_MPRN { get; set; }
			public string PaymentMethod_ElectricityMI { get; set; }
			public string PaymentMethod_GasMI { get; set; }
			public string Forwarding_Address_type { get; set; }
			public For_ROI For_ROI { get; set; }
			public For_PO_Box For_PO_Box { get; set; }
			public For_Non_ROI For_Non_ROI { get; set; }
		}

		public class IncomingOcc_Landlord_LettingAgent
		{
			public string Name { get; set; }
			public string PhoneNo { get; set; }
		}

		public class Gas_MoveOut_Reading
		{
			public string DeviceID { get; set; }
			public string Reading { get; set; }
		}

		public class Electricity_MoveOut_Reading
		{
			public string DeviceID { get; set; }
			public string Reading { get; set; }
			public string RegisterNo { get; set; }
		}

		public class Gas_MoveIn_Reading
		{
			public string DeviceID { get; set; }
			public string Reading { get; set; }
		}

		public class Electricity_MoveIn_Reading
		{
			public string DeviceID { get; set; }
			public string Reading { get; set; }
			public string RegisterNo { get; set; }
		}

		public class For_ROI
		{
			public string AddressLine1 { get; set; }
			public string HouseNo { get; set; }
			public string Street_Name { get; set; }
			public string AddressLine2 { get; set; }
			public string Town { get; set; }
			public string County { get; set; }
			public string PostCode { get; set; }
		}

		public class For_PO_Box
		{
			public string PO_Box_No { get; set; }
			public string PO_Box_Post_Code { get; set; }
			public string PO_Box_County { get; set; }
			public string PO_Box_Country { get; set; }
		}

		public class For_Non_ROI
		{
			public string Country { get; set; }
			public string AddressLine1 { get; set; }
			public string HouseNo { get; set; }
			public string Street_Name { get; set; }
			public string AddressLine2 { get; set; }
			public string Town { get; set; }
			public string County { get; set; }
			public string PostCode { get; set; }
		}
	}
}