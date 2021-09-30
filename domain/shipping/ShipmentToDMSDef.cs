using System;
using System.Collections;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using System.Web.UI.WebControls;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class ShipmentToDMSDef : DomainData
    {
        private int shipmentId;
        private int vendorId;
        private string supplierName;
        private string contractNo;
        private string invoiceNo;
        private string supplierInvoiceNo;
        private string currencyCode;
        private decimal qaCommissionPercent;
        private decimal vendorPaymentDiscountPercent;
        private decimal qaCommissionAmt;
        private decimal vendorPaymentDiscountAmt;
        private decimal netAmt;
        private DateTime shippingDocReceiptDate;
        private DateTime shippingDocCheckedDate;
        private UserRef shippingDocCheckedBy;
        private DateTime invoiceDate;
        private string customerCode;
        private int totalShippedQty;
        private int splitCount;
        private string itemNo;
        private bool isUploadDMSDocument;
        private decimal labTestIncome;


        public ShipmentToDMSDef()
        {

        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
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

        public decimal QACommissionPercent
        {
            get { return qaCommissionPercent; }
            set { qaCommissionPercent = value; }
        }

        public decimal QACommissionAmount
        {
            get { return qaCommissionAmt; }
            set { qaCommissionAmt = value; }
        }

        public decimal VendorPaymentDiscountPercent
        {
            get { return vendorPaymentDiscountPercent; }
            set { vendorPaymentDiscountPercent = value; }
        }

        public decimal VendorPaymentDiscountAmount
        {
            get { return vendorPaymentDiscountAmt; }
            set { vendorPaymentDiscountAmt = value; }
        }

        public UserRef ShippingDocCheckedBy
        {
            get { return shippingDocCheckedBy; }
            set { shippingDocCheckedBy = value; }
        }

        public DateTime ShippingDocCheckedDate
        {
            get { return shippingDocCheckedDate; }
            set { shippingDocCheckedDate = value; }
        }

        public string CustomerCode
        {
            get { return customerCode; }
            set { customerCode = value; }
        }

        public int SplitCount
        {
            get { return splitCount; }
            set { splitCount = value; }
        }

        public int TotalShippedQty
        {
            get { return totalShippedQty; }
            set { totalShippedQty = value; }
        }

        public DateTime InvoiceDate
        {
            get { return invoiceDate; }
            set { invoiceDate = value; }
        }

        public string ItemNo
        {
            get { return itemNo; }
            set { itemNo = value; }
        }

        public bool IsUploadDMSDocument
        {
            get { return isUploadDMSDocument; }
            set { isUploadDMSDocument = value; }
        }

        public decimal LabTestIncome
        {
            get { return labTestIncome; }
            set { labTestIncome = value; }
        }


        public class ShipmentToDMSComparer : IComparer
        {
            public enum CompareType
            {
                ContractNo = 1,
                InvoiceNo = 2,
                SupplierInvoiceNo = 3,
                SupplierName = 4,
                NetAmt = 5,
                InvoiceDate = 6
            }

            private CompareType compareType;
            private SortDirection direction;

            public ShipmentToDMSComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(object x, object y)
            {
                ShipmentToDMSDef defX = (ShipmentToDMSDef)x;
                ShipmentToDMSDef defY = (ShipmentToDMSDef)y;

                if (compareType.GetHashCode() == CompareType.ContractNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.ContractNo.CompareTo(defY.ContractNo);
                    else
                        return defY.ContractNo.CompareTo(defX.ContractNo);
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
                else if (compareType.GetHashCode() == CompareType.NetAmt.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.NetAmount.CompareTo(defY.NetAmount);
                    else
                        return defY.NetAmount.CompareTo(defX.NetAmount);
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
