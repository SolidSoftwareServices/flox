using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using NLog;

namespace EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity
{
	internal class PublishBusinessActivityCommandHandler : ICommandHandler<PublishBusinessActivityDomainCommand>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private static readonly IReadOnlyDictionary<PublishBusinessActivityDomainCommand.BusinessActivityType, Activity>
			ActivityDefinitions = new Dictionary<PublishBusinessActivityDomainCommand.BusinessActivityType, Activity>
			{
				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.EditDirectDebit,
					new Activity("DD000001", "DD02")
				},

				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.MeterReading,
					new Activity("METERR01", "ME01")
				},
				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.SubmitDirectDebit,
					new Activity("DD000001", "DD01")

				},
				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.PaymentResultSuccessful,
					new Activity("RESPORTL", "PYMT")
				},
				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.PaymentResultFailed, new Activity(
						"RESPORTL",
						"PYMT")
				},

				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.AddAccount,
					new Activity("RESPORTL", "ACAD")
				},

				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.RefundRequestForDirectDebit,
					new Activity("PYQUDIDE", "PA01")
				},
				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.RefundRequestForNonDirectDebit,
					new Activity("PYQUNODI", "PA01")
				},
				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.EqualizerSetup,
					new Activity("EQUALI01", "EQ01")
				},
				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.UpdateUserContactDetails, new Activity
					(
						"BUP00000",
						"0001"
					)
				},
				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.DeregisterSingleUserBp, new Activity
					(
						"RESPORTL",
						"DREG"
					)
				},
				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.DeregisterMultipleUserBp, new Activity
					(
						"RESPORTL",
						"ACRM"
					)
				},
				{
					PublishBusinessActivityDomainCommand.BusinessActivityType.MoveHouse, new Activity
					(

						"RESPORTL",
						"MHAT"
					)
				}
			};

		private readonly ISapRepositoryOfCrmUmc _dataRepository;

		public PublishBusinessActivityCommandHandler(ISapRepositoryOfCrmUmc dataRepository)
		{
			_dataRepository = dataRepository;
		}

		public async Task ExecuteAsync(PublishBusinessActivityDomainCommand command)
		{
			var activityDefinition = ActivityDefinitions[command.ActivityType];

			var activity = new BusinessActivityDto();
			activity.ActivityID = "";
			activity.InteractionRecordID = "";
			activity.AccountID = command.BusinessPartner;
			activity.Description = command.Subject;
			activity.PremiseID = "";
			activity.BusinessAgreementID = command.AccountNumber;
			activity.Priority = "1";
			activity.ActivityReasonID = activityDefinition.ActivityReasonID;
			activity.ChannelID = "ZON";
			activity.Note = command.Description;
			var reason = new ReasonDto();
			reason.CategoryType = "A1";
			reason.CodeGroup = activityDefinition.CodeGroup;
			reason.Code = activityDefinition.Code;
			activity.Reason = reason;
			activity.DateFrom = null;
			activity.DocumentStatusID =
				command.DocumentStatus == string.Empty ? "E0004" : command.DocumentStatus;
			activity.ProcessType =
				command.ProcessType == string.Empty ? "ZACT" : command.ProcessType;
			activity.PremiseID = string.Empty;
			
			await _dataRepository.Add(activity);
		}

		private class Activity
		{
			public Activity(string codeGroup, string code)
			{
				CodeGroup = codeGroup;
				Code = code;
				ActivityReasonID = $"A1-{CodeGroup}-{Code}";
			}

			public string ActivityReasonID { get; }
			public string CodeGroup { get; }
			public string Code { get; }
		}
	}
}