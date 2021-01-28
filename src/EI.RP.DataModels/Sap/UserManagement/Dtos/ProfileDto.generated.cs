using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.UserManagement.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class ProfileDto : ODataDtoModel
    {
        public override object[] UniqueId()
        {
            return new object[]{};
        }

        public override string GetEntityContainerName()
        {
            return "USERMANAGEMENT";
        }

        [Sortable]
        [Filterable]
        public virtual DateTime? ValidFrom
        {
            get => _ValidFrom;
            set => _ValidFrom = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ValidFrom;
        [Sortable]
        [Filterable]
        public virtual DateTime? ValidTo
        {
            get => _ValidTo;
            set => _ValidTo = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ValidTo;
        [StringLength(12)]
        [Sortable]
        [Filterable]
        public virtual string UserGroup
        {
            get;
            set;
        }

        [StringLength(6)]
        [Sortable]
        [Filterable]
        public virtual string Timezone
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual TimeSpan? LastLogon
        {
            get;
            set;
        }

        [StringLength(1)]
        [Sortable]
        [Filterable]
        public virtual string DateFormat
        {
            get;
            set;
        }

        [StringLength(1)]
        [Sortable]
        [Filterable]
        public virtual string DecimalNotation
        {
            get;
            set;
        }

        [StringLength(2)]
        [Sortable]
        [Filterable]
        public virtual string Language
        {
            get;
            set;
        }

        [StringLength(1)]
        [Sortable]
        [Filterable]
        public virtual string TimeFormat
        {
            get;
            set;
        }
    }
}