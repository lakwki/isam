using System;
using System.Collections;
using System.Web;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.isam.appserver.account;
using System.Web.UI.WebControls;
using com.next.common.domain.types;
using com.next.infra.util;
using com.next.common.domain.module;
using com.next.infra.web;
using com.next.isam.webapp.commander.account;
using System.Collections.Generic;

namespace com.next.isam.webapp.account
{
    public partial class AdvancePaymentSearch : com.next.isam.webapp.usercontrol.PageTemplate
    {
        List<AdvancePaymentDef> vwPaymentList
        {
            get
            {
                return (ViewState["vwPaymentList"] != null) ? (List<AdvancePaymentDef>)ViewState["vwPaymentList"] : new List<AdvancePaymentDef>();
            }
            set
            {
                ViewState["vwPaymentList"] = value;
            }
        }

        public class PaymentStatus
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                this.ddl_Office.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--All--", GeneralCriteria.ALL.ToString());
                uclVendor.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

                IList statuslist = new List<PaymentStatus>()
                {
                 new PaymentStatus(){ ID=1, Name="ALL"},
                 new PaymentStatus(){ ID=2, Name="FULLY SETTLED"},
                 new PaymentStatus(){ ID=3, Name="OUTSTANDING"},
                };
                ddlStatus.DataSource = statuslist;
                ddlStatus.DataTextField = "Name";
                ddlStatus.DataValueField = "ID";
                ddlStatus.DataBind();

                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.advancePayment.Id, ISAMModule.advancePayment.Super))
                    this.btn_NewInstalment.Visible = false;
    
            }
        }

        private void bindGrid()
        {
            string contractNo = txtContractNo.Text.Trim();
            string LCBillRefNo = txtLCBillRefNo.Text.Trim();
            string paymentNo = txt_PaymentNo.Text.Trim();
            DateTime fromDate = (String.IsNullOrEmpty(txtPaymentDateFrom.Text)) ? DateTime.MinValue : txtPaymentDateFrom.DateTime;
            DateTime toDate = (String.IsNullOrEmpty(txtPaymentDateTo.Text)) ? DateTime.MinValue : txtPaymentDateTo.DateTime;
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetAdvancePaymentByCriteria);
            int officeId = int.Parse(ddl_Office.SelectedValue);
            if (officeId > 0)
            {
                Context.Items.Add(AccountCommander.Param.officeId, officeId);
            }
            int vendorId = uclVendor.VendorId;
            if (vendorId > 0)
            {
                Context.Items.Add(AccountCommander.Param.vendorId, vendorId);
            }
            Context.Items.Add(AccountCommander.Param.contractNo, contractNo);
            Context.Items.Add(AccountCommander.Param.paymentNo, paymentNo);
            Context.Items.Add(AccountCommander.Param.lcBillRefNo, LCBillRefNo);
            Context.Items.Add(AccountCommander.Param.paymentDateFrom, fromDate);
            Context.Items.Add(AccountCommander.Param.paymentDateTo, toDate);
            Context.Items.Add(AccountCommander.Param.advancePaymentSettlementStatus, this.ddlStatus.SelectedValue);
            forwardToScreen(null);
            this.vwPaymentList = (List<AdvancePaymentDef>)Context.Items[AccountCommander.Param.advancePaymentList];
            //createPaymentTypeConfig();
            gv_Request.DataSource = this.vwPaymentList;
            gv_Request.DataBind();
        }
        

        #region Button 
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            this.bindGrid();
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddl_Office.SelectedIndex = 0;
            txt_PaymentNo.Text = "";
            txtPaymentDateFrom.Text = txtPaymentDateTo.Text = "";
            uclVendor.clear();
            txtLCBillRefNo.Text = "";
            txtContractNo.Text = "";
        }
        protected void btn_NewInstalment_Click(object sender, EventArgs e)
        {
            newInstalment();
        }
        private void newInstalment()
        {
            string targetLocation = "AdvPaymentInstalment.aspx?PaymentId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam("-1"));
            string targetName = "AdvPaymentInstalment";
            com.next.infra.util.ClientScript.windowOpen(targetLocation, targetName, 800, 700, false, true, false, false, true, Page);
        }
        #endregion Button

        #region Payment Type Configs
        /// <summary>
        /// Some minor config for Payment Type
        /// </summary>
        private enum PaymentTypeConfig
        {
            TypeName = 0, Popup = 1 // string[] = {TypeName, Popup}
        }
        /*
        private void createPaymentTypeConfig()
        {
            dicPaymentTypeConfig.Add(PaymentType.FABRIC, new string[] { PaymentType.FABRIC.Name, "EditFabric" });
            dicPaymentTypeConfig.Add(PaymentType.INSTALMENT, new string[] { PaymentType.INSTALMENT.Name, "AdvPaymentInstalment" });
        }
        */

        private string getPaymentTypeConfig(int paymentTypeId, PaymentTypeConfig config)
        {
            Dictionary<PaymentType, string[]> dicPaymentTypeConfig = new Dictionary<PaymentType, string[]>();

            dicPaymentTypeConfig.Add(PaymentType.FABRIC, new string[] { PaymentType.FABRIC.Name, "EditFabric" });
            dicPaymentTypeConfig.Add(PaymentType.INSTALMENT, new string[] { PaymentType.INSTALMENT.Name, "AdvPaymentInstalment" });

            PaymentType ptObject = PaymentType.getType(paymentTypeId);
            try
            {
                return dicPaymentTypeConfig[ptObject][config.GetHashCode()];
            }
            catch
            {
                return "";
            }
        }
        #endregion Payment Type Config

        protected void gv_Request_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            bool isAuthenticated = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.View);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AdvancePaymentDef apDef = (AdvancePaymentDef)this.vwPaymentList[e.Row.RowIndex];
                LinkButton lnk_Edit = ((LinkButton)e.Row.FindControl("lnk_Edit"));
                lnk_Edit.Text = "<img src=\"../images/icon_edit.gif\" border=0>";
                string popup = getPaymentTypeConfig(apDef.PaymentTypeId, PaymentTypeConfig.Popup);
                string typeName = getPaymentTypeConfig(apDef.PaymentTypeId, PaymentTypeConfig.TypeName);
                string parameter = "PaymentId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(apDef.PaymentId.ToString()) );
                string js = String.Format(@"window.open('{0}?{2}', '{1}', 'width=1000,height=600, scrollbars=1,resizable=1,status=1');", popup + ".aspx", popup, parameter);
                lnk_Edit.Attributes.Add("onclick", js);

                // Delete button
                LinkButton lnk_Delete = ((LinkButton)e.Row.FindControl("lnk_Delete"));
                if (apDef.PaymentTypeId == PaymentType.INSTALMENT.Id)
                {
                    lnk_Delete.Text = "<img src=\"../images/icon_delete.gif\" border=0>";
                    js = String.Format(@"return confirm('Are you sure to delete this record?');");
                    lnk_Delete.Attributes.Add("onclick", js);
                    lnk_Delete.CommandArgument = e.Row.RowIndex.ToString();
                    lnk_Delete.CommandName = "DeleteAdvancePaymentInstalment";
                    lnk_Delete.Visible = false;
                }
                else
                {
                    lnk_Delete.Visible = false;
                }

                ((Label)e.Row.FindControl("lbl_PaymentNo")).Text = apDef.PaymentNo; 
                ((Label)e.Row.FindControl("lbl_PaymentType")).Text = PaymentType.getType(apDef.PaymentTypeId).Name;
                ((Label)e.Row.FindControl("lbl_Office")).Text = OfficeId.getName(apDef.OfficeId);//OfficeId.getName(def.OfficeId);
                ((Label)e.Row.FindControl("lbl_Vendor")).Text = (apDef.Vendor != null) ? apDef.Vendor.Name : "--";
                //((Label)e.Row.FindControl("lbl_IssuedTo")).Text = def.PartyName;
                ((Label)e.Row.FindControl("lbl_Currency")).Text = apDef.Currency.CurrencyCode;//CurrencyId.getName(def.CurrencyId);
                ((Label)e.Row.FindControl("lbl_TotalAmt")).Text = (Math.Abs(apDef.TotalAmount) - Math.Abs(apDef.InterestChargedAmt)).ToString("#,##0.00");
                ((Label)e.Row.FindControl("lbl_PaymentDate")).Text = DateTimeUtility.getDateString(apDef.PaymentDate);//CurrencyId.getName(def.CurrencyId);
                ((Label)e.Row.FindControl("lbl_CreatedBy")).Text = apDef.CreatedBy.DisplayName;

                AdvancePaymentSummaryDef summaryDef = AccountManager.Instance.getAdvancePaymentSummaryDef(apDef.PaymentId);
                ((Label)e.Row.FindControl("lbl_Balance")).Text = summaryDef.Balance.ToString("#,##0.00");
                ((Label)e.Row.FindControl("lbl_Variance")).Text = summaryDef.Variance.ToString("#,##0.00");
                ((Label)e.Row.FindControl("lbl_NoRecoveryPlanBalance")).Text = summaryDef.NoRecoveryPlanBalance < 0 ? "0.00" : summaryDef.NoRecoveryPlanBalance.ToString("#,##0.00");

                if (apDef.PaymentTypeId == PaymentType.INSTALMENT.Id)
                {
                    string s = string.Empty;
                    List<AdvancePaymentInstalmentDetailDef> instalmentDetailList = AccountManager.Instance.getAdvancePaymentInstalmentDetailList(apDef.PaymentId);
                    foreach (AdvancePaymentInstalmentDetailDef detailDef in instalmentDetailList)
                    {
                        if (detailDef.SettlementDate == DateTime.MinValue && !detailDef.IsInterestCharge)
                        {
                            s = String.Format("{0}&nbsp;&nbsp;&nbsp;&nbsp;{1}{2}", DateTimeUtility.getDateString(detailDef.PaymentDate), apDef.Currency.CurrencyCode, detailDef.PaymentAmount.ToString("#,###.00"));
                            break;
                        }
                    }
                    ((Label)e.Row.FindControl("lbl_ComingDeduction")).Text = s;
                }
            }
        }

        protected void gv_Request_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAdvancePaymentInstalment")
            {
                int RowIndex = int.Parse((string)e.CommandArgument);
                AdvancePaymentDef apDef = (AdvancePaymentDef)this.vwPaymentList[RowIndex];
                if (apDef != null && apDef.PaymentTypeId == PaymentType.INSTALMENT.Id)
                {
                    apDef.Status = 0; // Update for delete
                    List<AdvancePaymentActionHistoryDef> historys = new List<AdvancePaymentActionHistoryDef>();
                    historys.Add(new AdvancePaymentActionHistoryDef()
                    {
                        PaymentId = apDef.PaymentId,
                        Description = "DELETE By Installment Advance Payment",
                        ActionBy = this.LogonUserId,
                        ActionOn = DateTime.Now,
                        Status = 1
                    });
                    Context.Items.Clear();
                    Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                    Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateAdvancePaymentAndInstalment);
                    Context.Items.Add(AccountCommander.Param.advancePayment, apDef);
                    Context.Items.Add(AccountCommander.Param.instalments, new List<AdvancePaymentInstalmentDetailDef>()); // no item add/update
                    Context.Items.Add(AccountCommander.Param.paymenthistory, historys);
                    forwardToScreen(null);
                    this.vwPaymentList.RemoveAt(RowIndex);
                    gv_Request.DataSource = this.vwPaymentList;
                    gv_Request.DataBind();
                }
            }
        }

        private string sortExpression
        {
            get { return (string)ViewState["SortExpression"]; }
            set { ViewState["SortExpression"] = value; }
        }

        private SortDirection sortDirection
        {
            get { return (SortDirection)ViewState["SortDirection"]; }
            set { ViewState["SortDirection"] = value; }

        }

        protected void gv_Request_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (sortExpression == e.SortExpression)
            {
                sortDirection = (sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending);
            }
            else
            {
                sortExpression = e.SortExpression;
                sortDirection = SortDirection.Ascending;
            }
            
            AdvancePaymentDef.AdvancePaymentComparer.CompareType compareType;

            if (sortExpression == "PaymentNo")
                compareType = AdvancePaymentDef.AdvancePaymentComparer.CompareType.PaymentNo;
            else if (sortExpression == "Vendor")
                compareType = AdvancePaymentDef.AdvancePaymentComparer.CompareType.Vendor;
            else if (sortExpression == "Currency")
                compareType = AdvancePaymentDef.AdvancePaymentComparer.CompareType.Currency;
            else if (sortExpression == "PaymentAmt")
                compareType = AdvancePaymentDef.AdvancePaymentComparer.CompareType.PaymentAmt;
            else if (sortExpression == "PaymentDate")
                compareType = AdvancePaymentDef.AdvancePaymentComparer.CompareType.PaymentDate;
            else
                compareType = AdvancePaymentDef.AdvancePaymentComparer.CompareType.PaymentNo;

            vwPaymentList.Sort(new AdvancePaymentDef.AdvancePaymentComparer(compareType, sortDirection));
            this.gv_Request.DataSource = vwPaymentList;
            this.gv_Request.DataBind();
        }

    }
}
