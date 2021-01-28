using EI.RP.WebApp.Models.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.WebApp.Controllers
{
	public class StaticController : Ei.Rp.Mvc.Core.Controllers.ControllerBase
	{
		[AllowAnonymous]
		[Route("Help")]
		public IActionResult Help()
		{
			return View(new StaticModel());
		}

		[AllowAnonymous]
		[Route("TermsAndConditions")]
		public IActionResult TermsAndConditions()
		{
			return View(new StaticModel());
		}

		[AllowAnonymous]
		[Route("Disclaimer")]
		public IActionResult Disclaimer()
		{
			return View(new StaticModel());
		}

		[AllowAnonymous]
		[Route("PrivacyNotice")]
		public IActionResult PrivacyNotice()
		{
			return View(new StaticModel());
		}

		[AllowAnonymous]
		[Route("ContactUs")]
		public IActionResult ContactUs()
		{
			return View(new StaticModel());
		}
	}
}
