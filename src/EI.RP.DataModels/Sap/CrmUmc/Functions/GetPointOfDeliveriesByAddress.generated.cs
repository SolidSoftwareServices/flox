using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;

namespace EI.RP.DataModels.Sap.CrmUmc.Functions
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    public partial class GetPointOfDeliveriesByAddressFunction : ODataFunction<GetPointOfDeliveriesByAddressFunction.QueryObject, PointOfDeliveryDto>
    {
        public GetPointOfDeliveriesByAddressFunction(params Expression<Func<PointOfDeliveryDto, object>>[] expands): base("GetPointOfDeliveriesByAddress", expands)
        {
        }

        public override bool ReturnsComplexType() => false;
        public override bool ReturnsCollection() => true;
        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        public class QueryObject
        {
            [StringLength(40)]
            public virtual string City
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(3)]
            public virtual string CountryID
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string HouseNo
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string PostalCode
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(3)]
            public virtual string Region
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string RoomNo
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(60)]
            public virtual string Street
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(2)]
            public virtual string DivisionID
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}