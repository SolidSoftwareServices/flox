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
    public partial class ChangeProductFunction : ODataFunction<ChangeProductFunction.QueryObject, ContractItemDto>
    {
        public ChangeProductFunction(params Expression<Func<ContractItemDto, object>>[] expands): base("ChangeProduct", expands)
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
            public virtual DateTime ChangeProductDate
            {
                get => _ChangeProductDate;
                set => _ChangeProductDate = CompensateSapDateTimeBug(value);
            }

            private DateTime _ChangeProductDate;
            [StringLength(40)]
            public virtual string NewProductID
            {
                get;
                set;
            }

            = string.Empty;
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