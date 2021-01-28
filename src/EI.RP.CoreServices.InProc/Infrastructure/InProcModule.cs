using Autofac;
using EI.RP.CoreServices.IoC.Autofac;

namespace EI.RP.CoreServices.InProc.Infrastructure
{
    public class InProcModule : BaseModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces().WithInterfaceProfiling();
        }
    }
}
