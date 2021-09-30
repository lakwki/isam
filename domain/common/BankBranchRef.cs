using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class BankBranchRef : DomainData
	{
		private int bankId;
        private int bankBranchId;
		private string branchName;
        private string address1;
        private string address2;
        private string address3;
        private string address4;
        private CountryRef country;
        private string contactPerson;
        private string phone;
        private int status;

		public BankBranchRef()
		{
		}

        public int BankId
        {
            get { return bankId; }
            set { bankId = value; }
        }

		public int BankBranchId
		{
            get { return bankBranchId; }
            set { bankBranchId = value; }
		}

		public string BranchName
		{
			get { return branchName; }
            set { branchName = value; }
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
            set { address3 = value; }
        }

        public string Address4
        {
            get { return address4; }
            set { address4 = value; }
        }

        public CountryRef Country
        {
            get { return country; }
            set { country = value; }
        }

        public string ContactPerson
        {
            get { return contactPerson; }
            set { contactPerson = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

    }
}
