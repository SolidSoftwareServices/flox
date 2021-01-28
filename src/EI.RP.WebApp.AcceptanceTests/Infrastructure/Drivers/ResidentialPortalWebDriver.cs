using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.NUnitExtensions;
using NUnit.Framework;
using OpenQA.Selenium;
using Cookie = System.Net.Cookie;

namespace EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers
{
	public class ResidentialPortalWebDriver : IDisposable
	{
		private readonly object _syncLock = new object();
		private bool _disposed;
		public Guid DriverId { get; } = Guid.NewGuid();
		public ResidentialPortalWebDriver(DriverType driverType)
		{
			DriverType = driverType;
			Instance = WebDriverFactory.Default.ResolveDriver(driverType, out var processId);
			if (processId != null)
			{
				Process = Process.GetProcessById(processId.Value);
			}

			_requestLogger=new WebDriverMetricsLogger(Instance);
		}

		private readonly WebDriverMetricsLogger _requestLogger;


		private Process Process { get; }


		public DriverType DriverType { get; }
		public IWebDriver Instance { get; private set; }

		public bool IsPublicTarget => Instance.Url.StartsWith(TestSettings.Default.PublicTargetUrl,
			StringComparison.InvariantCultureIgnoreCase);

		public bool IsInternalTarget => Instance.Url.StartsWith(TestSettings.Default.InternalTargetUrl,
			StringComparison.InvariantCultureIgnoreCase);

		public void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (!_disposed)
				lock (_syncLock)
				{
					if (!_disposed)
					{
						_requestLogger.Dispose();
						try
						{
							Instance.Close();
						}
						catch (WebDriverException ex)
						{
							TestContext.Progress.WriteOutputLine(ex);
						}

						try
						{
							Instance.Quit();
						}
						catch (WebDriverException ex)
						{
							TestContext.Progress.WriteOutputLine(ex);
						}
						try
						{
							Instance.Dispose();
						}
						catch (WebDriverException ex)
						{
							TestContext.Progress.WriteOutputLine(ex);
						}
						Instance = null;
						//give it sometime to kill it

						SpinWait.SpinUntil(() =>
						{
							bool result;
							try
							{
								result = Process == null || Process.HasExited;
								if (!result)
								{
									Thread.Sleep(250);
								}
							}
							catch (Exception ex)
							{
								result = false;
								TestContext.Progress.WriteOutputLine(ex);
							}

							return result;
						}, TimeSpan.FromSeconds(10));
						
						try
						{
							if (Process != null && !Process.HasExited) Process.Kill();
						}
						catch (Exception ex)
						{
							TestContext.Progress.WriteOutputLine(ex);
						}

						_disposed = true;
					}
				}
		}

		~ResidentialPortalWebDriver()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception ex)
			{
				TestContext.Progress.WriteOutputLine(ex.ToString());
			}
		}

		public async Task<Stream> RequestFileInSession(string url)
		{
			var cookieContainer = ResolveCookies();


			var handler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
				DefaultProxyCredentials = CredentialCache.DefaultCredentials,
				CookieContainer = cookieContainer
			};
			using (var client = new HttpClient(handler))
			{
				var result = await client.GetAsync(url);
				result.EnsureSuccessStatusCode();
                Assert.AreEqual(result.StatusCode,HttpStatusCode.OK);
                return await result.Content.ReadAsStreamAsync();
				
			}


			CookieContainer ResolveCookies()
			{
				var resolveCookies = new CookieContainer();
				var cookies = Instance.Manage().Cookies;
				foreach (var browserCookie in cookies.AllCookies)
				{
					var httpClientCookie = new Cookie(browserCookie.Name, browserCookie.Value);
					httpClientCookie.Path = browserCookie.Path;
					httpClientCookie.Domain = browserCookie.Domain;
					resolveCookies.Add(httpClientCookie);
				}

				return resolveCookies;
			}
		}
    }
}