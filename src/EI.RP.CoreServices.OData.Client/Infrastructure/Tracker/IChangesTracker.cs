using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.Tracker
{
	internal interface IChangesTracker
	{
		void TrackInstance<TDto>(TDto updateable) where TDto : class;
		void Detach<TDto>(TDto changedEntity) where TDto : class;
		IDictionary<string, object> GetChanges<TDto>(TDto changedEntity) where TDto : ODataDtoModel;
	}
}