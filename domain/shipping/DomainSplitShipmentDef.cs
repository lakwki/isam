using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.order;
using com.next.isam.domain.product;


namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class DomainSplitShipmentDef : DomainData
    {
        private ContractDef contract;
        private ShipmentDef shipment;
        private SplitShipmentDef splitShipment;
        private ArrayList splitShipmentDetail;
        private ArrayList updatedSplitShipmentDetail;
        private InvoiceDef invoice;
        private LCApplicationDef lcApplication;
        private LCBatchRef lcBatch;

        public ContractDef Contract
        {
            get { return contract; }
            set { contract = value; }
        }


        public ShipmentDef Shipment
        {
            get { return shipment; }
            set { shipment = value; }
        }

        public SplitShipmentDef SplitShipment
        {
            get { return splitShipment; }
            set { splitShipment = value; }
        }

        public ArrayList SplitShipmentDetail
        {
            get { return splitShipmentDetail; }
            set { splitShipmentDetail = value; }
        }

        public ArrayList UpdatedSplitShipmentDetail
        {
            get
            {
                return updatedSplitShipmentDetail;
            }
            set
            {
                updatedSplitShipmentDetail = value;
            }
        }

        public InvoiceDef Invoice
        {
            get
            {
                return invoice;
            }
            set
            {
                invoice = value;
            }
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

    }
}
