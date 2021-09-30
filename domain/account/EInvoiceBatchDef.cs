using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class EInvoiceBatchDef : DomainData
    {
        private int eInvoiceBatchId;
        private string eInvoiceBatchNo;
        private UserRef submittedBy;
        private DateTime submittedOn;
        //private EInvoiceBatchWFS workflowStatus;
        private int status;

        public EInvoiceBatchDef()
        {
        }

        public EInvoiceBatchDef( string eInvoiceBatchNo, UserRef submittedBy, DateTime submittedOn)
        {
            this.eInvoiceBatchId = -1;
            this.eInvoiceBatchNo = eInvoiceBatchNo;
            this.submittedBy = submittedBy;
            this.submittedOn = submittedOn;
            this.status = GeneralCriteria.TRUE;
        }

        public int EInvoiceBatchId
        {
            get { return eInvoiceBatchId; }
            set { eInvoiceBatchId = value; }
        }

        public string EInvoiceBatchNo
        {
            get { return eInvoiceBatchNo; }
            set { eInvoiceBatchNo = value; }
        }

        public UserRef SubmittedBy
        {
            get { return submittedBy; }
            set { submittedBy = value; }
        }

        public DateTime SubmittedOn
        {
            get { return submittedOn; }
            set { submittedOn = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
    }

}
