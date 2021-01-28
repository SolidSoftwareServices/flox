using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.NUnitExtensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers
{
	class WebDriverMetricsLogger:IDisposable
	{
		private readonly IWebDriver _browser;
		private string _lastUrl;
		private readonly Timer _timer;
		public WebDriverMetricsLogger(IWebDriver browser)
		{
			_browser = browser;

			_lastUrl = _browser.Url;
			_timer=new Timer(LogNewUrlMetrics,this,TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(1) );
		}

		public static void LogNewUrlMetrics(Object stateInfo)
		{
			var logger = (WebDriverMetricsLogger) stateInfo;
			var url = logger._browser.Url;
			if (logger._lastUrl != url)
			{
				logger._lastUrl = url;
				var js = (IJavaScriptExecutor) logger._browser;

				var msString = js.ExecuteScript("return window.performance.timing.domContentLoadedEventEnd-window.performance.timing.navigationStart;");
				try
				{
					var totalTime = TimeSpan.FromMilliseconds(Convert.ToInt32(msString));
					TestContext.Progress.WriteOutputLine($"==>New Page Loaded Total time: {totalTime} - {url} ");
				}
				catch (OverflowException)
				{
					TestContext.Progress.WriteOutputLine($"==>New Page Loaded Total time: ERROR PARSING VALUE to int({msString}) - {url} ");
				}
			}
		}
		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}