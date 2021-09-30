using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using System.Runtime.Remoting.Lifetime;
using com.next.infra.util;
using com.next.isam.reporter.invoice;
using com.next.isam.reporter.shipping;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.shipping;
using com.next.isam.reporter.helper;
using com.next.isam.domain.order;
using com.next.infra.persistency.transactions;
using com.next.infra.persistency.dataaccess;
using com.next.isam.domain.types;

namespace com.next.isam.appserver.startup
{
    public class NSSDiscrepancyMailHandler : IStartupHandler
    {
        private static readonly string DEFAULT_NAME = "NSSDiscrepancyMailHandler";

        class TimeState
        {
            public DateTime lastStartTime;
            public DateTime lastRunTime;
        }

        public NSSDiscrepancyMailHandler()
        {
        }

        public void Initialize()
        {
            StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
        }

        private void workOnIt(object state)
        {
            TimeState ts = (TimeState)state;
            if (DateTime.Now.Hour != 2) return;
            if (DateTime.Now.Minute < 25) return;
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;
            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public string handlerName()
        {
            return NSSDiscrepancyMailHandler.DEFAULT_NAME;
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
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                MailHelper.sendGeneralMessage("NSS Discrepancy Mail Handler Started", DateTime.Now.ToString());
                DataSet ds = ShipmentManager.Instance.getNSSDiscrepancyDataSet(new DateTime(2013, 9, 29));
                DataSet tmpDs = new DataSet();
                string fileName = string.Empty;
                int i = 1;
                StringBuilder strResult = new StringBuilder();
                ArrayList  fileList = new ArrayList();

                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Columns.Count == 1 && dt.Rows.Count == 1)
                        fileName = (dt.Rows[0][0].ToString() + ".xls");
                    else
                    {
                        strResult.AppendLine(i.ToString() + ". " + fileName + " : " + dt.Rows.Count.ToString() + " record(s).");
                        i++;
                        fileName = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + fileName;
                        fileList.Add(fileName);
                        tmpDs = new DataSet();
                        tmpDs.Tables.Add(dt.Copy());
                        ExcelWorkbookEngine.CreateWorkbook(tmpDs, fileName);
                    }
                }

                NoticeHelper.sendNSSDiscrepancyMail(strResult.ToString(), fileList);
                MailHelper.sendGeneralMessage("NSS Discrepancy Mail Handler Completed", DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                MailHelper.sendErrorAlert(e, String.Empty);
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void sendInvoiceToGT(ArrayList shipmentIdList, ArrayList invoiceList)
        {
            DateTime genDate = DateTime.Today.AddDays(-1);
            string outputFolder = WebConfig.getValue("appSettings", "INVOICE_OUTPUT_FOLDER");
            string alertFileName = outputFolder + genDate.ToString("yyyyMMdd") + ".xls";
            string fileName = outputFolder + genDate.ToString("yyyyMMdd") + ".pdf";

            InvoiceRpt rpt = InvoiceReportManager.Instance.getInvoiceReport(shipmentIdList, 99999);
            rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, fileName);
            rpt.Dispose();

            EziBuyOSPaymentList osRpt = ShippingReportManager.Instance.getEziBuyOSPaymentReport("GT");
            osRpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, alertFileName);
            osRpt.Dispose();
            NoticeHelper.sendEziBuyInvoiceMail(fileName, alertFileName, invoiceList, genDate);
        }

        private void sendOSPaymentListToGT()
        {
            DateTime genDate = DateTime.Today.AddDays(-1);
            string outputFolder = WebConfig.getValue("appSettings", "INVOICE_OUTPUT_FOLDER");
            string alertFileName = outputFolder + genDate.ToString("yyyyMMdd") + ".xls";

            EziBuyOSPaymentList osRpt = ShippingReportManager.Instance.getEziBuyOSPaymentReport("GT");
            osRpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, alertFileName);
            osRpt.Dispose();
            NoticeHelper.sendEziBuyInvoiceEmptyMail(alertFileName, genDate);

        }

        /*
        private bool isSendToEziBuy(int seasonId, int phaseId)
        {
            if ((seasonId == 18 && phaseId >= 3) || (seasonId > 18))
                return true;
            else
                return false;
        }
        */

        private bool isSendToEziBuy(int phaseId)
        {
            if (phaseId >= 3)
                return true;
            else
                return false;
        }

        private void sendInvoiceToEziBuy(InvoiceDef def, string inboundDeliveryNo)
        {
            string outputFolder = WebConfig.getValue("appSettings", "INVOICE_OUTPUT_FOLDER");
            string fileName = outputFolder + def.InvoiceNo.Replace("/", "") + ".pdf";
            InvoiceRpt rpt = InvoiceReportManager.Instance.getInvoiceReport(ConvertUtility.createArrayList(def.ShipmentId), 99999);
            rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, fileName);
            rpt.Dispose();

            NoticeHelper.sendEziBuyInvoiceMail(fileName, inboundDeliveryNo);
        }

        #endregion
    }
}