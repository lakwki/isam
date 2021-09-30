using System;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using com.next.isam.appserver.account;
using com.next.isam.domain.account;
using com.next.isam.domain.shipping;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.product;
using com.next.isam.appserver.shipping;
using com.next.infra.web;
using com.next.infra.util;
using com.next.infra.smartwebcontrol;
using System.Collections.Generic;
using com.next.isam.webapp.commander.account;
using System.Linq;

namespace com.next.isam.webapp.account
{
    public partial class EditFabric : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private int paymentId;
        protected int vwPaymentId
        {
            get
            {
                return (ViewState["vwPaymentId"] != null) ? (int)ViewState["vwPaymentId"] : 0;
            }
            set
            {
                ViewState["vwPaymentId"] = value;
            }
        }
        /// <summary>
        /// For list object in Session
        /// </summary>
        protected string listIndex
        {
            get { return vwPaymentId + "_ContractList"; }
        }
        protected PaymentType paymentType
        {
            get { return PaymentType.FABRIC; }
        }
        /// <summary>
        /// Current Def for data update (Update advancePayment & Add/Update orderDetailList)
        /// </summary>
        DomainAdvancePaymentDef vwDomainAdvancePaymentDef
        {
            get
            {
                if (ViewState["vwDomainAdvancePaymentDef"] == null)
                {
                    ViewState["vwDomainAdvancePaymentDef"] = new DomainAdvancePaymentDef(typeof(AdvancePaymentOrderDetailDef));
                }
                return (DomainAdvancePaymentDef)ViewState["vwDomainAdvancePaymentDef"];
            }
            set
            {
                ViewState["vwDomainAdvancePaymentDef"] = value;
            }
        }

        /// <summary>
        /// Original version of AdvancePaymentOrderDetailDefList (Change Comparsion)
        /// </summary>
        List<AdvancePaymentOrderDetailDef> vwAdvancePaymentOrderDetailDefList0
        {
            get
            {
                return (ViewState["vwAdvancePaymentOrderDetailDefList0"] != null) ? (List<AdvancePaymentOrderDetailDef>)ViewState["vwAdvancePaymentOrderDetailDefList0"] : new List<AdvancePaymentOrderDetailDef>();
            }
            set
            {
                ViewState["vwAdvancePaymentOrderDetailDefList0"] = value;
            }
        }

        /// <summary>
        /// User input
        /// </summary>
        List<String> vwTxt_ExpectedValueList
        {
            get
            {
                return (ViewState["vwTxt_ExpectedValueList"] != null) ? (List<String>)(ViewState["vwTxt_ExpectedValueList"]) : new List<String>();
            }
            set
            {
                ViewState["vwTxt_ExpectedValueList"] = value;
            }
        }

        private List<AdvancePaymentBalanceSettlementDef> vwBalances
        {
            get
            {
                if (ViewState["vwBalances"] == null)
                    ViewState["vwBalances"] = new List<AdvancePaymentBalanceSettlementDef>();

                return (List<AdvancePaymentBalanceSettlementDef>)ViewState["vwBalances"];
            }
            set { ViewState["vwBalances"] = value; }
        }

        private string getExpectedValueInput(int index)
        {
            string inputValue = null;
            if (vwTxt_ExpectedValueList.Count > 0 && index < vwTxt_ExpectedValueList.Count)
            {
                try
                {
                    inputValue = vwTxt_ExpectedValueList[index];
                }
                catch
                {
                }
            }
            return inputValue;
        }

        private void setExpectedValueInput(int index, string strValue)
        {
            if (index < vwTxt_ExpectedValueList.Count)
            {
                vwTxt_ExpectedValueList[index] = strValue;
            }
        }

        /// <summary>
        /// The Total Payment Amt in an Advance Payment by Fabric Cost (readonly)
        /// </summary>
        protected decimal TotalAmount
        {
            get
            {
                return vwDomainAdvancePaymentDef.AdvancePayment.TotalAmount;
            }
        }

        /// <summary>
        /// The total amount of Actual Values (readonly)
        /// </summary>
        protected decimal TotalActualValue
        {
            get
            {
                decimal value = vwAdvancePaymentOrderDetailDefList0.Select(ex => ex.ActualValue).Sum();
                return value;
            }
        }

        protected decimal TotalExpectedValue
        {
            get
            {
                decimal value = vwAdvancePaymentOrderDetailDefList0.Select(ex => ex.ExpectedValue).Sum();
                return value;
            }
        }

        /// <summary>
        /// The total amount of Expected Values (to be updated)
        /// </summary>
        protected decimal totalExpectedValue;


        /// <summary>
        /// The total amount of Deduction Amount from Balance Settlement (readonly)
        /// </summary>
        protected decimal TotalDeductionAmount
        {
            get
            {
                decimal value = vwBalances.Where(c => c.Status == 1).Select(d => d.ExpectedAmount).Sum();
                return value;
            }
        }

        protected decimal BalDiff
        {
            get
            {
                //return TotalAmount - TotalActualValue - TotalDeductionAmount;
                decimal totalActualValue = vwAdvancePaymentOrderDetailDefList0.Where(ex => ex.SettlementDate > DateTime.MinValue).Select(ex => ex.ActualValue).Sum();
                decimal totalDeductionAmount = vwBalances.Where(d => d.SettlementDate > DateTime.MinValue).Select(d => d.PaymentAmount).Sum();
                return TotalAmount - totalActualValue - totalDeductionAmount;
            }
        }

        protected decimal NoRecoveryPlanBal
        {
            get
            {
                AdvancePaymentSummaryDef summaryDef = AccountManager.Instance.getAdvancePaymentSummaryDef(vwPaymentId);
                return summaryDef.NoRecoveryPlanBalance < 0 ? 0 : summaryDef.NoRecoveryPlanBalance;
            }
        }

        protected decimal TotalAmtVariance
        {
            get
            {
                return TotalAmount - TotalExpectedValue - TotalDeductionAmount;
            }
        }

        // Pre-NOW version
        private new string decryptParameter(string param) { return (string.IsNullOrEmpty(param) ? string.Empty : param); }
        private new string encryptParameter(string param) { return (string.IsNullOrEmpty(param) ? string.Empty : param); }
        private new string openPopupWindowFunction = "window.open";
        //NOW version
        //private string decryptParameter(string param) { return WebUtil.DecryptParameter(param); }
        //private string encryptParameter(string param) { return WebUtil.EncryptParameter(param); }
        //private string openPopupWindowFunction = "openPopupWindow";

