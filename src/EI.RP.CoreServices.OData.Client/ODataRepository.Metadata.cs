using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.OData.Client.Infrastructure.Edmx;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using Microsoft.OData.Edm;

namespace EI.RP.CoreServices.OData.Client
{
	public abstract partial class ODataRepository
	{
		private readonly IEdmxResolver _edmxResolver;
		private readonly MetadatasRepository _metadatas=new MetadatasRepository();

		private string _entityContainerName;

		public async Task<string> GetEntityContainerName()
		{
				return _entityContainerName ?? (_entityContainerName =
					await GetName());

				async Task<string> GetName()
				{
					return (await ResolveMetadata()).EdmModel.EntityContainer.Namespace;
				}
		}

		public async Task<ODataRepositoryMetadata> ResolveMetadata(CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _metadatas.GetOrAdd(Options, async() =>  await OnResolveMetadata(cancellationToken),
				_edmxResolver,cancellationToken);
			
		}

		public abstract Task<string> OnResolveMetadata(CancellationToken cancellationToken = default(CancellationToken));

		private class MetadatasRepository
		{
			private static readonly ConcurrentDictionary<string,ODataRepositoryMetadata> Metadatas=new ConcurrentDictionary<string, ODataRepositoryMetadata>();


			public async Task<ODataRepositoryMetadata> GetOrAdd(ODataRepositoryOptions options, Func<Task<string>> edmxProvider, IEdmxResolver edmxResolver,CancellationToken cancellationToken = default(CancellationToken))
			{
				var metadata = await Metadatas.GetOrAddAsync(options.EndpointUrl, async () =>
					{
						var edmxString = (await edmxProvider())
							//fixes malformations
							.Replace("Type=\"Edm.DateTime\" Mode=\"In\" Precision=\"7\"", "Type=\"Edm.DateTime\" Mode=\"In\" ")
							.Replace("Type=\"Edm.DateTime\" Mode=\"In\" Precision=\"0\"", "Type=\"Edm.DateTime\" Mode=\"In\" ");
						var entry = new ODataRepositoryMetadata(edmxString, edmxResolver.Parse2(edmxString));

						return entry;
					},
					cancellationToken);
				return metadata;
			}

			public ODataRepositoryMetadata Get(ODataRepositoryOptions options)
			{
				return Metadatas[options.EndpointUrl];
			}
		}

	}


	
}