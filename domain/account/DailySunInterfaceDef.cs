using System;
using System.Collections.Generic;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class DailySunInterfaceDef
    {
        private int shipmentId;
        private int splitShipmentId;
        private int sunInterfaceTypeId;
        private bool isActive;
        private DateTime extractedDate;

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

        public int SunInterfaceTypeId
        {
            get { return sunInterfaceTypeId; }
            set { sunInterfaceTypeId = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public DateTime ExtractedDate
        {
            get { return extractedDate; }
            set { extractedDate = value; }
        }

    }
}
