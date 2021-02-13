using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Win32;
using NUnit.Framework;
using S3.CoreServices.System;
using S3.UI.TestServices.Sut;

namespace S3.UI.TestServices.Test
{
	[TestFixture]
	public abstract class WebAppPageTestsBase<TApp,TPage>: WebAppTestsBase<TApp> where TApp:ISutApp where TPage:ISutPage
	{


		protected WebAppPageTestsBase(Func<TApp> appFactoryFunc) : base(appFactoryFunc)
		{
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			Logger.Info("Preparing Scenario");
			try
			{
				TestScenarioArrangement().Wait();
			}
			catch (Exception ex)
			{
				Logger.Fatal(()=>ex.ToString());
				throw;
			}

			Logger.Info("Scenario Ready");
		}

		protected abstract Task TestScenarioArrangement();

		[TearDown]
		public override void TearDown()
        {
            if (Logger.IsInfoEnabled)
            {
                var currentHtml = App.CurrentPage?.Document?.Body?.InnerHtml;
                if (currentHtml != null)
                {
	                Logger.Info(() => $"Current Page:{Environment.NewLine}{currentHtml}");
	                ShowFailedPage(currentHtml);
                }
            }

            base.TearDown();

            void ShowFailedPage(string currentHtml)
            {
	            if (Debugger.IsAttached && TestContext.CurrentContext.Result.FailCount>0)
	            {
		            var filePath = Path.Combine(Assembly.GetExecutingAssembly().GetDirectory().FullName,
			            $"{TestContext.CurrentContext.Test.Name}.html");
		            File.WriteAllText(filePath, currentHtml);

		            var browserPath = Registry.GetValue(
			            @"HKEY_CLASSES_ROOT\ChromeHTML\shell\open\command", null, null) as string;
		            if (browserPath != null)
		            {
			            var split = browserPath.Split('\"');
			            browserPath = split.Length >= 2 ? split[1] : null;
		            }

		            if (!string.IsNullOrWhiteSpace(browserPath))
		            {
			            Process.Start(browserPath, filePath);
		            }
		            else
		            {
						Logger.Warn(()=>"Install chrome for an enhanced debugging experience");
		            }
	            }
            }
        }


		protected TPage Sut { get; set; }
	}
}