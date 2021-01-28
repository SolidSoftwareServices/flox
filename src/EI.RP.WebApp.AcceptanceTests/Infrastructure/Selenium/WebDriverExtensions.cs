using System;
using System.Diagnostics;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.NUnitExtensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium
{
	
	static class WebDriverExtensions
	{
		public static IWebElement FindElementEx(this IWebDriver driver,By by,bool throwIfNotFoundAfterTimeout=true, TimeSpan? timeout=null,
			TimeSpan? waitBetweenAttempts = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return driver.FindElementEx<IWebElement>(by,throwIfNotFoundAfterTimeout, timeout, waitBetweenAttempts, cancellationToken);

		}
		public static TElement FindElementEx<TElement>(this IWebDriver driver,By by,bool throwIfNotFoundAfterTimeout=true, TimeSpan? timeout=null,
			TimeSpan? waitBetweenAttempts = null, CancellationToken cancellationToken = default(CancellationToken)) where TElement : class, IWebElement
		{
			var sw=new Stopwatch();
			sw.Start();

			try
			{
				return PageElementAwaiter.Instance.Poll(
					(attempt) =>
					{
					
						TestContext.Progress.WriteOutputLine(
							$"{nameof(FindElementEx)} - {by}: attempt:{attempt} - Total Time:{sw.Elapsed}");
						var element = (TElement) driver.FindElement(@by);
						return element;
					},
					timeout ??TestSettings.Default.AnyOperationTimeout,
					waitBetweenAttempts ?? TimeSpan.FromSeconds(0.5)
				);
			}
			catch (NoSuchElementException)
			{
				if (throwIfNotFoundAfterTimeout) throw;

				return null;
			}
			finally
			{
				sw.Stop();
			}
		}

		public static IWebElement ClickElementEx(this IWebDriver driver,By by,bool throwIfNotFoundAfterTimeout=true, TimeSpan? timeout=null,
			TimeSpan? waitBetweenAttempts = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			//driver.Manage().Window.Maximize();
			var element= driver.FindElementEx<IWebElement>(by,throwIfNotFoundAfterTimeout, timeout, waitBetweenAttempts, cancellationToken);

			if (element != null)
			{
				ClickElementEx(driver, element,false);
				TestContext.Progress.WriteOutputLine("{0} {1} {2}", $"{nameof(ClickElementEx)} Element: " + by.ToString().PadRight(30), " : ".PadRight(20) , "Clicked" );
			}
			else
			{
				TestContext.Progress.WriteOutputLine("{0} {1} {2}", $"{nameof(ClickElementEx)} Element: " + by.ToString().PadRight(30), " : ".PadRight(20), "NotFound");
			}

			return element;
		}

		public static void ClickElementEx(this IWebDriver driver, IWebElement element,bool logClick=true)
		{
			if (element == null) throw new ArgumentNullException(nameof(element));
			
			PageElementAwaiter.Instance.Retry((attempts) =>
				{
					driver.MoveToElement(element);
					element.Click();
				}
			,TimeSpan.FromSeconds(5));
			if (logClick)
			{
				TestContext.Progress.WriteOutputLine("{0} {1} {2}", $"{nameof(ClickElementEx)} Element: " + element.GetAttribute("name")??element.Text, " : ".PadRight(20), "Clicked" );
			}
		}


		private static void MoveToElement(this IWebDriver driver,IWebElement element)
		{
			((IJavaScriptExecutor) driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
		}

		
	}
}