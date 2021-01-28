using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Infrastructure.Extensions
{
	public static class ContextDataExtensions
	{
		public static string GetLastError(this IUiFlowContextData contextData)
		{
			var result = contextData.LastError?.ExceptionMessage ;
			if (result != null)
			{
				var domainErrorParts = result.Split('-');
				if (domainErrorParts.Length >= 3)
				{
					result = domainErrorParts[1];
				}
			}

			return result;
		}
	}

	
}