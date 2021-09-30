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
    public class DMSUploadRevisionHandler : IStartupHandler
    {
        private static readonly string DEFAULT_NAME = "DMSUploadRevisionHandler";

        class TimeState
        {
            public DateTime lastStartTime;
            public DateTime lastRunTime;
        }

        public DMSUploadRevisionHandler()
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
                DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 12)
                return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public string handlerName()
        {
            return DMSUploadRevisionHandler.DEFAULT_NAME;
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
            /* MailHelper.sendGeneralMessage("DMS Upload NS Invoice Started", DateTime.Now.ToString()); */
            /* this.replaceDocuments(); */
            /*
            this.queryDummyDoc();
            */
            this.uploadDocuments();
            /* MailHelper.sendGeneralMessage("DMS Upload NS Invoice Completed", DateTime.Now.ToString()); */
        }

        private void queryDummyDoc()
        {
            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Shipping - UK Contract"));
            queryStructs.Add(new QueryStructDef("UK Contract - Delivery", "Hello World"));
            ArrayList qList = DMSUtil.queryDocument(queryStructs);
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private void uploadDocuments()
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                
                ArrayList officeList = GeneralManager.Instance.getOfficeList();
                officeList.Sort(new ArrayListHelper.Sorter("OfficeId"));
                /*
                ArrayList officeList = ConvertUtility.createArrayList(GeneralWorker.Instance.getOfficeRefByKey(3));
                officeList.Add(GeneralWorker.Instance.getOfficeRefByKey(1));
                */

                foreach (OfficeRef office in officeList)
                {
                    if (OfficeId.getDMSFolder(office.OfficeId) != String.Empty)
                    {
                        string uploadFolder = OfficeId.getDMSUploadPath(office.OfficeId);
                        string[] docFiles = System.IO.Directory.GetFiles(uploadFolder + "Upload-Doc\\");
                        Array.Sort(docFiles);
                        ArrayList attachmentList = null;
                        string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                        for (int i = 0; i < docFiles.Length; i++)
                        {

                            try
                            {
                                File.SetAttributes(docFiles[i], FileAttributes.Normal);
                            }
                            catch (UnauthorizedAccessException e)
                            {
                                MailHelper.sendErrorAlert(e, String.Empty);
                                continue;
                            }
                            FileUtility.clearFolder(uploadFolder + "Working", true);

                            System.IO.FileInfo fi = new FileInfo(docFiles[i]);

                            if (fi.Name.IndexOf('.') == -1)
                            {
                                NoticeHelper.sendDMSDocUploadInvalidFileFailureEmail(office.OfficeId, fi.Name, docFiles[i], DateTime.Now);
                                File.Copy(docFiles[i], uploadFolder + "Working\\" + fi.Name);
                                this.moveToFolder(uploadFolder, tsFolderName, "Failure");
                                File.Delete(docFiles[i]);
                                continue;
                            }
                            string name = fi.Name.Substring(0, fi.Name.IndexOf('.')).ToUpper();
                            if (name.IndexOf('-') == -1)
                            {
                                NoticeHelper.sendDMSDocUploadInvalidFileFailureEmail(office.OfficeId, fi.Name, docFiles[i], DateTime.Now);
                                File.Copy(docFiles[i], uploadFolder + "Working\\" + fi.Name);
                                this.moveToFolder(uploadFolder, tsFolderName, "Failure");
                                File.Delete(docFiles[i]);
                                continue;
                            }

                            int j = 0;
                            int idx = 0;

                            while (name.IndexOf('-', j) != -1)
                            {
                                idx = name.IndexOf('-', j);
                                j = idx + 1;
                            }

                            string contractNo = name.Substring(0, idx).Trim();
                            int noOfDigit;
                            for (noOfDigit = 0; j < name.Length; j++,noOfDigit++)
                                if (!Char.IsNumber(name[j])) break;
                            bool isFCR = (name.Substring(j).IndexOf("FCR") >= 0);
                            string extension = fi.Name.Substring(fi.Name.LastIndexOf('.'));

                            string n = name.Substring(idx + 1, noOfDigit).Trim();
                            name = contractNo + "-" + n;

                            int deliveryNo = 0;

                            if (n.IndexOf('0') == 0 || !int.TryParse(n, out deliveryNo))
                            {
                                NoticeHelper.sendDMSDocUploadInvalidFileFailureEmail(office.OfficeId, fi.Name, docFiles[i], DateTime.Now);
                                File.Copy(docFiles[i], uploadFolder + "Working\\" + fi.Name);
                                this.moveToFolder(uploadFolder, tsFolderName, "Failure");
                                File.Delete(docFiles[i]);
                                continue;
                            }

                            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByContractNoAndDeliveryNo(contractNo, deliveryNo);
                            ContractDef contractDef = OrderSelectWorker.Instance.getContractByContractNo(contractNo);

                            if (shipmentDef == null)
                            {
                                NoticeHelper.sendDMSDocUploadShipmentNotFoundFailureEmail(office.OfficeId, fi.Name, docFiles[i], DateTime.Now);
                                File.Copy(docFiles[i], uploadFolder + "Working\\" + fi.Name);
                                /*
                                File.Copy(docFiles[i], WebConfig.getValue("appSettings", "DMS_FAILURE_UPLOAD_Folder") + fi.Name); 
                                */
                                this.moveToFolder(uploadFolder, tsFolderName, "Failure");
                                File.Delete(docFiles[i]);
                                continue;
                            }
                            /*
                            if (office.OfficeId != contractDef.Office.OfficeId && ((office.OfficeId == OfficeId.HK.Id && contractDef.Office.OfficeId != OfficeId.DG.Id)
                                                                                   || (office.OfficeId == OfficeId.TH.Id && contractDef.Office.OfficeId != OfficeId.VN.Id)
                                                                                   || (office.OfficeId == OfficeId.BD.Id && contractDef.Office.OfficeId != OfficeId.PK.Id)
                                                                                   || (office.OfficeId == OfficeId.TR.Id && contractDef.Office.OfficeId != OfficeId.EG.Id)
                                                                                   || (office.OfficeId == OfficeId.DG.Id && contractDef.Office.OfficeId != OfficeId.HK.Id)
                                                                                   || (office.OfficeId == OfficeId.VN.Id && contractDef.Office.OfficeId != OfficeId.TH.Id)
                                                                                   || (office.OfficeId == OfficeId.PK.Id && contractDef.Office.OfficeId != OfficeId.BD.Id)
                                                                                   || (office.OfficeId == OfficeId.EG.Id && contractDef.Office.OfficeId != OfficeId.TR.Id)
                                                                                   || (office.OfficeId == OfficeId.SH.Id && office.OfficeId == OfficeId.SL.Id 
                                                                                       && office.OfficeId == OfficeId.ND.Id && office.OfficeId == OfficeId.IND.Id)))
                            */
                            if (office.OfficeId != contractDef.Office.OfficeId && ((office.OfficeId == OfficeId.HK.Id && contractDef.Office.OfficeId != OfficeId.DG.Id)
                                                                                   || (office.OfficeId == OfficeId.DG.Id && contractDef.Office.OfficeId != OfficeId.HK.Id)))
                            {
                                NoticeHelper.sendDMSDocUploadOfficeDiscrepancyFailureEmail(office.OfficeId, fi.Name, docFiles[i], DateTime.Now, contractDef.Office.OfficeId);
                                File.Copy(docFiles[i], uploadFolder + "Working\\" + fi.Name);
                                this.moveToFolder(uploadFolder, tsFolderName, "Failure");
                                File.Delete(docFiles[i]);
                                continue;
                            }

                            ArrayList queryStructs = new ArrayList();
                            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Shipping - UK Contract"));
                            queryStructs.Add(new QueryStructDef("UK Contract - Delivery", name));
                            ArrayList qList = DMSUtil.queryDocument(queryStructs);
                            long docId = 0;
                            attachmentList = new ArrayList();

                            foreach (DocumentInfoDef docInfoDef in qList)
                            {
                                docId = docInfoDef.DocumentID;
                                foreach (AttachmentInfoDef attDef in docInfoDef.AttachmentInfos)
                                {
                                    byte[] attContent = DMSUtil.getAttachment(attDef.AttachmentID);
                                    System.IO.FileStream fs = new System.IO.FileStream(uploadFolder + "Working\\" + attDef.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                                    fs.Write(attContent, 0, attContent.Length);
                                    fs.Close();
                                    /*
                                    attachmentList.Add(uploadFolder + "Working\\" + attDef.FileName);
                                    */
                                }
                            }

                            if (File.Exists(uploadFolder + "Working\\" + fi.Name))
                                File.Delete(uploadFolder + "Working\\" + fi.Name);

                            if (fi.Extension.ToLower() == ".pdf" && (office.OfficeId != OfficeId.SH.Id && !IndustryManager.Instance.IsNoDMSRepairVendor(shipmentDef.Vendor.VendorId)))
                            {
                                /*
                                File.Copy(docFiles[i], uploadFolder + "Working\\" + fi.Name);
                                */

                                bool rtnVal = this.repairPdf(docFiles[i], uploadFolder + "Working\\" + fi.Name);
                                if (!File.Exists(uploadFolder + "Working\\" + fi.Name) || !rtnVal)
                                {
                                    NoticeHelper.sendDMSDocUploadPdfRepairFailureEmail(office.OfficeId, fi.Name, docFiles[i], DateTime.Now);
                                    File.Copy(docFiles[i], uploadFolder + "Working\\" + fi.Name, true);
                                }
                            }
                            else
                                File.Copy(docFiles[i], uploadFolder + "Working\\" + fi.Name);

                            if (docId > 0)
                            {
                                if (attachmentList.Count == 0)
                                    attachmentList.Add(uploadFolder + "Working\\" + fi.Name);

                                /*
                                DMSUtil.DeleteSingleAttachment(docId, queryStructs, fi.Name);
                                string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                                */
                                string strReturn = DMSUtil.UpdateDocumentWithoutExistingAttachments(docId, queryStructs, attachmentList);

                                if (strReturn != string.Empty) NoticeHelper.sendDMSAPIUploadError("Shipping Doc Amendment", strReturn, docFiles[i]);
                                /*
                                NoticeHelper.sendDMSUploadDocEmail(OfficeId.getName(contractDef.Office.OfficeId), fi.Name, docFiles[i], 1);
                                */

                                this.moveToFolder(uploadFolder, tsFolderName, "Complete");
                                File.Copy(docFiles[i], uploadFolder + "Complete" + "\\" + tsFolderName + "\\" + fi.Name, true);
                                if (strReturn == string.Empty) File.Delete(docFiles[i]);
                            }
                            else
                            {
                                attachmentList.Add(uploadFolder + "Working\\" + fi.Name);

                                string msg = DMSUtil.CreateDocument(0, OfficeId.getDMSFolder(contractDef.Office.OfficeId), name , "Shipping - UK Contract", queryStructs, attachmentList);
                                if (msg != string.Empty) NoticeHelper.sendDMSAPIUploadError("Shipping Doc Creation", msg, docFiles[i]);
                                /*
                                NoticeHelper.sendDMSUploadDocEmail(OfficeId.getName(contractDef.Office.OfficeId), fi.Name, docFiles[i], 2);
                                */

                                this.moveToFolder(uploadFolder, tsFolderName, "Complete");
                                File.Copy(docFiles[i], uploadFolder + "Complete" + "\\" + tsFolderName + "\\" + fi.Name, true);
                                if (msg == string.Empty) File.Delete(docFiles[i]);
                            }

                            if (isFCR)
                                ShippingWorker.Instance.insertShipmentAttribute(shipmentDef.ShipmentId);
                        }
                    }
                }
                ctx.VoteCommit();
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

        private void replaceDocuments()
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();


                string outputFolder = WebConfig.getValue("appSettings", "DMS_REVISION_UPLOAD_Folder");
                string[] docFiles = System.IO.Directory.GetFiles(outputFolder + "Revised-Doc\\");

                Array.Sort(docFiles);
                ArrayList attachmentList = null;

                string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                for (int i = 0; i < docFiles.Length; i++)
                {
                    File.SetAttributes(docFiles[i], FileAttributes.Normal);
                    FileUtility.clearFolder(outputFolder + "Working", true);
                    System.IO.FileInfo fi = new FileInfo(docFiles[i]);
                    string name = fi.Name.Substring(0, fi.Name.IndexOf('.'));
                    string contractNo = name.Substring(0, name.IndexOf('-'));
                    ContractDef contractDef = OrderSelectWorker.Instance.getContractByContractNo(contractNo);

                    ArrayList queryStructs = new ArrayList();
                    queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Shipping - UK Contract"));
                    queryStructs.Add(new QueryStructDef("UK Contract - Delivery", name));
                    ArrayList qList = DMSUtil.queryDocument(queryStructs);
                    long docId = 0;
                    foreach (DocumentInfoDef docInfoDef in qList)
                    {
                        docId = docInfoDef.DocumentID;
                        attachmentList = new ArrayList();
                        foreach (AttachmentInfoDef attDef in docInfoDef.AttachmentInfos)
                        {
                            byte[] attContent = DMSUtil.getAttachment(attDef.AttachmentID);
                            System.IO.FileStream fs = new System.IO.FileStream(outputFolder + "Working\\" + attDef.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                            fs.Write(attContent, 0, attContent.Length);
                            fs.Close();
                            attachmentList.Add(outputFolder + "Working\\" + attDef.FileName);
                        }
                    }

                    if (File.Exists(outputFolder + "Working\\" + fi.Name))
                        File.Delete(outputFolder + "Working\\" + fi.Name);
                    File.Copy(docFiles[i], outputFolder + "Working\\" + fi.Name);

                    if (docId > 0)
                    {
                        ArrayList queryStructs_ForUpdate = new ArrayList();

                        queryStructs_ForUpdate.Add(new QueryStructDef("UK Contract - Delivery", name));

                        string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs_ForUpdate, attachmentList);

                        this.moveToFolder(outputFolder, tsFolderName, "Complete");
                    }
                    else
                    {
                        this.moveToFolder(outputFolder, tsFolderName, "Failure");
                        if (contractDef != null)
                            NoticeHelper.sendDMSRevisedDocUploadFailureEmail(fi.Name, outputFolder + "Failure\\" + tsFolderName + "\\" + fi.Name, contractDef.Office.OfficeCode, DateTime.Now);
                        else
                            NoticeHelper.sendDMSRevisedDocUploadFailureEmail(fi.Name, outputFolder + "Failure\\" + tsFolderName + "\\" + fi.Name, "N/A", DateTime.Now);
                    }
                    File.Delete(docFiles[i]);
                }

                ctx.VoteCommit();
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

        private bool repairPdf(string sourcePath, string targetPath)
        {
            try
            {
                string processName = "D:\\PACL\\pdftk.exe";
                string s = String.Format(" {0} output {1}", sourcePath, targetPath);
                ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(processName, String.Format(" {0} output {1}", sourcePath, targetPath));
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                /*
                psi.RedirectStandardOutput = true;
                */
                Process proc = new Process();
                proc.StartInfo = psi;
                bool returnVal;
                returnVal = proc.Start();
                /*
                StreamReader sr = proc.StandardOutput;
                string returnMsg = sr.ReadToEnd();
                */
                if (!proc.WaitForExit(60000)) proc.Kill();
                proc.Close();
                /*
                if (returnMsg.Trim() == String.Empty)
                    return true;
                else
                    return false;
                */
                return true;
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorAlert(e, "DEBUG");
                return false;
            }
        }

        private void moveToFolder(string folder, string tsFolderName, string subFolderName)
        {
            string[] docFiles = System.IO.Directory.GetFiles(folder + "Working");
            string path = folder + subFolderName + "\\" + tsFolderName;
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            for (int i = 0; i < docFiles.Length; i++)
            {
                FileInfo fi = new FileInfo(docFiles[i]);
                File.Move(docFiles[i], path + "\\" + fi.Name);
            }
        }

        #endregion
    }
}