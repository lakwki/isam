using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSMonthEndLogDef : DomainData
    {
        private int orderRefId;
        private string fileNo;
        private string contractNo;
        private string deliveryNo;
        private string status;
        private DateTime nukExtractDate;
        private DateTime createdOn;

        public ILSMonthEndLogDef()
        {
        }

        public int OrderRefId
        {
            get { return orderRefId; }
            set { orderRefId = value; }
        }

        public string FileNo
        {
            get { return fileNo; }
            set { fileNo = value; }
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

        public string Status
        {
            get { return status; }
            set { status = value; }
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
    }
}
