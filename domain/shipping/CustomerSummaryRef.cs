using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class CustomerSummaryRef : DomainData
    {

        private int deliveryNo;
        private string supplierCode = "N/A";
        private string orderCurrency = "N/A";
        private string invoiceCurrency = "N/A";
        private string refurb = "N/A";
        private string quotaCat = "N/A";
        private string packingMethod = "N/A";
        private string departure = "N/A";
        private string destination = "N/A";
        private string transportMode = "N/A";
        private int freightPercentForNUK = 0;
        private int freightPercentForNSL = 0;
        private DateTime atWareHouseDate = DateTime.MinValue;
        private DateTime actualAtWareHouseDate = DateTime.MinValue;



        public int DeliveryNo { get; set; }

        public string SupplierCode
        {
            get { return supplierCode; }
            set { supplierCode = value; }
        }

        public string OrderCurrency
        {
            get { return orderCurrency; }
            set { orderCurrency = value; }
        }

        public string InvoiceCurrency
        {
            get { return invoiceCurrency; }
            set { invoiceCurrency = value; }
        }

        public string Refurb
        {
            get { return refurb; }
            set { refurb = value; }
        }

        public string QuotaCategory
        {
            get { return quotaCat; }
            set { quotaCat = value; }
        }

        public string PackingMethod
        {
            get { return packingMethod; }
            set { packingMethod = value; }
        }

        public string Departure
        {
            get { return departure; }
            set { departure = value; }
        }

        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public string TransportMode
        {
            get { return transportMode; }
            set { transportMode = value; }
        }

        public int FreightPercentForNUK
        {
            get { return freightPercentForNUK; }
            set { freightPercentForNUK = value; }
        }

        public int FreightPercentForNSL
        {
            get { return freightPercentForNSL; }
            set { freightPercentForNSL = value; }
        }

        public DateTime AtWarehouseDate
        {
            get { return atWareHouseDate; }
            set { atWareHouseDate = value; }
        }

        public DateTime ActualAtWarehouseDate
        {
            get { return actualAtWareHouseDate; }
            set { actualAtWareHouseDate = value; }
        }

        public int TotalOrderQuantity { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public int TotalShippedQuantity { get; set; }
        public decimal TotalShippedAmount { get; set; }
        public decimal TotalShippedSupplierGarmentAmount { get; set; }

        public string PurchaseOrder { get; set; }
        public string InboundDeliveryNumber { get; set; }
        public DateTime PromotionStartDate { get; set; }
        public DateTime ScheduledDeliveryDate { get; set; }
        public string ShipmentMethodDesc { get; set; }
        public string Forwarder { get; set; }

    }
}
