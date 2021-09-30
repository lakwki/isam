using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSPackingListDetailDef : DomainData
    {
        private int orderRefId;
        private string optionNo;
        private string optionDesc;
        private int qty;
        private decimal weight;
        private decimal volume;
        
        public ILSPackingListDetailDef()
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

        public int Qty 
        { 
            get { return qty; } 
            set { qty = value; } 
        }

        public decimal Weight 
        { 
            get { return weight; } 
            set { weight = value; } 
        }

        public decimal Volume 
        { 
            get { return volume; } 
            set { volume = value; } 
        }
        
    }
}
