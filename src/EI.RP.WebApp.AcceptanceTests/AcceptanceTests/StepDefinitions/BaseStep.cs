using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    public class BaseStep
    {
        public SingleTestContext shared;
		public IWebDriver driver;
        public BaseStep(SingleTestContext shared)
        {
            this.shared = shared;
        }
		
	}
}
