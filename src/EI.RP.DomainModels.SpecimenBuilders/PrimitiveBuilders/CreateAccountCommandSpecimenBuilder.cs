using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using EI.RP.DomainServices.Commands.Users.Membership.CreateAccount;

namespace EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders
{
	public class CreateAccountCommandSpecimenBuilder : ISpecimenBuilder
	{
		public object Create(object request, ISpecimenContext context)
		{
			var pi = request as ParameterInfo;
			if (pi == null)
				return new NoSpecimen();

			if (pi.Member.DeclaringType != typeof(CreateAccountCommand) ||
			    !pi.Name.ToLowerInvariant().Contains("mprn"))
				return new NoSpecimen();

			return context.Create<string>().Substring(0, 6);

		}
	}
}