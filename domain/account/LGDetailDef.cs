using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    class LGDetailDef : DomainData
    {
        private int lgId;
        private int shipmentId;
        private int splitShipmentId;
        private int status;
        private int createdBy;
        private DateTime createdOn;
        private int modifiedBy;
        private DateTime modifiedOn;

        public int LGId
        {
            get { return lgId; }
            set { lgId = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public int SplitShipmentId
        {
            get { return splitShipmentId; }
            set { splitShipmentId = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public int CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }

        public int ModifiedBy
        {
            get { return modifiedBy; }
            set { modifiedBy = value; }
        }

        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { modifiedOn = value; }
        }
    }
}
