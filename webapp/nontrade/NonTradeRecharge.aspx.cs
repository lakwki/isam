using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.infra.smartwebcontrol;
using com.next.infra.web;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.types;
using com.next.isam.appserver.account;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.accounts;
using com.next.common.domain.types;

namespace com.next.isam.webapp.nontrade
{
    public partial class NonTradeRecharge : com.next.isam.webapp.usercontrol.PageTemplate
    {
        ArrayList vwNTInvoiceList
        {
            get
            {
                return (ArrayList)ViewState["vwNTInvoiceList"];
            }
            set
            {
                ViewState["vwNTInvoiceList"] = value;
            }
        }
        ArrayList vwNTInvoiceDetailList
        {
            get
            {
                return (ArrayList)ViewState["vwNTInvoiceDetailList"];
            }
            set
            {
                ViewState["vwNTInvoiceDetailList"] = value;
            }

        }
        ArrayList vwNTRechargeCurrencyList
        {
            get
            {
                return (ArrayList)ViewState["vwNTRechargeCurrencyList"];
            }
            set
            {
                ViewState["vwNTRechargeCurrencyList"] = value;
            }
        }
        int maxDescriptionLength;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                /*
                ddl_Office.bindList(WebUtil.getNTOfficeList(this.LogonUserId), "OfficeCode", "OfficeId", "");
                */
                ddl_Office.bindList(NonTradeManager.Instance.getNTUserOfficeList(this.LogonUserId, NTRoleType.RECHARGE_DCNOTE_USER.Id, GeneralCriteria.ALL), "OfficeCode", "OfficeId", "");

                vwNTRechargeCurrencyList = new ArrayList();
                foreach (CurrencyRef cy in WebUtil.getEffectiveCurrencyList())
                    if (cy.CurrencyId == CurrencyId.GBP.Id || cy.CurrencyId == CurrencyId.USD.Id || cy.CurrencyId == CurrencyId.EUR.Id)
                        vwNTRechargeCurrencyList.Add(cy);

