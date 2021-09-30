using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class CartonTypeRef : DomainData
	{
		private int cartonTypeId;
		private string cartonTypeDesc;
		private int packingMethodId;

		public CartonTypeRef()
		{
		}

		public int CartonTypeId 
        {
            get {return cartonTypeId;} 
            set{cartonTypeId = value;}
        }

		public string CartonTypeDesc 
        {
            get {return cartonTypeDesc;} 
            set {cartonTypeDesc = value;}
        }

		public int PackingMethodId 
        { 
            get { return packingMethodId; } 
            set { packingMethodId = value; } 
        }
	}
}
