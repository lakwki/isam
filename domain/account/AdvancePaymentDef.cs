using System;
using System.Collections.Generic;
using System.Linq;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.domain.industry.vendor;
using com.next.common.domain.types;
using com.next.common.datafactory.worker.industry;
using com.next.common.datafactory.worker;
using com.next.common.web.commander;
using System.Web.UI.WebControls;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class AdvancePaymentDef : DomainData
    {
        private int paymentId;
        private int paymentTypeId;  // 1 = Fabric, 2= By Instalment
        private string paymentNo;
        private int officeId;
        private VendorRef vendor;
        private CurrencyRef currency;
        private decimal totalAmt;
        private DateTime paymentDate;
        private UserRef uploadedBy = null;
        private DateTime uploadedOn = DateTime.MinValue;
        private UserRef submittedBy;
        private DateTime submittedOn;
        private UserRef approvedBy = null;
        private DateTime approvedOn = DateTime.MinValue;
        private UserRef rejectedBy = null;
        private DateTime rejectedOn = DateTime.MinValue;
        private int rejectReasonId = -1;
        private string remark = string.Empty;
        private int workflowStatusId = 2;
        private UserRef createdBy;
        private UserRef initiatedBy;
        private int status;
        private DateTime settlementDate;
        private decimal interestChargedAmt;
        private int isInterfaced;
        private string payableTo;
        private decimal interestRate;
        private int isC19;
        private string flRefNo = string.Empty;


        public int PaymentId
        {
            get { return paymentId; }
            set { paymentId = value; }
        }

        public string PaymentNo
        {
            get { return paymentNo; }
            set { paymentNo = value; }
        }

        public int PaymentTypeId
        {
            get { return paymentTypeId; }
            set { paymentTypeId = value; }
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public VendorRef Vendor
        {
            get { return vendor; }
            set { vendor = value; }
        }

        public CurrencyRef Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        public decimal TotalAmount
        {
            get { return totalAmt; }
            set { totalAmt = value; }
        }

        public DateTime PaymentDate
        {
            get { return paymentDate; }
            set { paymentDate = value; }
        }

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        public UserRef UploadedBy
        {
            get { return uploadedBy; }
            set { uploadedBy = value; }
        }

        public DateTime UploadedOn
        {
            get { return uploadedOn; }
            set { uploadedOn = value; }
        }

        public UserRef ApprovedBy
        {
            get { return approvedBy; }
            set { approvedBy = value; }
        }

        public DateTime ApprovedOn
        {
            get { return approvedOn; }
            set { approvedOn = value; }
        }

        public UserRef RejectedBy
        {
            get { return rejectedBy; }
            set { rejectedBy = value; }
        }

        public DateTime RejectedOn
        {
            get { return rejectedOn; }
            set { rejectedOn = value; }
        }

        public int RejectReasonId
        {
            get { return rejectReasonId; }
            set { rejectReasonId = value; }
        }

        public int WorkflowStatusId
        {
            get { return workflowStatusId; }
            set { workflowStatusId = value; }
        }

        public UserRef SubmittedBy
        {
            get { return submittedBy; }
            set { submittedBy = value; }
        }

        public DateTime SubmittedOn
        {
            get { return submittedOn; }
            set { submittedOn = value; }
        }

        public UserRef CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        public UserRef InitiatedBy
        {
            get { return initiatedBy; }
            set { initiatedBy = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        public DateTime SettlementDate
        {
            get { return settlementDate; }
            set { settlementDate = value; }
        }

        public Decimal InterestChargedAmt
        {
            get { return interestChargedAmt; }
            set { interestChargedAmt = value; }
        }

        public Decimal InterestRate
        {
            get { return interestRate; }
            set { interestRate = value; }
        }

        public int IsInterfaced
        {
            get { return isInterfaced; }
            set { isInterfaced = value; }
        }

        public string PayableTo
        {
            get { return payableTo; }
            set { payableTo = value; }
        }

        public int IsC19
        {
            get { return isC19; }
            set { isC19 = value; }
        }

        public string FLRefNo
        {
            get { return flRefNo; }
            set { flRefNo = value; }
        }

        public class AdvancePaymentComparer : IComparer<AdvancePaymentDef>
        {
            public enum CompareType
            {
                PaymentNo = 1,
                PaymentDate = 2,
                Vendor = 3,
                Currency = 4,
                PaymentAmt = 5
            }

            private CompareType compareType;
            private SortDirection direction;

            public AdvancePaymentComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(AdvancePaymentDef x, AdvancePaymentDef y)
            {
                AdvancePaymentDef defX = x;
                AdvancePaymentDef defY = y;

                if (compareType.GetHashCode() == CompareType.PaymentNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.PaymentNo.CompareTo(defY.PaymentNo);
                    else
                        return defY.PaymentNo.CompareTo(defX.PaymentNo);
                }
                else if (compareType.GetHashCode() == CompareType.Currency.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.Currency.CurrencyCode.CompareTo(defY.Currency.CurrencyCode);
                    else
                        return defY.Currency.CurrencyCode.CompareTo(defX.Currency.CurrencyCode);
                }
                else if (compareType.GetHashCode() == CompareType.Vendor.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.Vendor.Name.CompareTo(defY.Vendor.Name);
                    else
                        return defY.Vendor.Name.CompareTo(defX.Vendor.Name);
                }
                else if (compareType.GetHashCode() == CompareType.PaymentAmt.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.TotalAmount.CompareTo(defY.TotalAmount);
                    else
                        return defY.TotalAmount.CompareTo(defX.TotalAmount);
                }
                else if (compareType.GetHashCode() == CompareType.PaymentDate.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.PaymentDate.CompareTo(defY.PaymentDate);
                    else
                        return defY.PaymentDate.CompareTo(defX.PaymentDate);
                }

                return 0;
            }
        }

    }

    public class AdvancePaymentSummaryDef : DomainData
    {
        private decimal balance;
        private decimal variance;
        private decimal noRecoveryPlanBalance;

        public decimal Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public decimal Variance
        {
            get { return variance; }
            set { variance = value; }
        }

        public decimal NoRecoveryPlanBalance
        {
            get { return noRecoveryPlanBalance; }
            set { noRecoveryPlanBalance = value; }
        }

    }


}
