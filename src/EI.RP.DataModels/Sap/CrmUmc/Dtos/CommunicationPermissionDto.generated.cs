using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.CrmUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = true, CanUpdate = false, CanDelete = true, CanQuery = true)]
    public partial class CommunicationPermissionDto : ODataDtoModel
    {
        public override string CollectionName() => "CommunicationPermissions";
        public override object[] UniqueId()
        {
            return new object[]{CommunicationPermissionGUID, AccountID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(22)]
        public virtual string CommunicationPermissionGUID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string CommunicationChannelID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string CommunicationPermissionStatusID
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
        [StringLength(241)]
        public virtual string CommunicationDetail
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? ValidFrom
        {
            get => _ValidFrom;
            set => _ValidFrom = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ValidFrom;
        [Sortable]
        [Filterable]
        public virtual CommunicationChannelDto CommunicationChannel
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual CommunicationPermissionStatusDto CommunicationPermissionStatus
        {
            get;
            set;
        }
    }
}