using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSOrderCopyDetailDef : DomainData
    {
        private int orderRefId;
        private string optionNo;
        private string optionDesc;
        private decimal price;
        private int qty;

        public ILSOrderCopyDetailDef()
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

        public string optionDescription 
        { 
            get { return optionDesc; } 
            set { optionDesc = value; } 
        }

        public decimal Price 
        { 
            get { return price; } 
            set { price = value; } 
        }

        public int Qty 
        { 
            get { return qty; } 
            set { qty = value; } 
        }
    }
}
