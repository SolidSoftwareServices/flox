using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.SmartActivation
{
	public class SmartActivationNotification :IQueryResult
    {
	    public bool IsNotificationDismissed { get; set; }
    }
}
