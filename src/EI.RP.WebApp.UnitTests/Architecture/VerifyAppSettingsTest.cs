using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EI.RP.CoreServices.System.FastReflection;
using EI.RP.WebApp.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Architecture
{
    [TestFixture]
    public class VerifyAppSettingsTest
    {
        public static IEnumerable AppSettingsAreConfiguredForEnvironmentsCases()
        {

            var environmentsWithSettings = GetAppSettingsFilePaths().Select(x =>
                    (x.Name.Replace("appSettings.", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                        .Replace(".json", string.Empty, StringComparison.InvariantCultureIgnoreCase), x))
                .ToDictionary(x => x.Item1, x => x.Item2);

            var propertiesInSecretVault = new[]
            {

	            nameof(AppSettings.SapCrmUmcBearerTokenProviderUrlAsync),
	            nameof(AppSettings.SapCrmUmcUrmBearerTokenProviderUrlAsync),
	            nameof(AppSettings.SapErpUmcBearerTokenProviderUrlAsync),
	            nameof(AppSettings.SapUserManagementBearerTokenProviderUrlAsync),
	            nameof(AppSettings.SwitchApiBearerTokenProviderUrlAsync),
	            nameof(AppSettings.EventsBearerTokenProviderUrlAsync),
	            nameof(AppSettings.StreamServeLiveBearerTokenProviderUrlAsync),
	            nameof(AppSettings.ResidentialPortalDataSourceBearerTokenProviderUrlAsync),
	            nameof(AppSettings.EncryptionPassPhraseAsync),
	            nameof(AppSettings.EncryptionSaltValueAsync),
	            nameof(AppSettings.EncryptionInitVectorAsync),
	            nameof(AppSettings.StreamServeUserName),
	            nameof(AppSettings.StreamServePassword),
	            nameof(AppSettings.PaymentGatewayAccountAsync),
	            nameof(AppSettings.PaymentGatewayMerchantIdAsync),
	            nameof(AppSettings.PaymentGatewaySecretAsync),
	            nameof(AppSettings.ResidentialOnlineEmailRecipientAsync),
	            nameof(AppSettings.AccountQueryCcEmailAsync),
	            nameof(AppSettings.ApiMSubscriptionKeyAsync)
            };

            var nonAzureProperties = new[]
            {
	            //TODO STRICTLY TYPE THE FOLLOWING LIKE THE PREVIOUS
	            nameof(AppSettings.StreamServeLive),
	            nameof(AppSettings.StreamServeArchive)
            };
            var exclude = propertiesInSecretVault.Union(nonAzureProperties).ToArray();

			var properties =
                typeof(AppSettings).GetPropertiesFast(
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, x => !exclude.Contains(x.Name));

            foreach (var environment in environmentsWithSettings.Keys)
            {
                var file = environmentsWithSettings[environment];
                yield return
                    new TestCaseData(environment, file, properties).SetName($"Test settings for {environment}");

            }

            IEnumerable<FileInfo> GetAppSettingsFilePaths()
            {
                var assemblyDir =
                    new FileInfo(new Uri(typeof(VerifyAppSettingsTest).Assembly.GetName().CodeBase).LocalPath)
                        .Directory;
                var webAppDirectory = Path.Combine(assemblyDir.FullName, "../../../../EI.RP.WebApp");
                return new DirectoryInfo(webAppDirectory).GetFiles("appsettings.*.json");
            }
        }

        [Test, TestCaseSource(nameof(AppSettingsAreConfiguredForEnvironmentsCases))]
        public void AppSettingsAreConfiguredForEnvironment(string environmentName, FileInfo settingsFile,
            PropertyInfo[] typeProperties)
        {
            var cfgRoot = new ConfigurationBuilder()
                .SetBasePath(settingsFile.Directory.FullName)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .Build();
            var sut = new AppSettings(cfgRoot,null);
            foreach (var propertyInfo in typeProperties)
            {
                object actual = null;
                Assert.DoesNotThrow(() => { actual = sut.GetPropertyValueFast(propertyInfo); },
                    $"{settingsFile.Name} failed when reading {propertyInfo.Name}");
                Assert.IsNotNull(actual, $"{settingsFile.Name} returned null when reading {propertyInfo.Name}");
                if (actual is string actualStr)
                {
                    Assert.IsNotEmpty(actualStr, $"{settingsFile.Name} returned Empty when reading {propertyInfo.Name}");
                }
            }
        }
    }
}

