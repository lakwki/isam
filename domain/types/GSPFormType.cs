using System;
using System.Collections;
using com.next.common.domain;

namespace com.next.isam.domain.types
{
	[Serializable()]
	public class GSPFormType : DomainData
	{
		private eType _type;
		
		public enum eType
		{
			NoGSPRequired = 0,
			GSPFormA = 1,
			GSPplus = 2,
		}


		public GSPFormType(eType type)
		{
			this._type = type;
		}

		public eType Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public int Id 
		{
			get { return _type.GetHashCode(); }
		}

		public string Name 
		{
			get { return getName(_type.GetHashCode()); }
		}

		public static string getName(int typeId) 
		{			
			if (typeId == eType.GSPFormA.GetHashCode()) 
                return "GSP";
			else if (typeId == eType.GSPplus.GetHashCode()) 
                return "GSP +";
			else return "N/A";
		}

        public static ArrayList getGSPFormTypeList()
        {
            ArrayList list = new ArrayList();
            list.Add(new GSPFormType(eType.NoGSPRequired));
            list.Add(new GSPFormType(eType.GSPFormA));
            list.Add(new GSPFormType(eType.GSPplus));

            return list;
        }
	}
}
