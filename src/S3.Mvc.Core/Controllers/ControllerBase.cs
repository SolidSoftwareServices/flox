using S3.Mvc.Core.Cryptography.AntiTampering;
using S3.Mvc.Core.Cryptography.Urls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace S3.Mvc.Core.Controllers
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