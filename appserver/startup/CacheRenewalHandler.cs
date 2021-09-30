using System;
using System.Threading;
using System.Runtime.Remoting.Lifetime;

using com.next.infra.util;
using com.next.infra.configuration.appserver;
using com.next.common.security;
using com.next.common.domain.module;
using com.next.isam.appserver.helper;
using com.next.isam.dataserver.worker;
/*
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration; 
*/

namespace com.next.isam.appserver.startup
{
	public class CacheRenewalHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "CacheRenewalHandler";

		// use this to help your determination, otherwise set null in the param in timer constructor
		class TimeState
		{
			public DateTime lastStartTime;
			public DateTime lastRunTime;
		}

		public void Initialize()
		{
			StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
		}

		private void workOnIt(object state)
		{
			TimeState ts = (TimeState) state;

			//can't run within one hour
			if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;

			//refresh cache at 8am & 12noon;
			if (DateTime.Now.Hour != 8 && DateTime.Now.Hour != 12) return;

			ts.lastStartTime = DateTime.Now;
			runNow();
			ts.lastRunTime = DateTime.Now;
		}

		public void runNow()
		{
            DateTime startTime = DateTime.Now;
            /*
            LogWriter defaultWriter = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
            LogEntry log = new LogEntry();
            log.Message = "Started";
            log.Categories.Add("General");
            log.Priority = 2;
            log.Title = "CacheRenewalHandler";
            defaultWriter.Write(log); 
            */

			//clear menu & access
			SecurityManager.Instance.notifyChanged();

			//reload menu & access
			SecurityManager.Instance.getAccessibleMenu(AccessMapper.ISAM.Id, 0);

            /*
            log = new LogEntry();
            log.Message = "Completed [elapsed time : " + getElapsedTimeString(startTime) + "]";
            log.Categories.Add("General");
            log.Priority = 2;
            log.Title = "CacheRenewalHandler";
            defaultWriter.Write(log); 
            */

			//send email after completed reloaded
			NoticeHelper.sendGeneralMessage("Menu Cache Renewal", "Menu Cache has been reloaded.");
		}


        private string getElapsedTimeString(DateTime startTime)
        {
            string s = string.Empty;
            DateTime currentTime = DateTime.Now;
            if (currentTime.Subtract(startTime).Days > 0)
                s = currentTime.Subtract(startTime).Days.ToString() + " day(s)";
            if (currentTime.Subtract(startTime).Hours > 0)
                s += ((s == string.Empty ? string.Empty : " ") + (currentTime.Subtract(startTime).Hours.ToString() + " hour(s)"));
            if (currentTime.Subtract(startTime).Minutes > 0)
                s += ((s == string.Empty ? string.Empty : " ") + (currentTime.Subtract(startTime).Minutes.ToString() + " minute(s)"));
            if (currentTime.Subtract(startTime).Seconds > 0)
                s += ((s == string.Empty ? string.Empty : " ") + (currentTime.Subtract(startTime).Seconds.ToString() + " second(s)"));
            return s;
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
			return CacheRenewalHandler.DEFAULT_NAME;
		}
	}
}
