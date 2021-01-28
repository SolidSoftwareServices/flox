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
    [CrudOperations(CanAdd = false, CanUpdate = true, CanDelete = false, CanQuery = true)]
    public partial class UserDto : ODataDtoModel
    {
        public override string CollectionName() => "UserCollection";
        public override object[] UniqueId()
        {
            return new object[]{UserName, };
        }

        public override string GetEntityContainerName()
        {
            return "USERMANAGEMENT";
        }

        [Sortable]
        [Filterable]
        public virtual ProfileDto UserProfile
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual AddressDto UserAddress
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string UserName
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(30)]
        [Sortable]
        [Filterable]
        public virtual string Title
        {
            get;
            set;
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string FirstName
        {
            get;
            set;
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string LastName
        {
            get;
            set;
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string NickName
        {
            get;
            set;
        }

        [StringLength(241)]
        [Sortable]
        [Filterable]
        public virtual string Email
        {
            get;
            set;
        }

        [StringLength(42)]
        [Sortable]
        [Filterable]
        public virtual string Company
        {
            get;
            set;
        }

        [StringLength(30)]
        [Sortable]
        [Filterable]
        public virtual string PhoneNumber
        {
            get;
            set;
        }
    }
}