using System;
using AutoFixture.Kernel;
using EI.RP.CoreServices.System;

namespace EI.RP.TestServices.SpecimenGeneration.DefaultSpecimens
{
    public class EuroMoneySpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;
            if (type != typeof(EuroMoney))
                return new NoSpecimen();

            return new EuroMoney((decimal)context.Resolve(typeof(decimal)));
        }
    }
}