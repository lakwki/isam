using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSCommissionInvoiceDetailDef : DomainData
    {
        private int orderRefId;
        private int seqId;
        private string lineDesc;
        private decimal amt;
        private string vatCode;
        
        public ILSCommissionInvoiceDetailDef()
		{

		}

		public int OrderRefId 
        { 
            get { return orderRefId; } 
            set { orderRefId = value; } 
        }

        public int SeqId
        {
            get { return seqId; }
            set { seqId = value; }
        }

        public string LineDescription 
        { 
            get { return lineDesc; }
            set { lineDesc = value; } 
        }

        public decimal Amount 
        { 
            get { return amt; }
            set { amt = value; } 
        }

        public string VATCode 
        { 
            get { return vatCode; } 
            set { vatCode = value; } 
        }
    }
}
