using System;

namespace EI.RP.CoreServices.Interop
{
	public interface IObservableMessage
	{
		Guid ReceptionBatchId { get; set; }
	}
}