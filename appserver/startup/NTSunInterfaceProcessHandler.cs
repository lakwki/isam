using System;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.isam.appserver.account;
using System.Runtime.Remoting.Lifetime;

namespace com.next.isam.appserver.startup
{
	public class NTSunInterfaceProcessHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "NTSunInterfaceProcessHandler";
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

            if (ts.lastStartTime > ts.lastRunTime || 
				DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 3) 
				return;

			ts.lastStartTime=DateTime.Now;
			runNow();
			ts.lastRunTime=DateTime.Now;
		}


		public void runNow() 
		{
            try
            {
			    //MailHelper.sendGeneralMessage("ISAM - NT Sun Interface Process Start...", DateTime.Now.ToString());
			    NonTradeManager.Instance.processQueues();
                //MailHelper.sendGeneralMessage("ISAM - NT Sun Interface Process Completed", DateTime.Now.ToString());
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
			return NTSunInterfaceProcessHandler.DEFAULT_NAME;
		}
	}
}
