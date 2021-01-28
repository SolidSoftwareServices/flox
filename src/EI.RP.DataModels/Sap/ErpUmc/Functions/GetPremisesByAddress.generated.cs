using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;

namespace EI.RP.DataModels.Sap.ErpUmc.Functions
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    public partial class GetPremisesByAddressFunction : ODataFunction<GetPremisesByAddressFunction.QueryObject, PremiseDto>
    {
        public GetPremisesByAddressFunction(params Expression<Func<PremiseDto, object>>[] expands): base("GetPremisesByAddress", expands)
        {
        }

        public override bool ReturnsComplexType() => false;
        public override bool ReturnsCollection() => true;
        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        public class QueryObject
        {
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
            [StringLength(40)]
            public virtual string City
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
        }
    }
}