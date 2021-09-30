using System;
using com.next.common.domain;

namespace com.next.isam.domain.types
{
	[Serializable()]
	public class TradingAgencyId : DomainData
	{
		public static TradingAgencyId NSL = new TradingAgencyId(Agency.nsl);
		public static TradingAgencyId NSLVM = new TradingAgencyId(Agency.nslvm);
		public static TradingAgencyId NSLUK = new TradingAgencyId(Agency.nsluk);
		public static TradingAgencyId NSLVMOT = new TradingAgencyId(Agency.nslvmot);

		private Agency _agency;

        public enum Agency
        {
            nsl = 1,
            nslvm = 2,
            nsluk = 3,
            nslvmot = 4,
        }

        public TradingAgencyId(Agency agency)
        {
            this._agency = agency;
        }

        public int AgencyId
        {
            get
            {
                return _agency.GetHashCode();
            }
        }


        public string AgencyName
        {
            get
            {
                switch (_agency)
                {
                    case Agency.nsl:
                        return "NSL";
                    case Agency.nslvm:
                        return "NSL VM";
                    case Agency.nsluk:
                        return "NSL UK";
                    case Agency.nslvmot:
                        return "NSL VM (OT)";
                    default:
                        return "ERROR";
                }
            }
        }

        public static string getName(int id)
        {
            if (id == Agency.nsl.GetHashCode()) return "NSL";
            else if (id == Agency.nslvm.GetHashCode()) return "NSL VM (TR)";
            else if (id == Agency.nsluk.GetHashCode()) return "NSL UK";
            else if (id == Agency.nslvmot.GetHashCode()) return "NSL VM (OT)";
            else return "N/A";
        }

    }
}
