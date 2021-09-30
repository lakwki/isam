using System;
using System.Collections;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using System.Web.UI.WebControls;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class PaymentStatusEnquiryDef : DomainData
    {
        private int shipmentId;
        private int splitShipmentId;
        private int vendorId;
        private string supplierName;
        private string contractNo;
        private string itemNo;
        private string invoiceNo;
        private string supplierInvoiceNo;
        private string currencyCode;
        private decimal netAmt;
        private decimal netAmtInUSD;
        private bool editLock;
        private int isInterfaced;
        private DateTime shippingDocReceiptDate;
        private DateTime apDate;
        private long dmsAttachmentId;
        private string paymentStatus;
        private int dmsWorkflowStatusId;
        private bool isPaymentHold;
        private string reviewUser;
        private DateTime reviewDateTime;
        private DateTime invoiceDate;
        private bool isUploadDMSDocument;
        private decimal labTestIncome;
        private int totalShippedQty;
        private bool isChinaGBTestRequired;
        private int seasonId;
        private int productId;
        private string productTeamCode;
        private string productTeamDescription;
        private decimal deductionAmt;
        private DateTime lgDate;

        public PaymentStatusEnquiryDef()
        {

        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public int SplitShipmentId
        {
            get { return splitShipmentId; }
            set { splitShipmentId = value; }
        }

        public int VendorId
        {
            get { return vendorId; }
            set { vendorId = value; }
        }

        public string SupplierName
        {
            get { return supplierName; }
            set { supplierName = value; }
        }

        public DateTime APDate
        {
            get { return apDate; }
            set { apDate = value; }
        }

        public DateTime ShippingDocReceiptDate
        {
            get { return shippingDocReceiptDate; }
            set { shippingDocReceiptDate = value; }
        }

        public string ContractNo
        {
            get { return contractNo; }
            set { contractNo = value; }
        }

        public string ItemNo
        {
            get { return itemNo; }
            set { itemNo = value; }
        }

        public string InvoiceNo
        {
            get { return invoiceNo; }
            set { invoiceNo = value; }
        }

        public string SupplierInvoiceNo
        {
            get { return supplierInvoiceNo; }
            set { supplierInvoiceNo = value; }
        }

        public string CurrencyCode
        {
            get { return currencyCode; }
            set { currencyCode = value; }
        }

        public decimal NetAmount
        {
            get { return netAmt; }
            set { netAmt = value; }
        }

        public decimal NetAmountInUSD
        {
            get { return netAmtInUSD; }
            set { netAmtInUSD = value; }
        }

        public bool EditLock
        {
            get { return editLock; }
            set { editLock = value; }
        }

        public int IsInterfaced
        {
            get { return isInterfaced; }
            set { isInterfaced = value; }
        }

        public long DMSAttachmentId
        {
            get { return dmsAttachmentId; }
            set { dmsAttachmentId = value; }
        }

        public string PaymentStatus
        {
            get { return paymentStatus; }
            set { paymentStatus = value; }
        }

        public int DMSWorkflowStatusId
        {
            get { return dmsWorkflowStatusId; }
            set { dmsWorkflowStatusId = value; }
        }

        public bool IsPaymentHold
        {
            get { return isPaymentHold; }
            set { isPaymentHold = value; }
        }

        public string ReviewUser
        {
            get { return reviewUser; }
            set { reviewUser = value; }
        }

        public DateTime ReviewDateTime
        {
            get { return reviewDateTime; }
            set { reviewDateTime = value; }
        }

        public DateTime InvoiceDate
        {
            get { return invoiceDate; }
            set { invoiceDate = value; }
        }

        public Boolean IsUploadDMSDocument
        {
            get { return isUploadDMSDocument; }
            set { isUploadDMSDocument = value; }
        }

        public decimal LabTestIncome
        {
            get { return labTestIncome; }
            set { labTestIncome = value; }
        }

        public int TotalShippedQty
        {
            get { return totalShippedQty; }
            set { totalShippedQty = value; }
        }

        public bool IsChinaGBTestRequired
        {
            get { return isChinaGBTestRequired; }
            set { isChinaGBTestRequired = value; }
        }

        public int SeasonId
        {
            get { return seasonId; }
            set { seasonId = value; }
        }

        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }

        public string ProductTeamCode
        {
            get { return productTeamCode; }
            set { productTeamCode = value; }
        }

        public string ProductTeamDescription
        {
            get { return productTeamDescription; }
            set { productTeamDescription = value; }
        }

        public decimal DeductionAmount
        {
            get { return deductionAmt; }
            set { deductionAmt = value; }
        }

        public DateTime LGDate
        {
            get { return lgDate; }
            set { lgDate = value; }
        }

        public class PaymentStatusEnquiryComparer : IComparer
        {
            public enum CompareType
            {
                ContractNo = 1,
                InvoiceNo = 2,
                SupplierInvoiceNo = 3,
                SupplierName = 4,
                NetAmtInUSD = 5,
                InvoiceDate = 6,
                ItemNo = 7
            }

            private CompareType compareType;
            private SortDirection direction;

            public PaymentStatusEnquiryComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(object x, object y)
            {
                PaymentStatusEnquiryDef defX = (PaymentStatusEnquiryDef)x;
                PaymentStatusEnquiryDef defY = (PaymentStatusEnquiryDef)y;

                if (compareType.GetHashCode() == CompareType.ContractNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.ContractNo.CompareTo(defY.ContractNo);
                    else
                        return defY.ContractNo.CompareTo(defX.ContractNo);
                }
                else if (compareType.GetHashCode() == CompareType.ItemNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.ItemNo.CompareTo(defY.ItemNo);
                    else
                        return defY.ItemNo.CompareTo(defX.ItemNo);
                }
                else if (compareType.GetHashCode() == CompareType.SupplierName.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.SupplierName.CompareTo(defY.SupplierName);
                    else
                        return defY.SupplierName.CompareTo(defX.SupplierName);
                }
                else if (compareType.GetHashCode() == CompareType.InvoiceNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.InvoiceNo.CompareTo(defY.InvoiceNo);
                    else
                        return defY.InvoiceNo.CompareTo(defX.InvoiceNo);
                }
                else if (compareType.GetHashCode() == CompareType.SupplierInvoiceNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.SupplierInvoiceNo.CompareTo(defY.SupplierInvoiceNo);
                    else
                        return defY.SupplierInvoiceNo.CompareTo(defX.SupplierInvoiceNo);
                }
                else if (compareType.GetHashCode() == CompareType.NetAmtInUSD.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.NetAmountInUSD.CompareTo(defY.NetAmountInUSD);
                    else
                        return defY.NetAmountInUSD.CompareTo(defX.NetAmountInUSD);
                }
                else if (compareType.GetHashCode() == CompareType.InvoiceDate.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.InvoiceDate.CompareTo(defY.InvoiceDate);
                    else
                        return defY.InvoiceDate.CompareTo(defX.InvoiceDate);
                }

                return 0;
            }
        }


    }
}
