using System;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.isam.appserver.order;
using System.Runtime.Remoting.Lifetime;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.isam.appserver.account;
using com.next.common.domain.types;
using com.next.common.web.commander;

namespace com.next.isam.appserver.startup
{
	public class MockShopSunAccountUploadHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "MockShopSunAccountUploadHandler";
		private static readonly ArrayList timeRepository = new ArrayList();
				
		class TimeState 
		{
			public DateTime lastStartTime;
			public DateTime lastRunTime;
		}

		public void Initialize() 
		{
			StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
		}

		//we don't need to use the state here
		private void workOnIt(object state)
		{
			TimeState ts = (TimeState)state;
			/*
			if (DateTime.Now.Hour ==7 && ts.needToRun!=1 && ts.needToRun!=2)
			{
				ts.needToRun=0;
			}
			*/
			if (DateTime.Now.Hour != 21) return;
			if (DateTime.Now.Minute < 45) return;
			if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;
			/*
			if (ts.needToRun == 0)
			{
				ts.lastStartTime=DateTime.Now;
				ts.needToRun = 1;
				runNow();	
				ts.lastRunTime=DateTime.Now;	
			}
			else if (DateTime.Now.Subtract(ts.lastRunTime).TotalMinutes>2 && ts.needToRun == 1)
			{
				ts.needToRun = 2;
				runReadXML();	
			}
			else
			{
				return;
			}
			*/
			ts.lastStartTime=DateTime.Now;
			runNow();	
			ts.lastRunTime=DateTime.Now;
		}

		public void runNow() 
		{
            /*
			MailHelper.sendGeneralMessage("Sun Interface Submission Handler Start", DateTime.Now.ToString());
            MailHelper.sendGeneralMessage("Sun Interface Submission Handler Completed", DateTime.Now.ToString());
            */
		}

        private Timer load(TimeSpan ts)
		{
			TimeState state = new TimeState();
			TimerCallback tcall = new TimerCallback(this.workOnIt);
			Timer timer = new Timer(tcall, state, new TimeSpan(0), ts);
			return timer;
		}

		public string handlerName() 
		{
			return MockShopSunAccountUploadHandler.DEFAULT_NAME;
		}
	}
}
