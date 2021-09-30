using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class TermOfPurchaseRef : DomainData
	{
		private int termOfPurchaseId;
		private string termOfPurchaseDescription;
		private string orderType;

		public enum Id 
		{
			FOB = 1,
			CMT = 2,
			CM = 3,
			VMTrading = 4,
            FOB_UT = 5
		}

		public static bool isVM(int id) 
		{
			if (id == Id.CMT.GetHashCode()) return true;
			else if (id == Id.CM.GetHashCode()) return true;
			else if (id == Id.VMTrading.GetHashCode()) return true;
			else return false;
		}

		public TermOfPurchaseRef()
		{

        }

		public int TermOfPurchaseId
		{
			get { return termOfPurchaseId; }
			set { termOfPurchaseId = value; }
		}

		public string TermOfPurchaseDescription
		{
			get { return termOfPurchaseDescription; }
			set { termOfPurchaseDescription = value; }
		}

		public string OrderType
		{
			get { return orderType; }
			set { orderType = value; }
		}

		public static string getDescription(int id) 
		{
			if (id == Id.FOB.GetHashCode()) return "FOB";
			else if (id == Id.CMT.GetHashCode()) return "CMT";
			else if (id == Id.CM.GetHashCode()) return "CM";
			else if (id == Id.VMTrading.GetHashCode()) return "VM Trading";
            else if (id == Id.FOB_UT.GetHashCode()) return "FOB (UT)";
			else return "N/A";
		}

	}
}
