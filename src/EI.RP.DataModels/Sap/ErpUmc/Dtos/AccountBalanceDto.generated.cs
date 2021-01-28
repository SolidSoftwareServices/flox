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
    public partial class AccountBalanceDto : ODataDtoModel
    {
        public override string CollectionName() => "AccountBalances";
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
        public virtual decimal CurrentBalance
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal TotalAmount
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal NetAmount
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal OpenCollectable
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal OpenDebits
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal OpenCredits
        {
            get;
            set;
        }

        [StringLength(5)]
        public virtual string Currency
        {
            get;
            set;
        }
    }
}