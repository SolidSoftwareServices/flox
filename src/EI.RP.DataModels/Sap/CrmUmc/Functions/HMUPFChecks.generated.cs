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
    public partial class HMUPFChecksFunction : ODataFunction<HMUPFChecksFunction.QueryObject, HMUPFCheckResultDto>
    {
        public HMUPFChecksFunction(params Expression<Func<HMUPFCheckResultDto, object>>[] expands): base("HMUPFChecks", expands)
        {
        }

        public override bool ReturnsComplexType() => true;
        public override bool ReturnsCollection() => false;
        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        public class QueryObject
        {
            public virtual DateTime MoveInDate
            {
                get => _MoveInDate;
                set => _MoveInDate = CompensateSapDateTimeBug(value);
            }

            private DateTime _MoveInDate;
            public virtual string MIElecPointOfDeliveryID
            {
                get;
                set;
            }

            = string.Empty;
            public virtual string MIGasPointOfDeliveryID
            {
                get;
                set;
            }

            = string.Empty;
            public virtual string MOElecContractID
            {
                get;
                set;
            }

            = string.Empty;
            public virtual string MOGasContractID
            {
                get;
                set;
            }

            = string.Empty;
            public virtual DateTime MoveOutDate
            {
                get => _MoveOutDate;
                set => _MoveOutDate = CompensateSapDateTimeBug(value);
            }

            private DateTime _MoveOutDate;
            [StringLength(1)]
            public virtual string CheckOnly
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}