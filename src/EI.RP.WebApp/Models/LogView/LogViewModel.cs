using System.Collections.Generic;
using EI.RP.WebApp.Models.Shared;

namespace EI.RP.WebApp.Models.LogView
{
	public class LogViewModel : LayoutWithNavModel
	{
		public IEnumerable<LogFileModel> LogFiles { get; set; }
	}
}
