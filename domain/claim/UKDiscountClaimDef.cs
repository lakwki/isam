using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using System.Web.UI.WebControls;
using com.next.common.domain.industry.vendor;
using com.next.common.domain.types;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKDiscountClaimDef : DomainData
    {
        private int claimId;
        private string itemNo = string.Empty;
        private string contractNo = string.Empty;
        private int officeId = -1;
        private int handlingOfficeId;
        private VendorRef vendor;
        private int vendorId = -1;
        private int productTeamId = -1;
        private int termOfPurchaseId = -1;
        private int qty;
        private CurrencyRef currency;
        private int currencyId = -1;
        private decimal amount;
        private bool hasUKDN = true;
        private string uKDebitNoteNo = string.Empty;
        private DateTime uKDebitNoteDate;
        private DateTime uKDebitNoteReceivedDate;
        private string remark = string.Empty;
        private int isInterfaced = 0;
        private int workflowStatusId = 0;
        private int paymentOfficeId = -1;
        private UKDiscountClaimWFS workflowStatus = UKDiscountClaimWFS.OUTSTANDING;
        private bool isUKDiscount;
        private DateTime issueDate;

        public int ClaimId
        {
            get { return claimId; }
            set { claimId = value; }
        }

        public string ItemNo
        {
            get { return itemNo; }
            set { itemNo = value; }
        }

        public string ContractNo
        {
            get { return contractNo; }
            set { contractNo = value; }
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public int HandlingOfficeId
        {
            get { return handlingOfficeId; }
            set { handlingOfficeId = value; }
        }

        public VendorRef Vendor
        {
            get { return vendor; }
            set { vendor = value; }
        }

        public int VendorId
        {
            get { return vendorId; }
            set { vendorId = value; }
        }

        public int ProductTeamId
        {
            get { return productTeamId; }
            set { productTeamId = value; }
        }

        public int TermOfPurchaseId
        {
            get { return termOfPurchaseId; }
            set { termOfPurchaseId = value; }
        }

        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }

        public CurrencyRef Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public bool HasUKDN
        {
            get { return hasUKDN; }
            set { hasUKDN = value; }
        }

        public string UKDebitNoteNo
        {
            get { return uKDebitNoteNo; }
            set { uKDebitNoteNo = value; }
        }

        public DateTime UKDebitNoteDate
        {
            get { return uKDebitNoteDate; }
            set { uKDebitNoteDate = value; }
        }

        public DateTime UKDebitNoteReceivedDate
        {
            get { return uKDebitNoteReceivedDate; }
            set { uKDebitNoteReceivedDate = value; }
        }

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        public int IsInterfaced
        {
            get { return isInterfaced; }
            set { isInterfaced = value; }
        }

        public int WorkflowStatusId
        {
            get { return workflowStatusId; }
            set { workflowStatusId = value; }
        }

        public int PaymentOfficeId
        {
            get { return paymentOfficeId; }
            set { paymentOfficeId = value; }
        }

        public UKDiscountClaimWFS WorkflowStatus
        {
            get { return workflowStatus; }
            set { workflowStatus = value; }
        }

        public bool IsUKDiscount
        {
            get { return isUKDiscount; }
            set { isUKDiscount = value; }
        }

        public DateTime IssueDate
        {
            get { return issueDate; }
            set { issueDate = value; }
        }

        public class UKDiscountClaimComparer : IComparer<UKDiscountClaimDef>
        {
            public enum CompareType
            {
                Office = 1,
                ClaimType = 2,
                UKDebitNoteNo = 3,
                SupplierName = 4,
                CurrencyId = 5
            }

            private CompareType compareType;
            private SortDirection direction;

            public UKDiscountClaimComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(UKDiscountClaimDef x, UKDiscountClaimDef y)
            {
                UKDiscountClaimDef defX = x;
                UKDiscountClaimDef defY = y;

                if (compareType.GetHashCode() == CompareType.Office.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return com.next.common.domain.types.OfficeId.getStatus(defX.OfficeId).Name.CompareTo(com.next.common.domain.types.OfficeId.getStatus(defY.OfficeId).Name);
                    else
                        return com.next.common.domain.types.OfficeId.getStatus(defY.OfficeId).Name.CompareTo(com.next.common.domain.types.OfficeId.getStatus(defX.OfficeId).Name);
                }
                else if (compareType.GetHashCode() == CompareType.SupplierName.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return VendorWorker.Instance.getVendorByKey(defX.vendorId).Name.CompareTo(VendorWorker.Instance.getVendorByKey(defY.vendorId).Name);
                    else
                        return VendorWorker.Instance.getVendorByKey(defY.vendorId).Name.CompareTo(VendorWorker.Instance.getVendorByKey(defX.vendorId).Name);
                }
                else if (compareType.GetHashCode() == CompareType.UKDebitNoteNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.UKDebitNoteNo.CompareTo(defY.UKDebitNoteNo);
                    else
                        return defY.UKDebitNoteNo.CompareTo(defX.UKDebitNoteNo);
                }
                else if (compareType.GetHashCode() == CompareType.CurrencyId.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.CurrencyId.CompareTo(defY.CurrencyId);
                    else
                        return defY.CurrencyId.CompareTo(defX.CurrencyId);
                }
                return 0;
            }
        }

    }
}
