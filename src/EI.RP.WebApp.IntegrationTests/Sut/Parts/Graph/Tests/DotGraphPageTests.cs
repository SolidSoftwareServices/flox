using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UiFlows.Mvc.Controllers;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Graph.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Graph.Tests
{
	[TestFixture]
	internal class DotGraphPageTests : WebAppPageTests<DotGraphPage>
	{
		private AppUserConfigurator _userConfig;

		protected override async Task TestScenarioArrangement()
		{
			_userConfig = App.ConfigureUser("a@A.com", "test");
			_userConfig.AddElectricityAccount();
			_userConfig.Execute();

            await ((ResidentialPortalApp) await App.WithValidSessionFor(_userConfig.UserName, _userConfig.Role)).ToFirstAccount();
        }

		private static IEnumerable CanGenerateTheDotGraphCases()
		{
			return Enum.GetValues(typeof(ResidentialPortalFlowType)).Cast<ResidentialPortalFlowType>().Where(x=>x!=ResidentialPortalFlowType.NoFlow)
				.Select(x => new TestCaseData(x).SetName(x.ToString()));
		}
		[Explicit]
		[TestCaseSource(nameof(CanGenerateTheDotGraphCases))]
		public async Task CanGenerateTheDotGraph(ResidentialPortalFlowType flowType)
		{
			var page = (await App.ToUrl($"{flowType}/{nameof(UiFlowController.DotGraph)}"))
				.CurrentPageAs<DotGraphPage>();
			Assert.IsFalse(string.IsNullOrWhiteSpace(page.GraphDefinition.TextContent));
			
		}
	}
}