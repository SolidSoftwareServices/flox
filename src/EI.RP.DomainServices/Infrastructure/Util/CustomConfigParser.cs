using System.Collections.Generic;
using System.Configuration;

namespace EI.RP.DomainServices.Infrastructure.Util
{
	public static class CustomConfigParser
	{
		private static Dictionary<string, Dictionary<string, string>> _configNiUserElements;

		static CustomConfigParser()
		{
			_configNiUserElements = new Dictionary<string, Dictionary<string, string>>();
		}

		public static Dictionary<string, string> LoadConfigEntriesforNIPortalUsers(string sectionName)
		{
			if (_configNiUserElements == null)
				_configNiUserElements = new Dictionary<string, Dictionary<string, string>>();

			if (_configNiUserElements.ContainsKey(sectionName))
				return _configNiUserElements[sectionName];

			var section = ConfigurationManager.GetSection(sectionName) as CustomConfigurationSection;
			var sectionConfigElements = new Dictionary<string, string>();

			if (section != null)
				foreach (CustomConfigurationElement elem in section.ConfigurationElements)
					if (!sectionConfigElements.ContainsKey(elem.Id.Trim()))
						sectionConfigElements.Add(elem.Id.Trim(), elem.Value.Trim());

			if (!_configNiUserElements.ContainsKey(sectionName))
				_configNiUserElements.Add(sectionName, sectionConfigElements);
			return sectionConfigElements;
		}
	}
}