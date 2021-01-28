using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Http.Session;
using Ei.Rp.DomainModels.MoveHouse;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	public interface IMovingHouseValidator
	{
		Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query);
		string ResolveCacheKey(MovingHouseValidationQuery query);
	}

	
}