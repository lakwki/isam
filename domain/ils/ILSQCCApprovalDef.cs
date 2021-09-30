using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
	[Serializable()]
	public class ILSQCCApprovalDef : DomainData
	{
        private int orderRefId;
		private string orderRef;
		private DateTime completedTime;
		private DateTime startTime;
        private int shipmentId;
        private int contractId;
        private int inspectionId;
        private string status;

		public ILSQCCApprovalDef()
		{
		}

        public int OrderRefId 
        { 
            get { return orderRefId; } 
            set { orderRefId = value; } 
        }

		public string OrderRef 
        { 
            get { return orderRef; } 
            set { orderRef = value; } 
        }

		public DateTime CompletedTime 
        { 
            get { return completedTime; } 
            set { completedTime = value; } 
        }

		public DateTime StartTime 
        { 
            get { return startTime; } 
            set { startTime = value; } 
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public int ContractId 
        { 
            get { return contractId; } 
            set { contractId = value; } 
        }

        public int InspectionId 
        { 
            get { return inspectionId; } 
            set { inspectionId = value; } 
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

	}
}
