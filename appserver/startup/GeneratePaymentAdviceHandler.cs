using System;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;

using System.Runtime.Remoting.Lifetime;

using com.next.isam.appserver.account;



namespace com.next.isam.appserver.startup
{
	public class GeneratePaymentAdviceHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "GeneratePaymentAdviceHandler";
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

			if (ts.lastStartTime > ts.lastRunTime || DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;


			ts.lastStartTime=DateTime.Now;
			runNow();
			ts.lastRunTime=DateTime.Now;
		}

		public void runNow() 
		{
            try
            {

                if (DateTime.Now.Hour == 11)
                {
                    PaymentAdviceManager.Instance.markPaymentAdviceAsChecked();
                }

                PaymentAdviceManager.Instance.importFile();
                PaymentAdviceManager.Instance.generatePaymentAdvice();
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
			return GeneratePaymentAdviceHandler.DEFAULT_NAME;
		}
	}
}
