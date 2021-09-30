using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class TradingAgencyDef : DomainData
	{
		private int tradingAgencyId;
        private string name;
        private string shortName;
		private CountryRef country;
		private string address1;
		private string address2;
		private string address3;
		private string address4;
		private string telNo;
		private string faxNo;
		private string remark;
        private string bankName;
        private string bankAddress1;
        private string bankAddress2;
        private string bankAddress3;
        private string bankAddress4;
        private string bankAccountName;
        private string accountNo;

		public TradingAgencyDef()
		{
		}

		public int TradingAgencyId
		{
			get { return tradingAgencyId; }
            set { tradingAgencyId = value; }
		}

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string ShortName
		{
			get { return shortName; }
            set { shortName = value; }
		}

		public CountryRef Country
		{
			get { return country; }
            set { country = value; }
		}

		public string Address1
		{
			get { return address1; }
			set { address1 = value; }
		}

		public string Address2 
		{
			get { return address2; }
			set { address2 = value; }
		}

		public string Address3
		{
			get { return address3; }
			set	{ address3 = value;	}
		}
	
		public string Address4
		{
			get { return address4; }
			set { address4 = value; }
		}

		public string TelNo
		{
			get { return telNo; }
			set { telNo = value; }
		}

		public string FaxNo
		{
			get { return faxNo; }
			set { faxNo = value; }
		}

		public string Remark
		{
			get { return remark; }
            set { remark = value; }
		}

        public string BankName
        {
            get { return bankName; }
            set { bankName = value; }
        }

        public string BankAddress1
        {
            get { return bankAddress1; }
            set { bankAddress1 = value; }
        }

        public string BankAddress2
        {
            get { return bankAddress2; }
            set { bankAddress2 = value; }
        }

        public string BankAddress3
        {
            get { return bankAddress3; }
            set { bankAddress3 = value; }
        }

        public string BankAddress4
        {
            get { return bankAddress4; }
            set { bankAddress4 = value; }
        }

        public string BankAccountName
        {
            get { return bankAccountName; }
            set { bankAccountName = value; }
        }

        public string AccountNo
        {
            get { return accountNo; }
            set { accountNo = value; }
        }

    }
}
