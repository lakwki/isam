using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
	[Serializable()]
	public class ILSMessageResultDef : DomainData
	{
		private string fileNo;
		private int orderRefId;
        private string contractNo;
        private string deliveryNo;
		private string type;
		private DateTime sentDate;
		private DateTime processedDate;
		private string status;
		private int errorNo;

		public ILSMessageResultDef()
		{
		}

		public string FileNo 
        { 
            get { return fileNo; } 
            set { fileNo = value; } 
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

		public string Type 
        { 
            get { return type; } 
            set { type = value; } 
        }

		public DateTime SentDate 
        { 
            get { return sentDate; } 
            set { sentDate = value; } 
        }

		public DateTime ProcessedDate 
        { 
            get { return processedDate; } 
            set { processedDate = value; } 
        }

		public string Status 
        { 
            get { return status; } 
            set { status = value; } 
        }

		public int ErrorNo 
        { 
            get { return errorNo; } 
            set { errorNo = value; } 
        }
	}
}
