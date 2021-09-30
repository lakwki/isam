using System;
using System.Collections;
using com.next.common.domain;

namespace com.next.isam.domain.types
{
	/// <summary>
	/// Summary description for OPRFabricType.
	/// </summary>
	[Serializable()]
	public class OPRFabricType : DomainData
	{
		public static OPRFabricType NA = new OPRFabricType(Type.na);
		public static OPRFabricType Turkish = new OPRFabricType(Type.turkish);
		public static OPRFabricType European = new OPRFabricType(Type.european);
		public static OPRFabricType TurkishAndEuropean = new OPRFabricType(Type.turkishandeuropean);
		public static OPRFabricType WithOPRFabric = new OPRFabricType(Type.withoprfabric);
		
		private Type _type;
		
		public enum Type 
		{
			na = -1,
			turkish = 1,
			european = 2,
			turkishandeuropean = 3,
			withoprfabric = -2,
		}

		
		public OPRFabricType(Type type)
		{
			this._type = type;
		}
		

		public int OPRFabricTypeId
		{
			get 
			{
				return _type.GetHashCode();
			}
		}


		public string OPRFabricName 
		{
			get 
			{ 
				switch (_type)
				{
					case Type.turkish :
						return "Turkish";
					case Type.european :
						return "European";
					case Type.turkishandeuropean :
						return "Turkish and European";
					default:
						return "N/A";
				}				
			}
		}

		
		public static ICollection getCollectionValue() 
		{
			ArrayList values = new ArrayList();

			values.Add(OPRFabricType.NA);
			values.Add(OPRFabricType.Turkish);
			values.Add(OPRFabricType.European);
			values.Add(OPRFabricType.TurkishAndEuropean);
		
			return values;
		}

		
		public static string getName(int id) 
		{
			if (id == Type.turkish.GetHashCode()) return "Turkish";
			else if (id == Type.european.GetHashCode()) return "European";
				else if (id == Type.turkishandeuropean.GetHashCode()) return "Turkish and European";
			else return "N/A";
		}


		public string OPRFabricSelectionName 
		{
			get 
			{ 
				switch (_type)
				{
					case Type.turkish :
						return "Turkish OPR Only";
					case Type.european :
						return "European OPR Only";
					case Type.turkishandeuropean :
						return "Both Turkish and European OPR";
					case Type.withoprfabric :
						return "All OPR Fabric";
					default:
						return "No OPR Fabric";
				}				
			}
		}


		public string OPRFabricReportSelectionName 
		{
			get 
			{ 
				switch (_type)
				{
					case Type.turkish :
						return "Turkish OPR Only";
					case Type.european :
						return "European OPR Only";
					case Type.turkishandeuropean :
						return "Both Turkish and European OPR";
					case Type.withoprfabric :
						return "All OPR Fabric";
					default:
						return "--";
				}				
			}
		}

		
		public static ICollection getCollectionValueForReport() 
		{
			ArrayList values = new ArrayList();

			values.Add(OPRFabricType.NA);
			values.Add(OPRFabricType.WithOPRFabric);
			values.Add(OPRFabricType.Turkish);
			values.Add(OPRFabricType.European);
			values.Add(OPRFabricType.TurkishAndEuropean);
		
			return values;
		}


	}
}
