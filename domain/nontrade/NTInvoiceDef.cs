using System;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using System.Web.UI.WebControls;
using com.next.common.datafactory.worker;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public partial class NTInvoiceDef : DomainData
    {
        public NTInvoiceDef()
        {
            SettlementRefNo = string.Empty;
            ChequeNo = string.Empty;
            SUNInterfaceDate = DateTime.MinValue;
            LogoInterfaceDate = DateTime.MinValue;
            ReleaseReason = string.Empty;
            AccountFirstApproverId = -1;
            AccountFirstApprovedOn = DateTime.MinValue;
            AccountSecondApproverId = -1;
            AccountSecondApprovedOn = DateTime.MinValue;
            ProcurementRequestId = -1;
        }

        public int InvoiceId { get; set; }
        public OfficeRef Office { get; set; }
        public CompanyType Company { get; set; }
        public DepartmentRef Dept { get; set; }
        public NTVendorDef NTVendor { get; set; }        
        public string CustomerNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string NSLInvoiceNo { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PaymentFromDate { get; set; }
        public DateTime PaymentToDate { get; set; }
        public DateTime InvoiceReceivedDate { get; set; }
        public CurrencyRef Currency { get; set; }
        public NTPaymentMethodRef PaymentMethod { get; set; }
        public Decimal Amount { get; set; }
        public decimal TotalVAT { get; set; }
        public DateTime SettlementDate { get; set; }
        public Decimal SettlementAmount { get; set; }
        public string SettlementRefNo { get; set; }
        public int SettlementBankAccountId { get; set; }
        public int UpdateSettlementUserId { get; set; }
        public string ChequeNo { get; set; }
        public int IsPayByHK { get; set; }
        public string RejectReason { get; set; }
        public string DCIndicator { get; set; }
        public int IsSUNInterfaced { get; set; }
        public int IsSUNInterfacedForSettlement { get; set; }
        public int FiscalYear { get; set; }
        public int FiscalPeriod { get; set; }
        public DateTime SUNInterfaceDate { get; set; }
        public string JournalNo { get; set; }
        public string ReleaseReason { get; set; }
        public string UserRemark { get; set; }
        public UserRef SubmittedBy { get; set; }
        public DateTime SubmittedOn { get; set; }
        public UserRef Approver { get; set; }
        public int AccountFirstApproverId { get; set; }
        public DateTime AccountFirstApprovedOn { get; set; }
        public int AccountSecondApproverId { get; set; }
        public DateTime AccountSecondApprovedOn { get; set; }
        public int ProcurementRequestId { get; set; }
        public Decimal BankCharge { get; set; }
        public NTWFS WorkflowStatus { get; set; }
        public int Status { get; set; }
        public ArrayList InvoiceDetail { get; set; }
        public UserRef CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public UserRef ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DateTime LogoInterfaceDate { get; set; }

        public class NTInvoiceComparer : IComparer
        {
            public enum CompareType
            {
                Office = 1,
                InvoiceNo = 2,
                Vendor = 3,
                Requester = 4,
                FirstApprover = 5
            }

            private CompareType compareType;
            private SortDirection direction;

            public NTInvoiceComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(object x, object y)
            {
                NTInvoiceDef defX = (NTInvoiceDef)x;
                NTInvoiceDef defY = (NTInvoiceDef)y;

                if (compareType.GetHashCode() == CompareType.Office.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.Office.Description.CompareTo(defY.Office.Description);
                    else
                        return defY.Office.Description.CompareTo(defX.Office.Description);
                }
                else if (compareType.GetHashCode() == CompareType.InvoiceNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.InvoiceNo.CompareTo(defY.InvoiceNo);
                    else
                        return defY.InvoiceNo.CompareTo(defX.InvoiceNo);
                }
                else if (compareType.GetHashCode() == CompareType.Vendor.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.NTVendor.VendorName.CompareTo(defY.NTVendor.VendorName);
                    else
                        return defY.NTVendor.VendorName.CompareTo(defX.NTVendor.VendorName);
                }
                else if (compareType.GetHashCode() == CompareType.Requester.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return (defX.SubmittedBy != null ? defX.SubmittedBy.DisplayName : string.Empty).CompareTo((defY.SubmittedBy != null ? defY.SubmittedBy.DisplayName : string.Empty));

                    else
                        return (defY.SubmittedBy != null ? defY.SubmittedBy.DisplayName : string.Empty).CompareTo((defX.SubmittedBy != null ? defX.SubmittedBy.DisplayName : string.Empty));
                }
                else if (compareType.GetHashCode() == CompareType.FirstApprover.GetHashCode())
                {
                    string firstApproverX = string.Empty;
                    string firstApproverY = string.Empty;

                    if (defX.AccountFirstApproverId != -1)
                        firstApproverX = GeneralWorker.Instance.getUserByKey(defX.AccountFirstApproverId).DisplayName;
                    if (defY.AccountFirstApproverId != -1)
                        firstApproverY = GeneralWorker.Instance.getUserByKey(defY.AccountFirstApproverId).DisplayName;

                    if (direction == SortDirection.Ascending)
                        return firstApproverX.CompareTo(firstApproverY);

                    else
                        return firstApproverY.CompareTo(firstApproverX);
                }

                return 0;
            }
        }

    }

}
