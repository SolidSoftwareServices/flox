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
    public partial class NotificationDto : ODataDtoModel
    {
        public override string CollectionName() => "NotificationCollection";
        public override object[] UniqueId()
        {
            return new object[]{ID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(32)]
        [Sortable]
        [Filterable]
        public virtual string ID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string collection
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual string title
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual DateTime? updated
        {
            get => _updated;
            set => _updated = CompensateSapDateTimeBug(value);
        }

        private DateTime? _updated;
        [StringLength(30)]
        [Sortable]
        [Filterable]
        public virtual string changeType
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual int? entriesOfInterest
        {
            get;
            set;
        }

        [StringLength(112)]
        [Sortable]
        [Filterable]
        public virtual string recipient
        {
            get;
            set;
        }
    }
}