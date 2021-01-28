using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Infrastructure.Mappers
{
	

	interface IDomainMapper<in TDataModel, TDomainModel> where TDomainModel : IQueryResult
	{
		Task<TDomainModel> Map(TDataModel dataModel);
	}

	interface IDomainMapper<in TDataModel1, in TDataModel2, TDomainModel> where TDomainModel : IQueryResult
	{
		Task<TDomainModel> Map(TDataModel1 dataModel, TDataModel2 arg1);
	}

	interface IDomainMapper
	{
		Task<TDomainModel> Map<TDataModel, TDomainModel> (TDataModel dataModel) where TDomainModel : IQueryResult;
		Task<TDomainModel> Map<TDataModel1,TDataModel2, TDomainModel> (TDataModel1 dataModel1,TDataModel2 dataModel2) where TDomainModel : IQueryResult;
	}
}