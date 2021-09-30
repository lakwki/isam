using System;
using System.Collections;
using com.next.account.dataserver.worker;

namespace com.next.isam.appserver.account
{
	public class SunManager
	{
		public static SunManager _instance;
		
		private SunWorker sunWorker;

		public SunManager()
		{
			sunWorker = new SunWorker();
		}

		public static SunManager Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new SunManager();
				}
				return _instance;
			}
		}			

		public ArrayList getSunDBList()
		{
			return sunWorker.getSunDBList();
		}
	}
}
