using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace S3.UiFlows.Mvc.Views
{
	internal class FlowsViewEngine : IViewEngine
	{
		private readonly IRazorViewEngine _razor;



		public FlowsViewEngine(IRazorViewEngine razor)
		{
			_razor = razor;
		}

		public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
		{
			if (viewName.StartsWith("Components", StringComparison.InvariantCultureIgnoreCase))
			{
				const char separator = '/';
				viewName = string.Join(separator, viewName.Split(separator).Skip(2));
			}

			var result = _razor.FindView(context, viewName, isMainPage);

			return result;
		}

		public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
		{
			var result = _razor.GetView(executingFilePath, viewPath, isMainPage);

			return result;
		}
	}
}