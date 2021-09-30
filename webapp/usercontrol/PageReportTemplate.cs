using System;
using System.Collections;
using System.IO;

using com.next.infra.web;
using com.next.infra.util;
using com.next.infra.smartwebcontrol;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.industry.vendor;
using com.next.common.web.commander;
using com.next.isam.domain.types;
using com.next.isam.domain.common;
using com.next.isam.webapp.usercontrol;
using com.next.isam.webapp.commander;

namespace com.next.isam.webapp.usercontrol
{
	public class PageReportTemplate : com.next.isam.webapp.usercontrol.PageTemplate
	{
		public enum DateRangeType
		{
			TestSubmission=1,
			TestValidation=2,
			TestPerforming=3,
			TestCompletion=4,
			TestVerified=5,
			TestApproval=6
		}

		public PageReportTemplate()
		{
		}

		public void setReportName(string nameFilename)
		{
			this.setHeaderText(nameFilename);
			this.setHeaderImage("../images/banner_statisticreports.gif");
		}
	}
}
