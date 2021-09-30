using System;
using System.Text;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class QACommissionDNShipmentDef
    {
        private int dnId;
        private int shipmentId;
        private int vendorId;
        private int shippedQty;
        private decimal supplierAmt;
        private decimal qaCommissionPercent;
        private decimal qaCommissionAmt;
        private int currencyId;
        private int status;
        private int createdBy;
        private DateTime createdOn;

        public QACommissionDNShipmentDef()
        {

        }

        public int DNId
        {
            get { return dnId; }
            set { dnId = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public int VendorId
        {
            get { return vendorId; }
            set { vendorId = value; }
        }

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public int ShippedQty
        {
            get { return shippedQty; }
            set { shippedQty = value; }
        }

        public decimal SupplierAmount
        {
            get { return supplierAmt; }
            set { supplierAmt = value; }
        }

        public decimal QACommissionPercent
        {
            get { return qaCommissionPercent; }
            set { qaCommissionPercent = value; }
        }

        public decimal QACommissionAmount
        {
            get { return qaCommissionAmt; }
            set { qaCommissionAmt = value; }
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
    }
}
