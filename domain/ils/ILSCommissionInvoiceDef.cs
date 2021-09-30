using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSCommissionInvoiceDef : DomainData
    {
        private int orderRefId;
        private string itemNo;
        private string supplierCode;
        private string invoiceNo;
        private DateTime invoiceDate;
        private string currency;
        private decimal totalVAT;
        private decimal totalAmt;
        private string fileNo;
        private DateTime importDate;
        private int status;

        public ILSCommissionInvoiceDef()
		{

		}

        public int OrderRefId 
        { 
            get { return orderRefId; } 
            set { orderRefId = value; } 
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

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

    }
}
