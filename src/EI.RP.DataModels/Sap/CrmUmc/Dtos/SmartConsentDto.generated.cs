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
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class SmartConsentDto : ODataDtoModel
    {
        public override string CollectionName() => "SmartConsents";
        public override object[] UniqueId()
        {
            return new object[]{GUID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        public virtual Guid GUID
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string ParentID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string ParentItemPosition
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime ParentStartDate
        {
            get => _ParentStartDate;
            set => _ParentStartDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _ParentStartDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string PermissionStatusID
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
        [StringLength(50)]
        public virtual string PointOfDeliveryExternalID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime PermissionStartDate
        {
            get => _PermissionStartDate;
            set => _PermissionStartDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _PermissionStartDate;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime PermissionEndDate
        {
            get => _PermissionEndDate;
            set => _PermissionEndDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _PermissionEndDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string GivenBy
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string ChannelID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime CreatedOn
        {
            get => _CreatedOn;
            set => _CreatedOn = CompensateSapDateTimeBug(value);
        }

        private DateTime _CreatedOn;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string Cancelled
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(16)]
        public virtual string LastProcess
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AmendedBy
        {
            get;
            set;
        }

        = string.Empty;
    }
}