using System;
using System.Collections;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class ReportOfficeGroupRef : DomainData
	{
		private int officeGroupId;
		private string groupName;
        private string shortName;

		public ReportOfficeGroupRef()
		{
		}

		public int OfficeGroupId
		{
			get { return officeGroupId; }
            set { officeGroupId = value; }
		}

		public string GroupName
		{
			get { return groupName; }
			set { groupName = value; }
		}

        public string ShortName
        {
            get { return shortName; }
            set { shortName = value; }
        }


	}
}
