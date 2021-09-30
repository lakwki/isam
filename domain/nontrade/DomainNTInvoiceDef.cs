using System;
using System.Collections;
using com.next.common.domain;


namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class DomainNTInvoiceDef : DomainData
    {
        private NTInvoiceDef ntInvoice;
        private ArrayList ntInvoiceDetailList;
        private ArrayList ntRechargeDetailList;
        private ArrayList actionHistoryList;

        public DomainNTInvoiceDef()
        {

        }


        public NTInvoiceDef NTInvoice
        {
            get { return ntInvoice; }
            set { ntInvoice = value; }
        }

        public ArrayList NTInvoiceDetailList
        {
            get { return ntInvoiceDetailList; }
            set { ntInvoiceDetailList = value; }
        }

        public ArrayList NTRechargeDetailList
        {
            get { return ntRechargeDetailList; }
            set { ntRechargeDetailList = value; }
        }

        public ArrayList ActionHistoryList
        {
            get { return actionHistoryList; }
            set { actionHistoryList = value; }
        }



    }
}
