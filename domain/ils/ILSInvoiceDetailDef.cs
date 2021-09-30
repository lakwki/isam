using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSInvoiceDetailDef : DomainData
    {
        private int orderRefId;
        private string optionNo;
        private int qty;
        private decimal price;
        private string vatCode;
        
        public ILSInvoiceDetailDef()
		{

		}

		public int OrderRefId 
        { 
            get { return orderRefId; } 
            set { orderRefId = value; } 
        }

        public string OptionNo 
        { 
            get { return optionNo; } 
            set { optionNo = value; } 
        }

        public int Qty 
        { 
            get { return qty; } 
            set { qty = value; } 
        }

        public decimal Price 
        { 
            get { return price; } 
            set { price = value; } 
        }

        public string VATCode 
        { 
            get { return vatCode; } 
            set { vatCode = value; } 
        }
    }
}
