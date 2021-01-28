namespace EI.RP.CoreServices.OData.Client.Infrastructure.Validation
{
	public interface IProxyModelValidator
	{
		void Validate<TModel>(TModel model, ProxyModelOperation operation);
	}
}