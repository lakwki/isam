using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using System.Runtime.Remoting.Lifetime;
using com.next.infra.util;
using com.next.isam.reporter.invoice;
using com.next.isam.reporter.shipping;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.Shipping;
using com.next.isam.reporter.helper;
using com.next.isam.domain.order;
using com.next.infra.persistency.transactions;
using com.next.infra.persistency.dataaccess;
using com.next.isam.domain.types;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using com.next.common.appserver;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.datafactory.worker;

namespace com.next.isam.appserver.startup
{
    public class DMSKeepAliveHandler : IStartupHandler
    {
        private static readonly string DEFAULT_NAME = "DMSKeepAliveHandler";

        class TimeState
        {
            public DateTime lastStartTime;
            public DateTime lastRunTime;
        }

        public DMSKeepAliveHandler()
        {
        }

        public void Initialize()
        {
            StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
        }

        private void workOnIt(object state)
        {
            TimeState ts = (TimeState)state;

            if (ts.lastStartTime > ts.lastRunTime ||
                DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 5)
                return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public string handlerName()
        {
            return DMSKeepAliveHandler.DEFAULT_NAME;
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
            /*
            MailHelper.sendGeneralMessage("DMS Keep Alive Started - " + DMSUtil.getSessionId(), DateTime.Now.ToString());
            */
            try
            {

                DMSUtil.KeepSessionOnline();
                ArrayList queryStructs = new ArrayList();
                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Shipping - UK Contract"));
                queryStructs.Add(new QueryStructDef("UK Contract - Delivery", "Hello World"));
                ArrayList qList = DMSUtil.queryDocument(queryStructs);
                /*
                MailHelper.sendGeneralMessage("DMS Keep Alive End - " + DMSUtil.getSessionId(), DateTime.Now.ToString());
                */
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
            }
        }


        #endregion
    }
}