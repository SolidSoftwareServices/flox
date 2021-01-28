using System.Configuration;

namespace EI.RP.DomainServices.Infrastructure.Util
{
	public class CustomConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("id", IsKey = true, IsRequired = true)]
		public string Id
		{
			get => (string) base["id"];
			set => base["id"] = value;
		}

		[ConfigurationProperty("value", IsRequired = true)]
		public string Value
		{
			get => (string) this["value"];
			set => base["value"] = value;
		}
	}
}