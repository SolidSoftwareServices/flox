using System.Diagnostics;
using EI.RP.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.WebApp.Controllers
{
	public class ErrorController : Ei.Rp.Mvc.Core.Controllers.ControllerBase
	{
		[AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
	        return View(new ErrorViewModel
	        {
				ShowBackToLogin = false,
		        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
	        });
        }

		[AllowAnonymous]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult NotAuthorized()
		{
			return View("NotAuthorized");
		}
	}

}