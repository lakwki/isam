using System;
using System.Collections.Generic;
using System.Text;

namespace com.next.isam.domain.account
{
    public class SettlementBankDetailDef
    {
        private int accountId;
        private string accountName;
        private string accountNo;
        private string officeId;
        private int currencyId;
        private string tradingAgencyId;
        private string swiftCode;
        private string bankName;
        private string bankAddress;

        public SettlementBankDetailDef()
        {
        }

        public int AccountId
        {
            get { return accountId; }
            set { accountId = value; }
        }

        public string AccountName
        {
            get { return accountName; }
            set { accountName = value; }
        }

        public string AccountNo
        {
            get { return accountNo; }
            set { accountNo = value; }
        }

        public string OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public string TradingAgencyId
        {
            get { return tradingAgencyId; }
            set { tradingAgencyId = value; }
        }

        public string SwiftCode
        {
            get { return swiftCode; }
            set { swiftCode = value; }
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
    }
}
