using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.isam.dataserver.model.nss;

namespace com.next.isam.dataserver.model.sync
{
    public class DomainLetterOfGuaranteeSyncDef
    {
        private NssLGPaymentDs payment;
        private NssLGPaymentOrderDetailDs paymentOrderDetail;

        public NssLGPaymentDs Payment
        {
            get { return payment; }
            set { payment = value; }
        }

        public NssLGPaymentOrderDetailDs PaymentOrderDetail
        {
            get { return paymentOrderDetail; }
            set { paymentOrderDetail = value; }
        }
    }
}
