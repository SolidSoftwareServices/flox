using Ei.Rp.Mvc.Core.Cryptography.AntiTampering;
using Ei.Rp.Mvc.Core.Cryptography.Urls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ei.Rp.Mvc.Core.Controllers
{

	/// <summary>
	/// Base controller for all the app controllers
	/// </summary>
	[Authorize]
	[AutoValidateAntiforgeryToken]
	[SupportEncryptedUrls]
	[ValidateHiddenInputsAntiTampering]
	[DecryptHiddenInputs]
	public abstract class ControllerBase : Controller{}
}