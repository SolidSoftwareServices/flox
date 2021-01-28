using System;
using System.Configuration;

namespace EI.RP.WebApp.AcceptanceTests
{
	public  class TestSettings
	{
		private const string LocalUrl = "https://localhost:44371/";
		public static  readonly TestSettings Default=new TestSettings();
		private TestSettings(){}
		public  string PublicTargetUrl => UseLocalApp
			?LocalUrl
			:ConfigurationManager.AppSettings["TargetUrl"];
		public  string InternalTargetUrl =>UseLocalApp
			? LocalUrl
			: ConfigurationManager.AppSettings["AgentTargetUrl"];
		public  bool SecurityTest => bool.Parse(ConfigurationManager.AppSettings["SecurityTest"]);
		public  string TestType => ConfigurationManager.AppSettings["TestType"];
		public  string ProxyUrl => ConfigurationManager.AppSettings["ProxyUrl"];
		public  bool UseLocalApp => bool.Parse(ConfigurationManager.AppSettings["UseLocalApp"]);
		public  bool UseLocalWebdriver => bool.Parse(ConfigurationManager.AppSettings["UseLocalWebdriver"]);
		public  bool UseHeadlessBrowser =>bool.Parse( ConfigurationManager.AppSettings["UseHeadlessBrowser"]);

		public int NumberOfAttemptsOnDriverFailure =>
			int.Parse(ConfigurationManager.AppSettings["NumberOfAttemptsOnDriverFailure"]);

		public string SmartHubUrl() => $"{PublicTargetUrl}?source=smarthubpage";

		public string AccountsSet =>
			ConfigurationManager.AppSettings["AccountsSet"];

		public TimeSpan AnyOperationTimeout =>
			TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["AnyOperationTimeoutSeconds"]));
		
	}
}