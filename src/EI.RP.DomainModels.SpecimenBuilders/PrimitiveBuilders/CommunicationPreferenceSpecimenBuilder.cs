using System.Linq;
using System.Reflection;
using System.Threading;
using AutoFixture.Kernel;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.User;

namespace EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders
{
    public class CommunicationPreferenceSpecimenBuilder : ISpecimenBuilder
    {

        private static readonly CommunicationPreferenceType[] Values =
            CommunicationPreferenceType.AllValues.Cast<CommunicationPreferenceType>().ToArray();

        private static int Index = 0;
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as ParameterInfo;
            if (pi == null)
                return new NoSpecimen();

            if (pi.Member.DeclaringType != typeof(CommunicationPreference) )
                
                return new NoSpecimen();

            var index = Interlocked.Increment(ref Index) % Values.Length;
            
             return Values[index];


        }

    }
}