using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.order
{
    [Serializable()]
    public class VendorOrderSummaryRef : DomainData  
    {
        private int futureOrderCount = 0;
        private decimal futureOrderTotalSalesInUSD = 0;
        private DateTime lastShipmentDate = DateTime.MinValue;
        private DateTime nextShipmentDate = DateTime.MinValue;

        public int FutureOrderCount
        {
            get { return futureOrderCount; }
            set { futureOrderCount = value; }
        }

        public decimal FutureOrderTotalSalesInUSD
        {
            get { return futureOrderTotalSalesInUSD; }
            set { futureOrderTotalSalesInUSD = value; }
        }

        public DateTime LastShipmentDate
        {
            get { return lastShipmentDate; }
            set { lastShipmentDate = value; }
        }

        public DateTime NextShipmentDate
        {
            get { return nextShipmentDate; }
            set { nextShipmentDate = value; }
        }

    }
}
