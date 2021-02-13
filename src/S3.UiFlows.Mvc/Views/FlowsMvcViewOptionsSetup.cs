using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace S3.UiFlows.Mvc.Views
{
	internal class FlowsMvcViewOptionsSetup : IConfigureOptions<MvcViewOptions>
	{
		private readonly FlowsViewEngine _engine;

		public FlowsMvcViewOptionsSetup(FlowsViewEngine engine)
		{
			if (engine == null)
			{
				throw new ArgumentNullException(nameof(engine));
			}

			_engine = engine;
		}

		public void Configure(MvcViewOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}
			options.ViewEngines.Add(_engine);
		}
	}
}