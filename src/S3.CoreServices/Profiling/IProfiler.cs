using System;

namespace S3.CoreServices.Profiling
{
	public interface IProfiler
	{
		IDisposable RecordStep(string stepId);

		IDisposable Profile(string categoryId, string commandId);
	}
}
