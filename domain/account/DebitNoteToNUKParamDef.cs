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
    public class DebitNoteToNUKParamDef : DomainData
    {
        private int officeId;
        private int currencyId;
        private string supplierCode;
        private string beneficiaryAccountNo;
        private string beneficiaryName;
        private string bankName;
        private string bankAddress;
        private string swiftCode;

        public DebitNoteToNUKParamDef()
        {

        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public string SupplierCode
        {
            get { return supplierCode; }
            set { supplierCode = value; }
        }

        public string BeneficiaryAccountNo
        {
            get { return beneficiaryAccountNo; }
            set { beneficiaryAccountNo = value; }
        }

        public string BeneficiaryName
        {
            get { return beneficiaryName; }
            set { beneficiaryName = value; }
        }

        public string BankName
        {
            get { return bankName; }
            set { bankName = value; }
        }

        public string BankAddress
        {
            get { return bankAddress; }
            set { bankAddress = value; }
        }

        public string SwiftCode
        {
            get { return swiftCode; }
            set { swiftCode = value; }
        }

    }
}
