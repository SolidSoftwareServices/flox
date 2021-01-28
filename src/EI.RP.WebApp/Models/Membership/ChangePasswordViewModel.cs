using System.Collections.Generic;
using Ei.Rp.Mvc.Core.ViewModels;

namespace EI.RP.WebApp.Models.Membership
{
	public class ChangePasswordViewModel : IErrorsViewModel
	{
        public string RequestId { get; set; }
        public string ActivationKey { get; set; }
		public IEnumerable<string> ErrorMessages { get; set; }
	}
}
