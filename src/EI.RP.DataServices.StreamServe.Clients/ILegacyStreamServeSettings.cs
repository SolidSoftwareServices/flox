
namespace EI.RP.DataServices.StreamServe.Clients
{
	public interface ILegacyStreamServeSettings
	{
		string StreamServeLive { get; }
		string StreamServeArchive { get; }
		string StreamServeUserName { get; }
		string StreamServePassword { get; }
	}
}