using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class DocumentDef : DomainData  
    {
        private int docId;
        private int shipmentId;
        private DocumentType docType;
        private string docNo;
        private CountryOfOriginRef country;
        private DateTime issueDate;
        private DateTime expiryDate;
        private QuotaCategoryRef quotaCat;
        private decimal weight;
        private int qty;
        private PackingUnitRef unit;
        private int orderQty;
        private PackingUnitRef orderUnit;
        private int poQty;
        private PackingUnitRef poUnit;
        private DateTime despatchToUKDate;
        private string despatchAWBNo;
        private int status;

        public QuotaCategoryRef QuotaCategory
        {
            get { return quotaCat; }
            set { quotaCat = value; }
        }

        public decimal Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }

        public PackingUnitRef Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        public int OrderQty
        {
            get { return orderQty; }
            set { orderQty = value; }
        }

        public PackingUnitRef OrderUnit
        {
            get { return orderUnit; }
            set { orderUnit = value; }
        }

        public int POQty
        {
            get { return poQty; }
            set { poQty = value; }
        }

        public PackingUnitRef POUnit
        {
            get { return poUnit; }
            set { poUnit = value; }
        }

        public DateTime DespatchToUKDate
        {
            get { return despatchToUKDate; }
            set { despatchToUKDate = value; }
        }

        public string DespatchAWBNo
        {
            get { return despatchAWBNo; }
            set { despatchAWBNo = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        
        public int DocId
        {
            get { return docId; }
            set { docId = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }        
        
        public DocumentType DocumentType
        {
            get { return docType; }
            set { docType = value; }
        }

        public string DocumentNo
        {
            get { return docNo; }
            set { docNo = value; }
        }

        public CountryOfOriginRef Country
        {
            get { return country; }
            set { country = value; }
        }

        public DateTime IssueDate
        {
            get { return issueDate; }
            set { issueDate = value; }
        }
        public DateTime ExpiryDate
        {
            get { return expiryDate; }
            set { expiryDate = value; }
        }
    }
}
