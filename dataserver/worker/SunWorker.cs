using System.Collections;
using System.Data;
using com.next.common.datafactory.worker;
using com.next.infra.persistency.dataaccess;

namespace com.next.account.dataserver.worker
{
	public class SunWorker : Worker
	{
		private static SunWorker _instance;

		public SunWorker()
		{
		}

		public static SunWorker Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new SunWorker();
				}
				return _instance;
			}
		}

		public ArrayList getSunDBList()
		{
			IDataSetAdapter ad = getDataSetAdapter("SunDbApt", "GetAllSunDbNameCmd");
			DataSet dataset = new DataSet();

			int recordsAffected = ad.Fill(dataset);

			if (recordsAffected < 1) return null;
			ArrayList list = new ArrayList();
			foreach(DataRow row in dataset.Tables[0].Rows)
			{
				list.Add((string)row[0]);
			}
			return list;
		}

	}
}
