using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.System;

namespace EI.RP.CoreServices.IoC.Autofac
{
	public abstract class BaseModule : Module
	{

//#if DEBUG || FrameworkDeveloper
//		protected override void AttachToRegistrationSource(IComponentRegistryBuilder componentRegistry, IRegistrationSource registrationSource)
//		{

//			base.AttachToRegistrationSource(componentRegistry, registrationSource);
//		}

//		protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
//		{
			
//			registration.Preparing += RegistrationOnPreparing;
//			registration.Activated += RegistrationActivated;

//			base.AttachToComponentRegistration(componentRegistry, registration);
//		}



//		private readonly ConcurrentDictionary<Guid, IDisposable> _resolutionProfilings = new ConcurrentDictionary<Guid, IDisposable>();

//		private void RegistrationOnPreparing(object sender, PreparingEventArgs preparingEventArgs)
//		{

//			var type = preparingEventArgs.Component.Activator.LimitType;
//			if (type.Namespace == null || !type.Namespace.StartsWith("ei.rp", StringComparison.InvariantCultureIgnoreCase) || type.Implements(typeof(IProfiler))) return;

//			var collection = type.FullName.Split('.');
//			var disposable = preparingEventArgs.Context.Resolve<IProfiler>().RecordStep($"IoC Resolution of {string.Join(".", collection.Skip(Math.Max(0, collection.Length - 2)))}");
//			_resolutionProfilings.GetOrAdd(preparingEventArgs.Component.Id, disposable);
//		}

//		private void RegistrationActivated(object sender, ActivatedEventArgs<object> activatedEventArgs)
//		{

//			var type = activatedEventArgs.Component.Activator.LimitType;

//			if (type.Namespace == null || !type.Namespace.StartsWith("ei.rp", StringComparison.InvariantCultureIgnoreCase) || type.Implements(typeof(IProfiler))) return;

//			var valueTuple = _resolutionProfilings[activatedEventArgs.Component.Id];

//			valueTuple?.Dispose();
//			_resolutionProfilings.TryRemove(activatedEventArgs.Component.Id, out var x);
//		}
//#endif

	}
}