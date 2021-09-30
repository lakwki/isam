using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class PackingMethodRef : DomainData
	{
		private int packingMethodId;
		private string packingMethodDescription;
		private string packingMethodCode;
		private string opsKey;
        private bool refurb;

		public PackingMethodRef()
		{
		}

		public int PackingMethodId
		{
			get { return packingMethodId; }
			set { packingMethodId = value; }
		}

		public string PackingMethodDescription
		{
			get { return packingMethodDescription; }
			set { packingMethodDescription = value; }
		}

		public string PackingMethodCode
		{
			get { return packingMethodCode; }
			set { packingMethodCode = value; }
		}

        public bool Refurb
        {
            get { return refurb; }
            set { refurb = value; }
        }

        public string OPSKey
		{
			get { return opsKey; }
			set { opsKey = value; }
		}

        public string DeliveryUnitDescription
        {
            get
            {
                if (packingMethodId == 1)
                    return "HANGERS";
                else if (packingMethodId == 2)
                    return "CARTONS";
                else if (packingMethodId == 3)
                    return "CARTONS";
                else
                    return "N/A";

            }
        }

		public enum Id
		{
			goh = 1,
			fp = 2,
			fp_pr = 3,
		}
		
		public static string getDescription(int id) 
		{
			if (id == Id.goh.GetHashCode()) return "GOH";
			else if (id == Id.fp.GetHashCode()) return "FLAT PACK";
			else if (id == Id.fp_pr.GetHashCode()) return "PROGRAMMED REFURB";
			else return "N/A";
		}

        public static string getDescription(string OPSKey)
        {
            if (OPSKey == "H") return "GOH";
            else if (OPSKey == "B") return "FLAT PACK";
            else if (OPSKey == "X") return "PROGRAMMED REFURB";
            else return "N/A";
        }


        public static string getDeliveryUnitDescription(int id)
        {
            if (id == Id.goh.GetHashCode()) return "HANGERS";
            else if (id == Id.fp.GetHashCode()) return "CARTONS";
            else if (id == Id.fp_pr.GetHashCode()) return "CARTONS";
            else return "N/A";
        }

        public static string getCode(int id) 
		{
			if (id == Id.goh.GetHashCode()) return "GOH";
			else if (id == Id.fp.GetHashCode()) return "FP";
			else if (id == Id.fp_pr.GetHashCode()) return "FP-PR";
			else return "N/A";
		}

	}
}
