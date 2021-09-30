using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSPackingListCartonDetailDef : DomainData
    {
        private int orderRefId;
        private string optionNo;
        private int seqNo;
        private string cartonSize;
        private int cartonLength;
        private int cartonWidth;
        private int cartonHeight;
        private int pieces;
        private int noOfCartons;
        private int firstCarton;
        private int lastCarton;

        public ILSPackingListCartonDetailDef()
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

        public int SeqNo 
        { 
            get { return seqNo; } 
            set { seqNo = value; } 
        }

        public string CartonSize 
        { 
            get { return cartonSize; } 
            set { cartonSize = value; } 
        }

        public int CartonLength 
        { 
            get { return cartonLength; } 
            set { cartonLength = value; } 
        }

        public int CartonWidth 
        { 
            get { return cartonWidth; } 
            set { cartonWidth = value; } 
        }

        public int CartonHeight 
        { 
            get { return cartonHeight; } 
            set { cartonHeight = value; } 
        }

        public int Pieces 
        { 
            get { return pieces; } 
            set { pieces = value; } 
        }

        public int NoOfCartons 
        { 
            get { return noOfCartons; } 
            set { noOfCartons = value; } 
        }

        public int FirstCarton 
        { 
            get { return firstCarton; } 
            set { firstCarton = value; } 
        }

        public int LastCarton 
        { 
            get { return lastCarton; } 
            set { lastCarton = value; } 
        }
    }
}
