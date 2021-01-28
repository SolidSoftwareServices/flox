using System;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;


namespace Ei.Rp.DomainModels.Metering
{
    public partial class PointOfDeliveryInfo : IQueryResult
    {
	    private string _premiseId;
	    public string PointOfDeliveryId { get; set; }

	    public string PremiseId
	    {
		    get => _premiseId;
		    set
		    {
			    if (value != null) _premiseId = value;
			    else throw new NullReferenceException();
		    }
	    }

	    public string Prn { get; set; }

		public AddressInfo AddressInfo { get; set; }
	    public bool IsNewAcquisition { get; set; }

	    public bool IsAddressInSwitch { get; set; }

    }
}
