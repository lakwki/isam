using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSManifestDetailDef : DomainData
    {
        private string containerNo;
        private int orderRefId;
        private string contractNo;
        private string deliveryNo;
        private DateTime ctsDate;
        private DateTime sobDate;
        private DateTime paDate;
        private string containerPosition;
        private bool isCancelled;

        public ILSManifestDetailDef()
		{

		}

		public string ContainerNo 
        { 
            get { return containerNo; } 
            set { containerNo = value; } 
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

        public DateTime ConfirmedToShipDate 
        { 
            get { return ctsDate; } 
            set { ctsDate = value; } 
        }

        public DateTime ShippedOnBoardDate 
        { 
            get { return sobDate; } 
            set { sobDate = value; } 
        }

        public DateTime PreAdviceDate 
        { 
            get { return paDate; } 
            set { paDate = value; } 
        }

        public string ContainerPosition 
        { 
            get { return containerPosition; } 
            set { containerPosition = value; } 
        }

        public bool IsCancelled 
        { 
            get { return isCancelled; } 
            set { isCancelled = value; } 
        }
    }
}
