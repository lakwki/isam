using System;
using System.Collections.Generic;
using com.next.isam.domain.common;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSCancelledOrderDef : DomainData
    {
        private int orderRefId;
        private string contractNo;
        private string deliveryNo;
        private string fileNo;
        private DateTime importDate;
        private int status;
        private string remark;

        public ILSCancelledOrderDef()
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

        public string FileNo 
        { 
            get { return fileNo; } 
            set { fileNo = value; } 
        }

        public DateTime ImportDate 
        { 
            get { return importDate; } 
            set { importDate = value; } 
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

    }
}
