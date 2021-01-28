namespace EI.RP.DomainServices.Infrastructure.Settings
{
	public interface IDomainSettings 
	{
		bool IsInternalDeployment { get; }

		bool IsSmartActivationEnabled { get; }

	
	}
}
