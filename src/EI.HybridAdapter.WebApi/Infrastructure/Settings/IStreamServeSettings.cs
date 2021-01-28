
namespace EI.HybridAdapter.WebApi.Infrastructure.Settings
{
	public interface IStreamServeSettings
	{
		string StreamServeLive { get; }
		string StreamServeArchive { get; }
		string StreamServeUserName { get; }
		string StreamServePassword { get; }
	}
}