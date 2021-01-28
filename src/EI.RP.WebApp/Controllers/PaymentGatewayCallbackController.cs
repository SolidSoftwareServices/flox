using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Sap;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.Banking.ProcessPayment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.WebApp.Controllers
{



	[AllowAnonymous]
	public class PaymentGatewayCallbackController : Controller
	{
		private readonly IDomainCommandDispatcher _commandDispatcher;
		private readonly ISapSession _sapSession;
		private readonly IEncryptionService _encryptionService;

		public PaymentGatewayCallbackController(IDomainCommandDispatcher commandDispatcher,ISapSession sapSession,IEncryptionService encryptionService)
		{
			_commandDispatcher = commandDispatcher;
			_sapSession = sapSession;
			_encryptionService = encryptionService;
		}

		[HttpPost]
		public async Task<IActionResult> CollectPaymentResult(CollectPaymentResultRequest request)
		{
			_sapSession.SapCsrf = await _encryptionService.DecryptAsync(request.Csrf.FromBase64UrlEncoded());
			_sapSession.SapJsonCookie = await _encryptionService.DecryptAsync(request.Hash_Value.FromBase64UrlEncoded());

			if (request.Result.StartsWith("2")) request.Result = "2";
			else if (request.Result.StartsWith("3")) request.Result = "3";
			else if (request.Result.StartsWith("5") && request.Result!="509") request.Result = "5";
			var model = new PaymentResultViewModel();
			switch (request.Result)
			{
				case "00":
					model.PaymentWasSuccessful = true;
					model.Message= $"Thank you. We will debit your card for {(EuroMoney)(Convert.ToDecimal(request.Amount)/100)}."
					                        +$"  We have recorded your payment and your payment reference number is {request.Order_Id}.  Please remember it can take up to two working days for this payment to appear on your Electric Ireland account.";
					break;
				case "101":
				case "102":
				case "103":
					model.Message =
						"We regret that your payment has been declined by your bank, please contact them directly for more information.";
					break;
				case "2":
				case "400":
				case "999":
					model.Message =
						"We are experiencing technical difficulties and are unable to process your payment at this time.  Apologies for any inconvenience this causes.  Please try again later.";
					break;
				case "3":
					model.Message =
						"We are having difficulties processing your payment or we are unable to process your payment at this time please try again later";
					break;

				case "509":
					model.Message =
						"The card number entered does not appear to be a valid card. Please check the number entered.";
					break;
				case "5":
					model.Message =
						"We cannot process your payment at this time. Apologies for any inconvenience this causes. Please try again later.";
					break;
				default:
					model.Message = "Sorry.. Unable to make payment. Please try again later.";
					break;
			}

			if (model.PaymentWasSuccessful)
			{
				await _commandDispatcher.ExecuteAsync(MapSuccessCommand(request));
			}
			else
			{
				await _commandDispatcher.ExecuteAsync(MapFailedCommand(request));
			}

			return View(model);
		}

		private ProcessPaymentFailedResultCommand MapFailedCommand(CollectPaymentResultRequest request)
		{
			return new ProcessPaymentFailedResultCommand(
				partner: request.Partner.ToString(),
				paymentCardType: "ELAV",
				payerReference: request.Saved_Payer_Ref,
				userName: request.User_Name,
				accountNumber: request.Account_Number.ToString());
		}
		private ProcessPaymentSuccessfulResultCommand MapSuccessCommand(CollectPaymentResultRequest request)
		{
			return new ProcessPaymentSuccessfulResultCommand(
				partner:request.Partner.ToString(),
				paymentCardType: "ELAV",
				payerReference:request.Saved_Payer_Ref,
				userName:request.User_Name,
				accountNumber:request.Account_Number.ToString()
				);
		}

		public class PaymentResultViewModel
		{
			public bool PaymentWasSuccessful { get; set; } = false;
			public string Message { get; set; } = string.Empty;
		}

		//must match  form defined paymentgateway.cshtml


		public class CollectPaymentResultRequest 
		{

			public string TimeStamp { get; set; }
			public string Result { get; set; }
			// ReSharper disable once InconsistentNaming
			public string Order_Id { get; set; }
			public string Message { get; set; }
			public string AuthCode { get; set; }
			public string PasRef { get; set; }
			public string Sha1Hash { get; set; }
			// ReSharper disable once InconsistentNaming
			public long Account_Number { get; set; }
			public long Partner { get; set; }
			public int Amount { get; set; }
			// ReSharper disable once InconsistentNaming
			public string Hash_Value { get; set; }
			public string Csrf { get; set; }
			// ReSharper disable once InconsistentNaming
			public string Saved_Payer_Ref { get; set; }
			// ReSharper disable once InconsistentNaming
			public string User_Name { get; set; }
			
		}
	}
}