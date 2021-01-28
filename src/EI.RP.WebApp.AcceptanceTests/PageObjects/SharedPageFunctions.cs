using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.ComponentModel.Design;
using System.IO;
namespace EI.RP.WebApp.AcceptanceTests.PageObjects
{
    class SharedPageFunctions
    {
        protected IWebDriver driver { get; set; }

        internal SharedPageFunctions(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal void IsElementPresent(By by)
        {
	        var waitTime = TimeSpan.FromSeconds(30);
	        WebDriverWait wait = new WebDriverWait(driver, waitTime);
            wait.Timeout = waitTime;

            wait.PollingInterval = TimeSpan.FromMilliseconds(250);
            bool displayed;
            try
            {
                wait.Until(d => d.FindElement(by));
                displayed = true;
            }
            catch(NoSuchElementException)
            {
                displayed = false;
            }
            Assert.IsTrue(displayed);
        }

        internal void IsElementPopulated(By by, string input)
        {
            IWebElement elem = driver.FindElement(by);
            string textboxContents = elem.GetAttribute("value");
            Assert.IsTrue(textboxContents==input);
        }

        internal void AssertValueLatestBill(string id, string value)
        {
            try
            {
                driver.FindElement(By.XPath("//*[@id='" + id + "']/following-sibling::dd[1][contains(text()," + value + ")]"));
                IsElementPresent(By.XPath("//*[@id='" + id + "']/following-sibling::dd[1][contains(text()," + value + ")]"));
            }
            catch
            {
                driver.FindElement(By.XPath("//*[@id='" + id + "']/following-sibling::dd[1][contains(text(),'" + value + "')]"));
                IsElementPresent(By.XPath("//*[@id='" + id + "']/following-sibling::dd[1][contains(text(),'" + value + "')]"));
            }
         
        }

        internal void ClickElement(By by)
        {
            driver.FindElement(by).Click();
			TestContext.Progress.WriteLine("{0} {1} {2}", "Element: " + by.ToString().PadRight(30), " : ".PadRight(20), "Clicked (" + TestContext.CurrentContext.Test.Name + ")");
		}

        internal void ClickElement(IWebElement element)
        {
            element.Click();
			TestContext.Progress.WriteLine("{0} {1} {2}", "Element: " + element.ToString().PadRight(30), " : ".PadRight(20), "Clicked (" + TestContext.CurrentContext.Test.Name + ")");
		}

        internal void ClickElement(By by, TimeSpan waitUntil)
        {
            var el = new WebDriverWait(driver, waitUntil)
                .Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));

            ClickElement(el);
        }

        internal void SendElementKeys(By by, string keys)
        {
            var el = driver.FindElement(by);
            el.Clear();
            el.SendKeys(keys);
            el.SendKeys(Keys.Tab);
            TestContext.Progress.WriteLine("{0} {1} {2} {3}", "Element: " + by.ToString().PadRight(30), "------------------->".PadRight(20), driver.FindElement(by).GetAttribute("value").PadRight(30), "("+TestContext.CurrentContext.Test.Name + ")");
        }

        internal void SendElementKeys(IWebElement element, string keys)
        {
            element.Clear();
            element.SendKeys(keys);
            element.SendKeys(Keys.Tab);
            TestContext.Progress.WriteLine("{0} {1} {2} {3}", "Element: " + element.ToString().PadRight(30), "------------------->".PadRight(20), element.GetAttribute("value").PadRight(30), "(" + TestContext.CurrentContext.Test.Name + ")");
        }

        internal bool SendElementKeys(By by, string keys, TimeSpan waitUntil)
        {
            var el = new WebDriverWait(driver, waitUntil)
                .Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));

            SendElementKeys(el, keys);

            return el.GetAttribute("value") == keys;
        }

        internal void AssertTextInElement(string id, string date)
        {
            IsElementPresent(By.XPath("//*[@id='" + id + "'][contains(text(),'" + date + "')]"));
        }

        internal void IsElementNotPresent(By by)
        {
            bool displayed;
            try
            {
                driver.FindElement(by);
                displayed = true;
            }
            catch (NoSuchElementException)
            {
                displayed = false;
            }
            Assert.IsFalse(displayed);
        }

        internal By ByDataAttribute(string attr = "testid", string val = null)
        {
            return string.IsNullOrWhiteSpace(val)
                ? By.CssSelector($"[data-{attr}]")
                : By.CssSelector($"[data-{attr}='{val}']");
        }

        internal Layout GetCurrentLayout()
        {
            var body = driver.FindElement(By.CssSelector("body"));

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
            var body = driver.FindElement(By.CssSelector("body"));

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
            var el = driver.FindElement(By.CssSelector("[data-navigation]"));

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
