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
    [CrudOperations(CanAdd = true, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class PaymentDocumentItemDto : ODataDtoModel
    {
        public override string CollectionName() => "PaymentDocumentItems";
        public override object[] UniqueId()
        {
            return new object[]{PaymentDocumentID, InvoiceID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(24)]
        public virtual string PaymentDocumentID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string InvoiceID
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? ExecutionDate
        {
            get => _ExecutionDate;
            set => _ExecutionDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ExecutionDate;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal Amount
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