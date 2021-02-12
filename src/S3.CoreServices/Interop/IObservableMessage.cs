using System;

namespace S3.CoreServices.Interop
{
	public interface IObservableMessage
	{
		Guid ReceptionBatchId { get; set; }
	}
}