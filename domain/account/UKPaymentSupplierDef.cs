using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class UKPaymentSupplierDef : DomainData 
    {
        private string supplierCode;
        private string bankAccountNo;
        private string name;
        private int currencyId;
        private int officeId;
        private string sunAccountCode;
        private string epicorAccountCode;
        private string t0Code;
        private bool isNSOffice;

        public string SupplierCode
        {
            get { return supplierCode; }
            set { supplierCode = value; }
        }

        public string BankAccountNo
        {
            get { return bankAccountNo; }
            set { bankAccountNo = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public string SunAccountCode
        {
            get { return sunAccountCode; }
            set { sunAccountCode = value; }
        }

        public string EpicorAccountCode
        {
            get { return epicorAccountCode; }
            set { epicorAccountCode = value; }
        }

        public string T0Code
        {
            get { return t0Code; }
            set { t0Code = value; }
        }

        public bool IsNSOffice
        {
            get { return isNSOffice; }
            set { isNSOffice = value; }
        }

    }
}
