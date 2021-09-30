using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class QuotaCategoryRef : DomainData
	{
		private int quotaCategoryId;
		private string quotaCategoryNo;
		private string quotaCategoryDesc;
		private string quotaCategoryFullDesc;
		private string opsKey;

		public QuotaCategoryRef()
		{
		}

        public QuotaCategoryRef(int id, string desc, string opsKey)
        {
            quotaCategoryId = id;
            quotaCategoryDesc = desc;
            this.opsKey = opsKey;
        }

		public int QuotaCategoryId 
        { 
            get { return quotaCategoryId; } 
            set { quotaCategoryId = value; } 
        }

		public string QuotaCategoryNo 
        { 
            get { return quotaCategoryNo; } 
            set { quotaCategoryNo = value; } 
        }

		public string QuotaCategoryDesc 
        { 
            get { return quotaCategoryDesc; } 
            set { quotaCategoryDesc = value; } 
        }

		public string QuotaCategoryFullDesc 
        { 
            get { return quotaCategoryFullDesc; } 
            set { quotaCategoryFullDesc = value; } 
        }

		public string OPSKey 
        { 
            get { return opsKey; } 
            set { opsKey = value; } 
        }
	}
}
