using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels;
using Ei.Rp.DomainModels.Banking;

namespace EI.RP.DomainServices.Queries.Banking.PaymentCards
{
	 class PaymentCardsQueryHandler : QueryHandler<PaymentCardsQuery>
	{
		private readonly ISapRepositoryOfCrmUmc _repository;

		public PaymentCardsQueryHandler(ISapRepositoryOfCrmUmc repository)
		{
			_repository = repository;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(PaymentCardInfo)};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			PaymentCardsQuery query)
		{
			var response = await _repository
				.NewQuery<AccountDto>()
				.Key(query.Partner)
				.NavigateTo<PaymentCardDto>()
				.GetMany();

			return response.Select(Map).Cast<TQueryResult>();
		}

		private PaymentCardInfo Map(PaymentCardDto dto)
		{
			var map = new PaymentCardInfo();
			map.CardHolder = dto.Cardholder;
			map.Description = dto.Description;
			map.IsStandard = dto.StandardFlag;
			map.Issuer = dto.Issuer;
			return map;
		}
	}
}