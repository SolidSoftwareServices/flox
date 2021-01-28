using System.Reflection;
using System.Threading;
using AutoFixture.Kernel;
using Ei.Rp.DomainModels.Billing;

namespace EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders
{
	public class AccountBillingActivitySpecimenBuilder : ISpecimenBuilder
	{
		

		private static int _counter = 0;
		public object Create(object request, ISpecimenContext context)
		{
			
			var pi = request as ParameterInfo;
			if (pi == null)
				return new NoSpecimen();

			if (pi.Member.DeclaringType != typeof(AccountBillingActivity) ||
			    pi.ParameterType != typeof(AccountBillingActivity.ActivitySource))
				return new NoSpecimen();

			var current = Interlocked.Increment(ref _counter);

			return (AccountBillingActivity.ActivitySource)(current % 2) + 1;
		}
	}


}