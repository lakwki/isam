using System;
using System.Collections;
using System.Collections.Generic;
using com.next.common.domain;


namespace com.next.isam.domain.account
{
    [Serializable()]
    public class DomainAdvancePaymentDef : DomainData
    {
        private AdvancePaymentDef advancePayment;
        private List<AdvancePaymentOrderDetailDef> orderDetailList;
        private List<AdvancePaymentInstalmentDetailDef> instalmentDetailList;
        private List<AdvancePaymentBalanceSettlementDef> balanceSettlementList;

        public DomainAdvancePaymentDef()
        {

        }

        public DomainAdvancePaymentDef(Type t)
        {
            advancePayment = new AdvancePaymentDef();
            if (t == typeof(AdvancePaymentOrderDetailDef))
            {
                orderDetailList = new List<AdvancePaymentOrderDetailDef>();
                instalmentDetailList = null;
            }
            else if (t == typeof(AdvancePaymentInstalmentDetailDef))            
            {
                instalmentDetailList = new List<AdvancePaymentInstalmentDetailDef>();
                orderDetailList = null;
                balanceSettlementList = null;
            }
        }

        public AdvancePaymentDef AdvancePayment
        {
            get { return advancePayment; }
            set { advancePayment = value; }
        }

        public List<AdvancePaymentOrderDetailDef> OrderDetailList
        {
            get { return orderDetailList; }
            set { orderDetailList = value; }
        }

        public List<AdvancePaymentInstalmentDetailDef> InstalmentDetailList
        {
            get { return instalmentDetailList; }
            set { instalmentDetailList = value; }
        }

        public List<AdvancePaymentBalanceSettlementDef> BalanceSettlementList
        {
            get { return balanceSettlementList; }
            set { balanceSettlementList = value; }
        }



    }
}
