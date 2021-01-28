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
    public partial class ZMoveOutFunction : ODataFunction<ZMoveOutFunction.QueryObject, ContractItemDto>
    {
        public ZMoveOutFunction(params Expression<Func<ContractItemDto, object>>[] expands): base("ZMoveOut", expands)
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
            public virtual string ElecContractID
            {
                get;
                set;
            }

            = string.Empty;
            public virtual string GasContractID
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
            public virtual decimal ElecMOMRReg1
            {
                get;
                set;
            }

            public virtual decimal ElecMOMRReg2
            {
                get;
                set;
            }

            public virtual decimal GasMOMRReg1
            {
                get;
                set;
            }
        }
    }
}