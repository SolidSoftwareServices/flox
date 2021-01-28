
namespace S3.CoreServices.Platform
{
	public interface IPlatformSettings 
	{

		bool ProfileInDetail { get; }
		bool IsDeferredComponentLoadEnabled { get; }

		string DeferredComponentLoaderViewEmbeddedResourceName { get; }
		bool IsProfilerEnabled { get; }
	}
}