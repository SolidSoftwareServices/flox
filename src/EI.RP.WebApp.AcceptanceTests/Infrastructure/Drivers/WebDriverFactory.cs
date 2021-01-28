using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers
{
	class WebDriverFactory
	{
		public static readonly WebDriverFactory Default=new WebDriverFactory();
		private WebDriverFactory(){}

		public IWebDriver ResolveDriver(DriverType driverType, out int? processId)
		{
			WebDriverBuilder builder;
			switch (driverType)
			{
				case DriverType.Chrome:
					builder = new ChromeWebDriverBuilder();
					break;
				case DriverType.ZapChrome:
					builder = new ZapChromeWebDriverBuilder();
					break;
				case DriverType.RemoteChrome:
					
					builder = new RemoteChromeWebDriverBuilder();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(driverType), driverType, null);
			}

			var driver = builder.Build(out processId);
			return driver;
		}

		private abstract class WebDriverBuilder
		{
			public IWebDriver Build(out int? processId)
			{
				var driver = DoBuild(out processId);
				driver.Manage().Timeouts().ImplicitWait=TimeSpan.FromSeconds(2);
				driver.Manage().Timeouts().PageLoad=TestSettings.Default.AnyOperationTimeout;
				return driver;
			}

			protected abstract IWebDriver DoBuild(out int? processId);
		}

		private class ChromeWebDriverBuilder : WebDriverBuilder
		{
			protected override IWebDriver DoBuild(out int? processId)
			{
				var options = BuildChromeOptions();

				var executingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				var chromeDriverService = ChromeDriverService.CreateDefaultService( executingFolder);
				if (TestSettings.Default.UseHeadlessBrowser)
				{
					chromeDriverService.HideCommandPromptWindow = true;
				}
				SetChromeLogs();
				var result = new ChromeDriver(chromeDriverService, options, TestSettings.Default.AnyOperationTimeout);
					
				processId = chromeDriverService.ProcessId;
				
				return result;

				void SetChromeLogs()
				{
#if DEBUG
					chromeDriverService.LogPath = Path.Combine(executingFolder, "chromedriver.logs");
					chromeDriverService.EnableVerboseLogging = true;
#endif
				}
			}

			protected virtual ChromeOptions BuildChromeOptions(Proxy proxy=null)
			{
				ChromeOptions options = new ChromeOptions();
				if (proxy != null)
				{
					options.Proxy = proxy;
				}

				options.AddArguments("--window-size=1920,1080", "--ignore-certificate-errors");
				if (TestSettings.Default.UseHeadlessBrowser)
				{
					options.AddArguments("--headless");
				}

				return options;
			}
		}
		private class ZapChromeWebDriverBuilder : ChromeWebDriverBuilder
		{
			protected override ChromeOptions BuildChromeOptions(Proxy proxy = null)
			{
				return base.BuildChromeOptions(new Proxy {HttpProxy = TestSettings.Default.ProxyUrl, SslProxy = TestSettings.Default.ProxyUrl});
			}
		}
		private class RemoteChromeWebDriverBuilder : WebDriverBuilder
		{
			protected override IWebDriver DoBuild(out int? processId)
			{
				processId = null;

				DesiredCapabilities capabilities = new DesiredCapabilities();
				capabilities.SetCapability(CapabilityType.Platform, "WINDOWS");
				capabilities.SetCapability(CapabilityType.BrowserName, "chrome");
				capabilities.SetCapability("server:CONFIG_UUID", "f44c3fc4-58c9-47ea-b3af-79e4731a634a");
				capabilities.SetCapability(CapabilityType.AcceptInsecureCertificates, true);
				capabilities.SetCapability("applicationName", "ServerNode");
				capabilities.SetCapability("headless", TestSettings.Default.UseHeadlessBrowser);

				var result = new RemoteWebDriver(new Uri("http://seleniumhub:4455/wd/hub"), capabilities,TestSettings.Default.AnyOperationTimeout);

				return result;
			}
		}
	}
}