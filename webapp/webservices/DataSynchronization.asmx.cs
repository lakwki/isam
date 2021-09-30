using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using com.next.isam.appserver.synchronization;
using com.next.isam.appserver.ils;
using com.next.isam.appserver.order;
using com.next.infra.configuration.appserver;
using com.next.isam.appserver.account;
using com.next.isam.domain.account;
using com.next.common.appserver;
using com.next.isam.domain.types;
using com.next.common.web.commander;
using com.next.isam.appserver.common;
using com.next.infra.util;
using com.next.common.domain;
using com.next.common.datafactory.worker;
using com.next.common.domain.types;

namespace com.next.isam.webapp.webservices
{
    [WebService(Namespace = "http://www.nextsl.com.hk/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DataSynchronization : System.Web.Services.WebService
    {

        [WebMethod]
        public void synchronize(int shipmentId)
        {
            SynchronizationManager.Instance.synchronizeShipmentToISAM(shipmentId);
        }

        [WebMethod]
        public void synchronizeAdvancePaymentToISAM(int paymentId)
        {
            SynchronizationManager.Instance.synchronizeAdvancePaymentToISAM(paymentId);
        }


        [WebMethod]
        public void synchronizeAll()
        {
            if (CommonManager.Instance.getSystemParameterByName("IS_EB_RUNNING").ParameterValue == "N")
            {
                SynchronizationManager.Instance.beginSynchronization();
                SynchronizationManager.Instance.syncAdvancePayment();
                SynchronizationManager.Instance.syncLetterOfGuarantee();
            }
        }

        [WebMethod]
        public void processILS()
        {
            if (CommonManager.Instance.getSystemParameterByName("IS_EB_RUNNING").ParameterValue == "N")
            {
                ILSUploadManager.Instance.processILSResultFiles();
                ILSUploadManager.Instance.sendIncompleteOutgoingILSMsg();
                ILSUploadManager.Instance.createQCCApprovalMessage();
                ILSUploadManager.Instance.processFiles();
                ILSUploadManager.Instance.processNSCFiles();
                ILSUploadManager.Instance.createOriginContractMessage("00000");
                UKItemUploadManager.Instance.processFiles();
            }
        }

        [WebMethod]
        public void processEBooking()
        {
            BookingUploadXMLManager.instance.readXMLFile();
        }

        [WebMethod]
        public string getUserEmail(int userId)
        {
            return GeneralManager.Instance.getUserByKey(userId).EmailAddress;
        }

        [WebMethod]
        public void processInterfaceSubmission()
        {
            MailHelper.sendGeneralMessage("Sun Interface Submission Handler Start", DateTime.Now.ToString());

            AccountFinancialCalenderDef calDef = GeneralWorker.Instance.getAccountPeriodByDate(AppId.ISAM.Code, DateTime.Today);
            if (calDef.EndDate.AddDays(-7) == DateTime.Today)
            {
                StartupLoaderFactory.Instance.stopHandler("MockShopSunAccountUploadHandler");
                CommonManager.Instance.disableSystemParam(12, true);
                MailHelper.sendGeneralMessage("MockShopSunAccountUploadHandler has been stopped due to Mock Shop Sales Cut-Off", DateTime.Now.ToString());
            }
            if (calDef.EndDate == DateTime.Today)
            {
                StartupLoaderFactory.Instance.stopHandler("SunInterfaceSubmissionHandler");
                CommonManager.Instance.disableSystemParam(11, true);
                MailHelper.sendGeneralMessage("SunInterfaceSubmissionHandler has been stopped due to Sales Cut-Off", DateTime.Now.ToString());
            }
            else
            {
                submitUTOfficeRequests();
                submitNonUTOfficeRequests();
            }

            MailHelper.sendGeneralMessage("Sun Interface Submission Handler Completed", DateTime.Now.ToString());

        }

        private void submitUTOfficeRequests()
        {
            SunInterfaceQueueDef def = null;
            int[] officeIds;
            officeIds = new int[2] { 1, 2 };

            for (int i = 0; i <= officeIds.GetUpperBound(0); i++)
            {
                def = new SunInterfaceQueueDef();
                def.QueueId = -1;
                def.OfficeGroup = CommonManager.Instance.getReportOfficeGroupByKey(officeIds[i]);
                def.SunInterfaceTypeId = SunInterfaceTypeRef.Id.Purchase.GetHashCode();
                def.CategoryType = CategoryType.DAILY;
                if (officeIds[i] == 1 || officeIds[i] == 17)
                    def.User = CommonUtil.getUserByKey(246); // joey
                else if (officeIds[i] == 2)
                    def.User = CommonUtil.getUserByKey(264); // annie
                def.SourceId = 2;
                def.SubmitTime = DateTime.Now;
                def.FiscalYear = 0;
                def.Period = 0;
                def.PurchaseTerm = 0;
                def.UTurn = 2;
                AccountManager.Instance.submitSunInterfaceRequest(def);

                /*
                def = (SunInterfaceQueueDef)def.Clone();
                def.QueueId = -1;
                def.SubmitTime = DateTime.Now;
                def.UTurn = 2;
                AccountManager.Instance.submitSunInterfaceRequest(def);
                */

                def = (SunInterfaceQueueDef)def.Clone();
                def.QueueId = -1;
                def.CategoryType = CategoryType.REVERSAL;
                def.SubmitTime = DateTime.Now;
                def.UTurn = 2;
                AccountManager.Instance.submitSunInterfaceRequest(def);

                /*
                def = (SunInterfaceQueueDef)def.Clone();
                def.QueueId = -1;
                def.SubmitTime = DateTime.Now;
                def.UTurn = 1;
                AccountManager.Instance.submitSunInterfaceRequest(def);
                */
            }
        }

        private void submitNonUTOfficeRequests()
        {
            SunInterfaceQueueDef def = null;
            int[] officeIds;
            officeIds = new int[10] { 3, 4, 7, 8, 9, 13, 14, 16, 18, 19 };

            for (int i = 0; i <= officeIds.GetUpperBound(0); i++)
            {

                def = new SunInterfaceQueueDef();
                def.QueueId = -1;
                def.OfficeGroup = CommonManager.Instance.getReportOfficeGroupByKey(officeIds[i]);
                def.SunInterfaceTypeId = SunInterfaceTypeRef.Id.Purchase.GetHashCode();
                def.CategoryType = CategoryType.DAILY;
                if (officeIds[i] == 4 || officeIds[i] == 3 || officeIds[i] == 16)
                    def.User = CommonUtil.getUserByKey(264); // annie
                else if (officeIds[i] == 9 || officeIds[i] == 13 || officeIds[i] == 14 || officeIds[i] == 8 || officeIds[i] == 18 || officeIds[i] == 7 || officeIds[i] == 19)
                    def.User = CommonUtil.getUserByKey(1830); // jackson fok
                def.SourceId = 2;
                def.SubmitTime = DateTime.Now;
                def.FiscalYear = 0;
                def.Period = 0;
                def.PurchaseTerm = 0;
                def.UTurn = 0;
                AccountManager.Instance.submitSunInterfaceRequest(def);

                def = (SunInterfaceQueueDef)def.Clone();
                def.QueueId = -1;
                def.CategoryType = CategoryType.REVERSAL;
                def.SubmitTime = DateTime.Now;
                AccountManager.Instance.submitSunInterfaceRequest(def);
            }
        }


    }


}
