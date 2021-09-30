using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.infra.configuration.appserver;
using System.Collections;
using com.next.infra.util;
using System.Threading;
using com.next.isam.reporter.accounts;

namespace com.next.isam.appserver.startup
{
    public class SendNSSWeeklyReportHandler : IStartupHandler
    {
        private static readonly string DEFAULT_NAME = "SendNSSWeeklyReportHandler";
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

            if (DateTime.Now.DayOfWeek != DayOfWeek.Friday) return;
            if (DateTime.Now.Hour != 9) return;
            if (DateTime.Now.Minute < 5) return;
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public void runNow()
        {
            try
            {
                //MailHelper.sendGeneralMessage(DEFAULT_NAME + " Started", DateTime.Now.ToString());
                reporter.NSSReport.NSSReportManager.Instance.generateNNSWeeklyReport();
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
