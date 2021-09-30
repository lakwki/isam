using System;
using com.next.common.domain;
using com.next.isam.domain.types;

namespace com.next.isam.domain.order
{
    [Serializable()]
    public class ShipmentDeductionDef : DomainData  
    {
        private int shipmentDeductionId;
        private int shipmentId;
        private PaymentDeductionType deductionType;
        private string docNo = string.Empty;
        private decimal amt;
        private string remark = string.Empty;
        private int status;

        public int ShipmentDeductionId
        {
            get { return shipmentDeductionId; }
            set { shipmentDeductionId = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public PaymentDeductionType DeductionType
        {
            get { return deductionType; }
            set { deductionType = value; }
        }

        public string DocumentNo
        {
            get { return docNo; }
            set { docNo = value; }
        }

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        public decimal Amount
        {
            get { return amt; }
            set { amt = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

    }
}
