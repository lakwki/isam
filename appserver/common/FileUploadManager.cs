using System;
using System.Collections;
using System.Globalization;
using System.IO;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.shipping;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.common;
using com.next.infra.util;
using com.next.infra.persistency.transactions;
using com.next.infra.persistency.dataaccess;
using com.next.common.domain;
using com.next.common.datafactory.worker;
using Shell32;
using System.Collections.Generic;


namespace com.next.isam.appserver.common
{
    public class FileUploadManager
    {
        private static FileUploadManager _instance;
        private DataUploadWorker worker;
        private appserver.helper.TableHelper tableHelper;

        public FileUploadManager()
        {
            worker = DataUploadWorker.Instance;
            tableHelper = appserver.helper.TableHelper.Instance;
        }

        public static FileUploadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FileUploadManager();                    
                }
                return _instance;
            }
        }

        private void releaseObject(object obj)
        {
            DataUploadWorker.Instance.releaseObject(obj);
        }

        #region File/Directory acessing

        public void moveUploadFile(string srcFileServerPath, string destFileServerPath)
        {
            FileInfo destFile = new FileInfo(destFileServerPath);
            FileInfo srcFile = new FileInfo(srcFileServerPath);
            if (destFile != null)
                destFile.Delete();
            srcFile.MoveTo(destFileServerPath);
        }

        public void copyUploadFile(string srcFileServerPath, string destFileServerPath)
        {
            FileInfo destFile = new FileInfo(destFileServerPath);
            FileInfo srcFile = new FileInfo(srcFileServerPath);
            if (destFile != null)
                destFile.Delete();
            srcFile.CopyTo(destFileServerPath);
        }

        public void removeUploadFile(string fileServerPath)
        {
            FileInfo srcFile = new FileInfo(fileServerPath);
            if (srcFile != null)
                srcFile.Delete();
        }

        public ArrayList getServerFile(string fileFullName, int uploadFileTypeId)
        {
            int pos = fileFullName.LastIndexOf('\\');
            string fileName = fileFullName.Substring( pos + 1, 0);
            string serverPath = fileFullName.Substring(0, fileFullName.LastIndexOf('\\'));
            DirectoryInfo di = new DirectoryInfo(serverPath);
            ArrayList fileList = new ArrayList();
            //foreach (FileInfo f in di.GetFiles(searchPattern))
            //{
            //    fileList.Add(f.Name);
            //}
            return fileList;
        }

        public ArrayList getServerFileList(string serverPath, int uploadFileTypeId)
        {
            string searchPattern = worker.getUploadFileNamePattern(uploadFileTypeId);
            ArrayList fileList = new ArrayList();
            DirectoryInfo di = new DirectoryInfo(serverPath);

            //FileInfo[] files = di.GetFiles(searchPattern);
            //files.Sort(new ArrayListHelper.Sorter("Name", false));
            foreach (FileInfo f in di.GetFiles(searchPattern))
            {
                fileList.Add(f.Name);
            }
            //fileList.Sort(new ArrayListHelper.Sorter("FileInfo.Name", false));
            return fileList;
        }

        public string generateServerFileName(string fileName, int uploadFileTypeId, int userId)
        {
            return worker.generateNewUploadFileInfo(fileName, uploadFileTypeId, userId).ServerFileName;
            
        }

        public UploadFileRef getUploadFileInfo(string serverFileName)
        {
            return worker.getUploadFileInfo(serverFileName);
        }

        #endregion

        #region zip tools

        private Shell32.Folder getShell32NameSpace(Object folder)
        {
            // Handle the problem of getting namespace from Shell32 for various version of Windows OS.
            // This will work in all machines irrespective of the Windows OS (XP, 2003, Vista, 7) 
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            Object shell = Activator.CreateInstance(shellAppType);
            return (Shell32.Folder)shellAppType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { folder });
        }

        public FolderItems extractZipFile(string zipFileName)
        {
            Shell32.ShellClass shell = null;
            Shell32.Folder zipFolder = null, DestFolder = null;
            string zipFile = zipFileName.Trim();
            FolderItems extractedFolder = null;
            try
            {
                if (File.Exists(zipFile))
                {
                    string destinationFolder = Path.Combine(Path.GetDirectoryName(zipFile), Path.GetFileNameWithoutExtension(zipFile) + "_ZIP");
                    if (!Directory.Exists(destinationFolder))
                        Directory.CreateDirectory(destinationFolder);

                    shell = new Shell32.ShellClass();
                    zipFolder = getShell32NameSpace(zipFile);
                    DestFolder = getShell32NameSpace(destinationFolder);
                    DestFolder.CopyHere(zipFolder.Items(), 20);
                    extractedFolder = DestFolder.Items();

                    releaseObject(shell);
                    shell = null;
                }
                return extractedFolder;
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, " FileUploadManager - Extracting file " + zipFileName);
                throw e;
            }
            finally
            {
                if (shell != null)
                    releaseObject(shell);
            }
        }

        public List<string> extractFileFromZip(string zipFilePath, string fileTypeName)
        {
            List<string> list = new List<string>();
            FolderItems extractedFolder = extractZipFile(zipFilePath);
            foreach (FolderItem itm in extractedFolder)
                if (itm.IsFolder && itm.Path.Contains(".zip"))
                    list.AddRange(extractFileFromZip(itm.Path, fileTypeName));
                else if (itm.Type.ToLower().Contains(fileTypeName.ToLower()))
                        list.Add(itm.Path);
            return list;
        }

        public FolderItems getFolderItems(string folderName)
        {

            Shell32.ShellClass shell = null;
            try
            {
                shell = new Shell32.ShellClass();
                Shell32.Folder folder = getShell32NameSpace(folderName);
                releaseObject(shell);
                shell = null;
                return folder.Items();
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, " FileUploadManager - GetFolderItems " + folderName);
                throw e;
            }
            finally
            {
                if (shell != null)
                    releaseObject(shell);
            }
        }

        public FolderItem getFolderItem(string folderName, string fileNameWithExtension)
        {
            Shell32.ShellClass shell = null;
            try
            {
                Shell32.FolderItem folderItem = null;
                shell = new Shell32.ShellClass();
                Shell32.Folder folder = getShell32NameSpace(folderName);
                if (folder != null)
                {
                    foreach (FolderItem itm in folder.Items())
                        if (itm.Path.ToLower() == (folderName + "\\" + fileNameWithExtension).ToLower())
                        {
                            folderItem = itm;
                            break;
                        }
                }
                releaseObject(shell);
                shell = null;

                return folderItem;
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, " FileUploadManager - GetFolderItem('" + folderName + "', '" + fileNameWithExtension + "')");
                throw e;
            }
            finally
            {
                if (shell != null)
                    releaseObject(shell);
            }

        }
        
        #endregion

        #region Other Tools
        public string filterSpecialCharacter(string str)
        {
            string outString = string.Empty;
            foreach (char ch in str)
                if (!char.IsControl(ch))
                    switch (Convert.ToInt16(ch))
                    {
                        case 160: // No-Break Space (ASCII 160)-> normal Space (ASCII 32)
                            outString += ' ';
                            break;
                        case 150: // En Dash (ASCII 150)-> normal Dash '-' (ASCII 45)
                        case 8211:// En Dash (HTML Name :&ndash)-> normal Dash '-' (ASCII 45)
                            outString += '-';
                            break;
                        default: 
                            outString += ch;
                            break;
                    }
            return outString;
        }
        #endregion

        #region FileUploadLog

        public ArrayList getFileUploadLogByCriteria(int fileTypeId, string fileName, int userId)
        {
            return worker.getFileUploadLogByCriteria(fileTypeId, fileName, userId);
        }

        public void updateFileUploadLog(FileUploadLogDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                worker.updateFileUploadLog(def);

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }

        }

        #endregion

    }
}