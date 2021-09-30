using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Data;
using com.next.common.domain;


namespace com.next.isam.appserver.helper
{
    // Help to access the folder & message in Microsoft Exchange Server by EWS (Exchange Web Service)
    public class MailBoxHelper
    {
        private static MailBoxHelper _instance;
        private const int defaultNoOfSearchItem = 100;  //int.MaxValue;
        private const string defaultDomainName = "NEXTDOMAIN";
        private string selectedMailBox = null;
        private ExchangeService _ExchangeWebService = null;


        public static MailBoxHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MailBoxHelper();
                }
                return _instance;
            }
        }


        /*
        private ExchangeService requestService()
        {
            if (exchangeService == null)
            {
                exchangeService = requestService(shipmentBookingAccountName, shipmentBookingAccountPassword, shipmentBookingDomain);
                exchangeService.AutodiscoverUrl(shipmentBookingMailBox);
            }
            return exchangeService;
        }
        */
        #region EWS - Exchange Web Service Functions

        private void TestingService()
        {   // example : access the NSSAdmin mail box
            requestMailBoxService("APP-ISAM", "Work4ever", "nssAdmin@NextSL.com.hk", defaultDomainName);
        }

        public bool requestExchangeService(string AccountName, string password, string domainName)
        {
            //_ExchangeWebService = new ExchangeService(ExchangeVersion.Exchange2013);
            _ExchangeWebService = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
            _ExchangeWebService.Credentials = new WebCredentials(AccountName, password, domainName);
            // Setting credentials is unnecessary when you connect from a computer that is logged on to the domain.
            //service.UseDefaultCredentials = true;
            return (_ExchangeWebService != null);
        }

        public bool requestExchangeService(string AccountName, string password)
        {
            return requestExchangeService(AccountName, password, defaultDomainName);
        }

        public bool selectMailBoxService(string mailBox)
        {
            if (_ExchangeWebService != null)
            {
                _ExchangeWebService.AutodiscoverUrl(mailBox);
                selectedMailBox = mailBox;
                return true;
            }
            else
            {
                selectedMailBox = null;
                return false;
            }
        }

        public bool requestMailBoxService(string AccountName, string password, string mailBox, string domainName)
        {
            if (_ExchangeWebService == null)
                requestExchangeService(AccountName, password, domainName);
            return selectMailBoxService(mailBox);
        }

        public bool requestMailBoxService(string AccountName, string password, string mailBox)
        {
            return requestMailBoxService(AccountName, password, mailBox, defaultDomainName);
        }

        public void loadMailContent(EmailMessage mail)
        {
            mail.Load();
        }

        public List<EmailMessage> getMailHeader(string folderName)
        {
            return getMailHeader(folderName, selectedMailBox, defaultNoOfSearchItem);
        }

        public List<EmailMessage> getMailHeader(string folderName, int numberOfItem)
        {
            return getMailHeader(folderName, selectedMailBox, numberOfItem);
        }

        public List<EmailMessage> getMailHeader(string folderName, string subjectSubstring, string senderEMailSubstring, string receiverEMailSubstring)
        {
            return getMailHeader(folderName, subjectSubstring, senderEMailSubstring, receiverEMailSubstring, selectedMailBox, defaultNoOfSearchItem);
        }

        public List<EmailMessage> getMailHeader(string folderName, string subjectSubstring, string senderEMailSubstring, string receiverEMailSubstring, int numberOfItem)
        {
            return getMailHeader(folderName, subjectSubstring, senderEMailSubstring, receiverEMailSubstring, selectedMailBox, numberOfItem);
        }

        public List<EmailMessage> getMailHeader(string folderName, string subjectSubstring, string senderEMailSubstring, string receiverEMailSubstring, DateTime startTime, DateTime endTime, int hasAttachment, int isRead)
        {
            return searchMailHeader(folderName, subjectSubstring, senderEMailSubstring, receiverEMailSubstring, startTime, endTime, hasAttachment, isRead, selectedMailBox, defaultNoOfSearchItem);
        }

        public List<EmailMessage> getMailHeader(string folderName, string subjectSubstring, string senderEMailSubstring, string receiverEMailSubstring, DateTime startTime, DateTime endTime, int hasAttachment, int isRead, int numberOfItem)
        {
            return searchMailHeader(folderName, subjectSubstring, senderEMailSubstring, receiverEMailSubstring, startTime, endTime, hasAttachment, isRead, selectedMailBox, numberOfItem);
        }


        public List<EmailMessage> getMailHeader(string folderName, string mailBox)
        {
            return getMailHeader(folderName, mailBox, defaultNoOfSearchItem);
        }

        public List<EmailMessage> getMailHeader(string folderName, string mailBox, int numberOfItem)
        {
            return getMailHeader(folderName, GeneralCriteria.ALLSTRING, GeneralCriteria.ALLSTRING, GeneralCriteria.ALLSTRING, DateTime.MinValue, DateTime.MinValue, GeneralCriteria.ALL, GeneralCriteria.ALL, mailBox, numberOfItem);
        }

        public List<EmailMessage> getMailHeader(string folderName, string subjectSubstring, string senderEMailSubstring, string receiverEMailSubstring, string mailBox)
        {
            return getMailHeader(folderName, subjectSubstring, senderEMailSubstring, receiverEMailSubstring, mailBox, defaultNoOfSearchItem);
        }

        public List<EmailMessage> getMailHeader(string folderName, string subjectSubstring, string senderEMailSubstring, string receiverEMailSubstring, string mailBox, int numberOfItem)
        {
            return getMailHeader(folderName, subjectSubstring, senderEMailSubstring, receiverEMailSubstring, DateTime.MinValue, DateTime.MinValue, GeneralCriteria.ALL, GeneralCriteria.ALL, mailBox, numberOfItem);
        }

        public List<EmailMessage> getMailHeader(string folderName, string subjectSubstring, string senderEMailSubstring, string receiverEMailSubstring, DateTime startTime, DateTime endTime, int hasAttachment, int isRead, string mailBox, int numberOfItem)
        {
            return searchMailHeader(folderName, subjectSubstring, senderEMailSubstring, receiverEMailSubstring, startTime, endTime, hasAttachment, isRead, mailBox, numberOfItem);
        }

        public List<EmailMessage> searchMailHeader(string folderName, string subjectSubstring, string senderEMailSubstring, string receiverEMailSubstring, string mailBox)
        {
            return searchMailHeader(folderName, subjectSubstring, senderEMailSubstring, receiverEMailSubstring, DateTime.MinValue, DateTime.MinValue, GeneralCriteria.ALL, GeneralCriteria.ALL, mailBox, defaultNoOfSearchItem);
        }

        public List<EmailMessage> searchMailHeader(string folderName, string subjectSubstring, string senderEMailSubstring, string receiverEMailSubstring, DateTime startTime, DateTime endTime, int hasAttachment, int isRead, string mailBox, int numberOfItem)
        {
            List<EmailMessage> mailMessages = null;
            if (_ExchangeWebService != null)
            {
                mailMessages = new List<EmailMessage>();
                SearchFilter.SearchFilterCollection searchFilterCollection = new SearchFilter.SearchFilterCollection(LogicalOperator.And);
                if (subjectSubstring!=GeneralCriteria.ALLSTRING)
                    searchFilterCollection.Add(new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.ContainsSubstring(EmailMessageSchema.Subject, subjectSubstring)));
                if (senderEMailSubstring!=GeneralCriteria.ALLSTRING)
                    searchFilterCollection.Add(new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.ContainsSubstring(EmailMessageSchema.Sender, senderEMailSubstring)));
                if (receiverEMailSubstring != GeneralCriteria.ALLSTRING)
                    searchFilterCollection.Add(new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.ContainsSubstring(EmailMessageSchema.ToRecipients, receiverEMailSubstring)));
                if (hasAttachment != GeneralCriteria.ALL)
                    searchFilterCollection.Add(new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.HasAttachments, (hasAttachment == GeneralCriteria.TRUE))));
                if (isRead != GeneralCriteria.ALL)
                    searchFilterCollection.Add(new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, (isRead == GeneralCriteria.TRUE))));
                if (startTime != DateTime.MinValue || endTime != DateTime.MinValue)
                {
                    DateTime originalStartTime = startTime;
                    startTime = (startTime == null ? endTime : (startTime > endTime ? endTime : startTime));
                    endTime = (endTime == null ? startTime : (endTime < originalStartTime ? originalStartTime : endTime));
                    searchFilterCollection.Add(new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsGreaterThanOrEqualTo(EmailMessageSchema.DateTimeReceived, startTime)));
                    searchFilterCollection.Add(new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsLessThanOrEqualTo(EmailMessageSchema.DateTimeReceived, endTime)));
                }
                //if (!string.IsNullOrEmpty(bodySubstring))
                //    searchFilterCollection.Add(new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.ContainsSubstring(EmailMessageSchema.Body, bodySubstring)));

                FindItemsResults<Item> searchResults = getMailBoxItem(folderName, (searchFilterCollection.Count > 0 ? searchFilterCollection : null), numberOfItem, mailBox);
                if (searchResults != null)
                    foreach (EmailMessage msg in searchResults.Items)
                        mailMessages.Add(msg);
            }
            return mailMessages;
        }

        public FindItemsResults<Item> getMailBoxItem(string folderName, SearchFilter.SearchFilterCollection filterCollection, int noOfItems)
        {
            return getMailBoxItem(folderName, filterCollection, noOfItems, selectedMailBox);

            ItemView view = new ItemView(noOfItems);
            FolderView fView = new FolderView(noOfItems);
            FindItemsResults<Item> results = null;

            if (_ExchangeWebService != null)
            {
                //ExchangeService service = requestService();
                //_ExchangeWebService.AutodiscoverUrl(mailBox);
                fView.PropertySet = new PropertySet(BasePropertySet.IdOnly);
                fView.PropertySet.Add(FolderSchema.DisplayName);
                fView.Traversal = FolderTraversal.Deep;
                FindFoldersResults found;
                if (string.IsNullOrEmpty(folderName))
                    found = _ExchangeWebService.FindFolders(new FolderId(WellKnownFolderName.Inbox, new Mailbox()), fView);   // get the Inbox content by default
                else
                    found = _ExchangeWebService.FindFolders(new FolderId(WellKnownFolderName.Root, new Mailbox()), new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName.Trim()), fView);
                if (found.Folders.Count > 0)
                    results = (filterCollection == null ? found.Folders[0].FindItems(view) : found.Folders[0].FindItems(filterCollection, view));
            }
            return results;
        }
        
        public FindItemsResults<Item> getMailBoxItem(string folderName, SearchFilter.SearchFilterCollection filterCollection, int noOfItems, string mailBox)
        {
            ItemView view = new ItemView(noOfItems);
            FolderView fView = new FolderView(noOfItems);
            FindItemsResults<Item> results = null;

            if (_ExchangeWebService != null)
            {
                //ExchangeService service = requestService();
                _ExchangeWebService.AutodiscoverUrl(mailBox);
                fView.PropertySet = new PropertySet(BasePropertySet.IdOnly);
                fView.PropertySet.Add(FolderSchema.DisplayName);
                fView.Traversal = FolderTraversal.Deep;
                FindFoldersResults found;
                if (string.IsNullOrEmpty(folderName))
                    found = _ExchangeWebService.FindFolders(new FolderId(WellKnownFolderName.Inbox, new Mailbox(mailBox)), fView);   // get the Inbox content by default
                else
                    found = _ExchangeWebService.FindFolders(new FolderId(WellKnownFolderName.Root, new Mailbox(mailBox)), new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName.Trim()), fView);
                if (found.Folders.Count > 0)
                    results = (filterCollection == null ? found.Folders[0].FindItems(view) : found.Folders[0].FindItems(filterCollection, view));
            }
            return results;
        }
        
        public Folder getMailBoxFolder(string folderName)
        {
            return getMailBoxFolder(folderName, selectedMailBox);
        }

        public Folder getMailBoxFolder(string folderName, string mailBox)
        {
            Folder folder = null;
            if (_ExchangeWebService != null)
            {
                FolderView fView = new FolderView(defaultNoOfSearchItem);
                //ExchangeService service = requestService();
                fView.PropertySet = new PropertySet(BasePropertySet.IdOnly);
                fView.PropertySet.Add(FolderSchema.DisplayName);
                fView.Traversal = FolderTraversal.Deep;
                FindFoldersResults result = _ExchangeWebService.FindFolders(new FolderId(WellKnownFolderName.Root, new Mailbox(mailBox)), new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName), fView);
                folder = (result.Folders.Count > 0 ? result.Folders[0] : null);
            }
            return folder;
        }

        public string extractMailBodyText(EmailMessage eMail)
        {   // call the eMail.Load() function first
            string body = string.Empty;
            if (eMail.Body != null)
            {
                string mailBody = eMail.Body.Text;
                body = mailBody.ToLower();
                int start = -1, end = -1;
                if (body.Contains("<html>") || body.Contains("<html "))
                {
                    end = body.LastIndexOf("</body>") - 1;
                    start = (body.IndexOf("<body>"));
                    if (start < 0)
                        start = body.IndexOf("<body ");
                    if (start > 0 && end > start)
                        start = body.IndexOf(">", start) + 1;
                    else
                        start = -1;
                }
                if (start < 0)
                    body = mailBody;
                else
                    body = mailBody.Substring(start, end - start + 1).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("<o:p></o:p>", string.Empty).Replace("<o:p>&nbsp;</o:p>", string.Empty);
            }
            return body;
        }

        public void moveMessage(List<EmailMessage> messages, string folderName)
        {
            moveMessage(messages, getMailBoxFolder(folderName));
        }

        public void moveMessage(List<EmailMessage> messages, Folder folder)
        {
            try
            {
                //ArrayList list = getMessageInFolder(mailBox, folder, sender);
                //Folder finishedFolder = getMailBoxFolder(service, "Finished");
                if (folder != null)
                    foreach (EmailMessage eMail in messages)
                        eMail.Move(folder.Id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*
        private void replyAll(EmailMessage mail, string message, string toReceipent, string ccReceipent, ArrayList attachmentList)
        {
            // Reply thru Exchange Web Service
            ResponseMessage reply = mail.CreateReply(true);
            EmailMessage draftMsg = reply.Save(WellKnownFolderName.Drafts);

            //draftMsg.Attachments.Clear();
            //foreach (Shell32.FolderItem item in excelFileList)
            foreach (string filePath in attachmentList)
            {
                draftMsg.Attachments.AddFileAttachment(filePath);
                //draftMsg.Attachments.AddFileAttachment(item.Path);
                //draftMsg.Update(ConflictResolutionMode.AlwaysOverwrite);
            }
            draftMsg.ToRecipients.Add(toReceipent);
            draftMsg.CcRecipients.Add(ccReceipent);
            draftMsg.From = new EmailAddress("NSS Admin", "nssadmin@nextsl.com.hk", "SMTP");    // Sending on Behalf of NSS Admin
            draftMsg.Update(ConflictResolutionMode.AlwaysOverwrite);
            draftMsg.Send();
            //reply.SendAndSaveCopy();
            //draftMsg.Delete(DeleteMode.MoveToDeletedItems);
        }
        */ 
 
        #endregion
 
    }
}