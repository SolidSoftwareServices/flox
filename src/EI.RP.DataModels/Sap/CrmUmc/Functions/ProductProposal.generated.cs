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
    public partial class ProductProposalFunction : ODataFunction<ProductProposalFunction.QueryObject, ProductProposalResultDto>
    {
        public ProductProposalFunction(params Expression<Func<ProductProposalResultDto, object>>[] expands): base("ProductProposal", expands)
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
            public virtual decimal GasAnnualConsumption
            {
                get;
                set;
            }

            [StringLength(2)]
            public virtual string GasMeterType
            {
                get;
                set;
            }

            = string.Empty;
            public virtual bool GasDirectDebit
            {
                get;
                set;
            }

            [StringLength(1)]
            public virtual string GasBand
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(5)]
            public virtual string GasSalesType
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(12)]
            public virtual string GasBusinessAgreementID
            {
                get;
                set;
            }

            = string.Empty;
            public virtual bool GasFlag
            {
                get;
                set;
            }

            public virtual decimal ElecAnnualConsumption
            {
                get;
                set;
            }

            public virtual bool ElecQuarterHourly
            {
                get;
                set;
            }

            [StringLength(2)]
            public virtual string ElecCTF
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string ElecRegisterConfig
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(2)]
            public virtual string ElecMeterType
            {
                get;
                set;
            }

            = string.Empty;
            public virtual bool ElecDirectDebit
            {
                get;
                set;
            }

            [StringLength(4)]
            public virtual string ElecDuOSGroup
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(5)]
            public virtual string ElecSalesType
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(12)]
            public virtual string ElecBusinessAgreementID
            {
                get;
                set;
            }

            = string.Empty;
            public virtual bool ElecFlag
            {
                get;
                set;
            }

            [StringLength(4)]
            public virtual string BonusAmount
            {
                get;
                set;
            }

            = string.Empty;
            public virtual bool PaperlessBilling
            {
                get;
                set;
            }

            [StringLength(10)]
            public virtual string AccountID
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string SalesOrigin
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}