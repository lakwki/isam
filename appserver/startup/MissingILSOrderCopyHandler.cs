using System;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;

using System.Runtime.Remoting.Lifetime;

namespace com.next.isam.appserver.startup
{
	public class MissingILSOrderCopyHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "MissingILSOrderCopyHandler";
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

            if (DateTime.Now.Hour != 5) return;
            if (DateTime.Now.Minute < 30) return;
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;

            /*
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 30) return;
            */

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

		public void runNow() 
		{
            try
            {

                MailHelper.sendGeneralMessage("MissingILSOrderCopy Handler Start", DateTime.Now.ToString());
                com.next.isam.appserver.ils.ILSUploadManager.Instance.createOriginContractMessage("MISSING");
                MailHelper.sendGeneralMessage("MissingILSOrderCopy Handler Completed", DateTime.Now.ToString());
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
            return MissingILSOrderCopyHandler.DEFAULT_NAME;
		}
	}
}
