using System;
using System.Collections.Generic;
using System.Text;

namespace Ei.Rp.Mvc.Core.ViewModels
{
	public interface IErrorsViewModel
	{
		IEnumerable<string> ErrorMessages { get; set; }

	}
}
