namespace S3.UiFlows.Core.Facade.FlowResultResolver
{
	public interface IFlowResultResolverRequestHandler<TResult> : IFlowRequestHandler<FlowResultResolverRequest<TResult>, TResult>
	{

	}
}