using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;

namespace Ei.Rp.DomainModels.Contracts
{
    public class BusinessAgreement : IQueryResult
    {
	    public string BusinessAgreementId { get; set; }

		public PaymentMethodType IncomingPaymentMethodType { get; set; }
        public string CollectiveParentId { get; set; }
        public string AlternativePayerId { get; set; }
		public string AlternativePayerCA { get; set; }
		public AddressInfo BillToAccountAddress { get; set; }
        public string Language { get; set; }
        public IEnumerable<ContractItem> Contracts { get; set; }
        public string Description { get; set; }
        public string AccountCategory { get; set; }
        public string AccountDeterminationID { get; set; }

        public int? FixedBillingDateDay { get; set; }

        public bool IsEBiller { get;set; }


        public bool AllowsEqualizer()
        {
	        return IncomingPaymentMethodType != PaymentMethodType.DirectDebit &&
	               IncomingPaymentMethodType != PaymentMethodType.Equalizer &&
	               string.IsNullOrEmpty(AlternativePayerId) &&
	               AccountCategory == DivisionType.Electricity &&
	               string.IsNullOrEmpty(CollectiveParentId) && 
	               AccountDeterminationID == AccountDeterminationType.ResidentialCustomer;

        }
    }
}