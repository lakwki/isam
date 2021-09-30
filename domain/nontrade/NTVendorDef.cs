using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.domain.types;
using com.next.common.datafactory.worker;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public partial class NTVendorDef : DomainData
    {
        public NTVendorDef()
        {
            EPVendorCode = string.Empty;
            VendorTypeId = 0;
            WorkflowStatus = NTVendorWFS.DRAFT;
            Status = GeneralCriteria.TRUE;
        }

        public int NTVendorId { get; set; }
        public string VendorName { get; set; }
        public string OtherName { get; set; }
        public string Address { get; set; }
        public string SUNAccountCode { get; set; }
        public NTPaymentMethodRef PaymentMethod { get; set; }
        public CurrencyRef Currency { get; set; }
        public CountryRef Country { get; set; }
        public NTExpenseTypeRef ExpenseType { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string BankAccountNo { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string TallyCode { get; set; }
        public OfficeRef Office { get; set; }
        public int Status { get; set; }
        public int PaymentTermDays { get; set; }
        public string Remark { get; set; }
        public int IsInvoiceNoRequired { get; set; }
        public int IsCustomerNoRequired { get; set; }
        public string EPVendorCode { get; set; }
        public int VendorTypeId { get; set; }
        public UserRef ReviewedBy { get; set; }
        public DateTime ReviewedOn { get; set; }
        public NTVendorWFS WorkflowStatus { get; set; }
        public UserRef CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string AmendmentDetail { get; set; }
        public int ConsumptionUnitId { get; set; }
        public int UtilityProviderTypeId { get; set; }
        public int CompanyId { get; set; }
        public string EAdviceEmail { get; set; }

        public string VendorNameWithCompany
        {
            get { return VendorName + (CompanyId != -1 ? string.Format(" ({0})", GeneralWorker.Instance.getCompanyOfficeMappingByCriteria(CompanyId, Office.OfficeId).EpicorCompanyId)  : string.Empty); }
        }
    }

}
