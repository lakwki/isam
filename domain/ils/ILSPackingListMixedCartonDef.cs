using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSPackingListMixedCartonDetailDef : DomainData
    {
        private int orderRefId;
        private int cartonId;
        private string cartonSize;
        private int cartonLength;
        private int cartonWidth;
        private int cartonHeight;

        public ILSPackingListMixedCartonDetailDef()
		{

		}

		public int OrderRefId 
        { 
            get { return orderRefId; } 
            set { orderRefId = value; } 
        }

        public int CartonId 
        { 
            get { return cartonId; }
            set { cartonId = value; } 
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

    }
}
