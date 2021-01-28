using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.ErpUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class ESAProductBalanceDto : ODataDtoModel
    {
        public override string CollectionName() => "ESAProductBalances";
        public override object[] UniqueId()
        {
            return new object[]{AccountID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string SalesDocID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string ProductID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string ProductDesc
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual bool Subscription
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal MonthlyAmount
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal PaidAmount
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal OutstandAmount
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal TotalGrossAmount
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual short NumOutstandInt
        {
            get;
            set;
        }
    }
}