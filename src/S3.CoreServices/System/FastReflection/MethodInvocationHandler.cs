using System;
using System.Reflection;
using Fasterflect;

namespace S3.CoreServices.System.FastReflection
{
	public class MethodInvocationHandler
	{
		
		public Type[] ArgumentTypes { get; set; }

		public MethodInfo Info => _invoker.Method;

		private readonly MethodInvoker _invoker;

		public MethodInvocationHandler(MethodInvoker invoker)
		{
			_invoker = invoker;
		}

		public object Execute(object instance, params object[] parameters) => Execute<object>(instance, parameters);
		public TResult Execute<TResult>(object instance,params object[] parameters )
		{
			return (TResult)_invoker.Invoke(instance, parameters);
		}
	}
}