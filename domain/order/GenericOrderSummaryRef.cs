using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.order
{
    [Serializable()]
    public class GenericOrderSummaryRef : DomainData  
    {
        private int orderCount = 0;
        private int orderTotalQty = 0;
        private decimal orderTotalSalesInUSD = 0;

        public int OrderCount
        {
            get { return orderCount; }
            set { orderCount = value; }
        }

        public int OrderTotalQty
        {
            get { return orderTotalQty; }
            set { orderTotalQty = value; }
        }

        public decimal OrderTotalSalesInUSD
        {
            get { return orderTotalSalesInUSD; }
            set { orderTotalSalesInUSD = value; }
        }


    }
}
