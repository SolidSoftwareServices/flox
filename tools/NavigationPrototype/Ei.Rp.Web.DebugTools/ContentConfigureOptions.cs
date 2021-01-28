using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Ei.Rp.Web.DebugTools
{
    class ContentConfigureOptions : IPostConfigureOptions<StaticFileOptions>
    {
        private readonly IHostingEnvironment _environment;

        public ContentConfigureOptions(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public void PostConfigure(string name, StaticFileOptions options)
        {
            // Basic initialization in case the options weren't initialized by any other component
            options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();


            if (options.FileProvider == null && _environment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider = options.FileProvider ?? _environment.WebRootFileProvider;


			options.FileProvider = new CompositeFileProvider(options.FileProvider, new ManifestEmbeddedFileProvider(GetType().Assembly)); 


			_environment.WebRootFileProvider = options.FileProvider; // required to make asp-append-version work as it uses the WebRootFileProvider. https://github.com/aspnet/Mvc/issues/7459
        }
    }
}
