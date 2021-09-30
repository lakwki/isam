using System;
using com.next.common.domain;

namespace com.next.isam.domain.types
{
	[Serializable()]
	public class OrderType : DomainData
	{
		public static OrderType FOB = new OrderType(Type.FOB);
		public static OrderType VM = new OrderType(Type.VM);
				
		private Type _type;
		
		public enum Type
		{
			FOB = 1,
			VM = 2,
		}



		public OrderType(Type type)
		{
			this._type = type;
		}



		public string Code 
		{
			get 
			{ 
				switch (_type)
				{
					case Type.FOB :
						return "F";
					case Type.VM :
						return "V";
					default:
                        return String.Empty;
				}				
			}
		}

		public string Name 
		{
			get 
			{ 
				switch (_type)
				{
					case Type.FOB :
						return "FOB";
					case Type.VM :
						return "VM";
					default:
						return "ERROR";
				}				
			}
		}



		public static string getName(string code) 
		{
			if (code == "F") return "FOB";
			else if (code == "V") return "VM";
            else return String.Empty;
		}


	}
}
