using System;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.isam.appserver.synchronization;
using System.Runtime.Remoting.Lifetime;

//using com.next.isam.domain.account;
//using com.next.isam.appserver.account;

namespace com.next.isam.appserver.startup
{
	public class SynOPSHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "SynOPSHandler";
		private static readonly ArrayList timeRepository = new ArrayList();
        /*
		Set scheduler starting hour
		private static readonly int[] runableHour = {1, 13};
		private static readonly int firstTimeLine = 10;
		private static readonly int secondTimeLine = 15;
		private static readonly int pastDueDay = -5;
        */
				
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
            /*
			int currentHour = DateTime.Now.Hour;
			if (!timeRepository.Contains(currentHour)) return;
			if (DateTime.Now<ts.lastRunTime.AddMinutes(15)) return;
            */
			if (ts.lastStartTime>ts.lastRunTime || 
				DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 9) 
				return;

			ts.lastStartTime=DateTime.Now;
			runNow();
			ts.lastRunTime=DateTime.Now;
		}


		public void runNow() 
		{
			//MailHelper.sendGeneralMessage("Data Sync Start...", DateTime.Now.ToString());
            
			SynchronizationManager.Instance.beginSynchronization();
            SynchronizationManager.Instance.syncAdvancePayment();
            SynchronizationManager.Instance.syncLetterOfGuarantee();

            //AccountManager.Instance.submitSunInterfaceBatch(1, 2018, 5, SunInterfaceTypeRef.getSunMacroTypeIdList(), 2021);
            //AccountManager.Instance.submitSunInterfaceBatch(19, 2018, 5, SunInterfaceTypeRef.getSunMacroTypeIdList(), 2021);
            //MailHelper.sendGeneralMessage("Data Sync Completed", DateTime.Now.ToString());
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
			return SynOPSHandler.DEFAULT_NAME;
		}
	}
}
