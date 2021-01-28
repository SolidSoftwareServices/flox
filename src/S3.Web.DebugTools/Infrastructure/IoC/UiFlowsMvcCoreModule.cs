using Autofac;
using S3.Web.DebugTools.Flows.AppFlows.Components.FlowViewer.Resolvers;
using S3.Web.DebugTools.Infrastructure.FlowDiagnostics;
using Module = Autofac.Module;


namespace S3.Web.DebugTools.Infrastructure.IoC
{




	public class UiFlowsDebugToolsModule:Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly)
				.Where(x => x.Namespace == typeof(DebuggerFlowGraphResolver).Namespace).AsImplementedInterfaces();
			builder.RegisterType<FlowChangesRecorder>().AsImplementedInterfaces().SingleInstance();
		}
	}
}