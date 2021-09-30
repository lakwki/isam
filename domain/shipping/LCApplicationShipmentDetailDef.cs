using System;
using com.next.common.domain;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class LCApplicationShipmentDetailDef : DomainData
    {
        public int LCApplicationId { get; set; }
        public int ShipmentId { get; set; }
        public int SplitShipmentId { get; set; }
        public int ShipmentDetailId { get; set; }
        public int SizeOptionId { get; set; }
        public string Colour { get; set; }
        public int POQty { get; set; }
        public decimal ReducedSupplierGmtPrice { get; set; }
        public int Status { get; set; }

    }
}
