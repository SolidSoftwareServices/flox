using System.Configuration;

namespace EI.RP.DomainServices.Infrastructure.Util
{
	[ConfigurationCollection(typeof(CustomConfigurationElement),
		AddItemName = "add",
		RemoveItemName = "remove",
		ClearItemsName = "clear")]
	public class CustomConfigurationElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new CustomConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((CustomConfigurationElement) element).Id;
		}
	}
}