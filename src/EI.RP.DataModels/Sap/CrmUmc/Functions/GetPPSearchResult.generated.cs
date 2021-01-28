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
    public partial class GetPPSearchResultFunction : ODataFunction<GetPPSearchResultFunction.QueryObject, ProductDto>
    {
        public GetPPSearchResultFunction(params Expression<Func<ProductDto, object>>[] expands): base("GetPPSearchResult", expands)
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
            [StringLength(1)]
            public virtual string Online
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(1)]
            public virtual string DuelFuel
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(5)]
            public virtual string Creditworth
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(2)]
            public virtual string CustomerScore
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(5)]
            public virtual string Ex_Gratia
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(2)]
            public virtual string XDivision01
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(1)]
            public virtual string DirectDebit_Elec
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(4)]
            public virtual string Duos_Group
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(2)]
            public virtual string MeterType_Elec
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(5)]
            public virtual string SalesType_E
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(1)]
            public virtual string QuarterlyHour
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string E_Reg_Config
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(2)]
            public virtual string XDivision02
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(1)]
            public virtual string DirectDebit_Gas
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(1)]
            public virtual string Band
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(2)]
            public virtual string MeterType_Gas
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(5)]
            public virtual string SalesType_G
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}