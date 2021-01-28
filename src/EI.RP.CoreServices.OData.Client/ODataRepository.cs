using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Batching;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.OData.Client.Infrastructure.Batches;
using EI.RP.CoreServices.OData.Client.Infrastructure.Edmx;
using EI.RP.CoreServices.OData.Client.Infrastructure.Tracker;
using EI.RP.CoreServices.OData.Client.Infrastructure.Validation;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.Profiling;
using Simple.OData.Client;

#if !FrameworkDeveloper
using System.Diagnostics;

#endif
namespace EI.RP.CoreServices.OData.Client
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]

#endif

	public abstract partial class ODataRepository:IODataRepository
	{
		protected ODataRepositoryOptions Options { get; }
		protected string ProfileCategoryId => GetType().Name;

		private readonly IChangesTracker _changesTracker;
		
		private readonly Lazy<ODataClient> _client;

		private readonly IProxyModelValidator _modelValidator;
		protected IProfiler Profiler { get; }

		protected ODataRepository(ODataRepositoryOptions options)
		{
			Options = options;
			BatchEnlistTimeoutMilliseconds = options.BatchEnlistTimeoutMilliseconds;
			_modelValidator = options.ModelValidator;
			_edmxResolver = options.EdmxResolver;
			Profiler = options.Profiler;
			_changesTracker = new ChangesTracker(options.ODataSettings);

			EndpointUrl = options.EndpointUrl;
			_client = new Lazy<ODataClient>(() => new ODataClient(BuildClientSettings(options.EndpointUrl)));

			CreateNewBatch();
		}

		
		private double BatchEnlistTimeoutMilliseconds { get; }
		protected string EndpointUrl { get; }
		protected ODataClient Client => _client.Value;

		protected abstract ODataClientSettings BuildClientSettings(string endPointUrl);

		public abstract Task<string> GetName();

		private async Task ThrowIfTypeIsNotDeclaredInContainer<TEntityContainerItem>() where TEntityContainerItem : EntityContainerItem,new()
		{
			await ThrowIfTypeIsNotDeclaredInContainer(new TEntityContainerItem());
		}

		private async Task ThrowIfTypeIsNotDeclaredInContainer<TEntityContainerItem>(TEntityContainerItem instance)
			where TEntityContainerItem : EntityContainerItem
		{
			var entityContainerName = instance.GetEntityContainerName();
			var repoContainerName = await this.GetEntityContainerName();
			if (entityContainerName!=null&& entityContainerName != repoContainerName)
				throw new InvalidOperationException($"The type {typeof(TEntityContainerItem).FullName} is not declared in the odata entity container {repoContainerName}");
		}
	}
}