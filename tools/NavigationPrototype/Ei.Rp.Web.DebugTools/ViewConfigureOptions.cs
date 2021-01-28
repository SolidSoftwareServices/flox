using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Ei.Rp.Web.DebugTools
{
	class ViewConfigureOptions : IPostConfigureOptions<RazorViewEngineOptions>
	{
		private readonly IHostingEnvironment _environment;

		public ViewConfigureOptions(IHostingEnvironment environment)
		{
			_environment = environment;
		}

		public void PostConfigure(string name, RazorViewEngineOptions options)
		{
#if DEBUG
			// Looks for the physical file on the disk so it can pick up any view changes.
			var path = Path.Combine(_environment.ContentRootPath,
				$"..\\{GetType().Assembly.GetName().Name}");
			var path2 = Path.Combine(_environment.ContentRootPath,
				$"..\\..\\tools\\NavigationPrototype\\{GetType().Assembly.GetName().Name}");
			if (Directory.Exists(path))
			{
				options.FileProviders.Add(new PhysicalFileProvider(path));
			}
			else if (Directory.Exists(path2))
			{
				options.FileProviders.Add(new PhysicalFileProvider(path2));
			}
			else
			{
				throw new DirectoryNotFoundException(
					$"none of the following directories were found, at least one is expected : {path}, {path2}");
			}
#endif
		}
	}
}