        const string decimalFormat = "#,##0.00";

        protected void Page_Load(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "FilterInput", "filterInput(" + WebHelper.isInternalITIpAddress().ToString().ToLower() + ");", true);
            if (!Page.IsPostBack)
            {
                if (Request.Params["PaymentId"] == null)
                {
                    return;
                }
                if (int.TryParse(HttpUtility.UrlDecode(EncryptionUtility.DecryptParam(Request.Params["PaymentId"])), out paymentId))
                {
                    vwPaymentId = paymentId;
                    vwDomainAdvancePaymentDef = new DomainAdvancePaymentDef(typeof(AdvancePaymentOrderDetailDef));
                    try
                    {
                        AdvancePaymentDef apDef = AccountManager.Instance.getAdvancePaymentByKey(vwPaymentId);//(AdvancePaymentDef)Session[param_pid + "_EditFabricPayment"];
                        vwDomainAdvancePaymentDef.AdvancePayment = apDef;
                        vwDomainAdvancePaymentDef.OrderDetailList = AccountManager.Instance.getAdvancePaymentOrderDetailList(vwPaymentId); //new List<AdvancePaymentOrderDetailDef>();
                        vwDomainAdvancePaymentDef.BalanceSettlementList = AccountManager.Instance.getAdvancePaymentBalanceSettlementList(vwPaymentId);
                        vwBalances = new List<AdvancePaymentBalanceSettlementDef>(vwDomainAdvancePaymentDef.BalanceSettlementList);
                        lbl_PaymentNo.Text = apDef.PaymentNo;
                        lbl_Office.Text = com.next.common.domain.types.OfficeId.getName(apDef.OfficeId);
                        lbl_Vendor.Text = (apDef.Vendor != null) ? apDef.Vendor.Name : "--";
                        lbl_Currency.Text = apDef.Currency.CurrencyCode;
                        lbl_TotalPaymentAmt.Text = (Math.Abs(apDef.TotalAmount) - Math.Abs(apDef.InterestChargedAmt)).ToString("#,##0.00");
                        txt_InterestChargedAmt.Text = apDef.InterestChargedAmt.ToString();
                        lbl_TotalAmt.Text = Math.Abs(apDef.TotalAmount).ToString("#,##0.00");
                        uclPaymentDate.Text = DateTimeUtility.getDateString(apDef.PaymentDate);
                        txt_InterestRate.Text = apDef.InterestRate.ToString();
                        ViewState["vwPaymentDate"] = apDef.PaymentDate;
                        ViewState["vwInterestRate"] = apDef.InterestRate;
                        ViewState["vwInterestAmt"] = apDef.InterestChargedAmt;
                        lbl_PayableTo.Text = (apDef.PayableTo != null) ? apDef.PayableTo.Trim() : "";
                        lbl_CreatedBy.Text = apDef.CreatedBy.DisplayName;
                        lbl_InitiatedBy.Text = apDef.InitiatedBy.DisplayName;
                        txb_Remark.Text = hf_Remark.Value = apDef.Remark;

                        if (apDef.IsC19 == 1)
                        {
                            lbl_FLRefNo.Text = "(C19 - " + apDef.FLRefNo + ")";
                            lbl_FLRefNo.ForeColor = System.Drawing.Color.Red;
                            lbl_FLRefNo.Font.Bold = true;
                            lbl_FLRefNo.Visible = true;
                        }
                        else
                        {
                            lbl_FLRefNo.Text = string.Empty;
                            lbl_FLRefNo.Visible = false;
                        }


                        // Contract Details
                        loadBalance();
                        loadOrderDetailAndBind();
                        storeExpectedValueInputs();
                        btn_Save.Enabled = true;

                        this.btnLog.Attributes.Add("onclick", "window.open('ViewAdvancePaymentLog.aspx?paymentId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(vwPaymentId.ToString())) + "', 'loglist', 'width=800,height=400,scrollbars=1,status=0');return false;");

                    }
                    catch
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseFabricEdit", "window.close();", true);
                    }
                    finally
                    {
                        Session.Remove(listIndex);
                    }
                }
                else
                {
                    return;
                }

            }

        }

        #region Grid & Data Binding

        private void BindOrderData()
        {
            lbl_balDiff.Text = BalDiff.ToString();
            lbl_totalAmtVariance.Text = TotalAmtVariance.ToString();
            lbl_noRecoveryPlanBalance.Text = NoRecoveryPlanBal.ToString("#,##0.00");
            Page.Header.DataBind();
        }

        private void BindOrderDetailGridData(List<AdvancePaymentOrderDetailDef> list)
        {
            gv_ContractDetails.DataSource = list;
            gv_ContractDetails.DataBind();
            gv_ContractDetails.FooterRow.Cells[8].Text = Math.Abs(TotalActualValue).ToString(decimalFormat);
        }

        private void storeExpectedValueInputs()
        {
            List<string> newExpectedValueList = new List<string>(gv_ContractDetails.Rows.Count);
            foreach (GridViewRow row in gv_ContractDetails.Rows)
            {
                var txt_ExpectedValue = (TextBox)row.FindControl("txt_ExpectedValue");
                newExpectedValueList.Add(txt_ExpectedValue.Text.Trim());
            }
            vwTxt_ExpectedValueList = newExpectedValueList;
        }

        private void loadOrderDetailAndBind()
        {
            vwAdvancePaymentOrderDetailDefList0 = new List<AdvancePaymentOrderDetailDef>(vwDomainAdvancePaymentDef.OrderDetailList);
            BindOrderData();
            BindOrderDetailGridData(vwDomainAdvancePaymentDef.OrderDetailList);
        }

        private void loadBalance()
        {
            rep_BalanceDetail.DataSource = vwBalances;
            rep_BalanceDetail.DataBind();
        }

        /// <summary>
        /// Customization of GridView presentation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_ContractDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //TableCell aa = this.gv_ContractDetails.Rows[e.Row.RowIndex].Cells[e.];
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int RowIndex = e.Row.RowIndex;
                ImageButton ibtn_Delete = (ImageButton)e.Row.FindControl("ibtn_Delete");
                Label lbl_NewRecord = (Label)e.Row.FindControl("lbl_NewRecord");
                LinkButton btn_ContractNo = (LinkButton)e.Row.FindControl("btn_ContractNo");
                Label lbl_ItemNo = (Label)e.Row.FindControl("lbl_ItemNo");
                Label lbl_ProductTeam = (Label)e.Row.FindControl("lbl_ProductTeam");
                Label lbl_PaymentTerm = (Label)e.Row.FindControl("lbl_PaymentTerm");
                Label lbl_LCBillRefNo = (Label)e.Row.FindControl("lbl_LCBillRefNo");
                Label lbl_ExpectedValue = (Label)e.Row.FindControl("lbl_ExpectedValue");
                TextBox txt_ExpectedValue = (TextBox)e.Row.FindControl("txt_ExpectedValue");
                Label lbl_ActualValue = (Label)e.Row.FindControl("lbl_ActualValue");
                Label lbl_WorkflowStatus = (Label)e.Row.FindControl("lbl_WorkflowStatus");
                Label lbl_SettlementDate = (Label)e.Row.FindControl("lbl_SettlementDate");
                SmartCalendar txt_SettlementDate = (SmartCalendar)e.Row.FindControl("txt_SettlementDate");
                Label lbl_LCSettlementDate = (Label)e.Row.FindControl("lbl_LCSettlementDate");

                AdvancePaymentOrderDetailDef apOrderDetailDef = (AdvancePaymentOrderDetailDef)vwDomainAdvancePaymentDef.OrderDetailList[RowIndex];
                bool isInitial = apOrderDetailDef.IsInitial;
                lbl_ExpectedValue.Visible = false;
                txt_ExpectedValue.Visible = true;
                lbl_SettlementDate.Visible = false;
                txt_SettlementDate.Visible = true;

                ((Label)e.Row.FindControl("lbl_Seq")).Text = (e.Row.RowIndex + 1).ToString();

                if (apOrderDetailDef.Status == 0) // New item to be insert 
                {
                    lbl_NewRecord.Visible = true;
                }
                else if (apOrderDetailDef.IsInitial == false)
                {
                    ibtn_Delete.Visible = true;
                    ibtn_Delete.CommandArgument = RowIndex.ToString();
                    ibtn_Delete.CommandName = "DeleteContract";
                    //ibtn_Delete.Command += new CommandEventHandler(this.CommandBtn_Click);
                }

                int shipmentId = apOrderDetailDef.ShipmentId;
                DomainShipmentDef domainItem = ShipmentManager.Instance.getDomainShipmentDef(shipmentId);

                // Obtains related object for data
                ContractDef contract = domainItem.Contract;
                ProductDef product = domainItem.Product;
                ShipmentDef shipment = domainItem.Shipment;
                InvoiceDef invoice = domainItem.Invoice;

                btn_ContractNo.Text = (contract.ContractNo + "-" + shipment.DeliveryNo);
                btn_ContractNo.Attributes.Add("OnClick", openPopupWindowFunction + "('../Shipping/ShipmentDetail.aspx?ShipmentId=" + encryptParameter(shipment.ShipmentId.ToString()) + "&DefaultReceiptDate=" + encryptParameter(DateTimeUtility.getDateString(DateTime.Today)) + "', 'ShipmentDetail', 'width=800,height=600,scrollbars=1,resizable=1,status=1'); return false;");

                lbl_ItemNo.Text = product.ItemNo;
                lbl_ProductTeam.Text = (contract.ProductTeam != null) ? contract.ProductTeam.CodeDescription : "";
                lbl_PaymentTerm.Text = (shipment.PaymentTerm != null) ? shipment.PaymentTerm.PaymentTermDescription : "";
                lbl_LCBillRefNo.Text = invoice.LCBillRefNo;
                lbl_ExpectedValue.Text = Math.Abs(apOrderDetailDef.ExpectedValue).ToString(decimalFormat);
                string strExpectedValue = getExpectedValueInput(RowIndex);
                if (strExpectedValue != null)
                {
                    txt_ExpectedValue.Text = strExpectedValue;
                }
                else
                {
                    txt_ExpectedValue.Text = lbl_ExpectedValue.Text;
                }
                setExpectedValueInput(RowIndex, txt_ExpectedValue.Text);
                //txt_ExpectedValue.Attributes.Add("onchange", "updateBalanceDiff(this);");

                lbl_ActualValue.Text = Math.Abs(apOrderDetailDef.ActualValue).ToString(decimalFormat);
                lbl_WorkflowStatus.Text = shipment.WorkflowStatus.Name;
                bool haveLCSettlementDate = (invoice.APDate != DateTime.MinValue);
                bool haveSettlementDate = (apOrderDetailDef.SettlementDate != DateTime.MinValue);
                lbl_SettlementDate.Text = (haveSettlementDate) ? DateTimeUtility.getDateString(apOrderDetailDef.SettlementDate) : "N/A";
                lbl_LCSettlementDate.Text = (haveLCSettlementDate) ? DateTimeUtility.getDateString(invoice.APDate) : "N/A";
                txt_SettlementDate.Text = (haveSettlementDate) ? DateTimeUtility.getDateString(apOrderDetailDef.SettlementDate) : string.Empty;

                // Row Bg color handling
                string rowCssClass = "";
                if (ContractWFS.CANCELLED == shipment.WorkflowStatus)
                { //* When WorkflowStatus == CANCELLD, grey out the line
                    rowCssClass = "WorkflowCancelled";
                }
                else
                {
                    if (haveSettlementDate && apOrderDetailDef.ActualValue != 0 && apOrderDetailDef.ExpectedValue != apOrderDetailDef.ActualValue)
                    {
                        rowCssClass = "Settled";
                    }
                }
                e.Row.CssClass += "row " + rowCssClass;
                e.Row.CssClass = e.Row.CssClass.TrimEnd();
            }
        }

        #endregion

        protected void rep_BalanceDetail_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                AdvancePaymentBalanceSettlementDef balanceDetail = (AdvancePaymentBalanceSettlementDef)e.Item.DataItem;
                if (balanceDetail.Status == 0)
                    e.Item.Visible = false;
                /*
                if (balanceDetail.ExpectedAmount == 0)
                    ((TextBox)e.Item.FindControl("txtExpectedAmt")).Text = string.Empty;
                if (balanceDetail.PaymentAmount == 0)
                    ((TextBox)e.Item.FindControl("txtDeductionAmt")).Text = string.Empty;
                */
                ((Label)e.Item.FindControl("lbl_Seq")).Text = (e.Item.ItemIndex + 1).ToString();
                if (balanceDetail.PaymentDate != DateTime.MinValue)
                    ((SmartCalendar)e.Item.FindControl("txtExpectedDate")).DateTime = balanceDetail.PaymentDate;
                if (balanceDetail.SettlementDate != DateTime.MinValue)
                    ((SmartCalendar)e.Item.FindControl("txtDeductionDate")).DateTime = balanceDetail.SettlementDate;
            }

            int count = 0;
            AdvancePaymentBalanceSettlementDef balanceDef;
            foreach (RepeaterItem item in rep_BalanceDetail.Items)
            {
                balanceDef = vwBalances[item.ItemIndex];
                if (balanceDef.Status == 1)
                    count++;
            }
            if (rep_BalanceDetail.Items.Count < 1 || count == 0)
            {
                if (e.Item.ItemType == ListItemType.Footer)
                {
                    Label lblNoData = (Label)e.Item.FindControl("lblNoData");
                    lblNoData.Visible = true;
                }
            }

        }

        #region Button Event

        /// <summary>
        /// To add new contract(s) with opening new page window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_OpenContract_Click(object sender, EventArgs e)
        {
            //cv_ExpectedValue.Validate();
            if (Page.IsValid)
            {
                //storeExpectedValueInputs();
                Session[listIndex] = vwDomainAdvancePaymentDef.OrderDetailList;
                string jsWindowOpenURL = String.Format("AddContract.aspx?PaymentId={0}", HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(vwPaymentId.ToString())));
                string popupName = listIndex;
                string jsFunction0 = String.Format(@"
function contractListOpen(popup) {{
    if (popup == null) {{
        popup = window.open('{0}', '{1}', 'width=800,height=600, scrollbars=1,resizable=1,status=1');
    }}
    popup.focus();
    return popup;
}}
", jsWindowOpenURL, popupName);
                string script0 = jsFunction0;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AddContract", script0, true);
                BindOrderData();
            }
        }

        /// <summary>
        /// Reload any new added contract(s) into this parent page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_RefreshContract_Click(object sender, EventArgs e)
        {
            reloadAdvancePaymentOrderDetail();
        }
        private void reloadAdvancePaymentOrderDetail()
        {
            if (Session[listIndex] != null)
            {
                storeExpectedValueInputs();
                List<AdvancePaymentOrderDetailDef> newItemList = (List<AdvancePaymentOrderDetailDef>)Session[listIndex];
                int List0Cnt = vwAdvancePaymentOrderDetailDefList0.Count;
                // If new add Def is Duplicate
                for (int n = 0; n < newItemList.Count; n++)
                {
                    bool pass = isContractDuplicationFree(n, newItemList, vwAdvancePaymentOrderDetailDefList0);
                    if (!pass)
                    {
                        newItemList.RemoveAt(n);
                    }
                }

                // Renew vwTxt_ExpectedValueList
                List<string> newAddExpectedValueList = new List<string>();
                List<AdvancePaymentOrderDetailDef> pendingOrderDetailList = new List<AdvancePaymentOrderDetailDef>();
                for (int i = List0Cnt; i < vwDomainAdvancePaymentDef.OrderDetailList.Count; i++)
                {
                    AdvancePaymentOrderDetailDef pendingDef = vwDomainAdvancePaymentDef.OrderDetailList[i];
                    foreach (AdvancePaymentOrderDetailDef newDef in newItemList) // Handle for new add Def
                    {
                        if (pendingDef.ShipmentId == newDef.ShipmentId)
                        {
                            string ExpectedValue = getExpectedValueInput(i);
                            newAddExpectedValueList.Add(ExpectedValue);
                            pendingOrderDetailList.Add(newDef);
                            newItemList.Remove(newDef);
                            break;
                        }
                    }
                }
                if (List0Cnt > 0)
                {
                    vwTxt_ExpectedValueList = vwTxt_ExpectedValueList.GetRange(0, List0Cnt);
                }
                if (newAddExpectedValueList.Count > 0)
                {
                    vwTxt_ExpectedValueList.AddRange(newAddExpectedValueList);
                }

                // Re-arrange the contract list in form of GridView
                vwDomainAdvancePaymentDef.OrderDetailList = new List<AdvancePaymentOrderDetailDef>(vwAdvancePaymentOrderDetailDefList0);
                if (pendingOrderDetailList.Count > 0)
                {
                    vwDomainAdvancePaymentDef.OrderDetailList.AddRange(pendingOrderDetailList);
                }
                if (newItemList.Count > 0)
                {
                    vwDomainAdvancePaymentDef.OrderDetailList.AddRange(newItemList);
                }
                BindOrderData();
                BindOrderDetailGridData(vwDomainAdvancePaymentDef.OrderDetailList);
                Session.Remove(listIndex);
            }

        }

        protected void btn_AddFirstBalance_Click(object sender, EventArgs e)
        {
            vwBalances.Add(new AdvancePaymentBalanceSettlementDef());
            rep_BalanceDetail.DataSource = vwBalances;
            rep_BalanceDetail.DataBind();
        }

        protected void btn_Save_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && Context.Request.HttpMethod == "POST")
            {
                saveAdvancePaymentAndOrderDetail();
            }
        }
        private void saveAdvancePaymentAndOrderDetail()
        {
            List<AdvancePaymentActionHistoryDef> historys = new List<AdvancePaymentActionHistoryDef>();

            // Payment date
            //DateTimeUtility.getDateString(apDef.PaymentDate);
            DateTime newPaymentDate = uclPaymentDate.DateTime;
            string strNewPaymentDate = uclPaymentDate.Text.Trim();

            uclPaymentDate.validate();
            if (uclPaymentDate.isValid || uclPaymentDate.DateTime == DateTime.MinValue)
            {
                vwDomainAdvancePaymentDef.AdvancePayment.PaymentDate = newPaymentDate;
                DateTime oldPaymentDate = DateTime.MinValue;
                if (typeof(DateTime) == ViewState["vwPaymentDate"].GetType())
                {
                    oldPaymentDate = (DateTime)ViewState["vwPaymentDate"];
                }
                if (oldPaymentDate != newPaymentDate)
                {
                    historys.Add(NewAdvancePaymentActionHistory(vwPaymentId, "PAYMENTDATE '" + DateTimeUtility.getDateString(oldPaymentDate) + "' --> '" + strNewPaymentDate + "'"));
                    ViewState["vwPaymentDate"] = newPaymentDate;
                }
            }

            decimal interestAmt = 0;
            if (decimal.TryParse(this.txt_InterestChargedAmt.Text.Trim(), out interestAmt))
            {
                if (Decimal.Parse(interestAmt.ToString()) != (Decimal)ViewState["vwInterestAmt"])
                {
                    historys.Add(NewAdvancePaymentActionHistory(vwPaymentId, "INTEREST AMT '" + ((Decimal)ViewState["vwInterestAmt"]).ToString() + "' --> '" + interestAmt.ToString() + "'"));
                    ViewState["vwInterestAmt"] = interestAmt;
                }
            }
            vwDomainAdvancePaymentDef.AdvancePayment.InterestChargedAmt = interestAmt;

            decimal interestRate = 0;
            if (decimal.TryParse(txt_InterestRate.Text.Trim(), out interestRate))
            {
                if (Decimal.Parse(interestRate.ToString()) != (Decimal)ViewState["vwInterestRate"])
                {
                    historys.Add(NewAdvancePaymentActionHistory(vwPaymentId, "INTEREST RATE % '" + ((Decimal)ViewState["vwInterestRate"]).ToString() + "' --> '" + interestRate.ToString() + "'"));
                    ViewState["vwInterestRate"] = interestRate;
                }
            }
            vwDomainAdvancePaymentDef.AdvancePayment.InterestRate = interestRate;

            vwDomainAdvancePaymentDef.AdvancePayment.TotalAmount = Decimal.Parse(this.lbl_TotalPaymentAmt.Text) + interestAmt;

            // Remark
            string newRemark = txb_Remark.Text.Trim();
            vwDomainAdvancePaymentDef.AdvancePayment.Remark = newRemark;
            /*
            if (newRemark != hf_Remark.Value)
            {
                historys.Add(NewAdvancePaymentActionHistory(vwPaymentId, "REMARK '" + hf_Remark.Value + "' --> '" + newRemark + "'"));
            }
            */

            // AdvancePaymentOrderDetail List
            if (vwDomainAdvancePaymentDef.OrderDetailList.Count > 0)
            {
                for (int r = 0; r < gv_ContractDetails.Rows.Count; r++)
                {
                    AdvancePaymentOrderDetailDef currentItem = vwDomainAdvancePaymentDef.OrderDetailList[r];
                    decimal expectedValue = currentItem.ExpectedValue;
                    // Suppose validation passes (cv_ExpectedValue_ServerValidate)
                    TextBox txt_ExpectedValue = (TextBox)gv_ContractDetails.Rows[r].FindControl("txt_ExpectedValue");
                    decimal newExpectedValue = Decimal.Parse(txt_ExpectedValue.Text);
                    currentItem.ExpectedValue = newExpectedValue;
                    SmartCalendar txt_SettlementDate = (SmartCalendar)gv_ContractDetails.Rows[r].FindControl("txt_SettlementDate");
                    string settlementDate = DateTimeUtility.getDateString(currentItem.SettlementDate);
                    string newsettlementDate = txt_SettlementDate.Text.Trim();

                    if (currentItem.Status == 0)
                    { // For Creation log
                        currentItem.Status = 1;
                        currentItem.ActualValue = newExpectedValue;
                        historys.Add(NewAdvancePaymentActionHistory(vwPaymentId, "NEW ADD SHIPMENT '" + currentItem.ShipmentId + "', EXPECTEDVALUE '" + newExpectedValue + "'" + "', ACTUALVALUE '" + newExpectedValue + "'"));
                    }
                    else
                    {
                        if (newExpectedValue != expectedValue)
                        { // For Change log
                            historys.Add(NewAdvancePaymentActionHistory(vwPaymentId, String.Format("MODIFIED EXPECTEDVALUE OF SHIPMENT {0} '{1}' --> '{2}' ", currentItem.ShipmentId, expectedValue, newExpectedValue)));  //"Modified ExpectedValue of shipment'" + currentItem.ShipmentId + "' --> " + newExpectedValue)
                        }
                        else if (settlementDate != newsettlementDate)
                        {
                            string[] newsettlementDateSplit = newsettlementDate.Split('/');
                            if (newsettlementDate.Trim() != string.Empty)
                                currentItem.SettlementDate = new DateTime(int.Parse(newsettlementDateSplit[2]), int.Parse(newsettlementDateSplit[1]), int.Parse(newsettlementDateSplit[0]));
                            else
                            {
                                currentItem.SettlementDate = DateTime.MinValue;
                            }
                            historys.Add(NewAdvancePaymentActionHistory(vwPaymentId, String.Format("MODIFIED SETTLEMENTDATE OF SHIPMENT {0} '{1}' --> '{2}' ", currentItem.ShipmentId, settlementDate, newsettlementDate)));  //"Modified ExpectedValue of shipment'" + currentItem.ShipmentId + "' --> " + newExpectedValue)
                        }
                    }
                }
            }

            updateBalanceDetail();

            List<AdvancePaymentBalanceSettlementDef> balanceModified = new List<AdvancePaymentBalanceSettlementDef>();
            List<AdvancePaymentBalanceSettlementDef> oriBalance = AccountManager.Instance.getAdvancePaymentBalanceSettlementList(vwDomainAdvancePaymentDef.AdvancePayment.PaymentId);
            foreach (AdvancePaymentBalanceSettlementDef balanceSin in vwBalances)
            {
                if (balanceSin.PaymentDate != DateTime.MinValue)
                {
                    AdvancePaymentBalanceSettlementDef searchResult = AccountManager.Instance.getAdvancePaymentBalanceSettlementByKey(balanceSin.PaymentId, balanceSin.PaymentDate);
                    if (searchResult != null)
                    {
                        if (balanceSin.Status == 0 && searchResult.Status == 1)
                        {
                            balanceModified.Add(balanceSin);
                            historys.Add(NewAdvancePaymentActionHistory(vwDomainAdvancePaymentDef.AdvancePayment.PaymentId, "REMOVE BALANCE: " + DateTimeUtility.getDateString(balanceSin.PaymentDate) + " WITH EXPECTED/ACTUAL AMT " + Decimal.Round(searchResult.ExpectedAmount, 2) + "/" + Decimal.Round(searchResult.PaymentAmount, 2)));
                        }
                        else if (balanceSin.Status == 1 && searchResult.Status == 0)
                        {
                            balanceModified.Add(balanceSin);
                            historys.Add(NewAdvancePaymentActionHistory(vwDomainAdvancePaymentDef.AdvancePayment.PaymentId, "RECOVER BALANCE: " + DateTimeUtility.getDateString(balanceSin.PaymentDate) + " WITH EXPECTED/ACTUAL AMT " + Decimal.Round(balanceSin.ExpectedAmount, 2) + "/" + Decimal.Round(balanceSin.PaymentAmount, 2)));
                        }
                        else if (balanceSin.Status == searchResult.Status)
                        {
                            int rowchange = 0;
                            if (balanceSin.ExpectedAmount != searchResult.ExpectedAmount || balanceSin.PaymentAmount != searchResult.PaymentAmount)
                            {
                                historys.Add(NewAdvancePaymentActionHistory(vwDomainAdvancePaymentDef.AdvancePayment.PaymentId, "MODIFY BALANCE: " + DateTimeUtility.getDateString(balanceSin.PaymentDate) + " WITH EXPECTED/ACTUAL AMT " + Decimal.Round(searchResult.ExpectedAmount, 2) + "/" + Decimal.Round(searchResult.PaymentAmount, 2) + " --> " + Decimal.Round(balanceSin.ExpectedAmount, 2) + "/" + Decimal.Round(balanceSin.PaymentAmount, 2)));
                                rowchange = 1;
                            }
                            if (balanceSin.Remark != searchResult.Remark)
                            {
                                historys.Add(NewAdvancePaymentActionHistory(vwDomainAdvancePaymentDef.AdvancePayment.PaymentId, "MODIFY BALANCE: " + DateTimeUtility.getDateString(balanceSin.PaymentDate) + " WITH REMARK '" + searchResult.Remark + "'" + " --> '" + balanceSin.Remark + "'"));
                                rowchange = 1;
                            }
                            if (balanceSin.SettlementDate != searchResult.SettlementDate)
                            {
                                historys.Add(NewAdvancePaymentActionHistory(vwDomainAdvancePaymentDef.AdvancePayment.PaymentId, "MODIFY BALANCE: " + DateTimeUtility.getDateString(balanceSin.PaymentDate) + " WITH DEDUCTION DATE '" + DateTimeUtility.getDateString(searchResult.SettlementDate) + "'" + " --> '" + DateTimeUtility.getDateString(balanceSin.SettlementDate) + "'"));
                                rowchange = 1;
                            }

                            if (rowchange == 1)
                            {
                                balanceModified.Add(balanceSin);
                            }
                        }
                    }
                    else
                    {
                        balanceModified.Add(balanceSin);
                        historys.Add(NewAdvancePaymentActionHistory(vwDomainAdvancePaymentDef.AdvancePayment.PaymentId, "ADD BALANCE: " + DateTimeUtility.getDateString(balanceSin.PaymentDate) + " WITH EXPECTED/ACTUAL AMT " + Decimal.Round(balanceSin.ExpectedAmount, 2) + "/" + Decimal.Round(balanceSin.PaymentAmount, 2)));
                    }
                }
            }
            foreach (AdvancePaymentBalanceSettlementDef balanceSin in oriBalance)
            {
                AdvancePaymentBalanceSettlementDef searchResult = vwBalances.Find(item => item.PaymentDate == balanceSin.PaymentDate);
                if (searchResult == null)
                {
                    balanceSin.Status = 0;
                    balanceModified.Add(balanceSin);
                    historys.Add(NewAdvancePaymentActionHistory(vwDomainAdvancePaymentDef.AdvancePayment.PaymentId, "REMOVE BALANCE: " + DateTimeUtility.getDateString(balanceSin.PaymentDate) + " WITH EXPECTED/ACTUAL AMT " + Decimal.Round(balanceSin.ExpectedAmount, 2) + "/" + Decimal.Round(balanceSin.PaymentAmount, 2)));
                }
            }

            string savenewMess = "Save Successfully!";
            try
            {
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateAdvancePaymentAndOrderDetail);
                Context.Items.Add(AccountCommander.Param.advancePayment, vwDomainAdvancePaymentDef.AdvancePayment);
                Context.Items.Add(AccountCommander.Param.advancePaymentOrderDetailList, vwDomainAdvancePaymentDef.OrderDetailList);
                Context.Items.Add(AccountCommander.Param.advancePaymentBalanceSettlement, balanceModified);
                Context.Items.Add(AccountCommander.Param.paymenthistory, historys);
                forwardToScreen(null);
                AdvancePaymentDef apDef = AccountManager.Instance.getAdvancePaymentByKey(vwPaymentId);
                vwDomainAdvancePaymentDef.AdvancePayment = apDef;
                vwDomainAdvancePaymentDef.OrderDetailList = AccountManager.Instance.getAdvancePaymentOrderDetailList(vwPaymentId);
                vwTxt_ExpectedValueList.Clear();
                loadOrderDetailAndBind();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "save", "alert('" + savenewMess + "');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "save", "alert('Error occurred. Please try again later.');", true);
            }
        }

        protected void btnClosedContract_Click(object sender, EventArgs e)
        {
            Session.Remove(listIndex);
        }

        protected void btnAttachment_Click(object sender, EventArgs e)
        {
            string escapeValue = vwPaymentId.ToString(); // HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(vwPaymentId.ToString()));
            string targetLocation = "InstalmentAttachmentList.aspx?PaymentId=" + escapeValue;
            string targetName = "viewDoc";
            com.next.infra.util.ClientScript.windowOpen(targetLocation, targetName, 420, 300, false, true, false, false, true, Page);
        }

        protected void ibtn_Delete_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "DeleteContract")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                deleteAdvancePaymentOrderDetail(index);
            }
        }
        private void deleteAdvancePaymentOrderDetail(int index)
        {
            AdvancePaymentOrderDetailDef removeDef = vwDomainAdvancePaymentDef.OrderDetailList[index];
            if (removeDef != null && !removeDef.IsInitial)
            {
                try
                {

                    AdvancePaymentActionHistoryDef history = NewAdvancePaymentActionHistory(vwPaymentId, "REMOVE SHIPMENT '" + removeDef.ShipmentId + "'");
                    Context.Items.Clear();
                    Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                    Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.DeleteAdvancePaymentOrderDetail);
                    Context.Items.Add(AccountCommander.Param.paymentId, vwPaymentId);//*
                    Context.Items.Add(AccountCommander.Param.shipmentId, removeDef.ShipmentId);//*
                    Context.Items.Add(AccountCommander.Param.paymenthistory, history);
                    forwardToScreen(null);

                    // Update the list for remove immediately                    
                    for (int i = 0; i < vwDomainAdvancePaymentDef.OrderDetailList.Count; i++)
                    {
                        AdvancePaymentOrderDetailDef targetDef = vwDomainAdvancePaymentDef.OrderDetailList[i];
                        if (removeDef.ShipmentId == targetDef.ShipmentId)
                        {
                            vwDomainAdvancePaymentDef.OrderDetailList.RemoveAt(i);
                            vwAdvancePaymentOrderDetailDefList0.RemoveAt(i);
                            storeExpectedValueInputs();
                            vwTxt_ExpectedValueList.RemoveAt(i);
                            BindOrderData();
                            BindOrderDetailGridData(vwDomainAdvancePaymentDef.OrderDetailList);
                            break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "deleteContract", "alert('Error occurred. Please try again later.');", true);
                }

            }
        }

        #endregion

        protected void btn_AddBalance_Click(object sender, EventArgs e)
        {
            updateBalanceDetail();

            vwBalances.Add(new AdvancePaymentBalanceSettlementDef());
            loadBalance();
        }

        protected void btn_RemoveBalance_Click(object sender, EventArgs e)
        {
            int index = Convert.ToInt32(((ImageButton)sender).CommandArgument);
            vwBalances[index].Status = 0;
            //rep_BalanceDetail.Items[index].Visible = false;
            loadBalance();
        }

        private AdvancePaymentActionHistoryDef NewAdvancePaymentActionHistory(int paymentId, string description)
        {
            AdvancePaymentActionHistoryDef newhistory = new AdvancePaymentActionHistoryDef();
            newhistory.PaymentId = paymentId;
            newhistory.Description = description;
            newhistory.ActionBy = this.LogonUserId;
            newhistory.ActionOn = DateTime.Now;
            newhistory.Status = 1;
            return newhistory;
        }

        protected void updateBalanceDetail()
        {
            AdvancePaymentBalanceSettlementDef balanceDef;
            foreach (RepeaterItem item in rep_BalanceDetail.Items)
            {
                balanceDef = vwBalances[item.ItemIndex];
                if (vwDomainAdvancePaymentDef != null)
                {
                    if (vwDomainAdvancePaymentDef.AdvancePayment.PaymentId != 0)
                    {
                        balanceDef.PaymentId = vwDomainAdvancePaymentDef.AdvancePayment.PaymentId;
                    }
                }
                SmartCalendar payDate = ((SmartCalendar)item.FindControl("txtExpectedDate"));
                balanceDef.PaymentDate = new DateTime(payDate.DateTime.Year, payDate.DateTime.Month, payDate.DateTime.Day);
                decimal payAmt = 0;
                decimal expectedAmt = 0;
                if (((TextBox)item.FindControl("txtDeductionAmt")).Text.Trim() != "")
                {
                    payAmt = decimal.Parse(((TextBox)item.FindControl("txtDeductionAmt")).Text.Trim());
                }
                balanceDef.PaymentAmount = payAmt;
                if (((TextBox)item.FindControl("txtExpectedAmt")).Text.Trim() != "")
                {
                    expectedAmt = decimal.Parse(((TextBox)item.FindControl("txtExpectedAmt")).Text.Trim());
                }
                balanceDef.ExpectedAmount = expectedAmt;

                string remarkText = ((TextBox)item.FindControl("txtRemark")).Text.Trim();
                if (remarkText != string.Empty)
                {
                    balanceDef.Remark = remarkText;
                }

                SmartCalendar deductDate = ((SmartCalendar)item.FindControl("txtDeductionDate"));
                balanceDef.SettlementDate = new DateTime(deductDate.DateTime.Year, deductDate.DateTime.Month, deductDate.DateTime.Day);

                balanceDef.Status = vwBalances[item.ItemIndex].Status;
                vwBalances[item.ItemIndex] = balanceDef;
            }
        }

        #region Validation
        /// <summary>
        /// Internal List object checking
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cv_ContractDuplication_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = isContractDuplicationFree(vwDomainAdvancePaymentDef.OrderDetailList, vwDomainAdvancePaymentDef.OrderDetailList);
        }

        protected void cv_ExpectedValue_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var rows = gv_ContractDetails.Rows;
            for (int r = 0; r < rows.Count; r++)
            {
                TextBox txt_ExpectedValue = (TextBox)rows[r].FindControl("txt_ExpectedValue");
                txt_ExpectedValue.CssClass = "";
                if (!String.IsNullOrEmpty(txt_ExpectedValue.Text))
                {
                    decimal newExpectedValue;
                    bool parseOk = Decimal.TryParse(txt_ExpectedValue.Text, out newExpectedValue);
                    if (parseOk)
                    {
                        if (newExpectedValue >= 0)
                        {
                            continue;
                        }
                    }
                }
                // invalid
                txt_ExpectedValue.CssClass += "Invalid";
                args.IsValid = false;
            }
        }

        protected void cv_Submit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            cv_ContractDuplication.Validate();
            cv_ExpectedValue.Validate();
            string message = button_start(source);
            if (message != "")
            {
                cv_Submit.ErrorMessage = message;
                args.IsValid = false;
            }
            //cv_ContractDuplication_ServerValidate(source, args);
            //cv_ExpectedValue_ServerValidate(source, args);
            //cv_Submit.Text = "";
        }

        #endregion


        /// <summary>
        /// Perform self comparison accross the whole list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool isContractDuplicationFree(List<AdvancePaymentOrderDetailDef> listA, List<AdvancePaymentOrderDetailDef> listB)
        {
            for (int i = 0; i < listA.Count; i++)
            {
                bool result = isContractDuplicationFree(i, listA, listB);
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Perform comparison for a contract in specific index with the whole list
        /// </summary>
        /// <param name="index">Specific index of contract</param>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool isContractDuplicationFree(int index, List<AdvancePaymentOrderDetailDef> listA, List<AdvancePaymentOrderDetailDef> listB)
        {
            int shipmentId = listA[index].ShipmentId;
            for (int j = 0; j < listB.Count; j++)
            {
                if (listA == listB && j == index)
                {
                    continue; // To exclude the same object
                }
                int targetShipmentId = listB[j].ShipmentId;

                if (shipmentId == targetShipmentId)
                {
                    return false;
                }

            }
            return true;
        }

        /// <summary>
        /// Retrieve the sum of Expected Values from input fields
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private decimal retreiveTotalExpectedValue(List<string> list)
        {
            decimal totalExpectedValue = 0;
            foreach (string strVal in list) // latestTotalExpectedValue is now depending on user input(field)
            {
                decimal temp = 0;
                if (decimal.TryParse(strVal, out temp))
                {
                    totalExpectedValue += temp;
                }
            }
            return totalExpectedValue;
        }



        private string button_start(object sender)
        {
            bool error = false;
            string newLine = "<br/>";
            string message = "";

            decimal total = vwDomainAdvancePaymentDef.AdvancePayment.TotalAmount - vwDomainAdvancePaymentDef.AdvancePayment.InterestChargedAmt;
            AdvancePaymentBalanceSettlementDef balanceDef;
            int i = 0;
            decimal totalCal = 0;
            List<DateTime> paymentDates = new List<DateTime>();
            foreach (GridViewRow row in gv_ContractDetails.Rows)
            {
                TextBox txt_ExpectedValue = (TextBox)row.FindControl("txt_ExpectedValue");
                string fabricCostSingle = txt_ExpectedValue.Text.Trim();
                totalCal += decimal.Parse(fabricCostSingle);
            }

            foreach (RepeaterItem item in rep_BalanceDetail.Items)
            {
                balanceDef = vwBalances[item.ItemIndex];
                if (balanceDef.Status == 1)
                {
                    i++;
                    SmartCalendar payDate = ((SmartCalendar)item.FindControl("txtExpectedDate"));
                    TextBox payDateTextBox = (TextBox)payDate.Controls[0];
                    payDateTextBox.CssClass = "";
                    if (payDate.DateTime == DateTime.MinValue)
                    {
                        message += "Balance Settlement Line " + i + " [Empty Expected Date]" + newLine;
                        payDateTextBox.CssClass = "Invalid";
                        error = true;
                    }
                    else
                    {
                        if (paymentDates.Contains(payDate.DateTime))
                        {
                            message += "Balance Settlement Line " + i + " [Repeated Expected Date]" + newLine;
                            payDateTextBox.CssClass = "Invalid";
                            error = true;
                        }
                        else
                        {
                            paymentDates.Add(payDate.DateTime);
                        }
                    }

                    decimal check_decimal = 0;
                    string singleAmt = string.Empty;
                    TextBox expectedAmt = (TextBox)item.FindControl("txtExpectedAmt");
                    string expectedAmtText = expectedAmt.Text.Trim();
                    expectedAmt.CssClass = "";
                    if (expectedAmtText == "")
                    {
                        message += "Balance Settlement Line " + i + " [Empty Expected Amount]" + newLine;
                        expectedAmt.CssClass = "Invalid";
                        error = true;
                    }
                    else
                    {
                        singleAmt = expectedAmtText;
                        if (!decimal.TryParse(singleAmt, out check_decimal))
                        {
                            message += "Balance Settlement Line " + i + " [Invalid Decimal Format - Expected Amt]" + newLine;
                            expectedAmt.CssClass = "Invalid";
                            error = true;
                        }
                        else
                        {
                            /*
                            if (check_decimal == 0)
                            {
                                message += "Balance Settlement Line " + i + " [Invalid Expected Amount]" + newLine;
                                expectedAmt.CssClass = "Invalid";
                                error = true;
                            }
                            */
                            totalCal += decimal.Parse(singleAmt);
                        }
                    }

                    TextBox deductionAmt = (TextBox)item.FindControl("txtDeductionAmt");
                    string deductionAmtText = deductionAmt.Text.Trim();
                    deductionAmt.CssClass = "";
                    if (deductionAmtText == "")
                    {
                        message += "Balance Settlement Line " + i + " [Empty Deduction Amount]" + newLine;
                        deductionAmt.CssClass = "Invalid";
                        error = true;
                    }
                    else
                    {
                        singleAmt = deductionAmtText;
                        if (!decimal.TryParse(singleAmt, out check_decimal))
                        {
                            message += "Balance Settlement Line " + i + " [Invalid Decimal Format]" + newLine;
                            deductionAmt.CssClass = "Invalid";
                            error = true;
                        }
                        /*
                        else
                        {
                            if (check_decimal == 0)
                            {
                                message += "Balance Settlement Line " + i + " [Invalid Deduction Amount]" + newLine;
                                deductionAmt.CssClass = "Invalid";
                                error = true;
                            }
                        }
                        */
                    }
                }
            }

            decimal interestAmt = 0;
            if (txt_InterestChargedAmt.Text.Trim() != string.Empty)
            {
                if (!decimal.TryParse(txt_InterestChargedAmt.Text.Trim(), out interestAmt))
                {
                    message += "Invalid interest charged amt" + newLine;
                    error = true;
                }

                if (interestAmt < 0)
                {
                    message += "Invalid interest charged amt" + newLine;
                    error = true;
                }
            }

            decimal interestRate = 0;
            if (txt_InterestRate.Text.Trim() != string.Empty)
            {
                if (!decimal.TryParse(txt_InterestRate.Text.Trim(), out interestRate))
                {
                    message += "Invalid interest rate" + newLine;
                    error = true;
                }

                if (interestRate < 0)
                {
                    message += "Invalid interest rate" + newLine;
                    error = true;
                }
            }

            this.lbl_TotalAmt.Text = (total + interestAmt).ToString("#,##0.00");

            if ((total + interestAmt) != totalCal)
            {
                message += "Total amount (included interest charge) is not match with overall payment instalments amount" + newLine;
                error = true;
            }


            if (error == true)
            {
                return message;
            }
            else
            {
                return "";
            }
        }
    }
}
