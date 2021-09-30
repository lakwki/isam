using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.isam.dataserver.worker;
using com.next.common.domain;
using Microsoft.Exchange.WebServices.Data;
using com.next.isam.domain.types;
using com.next.isam.domain.shipping;
using com.next.isam.domain.order;
using com.next.isam.domain.ils;
using com.next.isam.domain.account;
using System.Text.RegularExpressions;

namespace com.next.isam.appserver.startup
{
    public class PriceDiffAlertHandler : MailHelper, IStartupHandler
    {
        private static readonly string DEFAULT_NAME = "PriceDiffAlertHandler";
        private static readonly ArrayList timeRepository = new ArrayList();
        private static string styleStr =
          "<LINK type=\"text/css\" rel=\"stylesheet\"> " +
          "<STYLE TYPE=\"text/css\"> " +
          "TD {font-family: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; font-size: 12px;} " +
          ".gridHeader {	FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold;	background-color: #ccccff;} " +
          ".colHeader{ FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold; background-color: #f5f5f5;	text-align: Center; vertical-align : middle; } " +
          ".dataCellStr{ FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold;	text-align: Center;	vertical-align : middle;} " +
          ".dataCellNum{ FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold;	text-align: Right;	vertical-align : middle;} " +
          "span { font-family: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; font-size: 12px;}" +
          "</STYLE>";

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
            DayOfWeek day = DateTime.Today.DayOfWeek;

            if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday) return;
            if (currentHour != 8) return;
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public void runNow()
        {
            try
            {
                appserver.helper.MailBoxHelper.Instance.requestExchangeService("APP-ISAM", "Work4ever");
                appserver.helper.MailBoxHelper.Instance.selectMailBoxService("AC_ILSDiscrepancy@nextsl.com.hk");

                //daily check the email reply
                List<EmailMessage> emailChecking = appserver.helper.MailBoxHelper.Instance.getMailHeader("inbox", "Price Difference", GeneralCriteria.ALLSTRING,
                                                "AC_ILSDiscrepancy@nextsl.com.hk", getLastWorkingDay(), DateTime.Now, GeneralCriteria.ALL, GeneralCriteria.ALL);

                if (emailChecking.Count > 0)
                {
                    foreach (EmailMessage temp in emailChecking)
                    {
                        string pattern = @"\[\D{3}/\d{5}/\d{4}\]";
                        var matches = Regex.Matches(temp.Subject, pattern);
                        string number = string.Empty;
                        foreach (Match s in matches)
                        {
                            number = s.ToString();
                        }

                        string[] invoiceNo = splitInvoiceNo(number);
                        InvoiceDef invoice = ShippingWorker.Instance.getInvoiceByInvoiceNo(invoiceNo[0], int.Parse(invoiceNo[1]), int.Parse(invoiceNo[2]));
                        ArrayList compare = ShippingWorker.Instance.getActionHistoryByShipmentIdAndType(invoice.ShipmentId, ActionHistoryType.PRICE_DIFF_RESOLVED.Id);
                        if (compare.Count == 0)
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoice.ShipmentId, 0, ActionHistoryType.PRICE_DIFF_RESOLVED, "Received Price Difference Resolved Email", 1, 99999));
                    }
                }

