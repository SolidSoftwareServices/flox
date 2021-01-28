using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Misc
{
	[Explicit]
	[TestFixture]
	public class PageParseTest
	{
		private ISutPage[] _pages;
		private ResidentialPortalApp _app;

		[OneTimeSetUp]
		public void OnSetUp()
		{
			_app = ResidentialPortalApp.StartNew(ResidentialPortalDeploymentType.Public);
			var pageTypes = GetType().Assembly.GetTypes().Where(x=>x.IsClass &&!x.IsAbstract &&typeof(ISutPage).IsAssignableFrom(x)).ToArray();
			_pages = pageTypes.Select(x=>(ISutPage)Activator.CreateInstance(x, _app)).ToArray();

		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			_app.Release();
		}

		[Test]
		public async Task DiagnoseParseIssues()
		{
			var pageBody =
				"<header class=\"main-header\" role=\"banner\">\r\n\r\n    <div class=\"wrapper-980\">\r\n        <div class=\"row\">\r\n            <div id=\"logo-wrapper\" class=\"small-3 medium-3 large-3 column\">\r\n                    <h1><a role=\"link\" href=\"/\"><span>Electric Ireland Smart Living</span></a></h1>\r\n            </div>\r\n            <span id=\"primary-nav__mobile-anchor\" tabindex=\"0\">Menu</span>\r\n            <nav class=\"desktop-nav column small-9 medium-9 large-9\" role=\"navigation\">\r\n                <div class=\"row\">\r\n                    <ul class=\"column small-12 medium-7 large-7 left-items\">\r\n                                <li class=\"dropdown-menu\" aria-label=\"my profile dropdown menu\" id=\"myAccountMenuItem\">\r\n                                    <a href=\"/Accounts/Init\">My Accounts</a>\r\n                                </li>\r\n\r\n\r\n                        <li id=\"helpMenuItem\"><a href=\"/Accounts/NewContainedView?i=EF-%2AjX%2FrODHxXEYI905h8E4dP6sGPhm60rtG5HuTJ%2B0H%2BSOCth0UEgnOZUWI8YrYLowtawDWss7IdU6wWw0OFDaqv8S%2BO%2FNd9yJPYvofnp%2FKJEo%2BPgot3qEQe%2BAfbgbwWGb%2B\">Help</a></li>\r\n                        <li id=\"contactUsMenuItem\"><a href=\"/Accounts/NewContainedView?i=EF-%2AKiy%2F%2BfACSkzhAc4%2BJQ1Q2ZZ4aJ0fz5L6E3vEGQCEc5706tobcEwCPNvmtLdJlZ4P87%2FpUk1YFrb1lZkGwbBicje43DcQNu3nKC4msT9dIcrb79%2B8syXjeGXb%2B2nzV8CzSKVTmPOkofpbdg0lNw8tvg%3D%3D\">Contact Us</a></li>\r\n                    </ul>\r\n\r\n                    <ul class=\"column small-12 medium-5 large-5 right-items\">\r\n                        <li class=\"dropdown-menu\" aria-label=\"my profile dropdown menu\">\r\n                            <p tabindex=\"0\" href=\"\" class=\"menu-title\">My Profile <span class=\"menu-icon fa fa-chevron-down\"></span></p>\r\n                            <ul class=\"menu\" role=\"menu\" style=\"display: none;\">\r\n                                <li id=\"changePasswordMenuItem\"><a href=\"/Accounts/NewContainedView?i=EF-%2AjX%2FrODHxXEYI905h8E4dP6sGPhm60rtG5HuTJ%2B0H%2BSOCth0UEgnOZUWI8YrYLowtawDWss7IdU6wWw0OFDaqvxc47xRW0rc5V9hL3TQ4OJIPUOHrPsDOFlg6vmSCMW7a\">Change Password</a></li>\r\n                                <li id=\"contactDetailsMenuItem\"><a href=\"/Accounts/NewContainedView?i=EF-%2AjX%2FrODHxXEYI905h8E4dP6sGPhm60rtG5HuTJ%2B0H%2BSOCth0UEgnOZUWI8YrYLowtawDWss7IdU6wWw0OFDaqvxt%2BXxj9p2lpZ4JynXAnzIqm4IUggj4E3uF8fAS0JMZZW%2BI6j%2FOpQCiqmzwX34iiCg%3D%3D\">Contact Details</a></li>\r\n                                <li id=\"billAndPaymentMenuItem\"><a href=\"/Accounts/NewContainedView?i=EF-%2AKiy%2F%2BfACSkzhAc4%2BJQ1Q2fUxMB4Q3FKLM4D99FD7SMOcJgvF7%2Ben9k3FU7%2Bz8FshfhSD7pq5YN9X4K4SbjC7c%2FlbCLF%2B8SnwXnhtd5VdNO6RvgcEWuSvNWTGPp1rS4vLCHKqoYva2REHVuRRUzU9BZZQdqm25TztXOoXa%2FjAKnGeGw%2FJblL0kzDEEjzPqM2W\">Billing &amp; Payment options</a></li>\r\n                                <li id=\"accountAndMeterDetails\"><a href=\"/Accounts/NewContainedView?i=EF-%2AKiy%2F%2BfACSkzhAc4%2BJQ1Q2ZZ4aJ0fz5L6E3vEGQCEc5706tobcEwCPNvmtLdJlZ4P87%2FpUk1YFrb1lZkGwbBicje43DcQNu3nKC4msT9dIcoFg71xdHfH%2BJ370BoXfTGTG1ltnRnHeJ%2BwV4cT0rTtpttZb6eC5BVjBDWyJ5Hl99A%3D\">Account &amp; Meter Details</a></li>\r\n\r\n                            </ul>\r\n                        </li>\r\n                        <li id=\"logOutMenuItem\"><a href=\"/Login/Logout\" role=\"menuitem\">Log out</a></li>\r\n\r\n                    </ul>\r\n\r\n                </div>\r\n            </nav>\r\n\r\n        </div>\r\n            <div class=\"account-list single-account\">\r\n                <span id=\"account-selector__mobile-anchor\"><span class=\"fa fa-chevron-down\"></span></span>\r\n                <div id=\"selectedAccount\">\r\n                    <div class=\"wrapper-980\">\r\n                        <span class=\"accounts-list__utility \">\r\n                            <strong>Electricity </strong>\r\n                            <span class=\"accounts-list__acc-number\">174</span>\r\n                        </span>\r\n                        <span class=\"accounts-list__address truncate\">Description66b466db-dc52-4763-baf0-fc992274efbc</span>\r\n                    </div>\r\n                </div>\r\n                <ul role=\"menu\" aria-label=\"accounts list\">\r\n                    <li></li>\r\n\r\n                    <li class=\"primary-nav__accounts-list-acitve\">\r\n                        <a href=\"#\">\r\n                            <span class=\"accounts-list__utility  \">\r\n                                <strong id=\"accountTypeElectricity\" class=\"accounts-list__utility \">Electricity</strong>\r\n                                <span class=\"accounts-list__acc-number\">174</span>\r\n                            </span>\r\n                            <span class=\"accounts-list__address truncate\"></span>\r\n                        </a>\r\n                    </li>\r\n                </ul>\r\n            </div>\r\n            <nav class=\"primary-nav row \" role=\"navigation\">\r\n                <ul class=\"wrapper-980 pr0\">\r\n                    <li id=\"accountOverviewTab\"><a href=\"/Accounts/NewContainedView?i=EF-%2AKiy%2F%2BfACSkzhAc4%2BJQ1Q2ZZ4aJ0fz5L6E3vEGQCEc5706tobcEwCPNvmtLdJlZ4P87%2FpUk1YFrb1lZkGwbBicje43DcQNu3nKC4msT9dIcq%2FE%2FI11ygJfpfONZNO%2BIkGm6Ve3HmNyrpaJ3OrUXhEZeQVbHCzmSnBr8ZRbGCAmzxbisquzfzIZi0TwuZzi%2Ftw\">Account Overview</a></li>\r\n                    <li id=\"billsAndPaymentsTab\"><a href=\"/Accounts/NewContainedView?i=EF-%2AKiy%2F%2BfACSkzhAc4%2BJQ1Q2RRa448qix6ODJ2WTLPBNe%2FS3NgTQociS1rhsDwuDyrgef5sGsE4Nx0N%2BZqnkSpw2aU%2FgiOBPwRosjjpjPKXdz8f3pMuvNweGHSKAlNwExa726EplUpeeU9gqK0%2Bfz5aqk3PMxfWrttfj1S2y0R%2FEd8%3D\">Bills &amp; Payments</a></li>\r\n                    <li id=\"paymentTab\"><a href=\"/Accounts/NewContainedView?i=EF-%2AjX%2FrODHxXEYI905h8E4dP6sGPhm60rtG5HuTJ%2B0H%2BSOCth0UEgnOZUWI8YrYLowtawDWss7IdU6wWw0OFDaqv9fw5pJSXUrantvQDCPZkOVhtyRXsNMUYo5iQ2sdGG2M\">Make a Payment</a></li>\r\n                    <li id=\"meterReadingTab\"><a href=\"/Accounts/NewContainedView?i=EF-%2AKiy%2F%2BfACSkzhAc4%2BJQ1Q2RRa448qix6ODJ2WTLPBNe%2BFMW9f%2F3Xs6PyxvMM4a%2Bfmp6xX%2BWH3lJmaEllbV3H6P9%2FZ%2F7%2BRvt1FirWQXmBXTRGRWNKkmKPROHXZXNOMvtUPeQqmnbCWXfTTddavfztDTvUqYAdqIsq1E1v%2Fl00J%2B25J85cyAlYiZOJRPubreRtC\">Meter Reading</a></li>\r\n                        <li id=\"movingHouseTab\"><a href=\"/Accounts/NewContainedView?i=EF-%2AKiy%2F%2BfACSkzhAc4%2BJQ1Q2ZZ4aJ0fz5L6E3vEGQCEc5706tobcEwCPNvmtLdJlZ4P87%2FpUk1YFrb1lZkGwbBicje43DcQNu3nKC4msT9dIcq6kfIxNMKgELY%2Bwkybj01Sf3yVs3jm9VzJ5j75AgueVQ%3D%3D\">Moving House</a></li>\r\n                </ul>\r\n            </nav>\r\n    </div>\r\n</header>\r\n\r\n<footer>\r\n    <div class=\"container\">\r\n        <div class=\"wrapper-980\">\r\n            <div class=\"row\">\r\n                <ul class=\"list-inline column\" aria-label=\"footer navigation\" role=\"list\">\r\n                    <li role=\"listitem\" id=\"helpFooter\"><a href=\"/Accounts/NewContainedView?i=EF-%2AjX%2FrODHxXEYI905h8E4dP6sGPhm60rtG5HuTJ%2B0H%2BSOCth0UEgnOZUWI8YrYLowtawDWss7IdU6wWw0OFDaqv8S%2BO%2FNd9yJPYvofnp%2FKJEo%2BPgot3qEQe%2BAfbgbwWGb%2B\">Help</a></li>\r\n                    <li role=\"listitem\" id=\"contactFooter\"><a href=\"/Accounts/NewContainedView?i=EF-%2AjX%2FrODHxXEYI905h8E4dP6sGPhm60rtG5HuTJ%2B0H%2BSOCth0UEgnOZUWI8YrYLowtawDWss7IdU6wWw0OFDaqv6PL8OIT6JGJWMgp%2Fev%2BdV7lZqrwBLD9X241zHXUUc%2Fb\">Contact Us</a></li>\r\n                    <li role=\"listitem\" id=\"termsAndConditionsFooter\"><a href=\"/Accounts/NewContainedView?i=EF-%2AENgGS5Rp1Fqfrdpmsefd2Vw7TU%2FmLxr6r4SnIxN44aLR6n5FjjFbTKnAdM7sKx%2BoKF6xVUULcJcjcyXYvwsmuTxxhoZ5bKCMxXJJFOnZ9qz0ZfC15LNLHVL3589A1qCXBLlax2WvJivk0w7nGb02QQ%3D%3D\">Terms &amp; Conditions</a></li>\r\n                    <li role=\"listitem\" id=\"disclaimerFooter\"><a href=\"/Accounts/NewContainedView?i=EF-%2AkLiBbz8RV16vWrg4A2SxHyPeSBMgu%2F8GZI4EJMzlJC8XC%2BweKB%2Bnb3ldurklWjN8vc9XLIg0LG4e711TRD0gDy7SdrzPsIyROHkAV9%2BAS4RLozTfBcL%2FEthI0WbZgGW%2B9hO60EieTgrRUcw9ymL9gg%3D%3D\">Disclaimer</a></li>\r\n                    <li role=\"listitem\" id=\"privacyFooter\"><a data-tag=\"lnkPrivacyNotice\" href=\"/Accounts/NewContainedView?i=EF-%2A%2Bg2fsjoHXG%2FWMvHvCd5DQt4T70vQhc8y8IleUr9zDFny7%2BubuxGgwvKInd%2B%2B606eoVNVI7mo8yZOof%2FvwI%2Fcl4bIzGgrSEntgQzbETFZqiZLViCOzHsKWOldxALFYs2ygY4nZhfIqunSKsAyPkOs7w%3D%3D\">Privacy Notice and Cookies Policy</a></li>\r\n                </ul>\r\n            </div>\r\n            <div class=\"row\">\r\n                <div class=\"column\">\r\n                    <span class=\"copyright\">\r\n                        <span aria-label=\"Copyright 2016\">\u00A9Copyright 2016 </span>\r\n                        <strong>Postal Address:</strong> Electric Ireland, PO Box 841, South City Delivery Office, Cork\r\n                    </span>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</footer>\r\n<div>";
			var config = Configuration.Default;

			var context = BrowsingContext.New(config);

			var document = (IHtmlDocument)await context.OpenAsync(req => req.Content(pageBody));

			var actual= _pages.Where(x => x.TryParse(document)).ToArray();


		}

		[Test]
		public async Task DiagnoseParseIssuesJavascript()
		{
			var pageBody =
				"<html>\r\n<body>\r\n\r\n<h2>What Can JavaScript Do?</h2>\r\n\r\n<p id=\"demo\">JavaScript can change HTML content.</p>\r\n\r\n<button  id=\"btn\" type=\"button\" onclick='document.getElementById(\"demo\").innerHTML = \"Hello JavaScript!\"'>Click Me!</button>\r\n\r\n</body>\r\n</html>";
			var config = Configuration.Default.WithDefaultLoader().WithJs().WithXPath().WithCss();

			var context = BrowsingContext.New(config); 

			var document = (IHtmlDocument)await context.OpenAsync(req => req.Content(pageBody));
			Assert.AreEqual("JavaScript can change HTML content.", document.QuerySelector("#demo").TextContent);

			((IHtmlButtonElement)document.QuerySelector("#btn")).DoClick();

			Assert.AreEqual("Hello JavaScript!", document.QuerySelector("#demo").TextContent);

			var actual = _pages.Where(x => x.TryParse(document)).ToArray();


		}
	}
}
