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
    public partial class CorrespondenceDto : ODataDtoModel
    {
        public override string CollectionName() => "Correspondences";
        public override object[] UniqueId()
        {
            return new object[]{DocumentID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(32)]
        public virtual string DocumentID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        [Filterable]
        public virtual string ContractAccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        [Filterable]
        public virtual string CorrespondenceTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Filterable]
        public virtual DateTime? CreatedDate
        {
            get => _CreatedDate;
            set => _CreatedDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _CreatedDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(128)]
        public virtual string MimeType
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(255)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual byte[] MediaResource
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual CorrespondenceTypeDto CorrespondenceTypes
        {
            get;
            set;
        }
    }
}