using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class PurchaseLocationRef : DomainData
	{
		private int purchaseLocationId;
		private string purchaseLocationDescription;

		public PurchaseLocationRef()
		{
		}

		public int PurchaseLocationId
		{
			get { return purchaseLocationId; }
			set { purchaseLocationId = value; }
		}

		public string PurchaseLocationDescription
		{
			get { return purchaseLocationDescription; }
			set { purchaseLocationDescription = value; }
		}
	}
}
