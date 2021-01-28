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
    [CrudOperations(CanAdd = false, CanUpdate = true, CanDelete = false, CanQuery = true)]
    public partial class AccountDto : ODataDtoModel
    {
        public override string CollectionName() => "Accounts";
        public override object[] UniqueId()
        {
            return new object[]{AccountID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
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
        [StringLength(4)]
        public virtual string AccountTitleID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string FirstName
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string LastName
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string MiddleName
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string SecondName
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string AccountTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string Sex
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Name1
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Name2
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Name3
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Name4
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string GroupName1
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string GroupName2
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(80)]
        public virtual string FullName
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string CorrespondenceLanguage
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string CorrespondenceLanguageISO
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string Language
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string LanguageISO
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? BirthDate
        {
            get => _BirthDate;
            set => _BirthDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _BirthDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string PartnerType
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Username
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual List<ContractItemDto> ContractItems
        {
            get;
            set;
        }

        = new List<ContractItemDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressIndependentEmailDto> AccountAddressIndependentEmails
        {
            get;
            set;
        }

        = new List<AccountAddressIndependentEmailDto>();
        [Sortable]
        [Filterable]
        public virtual AccountAddressDto StandardAccountAddress
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<AccountAddressDto> AccountAddresses
        {
            get;
            set;
        }

        = new List<AccountAddressDto>();
        [Sortable]
        [Filterable]
        public virtual List<PaymentCardDto> PaymentCards
        {
            get;
            set;
        }

        = new List<PaymentCardDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressIndependentMobilePhoneDto> AccountAddressIndependentMobilePhones
        {
            get;
            set;
        }

        = new List<AccountAddressIndependentMobilePhoneDto>();
        [Sortable]
        [Filterable]
        public virtual AccountSexDto AccountSex
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<BankAccountDto> BankAccounts
        {
            get;
            set;
        }

        = new List<BankAccountDto>();
        [Sortable]
        [Filterable]
        public virtual List<BusinessAgreementDto> BusinessAgreements
        {
            get;
            set;
        }

        = new List<BusinessAgreementDto>();
        [Sortable]
        [Filterable]
        public virtual AccountTitleDto AccountTitle
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<InteractionRecordDto> InteractionRecords
        {
            get;
            set;
        }

        = new List<InteractionRecordDto>();
        [Sortable]
        [Filterable]
        public virtual List<QuotationItemDto> QuotationItems
        {
            get;
            set;
        }

        = new List<QuotationItemDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressIndependentPhoneDto> AccountAddressIndependentPhones
        {
            get;
            set;
        }

        = new List<AccountAddressIndependentPhoneDto>();
        [Sortable]
        [Filterable]
        public virtual List<CommunicationPermissionDto> CommunicationPermissions
        {
            get;
            set;
        }

        = new List<CommunicationPermissionDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressIndependentFaxDto> AccountAddressIndependentFaxes
        {
            get;
            set;
        }

        = new List<AccountAddressIndependentFaxDto>();
        [Sortable]
        [Filterable]
        public virtual AccountTypeDto AccountType
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<BusinessActivityDto> BusinessActivities
        {
            get;
            set;
        }

        = new List<BusinessActivityDto>();
    }
}