using System;
using System.Collections.Generic;
using System.Text;

namespace S3.Mvc.Core.ViewModels
{
	public interface IErrorsViewModel
	{
		IEnumerable<string> ErrorMessages { get; set; }

	}
}
