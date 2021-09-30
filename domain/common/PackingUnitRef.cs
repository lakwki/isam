using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class PackingUnitRef : DomainData
	{
		private Unit _Unit;
		
		public enum Unit
		{
			PCS = 1,
			SET = 2,
			PAR = 3,
			PK = 4,
		}

		public static string getUnitDesc(int id) 
		{
			if (id == Unit.PCS.GetHashCode()) return "PIECES";
			else if (id == Unit.SET.GetHashCode()) return "SETS";
			else if (id == Unit.PAR.GetHashCode()) return "PAIRS";
			else if (id == Unit.PK.GetHashCode()) return "PACKS";
            else return String.Empty;
		}

		public static string getUnitCode(int id) 
		{
			if (id == Unit.PCS.GetHashCode()) return "PCS";
			else if (id == Unit.SET.GetHashCode()) return "SET";
			else if (id == Unit.PAR.GetHashCode()) return "PAR";
			else if (id == Unit.PK.GetHashCode()) return "PK";
            else return String.Empty;
		}

		private int packingUnitId;
		private string packingUnitDescription;
		private string opsKey;

		public PackingUnitRef()
		{
		}

        public PackingUnitRef(int id, string desc, string opsKey)
        {
            packingUnitId = id;
            packingUnitDescription = desc;
            this.opsKey = opsKey;
        }

		public int PackingUnitId
		{
			get { return packingUnitId; }
			set { packingUnitId = value; }
		}

		public string PackingUnitDescription
		{
			get { return packingUnitDescription; }
			set { packingUnitDescription = value; }
		}

		public string OPSKey
		{
			get { return opsKey; }
			set { opsKey = value; }
		}

	}
}
