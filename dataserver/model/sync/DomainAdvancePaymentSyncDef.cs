using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.isam.dataserver.model.nss;

namespace com.next.isam.dataserver.model.sync
{
    public class DomainAdvancePaymentSyncDef
    {
        private NssAdvancePaymentDs payment;
        private NssAdvancePaymentOrderDetailDs paymentOrderDetail;

        public NssAdvancePaymentDs Payment
        {
            get { return payment; }
            set { payment = value; }
        }

        public NssAdvancePaymentOrderDetailDs PaymentOrderDetail
        {
            get { return paymentOrderDetail; }
            set { paymentOrderDetail = value; }
        }
    }
}
