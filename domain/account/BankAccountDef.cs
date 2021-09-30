using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class BankAccountDef : DomainData 
    {
        private int bankAccountId;
        private string bankAccountNo;
        private string bankName;
        private string bankCode;
        private CurrencyRef currency;
        private string hexAccountNo;
        private string paymentReferenceCode;
        private string sunCOA;
        private string sunT0;
        private string paymentType;

        public int BankAccountId
        {
            get { return bankAccountId; }
            set { bankAccountId = value; }
        }

        public string BankAccountNo
        {
            get { return bankAccountNo; }
            set { bankAccountNo = value; }
        }

        public string BankName
        {
            get { return bankName; }
            set { bankName = value; }
        }

        public string BankCode
        {
            get { return bankCode; }
            set { bankCode = value; }
        }

        public CurrencyRef Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        public string HexAccountNo
        {
            get { return hexAccountNo; }
            set { hexAccountNo = value; }
        }

        public string PaymentReferenceCode
        {
            get { return paymentReferenceCode; }
            set { paymentReferenceCode = value; }
        }

        public string SunCOA
        {
            get { return sunCOA; }
            set { sunCOA = value; }
        }

        public string SunT0
        {
            get { return sunT0; }
            set { sunT0 = value; }
        }

        public string PaymentType
        {
            get { return paymentType; }
            set { paymentType = value; }
        }
    }
}
