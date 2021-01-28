using System;

namespace EI.RP.CoreServices.Profiling
{
	public interface IProfiler
	{
		IDisposable RecordStep(string stepId);

		IDisposable Profile(string categoryId, string commandId);
	}
}
