using System.Configuration;

namespace EI.RP.DomainServices.Infrastructure.Util
{
	public class CustomConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
		public CustomConfigurationElementCollection ConfigurationElements
		{
			get => (CustomConfigurationElementCollection) this[""];
			set => this[""] = value;
		}
	}
}