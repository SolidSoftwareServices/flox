using Microsoft.AspNetCore.Hosting;

namespace Ei.Rp.Web.DebugTools
{
	public static class HostingEnvironmentExtensions
	{
		public static bool IsTestEnvironment(this IHostingEnvironment env)
		{
			return env.EnvironmentName == "Test";
		}
	}
}