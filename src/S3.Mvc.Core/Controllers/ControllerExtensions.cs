using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S3.Mvc.Core.Cryptography.Urls;
using S3.Mvc.Core.ViewModels;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Newtonsoft.Json.Linq;

namespace S3.Mvc.Core.Controllers
{

	public static class ControllerExtensions
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        

		public static string GetNameWithoutSuffix(this Type controllerType)
		{
			const string suffix = "Controller";
			Debug.Assert(controllerType.IsSubclassOf(typeof(Controller)));
			return controllerType.Name.Replace(suffix, string.Empty);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetController"></param>
		/// <param name="controllerActionHandler"></param>
		/// <param name="onErrorRedirectToActionName">Action to redirect on invalid model. If not specified returns the model with errors</param>
		/// <param name="storeErrorsOn">viewmodel to use in the redirection in case of errors</param>
		/// <param name="onCustomErrorHandling">do not specify if you dont need to handle specific errors. It is called once per domain exception</param>
		/// <returns></returns>
		public static async Task<IActionResult> HandleActionAsync(this Controller targetController,
			Func<Task<IActionResult>> controllerActionHandler,
			string onErrorRedirectToActionName = null,
			IErrorsViewModel storeErrorsOn = null,
			Action<Exception> onCustomErrorHandling = null)
		{
			var errorRedirectToActionName = onErrorRedirectToActionName != null
				? async m => (IActionResult) targetController.RedirectToAction(onErrorRedirectToActionName,
					await m.ToValidRouteValueAsync(targetController.HttpContext))
				: (Func<IErrorsViewModel, Task<IActionResult>>) null;
			return await targetController.HandleActionAsync(controllerActionHandler, errorRedirectToActionName,
				storeErrorsOn,
				onCustomErrorHandling);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetController"></param>
		/// <param name="controllerActionHandler"></param>
		/// <param name="onErrorUseThisResultProvider">Action to provide result on invalid model. If not specified returns the model with errors</param>
		/// <param name="storeErrorsOn">viewmodel to use in the redirection in case of errors</param>
		/// <param name="onCustomErrorHandling">do not specify if you dont need to handle specific errors. It is called once per domain exception</param>
		/// <returns></returns>
		public static async Task<IActionResult> HandleActionAsync(this Controller targetController,
			Func<Task<IActionResult>> controllerActionHandler,
			Func<IErrorsViewModel, Task<IActionResult>> onErrorUseThisResultProvider,
			IErrorsViewModel storeErrorsOn = null,
			Action<Exception> onCustomErrorHandling = null)
		{
			if (controllerActionHandler == null) throw new ArgumentNullException(nameof(controllerActionHandler));
			return await HandleActionAsync(new HandleActionRequest(targetController, controllerActionHandler, onErrorUseThisResultProvider, storeErrorsOn, onCustomErrorHandling));
		}

		private class HandleActionRequest
		{

			public HandleActionRequest(Controller targetController, Func<Task<IActionResult>> controllerActionHandler, Func<IErrorsViewModel, Task<IActionResult>> onErrorUseThisResultProvider, IErrorsViewModel storeErrorsOn, Action<Exception> onCustomErrorHandling)
			{
				TargetController = targetController ?? throw new ArgumentNullException(nameof(targetController));
				ControllerActionHandler = controllerActionHandler ?? throw new ArgumentNullException(nameof(controllerActionHandler));
				OnErrorUseThisResultProvider = onErrorUseThisResultProvider;
				StoreErrorsOn = storeErrorsOn;
				OnCustomErrorHandling = onCustomErrorHandling;
			}

			public Controller TargetController { get; }
			public Func<Task<IActionResult>> ControllerActionHandler { get; }
			public Func<IErrorsViewModel, Task<IActionResult>> OnErrorUseThisResultProvider { get; }

			public IErrorsViewModel StoreErrorsOn { get; }
			public Action<Exception> OnCustomErrorHandling { get; }
		}

	
		private static async Task<IActionResult> HandleActionAsync(HandleActionRequest request)
        {
         

            if (request.TargetController.Request != null)
                Logger.Debug(() => $"Start Handling: {request.TargetController.Request.GetDisplayUrl()}");

            IActionResult result = null;
            try
            {
	          
	            if (request.OnErrorUseThisResultProvider==null || request.TargetController.ModelState.IsValid)
	            {
		            result = await request.ControllerActionHandler();
	            }
	            else
	            {
		            result = await BuildInvalidResult(request);
	            }
            }
            //Domain only throws Aggregate exceptions
            catch (AggregateException domainExceptions)
            {
	            result = await HandleDomainException(request,domainExceptions);
            }
           
            if (request.TargetController.Request != null)
                Logger.Debug(() => $"Completed Handling: {request.TargetController.Request.GetDisplayUrl()}");
            return result;
        }

        private static async Task<IActionResult> BuildInvalidResult(HandleActionRequest request)
        {
	       
			IActionResult result;
            if (request.StoreErrorsOn!= null)
            {
                var errors = new List<string>();
                foreach (var modelState in request.TargetController.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                request.StoreErrorsOn.ErrorMessages = errors;

            }

            result =await request.OnErrorUseThisResultProvider(request.StoreErrorsOn);
            return result;
        }

        private static async Task<IActionResult> HandleDomainException(HandleActionRequest request,
	        AggregateException domainExceptions)
        {
            //it is most likely an error because the UI validation passed but something happened when processing the request... 3rd party service down,etc...


			LogError();

            if (domainExceptions.InnerExceptions.Any(x => !(x is Exception) && !(x is ValidationException)))
            {
                //this is an unhandled exception--> bubble up and let the general exception mechanism take over(show error page)
                throw new AggregateException(domainExceptions.InnerExceptions.Where(x =>
                    !(x is Exception) && !(x is ValidationException)));
            }

            foreach (var ex in domainExceptions.InnerExceptions.Where(x => x is Exception)
                .Cast<Exception>())
            {
                if (request.OnCustomErrorHandling != null)
                {
                    request.OnCustomErrorHandling(ex);
                }
                else
                {
                    //TODO: Add token replacement mechanims and keep this code
                    

                    //key= "" --> adds to the validation summary
                    request.TargetController.ModelState.AddModelError(string.Empty, ex.Message);
                }

            }

            foreach (var ex in domainExceptions.InnerExceptions.Where(x => x is ValidationException)
                .Cast<ValidationException>())
            {
				//key= "" --> adds to the validation summary
				request.TargetController.ModelState.AddModelError(string.Empty, ex.Message);

            }

            return await BuildInvalidResult(request);

            void LogError()
            {
	            var sb = new StringBuilder();
	            foreach (var exception in domainExceptions.InnerExceptions)
	            {
		            if (exception is Exception)
		            {
			            var ex = (Exception) exception;
			            sb.AppendLine($"{exception}");
		            }
		            else
		            {
			            sb.AppendLine(exception.ToString());
		            }
	            }
	            Logger.Error(() => sb.ToString());

			}
		}
    }


}