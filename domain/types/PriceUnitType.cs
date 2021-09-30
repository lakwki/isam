using System;
using com.next.common.domain;

namespace com.next.isam.domain.types
{
	[Serializable()]
	public class PriceUnitType : DomainData
	{
		private int _type;
		
		private enum eType 
		{
			Length = 1,
			Weight = 2,
		}

		public static PriceUnitType LENGTH = new PriceUnitType(eType.Length.GetHashCode());
		public static PriceUnitType WEIGHT = new PriceUnitType(eType.Weight.GetHashCode());

		private PriceUnitType(int type)
		{
			//
			// TODO: Add constructor logic here
			//
			this._type = type;
		}

		public int PriceUnitTypeId 
		{
			get 
			{
				return this._type;
			}
		}

		public string PriceUnitTypeDesc
		{
			get 
			{ 
				
				if (_type == eType.Length.GetHashCode())
					return "Length";
				else if (_type == eType.Weight.GetHashCode())
					return "Weight";
				else
					return "ERROR";
						
			}
		}

	}
}
