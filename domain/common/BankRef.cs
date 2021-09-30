using System;
using System.Collections;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class BankRef : DomainData
	{
		private int bankId;
		private string bankName;
        private int status;
        ArrayList branches;

		public BankRef()
		{
		}

		public int BankId
		{
			get { return bankId; }
			set { bankId = value; }
		}

		public string BankName
		{
			get { return bankName; }
			set { bankName = value; }
		}

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public ArrayList Branches
        {
            get { return branches; }
            set { branches = value; }
        }

	}
}
