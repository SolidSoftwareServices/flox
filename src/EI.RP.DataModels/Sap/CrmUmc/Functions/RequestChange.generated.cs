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
    public partial class RequestChangeFunction : ODataFunction<RequestChangeFunction.QueryObject, InteractionRecordDto>
    {
        public RequestChangeFunction(params Expression<Func<InteractionRecordDto, object>>[] expands): base("RequestChange", expands)
        {
        }

        public override bool ReturnsComplexType() => false;
        public override bool ReturnsCollection() => false;
        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        public class QueryObject
        {
            [StringLength(32)]
            public virtual string Attribute2Name
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(120)]
            public virtual string Attribute2Value
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(32)]
            public virtual string Attribute1Name
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string InteractionRecordID
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(120)]
            public virtual string Attribute1Value
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}