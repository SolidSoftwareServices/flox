namespace ResidentialPortalCLI.OData.ODataProxy
{
	public class ODataSettingsFile
	{
		public string AppSettingsFileFolder { get; set; }
		public string AppSettingsFileName { get; set; }
		public string EnvironmentName { get; set; }
		public string UserName { get; set; }

		public string Password { get; set; }

		public string OutputFolder { get; set; }

		public bool RemovePreviouslyGeneratedFiles { get; set; }

		public string[] ExcludeEntities { get; set; } = new string[0];
		public string[] EntitiesWhichUpdatesHackOData { get; set; } = new string[0];
	}
}