                int threeWorkingDay = countWorkingDay(3);
                //3 days ago alert
                ArrayList alertb4Three = ShippingWorker.Instance.getActionHistoryByTypeAndDay(ActionHistoryType.PRICE_DIFF_ALERT.Id, -threeWorkingDay);
                foreach (ActionHistoryDef temp in alertb4Three)
                {
                    //find 3 days ago to now record
                    ArrayList resolvedb4Three = ShippingWorker.Instance.getActionHistoryByShipmentIdAndTypeAndDay(temp.ShipmentId, ActionHistoryType.PRICE_DIFF_RESOLVED.Id, -threeWorkingDay);
                    if (resolvedb4Three.Count == 0)
                    {
                        ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(temp.ShipmentId);
                        InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(temp.ShipmentId);
                        ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
                        ILSInvoiceDef ilsInvoiceDef = ILSUploadWorker.Instance.getILSInvoiceByShipmentId(temp.ShipmentId);
                        AdjustmentDetailDef adjDetailDef = AccountWorker.Instance.getOutstandingAdjustmentDetail(temp.ShipmentId, 0, AdjustmentType.SALES_ADJUSTMENT.Id);
                        if (shipmentDef.TotalShippedAmount != adjDetailDef.SettledAmount)
                            NoticeHelper.sendOverILSExceptionLimitEmail(contractDef.Merchandiser, temp.ShipmentId, ilsInvoiceDef.Currency, adjDetailDef.SettledAmount, invoiceDef.InvoiceNo, " - 1st Reminder");
                        else
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.PRICE_DIFF_RESOLVED, "Price Difference Resolved By User", 1, 99999));
                    }
                }

                int eightWorkingDay = countWorkingDay(8);
                //8 days ago alert
                ArrayList alertb4Eight = ShippingWorker.Instance.getActionHistoryByTypeAndDay(ActionHistoryType.PRICE_DIFF_ALERT.Id, -eightWorkingDay);
                foreach (ActionHistoryDef temp in alertb4Eight)
                {
                    //find 8 days ago to now record
                    ArrayList resolvedb4Eight = ShippingWorker.Instance.getActionHistoryByShipmentIdAndTypeAndDay(temp.ShipmentId, ActionHistoryType.PRICE_DIFF_RESOLVED.Id, -eightWorkingDay);
                    if (resolvedb4Eight.Count == 0)
                    {
                        ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(temp.ShipmentId);
                        InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(temp.ShipmentId);
                        ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
                        ILSInvoiceDef ilsInvoiceDef = ILSUploadWorker.Instance.getILSInvoiceByShipmentId(temp.ShipmentId);
                        AdjustmentDetailDef adjDetailDef = AccountWorker.Instance.getOutstandingAdjustmentDetail(temp.ShipmentId, 0, AdjustmentType.SALES_ADJUSTMENT.Id);
                        if (shipmentDef.TotalShippedAmount != adjDetailDef.SettledAmount)
                            NoticeHelper.sendOverILSExceptionLimitEmail(contractDef.Merchandiser, temp.ShipmentId, ilsInvoiceDef.Currency, adjDetailDef.SettledAmount, invoiceDef.InvoiceNo, " - 2nd Reminder");
                        else
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.PRICE_DIFF_RESOLVED, "Price Difference Resolved By User", 1, 99999));
                    }
                }

                int thirteenWorkingDay = countWorkingDay(13);
                //13 days ago alert
                ArrayList alertb4Thirteen = ShippingWorker.Instance.getActionHistoryByTypeAndDay(ActionHistoryType.PRICE_DIFF_ALERT.Id, -thirteenWorkingDay);
                foreach (ActionHistoryDef temp in alertb4Thirteen)
                {
                    //find 13 days ago to now record
                    ArrayList resolvedb4Thirteen = ShippingWorker.Instance.getActionHistoryByShipmentIdAndTypeAndDay(temp.ShipmentId, ActionHistoryType.PRICE_DIFF_RESOLVED.Id, -thirteenWorkingDay);
                    if (resolvedb4Thirteen.Count == 0)
                    {
                        ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(temp.ShipmentId);
                        InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(temp.ShipmentId);
                        ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
                        ILSInvoiceDef ilsInvoiceDef = ILSUploadWorker.Instance.getILSInvoiceByShipmentId(temp.ShipmentId);
                        AdjustmentDetailDef adjDetailDef = AccountWorker.Instance.getOutstandingAdjustmentDetail(temp.ShipmentId, 0, AdjustmentType.SALES_ADJUSTMENT.Id);
                        if (shipmentDef.TotalShippedAmount != adjDetailDef.SettledAmount)
                            NoticeHelper.sendOverILSExceptionLimitEmail(contractDef.Merchandiser, temp.ShipmentId, ilsInvoiceDef.Currency, adjDetailDef.SettledAmount, invoiceDef.InvoiceNo, " - 3rd Reminder");
                        else
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.PRICE_DIFF_RESOLVED, "Price Difference Resolved By User", 1, 99999));
                    }
                }

                int eighteenWorkingDay = countWorkingDay(18);
                //18 days ago alert
                ArrayList alertb4Eighteen = ShippingWorker.Instance.getActionHistoryByTypeAndDay(ActionHistoryType.PRICE_DIFF_ALERT.Id, -eighteenWorkingDay);
                foreach (ActionHistoryDef temp in alertb4Eighteen)
                {
                    //find 18 days ago to now record
                    ArrayList resolvedb4EighteenWorkingDay = ShippingWorker.Instance.getActionHistoryByShipmentIdAndTypeAndDay(temp.ShipmentId, ActionHistoryType.PRICE_DIFF_RESOLVED.Id, -eighteenWorkingDay);
                    if (resolvedb4EighteenWorkingDay.Count == 0)
                    {
                        ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(temp.ShipmentId);
                        InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(temp.ShipmentId);
                        ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
                        ILSInvoiceDef ilsInvoiceDef = ILSUploadWorker.Instance.getILSInvoiceByShipmentId(temp.ShipmentId);
                        AdjustmentDetailDef adjDetailDef = AccountWorker.Instance.getOutstandingAdjustmentDetail(temp.ShipmentId, 0, AdjustmentType.SALES_ADJUSTMENT.Id);
                        if (shipmentDef.TotalShippedAmount != adjDetailDef.SettledAmount)
                            NoticeHelper.sendOverILSExceptionLimitEmail(contractDef.Merchandiser, temp.ShipmentId, ilsInvoiceDef.Currency, adjDetailDef.SettledAmount, invoiceDef.InvoiceNo, " - 4th Reminder");
                        else
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.PRICE_DIFF_RESOLVED, "Price Difference Resolved By User", 1, 99999));
                    }
                }

                int twentyThreeWorkingDay = countWorkingDay(23);
                //23 days ago alert
                ArrayList alertb4TwentyThree = ShippingWorker.Instance.getActionHistoryByTypeAndDay(ActionHistoryType.PRICE_DIFF_ALERT.Id, -twentyThreeWorkingDay);
                foreach (ActionHistoryDef temp in alertb4TwentyThree)
                {
                    //find 23 days ago to now record
                    ArrayList resolvedb4TwentyThreeWorkingDay = ShippingWorker.Instance.getActionHistoryByShipmentIdAndTypeAndDay(temp.ShipmentId, ActionHistoryType.PRICE_DIFF_RESOLVED.Id, -twentyThreeWorkingDay);
                    if (resolvedb4TwentyThreeWorkingDay.Count == 0)
                    {
                        ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(temp.ShipmentId);
                        InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(temp.ShipmentId);
                        ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
                        ILSInvoiceDef ilsInvoiceDef = ILSUploadWorker.Instance.getILSInvoiceByShipmentId(temp.ShipmentId);
                        AdjustmentDetailDef adjDetailDef = AccountWorker.Instance.getOutstandingAdjustmentDetail(temp.ShipmentId, 0, AdjustmentType.SALES_ADJUSTMENT.Id);
                        if (shipmentDef.TotalShippedAmount != adjDetailDef.SettledAmount)
                            NoticeHelper.sendOverILSExceptionLimitEmail(contractDef.Merchandiser, temp.ShipmentId, ilsInvoiceDef.Currency, adjDetailDef.SettledAmount, invoiceDef.InvoiceNo, " - 5th Reminder");
                        else
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.PRICE_DIFF_RESOLVED, "Price Difference Resolved By User", 1, 99999));
                    }
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
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
            return PriceDiffAlertHandler.DEFAULT_NAME;
        }

        private string[] splitInvoiceNo(string subject)
        {
            string[] temp = subject.Split('[');
            string[] temp2 = temp[1].Split(']');
            string[] invoicelist = temp2[0].Split('/');
            return invoicelist;
        }


        private DateTime getLastWorkingDay()
        {
            DateTime lastWorkingDay = DateTime.Today;
            do
            {
                lastWorkingDay = lastWorkingDay.AddDays(-1);
            }
            while (lastWorkingDay.DayOfWeek == DayOfWeek.Saturday || lastWorkingDay.DayOfWeek == DayOfWeek.Sunday);
            return lastWorkingDay;
        }

        private int countWorkingDay(int day)
        {
            int count = day;
            DateTime ss = DateTime.Today;
            for (int i = 0; i < day; i++)
            {
                if (ss.DayOfWeek == DayOfWeek.Saturday || ss.DayOfWeek == DayOfWeek.Sunday)
                {
                    count++;
                    i--;
                }
                ss = ss.AddDays(-1);
            }
            return count;
        }
    }
}
