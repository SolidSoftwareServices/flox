using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Serialization;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.ResidentialPortal;
using Newtonsoft.Json.Linq;

namespace EI.RP.DataStore.SmartPlans
{
	class SmartPlansFileSource : ISmartPlansFileSource
	{ 
		private readonly ICacheProvider _cacheProvider;

		public SmartPlansFileSource(ICacheProvider cacheProvider)
		{
			_cacheProvider = cacheProvider;
		}

		public async Task<IEnumerable<SmartActivationPlanDataModel>> ReadFileData()
		{
			return await _cacheProvider.GetOrAddAsync($"SmartActivationPlans", ()=>GetPlans(),
				maxDurationFromNow: TimeSpan.FromMinutes(30));

			Task<IEnumerable<SmartActivationPlanDataModel>> GetPlans()
			{
				var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				var filePath=Path.Combine(dirPath??string.Empty,"SmartPlans", "SmartPlansData.json");
				ThrowIfFileNotExists(filePath);
				var src = File.ReadAllText(filePath).JsonToObject<SerializedData>();

				return Task.FromResult(src.Plans);
		
			}

			void ThrowIfFileNotExists(string filePath)
			{
				if (!File.Exists(filePath))
				{
					throw new InvalidOperationException("Could not find SmartPlansData.json");
				}
			}
		}

		private class SerializedData
		{
			public IEnumerable<SmartActivationPlanDataModel> Plans { get; set; }
		}
	}
}