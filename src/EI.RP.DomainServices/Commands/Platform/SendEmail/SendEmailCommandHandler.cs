using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.CrmUmc.Functions;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MappingValues;
using Newtonsoft.Json;

namespace EI.RP.DomainServices.Commands.Platform.SendEmail
{
    internal class SendEmailCommandHandler : ICommandHandler<SendEmailCommand>
    {
        private readonly ISapRepositoryOfCrmUmc _repository;
        private readonly IEmailSettings _settings;

        public SendEmailCommandHandler(ISapRepositoryOfCrmUmc repository, IEmailSettings settings)
        {
            _repository = repository;
            _settings = settings;
        }


        public async Task ExecuteAsync(SendEmailCommand command)
        {
          
            await SendSapEmail(command);
        }

        private async Task SendSapEmail(SendEmailCommand commandData)
        {
	        var otherSupportedSapTypes = ContactQueryType.AllValues
		        .Where(x => x != ContactQueryType.AddAdditionalAccount).ToArray();
	        var messageContent = string.Concat("Subject:", commandData.Subject, " ", commandData.MessageContent);
	        SendEmailFunction request;
	        if (commandData.QueryType == ContactQueryType.AddAdditionalAccount)
	        {
		        request = await MapSapIssueAddingAccountRequestAsync(commandData, messageContent);
	        }
	        else if (otherSupportedSapTypes.Any(x => x.Equals(commandData.QueryType)))
	        {
		        request = await MapSapAccountQueryRequestAsync(commandData, messageContent);
	        }
	        else
	        {
		        throw new NotSupportedException($"Unsupported type of email: {commandData.QueryType}");
	        }

	        var result = await _repository.ExecuteFunctionWithSingleResult(request);
	        if (!result.Result)
	        {
				throw new DomainException(ResidentialDomainError.SendEmailFailed);
	        }

        }

        private async Task<SendEmailFunction> MapSapIssueAddingAccountRequestAsync(SendEmailCommand commandData, 
            string messageContent)
        {
	        var accountQueryCcEmailAsync = _settings.AccountQueryCcEmailAsync();
	        var residentialOnlineEmailRecipientAsync = _settings.ResidentialOnlineEmailRecipientAsync();
            var request = new SendEmailFunction();
            request.Query.MailFormID = ContactEmailType.SapQueryIssueAddingAccount;
            request.Query.AccountID = commandData.Partner;
            request.Query.ActivityID = string.Empty;

            request.Query.SenderEmail = await residentialOnlineEmailRecipientAsync;
            request.Query.CCEmail1=await accountQueryCcEmailAsync;
            request.Query.ReplyToEmail = string.Empty;
            request.Query.Attribute1Name= "ZACTIVITY-TYPE";
            request.Query.Attribute1Value= commandData.QueryType;
            request.Query.Attribute2Name = "ZACTIVITY-NOTES";
            request.Query.Attribute2Value = messageContent;
            request.Query.Attribute3Name = "ZACTIVITY-BUAG";
            request.Query.Attribute3Value = commandData.AccountNumber;
            request.Query.Attribute4Name = string.Empty;
            request.Query.Attribute4Value = string.Empty;
            request.Query.Attribute5Name = string.Empty;
            request.Query.Attribute5Value = string.Empty;
            return request;
        }

        private async Task<SendEmailFunction> MapSapAccountQueryRequestAsync(SendEmailCommand commandData,
            string messageContent)
        {
	        var accountQueryCcEmailAsync = _settings.AccountQueryCcEmailAsync();
	        var residentialOnlineEmailRecipientAsync = _settings.ResidentialOnlineEmailRecipientAsync();
	        var request = new SendEmailFunction
	        {
		        Query =
		        {
			        MailFormID = ContactEmailType.SapAccountQuery,
			        AccountID = commandData.Partner,
			        ActivityID = string.Empty,
			        SenderEmail = $"Electric Ireland <{await residentialOnlineEmailRecipientAsync}>",
			        CCEmail1 = await accountQueryCcEmailAsync,
			        ReplyToEmail = string.Empty,
			        Attribute1Name = "ZACTIVITY-TYPE",
			        Attribute1Value = commandData.QueryType,
			        Attribute2Name = "ZACTIVITY-NOTES",
			        Attribute2Value = messageContent,
			        Attribute3Name = "ZACTIVITY-BUAG",
			        Attribute3Value = commandData.AccountNumber,
			        Attribute4Name = string.Empty,
			        Attribute4Value = string.Empty,
			        Attribute5Name = string.Empty,
			        Attribute5Value = string.Empty
		        }
	        };

	        return request;
        }

        private class ContactEmailType : TypedStringValue<ContactEmailType>
        {

            [JsonConstructor]
            private ContactEmailType()
            {
            }

            private ContactEmailType(string value) : base(value)
            {
            }
            public static readonly ContactEmailType SapAccountQuery = new ContactEmailType("ROP_ACCOUNTQUERY");
            public static readonly ContactEmailType SapQueryIssueAddingAccount = new ContactEmailType("ROP_ADD_ACCOUNT");
        }
    }
}