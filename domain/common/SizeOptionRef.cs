using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class SizeOptionRef : DomainData
	{
		private int sizeOptionId;
		private string sizeOptionNo;
		private DateTime effectiveDateFrom;
		private DateTime effectiveDateTo;
		private string sizeDescription;

		public SizeOptionRef()
		{
		}

		public int SizeOptionId
		{
			get { return sizeOptionId; }
			set { sizeOptionId = value; }
		}

		public string SizeOptionNo
		{
			get { return sizeOptionNo; }
			set { sizeOptionNo = value; }
		}

		public string SizeDescription
		{
			get { return sizeDescription; }
			set { sizeDescription = value;	}
		}

		public DateTime EffectiveDateFrom
		{
			get { return effectiveDateFrom; }
			set { effectiveDateFrom = value; }
		}

		public DateTime EffectiveDateTo
		{
			get { return effectiveDateTo; }
			set { effectiveDateTo = value; }
		}

	}
}
