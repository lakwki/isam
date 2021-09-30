using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
	[Serializable()]
	public class ILSFileSentLogDef : DomainData
	{
		private string fileNo;
		private string type;
		private DateTime createdOn;
		private DateTime completedOn;

		public ILSFileSentLogDef()
		{
		}

		public string FileNo
		{
			get { return fileNo; }
			set { fileNo = value; }
		}

		public string Type
		{
			get { return type; }
			set { type = value; }
		}

		public DateTime CreatedOn
		{
			get { return createdOn; }
			set { createdOn = value; }
		}

		public DateTime CompletedOn
		{
			get { return completedOn; }
			set { completedOn = value; }
		}
	}
}
