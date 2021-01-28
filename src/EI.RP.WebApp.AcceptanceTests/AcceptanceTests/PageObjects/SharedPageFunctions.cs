using System;
using System.Linq;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.NUnitExtensions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    public class SharedPageFunctions
    {
        public IWebDriver Driver { get; set; }
        internal SharedPageFunctions(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		public SharedPageFunctions(IWebDriver driver0)
        {
            Driver = driver0;
        }

		
		public void IsElementPresent(By by,Func<IWebElement,bool> condition=null)
		{
			var element=Driver.FindElementEx<IWebElement>(by);
			if (condition != null)
			{
				Assert.IsTrue(condition(element));
			}

			Assert.IsNotNull(element);
		}
		public void IsElementNotPresent(By by)
		{
			var present = PageElementAwaiter.Instance.Poll(
				(attempt) => Driver.FindElements(@by).Any(),
				TimeSpan.FromMinutes(1),
				TimeSpan.FromSeconds(0.5)
			);

			Assert.IsFalse(present);
		}
		public void IsElementTextPresent(By by, string text)
		{
			IsElementPresent(by, w => w.Text == text);
		}

        internal By ByDataAttribute(object val)
        {
            throw new NotImplementedException();
        }

        public void IsElementTextPresent(string text)
        {
			IsElementPresent(By.XPath("//*[contains(text(),'" + text + "')]"));
        }
        public void IsElementTextNotPresent(string text)
        {
	        IsElementNotPresent(By.XPath("//*[text()='" + text + "']"));
            
        }


        public void IsElementPopulated(By by, string input)
		{
			IWebElement elem = Driver.FindElementEx<IWebElement>(by);
			string textboxContents = elem.GetAttribute("value");
			Assert.IsTrue(textboxContents == input);
		}

		internal void CloseOpenedTab()
		{
			var tabs = Driver.WindowHandles;
			Driver.SwitchTo().Window(tabs[1]);
			Driver.Close();
			Driver.SwitchTo().Window(tabs[0]);
		}

		public void GoToOpenedTab()
		{
			int returnValue;
			bool boolReturnValue;
			for (var i = 0; i < 100; Thread.Sleep(100), i++)
			{
				returnValue = Driver.WindowHandles.Count;
				var tabs = Driver.WindowHandles;
				if (returnValue > 1)
				{
					Driver.SwitchTo().Window(tabs[1]);
					return;
				}
			}

		}



		public void AssertValueLatestBill(string id, string value)
        {
            try
            {
                Driver.FindElementEx(By.XPath("//*[@id='" + id + "']/following-sibling::dd[1][contains(text()," + value + ")]"));
                IsElementPresent(By.XPath("//*[@id='" + id + "']/following-sibling::dd[1][contains(text()," + value + ")]"));
            }
            catch
            {
                Driver.FindElementEx(By.XPath("//*[@id='" + id + "']/following-sibling::dd[1][contains(text(),'" + value + "')]"));
                IsElementPresent(By.XPath("//*[@id='" + id + "']/following-sibling::dd[1][contains(text(),'" + value + "')]"));
            }
         
        }

        internal void MoveToElement(IWebElement element)
		{
			((IJavaScriptExecutor) Driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
		}

        internal void ClickElement(By by)
        {
	        Driver.ClickElementEx(by);
            
        }

     
        internal void ClickElement(By by, TimeSpan waitUntil, bool moveToElement = true)
        {
            var element = new WebDriverWait(Driver, waitUntil).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));

            Driver.ClickElementEx(element);
        }

        internal void ClickElementFirst(string text)
        {
            Driver.FindElements(By.XPath("//*[contains(text(),'" + text + "')]"))[0].Click();
        }

        internal void SendElementKeys(By by, string keys)
		{
			var element = Driver.FindElementEx(@by);
			element.Clear();
			element.SendKeys(keys);
			element.SendKeys(Keys.Tab);
			TestContext.Progress.WriteOutputLine("{0} {1} {2}", "Element: " + by.ToString().PadRight(30), "------------------->".PadRight(20), element.GetAttribute("value").PadRight(30));
		}
		internal void SendElementKeys(IWebElement element, string keys)
		{
			element.Clear();
			element.SendKeys(keys);
			element.SendKeys(Keys.Tab);
			TestContext.Progress.WriteOutputLine("{0} {1} {2}", "Element: " + element.ToString().PadRight(30), "------------------->".PadRight(20), element.GetAttribute("value").PadRight(30));
		}

		public void AssertTextInElement(string id, string text)
		{
			IsElementPresent(By.XPath("//*[@id='" + id + "'][contains(text(),'" + text + "')]"));
		}


        internal By ByDataAttribute(string attr = "testid", string val = null)
        {
            return string.IsNullOrWhiteSpace(val)
                ? By.CssSelector($"[data-{attr}]")
                : By.CssSelector($"[data-{attr}='{val}']");
        }
        internal Layout GetCurrentLayout()
        {
            var body = Driver.FindElementEx(By.CssSelector("body"));

            if (body == null) return Layout.None;

            switch (body.GetAttribute("data-layout").ToLower())
            {
                case "electricity-and-gas":
                    return Layout.ElectricityAndGas;
                case "energy-services":
                    return Layout.EnergyServices;
                default:
                    return Layout.None;
            }
        }

        internal LayoutType GetCurrentLayoutType()
        {
            var body = Driver.FindElementEx(By.CssSelector("body"));

            if (body == null) return LayoutType.None;

            switch (body.GetAttribute("data-layout-type").ToLower())
            {
                case "legacy":
                    return LayoutType.Legacy;
                case "smart":
                    return LayoutType.Smart;
                default:
                    return LayoutType.None;
            }
        }

        internal LayoutType GetCurrentNavigationType()
        {
            var el = Driver.FindElementEx(By.CssSelector("[data-navigation]"));

            if (el == null) return LayoutType.None;

            switch (el.GetAttribute("data-navigation-type").ToLower())
            {
                case "legacy":
                    return LayoutType.Legacy;
                case "smart":
                    return LayoutType.Smart;
                default:
                    return LayoutType.None;
            }
        }

        internal void IsElementTextPresent(object amountDueIsTooLow)
        {
            throw new NotImplementedException();
        }
    }
    enum Layout
    {
        None = 0,
        ElectricityAndGas,
        EnergyServices
    }

    enum LayoutType
    {
        None = 0,
        Legacy,
        Smart
    }
}
