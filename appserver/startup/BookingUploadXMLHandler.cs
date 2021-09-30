using System;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.isam.appserver.order;
using System.Runtime.Remoting.Lifetime;

namespace com.next.isam.appserver.startup
{
	public class BookingUploadXMLHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "EBookingUploadXMLHandler";
		private static readonly ArrayList timeRepository = new ArrayList();
				
		class TimeState 
		{
			public DateTime lastStartTime;
			public DateTime lastRunTime;
		}

		public void Initialize() 
		{
			/*
			for (int i = 0; i<runableHour.Length; i++) 
			{
				timeRepository.Add(runableHour[i]);
			}
			*/
			StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
		}

		//we don't need to use the state here
		private void workOnIt(object state)
		{
            TimeState ts = (TimeState)state;

            if (ts.lastStartTime > ts.lastRunTime ||
                DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60)
                return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;

            /*
			TimeState ts = (TimeState)state;
			if (DateTime.Now.Hour != 3) return;
			if (DateTime.Now.Minute < 30) return;
			if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;
			ts.lastStartTime=DateTime.Now;
			runNow();	
			ts.lastRunTime=DateTime.Now;
            */
		}

		public void runNow() 
		{
            try
            {
                //MailHelper.sendGeneralMessage("BookingUploadXMLManager Start", DateTime.Now.ToString());
                com.next.isam.appserver.order.BookingUploadXMLManager.instance.readXMLFile();
                //MailHelper.sendGeneralMessage("BookingUploadXMLManager Completed", DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
            }
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
			return BookingUploadXMLHandler.DEFAULT_NAME;
		}
	}
}
