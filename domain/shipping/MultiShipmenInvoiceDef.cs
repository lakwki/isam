using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using com.next.common.domain;
using com.next.isam.domain.order;
using com.next.isam.domain.product;


namespace com.next.isam.domain.shipping
{

    [Serializable()]
    public class MultiShipmentInvoiceDef : DomainData 
    {
        //public string InvoicePrefix;
        //public int InvoiceSeq;
        //public int InvoiceYear;
        public int SequenceNo;
        //public DateTime InvoiceDate;
        //public bool Invoiced;
        public string UKProductGroupCode;
        public int NoOfSize;
        public int LineNo;
        public int NoOfLine;
        public ArrayList ShipmentIdList { get; set; }
        public ContractDef Contract { get; set; }
        public ShipmentDef Shipment { get; set; }
        public InvoiceDef Invoice { get; set; }
        public ProductDef Product { get; set; }
        public ArrayList ShipmentDetails { get; set; }
    }

    [Serializable()]
    public class MultiShipmentInvoiceDetailDef : DomainData 
    {
        public string InvoicePrefix;
        public int InvoiceSeq;
        public int InvoiceYear;
        public int SequenceNo;
        public DateTime InvoiceDate;
        public bool Invoiced;
        public string UKProductGroupCode;
        public int NoOfSize;
        public int LineNo;
        public int NoOfLine;
        public ArrayList ShipmentIdList { get; set; }
        public ContractDef Contract { get; set; }
        public ShipmentDef Shipment { get; set; }
        public InvoiceDef Invoice { get; set; }
        public ProductDef Product { get; set; }
        public ShipmentDetailDef ShipmentDetail { get; set; }
        public decimal TotalSellingAmount { get; set; }
        public decimal TotalSupplierAmount { get; set; }
    }


}
