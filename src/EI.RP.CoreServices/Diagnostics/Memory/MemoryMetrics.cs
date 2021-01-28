using System.Collections.Generic;
using System.Text;

namespace EI.RP.CoreServices.Diagnostics.Memory
{
	public class MemoryMetrics
	{
		public double Total { get; set; }
		public double Used{ get; set; }
		public double Free{ get; set; }
	}
}