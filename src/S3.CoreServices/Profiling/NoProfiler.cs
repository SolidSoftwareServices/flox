using System;

namespace S3.CoreServices.Profiling
{
	public class NoProfiler : IProfiler
	{
		private class Disposable : IDisposable
		{
			public void Dispose()
			{
			}
		}
		public IDisposable RecordStep(string stepId)
		{
			return new Disposable();
		}

		public IDisposable Profile(string categoryId, string commandId)
		{
			return new Disposable();
		}
	}
}