using System.Collections.Generic;

namespace S3.Mvc.Core.ViewModels
{
	public interface IErrorsViewModel
	{
		IEnumerable<string> ErrorMessages { get; set; }

	}
}
