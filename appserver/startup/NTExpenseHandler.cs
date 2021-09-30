using System;
using System.Threading;
using com.next.infra.configuration.appserver;
using System.Runtime.Remoting.Lifetime;
using com.next.infra.util;
using com.next.isam.appserver.account;

namespace com.next.isam.appserver.startup
{
    public class NTExpenseHandler : IStartupHandler 
    {
        private static readonly string DEFAULT_NAME = "NTExpenseHandler";

        class TimeState
        {
            public DateTime lastStartTime;
            public DateTime lastRunTime;
        }

        public NTExpenseHandler()
        {
        }

        public void Initialize()
        {
            StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
        }

        private void workOnIt(object state)
        {
            TimeState ts = (TimeState)state;
            if (DateTime.Now.Hour != 23) return;
            if (DateTime.Now.Minute < 30) return;
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public string handlerName()
        {
            return NTExpenseHandler.DEFAULT_NAME;
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
                //MailHelper.sendGeneralMessage("Non-Trade Expense Handler Started", DateTime.Now.ToString());
                NonTradeManager.Instance.sendNTInvoicePendingForApprovalNotification();
                //MailHelper.sendGeneralMessage("Non-Trade Expense Handler Completed", DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
            }
        }
        #endregion


    }
}
