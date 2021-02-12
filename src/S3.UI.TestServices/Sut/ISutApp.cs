using System;
using System.Threading.Tasks;
using AngleSharp.Dom;
using S3.TestServices.Profilers;
using S3.UI.TestServices.Http;

namespace S3.UI.TestServices.Sut
{
	public interface ISutApp : IDisposable
	{
		ConsoleProfiler Profiler { get; }
		ISutPage CurrentPage { get; }
        Task<ISutApp> ClickOnElement(IElement clickable);
		TResult CurrentPageAs<TResult>() where TResult : ISutPage;
		Task<ISutApp> BrowsePrevious();
		Task<ISutApp> BrowseForward();
		Task<ISutApp> ReloadCurrentPage();
		Lazy<TestHttpClient> Client { get; }

        void Release();

		AppOptions Options { get; }
	}
}