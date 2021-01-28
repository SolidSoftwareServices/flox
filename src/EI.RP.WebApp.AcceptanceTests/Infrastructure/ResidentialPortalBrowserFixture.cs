using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.NUnitExtensions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestContext = NUnit.Framework.TestContext;

namespace EI.RP.WebApp.AcceptanceTests.Infrastructure
{
	[TestFixture]
	public abstract class ResidentialPortalBrowserFixture
	{
		private readonly IDictionary<string, SingleTestContext> _contexts = new ConcurrentDictionary<string, SingleTestContext>();
		protected SingleTestContext Context => _contexts[ResolveTestContextKey()];
		private static string ResolveTestContextKey()
		{
			return TestContext.CurrentContext.Test.FullName;
		}
		
        [SetUp]
		public async Task SetUp()
		{
			_contexts.Add(ResolveTestContextKey(), new SingleTestContext());
             new SignInPage(Context.Driver.Value).AssertLoginPage(assertPublic:false);
		}
        [TearDown]
		public async Task TearDown()
		{
			try
			{
				if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
				{
					try
					{
						string txtErrorUrl = Context.Driver.Value.Instance.Url;
						var baseUrl = Context.IsPublicTarget
							? TestSettings.Default.PublicTargetUrl
							: TestSettings.Default.InternalTargetUrl;

						TestContext.Progress.WriteOutputLine("");
						TestContext.Progress.WriteOutputLine("*********************** Test Teardown ***********************");
						TestContext.Progress.WriteOutputLine("");
						await LogError(baseUrl);
						TestContext.Progress.WriteOutputLine("Error found at: " + txtErrorUrl);
						Debug.Print("Error found at: " + txtErrorUrl);
						TestContext.Progress.WriteOutputLine(Context.Driver.Value.Instance.PageSource);
						TestContext.Progress.WriteOutputLine("");
						TestContext.Progress.WriteOutputLine("************************************************************");
						TestContext.Progress.WriteOutputLine("");
					}
					catch
					{

						TestContext.Progress.WriteOutputLine(
							"-----------------------TEAR DOWN FAILURE----------------------");
					}

					Context.NotifyFailure();
				}

				Context.Dispose();
			}
			finally
			{

				_contexts.Remove(ResolveTestContextKey());
			}
		}   


        private async Task LogError(string baseUrl)
		{
			try
			{
				var value = await GetValue();

				TestContext.Progress.WriteOutputLine("");
				TestContext.Progress.WriteOutputLine("##################################################################");
				TestContext.Progress.WriteOutputLine("Log Dump");
				TestContext.Progress.WriteOutputLine("##################################################################");
				TestContext.Progress.WriteOutputLine(value);
				TestContext.Progress.WriteOutputLine("");
			}
			catch (Exception ex)
			{
				TestContext.Progress.WriteOutputLine($"An error occurred while trying to dump the logs: {ex}");
			}

			async Task<string> GetValue()
			{
				string value;
				var stream = await Context.Driver.Value.RequestFileInSession($"{baseUrl}logview/SessionLog");
				using (var streamReader = new StreamReader(stream))
				{
					value = streamReader.ReadToEnd();
				}

				return value;
			}
		}


        private async Task WriteLine(string text)
		{
			
		}

    }
}
