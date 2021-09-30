using System;
using System.Threading;
using com.next.infra.configuration.appserver;
using System.Runtime.Remoting.Lifetime;
using com.next.infra.util;
using com.next.common.domain;
using com.next.isam.appserver.account;


namespace com.next.isam.appserver.startup
{
    public class NUKSalesCutOffHandler : IStartupHandler 
    {
        private static readonly string DEFAULT_NAME = "NUKSalesCutOffHandler";

        class TimeState
        {
            public DateTime lastStartTime;
            public DateTime lastRunTime;
        }

        public NUKSalesCutOffHandler()
        {
        }

        public void Initialize()
        {
            StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
        }

        private void workOnIt(object state)
        {
            TimeState ts = (TimeState)state;
            if (DateTime.Now.Hour != 11) return;
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public string handlerName()
        {
            return NUKSalesCutOffHandler.DEFAULT_NAME;
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
                MailHelper.sendGeneralMessage("NUKSales Cut-off Handler Started", DateTime.Now.ToString());
                 
                AccountFinancialCalenderDef cDef = com.next.common.web.commander.CommonUtil.getAccountPeriodByDate(13, DateTime.Now);


                if (DateTime.Today == cDef.StartDate)
                {
                    MailHelper.sendGeneralMessage("NUKSales Cut-off Handler Starts Capturing Sales...", DateTime.Now.ToString());

                    if (cDef.Period != 1)
                        AccountManager.Instance.captureNUKSales(cDef.BudgetYear, cDef.Period - 1);
                    else
                        AccountManager.Instance.captureNUKSales(cDef.BudgetYear - 1, 12);
                }

                
       
                MailHelper.sendGeneralMessage("NUKSales Cut-off Handler Completed", DateTime.Now.ToString());                                
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
            }
        }
        #endregion


    }
}
