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
    [CrudOperations(CanAdd = true, CanUpdate = true, CanDelete = true, CanQuery = true)]
    public partial class SubscriptionDto : ODataDtoModel
    {
        public override string CollectionName() => "SubscriptionCollection";
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
        public virtual string ID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(12)]
        [Sortable]
        public virtual string user
        {
            get;
            set;
        }

        [Sortable]
        public virtual DateTime? updated
        {
            get => _updated;
            set => _updated = CompensateSapDateTimeBug(value);
        }

        private DateTime? _updated;
        [StringLength(255)]
        [Sortable]
        public virtual string title
        {
            get;
            set;
        }

        [Sortable]
        public virtual string deliveryAddress
        {
            get;
            set;
        }

        [Sortable]
        public virtual bool? persistNotifications
        {
            get;
            set;
        }

        [StringLength(40)]
        [Sortable]
        public virtual string collection
        {
            get;
            set;
        }

        [Sortable]
        public virtual string filter
        {
            get;
            set;
        }

        [StringLength(255)]
        [Sortable]
        public virtual string select
        {
            get;
            set;
        }

        [StringLength(30)]
        [Sortable]
        [Filterable]
        public virtual string changeType
        {
            get;
            set;
        }
    }
}