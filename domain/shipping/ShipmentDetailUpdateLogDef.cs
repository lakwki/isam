using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class ShipmentDetailUpdateLogDef : DomainData 
    {
        private int sizeOptionId;
        private decimal sellingPrice;
        private decimal netFOBPrice;
        private decimal supplierGmtPrice;
        private int shippedQty;
        private bool isRevised;

        public int SizeOptionId
        {
            get { return sizeOptionId; }
            set { sizeOptionId = value; }
        }

        public decimal SellingPrice
        {
            get { return sellingPrice; }
            set { sellingPrice = value; }
        }

        public decimal NetFOBPrice
        {
            get { return netFOBPrice; }
            set { netFOBPrice = value; }
        }

        public decimal SupplierGarmentPrice
        {
            get { return supplierGmtPrice; }
            set { supplierGmtPrice = value; }
        }

        public int ShippedQuantity
        {
            get { return shippedQty; }
            set { shippedQty = value; }
        }

        public bool IsRevised
        {
            get { return isRevised; }
            set { isRevised = value; }
        }

    }
}
