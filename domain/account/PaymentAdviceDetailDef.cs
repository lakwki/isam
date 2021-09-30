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
    public class PaymentAdviceDetailDef : DomainData
    {
        private int paymentAdviceId;
        private string manufacturerInvoiceNo;
        private string poNo;
        private decimal amount;
        private string currency;
        private DateTime transactionDate;
        private string refNo;

        public PaymentAdviceDetailDef()
        {

        }

        public PaymentAdviceDetailDef(int paymentAdviceId, string manufacturerInvoiceNo, string poNo, decimal amount, string currency, DateTime transactionDate)
        {
            this.paymentAdviceId = paymentAdviceId;
            this.manufacturerInvoiceNo = manufacturerInvoiceNo;
            this.poNo = poNo;
            this.amount = amount;
            this.currency = currency;
            this.transactionDate = transactionDate;
        }

        public int PaymentAdviceId
        {
            get { return paymentAdviceId; }
            set { paymentAdviceId = value; }
        }

        public string ManufacturerInvoiceNo
        {
            get { return manufacturerInvoiceNo; }
            set { manufacturerInvoiceNo = value; }
        }

        public string PONo
        {
            get { return poNo; }
            set { poNo = value; }
        }

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public string Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        public DateTime TransactionDate
        {
            get { return transactionDate; }
            set { transactionDate = value; }
        }

        public string RefNo
        {
            get { return refNo; }
            set { refNo = value; }
        }


    }
}
