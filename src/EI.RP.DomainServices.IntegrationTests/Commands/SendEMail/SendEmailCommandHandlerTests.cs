using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Platform.SendEmail;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Commands.SendEMail
{
	[Explicit]
	[TestFixture]
	class SendEmailCommandHandlerTests : DomainTests
	{
		//1-settings to come from the app settings

		//2-test it here, the email function is failing
		//	it should perform exactly the foillowing
		//Method: POST, RequestUri: 'https://virtdmz144s:4301/roi/sap/CRM_UTILITIES_UMC/SendEmail?AccountID=%271001239663%27&ActivityID=%27%27&MailFormID=%27ROP_ADD_ACCOUNT%27&SenderEmail=%27youronlineaccount@electricireland.ie%27&CCEmail1=%27service@crrtest.esb.ie%27&ReplyToEmail=%27%27&Attribute1Name=%27ZACTIVITY-TYPE%27&Attribute1Value=%270005%2BZECQ%27&Attribute2Name=%27ZACTIVITY-NOTES%27&Attribute2Value=%27Subject%3Aaesfsad asf asdf%27&Attribute3Name=%27ZACTIVITY-BUAG%27&Attribute3Value=%27900021432%27&Attribute4Name=%27%27&Attribute4Value=%27%27&Attribute5Name=%27%27&Attribute5Value

		[Test]
		public async Task CanSendEmail()
		{
			await LoginUser("elecdd33@esb.ie", "Test3333");
			await DomainCommandDispatcher.ExecuteAsync(
				new SendEmailCommand(
				ContactQueryType.AddAdditionalAccount,
				"The subject",
				"The content" ,
				"900021432",
				partner: "1001239663"
				), true);
		}
	}
}
