using System;
using System.Collections;
using System.Threading;
using System.Runtime.Remoting.Lifetime;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.isam.appserver.shipping;
using com.next.isam.dataserver.worker;

namespace com.next.isam.appserver.startup
{
    public class ShippingDataUploadHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "ShippingDataUploadHandler";
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
			int currentHour = DateTime.Now.Hour;
            
            //MailHelper.sendGeneralMessage(DEFAULT_NAME + " Trigger", "CurrentTime="+DateTime.Now.ToString() + "<br>LastStart=" +  ts.lastStartTime.ToString()+"<br>LastRun="+ts.lastRunTime.ToString());

			if (ts.lastStartTime > ts.lastRunTime || DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;

			ts.lastStartTime=DateTime.Now;
			runNow();
			ts.lastRunTime=DateTime.Now;
		}

        public void runNow()
        {
            try
            {
            //MailHelper.sendGeneralMessage(DEFAULT_NAME + " Started", DateTime.Now.ToString());
            Shipping.ShippingDataUploadManager.Instance.importAllFile();
            //MailHelper.sendGeneralMessage(DEFAULT_NAME + " Completed", DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, string.Empty);
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
			return DEFAULT_NAME;
		}
	}
}