                refreshOfficeCompanyList();
                this.txtIssueDate.DateTime = DateTime.Today;
            }
            maxDescriptionLength = int.Parse(hid_MaxDescLength.Value);
        }

        protected void searchInvoice()
        {
            if (ddl_Office.Items.Count > 0)
            {
                int officeId = int.Parse(ddl_Office.SelectedValue);
                int companyId = int.Parse(ddl_Company.SelectedValue);

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetNTInvoiceDetailListForRechargeDCNote);

                Context.Items.Add(AccountCommander.Param.officeId, officeId);
                Context.Items.Add(AccountCommander.Param.companyId, companyId);

                forwardToScreen(null);

                vwNTInvoiceDetailList = (ArrayList)Context.Items[AccountCommander.Param.ntRechargeDetailList];
                vwNTInvoiceList = (ArrayList)Context.Items[AccountCommander.Param.invoiceList];

                //pnl_Result.Visible = true;
                pnl_Result.Style.Add(HtmlTextWriterStyle.Display, "block");
                lbl_ErrorMessage.Visible = false;

                gv_InvoiceDetail.DataSource = vwNTInvoiceDetailList;
                gv_InvoiceDetail.DataBind();
            }
        }

        protected void Search_OnClick(object sender, EventArgs e)
        {
            searchInvoice();
        }

        protected void Reset_OnClick(object sender, EventArgs e)
        {
            //pnl_Result.Visible = false;
            pnl_Result.Style.Add(HtmlTextWriterStyle.Display, "none");
            lbl_ErrorMessage.Visible = false;
            ddl_Office.SelectedIndex = -1;
        }

        protected void Generate_OnClick(object sender, EventArgs e)
        {
            lbl_ErrorMessage.Visible = false;
            if (getRechargeDCNote(false))
                searchInvoice();
        }

        protected void Draft_OnClick(object sender, EventArgs e)
        {
            lbl_ErrorMessage.Visible = false;
            getRechargeDCNote(true);
        }

        protected void ddl_office_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshOfficeCompanyList();
        }

        protected void refreshOfficeCompanyList()
        {
            if (ddl_Office.SelectedValue != "")
                ddl_Company.bindList(NonTradeManager.Instance.getNTUserCompanyList(this.LogonUserId, ddl_Office.selectedValueToInt, NTRoleType.RECHARGE_DCNOTE_USER.Id), "CompanyName", "CompanyId", "");
            else
                ddl_Company.Items.Clear();
        }

        protected bool getRechargeDCNote(bool isDraft)
        {
            bool success = true;
            if (Page.IsValid)
            {
                ArrayList detailList = new ArrayList();

                foreach (GridViewRow gvRow in gv_InvoiceDetail.Rows)
                {
                    if (!((CheckBox)gvRow.FindControl("cbx")).Checked)
                        continue;

                    NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwNTInvoiceDetailList[gvRow.RowIndex];
                    detailDef.RechargeContactPerson = ((TextBox)gvRow.FindControl("txt_Attention")).Text;
                    detailDef.RechargeCurrency = CommonUtil.getCurrencyByKey(((SmartDropDownList)gvRow.FindControl("ddl_RechargeCurrency")).selectedValueToInt);
                    detailDef.Description = ((TextBox)gvRow.FindControl("txt_Description")).Text;
                    if (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.CUSTOMER.Id && detailDef.Customer.CustomerType.CustomerTypeId == domain.common.CustomerTypeRef.Type.retail.GetHashCode())
                        detailDef.RechargePartyDept = NTRechargePartyDeptType.getType(int.Parse(((SmartDropDownList)gvRow.FindControl("ddl_RechargeDept")).SelectedValue));
                    detailList.Add(detailDef);
                }

                ArrayList failList = new ArrayList();
                NTRechargeDCNote report = AccountReportManager.Instance.generateNTRechargeDCNote(detailList, isDraft, this.txtIssueDate.DateTime == DateTime.MinValue ? DateTime.Today : this.txtIssueDate.DateTime, this.LogonUserId, failList);
                if (report == null)
                {
                    lbl_ErrorMessage.Text = "Fail in generating debit note for the selected invoice" + (detailList.Count == 1 ? "" : "s") + " :";
                    if (failList.Count > 0)
                    {
                        ArrayList invList = new ArrayList();
                        foreach (NTInvoiceDetailDef dtl in failList)
                        {
                            NTInvoiceDef invoice = NonTradeManager.Instance.getNTInvoiceByKey(dtl.InvoiceId);
                            if (dtl.Description.Length > maxDescriptionLength)
                                lbl_ErrorMessage.Text += "<br>&nbsp;&nbsp;" + invoice.InvoiceNo + " - Description exceeds the " + maxDescriptionLength.ToString() + " characters size limit.";
                            else
                                if (dtl.RechargeDCNoteId > 0)
                                {
                                    if (!invList.Contains(invoice.InvoiceNo))
                                    {
                                        invList.Add(invoice.InvoiceNo);
                                        NTRechargeDCNoteDef dcNote = NonTradeManager.Instance.getNTRechargeDCNoteByKey(dtl.RechargeDCNoteId);
                                        lbl_ErrorMessage.Text += "<br>&nbsp;&nbsp;" + invoice.InvoiceNo + " - Debit note " + dcNote.RechargeDCNoteNo + " has just been generated for this invoice by other user.";
                                    }
                                }
                        }
                    }
                    lbl_ErrorMessage.Visible = true;
                    success = false;
                }
                else
                    ReportHelper.export(report, HttpContext.Current.Response,
                        CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "NT Recharge DC Note");
            }
            else
                success = false;

            return success;
        }


        protected void gv_InvoiceDetail_DataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                SmartDropDownList ddl;
                NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)e.Row.DataItem;
                NTInvoiceDef invoiceDef = (NTInvoiceDef)vwNTInvoiceList[e.Row.RowIndex];

                if (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.OFFICE.Id)
                {
                    ((Label)e.Row.FindControl("lbl_Name")).Text = detailDef.Office.Description + " - " + detailDef.Company.CompanyName;
                    ((HiddenField)e.Row.FindControl("hf_Id")).Value = "o" + detailDef.Office.OfficeId.ToString();
                }
                else if (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.CUSTOMER.Id)
                {
                    ((Label)e.Row.FindControl("lbl_Name")).Text = WebUtil.getCustomerByKey(detailDef.Customer.CustomerId).CustomerDescription;
                    ((HiddenField)e.Row.FindControl("hf_Id")).Value = "c" + detailDef.Customer.CustomerId.ToString() + "-" + (detailDef.IntercommOfficeId == -1 ? invoiceDef.Office.OfficeId : detailDef.IntercommOfficeId);
                }
                else if (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.NT_VENDOR.Id)
                {
                    ((Label)e.Row.FindControl("lbl_Name")).Text = detailDef.NTVendor.VendorName;
                    ((HiddenField)e.Row.FindControl("hf_Id")).Value = "n" + detailDef.NTVendor.NTVendorId.ToString() + "-" + (detailDef.IntercommOfficeId == -1 ? invoiceDef.Office.OfficeId : detailDef.IntercommOfficeId); ;
                }
                else
                {
                    ((Label)e.Row.FindControl("lbl_Name")).Text = detailDef.Vendor.Name;
                    ((HiddenField)e.Row.FindControl("hf_Id")).Value = "v" + detailDef.Vendor.VendorId.ToString() + "-" + (detailDef.IntercommOfficeId == -1 ? invoiceDef.Office.OfficeId : detailDef.IntercommOfficeId); ;
                }

                ((LinkButton)e.Row.FindControl("lnk_Invoice")).Text = invoiceDef.InvoiceNo + (string.IsNullOrEmpty(invoiceDef.InvoiceNo) || string.IsNullOrEmpty(invoiceDef.CustomerNo)? "" : "/") + invoiceDef.CustomerNo;
                ((LinkButton)e.Row.FindControl("lnk_Invoice")).Attributes.Add("onclick", string.Format("openInvoiceWindow({0}); return false;", invoiceDef.InvoiceId.ToString()));
                ((Label)e.Row.FindControl("lbl_Currency")).Text = invoiceDef.Currency.CurrencyCode;
                if (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.CUSTOMER.Id && detailDef.Customer.CustomerType.CustomerTypeId == domain.common.CustomerTypeRef.Type.retail.GetHashCode())
                {
                    ddl = (SmartDropDownList)e.Row.FindControl("ddl_RechargeDept");
                    ddl.Visible = true;
                    int partyDeptId = 2;
                    if (detailDef.RechargePartyDept != null)
                        partyDeptId = detailDef.RechargePartyDept.Id;
                    ddl.bindList(NTRechargePartyDeptType.getCollectionValues(), "Description", "Id", partyDeptId.ToString());
                }
                ddl = (SmartDropDownList)e.Row.FindControl("ddl_RechargeCurrency");
                ddl.bindList(vwNTRechargeCurrencyList, "CurrencyCode", "CurrencyId", detailDef.RechargeCurrency.CurrencyId.ToString());
                ((TextBox)e.Row.FindControl("txt_Attention")).Text = detailDef.RechargeContactPerson;

                //((TextBox)e.Row.FindControl("txt_Description")).Text = detailDef.Description.Trim() != string.Empty ? detailDef.Description : NonTradeManager.Instance.getFullItemDescriptionByInvoiceDetailId(detailDef); 
                //2017-11-23 requested by Wing to show default item description
                ((TextBox)e.Row.FindControl("txt_Description")).Text = NonTradeManager.Instance.getFullItemDescriptionByInvoiceDetailId(detailDef); 
                
            }
        }

        protected void valInvoiceDetail_OnServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            bool anyDescriptionError = false;
            ArrayList detailList = new ArrayList();

            foreach (GridViewRow gvRow in gv_InvoiceDetail.Rows)
            {
                TextBox tbox = ((TextBox)gvRow.FindControl("txt_Description"));
                tbox.BorderColor = System.Drawing.Color.Empty;
                if (((CheckBox)gvRow.FindControl("cbx")).Checked)
                {

                    NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwNTInvoiceDetailList[gvRow.RowIndex];
                    detailDef.RechargeContactPerson = ((TextBox)gvRow.FindControl("txt_Attention")).Text;
                    detailDef.RechargeCurrency = CommonUtil.getCurrencyByKey(((SmartDropDownList)gvRow.FindControl("ddl_RechargeCurrency")).selectedValueToInt);
                    detailDef.Description = ((TextBox)gvRow.FindControl("txt_Description")).Text;
                    if (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.CUSTOMER.Id && detailDef.Customer.CustomerType.CustomerTypeId == domain.common.CustomerTypeRef.Type.retail.GetHashCode())
                        detailDef.RechargePartyDept = NTRechargePartyDeptType.getType(int.Parse(((SmartDropDownList)gvRow.FindControl("ddl_RechargeDept")).SelectedValue));
                    if (detailDef.Description.Length > maxDescriptionLength)
                    {
                        tbox.BorderColor = System.Drawing.Color.Red;
                        anyDescriptionError = true;
                    }
                    detailList.Add(detailDef);
                }
            }
            ArrayList zeroList;
            if (detailList.Count == 0)
            {
                args.IsValid = false;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeDCNote", "alert('Please select the record for generating debit note.');", true);
            }
            else
                if (anyDescriptionError)
                {
                    args.IsValid = false;
                    valInvoiceDetail.ErrorMessage = "Description size exceeds the " + maxDescriptionLength.ToString() + " characters limit.";
                }
                else
                    if ((zeroList = NonTradeManager.Instance.generateZeroAmountNTRechargeDCNoteList(detailList, this.txtIssueDate.DateTime == DateTime.MinValue ? DateTime.Today : this.txtIssueDate.DateTime)).Count > 0)
                    {   // some DC note with zero total amount
                        args.IsValid = false;
                        valInvoiceDetail.ErrorMessage = "Fail to generate Debit/Credit note for the following invoices because the total amount is zero:";
                        foreach (ArrayList invNoList in zeroList)
                        {
                            string msg = string.Empty;
                            foreach (string invNo in invNoList)
                                msg += (msg == string.Empty ? "<br>&nbsp;&nbsp;&nbsp;&nbsp;- " : ", ") + invNo;
                            valInvoiceDetail.ErrorMessage += msg;
                        }
                    }
        }
    }
}