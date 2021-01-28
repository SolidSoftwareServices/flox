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
    public partial class MoveOutFunction : ODataFunction<MoveOutFunction.QueryObject, ContractItemDto>
    {
        public MoveOutFunction(params Expression<Func<ContractItemDto, object>>[] expands): base("MoveOut", expands)
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
            public virtual DateTime MoveOutDate
            {
                get => _MoveOutDate;
                set => _MoveOutDate = CompensateSapDateTimeBug(value);
            }

            private DateTime _MoveOutDate;
            [StringLength(10)]
            public virtual string ContractID
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}