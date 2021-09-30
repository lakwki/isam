using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
	[Serializable()]
	public class ILSUnitPriceMatrixDef : DomainData
	{
        private string nslOptionNo;
		private string nslSizeDesc;
		private decimal nslPrice;
		private int nslQty;
        private string nukOptionNo;
        private string nukSizeDesc;
        private decimal nukPrice;
        private int nukQty;
        private decimal fobPrice;

        public ILSUnitPriceMatrixDef()
		{
		}

        public string NSLOptionNo 
        { 
            get { return nslOptionNo; } 
            set { nslOptionNo = value; } 
        }

        public string NSLSizeDesc
        {
            get { return nslSizeDesc; }
            set { nslSizeDesc = value; }
        }

        public decimal NSLPrice 
        { 
            get { return nslPrice; }
            set { nslPrice = value; } 
        }

		public int NSLQty 
        { 
            get { return nslQty; }
            set { nslQty = value; } 
        }

        public string NUKOptionNo
        {
            get { return nukOptionNo; }
            set { nukOptionNo = value; }
        }

        public string NUKSizeDesc
        {
            get { return nukSizeDesc; }
            set { nukSizeDesc = value; }
        }

        public decimal NUKPrice
        {
            get { return nukPrice; }
            set { nukPrice = value; }
        }

        public int NUKQty
        {
            get { return nukQty; }
            set { nukQty = value; }
        }

        public decimal FOBPrice
        {
            get { return fobPrice; }
            set { fobPrice = value; }
        }

	}
}
