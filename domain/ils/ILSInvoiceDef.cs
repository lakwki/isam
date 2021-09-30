using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSInvoiceDef : DomainData
    {
        private int orderRefId;
        private string contractNo;
        private string deliveryNo;
        private string itemNo;
        private string supplierCode;
        private string invoiceNo;
        private DateTime invoiceDate;
        private string currency;
        private decimal totalVAT;
        private decimal totalAmt;
        private int totalQty;
        private string fileNo;
        private DateTime importDate;
        private DateTime processedDate;
        private int status;
        private bool isCancelled;

        public ILSInvoiceDef()
		{

		}

        public int OrderRefId 
        { 
            get { return orderRefId; } 
            set { orderRefId = value; } 
        }

		public string ContractNo 
        { 
            get { return contractNo; } 
            set { contractNo = value; } 
        }

        public string DeliveryNo 
        { 
            get { return deliveryNo; } 
            set { deliveryNo = value; } 
        }

		public string ItemNo 
        { 
            get { return itemNo; } 
            set { itemNo = value; } 
        }

        public string SupplierCode 
        { 
            get { return supplierCode; } 
            set { supplierCode = value; } 
        }

        public string InvoiceNo 
        { 
            get { return invoiceNo; } 
            set { invoiceNo = value; } 
        }

        public DateTime InvoiceDate 
        { 
            get { return invoiceDate; } 
            set { invoiceDate = value; } 
        }

        public string Currency 
        { 
            get { return currency; } 
            set { currency = value; } 
        }

        public decimal TotalAmount 
        { 
            get { return totalAmt; } 
            set { totalAmt = value; } 
        }

        public int TotalQty
        {
            get { return totalQty; }
            set { totalQty = value; }
        }

        public decimal TotalVAT 
        { 
            get { return totalVAT; } 
            set { totalVAT = value; } 
        }

        public string FileNo 
        { 
            get { return fileNo; } 
            set { fileNo = value; } 
        }

        public DateTime ImportDate 
        { 
            get { return importDate; } 
            set { importDate = value; } 
        }

        public DateTime ProcessedDate
        {
            get { return processedDate; }
            set { processedDate = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public bool IsCancelled
        {
            get { return isCancelled; }
            set { isCancelled = value; }
        }

    }
}
