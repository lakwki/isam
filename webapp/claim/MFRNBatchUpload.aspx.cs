using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using com.next.common.web.commander;
using com.next.isam.appserver.claim;
using com.next.isam.appserver.account;
using com.next.isam.appserver.shipping;
using System.Web.UI.WebControls;
using com.next.infra.web;
using com.next.isam.domain.claim;
using com.next.isam.domain.account;
using com.next.common.domain.types;
using com.next.infra.util;
using com.next.common.domain.dms;
using System.Data;
using System.Data.OleDb;
using System.Web.UI;
using com.next.isam.domain.order;
using com.next.common.domain.module;
using com.next.isam.dataserver.worker;

namespace com.next.isam.webapp.claim
{
    public partial class MFRNBatchUpload : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private bool hasAttachmentError = false;
        private bool hasVendorError = false;
        private bool hasDuplication = false;
        private bool hasUnDefinedPaymentSupplier = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            if (!Page.IsPostBack)
                this.btnConfirm.Enabled = false;
        }


        protected void gv_UKClaim_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string targetFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder") + "mfrn_upload\\";

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                MFRNUploadDef def = (MFRNUploadDef)this.vwExcelList[e.Row.RowIndex];

                ((Label)e.Row.FindControl("lbl_DNNo")).Text = def.DNNo;
                ((Label)e.Row.FindControl("lbl_DNDate")).Text = DateTimeUtility.getDateString(def.DNDate);
                ((Label)e.Row.FindControl("lbl_ReceivedDate")).Text = DateTimeUtility.getDateString(def.ReceivedDate);
                ((Label)e.Row.FindControl("lbl_Currency")).Text = def.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = def.ItemNo;
                ((Label)e.Row.FindControl("lbl_Qty")).Text = def.Qty.ToString("#,##0");
                ((Label)e.Row.FindControl("lbl_Amount")).Text = def.Amount.ToString("#,##0.00");
                Label lblAttach = ((Label)e.Row.FindControl("lbl_Attachment"));
                lblAttach.Font.Bold = false;
                lblAttach.ForeColor = System.Drawing.Color.Black;

                if (File.Exists(targetFolder + def.DNNo + ".pdf"))
                    lblAttach.Text = "Yes";
                else
                {
                    lblAttach.Text = "No";
                    lblAttach.Font.Bold = true;
                    lblAttach.ForeColor = System.Drawing.Color.Red;
                    hasAttachmentError = true;
                }

                ArrayList shipmentList = ShipmentManager.Instance.GetShipmentProductByItemNo(def.ItemNo, string.Empty, -1);
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

                    ((Label)e.Row.FindControl("lbl_Vendor")).Text = shipment.VendorName;
                    def.VendorId = shipment.VendorId;

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
                    def.OfficeId = shipment.OfficeId;
                    def.HandlingOfficeId = shipment.HandlingOfficeId;
                    def.ProductTeamId = shipment.ProductTeamId;
                    def.TermOfPurchaseId = shipment.TermOfPurchaseId;

                    ((Label)e.Row.FindControl("lbl_Office")).Text = OfficeId.getName(shipment.OfficeId);
                    ((Label)e.Row.FindControl("lbl_HandlingOffice")).Text = OfficeId.getName(shipment.HandlingOfficeId);
                    ((Label)e.Row.FindControl("lbl_ProductTeam")).Text = shipment.ProductTeamCode;
                    UKPaymentSupplierDef paymentSupplierDef = AccountManager.Instance.getUKPaymentSupplierByKey(def.PaymentSupplierCode.Trim());

                    if (paymentSupplierDef != null)
                        ((Label)e.Row.FindControl("lbl_PaymentOffice")).Text = OfficeId.getName(AccountManager.Instance.getUKPaymentSupplierByKey(def.PaymentSupplierCode.Trim()).OfficeId);
                    else
                    {
                        hasUnDefinedPaymentSupplier = true;
                        ((Label)e.Row.FindControl("lbl_Note")).Text = "Undefined Payment Supplier Code - " + def.PaymentSupplierCode.Trim();
                    }


                    //List<UKClaimDef> ukClaimList = UKClaimManager.Instance.getUKClaimListByTypeMapping(-1, UKClaimType.MFRN.Id, shipment.VendorId, def.ItemNo, txtMonth.Text.Trim(), def.Qty);
                    List<UKClaimDef> ukClaimList = UKClaimManager.Instance.getUKClaimListByTypeMapping(-1, UKClaimType.MFRN.Id, shipment.VendorId, def.ItemNo, def.DNNo.Trim(), def.Qty);

                    if (ukClaimList.Count > 0)
                    {
                        hasDuplication = true;
                        ((Label)e.Row.FindControl("lbl_Note")).Text = "Record already exists";
                    }

                }
                else
                {
                    hasVendorError = true;
                }
            }

        }


        protected void btnLoadExcel_Click(object sender, EventArgs e)
        {
            string destinationPath = string.Empty;
            hasAttachmentError = false;
            hasVendorError = false;
            this.btnConfirm.Enabled = false;

            if (this.uplExcelFile.HasFile)
            {
                destinationPath = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder") + "mfrn_upload\\" + this.uplExcelFile.FileName;
                this.uplExcelFile.PostedFile.SaveAs(destinationPath);
                string strConn = " Provider = Microsoft.ACE.OLEDB.12.0; Data Source = " + destinationPath + @";Extended Properties=""Excel 12.0 Xml;HDR=YES;IMEX=1"" ";
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                string strSQL = "SELECT * FROM MFRNLIST";
                OleDbDataAdapter ad = new OleDbDataAdapter(strSQL, conn);
                
                DataSet dataSet = new DataSet();
                ArrayList list = new ArrayList();
                int recordsAffected = ad.Fill(dataSet);

                foreach (DataRow r in dataSet.Tables[0].Rows)
                {
                    MFRNUploadDef def = new MFRNUploadDef();
                    def.DNNo = r[0].ToString();
                    def.PaymentSupplierCode = r[1].ToString().Trim();
                    def.DNDate = DateTime.Parse(r[4].ToString());
                    def.CurrencyCode = r[5].ToString();
                    def.ItemNo = r[3].ToString().Trim();
                    def.Qty = Math.Abs(int.Parse(r[6].ToString()));
                    def.Amount = Math.Abs(decimal.Parse(r[7].ToString()));
                    def.ReceivedDate = DateTime.Parse(r[8].ToString());
                    
                    list.Add(def);
                }
                conn.Close();
                this.vwExcelList = list;
                this.gv_UKClaim.DataSource = list;
                this.gv_UKClaim.DataBind();

                if (hasAttachmentError)
                    Page.Validators.Add(new ValidationError("Some attachment(s) are missing."));
                if (hasVendorError)
                    Page.Validators.Add(new ValidationError("Unmatched Vendor was encountered"));
                if (hasDuplication)
                    Page.Validators.Add(new ValidationError("Some MFRN(s) already exist"));
                if (hasUnDefinedPaymentSupplier)
                    Page.Validators.Add(new ValidationError("Undefined payment supplier code was encountered"));

                if (!hasAttachmentError && !hasVendorError && list.Count > 0)
                    this.btnConfirm.Enabled = true;
            }
            else
            {
                Page.Validators.Add(new ValidationError("Please upload the excel file."));
            }

        }

        ArrayList vwExcelList
        {
            get { return (ArrayList)ViewState["ExcelList"]; }
            set { ViewState["ExcelList"] = value; }
        }

        protected void btnUploadZip_Click(object sender, EventArgs e)
        {
            string targetFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder") + "mfrn_upload\\";
            if (this.uplZipFile.HasFile)
            {
                this.uplZipFile.PostedFile.SaveAs(targetFolder + this.uplZipFile.FileName);
                /*
                FileUtility.unZip(targetFolder + this.uplZipFile.FileName, targetFolder);
                */
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Zip File Upload", "alert('Zip contents have been loaded to the server.');", true);
        }

        private string uploadUKDebitNote(MFRNUploadDef def)
        {
            string outputFolder = WebConfig.getValue("appSettings", "MFRN_UPLOAD_FOLDER");

            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", "MFRN"));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Debit Note No", def.DNNo));
            queryStructs.Add(new QueryStructDef("Item No", def.ItemNo));
            queryStructs.Add(new QueryStructDef("MFRN Month", txtMonth.Text.Trim()));

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            long docId = 0;
            foreach (DocumentInfoDef docInfoDef in qList)
                docId = docInfoDef.DocumentID;

            string filename = outputFolder + def.DNNo + ".pdf";

            ArrayList attachmentList = new ArrayList();
            attachmentList.Add(filename);
            string uploadMsg;
            if (docId > 0)
                uploadMsg = DMSUtil.UpdateDocumentWithoutExistingAttachments(docId, queryStructs, attachmentList);
            else
                uploadMsg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", def.DNNo, "UK Claim", queryStructs, attachmentList);
            /*
            FileUtility.clearFolder(outputFolder, false);
            */

            return (uploadMsg == string.Empty ? def.DNNo + ".pdf" : string.Empty);
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            List<UKClaimDef> discrepancyList = new List<UKClaimDef>(); ;

            foreach (MFRNUploadDef def in this.vwExcelList)
            {
                
                UKClaimDef claimDef = new UKClaimDef();
                claimDef.Type = UKClaimType.MFRN;
                claimDef.ItemNo = def.ItemNo;
                claimDef.ProductTeamId = def.ProductTeamId;
                claimDef.OfficeId = def.OfficeId;
                claimDef.HandlingOfficeId = def.HandlingOfficeId;
                claimDef.PaymentOfficeId = AccountManager.Instance.getUKPaymentSupplierByKey(def.PaymentSupplierCode.Trim()).OfficeId;

                claimDef.Vendor = IndustryUtil.getVendorByKey(def.VendorId);
                claimDef.TermOfPurchaseId = def.TermOfPurchaseId;
                claimDef.Quantity = def.Qty;
                
                if (def.CurrencyCode == "USD")
                    claimDef.Currency = CommonUtil.getCurrencyByKey(CurrencyId.USD.Id);
                else if (def.CurrencyCode == "GBP")
                    claimDef.Currency = CommonUtil.getCurrencyByKey(CurrencyId.GBP.Id);
                else if (def.CurrencyCode == "EUR")
                    claimDef.Currency = CommonUtil.getCurrencyByKey(CurrencyId.EUR.Id);
                else if (def.CurrencyCode == "CNY")
                    claimDef.Currency = CommonUtil.getCurrencyByKey(CurrencyId.RMB.Id);
                else if (def.CurrencyCode == "HKD")
                    claimDef.Currency = CommonUtil.getCurrencyByKey(CurrencyId.HKD.Id);
                claimDef.Amount = def.Amount;
                claimDef.UKDebitNoteNo = def.DNNo;
                claimDef.UKDebitNoteDate = def.DNDate;
                claimDef.UKDebitNoteReceivedDate = def.ReceivedDate;
                claimDef.GUId = Guid.NewGuid().ToString();
                claimDef.ClaimMonth = txtMonth.Text.Trim();

                /*
                UKPaymentSupplierDef supplierDef = AccountManager.Instance.getUKPaymentSupplierByKey(def.PaymentSupplierCode);
                claimDef.PaymentOfficeId = supplierDef.OfficeId;
                */

                QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

                List<QAIS.ClaimRequestDef> list = new List<QAIS.ClaimRequestDef>();
                object[] activeList = svc.GetActiveClaimRequestListByType(-1, 3, def.VendorId, def.ItemNo, def.DNDate.ToString("yyyyMM"));

                if (activeList.Length == 1)
                {
                    foreach (object o in activeList)
                        if (o.GetType() == typeof(QAIS.ClaimRequestDef))
                        {
                            if (((((QAIS.ClaimRequestDef)o).IsAuthorized) || (3 == UKClaimType.MFRN.Id && ((QAIS.ClaimRequestDef)o).IsAuthorized)) && UKClaimManager.Instance.getUKClaimByClaimRequestId(((QAIS.ClaimRequestDef)o).RequestId) == null)
                                claimDef.ClaimRequestId = ((QAIS.ClaimRequestDef)o).RequestId;
                        }
                }

               
                bool isSubmitted = false;
                if (claimDef.WorkflowStatus.Id == ClaimWFS.NEW.Id && claimDef.ClaimRequestId  != -1)
                {
                    QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(claimDef.ClaimRequestId);
                    if (requestDef.WorkflowStatusId == 8)
                    {
                        claimDef.WorkflowStatus = ClaimWFS.USER_SIGNED_OFF;
                        isSubmitted = true;
                    }
                    else if ((requestDef.WorkflowStatusId == 10 || 
                              ((requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7) && requestDef.IsAuthorized) || 
                              ((requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7) && requestDef.IsAuthorized)) 
                        && requestDef.VendorRechargePercent == 100)
                    {
                        if (requestDef.ItemNo == claimDef.ItemNo && requestDef.Vendor.VendorId == claimDef.Vendor.VendorId)
                        {
                            if (requestDef.WorkflowStatusId == 10)
                                svc.sendCancelledClaimNotification(requestDef.RequestId, this.LogonUserId);

                            claimDef.WorkflowStatus = ClaimWFS.SUBMITTED;
                            isSubmitted = true;
                            if (requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7)
                            {
                                if (claimDef.Remark == string.Empty)
                                    claimDef.Remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier";
                                else
                                    claimDef.Remark += ("\n" + "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier");
                                claimDef.IsReadyForSettlement = true;
                            }
                            if (requestDef.WorkflowStatusId == 10)
                                claimDef.IsReadyForSettlement = false;
                        }
                    }
                }

                if (isSubmitted)
                {
                    QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(claimDef.ClaimRequestId);
                    if (claimDef.ClaimRequestId != -1 && requestDef.WorkflowStatusId != 10)
                        svc.SetClaimRequestStatus(claimDef.ClaimRequestId, 9, this.LogonUserId);
                }

                UKClaimManager.Instance.updateUKClaimDef(claimDef, this.LogonUserId);

                if (claimDef.OfficeId != claimDef.PaymentOfficeId)
                    discrepancyList.Add(claimDef);

                string fileUploaded = uploadUKDebitNote(def);
                
                if (isSubmitted)
                    this.updateDMSIndexFields(claimDef);
                
            }
            if (discrepancyList.Count > 0)
                NoticeHelper.sendUKClaimMFRNUploadAlertEmail(discrepancyList);

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "MFRN_Upload", "alert('Confirmed Complete.')", true);
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


    }
}
