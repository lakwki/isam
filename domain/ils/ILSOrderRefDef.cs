using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSOrderRefDef : DomainData
    {
        private int orderRefId;
        private string contractNo;
        private string deliveryNo;
        private int shipmentId;
        private bool isForced;
        private bool isReset;
        private bool isCancelled;
        private UserRef updateUser;

        public ILSOrderRefDef()
        {

        }

        public int OrderRefId 
        { 
            get { return orderRefId; } 
            set { orderRefId = value; } 
        }

        public string OrderRef 
        { 
            get { return contractNo + "-" + deliveryNo; } 
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

        public int ShipmentId 
        { 
            get { return shipmentId; } 
            set { shipmentId = value; } 
        }

        public bool IsForced 
        { 
            get { return isForced; } 
            set { isForced = value; } 
        }

        public bool IsReset
        {
            get { return isReset; }
            set { isReset = value; }
        }

        public bool IsCancelled 
        { 
            get { return isCancelled; } 
            set { isCancelled = value; } 
        }

        public UserRef UpdateUser 
        { 
            get { return updateUser; } 
            set { updateUser = value; } 
        }
    }
}
