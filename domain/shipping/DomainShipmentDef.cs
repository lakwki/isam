using System;
using System.Collections;
using com.next.common.domain;
using com.next.isam.domain.order;
using com.next.isam.domain.product;
using com.next.isam.domain.account;
using com.next.isam.domain.ils;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class DomainShipmentDef : DomainData
    {
        private ContractDef contract;
        private ProductDef product;
        private ShipmentDef shipment;
        private ArrayList shipmentDetails;
        private ArrayList updatedShipmentDetails;
        private ArrayList splitShipments;
        private ArrayList auditEntries;
        private ArrayList documents;
        private ArrayList updatedDocuments;
        private ArrayList ilsDocuments;
        private ArrayList manifests;
        private InvoiceDef invoice;
        //private ILSSummaryRef ilsSummary;
        private LCApplicationDef lcApplication;
        private LCBatchRef lcBatch;
        private ArrayList actionHistoryList;

        private ArrayList ilsPackingListDetail;
        private ArrayList ilsInvoiceDetail;

        private ArrayList ilsManifest;
        private ArrayList ilsManifestDetail;

        private SunInterfaceLogDef cogsSunInterfaceLog;
        private SunInterfaceLogDef salesSunInterfaceLog;
        private SunInterfaceLogDef salesCommSunInterfaceLog;
        private SunInterfaceLogDef apSunInterfaceLog;
        private SunInterfaceLogDef arSunInterfaceLog;
        private ArrayList reversingEntry;
        private ArrayList otherDelivery;
        private ArrayList paymentDeduction, updatedPaymentDeduction;

        
        public DomainShipmentDef()
        {
        }

        public ContractDef Contract 
        { 
            get { return contract; } 
            set { contract = value; } 
        }

        public ProductDef Product 
        { 
            get { return product; } 
            set { product = value; } 
        }

        public ShipmentDef Shipment 
        { 
            get { return shipment; } 
            set { shipment = value; } 
        }

        public ArrayList ShipmentDetails 
        { 
            get { return shipmentDetails; } 
            set { shipmentDetails = value; } 
        }

        public ArrayList UpdatedShipmentDetails
        {
            get { return updatedShipmentDetails; }
            set { updatedShipmentDetails = value; }
        }

        public ArrayList SplitShipments 
        { 
            get { return splitShipments; } 
            set { splitShipments = value; } 
        }

        public ArrayList AuditEntries 
        { 
            get { return auditEntries; }
            set { auditEntries = value; } 
        }

        public InvoiceDef Invoice 
        { 
            get { return invoice; } 
            set { invoice = value; } 
        }

        public ArrayList Documents
        {
            get { return documents; }
            set { documents = value; }
        }

        public ArrayList UpdatedDocuments
        {
            get { return updatedDocuments; }
            set { updatedDocuments = value; }
        }

        public ArrayList ILSDocuments
        {
            get { return ilsDocuments; }
            set { ilsDocuments = value; }
        }

        public ArrayList Manifests
        {
            get { return manifests; }
            set { manifests = value; }
        }

        //public ILSSummaryRef ILSSummary
        //{
        //    get { return ilsSummary; }
        //    set { ilsSummary = value; }
        //}

        public CustomerSummaryRef CustomerSummary { get; set; }


        public ArrayList ILSPackingListDetail
        {
            get { return ilsPackingListDetail; }
            set { ilsPackingListDetail = value; }
        }

        public ArrayList ILSInvoiceDetail
        {
            get { return ilsInvoiceDetail; }
            set { ilsInvoiceDetail = value; }
        }

        public ArrayList ILSManifest
        {
            get { return ilsManifest; }
            set { ilsManifest = value; }
        }

        public ArrayList ILSManifestDetail
        {
            get { return ilsManifestDetail; }
            set { ilsManifestDetail = value; }
        }

        public ArrayList ActionHistoryList
        {
            get { return actionHistoryList; }
            set { actionHistoryList = value; }
        }

        public LCApplicationDef LCApplication
        {
            get { return lcApplication; }
            set { lcApplication = value; }
        }

        public LCBatchRef LCBatch
        {
            get { return lcBatch; }
            set { lcBatch = value; }
        }

        public SunInterfaceLogDef COGSSunInterfaceLog
        {
            get { return cogsSunInterfaceLog; }
            set { cogsSunInterfaceLog = value; }
        }

        public SunInterfaceLogDef SalesSunInterfaceLog
        {
            get { return salesSunInterfaceLog; }
            set { salesSunInterfaceLog = value; }
        }

        public SunInterfaceLogDef SalesCommSunInterfaceLog
        {
            get { return salesCommSunInterfaceLog; }
            set { salesCommSunInterfaceLog = value; }
        }

        public SunInterfaceLogDef APSunInterfaceLog
        {
            get { return apSunInterfaceLog; }
            set { apSunInterfaceLog = value; }
        }

        public SunInterfaceLogDef ARSunInterfaceLog
        {
            get { return arSunInterfaceLog; }
            set { arSunInterfaceLog = value; }
        }

        public ArrayList ReversingEntry
        {
            get { return reversingEntry; }
            set { reversingEntry = value; }
        }

        public ArrayList OtherDelivery
        {
            get { return otherDelivery; }
            set { otherDelivery = value; }
        }

        public ArrayList PaymentDeduction
        {
            get { return paymentDeduction; }
            set { paymentDeduction = value; }
        }

        public ArrayList UpdatedPaymentDeduction
        {
            get { return updatedPaymentDeduction; }
            set { updatedPaymentDeduction = value; }
        }


    }
}
