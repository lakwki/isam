using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.common.domain.industry.vendor;
using com.next.isam.domain.common;

namespace com.next.isam.domain.order
{
    [Serializable()]
    public class ILSManifestDetailShipmentDef : DomainData
    {
        private string voyageNo;
        private string vesselName;
        private string containerNo;
        private string contractNo;
        private int deliveryNo;
        private VendorRef vendor;
        private PackingMethodRef packingMethod;
        private DateTime inWarehouseDate;
        private decimal totalVolume;
        private int totalCartons;
        private int totalShippedQty;
        private string bookingSONo;
        private string invoicePrefix;
        private int invoiceSeq;
        private int invoiceYear;
        private string itemNo;
        private string invoiceNo;
        private UserRef invoiceUploadUser;
        private int shipmentId;
        private string containerPosition;
        private int customerId;
        private int customerDestinationId;
        private string destinationCode;

        public string VoyageNo
        {
            get { return voyageNo; }
            set { voyageNo = value; }
        }

        public string VesselName
        {
            get { return vesselName; }
            set { vesselName = value; }
        }

        public string ContainerNo
        {
            get { return containerNo; }
            set { containerNo = value; }
        }

        public string ContractNo
        {
            get
            {
                return contractNo;
            }
            set
            {
                contractNo = value;
            }
        }

        public int DeliveryNo
        {
            get { return deliveryNo; }
            set { deliveryNo = value; }
        }

        public string ItemNo
        {
            get { return itemNo; }
            set { itemNo = value; }
        }

        public VendorRef Vendor
        {
            get { return vendor; }
            set { vendor = value; }
        }

        public PackingMethodRef PackingMethod
        {
            get { return packingMethod; }
            set { packingMethod = value; }
        }

        public DateTime InWarehouseDate
        {
            get { return inWarehouseDate; }
            set { inWarehouseDate = value; }
        }

        public decimal TotalVolume
        {
            get { return totalVolume; }
            set { totalVolume = value; }
        }

        public int TotalCartons
        {
            get { return totalCartons; }
            set { totalCartons = value; }
        }

        public int TotalShippedQty
        {
            get { return totalShippedQty; }
            set { totalShippedQty = value; }
        }

        public string BookingSONo
        {
            get { return bookingSONo; }
            set { bookingSONo = value; }
        }

        public string InvoicePrefix
        {
            get { return invoicePrefix; }
            set { invoicePrefix = value; }
        }

        public int InvoiceSeq
        {
            get { return invoiceSeq; }
            set { invoiceSeq = value; }
        }

        public int InvoiceYear
        {
            get { return invoiceYear; }
            set { invoiceYear = value; }
        }

        public string InvoiceNo
        {
            get
            {
                return invoiceNo;
            }
            set
            {
                invoiceNo = value; 
            }
         }

        public UserRef InvoiceUploadUser
        {
            get { return invoiceUploadUser; }
            set { invoiceUploadUser = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public string ContainerPosition
        {
            get { return containerPosition; }
            set { containerPosition = value; }
        }

        public int CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        public int CustomerDestinationId
        {
            get { return customerDestinationId; }
            set { customerDestinationId = value; }
        }

        public string DestinationCode
        {
            get { return destinationCode; }
            set { destinationCode = value; }
        }
    }
        
}

