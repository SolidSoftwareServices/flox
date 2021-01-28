using System;
using System.Net.Http;
using Autofac;
using AutoFixture;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.CoreServices.Platform;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Secrets;
using EI.RP.DomainServices.Infrastructure.Settings;
using Ei.Rp.Mvc.Core.Cryptography.Urls;
using EI.RP.Stubs.CoreServices.Authx;
using EI.RP.Stubs.CoreServices.Cqrs.Events;
using EI.RP.UI.TestServices.Services;
using EI.RP.UiFlows.Core.Infrastructure.DataSources.Repositories;
using EI.RP.WebApp.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EI.RP.WebApp.IntegrationTests.Infrastructure
{
	internal partial class ResidentialPortalApp
	{
		protected override HttpClient OnCreateClient(AppFactory<Startup> factory)
		{
			var result = factory
				.WithWebHostBuilder(b =>
				{
					var urlMock = BuildUrlEncryptionSettings();
					var platformSettingsMock = BuildPlatformSettingsMock();
					var domainSettingsMock = BuildDomainSettingsMock();
					var secretsRepository = BuildSecretsRepository();
					b.ConfigureTestContainer<ContainerBuilder>(
						cb =>
						{
							cb.RegisterInstance(DomainFacade.CommandDispatcher).As<IDomainCommandDispatcher>().SingleInstance();
							cb.RegisterInstance(DomainFacade.QueryResolver).As<IDomainQueryResolver>().SingleInstance();
							cb.RegisterType<FakeEventsPublisher>().AsImplementedInterfaces().SingleInstance();
							cb.RegisterType<FakeTokenProvider>().AsImplementedInterfaces();
							cb.RegisterInstance(urlMock.Object).As<IUrlEncryptionSettings>();
							cb.RegisterInstance(platformSettingsMock.Object)
								.As<IPlatformSettings>()
								.As<ICacheSettings>();
							cb.RegisterInstance(domainSettingsMock.Object)
								.As<IDomainSettings>();
							cb.Register(c=>new TestAppSettings(c.Resolve<AppSettings>())).As<IUiAppSettings>().SingleInstance();
							cb.RegisterInstance(Profiler).As<IProfiler>();

							cb.Register(c => DomainFacade.SessionProvider)
								.AsImplementedInterfaces()
								.SingleInstance();

							cb.Register(c =>
							{
								return new UiFlowContextDataRepositoryDecorator(_contextDataRepository, c.Resolve<IProfiler>());
							})
								.SingleInstance()
								.AsImplementedInterfaces();

							cb.RegisterInstance(secretsRepository)
								.SingleInstance()
								.AsImplementedInterfaces();
						});

				})
				.CreateClient(new WebApplicationFactoryClientOptions
				{
					AllowAutoRedirect = true
				});

			return result;

			Mock<IDomainSettings> BuildDomainSettingsMock()
			{
				var mock = new Mock<IDomainSettings>();
				mock.SetupGet(x => x.IsInternalDeployment)
					.Returns(this.RunningAsDeploymentType == ResidentialPortalDeploymentType.Internal);
				return mock;
			}

			Mock<IPlatformSettings> BuildPlatformSettingsMock()
			{
				var platformSettingsMock = new Mock<IPlatformSettings>();
				
				platformSettingsMock.SetupGet(x => x.IsCacheEnabled).Returns(false);
				platformSettingsMock.SetupGet(x => x.IsCachePreLoaderEnabled).Returns(false);
				platformSettingsMock.SetupGet(x => x.HealthChecksEnabled).Returns(false);
				
				return platformSettingsMock;
			}

			Mock<IUrlEncryptionSettings> BuildUrlEncryptionSettings()
			{
				var urlMock = new Mock<IUrlEncryptionSettings>();
				urlMock.SetupGet(x => x.EncryptUrls).Returns(false);
				return urlMock;
			}

			FakeSecretsRepository BuildSecretsRepository()
			{
				var secretsRepository = new FakeSecretsRepository(true);

				AddSiteSpecific(secretsRepository);

				AddTokenUrl(secretsRepository);

				return secretsRepository;
			}

			void AddSiteSpecific(FakeSecretsRepository secretsRepository)
			{
				var portals = new[] {"PublicPortal", "InternalPortal"};

				foreach (var portal in portals)
				{
					secretsRepository.Set($"{portal}-EncryptionSettings-PassPhrase", "asdfasdf1234");
					secretsRepository.Set($"{portal}-EncryptionSettings-SaltValue", "asdfasdf");
					secretsRepository.Set($"{portal}-EncryptionSettings-InitVector", "asdf-asdf-654321");

					secretsRepository.Set($"{portal}-PaymentGatewaySettings-Account", Fixture.Create<string>());
					secretsRepository.Set($"{portal}-PaymentGatewaySettings-MerchantId", Fixture.Create<string>());
					secretsRepository.Set($"{portal}-PaymentGatewaySettings-Secret", Fixture.Create<string>());

					secretsRepository.Set($"{portal}-Emails-RecipientEmail", "a@a.a");
					secretsRepository.Set($"{portal}-Emails-AccountQueryRecipientEmail", "b@b.b");
				}
			}

			void AddTokenUrl(FakeSecretsRepository secretsRepository)
			{
				var tokenSecretKeys = new[]
				{
					"DataServicesSettings-SapSettings-SapUserManagementApiTokenUrl",

					"DataServicesSettings-SapSettings-SapCrmUmcUrmApiTokenUrl",
					"DataServicesSettings-SapSettings-SapCrmUmcApiTokenUrl",
					"DataServicesSettings-SapSettings-SapErpUmcApiTokenUrl",
					"DataServicesSettings-StreamServe-StreamServeCurrentApiTokenUrl",
					"DataServicesSettings-StreamServe-StreamServeArchiveApiTokenUrl",
					"DataServicesSettings-EventsPublisherSettings-CmdmApiTokenUrl",
					"DataServicesSettings-SwitchApiSettings-SwitchApiTokenUrl",
					"DataServicesSettings-MprnApiSettings-MprnApiTokenUrl",
				};
				foreach (var tokenSecretKey in tokenSecretKeys)
				{
					secretsRepository.Set(tokenSecretKey, Fixture.Create<string>());
				}
			}
		}

		private class TestAppSettings : AppSettings
		{

			public TestAppSettings(AppSettings source) : base(source)
			{ }

			public override bool RequireCookiesPolicyCompliance { get; } = false;

			public override bool IsTagManagerEnabled { get; } = false;
			
		}

	}


}
