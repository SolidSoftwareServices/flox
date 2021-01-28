using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace S3.CoreServices.System.Async
{
	public class AsyncLazy<T> : Lazy<Task<T>>
	{
		public AsyncLazy(T value) :
			this(() => Task.FromResult(value)) { }
		public AsyncLazy(Func<T> valueFactory) :
			base(() => Task.Factory.StartNew(valueFactory)) { }

		public AsyncLazy(Func<Task<T>> taskFactory) :
			base(() => Task.Factory.StartNew(taskFactory).Unwrap()) { }

		public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
	}

	
}