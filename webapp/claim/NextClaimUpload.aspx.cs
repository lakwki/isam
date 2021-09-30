using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.appserver.claim;
using com.next.isam.appserver.shipping;
using System.Web.UI.WebControls;
using com.next.isam.domain.claim;
using com.next.common.domain.types;
using com.next.infra.util;
using com.next.common.domain.dms;
using System.Web.UI;
using com.next.isam.domain.order;
using com.next.common.domain.module;
using com.next.isam.dataserver.worker;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Linq;
using System.Globalization;
using com.next.common.datafactory.worker;
using System.Text;
using System.Text.RegularExpressions;

namespace com.next.isam.webapp.claim
{
    public partial class NextClaimUpload : com.next.isam.webapp.usercontrol.PageTemplate
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            if (!Page.IsPostBack)
            {
                txt_DebitNoteDate.DateTime = DateTime.Now;
                this.ddl_PaymentOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--- Please select ---", GeneralCriteria.ALL.ToString());
            }
        }


        protected void gv_NextClaim_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string targetFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UKClaimDef def = (UKClaimDef)this.vwPdfList[e.Row.RowIndex];

                if (def.Type != UKClaimType.MFRN)
                    ((Label)e.Row.FindControl("lbl_ClaimType")).Text = def.Type.DMSDescription;
                else
                    ((Label)e.Row.FindControl("lbl_ClaimType")).Text = def.Type.DMSDescription + " (" + def.ClaimMonth + ")";

                ((Label)e.Row.FindControl("lbl_PageNo")).Text = def.StartPageForUpload.ToString();
                ((Label)e.Row.FindControl("lbl_DNNo")).Text = def.UKDebitNoteNo;
                ((Label)e.Row.FindControl("lbl_DNDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteDate);
                ((Label)e.Row.FindControl("lbl_Currency")).Text = def.Currency.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = def.ItemNo + (def.ContractNo.Length > 0 ? (" / " + def.ContractNo) : string.Empty);
                ((Label)e.Row.FindControl("lbl_Qty")).Text = def.Quantity.ToString("#,##0");
                ((Label)e.Row.FindControl("lbl_Amount")).Text = def.Amount.ToString("#,##0.00");
                ((Label)e.Row.FindControl("lbl_Note")).ForeColor = System.Drawing.Color.Red;
                ((Label)e.Row.FindControl("lbl_Note")).Font.Bold = true;
                if (def.Amount == 0)
                {
                    ((Label)e.Row.FindControl("lbl_Note")).Text = "Invalid Amount";
                    ((CheckBox)e.Row.FindControl("chb_check")).Checked = false;
                    
                }
                if (def.Amount < 0)
                {
                    ((Label)e.Row.FindControl("lbl_Note")).Text = "Refund Not Supported";
                    ((CheckBox)e.Row.FindControl("chb_check")).Checked = false;
                    ((CheckBox)e.Row.FindControl("chb_check")).Enabled = false;
                }

                /*
                if (File.Exists(targetFolder + def.UKDebitNoteNo + ".pdf"))
                    ((Label)e.Row.FindControl("lbl_Attachment")).Text = "Yes";
                else
                    ((Label)e.Row.FindControl("lbl_Attachment")).Text = "No";
                */

                ArrayList shipmentList = ShipmentManager.Instance.GetShipmentProductByItemNo(def.ItemNo, def.ContractNo, -1);
                int idxTK = -1;
                int idx = 0;

                if (shipmentList.Count > 0)
                {
                    foreach (ShipmentProductDef spDef in shipmentList)
                    {
                        if (spDef.OfficeId == OfficeId.TR.Id)
                        {
                            idxTK = idx;
                            break;
                        }
                        idx++;
                    }

                    ShipmentProductDef shipment = ((ShipmentProductDef)shipmentList[(idxTK != -1 ? idxTK : 0)]);

                    def.Vendor = IndustryUtil.getVendorByKey(shipment.VendorId);
                    ((Label)e.Row.FindControl("lbl_Vendor")).Text = def.Vendor.Name;

                    if (shipment.OfficeId == OfficeId.DG.Id && shipment.HandlingOfficeId == OfficeId.HK.Id)
                    {
                        shipment.OfficeId = OfficeId.HK.Id;
                        shipment.HandlingOfficeId = OfficeId.DG.Id;
                    }
                    if (shipment.OfficeId == OfficeId.DG.Id && shipment.HandlingOfficeId == OfficeId.SH.Id)
                    {
                        shipment.OfficeId = OfficeId.SH.Id;
                        shipment.HandlingOfficeId = OfficeId.DG.Id;
                    }
                    if (shipment.OfficeId == OfficeId.DG.Id && shipment.HandlingOfficeId == OfficeId.VN.Id)
                    {
                        shipment.OfficeId = OfficeId.VN.Id;
                        shipment.HandlingOfficeId = OfficeId.VN.Id;
                    }
                    if (shipment.HandlingOfficeId == 41)
                    {
                        shipment.HandlingOfficeId = shipment.OfficeId;
                    }

                    def.OfficeId = shipment.OfficeId;
                    def.HandlingOfficeId = shipment.HandlingOfficeId;
                    def.ProductTeamId = shipment.ProductTeamId;
                    def.TermOfPurchaseId = shipment.TermOfPurchaseId;

                    ((Label)e.Row.FindControl("lbl_Office")).Text = OfficeId.getName(shipment.OfficeId);
                    ((Label)e.Row.FindControl("lbl_HandlingOffice")).Text = OfficeId.getName(shipment.HandlingOfficeId);
                    ((Label)e.Row.FindControl("lbl_ProductTeam")).Text = shipment.ProductTeamCode;

                    List<UKClaimDef> ukClaimList = UKClaimManager.Instance.getUKClaimListByTypeMapping(-1, def.Type.Id, def.Vendor.VendorId, def.ItemNo, def.UKDebitNoteNo, def.Quantity);
                    if (ukClaimList.Count > 0)
                    {
                        if (def.Amount > 0)
                        {
                            ((Label)e.Row.FindControl("lbl_Note")).Text = "Record already exists";
                            ((CheckBox)e.Row.FindControl("chb_check")).Checked = false;
                            ((CheckBox)e.Row.FindControl("chb_check")).Enabled = false;
                        }
                    }
                }
                else
                {
                    if (def.OfficeId == 0 || def.OfficeId == -1 || def.Vendor == null || def.ItemNo.Trim() == string.Empty || def.Quantity == 0)
                    {
                        ((Label)e.Row.FindControl("lbl_Note")).Text = "Incomplete Info";
                        ((CheckBox)e.Row.FindControl("chb_check")).Checked = false;
                        ((CheckBox)e.Row.FindControl("chb_check")).Enabled = false;
                    }
                }

            }

        }

        ArrayList vwPdfList
        {
            get { return (ArrayList)ViewState["PdfList"]; }
            set { ViewState["PdfList"] = value; }
        }

        string vwPdfPath
        {
            get { return (string)ViewState["PdfPath"]; }
            set { ViewState["PdfPath"] = value; }
        }

        private string uploadUKDebitNote(UKClaimDef def)
        {
            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", def.Type.DMSDescription));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Debit Note No", def.UKDebitNoteNo));
            queryStructs.Add(new QueryStructDef("Item No", def.ItemNo));
            queryStructs.Add(new QueryStructDef("MFRN Month", def.ClaimMonth));
            queryStructs.Add(new QueryStructDef("Supplier Name", def.Vendor.Name));
            queryStructs.Add(new QueryStructDef("Qty", def.Quantity.ToString()));

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            long docId = 0;
            foreach (DocumentInfoDef docInfoDef in qList)
                docId = docInfoDef.DocumentID;

            this.splitPdf2(this.vwPdfPath, hid_DatetimePath.Value, def);
            string filename = hid_DatetimePath.Value + def.UKDebitNoteNo + ".pdf";

            ArrayList attachmentList = new ArrayList();
            attachmentList.Add(filename);
            string uploadMsg;
            if (docId > 0)
                uploadMsg = DMSUtil.UpdateDocumentWithoutExistingAttachments(docId, queryStructs, attachmentList);
                //uploadMsg = "update existing attachment";
            else
                uploadMsg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", def.UKDebitNoteNo, "UK Claim", queryStructs, attachmentList);

            if (uploadMsg != string.Empty)
                NoticeHelper.sendGeneralMessage("Next Claim Upload - DMS Upload Failure", def.UKDebitNoteNo + "@" + DateTime.Now.ToString());

            return (uploadMsg == string.Empty ? def.UKDebitNoteNo + ".pdf" : string.Empty);
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            List<UKClaimDef> discrepancyList = new List<UKClaimDef>();
            for (int i = 0; i < this.vwPdfList.Count; i++)
            {
                if (((CheckBox)gv_NextClaim.Rows[i].FindControl("chb_check")).Checked == true)
                {
                    UKClaimDef def = (UKClaimDef)vwPdfList[i];
                    def.GUId = Guid.NewGuid().ToString();

                    QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

                    List<QAIS.ClaimRequestDef> list = new List<QAIS.ClaimRequestDef>();
                    object[] activeList = svc.GetActiveClaimRequestListByType(-1, 3, def.Vendor.VendorId, def.ItemNo, def.DebitNoteDate.ToString("yyyyMM"));

                    if (activeList.Length == 1)
                    {
                        foreach (object o in activeList)
                            if (o.GetType() == typeof(QAIS.ClaimRequestDef))
                            {
                                if (((((QAIS.ClaimRequestDef)o).IsAuthorized) || (3 == UKClaimType.MFRN.Id && ((QAIS.ClaimRequestDef)o).IsAuthorized)) && UKClaimManager.Instance.getUKClaimByClaimRequestId(((QAIS.ClaimRequestDef)o).RequestId) == null)
                                    def.ClaimRequestId = ((QAIS.ClaimRequestDef)o).RequestId;
                            }
                    }

                    bool isSubmitted = false;
                    if (def.WorkflowStatus.Id == ClaimWFS.NEW.Id && def.ClaimRequestId != -1)
                    {
                        QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(def.ClaimRequestId);
                        if (requestDef.WorkflowStatusId == 8)
                        {
                            def.WorkflowStatus = ClaimWFS.USER_SIGNED_OFF;
                            isSubmitted = true;
                        }
                        else if ((requestDef.WorkflowStatusId == 10 ||
                                  ((requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7) && requestDef.IsAuthorized) ||
                                  ((requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7) && requestDef.IsAuthorized))
                            && requestDef.VendorRechargePercent == 100)
                        {
                            if (requestDef.ItemNo == def.ItemNo && requestDef.Vendor.VendorId == def.Vendor.VendorId)
                            {
                                if (requestDef.WorkflowStatusId == 10)
                                    svc.sendCancelledClaimNotification(requestDef.RequestId, this.LogonUserId);

                                def.WorkflowStatus = ClaimWFS.SUBMITTED;
                                isSubmitted = true;
                                if (requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7)
                                {
                                    if (def.Remark == string.Empty)
                                        def.Remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier";
                                    else
                                        def.Remark += ("\n" + "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier");
                                    def.IsReadyForSettlement = true;
                                }
                                if (requestDef.WorkflowStatusId == 10)
                                    def.IsReadyForSettlement = false;
                            }
                        }
                    }

                    if (isSubmitted)
                    {
                        QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(def.ClaimRequestId);
                        if (def.ClaimRequestId != -1 && requestDef.WorkflowStatusId != 10)
                            svc.SetClaimRequestStatus(def.ClaimRequestId, 9, this.LogonUserId);
                    }

                    UKClaimManager.Instance.updateUKClaimDef(def, this.LogonUserId);

                    if (def.OfficeId != def.PaymentOfficeId)
                        discrepancyList.Add(def);

                    string fileUploaded = uploadUKDebitNote(def);

                    if (isSubmitted)
                        this.updateDMSIndexFields(def);
                }

            }
            if (discrepancyList.Count > 0)
                NoticeHelper.sendUKClaimMFRNUploadAlertEmail(discrepancyList);

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Next_Claim_Upload", "alert('Upload Completed')", true);
        }

        private void updateDMSIndexFields(UKClaimDef ukClaim)
        {
            long ukDNDocId = 0;
            long authFormId = 0;
            ArrayList updatedQueryStructs = new ArrayList();
            updatedQueryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            updatedQueryStructs.Add(new QueryStructDef("Item No", ukClaim.ItemNo));
            updatedQueryStructs.Add(new QueryStructDef("Debit Note No", ukClaim.UKDebitNoteNo));

            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", ukClaim.Type.DMSDescription));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Item No", ukClaim.ItemNo));
            queryStructs.Add(new QueryStructDef("Debit Note No", ukClaim.UKDebitNoteNo));
            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            foreach (DocumentInfoDef docInfoDef in qList)
                ukDNDocId = docInfoDef.DocumentID;

            queryStructs.Clear();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
            QAIS.ClaimRequestDef req = svc.GetClaimRequestByKey(ukClaim.ClaimRequestId);

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Authorization Form"));
            queryStructs.Add(new QueryStructDef("Item No", req.ItemNo));
            queryStructs.Add(new QueryStructDef("Claim Type", ukClaim.Type.DMSDescription));
            if (req.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                queryStructs.Add(new QueryStructDef("MFRN Month", req.ClaimMonth));
            else
                queryStructs.Add(new QueryStructDef("Form No", req.FormNo));
            qList = DMSUtil.queryDocument(queryStructs);

            foreach (DocumentInfoDef docInfoDef in qList)
            {
                authFormId = docInfoDef.DocumentID;
                foreach (FieldInfoDef fiDef in docInfoDef.FieldInfos)
                {
                    if (fiDef.FieldName != "Supporting Doc Type" && fiDef.FieldName != "Item No" && fiDef.FieldName != "Debit Note No" && fiDef.FieldName != "Qty")
                        updatedQueryStructs.Add(new QueryStructDef(fiDef.FieldName, fiDef.Content));
                }
                break;
            }

            if (authFormId != 0 && ukDNDocId != 0)
            {
                string strReturn = DMSUtil.UpdateDocument(ukDNDocId, updatedQueryStructs);
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (txt_DebitNoteDate.DateTime == DateTime.MinValue)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DCNoteDate", "alert('Please enter the received date');", true);
                return;
            }
            if (ddl_PaymentOffice.selectedValueToInt == GeneralCriteria.ALL)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "PaymentOffice", "alert('Please select Payment Office');", true);
                return;
            }
            if (uplPdfFile.HasFile)
            {
                string targetFolder = WebConfig.getValue("appSettings", "MFRN_UPLOAD_FOLDER");
                string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                hid_DatetimePath.Value = targetFolder + tsFolderName + "\\";
                Directory.CreateDirectory(hid_DatetimePath.Value);
                this.vwPdfPath = hid_DatetimePath.Value + uplPdfFile.FileName;

                uplPdfFile.SaveAs(this.vwPdfPath);

                PdfReader reader = new PdfReader(this.vwPdfPath);
                int noOfPages = reader.NumberOfPages;

                getClaimList(this.vwPdfPath, hid_DatetimePath.Value);

                if (this.vwPdfList.Count > 0)
                    btnConfirm.Visible = true;

                gv_NextClaim.DataSource = this.vwPdfList;
                gv_NextClaim.DataBind();

                this.lbl_FileSummary.Visible = true;
                this.lbl_FileSummary.Text = String.Format("File Summary : Total Num Of Page : {0}, System Identified : {1} Claim(s)", reader.NumberOfPages.ToString(), vwPdfList.Count.ToString());
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Upload file", "alert('Please upload pdf file');", true);
                return;
            }
        }

        private void getClaimList(string pdfPath, string outputPath)
        {
            try
            {
                this.vwPdfList = new ArrayList();
                PdfReader reader = new PdfReader(pdfPath);

                string[] monthList = new string[12];
                for (int i = 1; i <= 12; i++)
                {
                    DateTimeFormatInfo mfi = new DateTimeFormatInfo();
                    monthList[i - 1] = mfi.GetMonthName(i).ToString();
                }

                ArrayList list = new ArrayList();

                UKClaimDef def = null;
                string dcNoteNo = string.Empty;
                string dcNoteNoPrefix = string.Empty;
                StringBuilder fullSb = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    if (this.isDNStartingPage(sb))
                    {
                        fullSb = sb;
                        def = new UKClaimDef();
                        def.Quantity = 0;
                        dcNoteNo = string.Empty;
                        dcNoteNoPrefix = getDCNoteNoPrefix(sb, out dcNoteNo);
                        def.StartPageForUpload = i;
                        def.NumOfPageForUpload = 1;
                    }
                    else
                    {
                        def.NumOfPageForUpload += 1;
                        fullSb = fullSb.Append(sb.ToString());
                    }

                    using (StringReader sr = new StringReader(sb.ToString()))
                    {
                        string line;
                        int lineNo = 1;
                        int tableHeaderLineNo = -1;
                        int totalHeaderLineNo = -1;
                        int totalHeaderPageNo = -1;
                        int dnDateLineNo = -1;
                        DateTime dnDate = DateTime.MinValue;

                        def.UKDebitNoteNo = dcNoteNo;
                        def.PaymentOfficeId = ddl_PaymentOffice.selectedValueToInt;
                        def.UKDebitNoteReceivedDate = txt_DebitNoteDate.DateTime;

                        lineNo = 1;
                        while ((line = sr.ReadLine()) != null)
                        {
                            System.Diagnostics.Debug.WriteLine("Page#" + lineNo.ToString() + " : " + line.Trim());
                            if (line.IndexOf("Date:") != -1)
                            {
                                dnDateLineNo = lineNo;
                                dnDate = getDNDate(line);
                                if (dnDate != DateTime.MinValue)
                                    def.UKDebitNoteDate = dnDate;
                            }
                            if (line.Trim().StartsWith("Contract"))
                            {
                                MatchCollection matches = null;
                                matches = Regex.Matches(line.Trim(), @"\w{2}\d{7}");
                                foreach (Match v in matches)
                                    def.ContractNo = v.ToString();
                            }

                            if (dnDate == DateTime.MinValue && lineNo == dnDateLineNo + 1)
                            {
                                dnDate = getDNDate(line);
                                def.UKDebitNoteDate = dnDate;
                            }

                            switch (dcNoteNoPrefix)
                            {
                                case "DN":
                                    def.Type = UKClaimType.MFRN;

                                    if (line.IndexOf("Debit relates to returns for the") != -1)
                                    {
                                        string s = "Debit relates to returns for the";
                                        if (line.IndexOf("Debit relates to returns for the month of") != -1)
                                            s = "Debit relates to returns for the month of";

                                        string[] tmp = line.Replace(s, string.Empty).Trim().Split(' ');
                                        string claimMonth = tmp[1] + (Array.IndexOf(monthList, tmp[0]) + 1).ToString().PadLeft(2, '0');
                                        def.ClaimMonth = claimMonth;
                                    }

                                    if (line.IndexOf("Item ID Group Unit Price Qty Description Net VAT") != -1)
                                        tableHeaderLineNo = lineNo;

                                    if (lineNo == tableHeaderLineNo + 1 && line.Trim() == string.Empty)
                                        tableHeaderLineNo = lineNo;

                                    if (lineNo == tableHeaderLineNo + 1)
                                    {
                                        def.ItemNo = line.Substring(0, 6).Trim();
                                    }

                                    /*
                                    if (line.IndexOf("Total Quantity") != -1)
                                    {
                                        int idx = line.IndexOf("Total Quantity");
                                        if (idx >= 0)
                                        {
                                            def.Quantity = int.Parse(line.Substring(idx + 15).Trim());
                                            break;
                                        }
                                    }
                                    */
                                    if (line.IndexOf("Net Total VAT Total Gross Total") != -1)
                                    {
                                        totalHeaderLineNo = lineNo;
                                        totalHeaderPageNo = def.NumOfPageForUpload;
                                    }

                                    if (lineNo == totalHeaderLineNo + 1)
                                    {
                                        if (!assignClaimSummary(fullSb, line, def))
                                            totalHeaderLineNo++;
                                    }
                                    break;

                                case "F":
                                    if (line.IndexOf("Item ID") != -1)
                                        tableHeaderLineNo = lineNo;

                                    if (lineNo == tableHeaderLineNo + 1 && line.Trim() == string.Empty)
                                        tableHeaderLineNo = lineNo;

                                    if (lineNo == tableHeaderLineNo + 1)
                                    {
                                        if (line.IndexOf("SAFETY ISSUE") != -1)
                                        {
                                            int idx = line.IndexOf("SAFETY ISSUE");
                                            if (idx >= 0)
                                            {
                                                string[] text = line.Replace(" SAFETY ISSUE", "|").Split('|');
                                                text = text[0].Split(' ');
                                                def.ItemNo = text[0].Trim();
                                                def.Quantity = int.Parse(text[text.Length - 1].Trim());
                                                def.Type = UKClaimType.getClaimType(dcNoteNoPrefix, "Safety Issue");
                                                break;
                                            }
                                        }
                                        else if (line.IndexOf("METAL CONTAMINATION") != -1)
                                        {
                                            int idx = line.IndexOf("METAL CONTAMINATION");
                                            if (idx >= 0)
                                            {
                                                string[] text = line.Replace(" METAL CONTAMINATION", "|").Split('|');
                                                text = text[0].Split(' ');
                                                def.ItemNo = text[0].Trim();
                                                def.Quantity = int.Parse(text[text.Length - 1].Trim());
                                                def.Type = UKClaimType.getClaimType(dcNoteNoPrefix, "Metal Contamination");
                                                break;
                                            }
                                        }
                                        else if (line.IndexOf("QC FAIL") != -1)
                                        {
                                            int idx = line.IndexOf("QC FAIL");
                                            if (idx >= 0)
                                            {
                                                string[] text = line.Replace(" QC FAIL", "|").Split('|');
                                                text = text[0].Split(' ');
                                                def.ItemNo = text[0].Trim();
                                                def.Quantity = int.Parse(text[text.Length - 1].Trim());
                                                def.Type = UKClaimType.getClaimType(dcNoteNoPrefix, "QC Fail");
                                                break;
                                            }
                                        }
                                        else if (line.IndexOf("FINE (PERCENTAGE)") != -1)
                                        {
                                            int idx = line.IndexOf("FINE (PERCENTAGE)");
                                            if (idx >= 0)
                                            {
                                                string[] text = line.Replace(" FINE (PERCENTAGE)", "|").Split('|');
                                                text = text[0].Split(' ');
                                                def.ItemNo = text[0].Trim();
                                                def.Quantity = int.Parse(text[text.Length - 1].Trim());
                                                def.Type = UKClaimType.getClaimType(dcNoteNoPrefix, "Fine (Percentage)");
                                                break;
                                            }
                                        }
                                        else
                                            tableHeaderLineNo++;
                                    }

                                    if (line.IndexOf("Net Total VAT Total Gross Total") != -1)
                                    {
                                        totalHeaderLineNo = lineNo;
                                        totalHeaderPageNo = def.NumOfPageForUpload;
                                    }

                                    if (lineNo == totalHeaderLineNo + 1)
                                    {
                                        if (!assignClaimSummary(fullSb, line, def))
                                            totalHeaderLineNo++;
                                    }
                                    break;

                                case "FRA":
                                    if (line.IndexOf("Item ID") != -1)
                                        tableHeaderLineNo = lineNo;

                                    if (lineNo == tableHeaderLineNo + 1 && line.Trim() == string.Empty)
                                        tableHeaderLineNo = lineNo;

                                    if (lineNo == tableHeaderLineNo + 1)
                                    {
                                        if (line.ToUpper().IndexOf("FIRA CHARGE") != -1 || line.ToUpper().IndexOf("NET GOODS VALUE") != -1 || line.ToUpper().IndexOf("DELIVERY CHARGE") != -1)
                                        {
                                            int idx = line.ToUpper().IndexOf("FIRA CHARGE");
                                            if (idx < 0) idx = line.ToUpper().IndexOf("NET GOODS VALUE");
                                            if (idx < 0) idx = line.ToUpper().IndexOf("DELIVERY CHARGE");

                                            if (idx >= 0)
                                            {
                                                string s = line.ToUpper();
                                                s = s.Replace(" FIRA CHARGE", "|");
                                                s = s.Replace(" NET GOODS VALUE", "|");
                                                s = s.Replace(" DELIVERY CHARGE", "|");
                                                string[] text = s.Split('|');
                                                text = text[0].Split(' ');
                                                def.ItemNo = text[0].Trim();
                                                def.Quantity = int.Parse(text[text.Length - 1].Trim());
                                                def.Type = UKClaimType.getClaimType(dcNoteNoPrefix, "FIRA");
                                                break;
                                            }
                                        }
                                        else
                                            tableHeaderLineNo++;
                                    }

                                    if (line.IndexOf("Qty Total Net Total VAT Total Gross Total") != -1)
                                    {
                                        totalHeaderLineNo = lineNo;
                                        totalHeaderPageNo = def.NumOfPageForUpload;
                                    }

                                    if (lineNo == totalHeaderLineNo + 1)
                                    {
                                        if (!assignClaimSummary(fullSb, line, def))
                                            totalHeaderLineNo++;
                                    }
                                    break;

                                case "CFS":
                                    if (line.IndexOf("Item ID") != -1)
                                    {
                                        tableHeaderLineNo = lineNo;
                                        def.Quantity = 1;

                                    }
                                    if (lineNo == tableHeaderLineNo + 1 && line.Trim() == string.Empty)
                                        tableHeaderLineNo = lineNo;

                                    if (lineNo == tableHeaderLineNo + 1)
                                    {
                                        if (line.IndexOf("Supplier Compliance") != -1)
                                        {
                                            int idx = line.IndexOf("Supplier Compliance");
                                            if (idx >= 0)
                                            {
                                                string[] text = line.Split(' ');
                                                def.ItemNo = text[0].Trim();
                                                def.Type = UKClaimType.getClaimType(dcNoteNoPrefix, "Supplier Compliance");
                                                break;
                                            }
                                        }
                                        else
                                            tableHeaderLineNo++;
                                    }

                                    if (line.IndexOf("Net Total VAT Total Gross Total") != -1)
                                    {
                                        totalHeaderLineNo = lineNo;
                                        totalHeaderPageNo = def.NumOfPageForUpload;
                                    }

                                    if (lineNo == totalHeaderLineNo + 1)
                                    {
                                        if (!assignClaimSummary(fullSb, line, def))
                                            totalHeaderLineNo++;
                                    }
                                    break;

                                case "DFS":
                                case "PFS":
                                case "RFS":
                                    if (line.IndexOf("Item ID") != -1)
                                        tableHeaderLineNo = lineNo;

                                    if (lineNo == tableHeaderLineNo + 1)
                                    {
                                        string pattern = @"\(?\d+\.\d{2}\)?\s*\(?\d+\.\d{2}\)?"; // Net Value & VAT
                                        var matches = Regex.Matches(line, pattern);
                                        bool isItemLine = false;
                                        foreach (Match s in matches)
                                            isItemLine = true;

                                        if (line.Trim() == string.Empty || !isItemLine)
                                            tableHeaderLineNo = lineNo;
                                    }

                                    if (lineNo == tableHeaderLineNo + 1)
                                    {
                                        if (line.IndexOf("REJECTED") != -1 || line.IndexOf("STOCK") != -1 || line.IndexOf("DISPOSAL") != -1)
                                        {
                                            int idx = line.IndexOf("REJECTED STOCK");
                                            if (idx >= 0)
                                            {
                                                string[] text = line.Split(' ');
                                                def.ItemNo = text[0].Substring(0, 6).Trim();
                                                def.Type = UKClaimType.getClaimType(dcNoteNoPrefix, "Rejected Stock");
                                                break;
                                            }
                                            else if (line.IndexOf("DISPOSAL") >= 0)
                                            {
                                                string[] text = line.Split(' ');
                                                def.ItemNo = text[0].Substring(0, 6).Trim();
                                                def.Type = UKClaimType.getClaimType(dcNoteNoPrefix, "Rework");
                                                break;
                                            }
                                            else
                                            {
                                                idx = line.IndexOf("REJECTED");
                                                if (idx >= 0)
                                                {
                                                    tableHeaderLineNo = lineNo;
                                                }

                                                idx = line.IndexOf("STOCK");
                                                if (idx >= 0)
                                                {
                                                    string[] text = line.Split(' ');
                                                    def.ItemNo = text[0].Substring(0, 6).Trim();
                                                    def.Type = UKClaimType.getClaimType(dcNoteNoPrefix, "Rejected Stock");
                                                    break;
                                                }

                                            }
                                        }
                                        else if (line.IndexOf("REWORK") != -1)
                                        {
                                            int idx = line.IndexOf("REWORK");
                                            if (idx >= 0)
                                            {
                                                string[] text = line.Split(' ');
                                                def.ItemNo = text[0].Substring(0, 6).Trim();
                                                def.Type = UKClaimType.getClaimType(dcNoteNoPrefix, "Rework");
                                                break;
                                            }
                                        }
                                        else
                                            tableHeaderLineNo++;
                                    }

                                    if (line.IndexOf("Qty Total Net Total VAT Total Gross Total") != -1)
                                    {
                                        totalHeaderLineNo = lineNo;
                                        totalHeaderPageNo = def.NumOfPageForUpload;
                                    }

                                    if ((lineNo == totalHeaderLineNo + 1 && def.NumOfPageForUpload == totalHeaderPageNo) || totalHeaderPageNo != def.NumOfPageForUpload)
                                    {
                                        if (!assignClaimSummary(fullSb, line, def))
                                           totalHeaderLineNo++;
                                    }
                                    break;
                            }
                            lineNo++;
                        }
                    }
                    if (isDNEndingPage(sb, def.Type))
                        list.Add(def);
                }
                this.vwPdfList = list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool assignClaimSummary(StringBuilder sb, string line, UKClaimDef claimDef)
        {
            string pattern = string.Empty;
            MatchCollection matches;

            if (claimDef.Type == UKClaimType.MFRN)
            {
                string qtyString = string.Empty;
                pattern = @"Total Quantity\s+\d+";
                matches = Regex.Matches(sb.ToString(), pattern);
                foreach (Match s in matches)
                    qtyString = s.ToString();
                pattern = @"\d+";
                matches = Regex.Matches(qtyString, pattern);
                foreach (Match s in matches)
                    claimDef.Quantity = int.Parse(s.ToString());

                if (claimDef.Quantity == 0)
                {
                    pattern = @"Total +\d*\s*Quantity";
                    matches = Regex.Matches(sb.ToString(), pattern);
                    foreach (Match s in matches)
                        qtyString = s.ToString();
                    pattern = @"\d+";
                    matches = Regex.Matches(qtyString, pattern);
                    foreach (Match s in matches)
                        claimDef.Quantity = int.Parse(s.ToString());
                }
            }

            int noOfFields = 3;
            pattern = @"\(?\d+[,]?\d*\.?\d*\)? +\(?\d+[,]?\d*\.?\d*\)? +\(?\d+[,]?\d*\.?\d*\)? +(USD|GBP|EUR)";
            if (claimDef.Type == UKClaimType.REJECT || claimDef.Type == UKClaimType.REWORK || claimDef.Type == UKClaimType.FIRA_TEST)
            {
                noOfFields = 4;
                pattern = @"\d+[,]?\d*\.?\d* +\(?\d+[,]?\d*\.?\d*\)? +\(?\d+[,]?\d*\.?\d*\)? +\(?\d+[,]?\d*\.?\d*\)? +(USD|GBP|EUR)";
            }

            string ttlString = string.Empty;
            matches = Regex.Matches(line, pattern);
            foreach (Match s in matches)
                ttlString = s.ToString();

            if (ttlString != string.Empty)
            {
                string[] split = ttlString.Split(' ');
                if (noOfFields == 4)
                    claimDef.Quantity = int.Parse(split[0].Trim());
                claimDef.Amount = decimal.Parse(split[noOfFields - 1], NumberStyles.AllowParentheses | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);
                claimDef.Amount *= -1;
                claimDef.Currency = GeneralWorker.Instance.getCurrencyByCurrencyCode(split[noOfFields]);
                return true;
            }
            else
                return false;
        }


        private bool isDNStartingPage(StringBuilder sb)
        {
            bool b = false;
            using (StringReader sr = new StringReader(sb.ToString()))
            {
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.IndexOf("Voucher") != -1 || line.IndexOf("Debit Note No:") != -1)
                    {
                        b = true;
                        break;
                    }
                }
            }
            return b;
        }

        private bool isDNEndingPage(StringBuilder sb, UKClaimType claimType)
        {
            bool b = false;
            int tableLineNo = -1;
            int lineNo = 1;
            using (StringReader sr = new StringReader(sb.ToString()))
            {
                string line = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    System.Diagnostics.Debug.WriteLine(line);
                    if (line.IndexOf("Net Total VAT Total Gross Total") != -1)
                        tableLineNo = lineNo;
                    if (tableLineNo != -1 && lineNo > tableLineNo && lineNo <= tableLineNo + 5 && line.Trim() != string.Empty)
                    {
                        string pattern = @"\(?\d+[,]?\d*\.?\d*\)? +\(?\d+[,]?\d*\.?\d*\)? +\(?\d+[,]?\d*\.?\d*\)? +(USD|GBP|EUR)";
                        if (claimType == UKClaimType.REWORK || claimType == UKClaimType.REJECT || claimType == UKClaimType.FIRA_TEST)
                            pattern = @"\d+[,]?\d*\.?\d* +\(?\d+[,]?\d*\.?\d*\)? +\(?\d+[,]?\d*\.?\d*\)? +\(?\d+[,]?\d*\.?\d*\)? +(USD|GBP|EUR)";
                        string ttlString = string.Empty;
                        var matches = Regex.Matches(line, pattern);
                        foreach (Match s in matches)
                            ttlString = s.ToString();

                        if (ttlString != string.Empty)
                        {
                            b = true;
                            break;
                        }
                    }

                    if (line.IndexOf("queries relating") != -1 || line.ToLower().IndexOf("imports_sea@next.co.uk") != -1 || line.ToLower().IndexOf("merchandise_accounts@next.co.uk") != -1)
                    {
                        b = true;
                        break;
                    }
                    lineNo += 1;
                }
            }
            return b;
        }

        private string getDCNoteNoPrefix(StringBuilder sb, out string dcNoteNo)
        {
            dcNoteNo = string.Empty;
            using (StringReader sr = new StringReader(sb.ToString()))
            {
                int lineNo = 1;
                int dcNoteHeaderLineNo = -1;
                string line = string.Empty;
                string pattern = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    System.Diagnostics.Debug.WriteLine(line);
                    if (line.IndexOf("RETURNS DEBIT NOTE") != -1 || line.IndexOf("MANUFACTURER FAULT RETURNS") != -1)
                        pattern = @"DN\d{6,}";
                    else if (line.IndexOf("FINE DEBIT NOTE") != -1)
                        pattern = @"F\d{6,}";
                    else if (line.ToUpper().IndexOf("FIRA DEBIT NOTE") != -1)
                        pattern = @"FRA\d{6,}";
                    else if (line.IndexOf("Compliance Debit Note") != -1)
                        pattern = @"CFS\d{6,}";
                    else if (line.IndexOf("Rejected Stock Debit Note") != -1)
                        pattern = @"(PFS|RFS|DFS)\d{6,}";

                    if (line.IndexOf("Debit Note No:") != -1)
                    {
                        if (pattern == string.Empty)
                            pattern = @"(PFS|RFS|DFS)\d{6,}";

                        dcNoteHeaderLineNo = lineNo;
                        var matches = Regex.Matches(line, pattern);
                        foreach (Match s in matches)
                            dcNoteNo = s.ToString();
                    }

                    if (lineNo == dcNoteHeaderLineNo + 1)
                    {
                        if (dcNoteNo == string.Empty)
                        {
                            var matches = Regex.Matches(line, pattern);
                            foreach (Match s in matches)
                                dcNoteNo = s.ToString();
                        }
                        break;
                    }

                    lineNo++;
                }

                var prefixes = Regex.Matches(dcNoteNo, "[A-Z]+");
                string prefix = string.Empty;
                foreach (Match v in prefixes)
                    prefix = v.ToString();
                return prefix;
            }
        }

        private static DateTime getDNDate(string s)
        {
            string[] monthList = new string[12];
            for (int i = 1; i <= 12; i++)
            {
                DateTimeFormatInfo mfi = new DateTimeFormatInfo();
                monthList[i - 1] = mfi.GetMonthName(i).ToString();
            }

            DateTime d = DateTime.MinValue;
            MatchCollection matches = null;
            matches = Regex.Matches(s, @"\d{2} (January|February|March|April|May|June|July|August|September|October|November|December) \d{4}");
            foreach (Match v in matches)
            {
                string[] tmp = v.ToString().Split(' ');
                d = new DateTime(int.Parse(tmp[2]), (Array.IndexOf(monthList, tmp[1]) + 1), int.Parse(tmp[0]));
            }
            return d;
        }


        private void splitPdf2(string pdfFilePath, string outputPath, UKClaimDef def)
        {
            try
            {
                using (PdfReader reader = new PdfReader(pdfFilePath))
                {

                    string filePath = outputPath + def.UKDebitNoteNo + ".pdf";

                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    PdfCopy copy = new PdfCopy(document, new FileStream(filePath, FileMode.Create));

                    document.Open();

                    for (int i = 0; i < def.NumOfPageForUpload; i++)
                        copy.AddPage(copy.GetImportedPage(reader, def.StartPageForUpload + i));

                    document.Close();
                }
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, string.Empty);
            }
        }

    }
}
