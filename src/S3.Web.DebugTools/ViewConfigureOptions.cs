using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace S3.Web.DebugTools
{
	class ViewConfigureOptions : IPostConfigureOptions<MvcRazorRuntimeCompilationOptions>
	{
		private readonly IWebHostEnvironment _environment;

		public ViewConfigureOptions(IWebHostEnvironment environment)
		{
			_environment = environment;
		}

		public void PostConfigure(string name, MvcRazorRuntimeCompilationOptions options)
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