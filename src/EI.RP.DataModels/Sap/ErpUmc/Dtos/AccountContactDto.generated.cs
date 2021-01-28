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
    public partial class AccountContactDto : ODataDtoModel
    {
        public override string CollectionName() => "AccountContacts";
        public override object[] UniqueId()
        {
            return new object[]{AccountContactID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string AccountContactID
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
        [StringLength(4)]
        public virtual string ContactClassID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string ContactActionID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string ContactPriorityID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime ContactDate
        {
            get => _ContactDate;
            set => _ContactDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _ContactDate;
        [Required(AllowEmptyStrings = true)]
        public virtual string Note
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string ContactTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual ContactPriorityDto ContactPriority
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ContactActionDto ContactAction
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ContactClassDto ContactClass
        {
            get;
            set;
        }
    }
}