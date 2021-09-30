using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class UKItemPartRef : DomainData
    {
        private string itemNo;
        private int partNo;
        private string nameCode;
        private string description;
        private string supplierCode;
        private int qty;
        private string comment;

        public UKItemPartRef()
        {
        }

        public int PartNo
        {
            get { return partNo; }
            set { partNo = value; }
        }

        public string ItemNo
        {
            get { return itemNo; }
            set { itemNo = value; }
        }

        public string NameCode
        {
            get { return nameCode; }
            set { nameCode = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string SupplierCode
        {
            get { return supplierCode; }
            set { supplierCode = value; }
        }

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }

    }
}
