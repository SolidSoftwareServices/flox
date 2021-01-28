using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using EI.RP.CoreServices.Profiling;
using Ei.Rp.Mvc.Core.Cryptography.Urls;

using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container2;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.Index.Pages;
using EI.RP.UI.TestServices.Services;
using EI.RP.UI.TestServices.Sut;
using EI.RP.UiFlows.Mvc.FlowTagHelpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Moq;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Infrastructure
{
	internal partial class PrototypeApp : SutAppBase<PrototypeApp, Startup>
	{
		private PrototypeApp()
		{
			base.NewPageLoaded += PrototypeApp_NewPageLoaded;
		}

		private void PrototypeApp_NewPageLoaded(ISutApp src, ISutPage page)
		{
			if (page == null) return;
			var symbols = FlowActionTagHelper.DesignerOnlySymbols;
		    var content = page.Document.Body.InnerHtml;
		    var errors = new List<string>();
			foreach (var name in symbols)
			{
				if(content.Contains(name,StringComparison.InvariantCultureIgnoreCase))
					errors.Add($"Tag reserved symbol named: \"{name}\" should not be in the generated document");
			}

			if (errors.Any())
			{
				Assert.Fail(string.Join(Environment.NewLine,errors));
			}
		}

		protected override HttpClient OnCreateClient(AppFactory<Startup> factory)
		{
			var result = factory
				.WithWebHostBuilder(b =>
				{
					var urlMock = new Mock<IUrlEncryptionSettings>();
					urlMock.SetupGet(x => x.EncryptUrls).Returns(false);
					b.ConfigureTestContainer<ContainerBuilder>(
						cb =>
						{
							cb.RegisterInstance(urlMock.Object).As<IUrlEncryptionSettings>();
							cb.RegisterInstance(Profiler).As<IProfiler>();
						});
				})
				.CreateClient(new WebApplicationFactoryClientOptions
				{
					AllowAutoRedirect = true,
				});
				result.Timeout=TimeSpan.FromHours(2);
			return result;
		}

		public async Task<PrototypeApp> ToIndexPage()
		{
			return await ToUrl(string.Empty);
		}
		public async Task<Container1Page0> ToContainerFlow()
		{
			await ToIndexPage();
			await CurrentPageAs<IndexPage>().SelectContainersFlow();


			return CurrentPageAs<Container1Page0>();
		}

		public async Task<Container2Page0> ToContainer2Flow()
		{
			await ToIndexPage();
			await CurrentPageAs<IndexPage>().SelectContainersFlow2();


			return CurrentPageAs<Container2Page0>();
		}


		
	}
}