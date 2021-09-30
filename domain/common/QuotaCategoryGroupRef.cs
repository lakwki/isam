using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class QuotaCategoryGroupRef : DomainData
	{

		private enum QuotaCatGrpId
		{
			nil = 115,
		}

		public static QuotaCategoryGroupRef GetQuotaCategory_NIL()
		{
			QuotaCategoryGroupRef rf = new QuotaCategoryGroupRef();
            rf.QuotaCategoryGroupId = QuotaCatGrpId.nil.GetHashCode();
            rf.QuotaCategoryGroupDescription = "NIL";
            rf.OPSKey = "NIL";
            rf.OPAUpchargeHKD = 0;
            rf.OPAUpchargeUSD = 0;
            return rf;
		}

		private enum WithoutOPACheckingId
		{
			QuotaCategory_31 = 46,
		}

		private int quotaCategoryGroupId;
		private string quotaCategoryGroupDescription;
		private string opsKey;
		private decimal opaUpchargeHKD;
		private decimal opaUpchargeUSD;

		public decimal OPAUpchargeHKD 
        { 
            get { return opaUpchargeHKD; }	
            set	{ opaUpchargeHKD = value; } 
        }

		public decimal OPAUpchargeUSD 
        { 
            get { return opaUpchargeUSD; }	
            set	{ opaUpchargeUSD = value; } 
        }

		public static bool isRequiredOPAChecking(int quotaCategoryGroupId)
		{
			if (quotaCategoryGroupId == WithoutOPACheckingId.QuotaCategory_31.GetHashCode())
				return false;
			else
				return true;
		}

		public QuotaCategoryGroupRef()
		{
		}

        public QuotaCategoryGroupRef(int id, string desc, string opsKey)
        {
            quotaCategoryGroupId = id;
            quotaCategoryGroupDescription = desc;
            this.opsKey = opsKey;
        }

		public int QuotaCategoryGroupId
		{
			get { return quotaCategoryGroupId; }
			set { quotaCategoryGroupId = value; }
		}

		public string QuotaCategoryGroupDescription
		{
			get { return quotaCategoryGroupDescription; }
			set { quotaCategoryGroupDescription = value; }
		}

		public string OPSKey
		{
			get { return opsKey; }
			set { opsKey = value; }
		}
	}
}
