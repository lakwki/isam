using System;
using System.Text;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class StudioDCNoteShipmentDetailDef
    {
        private int dcNoteShipmentId;
        private int dcNoteShipmentDetailId;
        private int shipmentId;
        private int shipmentDetailId;
        private int sizeOptionId;
        private decimal sellingPrice;
        private int shippedQty;
        private int status;

        public StudioDCNoteShipmentDetailDef()
        {

        }

        public int DCNoteShipmentId
        {
            get { return dcNoteShipmentId; }
            set { dcNoteShipmentId = value; }
        }

        public int DCNoteShipmentDetailId
        {
            get { return dcNoteShipmentDetailId; }
            set { dcNoteShipmentDetailId = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public int ShipmentDetailId
        {
            get { return shipmentDetailId; }
            set { shipmentDetailId = value; }
        }

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

        public int ShippedQty
        {
            get { return shippedQty; }
            set { shippedQty = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
