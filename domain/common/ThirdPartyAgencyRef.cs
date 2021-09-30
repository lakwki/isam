using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class ThirdPartyAgencyRef : DomainData
	{
		private int thirdPartyAgencyId;
		private string description;
		private string name;
		private decimal agencyCommissionPercentage;
        private int officeId;

		public ThirdPartyAgencyRef()
		{

		}

		public int ThirdPartyAgencyId 
        { 
            get { return thirdPartyAgencyId; } 
            set { thirdPartyAgencyId = value; } 
        }
	
		public string Description 
        { 
            get { return description; } 
            set { description = value; } 
        }

		public string Name 
        { 
            get { return name; } 
            set { name = value; } 
        }
	
		public decimal AgencyCommissionPercentage 
        { 
            get { return agencyCommissionPercentage; } 
            set { agencyCommissionPercentage = value; } 
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

	}
}
