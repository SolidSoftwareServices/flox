using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using S3.CoreServices.Platform;
using S3.CoreServices.Profiling;
using S3.Mvc.Core.Cryptography.Urls;

using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container2;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages;
using S3.UI.TestServices.Services;
using S3.UI.TestServices.Sut;
using S3.UiFlows.Mvc.FlowTagHelpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Moq;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Infrastructure
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
		private readonly Lazy<Mock<IUrlEncryptionSettings>> _urlMock = new Lazy<Mock<IUrlEncryptionSettings>>(()=> {
			var mock = new Mock<IUrlEncryptionSettings>();
			mock.SetupGet(x => x.EncryptUrls).Returns(false);
			return mock;
		});
		private readonly Lazy<Mock<IPlatformSettings>> _platformSettingsMock = new Lazy<Mock<IPlatformSettings>>(()=> {
			var mock = new Mock<IPlatformSettings>();
			mock.SetupGet(x => x.ProfileInDetail).Returns(false);
			mock.SetupGet(x => x.IsDeferredComponentLoadEnabled).Returns(false);
			return mock;
		});

		
		protected override HttpClient OnCreateClient(AppFactory<Startup> factory)
		{
			var result = factory
				.WithWebHostBuilder(b =>
				{
					
					b.ConfigureTestContainer<ContainerBuilder>(
						cb =>
						{
							cb.RegisterInstance(_urlMock.Value.Object).As<IUrlEncryptionSettings>();
							cb.RegisterInstance(Profiler).As<IProfiler>();
							cb.RegisterInstance(_platformSettingsMock.Value.Object).As<IPlatformSettings>();
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