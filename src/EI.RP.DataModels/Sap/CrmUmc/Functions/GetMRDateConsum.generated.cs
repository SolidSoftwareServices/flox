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
    public partial class GetMRDateConsumFunction : ODataFunction<GetMRDateConsumFunction.QueryObject, MRDateConsumDto>
    {
        public GetMRDateConsumFunction(params Expression<Func<MRDateConsumDto, object>>[] expands): base("GetMRDateConsum", expands)
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
            [StringLength(10)]
            public virtual string AccountID
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(50)]
            public virtual string PoDExtUIElec
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(50)]
            public virtual string PoDExtUIGas
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(5)]
            public virtual string ProcessType
            {
                get;
                set;
            }

            = string.Empty;
            public virtual bool RealMROnly
            {
                get;
                set;
            }
        }
    }
}