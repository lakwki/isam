using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain.module;
using com.next.common.domain.dms;
using com.next.isam.domain.claim;
using com.next.infra.web;
using com.next.isam.appserver.claim;
using com.next.isam.domain.types;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.dataserver.worker;
using com.next.infra.util;

namespace com.next.isam.webapp.claim
{
    public partial class DNAttachmentList : com.next.isam.webapp.usercontrol.PageTemplate
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;

            int dcNoteId = 0;

            if (!Page.IsPostBack)
                dcNoteId = int.Parse(EncryptionUtility.DecryptParam(Request.Params["dcNoteId"].ToString()));
            else
                dcNoteId = this.vwUKClaimDCNoteDef.DCNoteId;

            this.vwUKClaimDCNoteDef = UKClaimManager.Instance.getUKClaimDCNoteByKey(dcNoteId);

            if (!Page.IsPostBack)
            {
                this.bindRecord();
            }
        }

        private void bindRecord()
        {
            UKClaimDCNoteDef noteDef = this.vwUKClaimDCNoteDef;

            this.lblDNNo.Text = noteDef.DCNoteNo;
            this.lblVendor.Text = noteDef.PartyName;

            ArrayList attachmentInfoList = new ArrayList();
            List<UKClaimDCNoteDetailDef> detailList = UKClaimWorker.Instance.getUKClaimDCNoteDetailListByDCNoteId(noteDef.DCNoteId);
            foreach (UKClaimDCNoteDetailDef def in detailList)
            {
                if (def.ClaimRefundId == 0 && def.RechargeableAmount != 0)
                {
                    UKClaimDef ukClaimDef = UKClaimWorker.Instance.getUKClaimByKey(def.ClaimId);

                    if (ukClaimDef.HasUKDebitNote && noteDef.SettledAmount == noteDef.TotalAmount)
                    {
                        ArrayList queryStructs = new ArrayList();

                        queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                        queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
                        queryStructs.Add(new QueryStructDef("Claim Type", ukClaimDef.Type.DMSDescription));
                        queryStructs.Add(new QueryStructDef("Item No", ukClaimDef.ItemNo));
                        queryStructs.Add(new QueryStructDef("Debit Note No", ukClaimDef.UKDebitNoteNo));

                        ArrayList qList = DMSUtil.queryDocument(queryStructs);
                        if (qList.Count > 1)
                        {
                            queryStructs.Add(new QueryStructDef("Qty", ukClaimDef.Quantity.ToString()));
                            qList = DMSUtil.queryDocument(queryStructs);
                        }

                        foreach (DocumentInfoDef docInfoDef in qList)
                        {
                            foreach (AttachmentInfoDef attDef in docInfoDef.AttachmentInfos)
                            {
                                FieldInfoDef fiDef = (FieldInfoDef)docInfoDef.FieldInfos[1];
                                attDef.Description = fiDef.Content + "," + docInfoDef.DocumentID.ToString();
                                if (attDef.FileName.IndexOf(".html") == -1)
                                    attachmentInfoList.Add(attDef);
                            }
                        }

                    }
                }
            }

            this.vwAttachmentList = attachmentInfoList;
            this.gvAttachment.DataSource = attachmentInfoList;
            this.gvAttachment.DataBind();
        }

        private QAIS.ClaimRequestDef vwClaimRequestDef
        {
            get
            {
                return (QAIS.ClaimRequestDef)ViewState["vwClaimRequestDef"];
            }
            set
            {
                ViewState["vwClaimRequestDef"] = value;
            }
        }

        private UKClaimDCNoteDef vwUKClaimDCNoteDef
        {
            get
            {
                return (UKClaimDCNoteDef)ViewState["vwUKClaimDCNoteDef"];
            }
            set
            {
                ViewState["vwUKClaimDCNoteDef"] = value;
            }
        }

        private ArrayList vwAttachmentList
        {
            set
            {
                ViewState["AttachmentList"] = value;
            }
            get
            {
                return (ArrayList)ViewState["AttachmentList"];
            }
        }


        protected void gvAttachment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AttachmentInfoDef def = (AttachmentInfoDef)this.vwAttachmentList[e.Row.RowIndex];
                ((LinkButton)e.Row.FindControl("lnk_FileName")).Text = def.FileName;
                ((LinkButton)e.Row.FindControl("lnk_FileName")).CommandArgument = e.Row.RowIndex.ToString();
                ((Label)e.Row.FindControl("lbl_FileName")).Text = def.FileName;
                ((Label)e.Row.FindControl("lbl_FileName")).Visible = false;

                ((Label)e.Row.FindControl("lbl_Type")).Text = def.Description.Split(',')[0]; 
                ((Label)e.Row.FindControl("lbl_UploadDate")).Text = def.LastModifyDate.ToString("dd/MM/yyyy HH:mm");
                ((Label)e.Row.FindControl("lbl_MajorId")).Text = def.MajorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_MinorId")).Text = def.MinorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_BuildId")).Text = def.Build.ToString();

            }
        }

        protected void gvAttachment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            AttachmentInfoDef def = (AttachmentInfoDef)this.vwAttachmentList[int.Parse((string)e.CommandArgument)];

            if (e.CommandName == "OpenAttachment")
            {
                Context.Items.Clear();
                string[] s = def.Description.Split(',');
                Context.Items.Add("docId", s[1]);
                Context.Items.Add("attId", def.AttachmentID.ToString());
                Context.Items.Add("majorId", def.MajorVerion.ToString());
                Context.Items.Add("minorId", def.MinorVerion.ToString());
                Context.Items.Add("buildId", def.Build.ToString());
                forwardToScreen("dms.Attachment");
            }
        }



    }
}
