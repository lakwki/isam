using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.infra.persistency.transactions;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using com.next.isam.appserver.claim;
using com.next.isam.domain.claim;
using System.Collections.Generic;

namespace com.next.isam.appserver.startup
{
    public class BlockRefDocHandler : IStartupHandler
    {
        private static readonly string DEFAULT_NAME = "BlockRefDocHandler";

        class TimeState
        {
            public DateTime lastStartTime;
            public DateTime lastRunTime;
        }

        public BlockRefDocHandler()
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
                DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 15)
                return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public string handlerName()
        {
            return BlockRefDocHandler.DEFAULT_NAME;
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
            //this.uploadDocuments();

            List<UKClaimDef> list = UKClaimManager.Instance.getNewlyMappedUKClaimList();
            foreach(UKClaimDef def in list)
            {
                this.uploadUKDebitNote(def);
            }

        }


        private void uploadUKDebitNote(UKClaimDef def)
        {
            string outputFolder = WebConfig.getValue("appSettings", "MFRN_UPLOAD_FOLDER");

            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", "MFRN"));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Debit Note No", def.UKDebitNoteNo));
            queryStructs.Add(new QueryStructDef("Item No", def.ItemNo));
            queryStructs.Add(new QueryStructDef("MFRN Month", def.ClaimMonth.Trim()));

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            long docId = 0;
            foreach (DocumentInfoDef docInfoDef in qList)
                docId = docInfoDef.DocumentID;

            string filename = outputFolder + def.UKDebitNoteNo + ".pdf";

            ArrayList attachmentList = new ArrayList();
            attachmentList.Add(filename);
            string uploadMsg;
            if (docId > 0)
                uploadMsg = DMSUtil.UpdateDocumentWithoutExistingAttachments(docId, queryStructs, attachmentList);
            else
                uploadMsg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", def.UKDebitNoteNo, "UK Claim", queryStructs, attachmentList);
            /*
            FileUtility.clearFolder(outputFolder, false);
            */

            System.Diagnostics.Debug.Print(uploadMsg);
        }


        private void uploadDocuments()
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                string uploadFolder = "\\\\ns-s16\\TH\\blk\\";
                string[] docFiles = System.IO.Directory.GetFiles(uploadFolder);
                Array.Sort(docFiles);
                ArrayList attachmentList = null;
                string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                bool isFirst = true;
                string docTypeId = string.Empty;
                string blkName = string.Empty;

                for (int i = 0; i < docFiles.Length; i++)
                {
                    File.SetAttributes(docFiles[i], FileAttributes.Normal);
                    System.IO.FileInfo fi = new FileInfo(docFiles[i]);
                    blkName = fi.Name.Substring(0, 4);

                    ArrayList queryStructs = new ArrayList();
                    queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Garment Block Reference"));
                    queryStructs.Add(new QueryStructDef("Block Reference", blkName));
                    attachmentList = new ArrayList();

                    ArrayList qList = DMSUtil.queryDocument(queryStructs);
                    long docId = 0;

                    foreach (DocumentInfoDef docInfoDef in qList)
                    {
                        docId = docInfoDef.DocumentID;
                        foreach (AttachmentInfoDef attDef in docInfoDef.AttachmentInfos)
                        {
                            byte[] attContent = DMSUtil.getAttachment(attDef.AttachmentID);
                            System.IO.FileStream fs = new System.IO.FileStream(uploadFolder + "Working\\" + attDef.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                            fs.Write(attContent, 0, attContent.Length);
                            fs.Close();
                            attachmentList.Add(uploadFolder + "Working\\" + attDef.FileName);
                        }
                    }

                    if (docId > 0)
                    {
                        File.Copy(docFiles[i], uploadFolder + "Working\\" + fi.Name.Replace(",", ""), true);
                        attachmentList.Add(uploadFolder + "Working\\" + fi.Name.Replace(",", ""));

                        string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                    }
                    else
                    {
                        File.Copy(docFiles[i], uploadFolder + "Working\\" + fi.Name.Replace(",", ""), true);
                        attachmentList.Add(uploadFolder + "Working\\" + fi.Name.Replace(",", ""));
                        string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\Garment Block Reference\\", blkName, "Garment Block Reference", queryStructs, attachmentList);
                    }

                    File.Move(docFiles[i], uploadFolder + "Processed\\" + fi.Name);

                    FileUtility.clearFolder(uploadFolder + "Working", true);
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
            string processName = "pdftk.exe";
            string s = String.Format(" {0} output {1}", sourcePath, targetPath);
            ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(processName, String.Format(" {0} output {1}", sourcePath, targetPath));
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            //psi.RedirectStandardOutput = true;
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