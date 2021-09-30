using System;
using System.Threading;
using com.next.infra.configuration.appserver;
using System.Runtime.Remoting.Lifetime;
using com.next.infra.util;

namespace com.next.isam.appserver.startup
{
	public class BankReconciliationHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "BankReconciliationHandler";

		class TimeState
		{
			public DateTime lastStartTime;
			public DateTime lastRunTime;
		}

		public BankReconciliationHandler()
		{
		}

		public void Initialize() 
		{
			StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
		}

		private void workOnIt(object state)
		{
			TimeState ts = (TimeState) state;
			if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 30) return;			

			ts.lastStartTime=DateTime.Now;
			runNow();	
			ts.lastRunTime=DateTime.Now;
		}

		public string handlerName() 
		{
			return BankReconciliationHandler.DEFAULT_NAME;
		}

		private Timer load(TimeSpan ts)
		{
			TimeState state = new TimeState();
			TimerCallback tcall = new TimerCallback(this.workOnIt);
			Timer timer = new Timer(tcall, state, new TimeSpan(0), ts);
			return timer;
		}

		#region TIMERJOBS
		public void runNow() 
		{
			try
			{
                //MailHelper.sendGeneralMessage("Bank Reconciliation Started", DateTime.Now.ToString());
                com.next.isam.appserver.account.BankReconciliationManager.instance.processBankReconciliation();
                //MailHelper.sendGeneralMessage("Bank Reconciliation Completed", DateTime.Now.ToString());
			}
			catch(Exception e) 
			{
				MailHelper.sendErrorAlert(e, String.Empty);
			}
		}
		#endregion
	}
}