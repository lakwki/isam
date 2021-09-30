using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSMonthEndShipmentDef : DomainData
    {
        private int orderRefId;
        private string contractNo;
        private string deliveryNo;
        private string invoiceNo;
        private string lastStatus;
        private DateTime nukExtractDate;
        private DateTime createdOn;
        private DateTime modifiedOn;

        public ILSMonthEndShipmentDef()
        {
        }

        public int OrderRefId
        {
            get { return orderRefId; }
            set { orderRefId = value; }
        }

        public string ContractNo
        {
            get { return contractNo; }
            set { contractNo = value; }
        }

        public string DeliveryNo
        {
            get { return deliveryNo; }
            set { deliveryNo = value; }
        }

        public string InvoiceNo
        {
            get { return invoiceNo; }
            set { invoiceNo = value; }
        }

        public string LastStatus
        {
            get { return lastStatus; }
            set { lastStatus = value; }
        }

        public DateTime NUKExtractDate
        {
            get { return nukExtractDate; }
            set { nukExtractDate = value; }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }

        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { modifiedOn = value; }
        }
    }
}
