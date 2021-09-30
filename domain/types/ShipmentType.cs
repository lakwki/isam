using System;
using com.next.common.domain;

namespace com.next.isam.domain.types
{
	[Serializable()]
	public class ShipmentType : DomainData
	{
		public static ShipmentType NORMAL = new ShipmentType(Code.Normal);
		public static ShipmentType SPLIT = new ShipmentType(Code.Split);
		
		private Code _code;
		
		private enum Code 
		{
			Normal = 1,
			Split = 2
		}

		private ShipmentType(Code code)
		{
			this._code = code;
		}

		public int Id
		{
			get 
			{
				return Convert.ToUInt16(_code.GetHashCode());
			}
		}

		public string Name 
		{
			get 
			{ 
				switch (_code)
				{
					case Code.Normal :
						return "Normal";
					case Code.Split :
						return "Split";
					default:
						return "ERROR";
				}				
			}
		}

		public static ShipmentType getType(int id) 
		{
			if (id == Code.Normal.GetHashCode()) return ShipmentType.NORMAL;
			else if (id == Code.Split.GetHashCode()) return ShipmentType.SPLIT;
			else return null;
		}


		
		public enum ShipmentCategory
		{
			SEA = 1,
			AIR = 2,
			SPECIALAPPROVAL = 3,
		}

	}
}
