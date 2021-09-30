using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class AdvancePaymentActionHistoryDef : DomainData
    {
        private int actionHistoryId;
        private int paymentId;
        private string description;
        private int actionBy;
        private DateTime actionOn;
        private int status;

        public int ActionHistoryId
        {
            get { return actionHistoryId; }
            set { actionHistoryId = value; }
        }

        public int PaymentId
        {
            get { return paymentId; }
            set { paymentId = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public int ActionBy
        {
            get { return actionBy; }
            set { actionBy = value; }
        }

        public DateTime ActionOn
        {
            get { return actionOn; }
            set { actionOn = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

    }
}
