using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.dms;
using com.next.infra.util;
using com.next.infra.web;
using com.next.infra.smartwebcontrol ;
using com.next.isam.appserver.account;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.types;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.common.security;
using com.next.isam.appserver.order;
using com.next.isam.domain.order;
using com.next.isam.webapp.webservices;
using com.next.common.domain.module;

namespace com.next.isam.webapp.nontrade
{
    public partial class NonTradeInvoice : com.next.isam.webapp.usercontrol.PageTemplate
    {
        DomainNTInvoiceDef vwDomainNTInvoiceDef
        {
            get { return (DomainNTInvoiceDef)ViewState["DomainNTInvoiceDef"]; }
            set { ViewState["DomainNTInvoiceDef"] = value; }
        }

        ArrayList vwNTVendorExpenseTypeList
        {
            get { return (ArrayList)ViewState["vwNTVendorExpenseTypeList"]; }
            set { ViewState["vwNTVendorExpenseTypeList"] = value; }
        }

        long vwDocId
        {
            get { return (long)ViewState["vwDocId"]; }
            set { ViewState["vwDocId"] = value; }
        }

        ArrayList vwInvoiceUploadList
        {
            get { return (ArrayList)ViewState["vwInvoiceUploadList"]; }
            set { ViewState["vwInvoiceUploadList"] = value; }
        }

        decimal totalAmountForCostCenter = 0;
        decimal totalAmountForRecharge = 0;
        decimal totalVATForCostCenter = 0;
        decimal totalVATForRecharge = 0; 
        bool isAccountUser = false;

        string[] carbonEmissionExpenseTypeAccountCode = new string[] { "4102301", "4102303", "4104310", "4104312", "4104336" };
        string[] mileageExpenseTypeAccountCode = new string[] { "4104310", "4104312", "4104336" };
        string[] utilityExpenseTypeAccountCode = new string[] { "4102301", "4102303" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int invoiceId = -1;

                if (Request.Params["InvoiceId"] != null)
                    invoiceId = Convert.ToInt32(Request.Params["InvoiceId"]);

                if (invoiceId == -1)
                {
                    vwDomainNTInvoiceDef = new DomainNTInvoiceDef();
                }
                else
                    vwDomainNTInvoiceDef = NonTradeManager.Instance.getDomainNTInvoiceDef(invoiceId);

                bindData();
            }
            else
                isAccountUser = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.ViewForAccountsUser);
        }

        private void setDefaultApprover()
        {
            ArrayList approverList = NonTradeManager.Instance.getNTApproverListByOfficeId(ConvertUtility.createArrayList(vwDomainNTInvoiceDef.NTInvoice.Office == null ? this.LogonUserHomeOffice : vwDomainNTInvoiceDef.NTInvoice.Office));

            approverList.Sort(new ArrayListHelper.Sorter("ApproverName"));
            ddl_Approver.bindList(approverList, "ApproverName", "ApproverId", (vwDomainNTInvoiceDef.NTInvoice.Approver == null ? "-1" : vwDomainNTInvoiceDef.NTInvoice.Approver.UserId.ToString()), "-- Please select --", "-1");

            if (vwDomainNTInvoiceDef.NTInvoice.InvoiceId == 0)
            {
                UserRef userRef = CommonUtil.getUserByKey(this.LogonUserId);

                bool isPDUser = false;
                isPDUser = (userRef.Department.DepartmentId == 8 ||
                        userRef.Department.DepartmentId == 24 ||
                        userRef.Department.DepartmentId == 25 ||
                        userRef.Department.DepartmentId == 26 ||
                        userRef.Department.DepartmentId == 482 ||
                        userRef.Department.DepartmentId == 483 ||
                        userRef.Department.DepartmentId == 33);

                NTApproverDef approverDef = null;
                bool hasApprover = false;

                for (int i = 0; i < approverList.Count; i++)
                {
                    approverDef = (NTApproverDef)approverList[i];
                    if (approverDef.Approver.Department.DepartmentId == userRef.Department.DepartmentId)
                    {
                        hasApprover = true;
                        ddl_Approver.SelectedIndex = -1;
                        ddl_Approver.Items[i + 1].Selected = true;
                        break;
                    }
                }

                if (!hasApprover)
                {
                    for (int i = 0; i < approverList.Count; i++)
                    {
                        approverDef = (NTApproverDef)approverList[i];
                        if (approverDef.Approver.Department.DepartmentId == userRef.Department.ParentId)
                        {
                            ddl_Approver.SelectedIndex = -1;
                            ddl_Approver.Items[i + 1].Selected = true;
                            break;
                        }
                    }
                }

                if (isPDUser)
                {
                    this.ddl_Approver.selectByValue("2002");  // lucy hunt
                    /*
                    this.ddl_Approver.Enabled = false;
                    */
                }

                if (this.LogonUserId == 31475) //Prabodha
                    this.ddl_Approver.selectByValue("89799");  // K. G. S. Sithari Silva

            }
        }

        private void bindData()
        {
            txt_SupplierName.setWidth(305);
            txt_SupplierName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.ntVendor);

            for (int i = -1; i <= 6; i++)
            {
                ddl_PaymentYearFrom.Items.Add((DateTime.Today.Year + i).ToString());
                ddl_PaymentYearTo.Items.Add((DateTime.Today.Year + i).ToString());
            }

            isAccountUser = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.ViewForAccountsUser);

            if (vwDomainNTInvoiceDef.NTInvoice == null) //new invoice
            {
                vwDomainNTInvoiceDef.NTInvoice = new NTInvoiceDef();
                vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus = NTWFS.DRAFT;
                vwDomainNTInvoiceDef.NTInvoiceDetailList = ConvertUtility.createArrayList(new NTInvoiceDetailDef(NTInvoiceDetailType.COSTCENTER));
                //vwDomainNTInvoiceDef.NTRechargeDetailList = ConvertUtility.createArrayList(new NTInvoiceDetailDef());

                (ddl_PaymentMonthFrom.Items.FindByValue(DateTime.Now.Month.ToString())).Selected = true;
                (ddl_PaymentMonthTo.Items.FindByValue(DateTime.Now.Month.ToString())).Selected = true;
                (ddl_PaymentYearFrom.Items.FindByValue(DateTime.Now.Year.ToString())).Selected = true;
                (ddl_PaymentYearTo.Items.FindByValue(DateTime.Now.Year.ToString())).Selected = true;

                ddl_Office.bindList(WebUtil.getNTOfficeList(this.LogonUserId), "OfficeCode", "OfficeId", this.LogonUserHomeOffice.OfficeId.ToString());

                //ddl_Department.bindList(CommonUtil.getDepartmentList(this.LogonUserHomeOffice.OfficeId), "Description", "DepartmentId", CommonUtil.getUserByKey(this.LogonUserId).Department.DepartmentId.ToString());
                //ddl_ExpenseType.bindList(WebUtil.getNonTradeExpenseTypeList(), "ExpenseType", "ExpenseTypeId", "", "-- Please select --", GeneralCriteria.ALL.ToString());
                ddl_Currency.bindList(WebUtil.getEffectiveCurrencyList(), "CurrencyCode", "CurrencyId");
                
                txt_InvoiceReceivedDate.Text = DateTimeUtility.getDateString(DateTime.Today);

                //vwNTVendorExpenseTypeList = WebUtil.getNTExpenseTypeListByOfficeId(this.LogonUserHomeOffice.OfficeId);
                vwNTVendorExpenseTypeList = WebUtil.getNTExpenseTypeListByOfficeId(int.Parse(ddl_Office.SelectedValue));
                hf_CalcTotalAmt.Value = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.CalculateTotalAmount) ? "1" : "0";
            }
            else
            {
                txt_Status.Text = vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Name;

                ddl_Office.bindList(WebUtil.getNTOfficeList(this.LogonUserId), "OfficeCode", "OfficeId", vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId.ToString());
                //ddl_Department.bindList(CommonUtil.getDepartmentList(vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId), "Description", "DepartmentId", vwDomainNTInvoiceDef.NTInvoice.Dept.DepartmentId.ToString());
                txt_NSLRefNo.Text = vwDomainNTInvoiceDef.NTInvoice.NSLInvoiceNo;
                txt_SupplierName.NTVendorId = vwDomainNTInvoiceDef.NTInvoice.NTVendor.NTVendorId;
                hf_PaymentTermDays.Value = vwDomainNTInvoiceDef.NTInvoice.NTVendor.PaymentTermDays.ToString();
                ddl_DocumentType.SelectedIndex = vwDomainNTInvoiceDef.NTInvoice.DCIndicator == "D" ? 0 : 1;
                if (vwDomainNTInvoiceDef.NTInvoice.NTVendor.PaymentTermDays != 0)
                    hf_PaymentTermDays.Value = vwDomainNTInvoiceDef.NTInvoice.NTVendor.PaymentTermDays.ToString();
                //ddl_ExpenseType.bindList(WebUtil.getNonTradeExpenseTypeList(), "ExpenseType", "ExpenseTypeId", vwDomainNTInvoiceDef.NTInvoice.ExpenseType.ExpenseTypeId.ToString());
                txt_InvoiceNo.Text = vwDomainNTInvoiceDef.NTInvoice.InvoiceNo;
                txt_CustomerNo.Text = vwDomainNTInvoiceDef.NTInvoice.CustomerNo;
                txt_InvoiceDate.Text = DateTimeUtility.getDateString(vwDomainNTInvoiceDef.NTInvoice.InvoiceDate);
                txt_DueDate.Text = vwDomainNTInvoiceDef.NTInvoice.DueDate == DateTime.MinValue ? string.Empty : DateTimeUtility.getDateString(vwDomainNTInvoiceDef.NTInvoice.DueDate);
                ddl_Currency.bindList(WebUtil.getEffectiveCurrencyList(), "CurrencyCode", "CurrencyId", vwDomainNTInvoiceDef.NTInvoice.Currency.CurrencyId.ToString());
                txt_Amount.Text = vwDomainNTInvoiceDef.NTInvoice.Amount.ToString();
                txt_TotalVAT.Text = vwDomainNTInvoiceDef.NTInvoice.TotalVAT.ToString();                

                ddl_PaymentMonthFrom.SelectedIndex = ddl_PaymentMonthTo.SelectedIndex = ddl_PaymentYearFrom.SelectedIndex = ddl_PaymentYearTo.SelectedIndex = -1;
                ddl_PaymentMonthFrom.Items.FindByValue(vwDomainNTInvoiceDef.NTInvoice.PaymentFromDate.Month.ToString()).Selected = true;
                ddl_PaymentMonthTo.Items.FindByValue(vwDomainNTInvoiceDef.NTInvoice.PaymentToDate.Month.ToString()).Selected = true;
                ListItem item = ddl_PaymentYearFrom.Items.FindByValue(vwDomainNTInvoiceDef.NTInvoice.PaymentFromDate.Year.ToString());
                if (item == null && vwDomainNTInvoiceDef.NTInvoice.PaymentFromDate.Year < DateTime.Now.Year)
                {
                    int year = DateTime.Today.Year;
                    do
                    {
                        year -= 1;
                        ddl_PaymentYearTo.Items.Insert(0, new ListItem(year.ToString()));
                        ddl_PaymentYearFrom.Items.Insert(0, new ListItem(year.ToString()));
                    } while (year != vwDomainNTInvoiceDef.NTInvoice.PaymentFromDate.Year);
                }
                ddl_PaymentYearFrom.Items.FindByValue(vwDomainNTInvoiceDef.NTInvoice.PaymentFromDate.Year.ToString()).Selected = true;
                ddl_PaymentYearTo.Items.FindByValue(vwDomainNTInvoiceDef.NTInvoice.PaymentToDate.Year.ToString()).Selected = true;
                txt_InvoiceReceivedDate.Text = DateTimeUtility.getDateString(vwDomainNTInvoiceDef.NTInvoice.InvoiceReceivedDate);
                txt_RejectReason.Text = vwDomainNTInvoiceDef.NTInvoice.RejectReason;
                txt_Remark.Text = vwDomainNTInvoiceDef.NTInvoice.UserRemark;

                // todo:
                
                this.txtProcurementRequestNo.Text = string.Empty;

                if (vwDomainNTInvoiceDef.NTInvoice.ProcurementRequestId != -1)
                {
                    APDS.APDSService svc = new APDS.APDSService();
                    APDS.ProcurementRequestDef requestDef = svc.GetProcurementRequestByKey(vwDomainNTInvoiceDef.NTInvoice.ProcurementRequestId);
                    this.txtProcurementRequestNo.Text = requestDef.RequestNo;
                }

                txt_SettlementDate.Text = DateTimeUtility.getDateString(vwDomainNTInvoiceDef.NTInvoice.SettlementDate);
                txt_SettleAmt.Text = vwDomainNTInvoiceDef.NTInvoice.SettlementAmount == 0 ? string.Empty : vwDomainNTInvoiceDef.NTInvoice.SettlementAmount.ToString();
                //txt_SettlementRefNo.Text = vwDomainNTInvoiceDef.NTInvoice.SettlementRefNo;
                txt_ChequeNo.Text = vwDomainNTInvoiceDef.NTInvoice.ChequeNo;
                if (vwDomainNTInvoiceDef.NTInvoice.SettlementBankAccountId > 0)
                    txt_SettlementBankAcc.Text = NonTradeManager.Instance.getNSLBankAccountByKey(vwDomainNTInvoiceDef.NTInvoice.SettlementBankAccountId).AccountNo;
                else
                    txt_SettlementBankAcc.Text = string.Empty;
                //txt_SettlementBankAcc.Text = vwDomainNTInvoiceDef.NTInvoice.SettlementBankAccountId;
                ckb_PayByHK.Checked = (vwDomainNTInvoiceDef.NTInvoice.IsPayByHK == 1);

                txt_SUNInterfaceDate.Text = DateTimeUtility.getDateString(vwDomainNTInvoiceDef.NTInvoice.SUNInterfaceDate);
                txt_FiscalPeriod.Text = vwDomainNTInvoiceDef.NTInvoice.FiscalPeriod == 0 ? string.Empty : vwDomainNTInvoiceDef.NTInvoice.FiscalPeriod.ToString();
                txt_FiscalYear.Text = vwDomainNTInvoiceDef.NTInvoice.FiscalYear == 0 ? string.Empty : vwDomainNTInvoiceDef.NTInvoice.FiscalYear.ToString();
                txt_SubmittedBy.Text = vwDomainNTInvoiceDef.NTInvoice.SubmittedBy == null ? string.Empty : vwDomainNTInvoiceDef.NTInvoice.SubmittedBy.DisplayName;
                txt_SubmittedOn.Text = vwDomainNTInvoiceDef.NTInvoice.SubmittedOn == DateTime.MinValue ? string.Empty : ConvertUtility.toShortDateString(vwDomainNTInvoiceDef.NTInvoice.SubmittedOn);

                this.txt_LogoInterfaceDate.Text = DateTimeUtility.getDateString(vwDomainNTInvoiceDef.NTInvoice.LogoInterfaceDate);
                this.row_LogoInterface.Visible = (vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId == OfficeId.TR.Id);

                bool isSecondApproverUser = (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, GeneralCriteria.ALL).Contains(ddl_Office.selectedValueToInt));
                txt_JournalNo.Text = vwDomainNTInvoiceDef.NTInvoice.JournalNo;
                if (isSecondApproverUser && vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus == NTWFS.ACCOUNTS_RECEIVED)
                {   // Allow to Edit the Journal No.
                    txt_JournalNo.CssClass = "";
                    txt_JournalNo.Attributes.Remove("onfocus");
                    txt_JournalNo.ReadOnly = false;
                }

                if (isAccountUser && vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id >=  NTWFS.DEPARTMENT_APPROVED.Id)
                    vwNTVendorExpenseTypeList = WebUtil.getNTExpenseTypeListByOfficeId(vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId);
                else
                    vwNTVendorExpenseTypeList = WebUtil.getNTExpenseTypeByNTVendorId(vwDomainNTInvoiceDef.NTInvoice.NTVendor.NTVendorId);

                if (vwDomainNTInvoiceDef.NTInvoice.ReleaseReason != string.Empty)
                {
                    row_ReleaseReason.Visible = true;
                    txt_ReleaseReason.Text = vwDomainNTInvoiceDef.NTInvoice.ReleaseReason;
                }

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.CalculateTotalAmount) &&
                    vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id != NTWFS.ACCOUNTS_APPROVED.Id && vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id != NTWFS.SETTLED.Id)
                    hf_CalcTotalAmt.Value = "1";
                else
                    hf_CalcTotalAmt.Value = "0";

                getInvoiceFile();
            }

            if (this.ddl_Office.Items.Count > 0 && int.Parse(this.ddl_Office.SelectedValue) != -1)
                vwDomainNTInvoiceDef.NTInvoice.Office = CommonUtil.getOfficeRefByKey(int.Parse(this.ddl_Office.SelectedValue));

            this.setDefaultApprover();

            if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id != NTWFS.DRAFT.Id && vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id != NTWFS.PENDING_FOR_APPROVAL.Id &&
                vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id != NTWFS.DEPARTMENT_REJECTED.Id)
            {
                if (ddl_Approver.SelectedValue == "-1" && vwDomainNTInvoiceDef.NTInvoice.Approver != null)
                {
                    ddl_Approver.Items.Insert(0, new ListItem(vwDomainNTInvoiceDef.NTInvoice.Approver.DisplayName, vwDomainNTInvoiceDef.NTInvoice.Approver.UserId.ToString()));
                    ddl_Approver.SelectedIndex = 0;
                }
                ddl_Approver.Enabled = false;
                ddl_Approver.CssClass = "readOnlyField";
            }
            else
            {
                ddl_Approver.Enabled = true;
                ddl_Approver.CssClass = "";
            }

            int officeId = (vwDomainNTInvoiceDef.NTInvoice.Office == null ? ddl_Office.selectedValueToInt : vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId);

            int defCompany = getDefaultCompanyByNTVendor();
            ddl_BusinessEntity.bindList(CommonUtil.getCompanyList(officeId), "CompanyName", "CompanyId", (vwDomainNTInvoiceDef.NTInvoice.Company == null ? defCompany.ToString() : vwDomainNTInvoiceDef.NTInvoice.Company.Id.ToString()));
            if (ddl_BusinessEntity.selectedValueToInt == CompanyType.NSL_TK.Id)
                ddl_Currency.selectByText("TRY");

            ddl_PaymentMethod.bindList(NTPaymentMethodRef.getCollectionValues(officeId), "Name", "Id", (vwDomainNTInvoiceDef.NTInvoice.PaymentMethod == null ? "-1" : vwDomainNTInvoiceDef.NTInvoice.PaymentMethod.Id.ToString()));

            if (officeId == OfficeId.SL.Id || officeId == OfficeId.IND.Id || officeId == OfficeId.ND.Id || officeId == OfficeId.TR.Id || officeId == OfficeId.BD.Id || officeId == OfficeId.SH.Id || officeId == OfficeId.CA.Id)
            {
                div_VAT.Visible = true;
                row_PayByHK.Visible = true;
            }
            else
            {
                div_VAT.Visible = false;
                row_PayByHK.Visible = (officeId != OfficeId.HK.Id);
            }

            /*if (officeId == OfficeId.CA.Id || officeId == OfficeId.TH.Id || officeId == OfficeId.VN.Id)
                ckb_PayByHK.Checked = true;*/

            if (ddl_PaymentMethod.selectedValueToInt == NTPaymentMethodRef.CASH.Id)
                row_PayByHK.Visible = false;

            if (!isAccountUser)
            {
                ddl_BusinessEntity.Visible = false;
                //vwNTVendorExpenseTypeList.Sort(new NTExpenseTypeRef.ExpenseTypeComparer( NTExpenseTypeRef.ExpenseTypeComparer.CompareType.ExpenseType));
            }
            else
            {
                ddl_BusinessEntity.Visible = true;
                vwNTVendorExpenseTypeList.Sort(new NTExpenseTypeRef.ExpenseTypeComparer(NTExpenseTypeRef.ExpenseTypeComparer.CompareType.SUNAccountCode));
            }

            /*
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelAccountsApprove) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.SecondLevelAccountsApprove) ||
            
            if (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId) ||
                NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelARApprove) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.SecondLevelARApprove))
                ckb_PayByHK.Enabled = true;*/

            //payByHK();

            if (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(ddl_Office.selectedValueToInt) ||
                NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, GeneralCriteria.ALL).Contains(ddl_Office.selectedValueToInt) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelARApprove) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.SecondLevelARApprove))
            {
                ckb_PayByHK.Enabled = true;
            }

            buttonControl();

            rep_InvoiceDetail.DataSource = vwDomainNTInvoiceDef.NTInvoiceDetailList;
            rep_InvoiceDetail.DataBind();

            rep_RechargeDetail.DataSource = vwDomainNTInvoiceDef.NTRechargeDetailList;
            rep_RechargeDetail.DataBind();

            gv_ActionHistory.DataSource = vwDomainNTInvoiceDef.ActionHistoryList;
            gv_ActionHistory.DataBind();

            refreshProcurementControl();
        }


        private void buttonControl()
        {
            btn_Save.Visible = false;
            btn_Submit.Visible = false;
            btn_Copy.Visible = false;
            btn_Cancel.Visible = false;
            btn_Approve.Visible = false;
            btn_Reject.Visible = false;
            btn_AccountReceive.Visible = false;
            btn_AccountReject.Visible = false;
            btn_AccountApprove.Visible = false;
            btn_AccountEvaluate.Visible = false;            

            bool isMonthEnd = false;
            NTMonthEndStatusDef monthEndStatus = (NTMonthEndStatusDef)WebUtil.getCurrentNTMonthEndStatus(ConvertUtility.createArrayList(CommonUtil.getOfficeRefByKey(ddl_Office.selectedValueToInt)))[0];
            if (monthEndStatus.Status != NTMonthEndStatusDef.OPEN)
                isMonthEnd = true;

            UserRef user = CommonUtil.getUserByKey(this.LogonUserId);

            if (!isMonthEnd)
            {
                div_monthEnd.Visible = false;
                btn_Copy.Visible = true;
                if (vwDomainNTInvoiceDef.NTInvoice.InvoiceId == 0)
                {
                    btn_Save.Visible = true;
                    btn_Copy.Visible = false;
                    btn_Submit.Visible = true;
                }
                else if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.PENDING_FOR_APPROVAL.Id && vwDomainNTInvoiceDef.NTInvoice.Approver.UserId == this.LogonUserId)
                {
                    btn_Save.Visible = true;
                    btn_Cancel.Visible = true;
                    btn_Approve.Visible = true;
                    btn_Reject.Visible = true;
                }
                else if (vwDomainNTInvoiceDef.NTInvoice.Dept.DepartmentId == user.Department.DepartmentId)
                {
                    if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DRAFT.Id)
                    {

                        btn_Save.Visible = true;
                        btn_Cancel.Visible = true;
                        btn_Submit.Visible = true;
                    }
                }
            }
            else
            {
                btn_New.Visible = false;
                div_monthEnd.Visible = true;
                lbl_Office.Text = vwDomainNTInvoiceDef.NTInvoice.Office == null ? CommonUtil.getOfficeRefByKey(ddl_Office.selectedValueToInt).OfficeCode : vwDomainNTInvoiceDef.NTInvoice.Office.OfficeCode;
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.Audit))
            {
                btn_New.Visible = false;
                btn_Copy.Visible = false;
            }
            
            bool hasNSLInterComRecharge = false;
            if (vwDomainNTInvoiceDef.NTInvoice.Company != null && vwDomainNTInvoiceDef.NTInvoice.Company.Id == CompanyType.NEXT_SOURCING.Id && 
                vwDomainNTInvoiceDef.NTRechargeDetailList != null && vwDomainNTInvoiceDef.NTRechargeDetailList.Count > 0)
            {
                foreach (NTInvoiceDetailDef detail in vwDomainNTInvoiceDef.NTRechargeDetailList)
                {
                    if (detail.InvoiceDetailType.Id != NTInvoiceDetailType.USER.Id && !(detail.InvoiceDetailType.Id == NTInvoiceDetailType.OFFICE.Id && detail.Company.Id == vwDomainNTInvoiceDef.NTInvoice.Company.Id))
                    {
                        hasNSLInterComRecharge = true;
                        break;
                    }
                }
            }


            if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DEPARTMENT_APPROVED.Id || vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_EVALUATING.Id)
            {
                ArrayList firstApproverOfficeIdList = NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, vwDomainNTInvoiceDef.NTInvoice.Company.Id);
                bool enableApproval = firstApproverOfficeIdList.Contains(vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId);
                if ((hasNSLInterComRecharge && CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelARApprove)) ||
                    //(!hasNSLInterComRecharge && CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelAccountsApprove)
                    (!hasNSLInterComRecharge && enableApproval)
                   )
                {
                    btn_Save.Visible = true;
                    btn_AccountReceive.Visible = true;
                    btn_AccountReject.Visible = true;
                }
            }
            else if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_RECEIVED.Id)
            {
                ArrayList secondApproverOfficeIdList = NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, vwDomainNTInvoiceDef.NTInvoice.Company.Id);
                bool enableApproval = secondApproverOfficeIdList.Contains(vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId);
                if ((hasNSLInterComRecharge && CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.SecondLevelARApprove)) ||
                    (!hasNSLInterComRecharge && enableApproval))
                {
                    btn_Save.Visible = true;
                    btn_AccountApprove.Visible = true;
                    btn_AccountEvaluate.Visible = true;
                }
            }
            else if ((vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DEPARTMENT_REJECTED.Id ||
                vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_REJECTED.Id) && !isMonthEnd)
            {
                if (vwDomainNTInvoiceDef.NTInvoice.Dept.DepartmentId == user.Department.DepartmentId)
                {
                    btn_Submit.Visible = true;
                    btn_Cancel.Visible = true;
                }
            }

            if ((vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_APPROVED.Id || vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.SETTLED.Id) &&
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.ReleaseInvoice))
                btn_Release.Visible = true;

        }


        ArrayList costCenterList = null;
        protected void InvoiceDetailDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType ==  ListItemType.AlternatingItem )
            {
                NTInvoiceDetailDef ntInvoiceDetailDef = (NTInvoiceDetailDef)e.Item.DataItem;

                if (ntInvoiceDetailDef.Status == 0)
                {
                    e.Item.Visible = false;
                    ((TextBox)e.Item.FindControl("txt_Amount")).Text = "0";
                    return;
                }
                
                SmartDropDownList ddl = (SmartDropDownList)e.Item.FindControl("ddl_CostCenter");

                if (costCenterList == null)
                {
                    costCenterList = (ArrayList) com.next.common.appserver.GeneralManager.Instance.getCostCenterListByOffice(int.Parse(ddl_Office.SelectedValue));
                    //costCenterList.Sort(new CostCenterRef.SortCostCenterRef(CostCenterRef.SortCostCenterRef.CompareType.Code));
                    costCenterList.Sort(new ArrayListHelper.Sorter("OfficeDescription"));
                }
                ddl.bindList(costCenterList, "OfficeDescriptionWithCode", "CostCenterId",
                    ntInvoiceDetailDef.CostCenter == null ? "-1" : ntInvoiceDetailDef.CostCenter.CostCenterId.ToString(), "-- Please select --", GeneralCriteria.ALL.ToString());

                int officeId = (vwDomainNTInvoiceDef.NTInvoice.Office == null ? this.LogonUserHomeOffice.OfficeId : vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId);

                if (officeId == OfficeId.SL.Id || officeId == OfficeId.IND.Id || officeId == OfficeId.ND.Id || officeId == OfficeId.TR.Id || officeId == OfficeId.BD.Id || officeId == OfficeId.SH.Id || officeId == OfficeId.CA.Id)
                {
                    ((HtmlTableCell)e.Item.FindControl("row_VAT1")).Visible = true;
                    ((HtmlTableCell)e.Item.FindControl("row_VAT2")).Visible = true;
                }

                ddl = (SmartDropDownList)e.Item.FindControl("ddl_ExpenseType");
                if (ntInvoiceDetailDef.ExpenseType != null)
                {
                    ddl.bindList(vwNTVendorExpenseTypeList, (isAccountUser ? "AccountCodeDescription" : "Description"), "ExpenseTypeId", ntInvoiceDetailDef.ExpenseType.ExpenseTypeId.ToString());
                    ((Label)e.Item.FindControl("lbl_ItemDescHint")).Text = ntInvoiceDetailDef.ExpenseType.ItemDescriptionHints;
                }
                else
                    ddl.bindList(vwNTVendorExpenseTypeList, (isAccountUser ? "AccountCodeDescription" : "Description"), "ExpenseTypeId", "", "-- Please select --", "-1");

                if (ntInvoiceDetailDef.ItemDescription3 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc3")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription3 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription3")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc3")).Style["display"] = "none";
                if (ntInvoiceDetailDef.ItemDescription4 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc4")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription4 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription4")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc4")).Style["display"] = "none";
                if (ntInvoiceDetailDef.ItemDescription5 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc5")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription5 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription5")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc5")).Style["display"] = "none";

                com.next.isam.webapp.webservices.UclSmartSelection txt_User = (com.next.isam.webapp.webservices.UclSmartSelection)e.Item.FindControl("txt_User");
                txt_User.initControl(webservices.UclSmartSelection.SelectionList.user);
                com.next.isam.webapp.webservices.UclSmartSelection txt_ProductTeam = (com.next.isam.webapp.webservices.UclSmartSelection)e.Item.FindControl("ddl_ProductTeam");
                txt_ProductTeam.initControl(webservices.UclSmartSelection.SelectionList.productCode);

                if (ntInvoiceDetailDef.ExpenseType != null)
                {
                    if (ntInvoiceDetailDef.ExpenseType.IsDepartmentCode == 0)
                        ((Control)e.Item.FindControl("row_CostCenter")).Visible = false;    

                    if (ntInvoiceDetailDef.ExpenseType.IsProductCode == 1 && ntInvoiceDetailDef.CostCenter != null)
                    {
                        ((HtmlTableRow)e.Item.FindControl("row_CostCenter_ProductTeam")).Visible = true;
                        ((HtmlTableRow)e.Item.FindControl("row_CostCenter_ProductTeam")).Attributes.Add("onmouseover", "ctl00_ContentPlaceHolder1_ddl_Office.value = " + ntInvoiceDetailDef.Office.OfficeId.ToString());
                        txt_ProductTeam.setWidth(300);
                                                    
                        if (ntInvoiceDetailDef.ProductTeam != null)
                            txt_ProductTeam.ProductCodeId = ntInvoiceDetailDef.ProductTeam.ProductCodeId; 
                    }
                    if (ntInvoiceDetailDef.ExpenseType.IsSeasonCode == 1)
                    {
                        ((Control)e.Item.FindControl("row_CostCenter_Season")).Visible = true;
                        ((SmartDropDownList)e.Item.FindControl("ddl_Season")).bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", ntInvoiceDetailDef.Season == null ? "-1" : ntInvoiceDetailDef.Season.SeasonId.ToString(), "-- Please select --", "-1");
                    }
                    if (ntInvoiceDetailDef.ExpenseType.IsStaffCode == 1)
                    {
                        ((Control)e.Item.FindControl("row_CostCenter_User")).Visible = true;
                        ((HtmlTableRow)e.Item.FindControl("row_CostCenter_User")).Attributes.Add("onmouseover", "txt_UserOfficeId.value = '" + ddl_Office.SelectedValue + "';");
                        txt_User.setWidth(300);
                        if (ntInvoiceDetailDef.User != null)
                            txt_User.UserId = ntInvoiceDetailDef.User.UserId;
                    }
                    if (ntInvoiceDetailDef.ExpenseType.IsItemNo == 1)
                    {
                        ((Control)e.Item.FindControl("row_CostCenter_ItemNo")).Visible = true;
                        ((TextBox)e.Item.FindControl("txt_ItemNo")).Text = ntInvoiceDetailDef.ItemNo;
                    }
                    if (ntInvoiceDetailDef.ExpenseType.IsDevSampleCostType == 1)
                    {
                        ((Control)e.Item.FindControl("row_CostCenter_DevSampleType")).Visible = true;
                        ((SmartDropDownList)e.Item.FindControl("ddl_DevSampleType")).bindList(NTDevSampleCostType.getNTDevSampleCostTypeList(), "Description", "Id", ntInvoiceDetailDef.DevSampleCostTypeId == 0 ? "-1" : ntInvoiceDetailDef.DevSampleCostTypeId.ToString());
                    }
                    if (ntInvoiceDetailDef.ExpenseType.IsQtyRequired == 1)
                    {
                        ((Control)e.Item.FindControl("row_CostCenter_Quantity")).Visible = true;
                        ((TextBox)e.Item.FindControl("txt_Quantity")).Text = ntInvoiceDetailDef.Quantity.ToString();
                    }
                    if (ntInvoiceDetailDef.ExpenseType.SUNAccountCode == "1412101")
                    {
                        ((Control)e.Item.FindControl("row_CostCenter_ExpenseNature")).Visible = true;
                        ((SmartDropDownList)e.Item.FindControl("ddl_Nature")).bindList(NonTradeManager.Instance.getNTExpenseNatureList(txt_SupplierName.NTVendorId), "Description", "NatureId", ntInvoiceDetailDef.NatureIdForAccrual == 0 ? "-1" : ntInvoiceDetailDef.NatureIdForAccrual.ToString(), "-- Please select --", "-1");
                    }

                    /* TODO:TRADINGAF */
                    if (ntInvoiceDetailDef.ExpenseType.SUNAccountCode == "1452028")
                    {
                        ((Control)e.Item.FindControl("row_CostCenter_TradingAF")).Visible = true;
                        ((TextBox)e.Item.FindControl("txtContractNo")).Text = ntInvoiceDetailDef.ContractNo;
                        ((TextBox)e.Item.FindControl("txtDeliveryNo")).Text = ntInvoiceDetailDef.DeliveryNo.ToString();
                        ((HtmlTableRow)e.Item.FindControl("row_CostCenter_ProductTeam")).Visible = true;
                        ((Control)e.Item.FindControl("row_CostCenter_Season")).Visible = false;
                    }

                    if (ntInvoiceDetailDef.ExpenseType.IsSegmentValue == 1)
                    {
                        if (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId)
                            && vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DEPARTMENT_APPROVED.Id)
                        {
                            ((Control)e.Item.FindControl("row_CostCenter_SegmentField7")).Visible = true;
                            ((Control)e.Item.FindControl("row_CostCenter_SegmentField8")).Visible = true;
                        }
                        else if (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId) ||
                            NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId) ||
                                NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SUN_INTERFACE.Id, GeneralCriteria.ALL).Contains(officeId) ||
                                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelARApprove) ||
                                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.SecondLevelARApprove))
                        {
                            ((Control)e.Item.FindControl("row_CostCenter_SegmentField7")).Visible = true;
                            ((Control)e.Item.FindControl("row_CostCenter_SegmentField8")).Visible = true;
                            ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue7")).Enabled = true;
                            ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue8")).Enabled = true;
                        }

                    }
                }
                ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue7")).bindList(NonTradeManager.Instance.getNTEpicorSegmentValueListBySegmentField(7), "Description", "SegmentValueId", ntInvoiceDetailDef.SegmentValue7 == null ? "-1" : ntInvoiceDetailDef.SegmentValue7.SegmentValueId.ToString(), "-- Please select --", "-1");
                ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue8")).bindList(NonTradeManager.Instance.getNTEpicorSegmentValueListBySegmentField(8), "Description", "SegmentValueId", ntInvoiceDetailDef.SegmentValue8 == null ? "-1" : ntInvoiceDetailDef.SegmentValue8.SegmentValueId.ToString(), "-- Please select --", "-1");

                if (ntInvoiceDetailDef.ExpenseType != null && ntInvoiceDetailDef.ExpenseType.SUNAccountCode == "1452028")
                {
                    if (ntInvoiceDetailDef.SegmentValue7 == null)
                        ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue7")).selectByValue("1"); // U
                    if (ntInvoiceDetailDef.SegmentValue8 == null)
                        ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue8")).selectByValue("10"); // G
                }

                refreshCarbonConsumptionRow(((Control)e.Item.FindControl("row_CostCenter_Consumption")), ntInvoiceDetailDef);

            }
        }

        
        protected void reloadInvoiceDetailGrid(object sender, EventArgs e)
        {
            updateInvoiceDetail(-1, true);
            rep_InvoiceDetail.DataSource = vwDomainNTInvoiceDef.NTInvoiceDetailList;
            rep_InvoiceDetail.DataBind();

        }

        protected void AddCostCenter_Click(object sender, EventArgs e)
        {
            if (vwDomainNTInvoiceDef.NTInvoiceDetailList != null)
            {
                if (!updateInvoiceDetail(-1, true))
                    return;
            }
            else
                vwDomainNTInvoiceDef.NTInvoiceDetailList = new ArrayList();

            NTInvoiceDetailDef detailDef = new NTInvoiceDetailDef(NTInvoiceDetailType.COSTCENTER);           
            detailDef.IsRecharge = 0;

            if (vwDomainNTInvoiceDef.NTInvoice.NTVendor != null && vwDomainNTInvoiceDef.NTInvoice.NTVendor.ExpenseType != null  &&
                !(vwDomainNTInvoiceDef.NTInvoice.NTVendor.ExpenseType.SUNAccountCode == "1311303" || vwDomainNTInvoiceDef.NTInvoice.NTVendor.ExpenseType.SUNAccountCode == "1311307" ||
                vwDomainNTInvoiceDef.NTInvoice.NTVendor.ExpenseType.SUNAccountCode == "1311308"))
            {
                detailDef.ExpenseType = vwDomainNTInvoiceDef.NTInvoice.NTVendor.ExpenseType;
            }

            vwDomainNTInvoiceDef.NTInvoiceDetailList.Add(detailDef);
            rep_InvoiceDetail.DataSource = vwDomainNTInvoiceDef.NTInvoiceDetailList;
            rep_InvoiceDetail.DataBind();
        }
        

        protected void InvoiceDetailItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if (!updateInvoiceDetail(index, true))
                    return;
                NTInvoiceDetailDef ntInvoiceInvoiceDef = (NTInvoiceDetailDef) vwDomainNTInvoiceDef.NTInvoiceDetailList[index];
                if (ntInvoiceInvoiceDef.InvoiceDetailId == 0)
                    vwDomainNTInvoiceDef.NTInvoiceDetailList.RemoveAt(index);
                else
                    ntInvoiceInvoiceDef.Status = 0;
                
                rep_InvoiceDetail.DataSource = vwDomainNTInvoiceDef.NTInvoiceDetailList;
                rep_InvoiceDetail.DataBind();                
            }
            else if (e.CommandName == "copy")
            {
                if (!updateInvoiceDetail(-1, false))
                    return;

                int index = Convert.ToInt32(e.CommandArgument);
                NTInvoiceDetailDef ntInvoiceDetailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTInvoiceDetailList[index];
                NTInvoiceDetailDef newInvoiceDetailDef = new NTInvoiceDetailDef(NTInvoiceDetailType.COSTCENTER);
                newInvoiceDetailDef.IsRecharge = 0;
                newInvoiceDetailDef.ExpenseType = ntInvoiceDetailDef.ExpenseType;
                if (ntInvoiceDetailDef.User == null)
                {
                    newInvoiceDetailDef.CostCenter = ntInvoiceDetailDef.CostCenter;
                    newInvoiceDetailDef.Office = ntInvoiceDetailDef.Office == null ? null : ntInvoiceDetailDef.Office;
                    newInvoiceDetailDef.Company = ntInvoiceDetailDef.Company == null ? null : ntInvoiceDetailDef.Company;
                    newInvoiceDetailDef.Department = ntInvoiceDetailDef.Department == null ? null : ntInvoiceDetailDef.Department;
                }

                newInvoiceDetailDef.ItemDescription1 = ntInvoiceDetailDef.ItemDescription1;
                newInvoiceDetailDef.ItemDescription2 = ntInvoiceDetailDef.ItemDescription2;
                newInvoiceDetailDef.ItemDescription3 = ntInvoiceDetailDef.ItemDescription3;
                newInvoiceDetailDef.ItemDescription4 = ntInvoiceDetailDef.ItemDescription4;
                newInvoiceDetailDef.ItemDescription5 = ntInvoiceDetailDef.ItemDescription5;

                if (ntInvoiceDetailDef.SegmentValue7 != null)
                    newInvoiceDetailDef.SegmentValue7 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(ntInvoiceDetailDef.SegmentValue7.SegmentValueId);
                if (ntInvoiceDetailDef.SegmentValue8 != null)
                    newInvoiceDetailDef.SegmentValue8 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(ntInvoiceDetailDef.SegmentValue8.SegmentValueId);

                vwDomainNTInvoiceDef.NTInvoiceDetailList.Add(newInvoiceDetailDef);
                rep_InvoiceDetail.DataSource = vwDomainNTInvoiceDef.NTInvoiceDetailList;
                rep_InvoiceDetail.DataBind();
            }
            else if (e.CommandName == "addItemDesc")
            {
                //updateInvoiceDetail(-1, true);
                NTInvoiceDetailDef ntInvoiceDetailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTInvoiceDetailList[Convert.ToInt32(e.CommandArgument)];

                if (((HtmlTableRow)e.Item.FindControl("row_ItemDesc2")).Style["display"] == "none")
                {                    
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc2")).Style["display"] = "block";
                    ((TextBox)e.Item.FindControl("txt_ItemDescription2")).Text = string.Empty;
                }
                else if (ntInvoiceDetailDef.ItemDescription3 == string.Empty)
                {
                    ntInvoiceDetailDef.ItemDescription3 = ".";
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc3")).Style["display"] = "block";
                    ((TextBox)e.Item.FindControl("txt_ItemDescription3")).Text = string.Empty;
                }
                else if (ntInvoiceDetailDef.ItemDescription4 == string.Empty)
                {
                    ntInvoiceDetailDef.ItemDescription4 = ".";
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc4")).Style["display"] = "block";
                    ((TextBox)e.Item.FindControl("txt_ItemDescription4")).Text = string.Empty;
                }
                else if (ntInvoiceDetailDef.ItemDescription5 == string.Empty)
                {
                    ntInvoiceDetailDef.ItemDescription5 = ".";
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc5")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription5 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription5")).Text = string.Empty;
                }                
            }
            else if (e.CommandName == "removeItemDesc")
            {
                NTInvoiceDetailDef ntInvoiceDetailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTInvoiceDetailList[Convert.ToInt32(e.CommandArgument)];

                ImageButton imgButton = (ImageButton) e.CommandSource;

                string s_Desc2 = ((TextBox)e.Item.FindControl("txt_ItemDescription2")).Text;
                string s_Desc3 = ((TextBox)e.Item.FindControl("txt_ItemDescription3")).Text;
                string s_Desc4 = ((TextBox)e.Item.FindControl("txt_ItemDescription4")).Text;
                string s_Desc5 = ((TextBox)e.Item.FindControl("txt_ItemDescription5")).Text;

                ntInvoiceDetailDef.ItemDescription2 = s_Desc2;
                ntInvoiceDetailDef.ItemDescription3 = (s_Desc3 == string.Empty ? (ntInvoiceDetailDef.ItemDescription3 == "." ? "." : string.Empty) : s_Desc3);
                ntInvoiceDetailDef.ItemDescription4 = (s_Desc4 == string.Empty ? (ntInvoiceDetailDef.ItemDescription4 == "." ? "." : string.Empty) : s_Desc4);
                ntInvoiceDetailDef.ItemDescription5 = (s_Desc5 == string.Empty ? (ntInvoiceDetailDef.ItemDescription5 == "." ? "." : string.Empty) : s_Desc5);

                switch (imgButton.ID)
                {
                    case "btn_removeItemDesc2": ntInvoiceDetailDef.ItemDescription2 = (ntInvoiceDetailDef.ItemDescription3 == "." ? string.Empty : ntInvoiceDetailDef.ItemDescription3);
                        ((TextBox)e.Item.FindControl("txt_ItemDescription2")).Text = ntInvoiceDetailDef.ItemDescription2;
                        goto case "btn_removeItemDesc3";
                    case "btn_removeItemDesc3": ntInvoiceDetailDef.ItemDescription3 = ntInvoiceDetailDef.ItemDescription4;
                        ((TextBox)e.Item.FindControl("txt_ItemDescription3")).Text = ntInvoiceDetailDef.ItemDescription3;
                        goto case "btn_removeItemDesc4";
                    case "btn_removeItemDesc4": ntInvoiceDetailDef.ItemDescription4 = ntInvoiceDetailDef.ItemDescription5;
                        ((TextBox)e.Item.FindControl("txt_ItemDescription4")).Text = ntInvoiceDetailDef.ItemDescription4;
                        goto case "btn_removeItemDesc5";
                    case "btn_removeItemDesc5": ntInvoiceDetailDef.ItemDescription5 = string.Empty;
                        ((TextBox)e.Item.FindControl("txt_ItemDescription5")).Text = ntInvoiceDetailDef.ItemDescription5;
                        break;
                }

                if (imgButton.ID == "btn_removeItemDesc2" && ((HtmlTableRow)e.Item.FindControl("row_ItemDesc3")).Style["display"] == "none")
                {
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc2")).Style["display"] = "none";
                }
                else
                {
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc2")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription2 == string.Empty)
                        ((TextBox)e.Item.FindControl("txt_ItemDescription2")).Text = string.Empty;
                }
                if (ntInvoiceDetailDef.ItemDescription3 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc3")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription3 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription3")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc3")).Style["display"] = "none";
                if (ntInvoiceDetailDef.ItemDescription4 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc4")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription4 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription4")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc4")).Style["display"] = "none";
                if (ntInvoiceDetailDef.ItemDescription5 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc5")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription5 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription5")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_ItemDesc5")).Style["display"] = "none";

            }
        }

        private ArrayList arr_ExpenseType_All = null;
        protected void RechargeDetailDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType ==  ListItemType.AlternatingItem )
            {
                NTInvoiceDetailDef ntRechargeDetailDef = (NTInvoiceDetailDef)e.Item.DataItem;
                if (ntRechargeDetailDef.Status == 0 || ntRechargeDetailDef.IsPayByHK == 1)
                {
                    e.Item.Visible = false;
                    if (ntRechargeDetailDef.IsPayByHK == 1)
                    {
                        com.next.isam.webapp.webservices.UclSmartSelection selectList = (com.next.isam.webapp.webservices.UclSmartSelection)e.Item.FindControl("txt_Vendor");
                        selectList.setWidth(305);
                        selectList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.vendorForRecharge);
                        selectList = (com.next.isam.webapp.webservices.UclSmartSelection)e.Item.FindControl("txt_User");
                        selectList.setWidth(305);
                        selectList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.user);
                        selectList = (com.next.isam.webapp.webservices.UclSmartSelection)e.Item.FindControl("txt_ProductTeam");
                        selectList.setWidth(305);
                        selectList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
                    }
                    return;
                }

                int rechargeTypeId = ntRechargeDetailDef.InvoiceDetailType == null ? -1 : ntRechargeDetailDef.InvoiceDetailType.Id;

                SmartDropDownList dropdown = (SmartDropDownList)e.Item.FindControl("ddl_RechargeType");
                dropdown.bindList(NTInvoiceDetailType.getRechargeTypeCollectionValues(), "Description", "Id", rechargeTypeId == -1 ? "-1" : rechargeTypeId.ToString(), "-- Please select --", "-1");

                dropdown = (SmartDropDownList)e.Item.FindControl("ddl_ExpenseType");
                if (arr_ExpenseType_All == null)
                {
                    arr_ExpenseType_All = WebUtil.getNTExpenseTypeListByOfficeId(ddl_Office.selectedValueToInt);
                    if (isAccountUser)
                        arr_ExpenseType_All.Sort(new NTExpenseTypeRef.ExpenseTypeComparer(NTExpenseTypeRef.ExpenseTypeComparer.CompareType.SUNAccountCode));
                }
                if (ntRechargeDetailDef.ExpenseType != null)
                {
                    dropdown.bindList(arr_ExpenseType_All, (isAccountUser ? "AccountCodeDescription" : "Description"), "ExpenseTypeId", ntRechargeDetailDef.ExpenseType.ExpenseTypeId.ToString());
                    ((Label)e.Item.FindControl("lbl_ItemDescHint")).Text = ntRechargeDetailDef.ExpenseType.ItemDescriptionHints;
                }
                else
                    dropdown.bindList(arr_ExpenseType_All, (isAccountUser ? "AccountCodeDescription" : "Description"), "ExpenseTypeId", "-1", "-- Please select --", "-1");

                DropDownList ddl = (DropDownList)e.Item.FindControl("ddl_RechargeCurrency");
                if (ntRechargeDetailDef.RechargeCurrency != null)
                {
                    ddl.SelectedIndex = -1;
                    ((ListItem)ddl.Items.FindByValue(ntRechargeDetailDef.RechargeCurrency.CurrencyId.ToString())).Selected = true;
                }

                SmartDropDownList ddl_Intercomm = (SmartDropDownList)e.Item.FindControl("ddl_Intercomm");
                ddl_Intercomm.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "SAME AS OFFICE", GeneralCriteria.ALL.ToString());
                {
                    ddl_Intercomm.selectByValue("-1");
                    if (int.Parse(ddl_Office.SelectedValue) != ntRechargeDetailDef.IntercommOfficeId)
                        ddl_Intercomm.selectByValue(ntRechargeDetailDef.IntercommOfficeId.ToString());
                }

                ddl_Intercomm = (SmartDropDownList)e.Item.FindControl("ddl_Intercomm_Cust");
                ddl_Intercomm.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "SAME AS OFFICE", GeneralCriteria.ALL.ToString());
                {
                    ddl_Intercomm.selectByValue("-1");
                    if (int.Parse(ddl_Office.SelectedValue) != ntRechargeDetailDef.IntercommOfficeId)
                        ddl_Intercomm.selectByValue(ntRechargeDetailDef.IntercommOfficeId.ToString());
                }

                if (isAccountUser)
                {
                    ((HtmlTableCell)e.Item.FindControl("row_RechargeCcy1")).Visible = true;
                    ((HtmlTableCell)e.Item.FindControl("row_RechargeCcy2")).Visible = true;

                    bool isARRecharge = (NTExpenseTypeRef.isOtherReceivableRecharge(ntRechargeDetailDef.ExpenseType));
                    ((HtmlTableCell)e.Item.FindControl("row_Intercomm1")).Visible = isARRecharge;
                    ((HtmlTableCell)e.Item.FindControl("row_Intercomm2")).Visible = isARRecharge;
                    ((HtmlTableCell)e.Item.FindControl("row_Intercomm3")).Visible = isARRecharge;
                    ((HtmlTableCell)e.Item.FindControl("row_Intercomm4")).Visible = isARRecharge;
                }

                int officeId = (vwDomainNTInvoiceDef.NTInvoice.Office == null ? this.LogonUserHomeOffice.OfficeId : vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId);

                if (officeId == OfficeId.SL.Id || officeId == OfficeId.IND.Id || officeId == OfficeId.ND.Id || officeId == OfficeId.TR.Id || officeId == OfficeId.BD.Id || officeId == OfficeId.SH.Id || officeId == OfficeId.CA.Id)
                {
                    ((HtmlTableCell)e.Item.FindControl("row_VAT1")).Visible = true;
                    ((HtmlTableCell)e.Item.FindControl("row_VAT2")).Visible = true;
                }

                if (ntRechargeDetailDef.ItemDescription3 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc3")).Style["display"] = "block";
                    if (ntRechargeDetailDef.ItemDescription3 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription3")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc3")).Style["display"] = "none";
                if (ntRechargeDetailDef.ItemDescription4 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc4")).Style["display"] = "block";
                    if (ntRechargeDetailDef.ItemDescription4 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription4")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc4")).Style["display"] = "none";
                if (ntRechargeDetailDef.ItemDescription5 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc5")).Style["display"] = "block";
                    if (ntRechargeDetailDef.ItemDescription5 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription5")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc5")).Style["display"] = "none";


                com.next.isam.webapp.webservices.UclSmartSelection selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)e.Item.FindControl("txt_Vendor");
                selectionList.setWidth(305);

                if (rechargeTypeId != -1 && rechargeTypeId != NTInvoiceDetailType.USER.Id && rechargeTypeId != NTInvoiceDetailType.OFFICE.Id && rechargeTypeId != NTInvoiceDetailType.CUSTOMER.Id)
                {
                    ((Control)e.Item.FindControl("row_Recharge_Vendor")).Visible = true;
                    if (rechargeTypeId == NTInvoiceDetailType.NT_VENDOR.Id)
                    {
                        selectionList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.ntVendor);
                        if (ntRechargeDetailDef.NTVendor != null && ntRechargeDetailDef.NTVendor.NTVendorId != int.MinValue)
                            selectionList.NTVendorId = ntRechargeDetailDef.NTVendor.NTVendorId;
                    }
                    else
                    {
                        selectionList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.vendorForRecharge);
                        if (ntRechargeDetailDef.Vendor != null && ntRechargeDetailDef.Vendor.VendorId != int.MinValue)
                            selectionList.VendorId = ntRechargeDetailDef.Vendor.VendorId;

                        if (rechargeTypeId == NTInvoiceDetailType.FABRIC_VENDOR.Id)
                            txt_VendorType.Value = "1";
                        else if (rechargeTypeId == NTInvoiceDetailType.GARMENT_VENDOR.Id)
                            txt_VendorType.Value = "2";
                        else if (rechargeTypeId == NTInvoiceDetailType.LAUNDRY_VENDOR.Id)
                            txt_VendorType.Value = "5";
                        else if (rechargeTypeId == NTInvoiceDetailType.NON_CLOTHING_VENDOR.Id)
                            txt_VendorType.Value = "4";
                        else if (rechargeTypeId == NTInvoiceDetailType.PACKAGING_VENDOR.Id)
                            txt_VendorType.Value = "7";
                        else if (rechargeTypeId == NTInvoiceDetailType.TRIM_VENDOR.Id)
                            txt_VendorType.Value = "6";
                    }
                }
                else
                    selectionList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.vendorForRecharge);

                if (rechargeTypeId == NTInvoiceDetailType.CUSTOMER.Id)
                {
                    dropdown = (SmartDropDownList)e.Item.FindControl("ddl_Customer");
                    ArrayList tmp_Customer = (ArrayList)WebUtil.getCustomerList();
                    ArrayList arr_Customer = new ArrayList();
                    foreach (domain.common.CustomerDef customer in tmp_Customer)
                    {
                        if (customer.CustomerCode != "DIRECTORY" && customer.CustomerCode != "NTN" && customer.CustomerCode != "LIME")
                            arr_Customer.Add(customer);
                    }

                    dropdown.bindList(arr_Customer, "CustomerDescription", "CustomerId",
                        (ntRechargeDetailDef.Customer == null) ? "-1" : ntRechargeDetailDef.Customer.CustomerId.ToString(), "-- Please select --", "-1");
                    ((Control)e.Item.FindControl("row_Recharge_Customer")).Visible = true;
                }

                //show office drop down for selection only when recharge type = office
                if (rechargeTypeId == NTInvoiceDetailType.OFFICE.Id)
                {
                    dropdown = (SmartDropDownList)e.Item.FindControl("ddl_Office");
                    ArrayList arr_OfficeList = WebUtil.getNTRechargeableOfficeList(-1);
                    ArrayList arr_CompanyList = CommonUtil.getCompanyList(ddl_Office.selectedValueToInt);

                    if (arr_CompanyList.Count == 1)
                    {
                        OfficeRef invOffice = null;
                        foreach (OfficeRef tmpOffice in arr_OfficeList)
                        {
                            if (tmpOffice.OfficeId == ddl_Office.selectedValueToInt)
                            {
                                invOffice = tmpOffice;
                                break;
                            }
                        }
                        arr_OfficeList.Remove(invOffice);
                    }

                    dropdown.bindList(arr_OfficeList, "OfficeCode", "OfficeId",
                        (ntRechargeDetailDef.Office == null) ? "-1" : ntRechargeDetailDef.Office.OfficeId.ToString(), "--", "-1");
                    ((Control)e.Item.FindControl("row_Recharge_Office")).Visible = true;

                    dropdown = (SmartDropDownList)e.Item.FindControl("ddl_BusinessEntity");

                    if (ntRechargeDetailDef.Office != null)
                    {
                        if (ntRechargeDetailDef.Office.OfficeId == ddl_Office.selectedValueToInt && arr_CompanyList.Count > 1)
                        {
                            CompanyRef invCom = null;
                            foreach (CompanyRef tmpCom in arr_CompanyList)
                            {
                                if (tmpCom.CompanyId == ddl_BusinessEntity.selectedValueToInt)
                                {
                                    invCom = tmpCom;
                                    break;
                                }
                            }
                            arr_CompanyList.Remove(invCom);
                            dropdown.bindList(arr_CompanyList, "CompanyName", "CompanyId", ntRechargeDetailDef.Company == null ? CompanyType.NEXT_SOURCING.Id.ToString() : ntRechargeDetailDef.Company.Id.ToString());
                        }
                        else
                        {
                            dropdown.bindList(CommonUtil.getCompanyList(ntRechargeDetailDef.Office.OfficeId), "CompanyName", "CompanyId", ntRechargeDetailDef.Company == null ? CompanyType.NEXT_SOURCING.Id.ToString() : ntRechargeDetailDef.Company.Id.ToString());
                        }
                    }


                    if (!isAccountUser)
                        dropdown.Visible = false;
                }

                if (rechargeTypeId != NTInvoiceDetailType.OFFICE.Id && rechargeTypeId != NTInvoiceDetailType.USER.Id)
                {
                    ((Control)e.Item.FindControl("row_Recharge_ContactPerson")).Visible = true;
                    ((TextBox)e.Item.FindControl("txt_ContactPerson")).Text = ntRechargeDetailDef.ContactPerson;
                }

                selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)e.Item.FindControl("txt_User");
                selectionList.setWidth(305);
                selectionList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.user);
                if (rechargeTypeId == NTInvoiceDetailType.USER.Id ||
                    (ntRechargeDetailDef.ExpenseType != null && ((rechargeTypeId == NTInvoiceDetailType.OFFICE.Id && ntRechargeDetailDef.ExpenseType.IsStaffCode == 1) ||
                    (ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1311303" || ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1311307"))))
                {
                    if (ntRechargeDetailDef.User != null && ntRechargeDetailDef.User.UserId != int.MinValue)
                        selectionList.UserId = ntRechargeDetailDef.User.UserId;
                    ((Control)e.Item.FindControl("row_Recharge_User")).Visible = true;
                    if (rechargeTypeId == NTInvoiceDetailType.OFFICE.Id && ntRechargeDetailDef.Office != null)
                        if (ntRechargeDetailDef.ExpenseType.SUNAccountCode == "4104104") // mobile phone
                            ((HtmlTableRow)e.Item.FindControl("row_Recharge_User")).Attributes.Add("onmouseover", "txt_UserOfficeId.value = '-1';");
                        else
                            ((HtmlTableRow)e.Item.FindControl("row_Recharge_User")).Attributes.Add("onmouseover", "txt_UserOfficeId.value = " + ntRechargeDetailDef.Office.OfficeId.ToString());
                    else
                        ((HtmlTableRow)e.Item.FindControl("row_Recharge_User")).Attributes.Add("onmouseover", "txt_UserOfficeId.value = '-1';");
                }

                selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)e.Item.FindControl("txt_ProductTeam");
                selectionList.setWidth(305);
                selectionList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
                if (ntRechargeDetailDef.ExpenseType != null && rechargeTypeId == NTInvoiceDetailType.OFFICE.Id)
                {
                    if (ntRechargeDetailDef.ExpenseType.IsProductCode == 1 && ntRechargeDetailDef.Office != null)
                    {
                        if (ntRechargeDetailDef.ProductTeam != null && ntRechargeDetailDef.ProductTeam.ProductCodeId != int.MinValue)
                        {
                            selectionList.ProductCodeId = ntRechargeDetailDef.ProductTeam.ProductCodeId;
                        }
                        ((HtmlTableRow)e.Item.FindControl("row_Recharge_ProductTeam")).Visible = true;
                        ((HtmlTableRow)e.Item.FindControl("row_Recharge_ProductTeam")).Attributes.Add("onmouseover", "ctl00_ContentPlaceHolder1_ddl_Office.value = " + ntRechargeDetailDef.Office.OfficeId.ToString());
                    }

                    if (ntRechargeDetailDef.ExpenseType.IsDepartmentCode == 1 && ntRechargeDetailDef.Office != null)
                    {
                        dropdown = (SmartDropDownList)e.Item.FindControl("ddl_CostCenter");
                        dropdown.bindList(com.next.common.appserver.GeneralManager.Instance.getCostCenterListByOffice(ntRechargeDetailDef.Office.OfficeId),
                            "OfficeDescriptionWithCode", "CostCenterId", ntRechargeDetailDef.CostCenter == null ? "-1" : ntRechargeDetailDef.CostCenter.CostCenterId.ToString(), "-- Please select --", "-1");
                        ((Control)e.Item.FindControl("row_Recharge_CostCenter")).Visible = true;
                    }

                    if (ntRechargeDetailDef.ExpenseType.IsSeasonCode == 1)
                    {
                        dropdown = (SmartDropDownList)e.Item.FindControl("ddl_Season");
                        dropdown.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", ntRechargeDetailDef.Season == null ? "" : ntRechargeDetailDef.Season.SeasonId.ToString(), "-- Please select --", "-1");
                        ((Control)e.Item.FindControl("row_Recharge_Season")).Visible = true;
                    }

                    if (ntRechargeDetailDef.ExpenseType.IsItemNo == 1)
                    {
                        ((Control)e.Item.FindControl("row_Recharge_ItemNo")).Visible = true;
                        ((TextBox)e.Item.FindControl("txt_ItemNo")).Text = ntRechargeDetailDef.ItemNo;
                    }

                    if (ntRechargeDetailDef.ExpenseType.IsDevSampleCostType == 1)
                    {
                        ((Control)e.Item.FindControl("row_Recharge_DevSampleType")).Visible = true;
                        dropdown = (SmartDropDownList)e.Item.FindControl("ddl_DevSampleType");
                        dropdown.bindList(NTDevSampleCostType.getNTDevSampleCostTypeList(), "Description", "Id", ntRechargeDetailDef.DevSampleCostTypeId == 0 ? "-1" : ntRechargeDetailDef.DevSampleCostTypeId.ToString());
                    }

                    if (ntRechargeDetailDef.ExpenseType.IsQtyRequired == 1)
                    {
                        ((Control)e.Item.FindControl("row_Recharge_Quantity")).Visible = true;
                        ((TextBox)e.Item.FindControl("txt_Quantity")).Text = ntRechargeDetailDef.Quantity.ToString();
                    }

                    if (ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1412101")
                    {
                        ((Control)e.Item.FindControl("row_Recharge_ExpenseNature")).Visible = true;
                        ((SmartDropDownList)e.Item.FindControl("ddl_Nature")).bindList(NonTradeManager.Instance.getNTExpenseNatureList(txt_SupplierName.NTVendorId), "Description", "NatureId", ntRechargeDetailDef.NatureIdForAccrual == 0 ? "-1" : ntRechargeDetailDef.NatureIdForAccrual.ToString(), "-- Please select --", "-1");
                    }
                }

                /* TODO:TRADINGAF */
                if (ntRechargeDetailDef.ExpenseType != null && ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1452028")
                {
                    ((Control)e.Item.FindControl("row_Recharge_TradingAF")).Visible = true;
                    ((TextBox)e.Item.FindControl("txtContractNo")).Text = ntRechargeDetailDef.ContractNo;
                    ((TextBox)e.Item.FindControl("txtDeliveryNo")).Text = ntRechargeDetailDef.DeliveryNo.ToString();
                    ((HtmlTableRow)e.Item.FindControl("row_Recharge_ProductTeam")).Visible = true;
                    ((Control)e.Item.FindControl("row_Recharge_Season")).Visible = false;
                }

                if (ntRechargeDetailDef.ExpenseType != null && ntRechargeDetailDef.ExpenseType.IsSegmentValue == 1)
                {
                    if (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId)
                            && vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DEPARTMENT_APPROVED.Id)
                    {
                        ((Control)e.Item.FindControl("row_Recharge_SegmentField7")).Visible = true;
                        ((Control)e.Item.FindControl("row_Recharge_SegmentField8")).Visible = true;
                    }
                    else if (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId) ||
                        NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId) ||
                            NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SUN_INTERFACE.Id, GeneralCriteria.ALL).Contains(officeId) ||
                            CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelARApprove) ||
                            CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.SecondLevelARApprove))
                    {
                        ((Control)e.Item.FindControl("row_Recharge_SegmentField7")).Visible = true;
                        ((Control)e.Item.FindControl("row_Recharge_SegmentField8")).Visible = true;
                        if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelARApprove) ||
                            CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.SecondLevelARApprove))
                        {
                            ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue7")).Enabled = true;
                            ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue8")).Enabled = true;
                        }
                        else
                        {
                            ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue7")).Enabled = true;
                            ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue8")).Enabled = true;
                        }
                    }
                }

                ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue7")).bindList(NonTradeManager.Instance.getNTEpicorSegmentValueListBySegmentField(7), "Description", "SegmentValueId", ntRechargeDetailDef.SegmentValue7 == null ? "-1" : ntRechargeDetailDef.SegmentValue7.SegmentValueId.ToString(), "-- Please select --", "-1");
                ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue8")).bindList(NonTradeManager.Instance.getNTEpicorSegmentValueListBySegmentField(8), "Description", "SegmentValueId", ntRechargeDetailDef.SegmentValue8 == null ? "-1" : ntRechargeDetailDef.SegmentValue8.SegmentValueId.ToString(), "-- Please select --", "-1");

                if (ntRechargeDetailDef.ExpenseType != null && ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1452028")
                {
                    if (ntRechargeDetailDef.SegmentValue7 == null)
                        ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue7")).selectByValue("1"); // U
                    if (ntRechargeDetailDef.SegmentValue8 == null)
                        ((SmartDropDownList)e.Item.FindControl("ddl_SegmentValue8")).selectByValue("10"); // G
                }

                refreshCarbonConsumptionRow(((Control)e.Item.FindControl("row_Recharge_Consumption")), ntRechargeDetailDef);

            }
        }

        protected void refreshCarbonConsumptionRow(Control rowConsumption, NTInvoiceDetailDef detail)
        {
            //NTInvoiceDef invoice = NonTradeManager.Instance.getNTInvoiceByKey(detail.InvoiceId);
            NTVendorDef vendor = vwDomainNTInvoiceDef.NTInvoice.NTVendor;
            refreshCarbonConsumptionRow(rowConsumption, detail, vendor, false);
        }

        protected void refreshCarbonConsumptionRow(Control rowConsumption, NTInvoiceDetailDef detail, bool clearConsumption)
        {
            //NTInvoiceDef invoice = NonTradeManager.Instance.getNTInvoiceByKey(detail.InvoiceId);
            NTVendorDef vendor = vwDomainNTInvoiceDef.NTInvoice.NTVendor;
            refreshCarbonConsumptionRow(rowConsumption, detail, vendor, clearConsumption);
        }

        protected void refreshCarbonConsumptionRow(Control rowConsumption, NTInvoiceDetailDef detail, NTVendorDef vendor, bool clearConsumption)
        {
            SmartDropDownList ddlUnit = (SmartDropDownList)rowConsumption.FindControl("ddl_ConsumptionUnit");
            SmartDropDownList ddlFuelType = (SmartDropDownList)rowConsumption.FindControl("ddl_FuelType");
            TextBox txtQty = ((TextBox)rowConsumption.FindControl("txtConsumptionQty"));
            TextBox txtUnitCost = ((TextBox)rowConsumption.FindControl("txtConsumptionUnitCost"));
            Label lblQty = ((Label)rowConsumption.FindControl("lblConsumptionQty"));
            Label lblUnitCost = ((Label)rowConsumption.FindControl("lblConsumptionUnitCost"));
            Label lblFuelType = ((Label)rowConsumption.FindControl("lblFuelType"));
            //NTVendorDef vendor = (invoice != null ? invoice.NTVendor : null);
            string prevQtyLabel = lblQty.Text;
            //ddlUnit.Enabled = false;

            if (detail.ExpenseType != null && carbonEmissionExpenseTypeAccountCode.Contains(detail.ExpenseType.SUNAccountCode))
            {
                rowConsumption.Visible = false;
                if (utilityExpenseTypeAccountCode.Contains(detail.ExpenseType.SUNAccountCode))
                {   //Utilities
                    rowConsumption.Visible = true;
                    lblQty.Text = "No. of Units Consumed : ";
                    ddlUnit.bindList(ConsumptionType.getCollectionValuesForUtilities(), "Name", "Id", "-1", "-- Please select --", "-1");
                    if (lblQty.Text == prevQtyLabel)
                        txtUnitCost.Text = detail.ConsumptionUnitCost.ToString();
                    txtUnitCost.Visible = true;
                    lblUnitCost.Visible = true;
                    ddlFuelType.Visible = false;
                    lblFuelType.Visible = false;
                    /*
                    ddlUnit.Enabled = false;
                    */
                }
                else if (mileageExpenseTypeAccountCode.Contains(detail.ExpenseType.SUNAccountCode))
                {   // Mileage
                    rowConsumption.Visible = (vendor != null && vendor.UtilityProviderTypeId == UtilityProviderType.PARKING.Id ? false : true);
                    lblQty.Text = "Distance : ";
                    ddlUnit.bindList(ConsumptionType.getCollectionValuesForMileage(), "Name", "Id", "-1", "-- Please select --", "-1");
                    if (lblQty.Text == prevQtyLabel)
                        txtUnitCost.Text = "0";
                    txtUnitCost.Visible = false;
                    lblUnitCost.Visible = false;

                    //ddlFuelType.bindList(NTFuelType.getCollectionValues(), "Name", "Id", "1");
                    int officeId = (vwDomainNTInvoiceDef.NTInvoice.Office == null ? ddl_Office.selectedValueToInt : vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId);
                    ddlFuelType.bindList(NTFuelType.getCollectionValuesByOfficeId(officeId), "Name", "Id", "1"); 
                    ddlFuelType.selectByValue("");
                    ddlFuelType.Visible = true;
                    lblFuelType.Visible = true;
                    /*
                    ddlUnit.Enabled = false;
                    if (this.txt_SupplierName.NTVendorId == 900209) // siva agencies
                        ddlUnit.Enabled = true;
                    */
                }


                if (clearConsumption)   //(lblQty.Text != prevQtyLabel)
                {
                    txtQty.Text = "0";
                    txtUnitCost.Text = "0";
                    if (vendor != null && vendor.ConsumptionUnitId > 0)
                        ddlUnit.selectByValue(vendor.ConsumptionUnitId.ToString());
                    else
                        ddlUnit.selectByValue("-1");
                    if (ddlFuelType.Items.Count > 0)
                        ddlFuelType.selectByValue("1");
                }
                else
                {
                    txtQty.Text = detail.NoOfUnitConsumed.ToString();
                    if (detail.ConsumptionUnitId > 0)
                        ddlUnit.selectByValue(detail.ConsumptionUnitId.ToString());
                    else if (vendor != null && vendor.ConsumptionUnitId > 0)
                        ddlUnit.selectByValue(vendor.ConsumptionUnitId.ToString());
                    else
                        ddlUnit.selectByValue("-1");
                    ddlFuelType.selectByValue(detail.FuelTypeId > 0 ? detail.FuelTypeId.ToString() : "1");
                }
            }
            else
            {
                rowConsumption.Visible = false;
                txtQty.Text = "0";
                txtUnitCost.Text = "0";
            }

        }

        protected void InvoiceUpload_ItemCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                UserRef user = CommonUtil.getUserByKey(this.LogonUserId);

                if (vwDomainNTInvoiceDef.NTInvoice.Dept != null && user.Department.DepartmentId != vwDomainNTInvoiceDef.NTInvoice.Dept.DepartmentId)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RemoveNTInvoiceFile", "alert('You do not have the access right to delete this invoice file.');", true);
                    return;
                }

                AttachmentInfoDef attDef = (AttachmentInfoDef)vwInvoiceUploadList[Convert.ToInt32(e.CommandArgument)];

                if (attDef.AttachmentID != 0)
                {
                    long docId = vwDocId;
                    ArrayList queryStructs = new ArrayList();
                    queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Non-Trade-Expense"));
                    queryStructs.Add(new QueryStructDef("NSL Invoice No", vwDomainNTInvoiceDef.NTInvoice.NSLInvoiceNo));
                    queryStructs.Add(new QueryStructDef("Invoice No", vwDomainNTInvoiceDef.NTInvoice.InvoiceNo));
                    queryStructs.Add(new QueryStructDef("Customer No", vwDomainNTInvoiceDef.NTInvoice.CustomerNo));
                    queryStructs.Add(new QueryStructDef("Document Type", (vwDomainNTInvoiceDef.NTInvoice.DCIndicator == "D" ? "Non-Trade-Invoice" : "Non-Trade-Refund")));

                    DMSUtil.DeleteSingleAttachment(docId, queryStructs, attDef.FileName);

                    NonTradeManager.Instance.updateNTActionHistory(vwDomainNTInvoiceDef.NTInvoice.InvoiceId, -1, "Remove invoice file: " + attDef.FileName, this.LogonUserId);
                }

                vwInvoiceUploadList.Remove(attDef);
                gv_InvoiceUpload.DataSource = vwInvoiceUploadList;
                gv_InvoiceUpload.DataBind();

            }
        }

        protected void ActionHistoryDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NTActionHistoryDef historyDef = (NTActionHistoryDef)e.Row.DataItem;

                ((Label)e.Row.FindControl("lbl_ActionBy")).Text = CommonUtil.getUserByKey(historyDef.ActionBy).DisplayName;
            }
        }

        protected void AddRechargeDetail_Click(object sender, EventArgs e)
        {
            if (vwDomainNTInvoiceDef.NTRechargeDetailList != null)
            {
                if (!updateRechargeDetail(-1, true))
                    return;
            }
            else
                vwDomainNTInvoiceDef.NTRechargeDetailList = new ArrayList();

            NTInvoiceDetailDef detailDef = new NTInvoiceDetailDef();
            detailDef.IsRecharge = 1;
            if (vwDomainNTInvoiceDef.NTRechargeDetailList == null)
                vwDomainNTInvoiceDef.NTRechargeDetailList = new ArrayList();

            if (vwDomainNTInvoiceDef.NTInvoice.NTVendor != null && vwDomainNTInvoiceDef.NTInvoice.NTVendor.ExpenseType != null)
            {
                detailDef.ExpenseType = vwDomainNTInvoiceDef.NTInvoice.NTVendor.ExpenseType;
            }

            vwDomainNTInvoiceDef.NTRechargeDetailList.Add(detailDef);
            rep_RechargeDetail.DataSource = vwDomainNTInvoiceDef.NTRechargeDetailList;
            rep_RechargeDetail.DataBind();
        }

        protected void RefreshRechargeGrid(object sender, EventArgs e)
        {
            updateRechargeDetail(-1, true);
            rep_RechargeDetail.DataSource = vwDomainNTInvoiceDef.NTRechargeDetailList;
            rep_RechargeDetail.DataBind();
        }

        protected void RechargeDetailItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                if (!updateRechargeDetail(index, true))
                    return;

                NTInvoiceDetailDef  ntRechargeDetailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[index];
                if (ntRechargeDetailDef.InvoiceDetailId == 0)
                    vwDomainNTInvoiceDef.NTRechargeDetailList.RemoveAt(index);
                else
                    ntRechargeDetailDef.Status = 0;

                rep_RechargeDetail.DataSource = vwDomainNTInvoiceDef.NTRechargeDetailList;
                rep_RechargeDetail.DataBind();                
            }
            else if (e.CommandName == "copy")
            {
                if (!updateRechargeDetail(-1, false))
                    return;

                int index = Convert.ToInt32(e.CommandArgument);
                NTInvoiceDetailDef ntRechargeDetailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[index];
                NTInvoiceDetailDef newRechargeDetailDef = (NTInvoiceDetailDef)ntRechargeDetailDef.Clone();
                newRechargeDetailDef.InvoiceDetailId = 0;
                newRechargeDetailDef.Amount = 0;
                newRechargeDetailDef.VAT = 0;
                newRechargeDetailDef.RechargeDCNoteId = 0;

                vwDomainNTInvoiceDef.NTRechargeDetailList.Add(newRechargeDetailDef);
                rep_RechargeDetail.DataSource = vwDomainNTInvoiceDef.NTRechargeDetailList;
                rep_RechargeDetail.DataBind();                
            }
            else if (e.CommandName == "addItemDesc")
            {
                //updateRechargeDetail(-1, true);
                //NTInvoiceDetailDef ntInvoiceDetailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[Convert.ToInt32(e.CommandArgument)];

                //if (ntInvoiceDetailDef.ItemDescription3 == string.Empty)
                //    ntInvoiceDetailDef.ItemDescription3 = ".";
                //else if (ntInvoiceDetailDef.ItemDescription4 == string.Empty)
                //    ntInvoiceDetailDef.ItemDescription4 = ".";
                //else if (ntInvoiceDetailDef.ItemDescription5 == string.Empty)
                //    ntInvoiceDetailDef.ItemDescription5 = ".";

                //rep_RechargeDetail.DataSource = vwDomainNTInvoiceDef.NTRechargeDetailList;
                //rep_RechargeDetail.DataBind();

                NTInvoiceDetailDef ntInvoiceDetailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[Convert.ToInt32(e.CommandArgument)];

                if (((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc2")).Style["display"] == "none")
                {                    
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc2")).Style["display"] = "block";
                    ((TextBox)e.Item.FindControl("txt_ItemDescription2")).Text = string.Empty;
                }
                else if (ntInvoiceDetailDef.ItemDescription3 == string.Empty)
                {
                    ntInvoiceDetailDef.ItemDescription3 = ".";
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc3")).Style["display"] = "block";
                    ((TextBox)e.Item.FindControl("txt_ItemDescription3")).Text = string.Empty;
                }
                else if (ntInvoiceDetailDef.ItemDescription4 == string.Empty)
                {
                    ntInvoiceDetailDef.ItemDescription4 = ".";
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc4")).Style["display"] = "block";
                    ((TextBox)e.Item.FindControl("txt_ItemDescription4")).Text = string.Empty;
                }
                else if (ntInvoiceDetailDef.ItemDescription5 == string.Empty)
                {
                    ntInvoiceDetailDef.ItemDescription5 = ".";
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc5")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription5 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription5")).Text = string.Empty;
                }                
            }
            else if (e.CommandName == "removeItemDesc")
            {
                NTInvoiceDetailDef ntInvoiceDetailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[Convert.ToInt32(e.CommandArgument)];

                ImageButton imgButton = (ImageButton)e.CommandSource;

                string s_Desc2 = ((TextBox)e.Item.FindControl("txt_ItemDescription2")).Text;
                string s_Desc3 = ((TextBox)e.Item.FindControl("txt_ItemDescription3")).Text;
                string s_Desc4 = ((TextBox)e.Item.FindControl("txt_ItemDescription4")).Text;
                string s_Desc5 = ((TextBox)e.Item.FindControl("txt_ItemDescription5")).Text;

                ntInvoiceDetailDef.ItemDescription2 = s_Desc2;
                ntInvoiceDetailDef.ItemDescription3 = (s_Desc3 == string.Empty ? (ntInvoiceDetailDef.ItemDescription3 == "." ? "." : string.Empty) : s_Desc3);
                ntInvoiceDetailDef.ItemDescription4 = (s_Desc4 == string.Empty ? (ntInvoiceDetailDef.ItemDescription4 == "." ? "." : string.Empty) : s_Desc4);
                ntInvoiceDetailDef.ItemDescription5 = (s_Desc5 == string.Empty ? (ntInvoiceDetailDef.ItemDescription5 == "." ? "." : string.Empty) : s_Desc5);

                switch (imgButton.ID)
                {
                    case "btn_removeItemDesc2": ntInvoiceDetailDef.ItemDescription2 = (ntInvoiceDetailDef.ItemDescription3 == "." ? string.Empty : ntInvoiceDetailDef.ItemDescription3);
                        ((TextBox)e.Item.FindControl("txt_ItemDescription2")).Text = ntInvoiceDetailDef.ItemDescription2;
                        goto case "btn_removeItemDesc3";
                    case "btn_removeItemDesc3": ntInvoiceDetailDef.ItemDescription3 = ntInvoiceDetailDef.ItemDescription4;
                        ((TextBox)e.Item.FindControl("txt_ItemDescription3")).Text = ntInvoiceDetailDef.ItemDescription3;
                        goto case "btn_removeItemDesc4";
                    case "btn_removeItemDesc4": ntInvoiceDetailDef.ItemDescription4 = ntInvoiceDetailDef.ItemDescription5;
                        ((TextBox)e.Item.FindControl("txt_ItemDescription4")).Text = ntInvoiceDetailDef.ItemDescription4;
                        goto case "btn_removeItemDesc5";
                    case "btn_removeItemDesc5": ntInvoiceDetailDef.ItemDescription5 = string.Empty;
                        ((TextBox)e.Item.FindControl("txt_ItemDescription5")).Text = ntInvoiceDetailDef.ItemDescription5;
                        break;
                }

                if (imgButton.ID == "btn_removeItemDesc2" && ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc3")).Style["display"] == "none")
                {
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc2")).Style["display"] = "none";
                }
                else
                {
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc2")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription2 == string.Empty)
                        ((TextBox)e.Item.FindControl("txt_ItemDescription2")).Text = string.Empty;
                }
                if (ntInvoiceDetailDef.ItemDescription3 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc3")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription3 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription3")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc3")).Style["display"] = "none";
                if (ntInvoiceDetailDef.ItemDescription4 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc4")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription4 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription4")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc4")).Style["display"] = "none";
                if (ntInvoiceDetailDef.ItemDescription5 != string.Empty)
                {
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc5")).Style["display"] = "block";
                    if (ntInvoiceDetailDef.ItemDescription5 == ".")
                        ((TextBox)e.Item.FindControl("txt_ItemDescription5")).Text = string.Empty;
                }
                else
                    ((HtmlTableRow)e.Item.FindControl("row_RechargeItemDesc5")).Style["display"] = "none";

            }

        }        

        private bool updateInvoiceDetail(int removeIndex, bool isRefresh)
        {
            bool isValid = true;
            decimal amount = 0;
            NTInvoiceDetailDef invoiceDetailDef ;
            SmartDropDownList dropdown;
            com.next.isam.webapp.webservices.UclSmartSelection selectionList;
            totalAmountForCostCenter = 0;
            totalVATForCostCenter = 0;
            /* TODO:TRADINGAF */
            Hashtable tradingAFTbl = new Hashtable();
            

            foreach (RepeaterItem item in rep_InvoiceDetail.Items)
            {
                if (item.ItemIndex == removeIndex)
                    continue;

                invoiceDetailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTInvoiceDetailList[item.ItemIndex];
                if (invoiceDetailDef.Status == 0)
                    continue;


                if (!isRefresh)
                {
                    dropdown = (SmartDropDownList)item.FindControl("ddl_ExpenseType");
                    if (dropdown.SelectedValue == "-1")
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please select Expense Type');", true);
                        break;
                    }
                }
                                                
                isValid = decimal.TryParse(((TextBox)item.FindControl("txt_Amount")).Text, out amount);

                if (isValid && amount != 0 && (amount % Math.Round(amount, 2)) == 0)
                { 
                    //allow user input negative and positive value
                    invoiceDetailDef.Amount = amount;
                    totalAmountForCostCenter += amount;
                }
                else
                {
                    if (!isRefresh)
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Invalid Amount. It should be a non-zero number with a maximum of 2 decimal places.');", true);
                        break;
                    }
                }

                
                string s_amt = ((TextBox)item.FindControl("txt_VAT")).Text.Trim();
                isValid = decimal.TryParse((s_amt == string.Empty ? "0" : s_amt), out amount);
                if (isValid && (amount == 0 || (amount % Math.Round(amount, 2)) == 0))
                {
                    //allow user input zero, negative and positive value
                    invoiceDetailDef.VAT = amount;
                    totalVATForCostCenter += amount;
                }
                else
                {
                    if (!isRefresh)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Invalid VAT Amount. It should be a number with a maximum of 2 decimal places.');", true);
                        break;
                    }
                }

                
                TextBox textBox = (TextBox)item.FindControl("txt_ItemDescription1");
                invoiceDetailDef.ItemDescription1 =  textBox.Text.Trim();
                textBox = (TextBox)item.FindControl("txt_ItemDescription2");
                invoiceDetailDef.ItemDescription2 = textBox.Text.Trim();
                textBox = (TextBox)item.FindControl("txt_ItemDescription3");
                invoiceDetailDef.ItemDescription3 = !isRefresh ? textBox.Text.Trim() : (invoiceDetailDef.ItemDescription3 == "." && textBox.Text.Trim() == string.Empty ? "." : textBox.Text.Trim());
                textBox = (TextBox)item.FindControl("txt_ItemDescription4");
                invoiceDetailDef.ItemDescription4 = !isRefresh ? textBox.Text.Trim() : (invoiceDetailDef.ItemDescription4 == "." && textBox.Text.Trim() == string.Empty ? "." : textBox.Text.Trim());
                textBox = (TextBox)item.FindControl("txt_ItemDescription5");
                invoiceDetailDef.ItemDescription5 = !isRefresh ? textBox.Text.Trim() : (invoiceDetailDef.ItemDescription5 == "." && textBox.Text.Trim() == string.Empty ? "." : textBox.Text.Trim());

                if (!isRefresh && invoiceDetailDef.ItemDescription1 == string.Empty)
                {
                    isValid = false;
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please enter Item Description');", true);
                    break;
                }

                if (invoiceDetailDef.ExpenseType != null)
                {
                    if (invoiceDetailDef.ExpenseType.IsDepartmentCode == 1 && !isRefresh)
                    {
                        dropdown = (SmartDropDownList)item.FindControl("ddl_CostCenter");
                        if (dropdown.SelectedValue == "-1")
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please select Cost Centre.');", true);
                            break;
                        }
                    }

                    if (invoiceDetailDef.ExpenseType.IsProductCode == 1)
                    //if (invoiceDetailDef.ExpenseType.IsProductCode == 1 && invoiceDetailDef.ExpenseType.SUNAccountCode != "1452028")
                    {
                        selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)item.FindControl("ddl_ProductTeam");
                        if (selectionList.ProductCodeId == int.MinValue)
                        {
                            if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please select Product Team');", true);
                                break;
                            }
                        }
                        else if (CommonUtil.getProductCodeByKey(selectionList.ProductCodeId).Description.IndexOf("(DO NOT USE)") != -1)
                        {
                            if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Invalid Product Team');", true);
                                break;
                            }
                        }
                        else
                        {
                            invoiceDetailDef.ProductTeam = CommonUtil.getProductCodeByKey(selectionList.ProductCodeId);
                        }
                    }
                    else
                        invoiceDetailDef.ProductTeam = null;

                    if (invoiceDetailDef.ExpenseType.IsSeasonCode == 1 && invoiceDetailDef.ExpenseType.SUNAccountCode != "1452028")
                    {
                        dropdown = (SmartDropDownList)item.FindControl("ddl_Season");
                        if (dropdown.SelectedValue == "" || dropdown.SelectedValue == "-1")
                        {
                            if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please select Season');", true);
                                break;
                            }
                        }
                        else
                        {
                            invoiceDetailDef.Season = CommonUtil.getSeasonByKey(int.Parse(dropdown.SelectedValue));
                        }
                    }
                    else
                        invoiceDetailDef.Season = null;

                    if (invoiceDetailDef.ExpenseType.IsStaffCode == 1)
                    {
                        //selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)item.FindControl("txt_User");
                        if (invoiceDetailDef.User == null)
                        {
                            if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please select User');", true);
                                break;
                            }
                        }
                    }
                    else
                        invoiceDetailDef.User = null;

                    if (invoiceDetailDef.ExpenseType.IsItemNo == 1)
                    {
                        textBox = (TextBox)item.FindControl("txt_ItemNo");
                        if (textBox.Text.Trim() == string.Empty && !isRefresh)
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please enter Item No.');", true);
                            break;
                        }
                        else
                            invoiceDetailDef.ItemNo = textBox.Text.Trim();
                    }
                    else
                        invoiceDetailDef.ItemNo = string.Empty;

                    if (invoiceDetailDef.ExpenseType.IsDevSampleCostType == 1)
                    {
                        dropdown = (SmartDropDownList)item.FindControl("ddl_DevSampleType");
                        if (dropdown.SelectedValue == "" || dropdown.SelectedValue == "-1")
                        {
                            if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please select Dev Sample Type');", true);
                                break;
                            }
                        }
                        else
                        {
                            invoiceDetailDef.DevSampleCostTypeId = int.Parse(dropdown.SelectedValue);
                        }
                    }
                    else
                        invoiceDetailDef.DevSampleCostTypeId = 0;

                    if (invoiceDetailDef.ExpenseType.IsQtyRequired == 1)
                    {
                        textBox = (TextBox)item.FindControl("txt_Quantity");
                        int qty = 0;
                        if ((textBox.Text.Trim() == string.Empty || !int.TryParse(textBox.Text, out qty)) && !isRefresh)
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please enter Quantity');", true);
                            break;
                        }
                        else
                            invoiceDetailDef.Quantity = qty;
                    }
                    else
                        invoiceDetailDef.Quantity = 0;

                    if (invoiceDetailDef.ExpenseType.SUNAccountCode == "1412101")
                    {
                        dropdown = (SmartDropDownList)item.FindControl("ddl_Nature");
                        if (dropdown.SelectedValue == "" || dropdown.SelectedValue == "-1")
                        {
                            if (isUIForAccount() && !isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please select Expense Nature for Accrual');", true);
                                break;
                            }
                        }
                        else
                        {
                            invoiceDetailDef.NatureIdForAccrual = int.Parse(dropdown.SelectedValue);
                        }
                    }
                    else
                        invoiceDetailDef.NatureIdForAccrual = 0;

                    /* TODO:TRADINGAF */
                    if (invoiceDetailDef.ExpenseType.SUNAccountCode == "1452028")
                    {
                        string contractNo = ((TextBox)item.FindControl("txtContractNo")).Text.Trim();
                        int deliveryNo = 0;
                        int.TryParse(((TextBox)item.FindControl("txtDeliveryNo")).Text.Trim(), out deliveryNo);
                        List<NTInvoiceDetailDef> currentList = NonTradeManager.Instance.getNTInvoiceTradingAFDetailByContractDeliveryNo(contractNo, deliveryNo);
                        NTInvoiceDetailDef currentDef = null;
                        foreach (NTInvoiceDetailDef dtlDef in currentList)
                        {
                            if (dtlDef.InvoiceDetailId != invoiceDetailDef.InvoiceDetailId)
                            {
                                currentDef = currentList[0];
                                break;
                            }
                        }

                        ShipmentDef shipmentDef = OrderManager.Instance.getShipmentByContractNoAndDeliveryNo(contractNo, deliveryNo);

                        if (!isRefresh && (shipmentDef == null || !shipmentDef.IsTradingAirFreight))
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Trading air freight shipment not found');", true);
                            break;
                        }
                        /* 2016-04-26
                        if (!isRefresh && (shipmentDef == null || shipmentDef.WorkflowStatus.Id != ContractWFS.INVOICED.Id))
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Trading air freight shipment has not been invoiced');", true);
                            break;
                        }
                        */
                        else if (!isRefresh && tradingAFTbl.ContainsKey(contractNo + "-" + deliveryNo.ToString()))
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Duplicate trading air freight shipment');", true);
                            break;
                        }
                        /*
                        else if (!isRefresh && currentDef != null)
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Trading air freight shipment has been referenced by another non-trade invoice');", true);
                            break;
                        }
                        */
                        else
                        {
                            ContractDef contractDef = OrderManager.Instance.getContractByKey(shipmentDef.ContractId);

                            invoiceDetailDef.ProductTeam = CommonUtil.getProductCodeByKey(contractDef.ProductTeam.ProductCodeId);
                            invoiceDetailDef.Season = contractDef.Season;

                            invoiceDetailDef.ContractNo = contractNo;
                            invoiceDetailDef.DeliveryNo = deliveryNo;
                            invoiceDetailDef.SegmentValue7 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(int.Parse(((SmartDropDownList)item.FindControl("ddl_SegmentValue7")).SelectedValue));
                            invoiceDetailDef.SegmentValue8 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(int.Parse(((SmartDropDownList)item.FindControl("ddl_SegmentValue8")).SelectedValue));
                            tradingAFTbl.Add(contractNo + "-" + deliveryNo.ToString(), contractNo + "-" + deliveryNo.ToString());
                        }
                    }
                    else
                    {
                        invoiceDetailDef.ContractNo = string.Empty;
                        invoiceDetailDef.DeliveryNo = 0;
                    }
                    
                    /* TODO:LK Provision for Air Freight */
                    if (invoiceDetailDef.ExpenseType.SUNAccountCode == "1412221" && this.ddl_Office.selectedValueToInt == OfficeId.SL.Id)
                    {
                        this.ddl_Approver.selectByValue("1727"); // Ivan chong
                    }



                    if (invoiceDetailDef.ExpenseType.IsSegmentValue == 1)
                    {
                        dropdown = (SmartDropDownList)item.FindControl("ddl_SegmentValue7");
                        if (dropdown.SelectedValue != "-1")
                            invoiceDetailDef.SegmentValue7 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(int.Parse(dropdown.SelectedValue));

                        dropdown = (SmartDropDownList)item.FindControl("ddl_SegmentValue8");
                        if (dropdown.SelectedValue != "-1")
                            invoiceDetailDef.SegmentValue8 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(int.Parse(dropdown.SelectedValue));

                        if ((invoiceDetailDef.SegmentValue7 == null || invoiceDetailDef.SegmentValue8 == null ) && isUIForAccount() && !isRefresh)
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please select Segment Value');", true);
                            break;
                        }
                    }
                    else
                    {
                        invoiceDetailDef.SegmentValue7 = null;
                        invoiceDetailDef.SegmentValue8 = null;
                    }

                    //if (invoiceDetailDef.ExpenseType.SUNAccountCode == "4102301")
                    if (invoiceDetailDef.ExpenseType != null && carbonEmissionExpenseTypeAccountCode.Contains(invoiceDetailDef.ExpenseType.SUNAccountCode))
                    {
                        decimal val;
                        SmartDropDownList ddlUnit = (SmartDropDownList)item.FindControl("ddl_ConsumptionUnit");
                        SmartDropDownList ddlFuelType = (SmartDropDownList)item.FindControl("ddl_FuelType");
                        TextBox txtQty = (TextBox)item.FindControl("txtConsumptionQty");
                        TextBox txtCost = (TextBox)item.FindControl("txtConsumptionUnitCost");
                        invoiceDetailDef.ConsumptionUnitId = ddlUnit.selectedValueToInt;
                        invoiceDetailDef.NoOfUnitConsumed = (decimal.TryParse(txtQty.Text, out val) ? val : 0);
                        invoiceDetailDef.ConsumptionUnitCost = (decimal.TryParse(txtCost.Text, out val) ? val : 0);
                        invoiceDetailDef.FuelTypeId = (ddlFuelType.Items.Count == 0 ? -1 : ddlFuelType.selectedValueToInt);
                    }
                    else
                    {
                        invoiceDetailDef.ConsumptionUnitId = -1;
                        invoiceDetailDef.NoOfUnitConsumed = 0;
                        invoiceDetailDef.ConsumptionUnitCost = 0;
                        invoiceDetailDef.FuelTypeId = -1;
                    }

                }
            }
            return isValid;
        }


        private bool isUIForAccount()
        {
            bool isFirstApproverUser = (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(ddl_Office.selectedValueToInt));

            return ((isFirstApproverUser && vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DEPARTMENT_APPROVED.Id) 
                || vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_RECEIVED.Id
                || vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_APPROVED.Id
                || vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_EVALUATING.Id
                || vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.SETTLED.Id);
        }

        private bool updateRechargeDetail(int removeIndex, bool isRefresh)
        {
            bool isValid = true;
            decimal amount = 0;
            totalAmountForRecharge = 0;
            totalVATForRecharge = 0;
            NTInvoiceDetailDef detailDef;
            int rechargeTypeId = -1;
            /* TODO:TRADINGAF */
            Hashtable tradingAFTbl = new Hashtable();

            foreach (RepeaterItem item in rep_RechargeDetail.Items)
            {
                if (item.ItemIndex == removeIndex)
                    continue;

                detailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[item.ItemIndex];
                if (detailDef.Status == 0 || detailDef.IsPayByHK == 1)
                    continue;

                com.next.isam.webapp.webservices.UclSmartSelection selectionList;
                SmartDropDownList dropdown;

                if (!isRefresh)
                {
                    dropdown = (SmartDropDownList)item.FindControl("ddl_ExpenseType");
                    if (dropdown.SelectedValue == "-1")
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Please select Expense Type');", true);
                        break;
                    }
                }
                
                //detailDef.InvoiceDetailType = NTInvoiceDetailType.getType(int.Parse(((SmartDropDownList)item.FindControl("ddl_RechargeType")).SelectedValue));
                rechargeTypeId = detailDef.InvoiceDetailType == null ? -1 : detailDef.InvoiceDetailType.Id;

                DropDownList ddl = (DropDownList)item.FindControl("ddl_RechargeCurrency");
                detailDef.RechargeCurrency = CommonUtil.getCurrencyByKey(int.Parse(ddl.SelectedValue));

                if (rechargeTypeId == NTInvoiceDetailType.USER.Id ||
                    (detailDef.ExpenseType != null && ((rechargeTypeId == NTInvoiceDetailType.OFFICE.Id && detailDef.ExpenseType.IsStaffCode == 1) ||
                    (detailDef.ExpenseType.SUNAccountCode == "1311303" || detailDef.ExpenseType.SUNAccountCode == "1311307"))))
                {
                    selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)item.FindControl("txt_User");
                    if (selectionList.UserId != int.MinValue)
                    {
                        if (selectionList.UserId == -1)
                        {
                            detailDef.User = new UserRef();
                            detailDef.User.UserId = -1;
                            detailDef.User.DisplayName = "UNCLASSIFIED";
                        }
                        else
                            detailDef.User = CommonUtil.getUserByKey(selectionList.UserId);
                    }
                    else if (!isRefresh)
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select User.');", true);
                        break;
                    }
                }
                else
                    detailDef.User = null;

                if (rechargeTypeId == NTInvoiceDetailType.OFFICE.Id)
                {
                    if (detailDef.User != null && detailDef.Office == null)
                    {
                        detailDef.Office = detailDef.User.Department.Office;
                        dropdown = (SmartDropDownList)item.FindControl("ddl_BusinessEntity");
                        if (dropdown.SelectedValue != string.Empty)
                            detailDef.Company = CompanyType.getType(int.Parse(dropdown.SelectedValue));
                        else
                        {
                            ArrayList compList = CommonUtil.getCompanyList(detailDef.Office.OfficeId);
                            if (compList.Count > 0)
                                detailDef.Company = CompanyType.getType(((CompanyRef)compList[0]).CompanyId);
                        }
                    }
                    else if (!isRefresh)
                    {
                        if (detailDef.Office == null)
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select Office.');", true);
                            break;
                        }
                        else if  (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.OFFICE.Id && detailDef.Office.OfficeId == OfficeId.UK.Id && detailDef.RechargeCurrency.CurrencyId != CurrencyId.GBP.Id)
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Recharge currency for NSL UK must be GBP.');", true);
                            break;
                        }
                        /*
                        else if (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.OFFICE.Id && detailDef.Office.OfficeId == OfficeId.CA.Id && ddl_Office.selectedValueToInt == OfficeId.HK.Id)
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Recharge to CA office is not allowed.');", true);
                            break;
                        }
                        */
                    }

                    if (detailDef.ExpenseType != null)
                    {
                        if (detailDef.ExpenseType.IsProductCode == 1)
                        //if (detailDef.ExpenseType.IsProductCode == 1 && detailDef.ExpenseType.SUNAccountCode != "1452028")
                        {
                            selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)item.FindControl("txt_ProductTeam");

                            if (selectionList.ProductCodeId != int.MinValue && CommonUtil.getProductCodeByKey(selectionList.ProductCodeId).Description.IndexOf("(DO NOT USE)") != -1)
                            {
                                if (!isRefresh)
                                {
                                    isValid = false;
                                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Invalid Product Team');", true);
                                    break;
                                }
                            }
                            else if (selectionList.ProductCodeId != int.MinValue)
                            {
                                detailDef.ProductTeam = CommonUtil.getProductCodeByKey(selectionList.ProductCodeId);
                            }
                            else if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select Product Team.');", true);
                                break;
                            }
                        }
                        else
                            detailDef.ProductTeam = null;

                        if (detailDef.ExpenseType.IsDepartmentCode == 1)
                        {
                            //if (detailDef.ProductTeam != null)
                            //{
                            //    detailDef.Department = CommonUtil.getDepartmentByKey(detailDef.ProductTeam.ProductDepartmentId);
                            //}
                            //else
                            //{
                            dropdown = (SmartDropDownList)item.FindControl("ddl_CostCenter");
                            if (dropdown.SelectedValue != "" && dropdown.SelectedValue != "-1")
                            {
                                detailDef.CostCenter = CommonUtil.getCostCenterByKey(dropdown.selectedValueToInt);
                                if (detailDef.CostCenter.DepartmentId != 0)
                                    detailDef.Department = CommonUtil.getDepartmentByKey(detailDef.CostCenter.DepartmentId);
                                else
                                    detailDef.Department = null;
                            }
                            else if (detailDef.User != null && detailDef.User.UserId != -1 && detailDef.CostCenter == null)
                            {
                                detailDef.CostCenter = detailDef.User.CostCenter;
                                detailDef.Department = CommonUtil.getDepartmentByKey(detailDef.CostCenter.DepartmentId);
                            }
                            else if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select Recharge Cost Centre.');", true);
                                break;
                            }
                            //}
                        }
                        else
                            detailDef.CostCenter = null;


                        if (detailDef.ExpenseType.IsSeasonCode == 1 && detailDef.ExpenseType.SUNAccountCode != "1452028")
                        {
                            dropdown = (SmartDropDownList)item.FindControl("ddl_Season");
                            if (dropdown.SelectedValue != "" && dropdown.SelectedValue != "-1")
                            {
                                detailDef.Season = CommonUtil.getSeasonByKey(int.Parse(dropdown.SelectedValue));
                            }
                            else if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select Season.');", true);
                                break;
                            }
                        }
                        else
                            detailDef.Season = null;

                        if (detailDef.ExpenseType.IsItemNo == 1)
                        {
                            TextBox tBox = (TextBox)item.FindControl("txt_ItemNo");
                            if (tBox.Text.Trim() != string.Empty)
                            {
                                detailDef.ItemNo = tBox.Text.Trim();
                            }
                            else if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeDetail", "alert('Please enter Item No.');", true);
                                break;
                            }
                        }
                        else
                            detailDef.ItemNo = string.Empty;

                        if (detailDef.ExpenseType.IsDevSampleCostType == 1)
                        {
                            dropdown = (SmartDropDownList)item.FindControl("ddl_DevSampleType");
                            if (dropdown.SelectedValue != "" && dropdown.SelectedValue != "-1")
                            {
                                detailDef.DevSampleCostTypeId = int.Parse(dropdown.SelectedValue);
                            }
                            else if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select Dev Sample Type.');", true);
                                break;
                            }
                        }
                        else
                            detailDef.DevSampleCostTypeId = 0;

                        if (detailDef.ExpenseType.IsQtyRequired == 1)
                        {
                            TextBox tBox = (TextBox)item.FindControl("txt_Quantity");
                            int qty = 0;
                            if (tBox.Text.Trim() != string.Empty && int.TryParse(tBox.Text.Trim(), out qty))
                            {
                                detailDef.Quantity = qty;
                            }
                            else if (!isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeDetail", "alert('Please enter Quantity');", true);
                                break;
                            }
                        }
                        else
                            detailDef.Quantity = 0;


                        if (detailDef.ExpenseType.SUNAccountCode == "1412101")
                        {
                            dropdown = (SmartDropDownList)item.FindControl("ddl_Nature");

                            if (dropdown.SelectedValue != "" && dropdown.SelectedValue != "-1")
                            {
                                detailDef.NatureIdForAccrual = int.Parse(dropdown.SelectedValue);
                            }
                            else if (isUIForAccount() && !isRefresh)
                            {
                                isValid = false;
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select Expense Nature for Accrual.');", true);
                                break;
                            }
                        }
                        else
                            detailDef.NatureIdForAccrual = 0;

                        
                        /* TODO:Provision for Air Freight - LK */
                        if (detailDef.ExpenseType.SUNAccountCode == "1412221" && this.ddl_Office.selectedValueToInt == OfficeId.SL.Id)
                        {
                            this.ddl_Approver.selectByValue("1727"); // Ivan Chong
                        }
                    }          
                }
                else if (detailDef.ExpenseType != null && detailDef.ExpenseType.IsOfficeCode == 0)
                    detailDef.Office = null;

                /* TODO:TRADINGAF */
                if (detailDef.ExpenseType != null && detailDef.ExpenseType.SUNAccountCode == "1452028")
                {
                    string contractNo = ((TextBox)item.FindControl("txtContractNo")).Text.Trim();
                    int deliveryNo = 0;
                    int.TryParse(((TextBox)item.FindControl("txtDeliveryNo")).Text.Trim(), out deliveryNo);
                    List<NTInvoiceDetailDef> currentList = NonTradeManager.Instance.getNTInvoiceTradingAFDetailByContractDeliveryNo(contractNo, deliveryNo);
                    NTInvoiceDetailDef currentDef = null;
                    foreach (NTInvoiceDetailDef dtlDef in currentList)
                    {
                        if (dtlDef.InvoiceDetailId != detailDef.InvoiceDetailId)
                        {
                            currentDef = currentList[0];
                            break;
                        }
                    }

                    if (currentList.Count > 0)
                        currentDef = currentList[0];

                    ShipmentDef shipmentDef = OrderManager.Instance.getShipmentByContractNoAndDeliveryNo(contractNo, deliveryNo);

                    if (!isRefresh && (shipmentDef == null || !shipmentDef.IsTradingAirFreight))
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Trading air freight shipment not found');", true);
                        break;
                    }
                    /* 2016-04-26
                    if (!isRefresh && (shipmentDef == null || shipmentDef.WorkflowStatus.Id != ContractWFS.INVOICED.Id))
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Trading air freight shipment has not been invoiced');", true);
                        break;
                    }
                    */
                    else if (!isRefresh && tradingAFTbl.ContainsKey(contractNo + "-" + deliveryNo.ToString()))
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Duplicate trading air freight shipment');", true);
                        break;
                    }
                    /*
                    else if (!isRefresh && currentDef != null)
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceDetail", "alert('Trading air freight shipment has been referenced by another non-trade invoice');", true);
                        break;
                    }
                    */
                    else
                    {
                        ContractDef contractDef = OrderManager.Instance.getContractByKey(shipmentDef.ContractId);

                        detailDef.ProductTeam = CommonUtil.getProductCodeByKey(contractDef.ProductTeam.ProductCodeId);
                        detailDef.Season = contractDef.Season;

                        detailDef.ContractNo = contractNo;
                        detailDef.DeliveryNo = deliveryNo;
                        detailDef.SegmentValue7 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(int.Parse(((SmartDropDownList)item.FindControl("ddl_SegmentValue7")).SelectedValue));
                        detailDef.SegmentValue8 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(int.Parse(((SmartDropDownList)item.FindControl("ddl_SegmentValue8")).SelectedValue));
                        tradingAFTbl.Add(contractNo + "-" + deliveryNo.ToString(), contractNo + "-" + deliveryNo.ToString());
                    }
                }
                else
                {
                    detailDef.ContractNo = string.Empty;
                    detailDef.DeliveryNo = 0;
                }


                if (rechargeTypeId == NTInvoiceDetailType.CUSTOMER.Id)
                {
                    dropdown = (SmartDropDownList)item.FindControl("ddl_Customer");
                    if (dropdown.SelectedValue != "" && dropdown.SelectedValue != "-1")
                    {
                        detailDef.Customer = WebUtil.getCustomerByKey(int.Parse(dropdown.SelectedValue));

                        SmartDropDownList ddl_Intercomm = (SmartDropDownList)item.FindControl("ddl_Intercomm_Cust");
                        detailDef.IntercommOfficeId = -1;
                        if (NTExpenseTypeRef.isOtherReceivableRecharge(detailDef.ExpenseType))
                        {
                            if (ddl_Intercomm.selectedValueToInt == -1)
                                detailDef.IntercommOfficeId = this.ddl_Office.selectedValueToInt;
                            else
                                detailDef.IntercommOfficeId = ddl_Intercomm.selectedValueToInt;
                        }

                    }
                    else if (!isRefresh)
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select Customer.');", true);
                        break;
                    }
                }
                else                
                    detailDef.Customer = null;


                if (rechargeTypeId != NTInvoiceDetailType.USER.Id && rechargeTypeId != NTInvoiceDetailType.OFFICE.Id && rechargeTypeId != NTInvoiceDetailType.CUSTOMER.Id)
                {
                    selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)item.FindControl("txt_Vendor");

                    SmartDropDownList ddl_Intercomm = (SmartDropDownList)item.FindControl("ddl_Intercomm");

                    detailDef.IntercommOfficeId = -1;
                    if (NTExpenseTypeRef.isOtherReceivableRecharge(detailDef.ExpenseType))
                    {
                        if (ddl_Intercomm.selectedValueToInt == -1)
                            detailDef.IntercommOfficeId = this.ddl_Office.selectedValueToInt;
                        else
                            detailDef.IntercommOfficeId = ddl_Intercomm.selectedValueToInt;
                    }

                    if (rechargeTypeId == NTInvoiceDetailType.NT_VENDOR.Id)
                    {
                        if (selectionList.NTVendorId != int.MinValue)
                        {
                            detailDef.NTVendor = WebUtil.getNTVendorByKey(selectionList.NTVendorId);
                            if (detailDef.NTVendor.Office.OfficeId != ddl_Office.selectedValueToInt)
                            {
                                detailDef.NTVendor = null;
                                if (!isRefresh)
                                {
                                    isValid = false;
                                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select Recharge Non-Trade Vendor.');", true);
                                    break;
                                }
                            }
                        }
                        else if (!isRefresh)
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select Recharge Non-Trade Vendor.');", true);
                            break;
                        }
                        detailDef.Vendor = null;
                    }
                    else
                    {
                        if (selectionList.VendorId != int.MinValue)
                        {
                            detailDef.Vendor = IndustryUtil.getVendorByKey(selectionList.VendorId);
                        }
                        else if (!isRefresh)
                        {
                            isValid = false;
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please select Vendor.');", true);
                            break;
                        }
                        detailDef.NTVendor = null;
                    }
                }
                else
                {
                    detailDef.Vendor = null;
                    detailDef.NTVendor = null;
                }

                if (rechargeTypeId != NTInvoiceDetailType.OFFICE.Id && rechargeTypeId != NTInvoiceDetailType.USER.Id)
                {
                    detailDef.ContactPerson = ((TextBox)item.FindControl("txt_ContactPerson")).Text.Trim();

                    if (detailDef.ContactPerson == string.Empty && !isRefresh)
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Please enter Contact Person.');", true);
                        break;

                    }
                }
                else
                    detailDef.ContactPerson = string.Empty;
      
                
                isValid = decimal.TryParse(((TextBox)item.FindControl("txt_RechargeAmt")).Text, out amount);
                if (isValid)    // && amount != 0 && (amount % Math.Round(amount, 2)) == 0          (Allow zero recharge amount)
                {
                    //allow user input negative and positive value (also allow zero)
                    detailDef.Amount = amount;
                    totalAmountForRecharge += amount;
                }
                else if (!isRefresh)
                {
                    isValid = false;
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Invalid recharge amount.  It should be a non-zero number with a maximum of 2 decimal places.');", true);
                    break;
                }

                string s_amt = ((TextBox)item.FindControl("txt_VAT")).Text.Trim();
                isValid = decimal.TryParse((s_amt == string.Empty ? "0" : s_amt), out amount);
                if (isValid && (amount == 0 || (amount % Math.Round(amount, 2)) == 0))
                {
                    //allow user input zero, negative and positive value
                    detailDef.VAT = amount;
                    totalVATForRecharge += amount;
                }
                else if (!isRefresh)
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeParty", "alert('Invalid VAT amount. It should be a number with a maximum of 2 decimal places.');", true);

                TextBox textBox = (TextBox)item.FindControl("txt_ItemDescription1");
                detailDef.ItemDescription1 = textBox.Text.Trim();
                textBox = (TextBox)item.FindControl("txt_ItemDescription2");
                detailDef.ItemDescription2 = textBox.Text.Trim();
                textBox = (TextBox)item.FindControl("txt_ItemDescription3");
                detailDef.ItemDescription3 = !isRefresh ? textBox.Text.Trim() : (detailDef.ItemDescription3 == "." && textBox.Text.Trim() == string.Empty ? "." : textBox.Text.Trim());
                textBox = (TextBox)item.FindControl("txt_ItemDescription4");
                detailDef.ItemDescription4 = !isRefresh ? textBox.Text.Trim() : (detailDef.ItemDescription4 == "." && textBox.Text.Trim() == string.Empty ? "." : textBox.Text.Trim());
                textBox = (TextBox)item.FindControl("txt_ItemDescription5");
                detailDef.ItemDescription5 = !isRefresh ? textBox.Text.Trim() : (detailDef.ItemDescription5 == "." && textBox.Text.Trim() == string.Empty ? "." : textBox.Text.Trim());

                if (!isRefresh && detailDef.ItemDescription1 == string.Empty)
                {
                    isValid = false;
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeDetail", "alert('Please enter Item Description.');", true);
                    break;
                }

                if (detailDef.ExpenseType != null && detailDef.ExpenseType.IsSegmentValue == 1)
                {
                    dropdown = (SmartDropDownList)item.FindControl("ddl_SegmentValue7");
                    if (dropdown.SelectedValue != "-1")
                        detailDef.SegmentValue7 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(int.Parse(dropdown.SelectedValue));

                    dropdown = (SmartDropDownList)item.FindControl("ddl_SegmentValue8");
                    if (dropdown.SelectedValue != "-1")
                        detailDef.SegmentValue8 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(int.Parse(dropdown.SelectedValue));

                    if ((detailDef.SegmentValue7 == null || detailDef.SegmentValue8 == null) && isUIForAccount() && !isRefresh)
                    {
                        isValid = false;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RechargeDetail", "alert('Please select Segment value.');", true);
                        break;
                    }
                }
                else
                {
                    detailDef.SegmentValue7 = null;
                    detailDef.SegmentValue8 = null;
                }

                //if (detailDef.ExpenseType.SUNAccountCode == "4102301")
                if (detailDef.ExpenseType != null && carbonEmissionExpenseTypeAccountCode.Contains(detailDef.ExpenseType.SUNAccountCode))
                {
                    decimal val;
                    SmartDropDownList ddlUnit = (SmartDropDownList)item.FindControl("ddl_ConsumptionUnit");
                    SmartDropDownList ddlFuelType = (SmartDropDownList)item.FindControl("ddl_FuelType");
                    TextBox txtQty = (TextBox)item.FindControl("txtConsumptionQty");
                    TextBox txtCost = (TextBox)item.FindControl("txtConsumptionUnitCost");
                    detailDef.ConsumptionUnitId = ddlUnit.selectedValueToInt;
                    detailDef.NoOfUnitConsumed = (decimal.TryParse(txtQty.Text, out val) ? val : 0);
                    detailDef.ConsumptionUnitCost = (decimal.TryParse(txtCost.Text, out val) ? val : 0);
                    detailDef.FuelTypeId = (ddlFuelType.Items.Count == 0 ? -1 : ddlFuelType.selectedValueToInt);
                }
                else
                {
                    detailDef.ConsumptionUnitId = -1;
                    detailDef.NoOfUnitConsumed = 0;
                    detailDef.ConsumptionUnitCost = 0;
                    detailDef.FuelTypeId = -1;
                }


            }

            return isValid;
        }

        protected void Save()
        {
            NTInvoiceDef invoiceDef = vwDomainNTInvoiceDef.NTInvoice;

            vwDomainNTInvoiceDef.NTInvoice.NTVendor = WebUtil.getNTVendorByKey(txt_SupplierName.NTVendorId);

            invoiceDef.Office = CommonUtil.getOfficeRefByKey(int.Parse(ddl_Office.SelectedValue));
            if (ddl_BusinessEntity.Items.Count > 0)
                invoiceDef.Company = CompanyType.getType(int.Parse(ddl_BusinessEntity.SelectedValue));
            if (invoiceDef.InvoiceId == 0)
                invoiceDef.Dept = CommonUtil.getUserByKey(this.LogonUserId).Department;
            invoiceDef.InvoiceNo = txt_InvoiceNo.Text.Trim();
            invoiceDef.CustomerNo = txt_CustomerNo.Text.Trim();
            invoiceDef.InvoiceDate = DateTimeUtility.getDate(txt_InvoiceDate.Text);
            invoiceDef.DueDate = txt_DueDate.Text.Trim() == string.Empty ? DateTime.MinValue : DateTimeUtility.getDate(txt_DueDate.Text);
            invoiceDef.Currency = CommonUtil.getCurrencyByKey(int.Parse(ddl_Currency.SelectedValue));
            invoiceDef.Amount = decimal.Parse(txt_Amount.Text);
            invoiceDef.TotalVAT = txt_TotalVAT.Text.Trim() == string.Empty ? 0 : decimal.Parse(txt_TotalVAT.Text);
            invoiceDef.PaymentMethod = NTPaymentMethodRef.getType(int.Parse(ddl_PaymentMethod.SelectedValue));
            invoiceDef.PaymentFromDate = new DateTime(int.Parse(ddl_PaymentYearFrom.SelectedValue), int.Parse(ddl_PaymentMonthFrom.SelectedValue), 1);
            invoiceDef.PaymentToDate = new DateTime(int.Parse(ddl_PaymentYearTo.SelectedValue), int.Parse(ddl_PaymentMonthTo.SelectedValue), 1);
            invoiceDef.InvoiceReceivedDate = DateTimeUtility.getDate(txt_InvoiceReceivedDate.Text);
            invoiceDef.DCIndicator = ddl_DocumentType.SelectedValue;
            invoiceDef.RejectReason = txt_RejectReason.Text.Trim();
            invoiceDef.UserRemark = txt_Remark.Text.Trim();
            invoiceDef.IsPayByHK = ckb_PayByHK.Checked ? 1 : 0;
            invoiceDef.ReleaseReason = txt_ReleaseReason.Text.Trim();
            invoiceDef.JournalNo = txt_JournalNo.Text.Trim();

            /* todo */

            if (this.txtProcurementRequestNo.Text.Trim() != string.Empty)
            {
                APDS.APDSService svc = new APDS.APDSService();
                APDS.ProcurementRequestDef requestDef = svc.GetApprovedProcurementRequestDefByRequestNo(this.ddl_Office.selectedValueToInt, this.txtProcurementRequestNo.Text.Trim());

                invoiceDef.ProcurementRequestId = requestDef.RequestId;
            }
            else
                invoiceDef.ProcurementRequestId = -1;

            //insert recharge office entry for the other business entities's invoice which paid by HK
            NTInvoiceDetailDef rechargeDef = null;
            if (invoiceDef.IsPayByHK == 1 && invoiceDef.Company.Id != CompanyType.NEXT_SOURCING.Id)
            {
                if (vwDomainNTInvoiceDef.NTRechargeDetailList != null)
                {
                    foreach (NTInvoiceDetailDef def in vwDomainNTInvoiceDef.NTRechargeDetailList)
                    {
                        if (def.IsPayByHK == 1)
                        {
                            rechargeDef = def;
                            break;
                        }
                        else
                            continue;
                    }
                }

                if (rechargeDef == null)
                {
                    rechargeDef = new NTInvoiceDetailDef(NTInvoiceDetailType.OFFICE);
                    rechargeDef.IsPayByHK = 1;
                    if (vwDomainNTInvoiceDef.NTRechargeDetailList == null)
                        vwDomainNTInvoiceDef.NTRechargeDetailList = ConvertUtility.createArrayList(rechargeDef);
                    else
                        vwDomainNTInvoiceDef.NTRechargeDetailList.Add(rechargeDef);
                }

                rechargeDef.InvoiceId = vwDomainNTInvoiceDef.NTInvoice.InvoiceId;
                rechargeDef.IsRecharge = 1;
                rechargeDef.Office = invoiceDef.Office;
                rechargeDef.Company = invoiceDef.Company;
                rechargeDef.RechargeCurrency = invoiceDef.Currency;
                rechargeDef.Amount = invoiceDef.Amount;
                rechargeDef.VAT = invoiceDef.TotalVAT;
                rechargeDef.Status = 1;
            }
            else
            {
                if (vwDomainNTInvoiceDef.NTRechargeDetailList != null)
                {
                    foreach (NTInvoiceDetailDef def in vwDomainNTInvoiceDef.NTRechargeDetailList)
                    {
                        if (def.IsPayByHK == 1)
                        {
                            rechargeDef = def;
                            break;
                        }
                        else
                            continue;
                    }

                    if (rechargeDef != null)
                    {
                        if (rechargeDef.InvoiceDetailId != 0)
                            rechargeDef.Status = 0;
                        else
                            vwDomainNTInvoiceDef.NTRechargeDetailList.Remove(rechargeDef);
                    }
                }
            }


            if (ddl_Approver.SelectedValue != "-1")
            {
                invoiceDef.Approver = CommonUtil.getUserByKey(int.Parse(ddl_Approver.SelectedValue));
            }
            else
                invoiceDef.Approver = null;

            //if (invoiceDef.WorkflowStatus.Id == NTWFS.ACCOUNTS_APPROVED.Id)
            //{
            //    invoiceDef.SettlementDate = DateTimeUtility.getDate(txt_SettlementDate.Text);
            //    invoiceDef.SettlementAmount = txt_SettleAmt.Text.Trim() == string.Empty ? 0 : decimal.Parse(txt_SettleAmt.Text);
            //    //invoiceDef.SettlementRefNo = txt_SettlementRefNo.Text.Trim();

            //    if (invoiceDef.SettlementDate != DateTime.MinValue)
            //    {
            //        invoiceDef.WorkflowStatus = NTWFS.SETTLED;
            //    }
            //}

            if (invoiceDef.InvoiceId == 0)
            {
                invoiceDef.WorkflowStatus = NTWFS.DRAFT;
                invoiceDef.Status = GeneralCriteria.TRUE;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateNTInvoice);

            Context.Items.Add(AccountCommander.Param.ntInvoice, invoiceDef);
            Context.Items.Add(AccountCommander.Param.ntInvoiceDetailList, vwDomainNTInvoiceDef.NTInvoiceDetailList);
            Context.Items.Add(AccountCommander.Param.ntRechargeDetailList, vwDomainNTInvoiceDef.NTRechargeDetailList);

            
            forwardToScreen(null);

        }

        protected void btn_Save_Click(Object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_APPROVED.Id || vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.SETTLED.Id)
            {
                if (txt_ReleaseReason.Text.Trim() == string.Empty)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ReleaseInvoice", "alert('Please enter Release Reason.');", true);
                    return;
                }
                else
                    NonTradeManager.Instance.updateNTActionHistory(vwDomainNTInvoiceDef.NTInvoice.InvoiceId, -1, "Invoice released.", this.LogonUserId);
            }

            Save();

            bindData();

            if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_APPROVED.Id || 
                    vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.SETTLED.Id)
                releaseInvoice(false);

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceSave", "alert('Save successful.');", true);
        }

        protected void btn_Submit_Click(Object sender, EventArgs e)
        {
            if (vwInvoiceUploadList == null || vwInvoiceUploadList.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ntinvoice", "alert('Please upload invoice file before submitting the invoice.');", true);
                return;
            }

            if (ddl_Approver.SelectedValue == "-1")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ntinvoice", "alert('Approver is not selected.');", true);
                return;
            }

            if (!Page.IsValid)
                return;

            vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus = NTWFS.PENDING_FOR_APPROVAL;
            vwDomainNTInvoiceDef.NTInvoice.SubmittedBy = CommonUtil.getUserByKey(this.LogonUserId);
            vwDomainNTInvoiceDef.NTInvoice.SubmittedOn = DateTime.Now;
            Save();

            bindData();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceSave", "alert('Invoice submitted.');", true);            
        }

        protected void btn_Approve_Click(Object sender, EventArgs e)
        {                        
            if (!Page.IsValid)
                return;

            vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus = NTWFS.DEPARTMENT_APPROVED;

            if (vwDomainNTInvoiceDef.NTRechargeDetailList != null)
            {
                foreach (NTInvoiceDetailDef detailDef in vwDomainNTInvoiceDef.NTRechargeDetailList)
                {
                    detailDef.RechargeContactPerson = detailDef.ContactPerson;
                }
            }


            Save();

            //bindData();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceSave", "alert('Invoice approved.'); window.close();", true);            

        }

        protected void btn_Reject_Click(Object sender, EventArgs e)
        {
            if (txt_RejectReason.Text.Trim() == string.Empty)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RejectInvoice", "alert('Please enter reject reason.');", true);
                return;
            }
            vwDomainNTInvoiceDef.NTInvoice.RejectReason = txt_RejectReason.Text.Trim();            

            if (!Page.IsValid)
                return;

            vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus = NTWFS.DEPARTMENT_REJECTED;

            Save();
            NonTradeManager.Instance.sendNTInvoiceRejectNotification(vwDomainNTInvoiceDef.NTInvoice.InvoiceId, this.LogonUserId);
            //bindData();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceSave", "alert('Invoice rejected.'); window.close();", true);                        
        }

        protected void btn_AccountReceive_Click(Object sender, EventArgs e)
        {                        
            if (!Page.IsValid)
                return;

            if (vwDomainNTInvoiceDef.NTInvoiceDetailList != null && vwDomainNTInvoiceDef.NTInvoiceDetailList.Count > 0)
            {
                foreach (NTInvoiceDetailDef detail in vwDomainNTInvoiceDef.NTInvoiceDetailList)
                    if (detail.Status == 1 && detail.ExpenseType.IsSegmentValue == 1 && (detail.SegmentValue7 == null || detail.SegmentValue8 == null))
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RecvInvoice", "alert('Please select Segment Value before approving this invoice.');", true);
                        return;
                    }                
            }

            if (vwDomainNTInvoiceDef.NTRechargeDetailList != null && vwDomainNTInvoiceDef.NTRechargeDetailList.Count > 0)
            {
                foreach (NTInvoiceDetailDef rechargeDtl in vwDomainNTInvoiceDef.NTRechargeDetailList)
                    if (rechargeDtl.Status == 1 && rechargeDtl.ExpenseType != null && (rechargeDtl.ExpenseType.IsSegmentValue == 1 && (rechargeDtl.SegmentValue7 == null || rechargeDtl.SegmentValue8 == null)))
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RecvInvoice", "alert('Please select Segment Value before approving this invoice.');", true);
                        return;
                    }
            }

            vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus = NTWFS.ACCOUNTS_RECEIVED;
            vwDomainNTInvoiceDef.NTInvoice.AccountFirstApproverId = this.LogonUserId;
            vwDomainNTInvoiceDef.NTInvoice.AccountFirstApprovedOn = DateTime.Now;
            Save();

            //bindData();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceSave", "alert('Invoice received.'); window.close();", true);            

        }

        protected void btn_AccountReject_Click(Object sender, EventArgs e)
        {
            rejectPopupExtender.Hide();

            string rejectReason = string.Empty;

            if (txt_AccRejectReason.Text.Trim() != string.Empty)
                rejectReason = txt_AccRejectReason.Text.Trim();

            string field = string.Empty;

            foreach (ListItem item in cbl_rejectReason.Items)
            {
                if (item.Selected)
                    field += (field == string.Empty ? item.Text : ", " + item.Text);
            }

            if (field != string.Empty)
                rejectReason += "Please correct the " + field;


            if (rejectReason == string.Empty)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RejectInvoice", "alert('Please enter reject reason.');", true);
                return;
            }

            txt_RejectReason.Text = rejectReason;
            vwDomainNTInvoiceDef.NTInvoice.RejectReason = rejectReason;

            if (!Page.IsValid)
                return;

            vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus = NTWFS.ACCOUNTS_REJECTED;

            Save();

            NonTradeManager.Instance.sendNTInvoiceRejectNotification(vwDomainNTInvoiceDef.NTInvoice.InvoiceId, this.LogonUserId);

            //bindData();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceSave", "alert('Invoice rejected.'); window.close();", true);                            
        }

        protected void btn_AccountApprove_Click(Object sender, EventArgs e)
        {                        

            if (!Page.IsValid)
                return;

            vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus = NTWFS.ACCOUNTS_APPROVED;
            vwDomainNTInvoiceDef.NTInvoice.AccountSecondApproverId = this.LogonUserId;
            vwDomainNTInvoiceDef.NTInvoice.AccountSecondApprovedOn = DateTime.Now;

                if (vwDomainNTInvoiceDef.NTRechargeDetailList != null)
                {
                    foreach (NTInvoiceDetailDef detailDef in vwDomainNTInvoiceDef.NTRechargeDetailList)
                    {
                        detailDef.RechargeContactPerson = detailDef.ContactPerson;
                    }

                    Context.Items.Add(AccountCommander.Param.ntRechargeDetailList, vwDomainNTInvoiceDef.NTRechargeDetailList);
                }


            Save();

            //bindData();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceSave", "alert('Invoice approved.'); window.close();", true);            

        }

        protected void btn_AccountEvaluate_Click(Object sender, EventArgs e)
        {                        
            if (!Page.IsValid)
                return;

            vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus = NTWFS.ACCOUNTS_EVALUATING;

            Save();

            //bindData();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceSave", "alert('Save successful.'); window.close();", true);            


        }

        protected void btn_Release_Click(Object sender, EventArgs e)
        {
            vwDomainNTInvoiceDef = NonTradeManager.Instance.getDomainNTInvoiceDef(vwDomainNTInvoiceDef.NTInvoice.InvoiceId);
            bindData();
            releaseInvoice(true);
        }

        private void releaseInvoice(bool isRelease)
        {
            string cssStyle = "";
            if (isRelease)
                cssStyle = "readOnlyField";

            ddl_Office.Enabled = !isRelease;
            ddl_Office.CssClass = cssStyle;
            ddl_BusinessEntity.Enabled = !isRelease;
            ddl_BusinessEntity.CssClass = cssStyle;
            txt_SupplierName.Enabled = !isRelease;
            txt_InvoiceReceivedDate.ReadOnly = isRelease;
            txt_InvoiceReceivedDate.CssClass = cssStyle;
            ce_InvoiceReceivedDate.Enabled = !isRelease;
            ddl_DocumentType.Enabled = !isRelease;
            ddl_DocumentType.CssClass = cssStyle;
            txt_InvoiceNo.ReadOnly = isRelease;
            txt_InvoiceNo.CssClass = cssStyle;
            txt_CustomerNo.ReadOnly = isRelease;
            txt_CustomerNo.CssClass = cssStyle;
            txt_InvoiceDate.ReadOnly = isRelease;
            txt_InvoiceDate.CssClass = cssStyle;
            ce_InvoiceDate.Enabled = !isRelease;
            txt_DueDate.ReadOnly = isRelease;
            txt_DueDate.CssClass = cssStyle;
            ce_DueDate.Enabled = !isRelease;
            ddl_Currency.Enabled = !isRelease;
            ddl_Currency.CssClass = cssStyle;
            txt_Amount.ReadOnly = isRelease;
            txt_Amount.CssClass = cssStyle;
            txt_TotalVAT.ReadOnly = isRelease;
            txt_TotalVAT.CssClass = cssStyle;
            //ddl_PaymentMethod.Enabled = !isRelease;
            //ddl_PaymentMethod.CssClass = cssStyle;
            ddl_PaymentMonthFrom.Enabled = !isRelease;
            ddl_PaymentMonthFrom.CssClass = cssStyle;
            ddl_PaymentMonthTo.Enabled = !isRelease;
            ddl_PaymentMonthTo.CssClass = cssStyle;
            ddl_PaymentYearFrom.Enabled = !isRelease;
            ddl_PaymentYearFrom.CssClass = cssStyle;
            ddl_PaymentYearTo.Enabled = !isRelease;
            ddl_PaymentYearTo.CssClass = cssStyle;
            txt_RejectReason.ReadOnly = isRelease;
            txt_RejectReason.CssClass = cssStyle;
            txt_Remark.ReadOnly = isRelease;
            txt_Remark.CssClass = cssStyle;
            ddl_Approver.Enabled = !isRelease;
            ddl_Approver.CssClass = cssStyle;
            /*
            if (this.ddl_Approver.Enabled)
            {
                ddl_Approver.Enabled = !isRelease;
                ddl_Approver.CssClass = cssStyle;
            }
            */

            ckb_PayByHK.Enabled = !isRelease;

            if (isRelease || vwDomainNTInvoiceDef.NTInvoice.ReleaseReason != string.Empty)
                row_ReleaseReason.Visible = true;
            else
                row_ReleaseReason.Visible = false;                

            btn_Save.Visible = isRelease;
            btn_Discard.Visible = isRelease;
            btn_Copy.Visible = !isRelease;
            btn_Release.Visible = !isRelease;
        }

        protected void btn_Discard_Click(Object sender, EventArgs e)
        {
            releaseInvoice(false);
            vwDomainNTInvoiceDef = NonTradeManager.Instance.getDomainNTInvoiceDef(vwDomainNTInvoiceDef.NTInvoice.InvoiceId);
            bindData();
        }

        protected void btn_Copy_Click(Object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            NTInvoiceDef invoiceDef = new NTInvoiceDef();

            invoiceDef.Office = vwDomainNTInvoiceDef.NTInvoice.Office;
            invoiceDef.Company = vwDomainNTInvoiceDef.NTInvoice.Company;
            invoiceDef.Dept = vwDomainNTInvoiceDef.NTInvoice.Dept;
            invoiceDef.NTVendor = vwDomainNTInvoiceDef.NTInvoice.NTVendor == null ? null : vwDomainNTInvoiceDef.NTInvoice.NTVendor;
            invoiceDef.DCIndicator = vwDomainNTInvoiceDef.NTInvoice.DCIndicator;            
            invoiceDef.CustomerNo = vwDomainNTInvoiceDef.NTInvoice.CustomerNo;
            invoiceDef.Currency = vwDomainNTInvoiceDef.NTInvoice.Currency == null ? null : vwDomainNTInvoiceDef.NTInvoice.Currency;
            invoiceDef.PaymentMethod = vwDomainNTInvoiceDef.NTInvoice.PaymentMethod == null ? null : vwDomainNTInvoiceDef.NTInvoice.PaymentMethod;            
            invoiceDef.PaymentFromDate = DateTime.Today;
            invoiceDef.PaymentToDate = DateTime.Today;
            invoiceDef.InvoiceReceivedDate = DateTime.Today;
            invoiceDef.WorkflowStatus = NTWFS.DRAFT;

            vwDomainNTInvoiceDef.NTInvoice = invoiceDef;

            if (vwDomainNTInvoiceDef.NTInvoiceDetailList != null && vwDomainNTInvoiceDef.NTInvoiceDetailList.Count > 0)
            {                
                foreach (NTInvoiceDetailDef detail in vwDomainNTInvoiceDef.NTInvoiceDetailList)
                {
                    detail.InvoiceDetailId = 0;
                    detail.InvoiceId = 0;
                    detail.RechargeDCNoteId = 0;
                    detail.Amount = 0;
                    detail.VAT = 0;
                }               
            }

            if (vwDomainNTInvoiceDef.NTRechargeDetailList != null && vwDomainNTInvoiceDef.NTRechargeDetailList.Count > 0)
            {
                foreach (NTInvoiceDetailDef detail in vwDomainNTInvoiceDef.NTRechargeDetailList)
                {
                    detail.InvoiceDetailId = 0;
                    detail.InvoiceId = 0;
                    detail.RechargeDCNoteId = 0;
                    detail.Amount = 0;
                    detail.VAT = 0;
                }

            }

            vwDomainNTInvoiceDef.ActionHistoryList = null;
            vwInvoiceUploadList = null;
            vwDocId = 0;

            if (invoiceDef.Office.OfficeId == OfficeId.PK.Id)
            {
                if (ddl_Approver.Items.FindByText("Ivan Ka Po Chong") != null)
                {
                    ddl_Approver.selectByText("Ivan Ka Po Chong");
                    vwDomainNTInvoiceDef.NTInvoice.Approver = CommonUtil.getUserByKey(int.Parse(ddl_Approver.SelectedItem.Value));
                }
            }

            if (invoiceDef.Currency.CurrencyId == CurrencyId.USD.Id && invoiceDef.Office.OfficeId == OfficeId.SL.Id)
            {
                vwDomainNTInvoiceDef.NTInvoice.IsPayByHK = 1;
            }
            else if ((invoiceDef.Office.OfficeId == OfficeId.CA.Id || invoiceDef.Office.OfficeId == OfficeId.TH.Id || invoiceDef.Office.OfficeId == OfficeId.VN.Id || invoiceDef.Office.OfficeId == OfficeId.PK.Id) 
                     && invoiceDef.Company.Id == CompanyType.NEXT_SOURCING.Id)
            {
                vwDomainNTInvoiceDef.NTInvoice.IsPayByHK = 1;
            }

            if (vwDomainNTInvoiceDef.NTInvoice.Company.Id != CompanyType.NEXT_SOURCING.Id)
                vwDomainNTInvoiceDef.NTInvoice.IsPayByHK = 0;

            bindData();

        }

        protected void btn_New_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/nontrade/NonTradeInvoice.aspx");
        }

        protected void btn_Cancel_Click(Object sender, EventArgs e)
        {
            if (vwDomainNTInvoiceDef.NTInvoice.InvoiceId == 0)
                return;
            updateStatus(NTWFS.CANCELLED);
        }

        protected void btn_UploadInvoice_Click(Object sender, EventArgs e)
        {
            if (vwDomainNTInvoiceDef.NTInvoice == null || string.IsNullOrEmpty(vwDomainNTInvoiceDef.NTInvoice.NSLInvoiceNo))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceFile", "alert('Please save the invoice before uploading invoice file.')", true);
                return;
            }

            if (!fu_InvoiceUpload.HasFile)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceFile", "alert('Please select a file to upload.');", true);
                return;
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(fu_InvoiceUpload.FileName, @"[^a-zA-Z0-9\-\\/()._\s]+"))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceFile", "alert('File name cannot contain special characters, e.g. , & $ % * \\' : ; #');", true);
                return;
            }

            string outputFolder = WebConfig.getValue("appSettings", "DMS_NTEXPENSE_Folder") + "\\";
            string path = outputFolder + fu_InvoiceUpload.FileName;
            fu_InvoiceUpload.SaveAs(path);

            ArrayList attachmentList = new ArrayList();
            attachmentList.Add(path);

            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Non-Trade-Expense"));
            queryStructs.Add(new QueryStructDef("NSL Invoice No", vwDomainNTInvoiceDef.NTInvoice.NSLInvoiceNo));
            ArrayList qList = DMSUtil.queryDocument(queryStructs);

            long docId = 0;
            if (qList.Count > 0)
                docId = ((DocumentInfoDef)qList[0]).DocumentID;


            ArrayList qStruct = new ArrayList();
            qStruct.Add(new QueryStructDef("DOCUMENTTYPE", "Non-Trade-Expense"));
            qStruct.Add(new QueryStructDef("NSL Invoice No", vwDomainNTInvoiceDef.NTInvoice.NSLInvoiceNo));
            qStruct.Add(new QueryStructDef("Invoice No", vwDomainNTInvoiceDef.NTInvoice.InvoiceNo));
            qStruct.Add(new QueryStructDef("Customer No", vwDomainNTInvoiceDef.NTInvoice.CustomerNo));
            qStruct.Add(new QueryStructDef("Document Type", (vwDomainNTInvoiceDef.NTInvoice.DCIndicator == "D" ? "Non-Trade-Invoice" : "Non-Trade-Refund")));


            if (docId > 0)
            {
                string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, qStruct, attachmentList);
                NonTradeManager.Instance.updateNTActionHistory(vwDomainNTInvoiceDef.NTInvoice.InvoiceId, -1, "Upload invoice file: " + fu_InvoiceUpload.FileName, this.LogonUserId);
                getInvoiceFile();
            }
            else
            {
                string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Account\\Non-Trade Expense\\", vwDomainNTInvoiceDef.NTInvoice.NSLInvoiceNo, "Non-Trade-Expense", qStruct, attachmentList);

                if (msg != string.Empty)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DMSUpload", "alert('Failed to upload document. " + msg + " ');", true);
                    return;
                }
                else
                {
                    NonTradeManager.Instance.updateNTActionHistory(vwDomainNTInvoiceDef.NTInvoice.InvoiceId, -1, "Upload invoice file: " + fu_InvoiceUpload.FileName, this.LogonUserId);                    
                    getInvoiceFile();
                }
            }
            
        }

        protected void getInvoiceFile()
        {
            if (vwDomainNTInvoiceDef.NTInvoice == null || vwDomainNTInvoiceDef.NTInvoice.NSLInvoiceNo == string.Empty)
                return;

            //ArrayList attachmentList = new ArrayList();            

            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Non-Trade-Expense"));
            queryStructs.Add(new QueryStructDef("NSL Invoice No", vwDomainNTInvoiceDef.NTInvoice.NSLInvoiceNo));
            ArrayList qList = DMSUtil.queryDocument(queryStructs);

            if (qList.Count > 0)
            {
                DocumentInfoDef doc = (DocumentInfoDef)qList[0];
                
                vwDocId = doc.DocumentID;
                vwInvoiceUploadList = doc.AttachmentInfos;
                gv_InvoiceUpload.DataSource = vwInvoiceUploadList;
                gv_InvoiceUpload.DataBind();
            }
            else
            {
                gv_InvoiceUpload.DataSource = null;
                gv_InvoiceUpload.DataBind();
            }
             
        }

        protected void lnk_FileLink_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            
            AttachmentInfoDef def = (AttachmentInfoDef) this.vwInvoiceUploadList[int.Parse(btn.CommandArgument)];

            Context.Items.Clear();
            string[] s = def.Description.Split('|');
            Context.Items.Add("docId", vwDocId);
            Context.Items.Add("attId", def.AttachmentID.ToString());
            Context.Items.Add("majorId", def.MajorVerion.ToString());
            Context.Items.Add("minorId", def.MinorVerion.ToString());
            Context.Items.Add("buildId", def.Build.ToString());
            forwardToScreen("dms.Attachment");

        }

        protected void updateStatus(NTWFS workflowStatus)
        {
            if (!updateInvoiceDetail(-1, false))
                return;
            if (!updateRechargeDetail(-1, false))
                return;

            vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus = workflowStatus;                       

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateNTInvoice);

            Context.Items.Add(AccountCommander.Param.ntInvoice, vwDomainNTInvoiceDef.NTInvoice);

            forwardToScreen(null);

            bindData();

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvoiceSave", "alert('Save successful.');", true);

        }

        protected void val_Vendor_Validate(object sender, ServerValidateEventArgs e)
        {
            if (txt_SupplierName.NTVendorId == int.MinValue)
            {
                e.IsValid = false;
                val_Vendor.ErrorMessage = "Please select Vendor.";
            }
        }

        protected void val_InvoiceDate_Validate(object sender, ServerValidateEventArgs e)
        {
            if (txt_InvoiceDate.Text.Trim() == string.Empty)
            {
                e.IsValid = false;
                val_InvoiceDate.ErrorMessage = "Please enter Invoice Date.";
                return;
            }

            if (txt_InvoiceReceivedDate.Text.Trim() == string.Empty)
            {
                e.IsValid = false;
                val_InvoiceDate.ErrorMessage = "Please enter Invoice Received Date.";
                return;
            }

            DateTime invoiceDate = DateTime.MinValue;

            if (!DateTime.TryParse(txt_InvoiceReceivedDate.Text, out invoiceDate))
            {
                e.IsValid = false;
                val_InvoiceDate.ErrorMessage = "Invalid Invoice Received Date.";
                return;
            }

            if (!DateTime.TryParse(txt_InvoiceDate.Text, out invoiceDate))
            {
                e.IsValid = false;
                val_InvoiceDate.ErrorMessage = "Invalid Invoice Date.";
                return;
            }

            if (invoiceDate > DateTime.Today)
            {
                e.IsValid = false;
                val_InvoiceDate.ErrorMessage = "Invalid Invoice Date.";
                return;
            }

            if (txt_DueDate.Text.Trim() == string.Empty)
            {
                e.IsValid = false;
                val_InvoiceDate.ErrorMessage = "Due Date is required.";
            }

            if (txt_DueDate.Text.Trim() != string.Empty)
            {
                DateTime dueDate = DateTime.MinValue;
                if (!DateTime.TryParse(txt_DueDate.Text, out dueDate))
                {
                    e.IsValid = false;
                    val_InvoiceDate.ErrorMessage = "Invalid Due Date.";
                    return;
                }
                if (invoiceDate > dueDate)
                {
                    e.IsValid = false;
                    val_InvoiceDate.ErrorMessage = "Invoice Date cannot be larger than Due Date.";
                }
            }
        }

        protected void val_Approver_Validate(object sender, ServerValidateEventArgs e)
        {
            if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id != NTWFS.DRAFT.Id && ddl_Approver.SelectedValue == "-1" && vwDomainNTInvoiceDef.NTInvoice.Approver == null)
            {
                e.IsValid = false;
                val_Approver.ErrorMessage = "Please select Approver.";
            }

            if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DRAFT.Id && ddl_Approver.selectedValueToInt == (vwDomainNTInvoiceDef.NTInvoice.CreatedBy == null ? this.LogonUserId : vwDomainNTInvoiceDef.NTInvoice.CreatedBy.UserId))
            {
                e.IsValid = false;
                val_Approver.ErrorMessage = "Cannot select the person creating the invoice as the approver, please choose another person.";
            }
        }

        

        protected void val_InvoiceNo_Validate(object sender, ServerValidateEventArgs e)
        {
            val_InvoiceNo.ErrorMessage = string.Empty;

            if (txt_InvoiceNo.Text.Trim() == string.Empty && txt_CustomerNo.Text.Trim() == string.Empty)
            {
                e.IsValid = false;
                val_InvoiceNo.ErrorMessage = "Please enter Invoice No. or Customer No.";
            }
            else if (vwDomainNTInvoiceDef.NTInvoice.NTVendor != null)
            {
                if (txt_InvoiceNo.Text.Trim() == string.Empty && vwDomainNTInvoiceDef.NTInvoice.NTVendor.IsInvoiceNoRequired == 1)
                {
                    e.IsValid = false;
                    val_InvoiceNo.ErrorMessage = "Please enter Invoice No.";
                }
                else if (txt_CustomerNo.Text.Trim() == string.Empty && vwDomainNTInvoiceDef.NTInvoice.NTVendor.IsCustomerNoRequired == 1)
                {
                    e.IsValid = false;
                    val_InvoiceNo.ErrorMessage = "Please enter Customer No.";
                }
                else if (NonTradeManager.Instance.isNTInvoiceDuplicated(vwDomainNTInvoiceDef.NTInvoice.InvoiceId, txt_InvoiceNo.Text.Trim(), txt_CustomerNo.Text.Trim(),
                    ConvertUtility.isDateTime(txt_InvoiceDate.Text) ? ConvertUtility.toDateTime(txt_InvoiceDate.Text) : DateTime.MinValue,
                    vwDomainNTInvoiceDef.NTInvoice.NTVendor == null ? string.Empty : vwDomainNTInvoiceDef.NTInvoice.NTVendor.EPVendorCode))

                {
                    e.IsValid = false;
                    val_InvoiceNo.ErrorMessage = "This invoice already exists.";
                }
            }
                                    
            DateTime paymentFrom = new DateTime(int.Parse(ddl_PaymentYearFrom.SelectedValue), int.Parse(ddl_PaymentMonthFrom.SelectedValue), 1);
            DateTime paymentTo = new DateTime(int.Parse(ddl_PaymentYearTo.SelectedValue), int.Parse(ddl_PaymentMonthTo.SelectedValue), 1);
            if (paymentFrom > paymentTo)
            {
                e.IsValid = false;
                val_InvoiceNo.ErrorMessage += "Invalid payment period. ";
            }

            OfficeRef office = CommonUtil.getOfficeRefByKey(ddl_Office.selectedValueToInt);

            if (ddl_PaymentMethod.selectedValueToInt == NTPaymentMethodRef.CASH.Id)
            {
                if (ddl_Currency.selectedValueToInt != office.Location.CurrencyId)
                {
                    e.IsValid = false;
                    val_InvoiceNo.ErrorMessage = "Local currency should be used if payment method is Cash.";
                }
            }

            /*01/02/2018 enable HK office to input RMB non-trade invoice
            if (ddl_Currency.selectedValueToInt == CurrencyId.RMB.Id && office.OfficeId == OfficeId.HK.Id)
            {
                    e.IsValid = false;
                    val_InvoiceNo.ErrorMessage = "Currency CNY is not allowed for HK Office";
            }*/

        }

        protected void val_InvoiceDetail_Validate(object sender, ServerValidateEventArgs e)
        {
            if (!updateInvoiceDetail(-1, false))
            {
                e.IsValid = false;
                return;
            }
            if (!updateRechargeDetail(-1, false))
            {
                e.IsValid = false;
                return;
            }

            if (txt_Amount.Text.Trim() == string.Empty)
            {
                e.IsValid = false;
                val_InvoiceDetail.ErrorMessage = "Please enter Total Invoice Amount.";
            }
            else
            {
                //find out which button is pressed
                Control control = null;
                string ctrlname = Page.Request.Params["__EVENTTARGET"];
                if (ctrlname != null && ctrlname != String.Empty)
                {
                    control = Page.FindControl(ctrlname);
                }
                else
                {
                    string ctrlStr = String.Empty;
                    Control c = null;
                    foreach (string ctl in Page.Request.Form)
                    {
                        if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                        {
                            ctrlStr = ctl.Substring(0, ctl.Length - 2);
                            c = Page.FindControl(ctrlStr);
                        }
                        else
                        {
                            c = Page.FindControl(ctl);
                        }
                        if (c is System.Web.UI.WebControls.Button ||
                                    c is System.Web.UI.WebControls.ImageButton)
                        {
                            control = c;
                            break;
                        }
                    }
                }

                decimal amount = 0;
                if (!decimal.TryParse(txt_Amount.Text, out amount) || amount <= 0)
                {
                    e.IsValid = false;
                    val_InvoiceDetail.ErrorMessage = "Invalid Total Invoice Amount.";
                }
                else if (amount != (totalAmountForCostCenter + totalAmountForRecharge) && !(vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DRAFT.Id && control.ID == "btn_Save"))
                {
                    e.IsValid = false;
                    val_InvoiceDetail.ErrorMessage = "Invalid Total Invoice Amount. It must be equal to the sum of invoice detail amount. The difference of the amount is " + (amount - totalAmountForCostCenter - totalAmountForRecharge).ToString();
                }

                amount = 0;

                if ((txt_TotalVAT.Text.Trim() != string.Empty &&  !decimal.TryParse(txt_TotalVAT.Text, out amount)) || amount < 0)
                {
                    e.IsValid = false;
                    val_InvoiceDetail.ErrorMessage = "Invalid Total VAT Amount.";
                }
                else if (amount != (totalVATForCostCenter + totalVATForRecharge) && !(vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DRAFT.Id && control.ID == "btn_Save"))
                {
                    e.IsValid = false;
                    val_InvoiceDetail.ErrorMessage = "Invalid Total VAT Amount. It must be equal to the sum of invoice detail VAT amount. The difference of the amount is " + (amount - totalVATForCostCenter - totalVATForRecharge).ToString();
                }

                string error = string.Empty;
                if ((error = errorInCarbonEmissionDetailRows(rep_InvoiceDetail)) != string.Empty)
                {
                    e.IsValid = false;
                    val_InvoiceDetail.ErrorMessage = error;
                }
                else if ((error = errorInCarbonEmissionDetailRows(rep_RechargeDetail)) != string.Empty)
                {
                    e.IsValid = false;
                    val_InvoiceDetail.ErrorMessage = error;
                }

            }            

        }

        protected string errorInCarbonEmissionDetailRows(Repeater repeater)
        {
            string error = string.Empty;
            string section = string.Empty;
            List<string> invalidItems = new List<string>();
            decimal val;
            Control rowConsumption;
            SmartDropDownList ddlExpenseType;
            SmartDropDownList ddlUnit;
            TextBox txtQty;
            TextBox txtUnitCost;

            foreach (RepeaterItem item in repeater.Items)
                if (item.Visible)
                {
                    NTExpenseTypeRef expenseType;
                    rowConsumption = null;
                    if ((rowConsumption = item.FindControl("row_CostCenter_Consumption")) != null)
                        section = "Cost Centre";
                    else
                        if ((rowConsumption = item.FindControl("row_Recharge_Consumption")) != null)
                            section = "Recharge";

                    if (rowConsumption != null && rowConsumption.Visible)
                    {
                        expenseType = null;
                        ddlExpenseType = (SmartDropDownList)item.FindControl("ddl_ExpenseType");
                        if (ddlExpenseType != null && ddlExpenseType.Items.Count > 0)
                            expenseType = NonTradeManager.Instance.getNTExpenseTypeByKey(ddlExpenseType.selectedValueToInt);
                        if (rowConsumption != null && expenseType != null && carbonEmissionExpenseTypeAccountCode.Contains(expenseType.SUNAccountCode))
                        {
                            ddlUnit = (SmartDropDownList)rowConsumption.FindControl("ddl_ConsumptionUnit");
                            txtQty = ((TextBox)rowConsumption.FindControl("txtConsumptionQty"));
                            txtUnitCost = ((TextBox)rowConsumption.FindControl("txtConsumptionUnitCost"));

                            if (ddlUnit.SelectedValue == "-1")
                                invalidItems.Add("Consumption Unit");

                            if (utilityExpenseTypeAccountCode.Contains(expenseType.SUNAccountCode))
                            {
                                val = -1;
                                decimal.TryParse(txtQty.Text, out val);
                                if (val <= 0)
                                    invalidItems.Add("No. of Units Consumed");

                                val = -1;
                                decimal.TryParse(txtUnitCost.Text, out val);
                                if (val <= 0)
                                    invalidItems.Add("Consumption Unit Cost");
                            }
                            else
                            {
                                val = -1;
                                decimal.TryParse(txtQty.Text, out val);
                                if (expenseType.SUNAccountCode != "4104310" && val <= 0) // not car rental
                                    invalidItems.Add("Distance");
                            }
                        }
                    }
                    if (invalidItems.Count > 0)
                        break;
                }

            // return error message
            if (invalidItems.Count > 0)
                if (invalidItems.Count == 1)
                    error = "Invalid " + invalidItems[0] + " in " + section + " Consumption Detail ";
                else
                {
                    error = "Invalid input of the following items in the " + section + " Consumption Detail : <br>";
                    foreach (string invalidItem in invalidItems)
                    {
                        if (invalidItems.Count > 1)
                            error += "&nbsp;&nbsp;-&nbsp;";
                        error += invalidItem + "<br>";
                    }
                }
            return error;
        }

        protected void OfficeSelectedIndexChange(object sender, EventArgs e)
        {
            OfficeRef office = CommonUtil.getOfficeRefByKey(int.Parse(ddl_Office.SelectedValue));
            vwDomainNTInvoiceDef.NTInvoice.Office = office;
            vwDomainNTInvoiceDef.NTInvoice.NTVendor = null;
            txt_SupplierName.clear();

            //ddl_Department.bindList(CommonUtil.getDepartmentList(office.OfficeId), "Description", "DepartmentId", 
            //    (vwDomainNTInvoiceDef.NTInvoice.Dept == null ? CommonUtil.getUserByKey(this.LogonUserId).Department.DepartmentId.ToString() : vwDomainNTInvoiceDef.NTInvoice.Dept.DepartmentId.ToString()));
            
            vwNTVendorExpenseTypeList = WebUtil.getNTExpenseTypeListByOfficeId(office.OfficeId);

            ArrayList arr_BusinessEntity = CommonUtil.getCompanyList(office.OfficeId);
            if (arr_BusinessEntity.Count > 0)
            {
                ddl_BusinessEntity.bindList(arr_BusinessEntity, "CompanyName", "CompanyId", "");
                if (ddl_BusinessEntity.selectedValueToInt == CompanyType.NSL_TK.Id)
                    ddl_Currency.selectByText("TRY");
            }

            ArrayList approverList = NonTradeManager.Instance.getNTApproverListByOfficeId(ConvertUtility.createArrayList(office));

            approverList.Sort(new ArrayListHelper.Sorter("ApproverName"));
            ddl_Approver.bindList(approverList, "ApproverName", "ApproverId", vwDomainNTInvoiceDef.NTInvoice.Approver == null ? "" : vwDomainNTInvoiceDef.NTInvoice.Approver.UserId.ToString(), "-- Please select --", "-1");

            if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id != NTWFS.DRAFT.Id && vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id != NTWFS.PENDING_FOR_APPROVAL.Id &&
                vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id != NTWFS.DEPARTMENT_REJECTED.Id && vwDomainNTInvoiceDef.NTInvoice.Approver != null && ddl_Approver.SelectedValue == "-1")
            {
                ddl_Approver.Items.Insert(0, new ListItem(vwDomainNTInvoiceDef.NTInvoice.Approver.DisplayName, vwDomainNTInvoiceDef.NTInvoice.Approver.UserId.ToString()));
                ddl_Approver.SelectedIndex = 0;
                ddl_Approver.Enabled = false;
                ddl_Approver.CssClass = "readOnlyField";
            }

            ddl_PaymentMethod.bindList(NTPaymentMethodRef.getCollectionValues(office.OfficeId), "Name", "Id");

            if (vwDomainNTInvoiceDef.NTInvoiceDetailList != null)
            {
                foreach (NTInvoiceDetailDef def in vwDomainNTInvoiceDef.NTInvoiceDetailList)
                {
                    def.ExpenseType = null;
                    def.Company = CompanyType.getType(int.Parse(ddl_BusinessEntity.SelectedValue));
                }
            }
            if (vwDomainNTInvoiceDef.NTRechargeDetailList != null)
            {
                NTInvoiceDetailDef rdef = null;
                for (int i = 0; i < vwDomainNTInvoiceDef.NTRechargeDetailList.Count; i++)
                {
                    rdef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[i];
                    rdef.ExpenseType = null;
                    if (rdef.InvoiceDetailType != null && rdef.InvoiceDetailType.Id == NTInvoiceDetailType.OFFICE.Id && rdef.Office != null &&
                        rdef.Office.OfficeId == office.OfficeId && rdef.Company.Id == ddl_BusinessEntity.selectedValueToInt)
                    {
                        rdef.Office = null;                        
                    }

                }                
            }

            row_PayByHK.Visible = (office.OfficeId != OfficeId.HK.Id);
            ckb_PayByHK.Checked = false;
            payByHK();
            this.setDefaultApprover();

            if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DRAFT.Id && office.OfficeId == OfficeId.PK.Id)
            {
                if (ddl_Approver.Items.FindByText("Ivan Ka Po Chong") != null)
                {
                    ddl_Approver.selectByText("Ivan Ka Po Chong");
                    vwDomainNTInvoiceDef.NTInvoice.Approver = CommonUtil.getUserByKey(int.Parse(ddl_Approver.SelectedItem.Value));
                }
            }
            
            if (office.OfficeId == OfficeId.SL.Id || office.OfficeId == OfficeId.IND.Id || office.OfficeId == OfficeId.ND.Id ||
                office.OfficeId == OfficeId.TR.Id || office.OfficeId == OfficeId.BD.Id || office.OfficeId == OfficeId.SH.Id || office.OfficeId == OfficeId.CA.Id)
            {
                div_VAT.Visible = true;
            }
            else
            {
                div_VAT.Visible = false;
            }

            reloadInvoiceDetailGrid(sender, e);
            RefreshRechargeGrid(sender, e);
            buttonControl();
            refreshProcurementControl();
        }

        private void refreshProcurementControl()
        {
            APDS.APDSService svc = new APDS.APDSService();
            bool isAssetController = svc.isAuthenticated(this.LogonUserId, 1, this.ddl_Office.selectedValueToInt);
            if (!isAssetController)
            {
                this.txtProcurementRequestNo.Enabled = false;
                if (vwDomainNTInvoiceDef.NTInvoice.ProcurementRequestId == -1)
                    this.btnProcurement.Enabled = false;
            }
            else
            {
                this.txtProcurementRequestNo.Enabled = true;
                this.btnProcurement.Enabled = true;
            }
        }

        protected void BusinessEntitySelectedIndexChange(object sender, EventArgs e)
        {
            CompanyType compType = CompanyType.getType(int.Parse(ddl_BusinessEntity.SelectedValue));

            if (vwDomainNTInvoiceDef.NTInvoiceDetailList != null && vwDomainNTInvoiceDef.NTInvoiceDetailList.Count > 0)
            {                
                foreach (NTInvoiceDetailDef def in vwDomainNTInvoiceDef.NTInvoiceDetailList)
                {
                    def.Company = compType;
                }
            }

            if (vwDomainNTInvoiceDef.NTRechargeDetailList != null && vwDomainNTInvoiceDef.NTRechargeDetailList.Count > 0)
            {
                NTInvoiceDetailDef def = null;
                for (int i = 0; i < vwDomainNTInvoiceDef.NTRechargeDetailList.Count; i++)
                {
                    def = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[i];
                    if (def.InvoiceDetailType != null)
                    {
                        if (def.InvoiceDetailType.Id != NTInvoiceDetailType.OFFICE.Id && def.ExpenseType != null && def.ExpenseType.IsOfficeCode == 1)
                        {
                            def.Company = compType;
                        }
                        else if (def.InvoiceDetailType.Id == NTInvoiceDetailType.OFFICE.Id && def.Office != null && def.Office.OfficeId == ddl_Office.selectedValueToInt && def.Company.Id == compType.Id)
                        {
                            ArrayList arr_ComList = CommonUtil.getCompanyList(def.Office.OfficeId);
                            foreach (CompanyRef comRef in arr_ComList)
                            {
                                if (comRef.CompanyId != def.Company.Id)
                                {
                                    def.Company = CompanyType.getType(comRef.CompanyId);
                                    break;
                                }
                            }
                            reloadRechargeItem(i, rep_RechargeDetail.Items[i]);
                        }
                    }
                }
            }
            this.payByHK();
        }

        protected void ddl_PaymentMethod_SelectedIndexChange(object sender, EventArgs e)
        {
            if (ddl_PaymentMethod.selectedValueToInt == NTPaymentMethodRef.CASH.Id)
            {
                row_PayByHK.Visible = false;
                ckb_PayByHK.Checked = false;
                OfficeRef office = CommonUtil.getOfficeRefByKey(ddl_Office.selectedValueToInt);
                if (ddl_Currency.Items.FindByValue(office.Location.CurrencyId.ToString()) != null)
                {
                    ddl_Currency.SelectedIndex = -1;
                    ((ListItem)ddl_Currency.Items.FindByValue(office.Location.CurrencyId.ToString())).Selected = true;
                }
            }
            else if (ddl_Office.selectedValueToInt != OfficeId.HK.Id)
                row_PayByHK.Visible = true;
        }


        protected void ddl_ExpenseType_SelectedIndexChange(object sender, EventArgs e)
        {
            SmartDropDownList ddl_ExpenseType = (SmartDropDownList)sender;

            int index = ((RepeaterItem)ddl_ExpenseType.NamingContainer).ItemIndex;
            NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef) vwDomainNTInvoiceDef.NTInvoiceDetailList[index];

            NTExpenseTypeRef expenseType = WebUtil.getNTExpenseTypeByKey(int.Parse(ddl_ExpenseType.SelectedValue));

            if (expenseType.SUNAccountCode == "1311303" || expenseType.SUNAccountCode == "1311307" || expenseType.SUNAccountCode == "1311308")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTInvoice", "alert('This expense type is for recharge only. Please select another expense type.');", true);
                if (detailDef.ExpenseType == null)
                    ddl_ExpenseType.SelectedIndex = -1;
                else
                    ddl_ExpenseType.selectByValue(detailDef.ExpenseType.ExpenseTypeId.ToString());
                return;
            }

            detailDef.ExpenseType = expenseType;


            if (detailDef.ExpenseType != null)
            {
                if (detailDef.ExpenseType.IsDepartmentCode == 1)
                {
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter")).Visible = true;
                    if (detailDef.CostCenter != null)
                    {
                        detailDef.Department = CommonUtil.getDepartmentByKey(detailDef.CostCenter.DepartmentId);
                        if (detailDef.ExpenseType.IsOfficeCode == 1)
                            detailDef.Office = detailDef.CostCenter.Office;
                        else
                            detailDef.Office = null;
                    }
                }
                else
                {
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter")).Visible = false;
                    detailDef.Department = null;
                    if (detailDef.ExpenseType.IsOfficeCode == 1)
                    {
                        detailDef.Office = CommonUtil.getOfficeRefByKey(ddl_Office.selectedValueToInt);
                        detailDef.Company = CompanyType.getType(ddl_BusinessEntity.selectedValueToInt);
                    }
                    else
                    {
                        detailDef.Office = null;
                        detailDef.Company = null;
                    }
                }

                if (detailDef.ExpenseType.IsProductCode == 1 && detailDef.CostCenter != null)
                //if (detailDef.ExpenseType.IsProductCode == 1 && detailDef.ExpenseType.SUNAccountCode != "1452028" && detailDef.CostCenter != null)
                {
                    ((HtmlTableRow)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_ProductTeam")).Visible = true;
                    ((HtmlTableRow)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_ProductTeam")).Attributes.Add("onmouseover", "ctl00_ContentPlaceHolder1_ddl_Office.value = " + detailDef.Office.OfficeId.ToString());

                    com.next.isam.webapp.webservices.UclSmartSelection txt_ProductTeam = (com.next.isam.webapp.webservices.UclSmartSelection)ddl_ExpenseType.NamingContainer.FindControl("ddl_ProductTeam");
                    txt_ProductTeam.initControl(webservices.UclSmartSelection.SelectionList.productCode);
                    txt_ProductTeam.setWidth(300);

                    if (detailDef.ProductTeam != null)
                        txt_ProductTeam.ProductCodeId = detailDef.ProductTeam.ProductCodeId;
                }
                else
                    ((HtmlTableRow)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_ProductTeam")).Visible = false;

                if (detailDef.ExpenseType.IsSeasonCode == 1 && detailDef.ExpenseType.SUNAccountCode != "1452028")
                {
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_Season")).Visible = true;
                    ((SmartDropDownList)ddl_ExpenseType.NamingContainer.FindControl("ddl_Season")).bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", detailDef.Season == null ? "-1" : detailDef.Season.SeasonId.ToString(), "-- Please select --", "-1");
                }
                else
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_Season")).Visible = false;
                if (detailDef.ExpenseType.IsStaffCode == 1)
                {
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_User")).Visible = true;
                    com.next.isam.webapp.webservices.UclSmartSelection txt_User = (com.next.isam.webapp.webservices.UclSmartSelection)ddl_ExpenseType.NamingContainer.FindControl("txt_User");
                    txt_User.initControl(webservices.UclSmartSelection.SelectionList.user);
                    txt_User.setWidth(300);
                    if (detailDef.User != null)
                        txt_User.UserId = detailDef.User.UserId;
                }
                else
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_User")).Visible = false;
                if (detailDef.ExpenseType.IsItemNo == 1)
                {
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_ItemNo")).Visible = true;
                    ((TextBox)ddl_ExpenseType.NamingContainer.FindControl("txt_ItemNo")).Text = detailDef.ItemNo;
                }
                else
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_ItemNo")).Visible = false;
                if (detailDef.ExpenseType.IsDevSampleCostType == 1)
                {
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_DevSampleType")).Visible = true;
                    ((SmartDropDownList)ddl_ExpenseType.NamingContainer.FindControl("ddl_DevSampleType")).bindList(NTDevSampleCostType.getNTDevSampleCostTypeList(), "Description", "Id", detailDef.DevSampleCostTypeId == 0 ? "-1" : detailDef.DevSampleCostTypeId.ToString(), "-- Please select --", "-1");
                }
                else
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_DevSampleType")).Visible = false;

                if (detailDef.ExpenseType.IsQtyRequired == 1)
                {
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_Quantity")).Visible = true;
                    ((TextBox)ddl_ExpenseType.NamingContainer.FindControl("txt_Quantity")).Text = detailDef.Quantity.ToString();
                }
                else
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_Quantity")).Visible = false;

                if (detailDef.ExpenseType.SUNAccountCode == "1412101")
                {
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_ExpenseNature")).Visible = true;
                    ((SmartDropDownList)ddl_ExpenseType.NamingContainer.FindControl("ddl_Nature")).bindList(NonTradeManager.Instance.getNTExpenseNatureList(txt_SupplierName.NTVendorId), "Description", "NatureId", detailDef.NatureIdForAccrual == 0 ? "-1" : detailDef.NatureIdForAccrual.ToString(), "-- Please select --", "-1");
                }
                else
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_ExpenseNature")).Visible = false;

                /* TODO:TRADINGAF */
                ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_TradingAF")).Visible = (detailDef.ExpenseType.SUNAccountCode == "1452028");

                ((Label)ddl_ExpenseType.NamingContainer.FindControl("lbl_ItemDescHint")).Text = detailDef.ExpenseType.ItemDescriptionHints;


                if (detailDef.ExpenseType.IsSegmentValue == 1)
                {
                    if ((NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(int.Parse(ddl_Office.SelectedValue))
                        && (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DEPARTMENT_APPROVED.Id || vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_EVALUATING.Id)) ||
                        (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, GeneralCriteria.ALL).Contains(int.Parse(ddl_Office.SelectedValue))
                        && vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_APPROVED.Id)                        
                        )
                    {
                        ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_SegmentField7")).Visible = true;
                        ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_SegmentField8")).Visible = true;
                    }
                    else if (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(int.Parse(ddl_Office.SelectedValue)) ||
                            NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, GeneralCriteria.ALL).Contains(int.Parse(ddl_Office.SelectedValue)) ||
                            NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SUN_INTERFACE.Id, GeneralCriteria.ALL).Contains(int.Parse(ddl_Office.SelectedValue)) ||
                            CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelARApprove) ||
                            CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.SecondLevelARApprove))
                    {
                        ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_SegmentField7")).Visible = true;
                        ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_SegmentField8")).Visible = true;
                        ((SmartDropDownList)ddl_ExpenseType.NamingContainer.FindControl("ddl_SegmentValue7")).Enabled = true;
                        ((SmartDropDownList)ddl_ExpenseType.NamingContainer.FindControl("ddl_SegmentValue8")).Enabled = true;
                    }
                }
                else
                {
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_SegmentField7")).Visible = false;
                    ((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_SegmentField8")).Visible = false;
                }
                ((SmartDropDownList)ddl_ExpenseType.NamingContainer.FindControl("ddl_SegmentValue7")).bindList(NonTradeManager.Instance.getNTEpicorSegmentValueListBySegmentField(7), "Description", "SegmentValueId", detailDef.SegmentValue7 == null ? "-1" : detailDef.SegmentValue7.SegmentValueId.ToString(), "-- Please select --", "-1");
                ((SmartDropDownList)ddl_ExpenseType.NamingContainer.FindControl("ddl_SegmentValue8")).bindList(NonTradeManager.Instance.getNTEpicorSegmentValueListBySegmentField(8), "Description", "SegmentValueId", detailDef.SegmentValue8 == null ? "-1" : detailDef.SegmentValue8.SegmentValueId.ToString(), "-- Please select --", "-1");

                if (detailDef.ExpenseType.SUNAccountCode == "1452028")
                {
                    if (detailDef.SegmentValue7 == null)
                        ((SmartDropDownList)ddl_ExpenseType.NamingContainer.FindControl("ddl_SegmentValue7")).selectByValue("1"); // U
                    if (detailDef.SegmentValue8 == null)
                        ((SmartDropDownList)ddl_ExpenseType.NamingContainer.FindControl("ddl_SegmentValue8")).selectByValue("10"); // G
                }

                refreshCarbonConsumptionRow(((Control)ddl_ExpenseType.NamingContainer.FindControl("row_CostCenter_Consumption")), detailDef, true);
            }
        }

        protected void ddl_RechargeBusinessEntity_SelectedIndexChange(object sender, EventArgs e)
        {
            SmartDropDownList ddl_Recharge_BusinessEntity = (SmartDropDownList)sender;

            int index = ((RepeaterItem)ddl_Recharge_BusinessEntity.NamingContainer).ItemIndex;
            NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[index];

            detailDef.Company = CompanyType.getType(int.Parse(ddl_Recharge_BusinessEntity.SelectedValue));                        

        }

        protected void ddl_RechargeType_SelectedIndexChange(object sender, EventArgs e)
        {
            SmartDropDownList ddl_RechargeType = (SmartDropDownList)sender;

            int index = ((RepeaterItem)ddl_RechargeType.NamingContainer).ItemIndex;
            //NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[index];

            reloadRechargeItem(index, (RepeaterItem)ddl_RechargeType.NamingContainer);
        }

        protected void ddl_Recharge_ExpenseType_SelectedIndexChange(object sender, EventArgs e)
        {
            SmartDropDownList ddl_ExpenseType = (SmartDropDownList)sender;

            int index = ((RepeaterItem)ddl_ExpenseType.NamingContainer).ItemIndex;
            //NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[index];

            //detailDef.ExpenseType = WebUtil.getNTExpenseTypeByKey(int.Parse(ddl_ExpenseType.SelectedValue));

            //if (detailDef.InvoiceDetailType != null && detailDef.InvoiceDetailType.Id != NTInvoiceDetailType.OFFICE.Id && detailDef.ExpenseType.IsOfficeCode == 1)
            //{
            //    detailDef.Office = CommonUtil.getOfficeRefByKey(ddl_Office.selectedValueToInt);
            //    detailDef.Company = CompanyType.getType(ddl_BusinessEntity.selectedValueToInt);
            //}

            //RefreshRechargeGrid(sender, e);
            reloadRechargeItem(index, (RepeaterItem)ddl_ExpenseType.NamingContainer);

            NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[index];
            refreshCarbonConsumptionRow(((Control)ddl_ExpenseType.NamingContainer.FindControl("row_Recharge_Consumption")), detailDef, true);
        }

        protected void ddl_CostCenter_SelectedIndexChange(object sender, EventArgs e)
        {
            SmartDropDownList ddl_CostCenter = (SmartDropDownList)sender;

            int index = ((RepeaterItem)ddl_CostCenter.NamingContainer).ItemIndex;
            NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTInvoiceDetailList[index];

            detailDef.CostCenter = CommonUtil.getCostCenterByKey(int.Parse(ddl_CostCenter.SelectedValue));
            if (ddl_BusinessEntity.Items.Count > 0)
                detailDef.Company = CompanyType.getType(int.Parse(ddl_BusinessEntity.SelectedValue));

            if (detailDef.ExpenseType != null)
            {
                if (detailDef.ExpenseType.IsDepartmentCode == 1)
                {
                    if (detailDef.CostCenter != null)
                    {
                        if (detailDef.ExpenseType.IsOfficeCode == 1)
                            detailDef.Office = detailDef.CostCenter.Office;

                        detailDef.Department = CommonUtil.getDepartmentByKey(detailDef.CostCenter.DepartmentId);
                    }

                    //if (detailDef.ExpenseType.IsProductCode == 1 && detailDef.ExpenseType.SUNAccountCode != "1452028")
                    if (detailDef.ExpenseType.IsProductCode == 1)
                    {
                        ((HtmlTableRow)ddl_CostCenter.NamingContainer.FindControl("row_CostCenter_ProductTeam")).Visible = true;
                        ((HtmlTableRow)ddl_CostCenter.NamingContainer.FindControl("row_CostCenter_ProductTeam")).Attributes.Add("onmouseover", "ctl00_ContentPlaceHolder1_ddl_Office.value = " + detailDef.Office.OfficeId.ToString());

                        com.next.isam.webapp.webservices.UclSmartSelection txt_ProductTeam = (com.next.isam.webapp.webservices.UclSmartSelection)ddl_CostCenter.NamingContainer.FindControl("ddl_ProductTeam");
                        txt_ProductTeam.initControl(webservices.UclSmartSelection.SelectionList.productCode);
                        txt_ProductTeam.setWidth(300);

                        if (detailDef.ProductTeam != null)
                            txt_ProductTeam.ProductCodeId = detailDef.ProductTeam.ProductCodeId;
                    }
                    else
                        ((HtmlTableRow)ddl_CostCenter.NamingContainer.FindControl("row_CostCenter_ProductTeam")).Visible = false;
                }
                else
                {
                    detailDef.Office = CommonUtil.getOfficeRefByKey(ddl_Office.selectedValueToInt);
                    detailDef.CostCenter = null;
                    detailDef.Department = null;
                }
            }

        }

        protected void ddl_RechargeOffice_SelectedIndexChange(object sender, EventArgs e)
        {
            SmartDropDownList ddl_Office = (SmartDropDownList)sender;

            int index = ((RepeaterItem)ddl_Office.NamingContainer).ItemIndex;
            NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[index];

            int officeId = int.Parse(ddl_Office.SelectedValue);
            if (officeId == -1)
                detailDef.Office = null;
            else
                detailDef.Office = CommonUtil.getOfficeRefByKey(officeId);

            if (officeId != -1)
            {
                SmartDropDownList ddl_BusinessEntity = ((SmartDropDownList)((RepeaterItem)ddl_Office.NamingContainer).FindControl("ddl_BusinessEntity"));
                ArrayList arr_CompanyList = CommonUtil.getCompanyList(officeId);

                if (officeId != OfficeId.UK.Id)
                    detailDef.Company = CompanyType.NEXT_SOURCING;

                else if (ddl_BusinessEntity.Items.Count == 1 || this.ddl_Office.selectedValueToInt != officeId)
                    detailDef.Company = CompanyType.getType(((CompanyRef)arr_CompanyList[0]).CompanyId);
                else
                {
                    CompanyRef tmpCompany = null;
                    foreach (CompanyRef company in arr_CompanyList)
                    {
                        if (company.CompanyId == this.ddl_BusinessEntity.selectedValueToInt)
                            tmpCompany = company;
                        else 
                            detailDef.Company = CompanyType.getType(company.CompanyId);                        
                    }
                    arr_CompanyList.Remove(tmpCompany);
                }

                ddl_BusinessEntity.bindList(arr_CompanyList, "CompanyName", "CompanyId", detailDef.Company.Id.ToString());

                if (isAccountUser)
                    ddl_BusinessEntity.Visible = true;
                else
                    ddl_BusinessEntity.Visible = false;


                if (detailDef.InvoiceDetailType != null && detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.OFFICE.Id && detailDef.Office.OfficeId == OfficeId.UK.Id)
                {
                    detailDef.RechargeCurrency = CommonUtil.getCurrencyByKey(CurrencyId.GBP.Id);
                    ((DropDownList)((RepeaterItem)ddl_Office.NamingContainer).FindControl("ddl_RechargeCurrency")).SelectedIndex = 0;
                }
            }

            reloadRechargeItem(index, (RepeaterItem)ddl_Office.NamingContainer);



            //if (detailDef.ExpenseType != null && detailDef.Office != null)
            //{
            //    if (detailDef.ExpenseType.IsProductCode == 1 )
            //    {
            //        com.next.isam.webapp.webservices.UclSmartSelection txt_ProductTeam = (com.next.isam.webapp.webservices.UclSmartSelection)ddl_Office.NamingContainer.FindControl("txt_ProductTeam");
            //        txt_ProductTeam.initControl(webservices.UclSmartSelection.SelectionList.productCode);
            //        txt_ProductTeam.setWidth(300);

            //        if (detailDef.ProductTeam != null)
            //            txt_ProductTeam.ProductCodeId = detailDef.ProductTeam.ProductCodeId;

            //        ((HtmlTableRow)ddl_Office.NamingContainer.FindControl("row_Recharge_ProductTeam")).Visible = true;
            //        ((HtmlTableRow)ddl_Office.NamingContainer.FindControl("row_Recharge_ProductTeam")).Attributes.Add("onmouseover", "ctl00_ContentPlaceHolder1_ddl_Office.value = " + detailDef.Office.OfficeId.ToString());
            //    }
            //    else
            //        ((HtmlTableRow)ddl_Office.NamingContainer.FindControl("row_Recharge_ProductTeam")).Visible = false;

            //    if (detailDef.ExpenseType.IsDepartmentCode == 1)
            //    {
            //        SmartDropDownList dropdown = (SmartDropDownList)(ddl_Office.NamingContainer).FindControl("ddl_CostCenter");
            //        dropdown.bindList(com.next.common.appserver.GeneralManager.Instance.getCostCenterListByOffice(detailDef.Office.OfficeId), "OfficeDescriptionWithCode", "CostCenterId", detailDef.CostCenter == null ? "-1" : detailDef.CostCenter.CostCenterId.ToString(), "-- Please select --", "-1");
            //        ((Control)(ddl_Office.NamingContainer).FindControl("row_Recharge_CostCenter")).Visible = true;
            //    }

            //    if (detailDef.ExpenseType.IsStaffCode == 1)
            //    {
            //        ((HtmlTableRow)ddl_Office.NamingContainer.FindControl("row_Recharge_User")).Attributes.Add("onmouseover", "txt_UserOfficeId.value = " + detailDef.Office.OfficeId.ToString());
            //    }

            //}
        }

        protected void txt_User_SelectionChange(object sender, EventArgs e)
        {
            com.next.isam.webapp.webservices.UclSmartSelection txt_User = (com.next.isam.webapp.webservices.UclSmartSelection)sender;
            int index = ((RepeaterItem)txt_User.NamingContainer).ItemIndex;
            NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTInvoiceDetailList[index];

            if (txt_User.UserId == -1)
            {
                detailDef.User = new UserRef();
                detailDef.User.UserId = -1;
                detailDef.User.DisplayName = "UNCLASSIFIED";
            }
            else
            {
                detailDef.User = CommonUtil.getUserByKey(txt_User.UserId);

                if (detailDef.ExpenseType != null )
                {
                    if (detailDef.ExpenseType.IsDepartmentCode == 1)
                    {
                        detailDef.CostCenter = detailDef.User.CostCenter;
                        SmartDropDownList ddl = (SmartDropDownList)txt_User.NamingContainer.FindControl("ddl_CostCenter");
                        ddl.SelectedIndex = -1;
                        ListItem item = ddl.Items.FindByValue(detailDef.User.CostCenter.CostCenterId.ToString());
                        if (item != null)
                            item.Selected = true;                    
                    }
                    else if (detailDef.ExpenseType.IsDepartmentCode == 0)
                    {
                        detailDef.CostCenter = null;
                        if (detailDef.ExpenseType.IsOfficeCode == 1)
                        {
                            detailDef.Office = CommonUtil.getOfficeRefByKey(ddl_Office.selectedValueToInt);
                            detailDef.Company = CompanyType.getType(ddl_BusinessEntity.selectedValueToInt);
                        }
                    }
                }


                    if (detailDef.ExpenseType != null && detailDef.CostCenter != null)
                    {
                        if (detailDef.ExpenseType.IsOfficeCode == 1)
                        {
                            detailDef.Office = detailDef.CostCenter.Office;
                            detailDef.Company = CompanyType.getType(int.Parse(ddl_BusinessEntity.SelectedValue));
                        }
                        if (detailDef.ExpenseType.IsDepartmentCode == 1)
                            detailDef.Department = CommonUtil.getDepartmentByKey(detailDef.CostCenter.DepartmentId);

                        if (detailDef.ExpenseType.IsProductCode == 1) 
                        //if (detailDef.ExpenseType.IsProductCode == 1 && detailDef.ExpenseType.SUNAccountCode != "1452028") 
                        {
                            ((HtmlTableRow)txt_User.NamingContainer.FindControl("row_CostCenter_ProductTeam")).Visible = true;
                            ((HtmlTableRow)txt_User.NamingContainer.FindControl("row_CostCenter_ProductTeam")).Attributes.Add("onmouseover", "ctl00_ContentPlaceHolder1_ddl_Office.value = " + detailDef.Office.OfficeId.ToString());

                            com.next.isam.webapp.webservices.UclSmartSelection txt_ProductTeam = (com.next.isam.webapp.webservices.UclSmartSelection)txt_User.NamingContainer.FindControl("ddl_ProductTeam");
                            txt_ProductTeam.initControl(webservices.UclSmartSelection.SelectionList.productCode);
                            txt_ProductTeam.setWidth(300);

                            if (detailDef.ProductTeam != null)
                                txt_ProductTeam.ProductCodeId = detailDef.ProductTeam.ProductCodeId;
                        }
                        else
                            ((HtmlTableRow)txt_User.NamingContainer.FindControl("row_CostCenter_ProductTeam")).Visible = false;
                    }
                
            }                                      
        }

        protected void txt_RechargeUser_SelectionChange(object sender, EventArgs e)
        {
            com.next.isam.webapp.webservices.UclSmartSelection txt_User = (com.next.isam.webapp.webservices.UclSmartSelection)sender;
            int index = ((RepeaterItem)txt_User.NamingContainer).ItemIndex;
            NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[index];

            if (txt_User.UserId != int.MinValue)
            {
                if (txt_User.UserId == -1)
                {
                    detailDef.User = new UserRef();
                    detailDef.User.UserId = -1;
                    detailDef.User.DisplayName = "UNCLASSIFIED";
                }
                else
                {
                    detailDef.User = CommonUtil.getUserByKey(txt_User.UserId);

                    if (detailDef.ExpenseType != null && detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.OFFICE.Id && detailDef.ExpenseType.IsDepartmentCode == 1)
                    {
                        detailDef.CostCenter = detailDef.User.CostCenter;
                        detailDef.Department = detailDef.User.Department;

                        SmartDropDownList dropdown = (SmartDropDownList)(txt_User.NamingContainer).FindControl("ddl_CostCenter");
                        if (detailDef.Office == null)
                            dropdown.bindList(new ArrayList(), "OfficeDescriptionWithCode", "CostCenterId", detailDef.CostCenter.CostCenterId.ToString(), "-- Please select --", "-1");
                        else
                            dropdown.bindList(com.next.common.appserver.GeneralManager.Instance.getCostCenterListByOffice(detailDef.Office.OfficeId), "OfficeDescriptionWithCode", "CostCenterId", detailDef.CostCenter.CostCenterId.ToString(), "-- Please select --", "-1");
                        ((Control)(txt_User.NamingContainer).FindControl("row_Recharge_CostCenter")).Visible = true;
                    }

                    if (detailDef.ExpenseType != null)
                    {                        
                        if (detailDef.ExpenseType.SUNAccountCode == "1311303" && ((detailDef.User.CostCenter != null && detailDef.User.CostCenter.Code != "341") && (detailDef.User.Department.CostCenter != null && detailDef.User.Department.CostCenter.Code != "341")))
                        {
                            ScriptManager.RegisterStartupScript(Page, GetType(), "InvoiceRecharge", "alert('The selected staff does not belongs to COP department');", true);
                        }
                        if (detailDef.ExpenseType.SUNAccountCode == "1311307" && ((detailDef.User.CostCenter != null && !detailDef.User.CostCenter.Code.Contains("342")) && (detailDef.User.Department.CostCenter != null && !detailDef.User.Department.CostCenter.Code.Contains("342"))))
                        {
                            ScriptManager.RegisterStartupScript(Page, GetType(), "InvoiceRecharge", "alert('The selected staff does not belongs to PAT department');", true);
                        }
                    }
                }
            }

        }
        
        protected void txtSupplierNameChange(object sender, EventArgs e)
        {
            
            if (txt_SupplierName.NTVendorId != int.MinValue)
            {
                int defCompany = getDefaultCompanyByNTVendor();
                if (defCompany != -1)
                {
                    ddl_BusinessEntity.selectByValue(defCompany.ToString());
                    this.BusinessEntitySelectedIndexChange(null, null);
                }

                NTVendorDef ntVendor = WebUtil.getNTVendorByKey(txt_SupplierName.NTVendorId);
                vwDomainNTInvoiceDef.NTInvoice.NTVendor = ntVendor;
                //if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.DepartmentApprove))
                //    vwNTVendorExpenseTypeList = WebUtil.getNTExpenseTypeList();
                //else
                    vwNTVendorExpenseTypeList = WebUtil.getNTExpenseTypeByNTVendorId(txt_SupplierName.NTVendorId);

                if (ntVendor != null)
                {
                    if (ntVendor.ExpenseType != null)
                    {
                        updateInvoiceDetail(-1, true);
                        updateRechargeDetail(-1, true);

                        if (vwDomainNTInvoiceDef.NTInvoiceDetailList != null && vwDomainNTInvoiceDef.NTInvoiceDetailList.Count > 0)
                        {
                            if (ntVendor.ExpenseType.SUNAccountCode != "1311303" && ntVendor.ExpenseType.SUNAccountCode != "1311307" && ntVendor.ExpenseType.SUNAccountCode != "1311308")
                            {
                                foreach (NTInvoiceDetailDef detailDef in vwDomainNTInvoiceDef.NTInvoiceDetailList)
                                {
                                    detailDef.ExpenseType = ntVendor.ExpenseType;
                                }
                            }
                            if ((ntVendor.ExpenseType.SUNAccountCode == "1311303" || ntVendor.ExpenseType.SUNAccountCode == "1311307" || ntVendor.ExpenseType.SUNAccountCode == "1311308") 
                                && vwDomainNTInvoiceDef.NTInvoiceDetailList.Count == 1)
                            {
                                NTInvoiceDetailDef detailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTInvoiceDetailList[0];
                                if (detailDef.ExpenseType == null && detailDef.CostCenter == null)
                                    vwDomainNTInvoiceDef.NTInvoiceDetailList.RemoveAt(0);
                            }
                        }

                        if (ntVendor.ExpenseType.SUNAccountCode != "1311303" && ntVendor.ExpenseType.SUNAccountCode != "1311307" && ntVendor.ExpenseType.SUNAccountCode != "1311308")
                        {
                            if (vwDomainNTInvoiceDef.NTRechargeDetailList != null && vwDomainNTInvoiceDef.NTRechargeDetailList.Count > 0)
                            {
                                foreach (NTInvoiceDetailDef detailDef in vwDomainNTInvoiceDef.NTRechargeDetailList)
                                {
                                    detailDef.ExpenseType = ntVendor.ExpenseType;
                                }
                            }
                        }
                        else
                        {
                            if (vwDomainNTInvoiceDef.NTRechargeDetailList == null || vwDomainNTInvoiceDef.NTRechargeDetailList.Count == 0)
                            {
                                vwDomainNTInvoiceDef.NTRechargeDetailList = new ArrayList();
                                NTInvoiceDetailDef detailDef = new NTInvoiceDetailDef();
                                detailDef.ExpenseType = ntVendor.ExpenseType;
                                if (ddl_BusinessEntity.selectedValueToInt != CompanyType.NEXT_SHANGHAI.Id)
                                {
                                    detailDef.InvoiceDetailType = NTInvoiceDetailType.CUSTOMER;
                                    detailDef.Customer = WebUtil.getCustomerByKey(2);
                                }
                                detailDef.Office = CommonUtil.getOfficeRefByKey(ddl_Office.selectedValueToInt);
                                detailDef.Company = CompanyType.getType(ddl_BusinessEntity.selectedValueToInt);
                                detailDef.IsRecharge = 1;
                                vwDomainNTInvoiceDef.NTRechargeDetailList.Add(detailDef);
                            }
                        }

                        rep_InvoiceDetail.DataSource = vwDomainNTInvoiceDef.NTInvoiceDetailList;
                        rep_InvoiceDetail.DataBind();

                        rep_RechargeDetail.DataSource = vwDomainNTInvoiceDef.NTRechargeDetailList;
                        rep_RechargeDetail.DataBind();
                    }

                    if (ntVendor.Currency != null)
                    {
                            ddl_Currency.SelectedIndex = -1;
                            ((ListItem)ddl_Currency.Items.FindByValue(ntVendor.Currency.CurrencyId.ToString())).Selected = true;
                    }

                    if (ntVendor.PaymentMethod != null)
                    {
                            ddl_PaymentMethod.SelectedIndex = -1;
                            ((ListItem)ddl_PaymentMethod.Items.FindByValue(ntVendor.PaymentMethod.Id.ToString())).Selected = true;
                    }

                    hf_PaymentTermDays.Value = ntVendor.PaymentTermDays.ToString();
                    if (txt_InvoiceDate.Text != string.Empty)
                    {
                        DateTime invoiceDate;
                        if (DateTime.TryParse(txt_InvoiceDate.Text, out invoiceDate))
                            txt_DueDate.Text = DateTimeUtility.getDateString(invoiceDate.AddDays(ntVendor.PaymentTermDays));
                    }

                }                
            }
        }

        protected void reloadRechargeItem(int index, RepeaterItem repeaterItem)
        {
            NTInvoiceDetailDef ntRechargeDetailDef = (NTInvoiceDetailDef)vwDomainNTInvoiceDef.NTRechargeDetailList[index];                        

            SmartDropDownList dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_RechargeType");
            int rechargeTypeId = dropdown.selectedValueToInt;
            if (dropdown.selectedValueToInt != -1)
                ntRechargeDetailDef.InvoiceDetailType = NTInvoiceDetailType.getType(dropdown.selectedValueToInt);

            dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_ExpenseType");
            if (dropdown.SelectedIndex != -1)
            {
                ntRechargeDetailDef.ExpenseType = WebUtil.getNTExpenseTypeByKey(dropdown.selectedValueToInt);
                if ((ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1311303" || ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1311307") && ddl_BusinessEntity.selectedValueToInt != CompanyType.NEXT_SHANGHAI.Id)
                {
                    rechargeTypeId = NTInvoiceDetailType.CUSTOMER.Id;
                    ntRechargeDetailDef.InvoiceDetailType = NTInvoiceDetailType.CUSTOMER;
                    dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_RechargeType");
                    dropdown.bindList(NTInvoiceDetailType.getRechargeTypeCollectionValues(), "Description", "Id", rechargeTypeId.ToString(), "-- Please select --", "-1");                               
                }
                if (rechargeTypeId != -1 && rechargeTypeId != NTInvoiceDetailType.OFFICE.Id && ntRechargeDetailDef.ExpenseType.IsOfficeCode == 1)
                {
                    ntRechargeDetailDef.Office = CommonUtil.getOfficeRefByKey(ddl_Office.selectedValueToInt);
                    ntRechargeDetailDef.Company = CompanyType.getType(ddl_BusinessEntity.selectedValueToInt);
                }
            }
            else
                ntRechargeDetailDef.ExpenseType = null;

            if (ntRechargeDetailDef.ExpenseType != null)
            {                
                ((Label)repeaterItem.FindControl("lbl_ItemDescHint")).Text = ntRechargeDetailDef.ExpenseType.ItemDescriptionHints;
            }

            int officeId = (vwDomainNTInvoiceDef.NTInvoice.Office == null ? this.LogonUserHomeOffice.OfficeId : vwDomainNTInvoiceDef.NTInvoice.Office.OfficeId);

            com.next.isam.webapp.webservices.UclSmartSelection selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)repeaterItem.FindControl("txt_Vendor");
            //selectionList.setWidth(305);
            //selectionList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.vendorForRecharge);
            if (rechargeTypeId != -1 && rechargeTypeId != NTInvoiceDetailType.USER.Id && rechargeTypeId != NTInvoiceDetailType.OFFICE.Id && rechargeTypeId != NTInvoiceDetailType.CUSTOMER.Id)
            {
                ((Control)repeaterItem.FindControl("row_Recharge_Vendor")).Visible = true;
                Control parentRow = ((Control)repeaterItem.FindControl("row_Recharge_Vendor"));

                if (isAccountUser)
                {
                    bool isARRecharge = NTExpenseTypeRef.isOtherReceivableRecharge(ntRechargeDetailDef.ExpenseType);
                    ((HtmlTableCell)parentRow.FindControl("row_Intercomm1")).Visible = isARRecharge;
                    ((HtmlTableCell)parentRow.FindControl("row_Intercomm2")).Visible = isARRecharge;
                }

                if (rechargeTypeId == NTInvoiceDetailType.NT_VENDOR.Id)
                {
                    selectionList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.ntVendor);
                    selectionList.clear();
                    if (ntRechargeDetailDef.NTVendor != null)
                        selectionList.NTVendorId = ntRechargeDetailDef.NTVendor.NTVendorId;
                }
                else
                {
                    selectionList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.vendorForRecharge);
                    selectionList.clear();
                    if (ntRechargeDetailDef.Vendor != null && ntRechargeDetailDef.Vendor.VendorId != int.MinValue)
                        selectionList.VendorId = ntRechargeDetailDef.Vendor.VendorId;

                    if (rechargeTypeId == NTInvoiceDetailType.FABRIC_VENDOR.Id)
                        txt_VendorType.Value = "1";
                    else if (rechargeTypeId == NTInvoiceDetailType.GARMENT_VENDOR.Id)
                        txt_VendorType.Value = "2";
                    else if (rechargeTypeId == NTInvoiceDetailType.LAUNDRY_VENDOR.Id)
                        txt_VendorType.Value = "5";
                    else if (rechargeTypeId == NTInvoiceDetailType.NON_CLOTHING_VENDOR.Id)
                        txt_VendorType.Value = "4";
                    else if (rechargeTypeId == NTInvoiceDetailType.PACKAGING_VENDOR.Id)
                        txt_VendorType.Value = "7";
                    else if (rechargeTypeId == NTInvoiceDetailType.TRIM_VENDOR.Id)
                        txt_VendorType.Value = "6";
                }
            }
            else
                ((Control)repeaterItem.FindControl("row_Recharge_Vendor")).Visible = false;

            if (rechargeTypeId == NTInvoiceDetailType.CUSTOMER.Id)
            {
                dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_Customer");
                if ((ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1311303" || ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1311307") && ddl_BusinessEntity.selectedValueToInt != CompanyType.NEXT_SHANGHAI.Id)
                {
                    ntRechargeDetailDef.Customer = WebUtil.getCustomerByKey(2);//NEXT RETAIL LTD.
                }
                else
                {
                    if (dropdown.SelectedIndex != -1)
                        ntRechargeDetailDef.Customer = WebUtil.getCustomerByKey(dropdown.selectedValueToInt);
                    else
                        ntRechargeDetailDef.Customer = null;
                }

                ArrayList tmp_Customer = (ArrayList)WebUtil.getCustomerList();
                ArrayList arr_Customer = new ArrayList();
                foreach (domain.common.CustomerDef customer in tmp_Customer)
                {
                    if (customer.CustomerCode != "DIRECTORY" && customer.CustomerCode != "NTN" && customer.CustomerCode != "LIME")
                        arr_Customer.Add(customer);
                }

                dropdown.bindList(arr_Customer, "CustomerDescription", "CustomerId",
                    (ntRechargeDetailDef.Customer == null) ? "-1" : ntRechargeDetailDef.Customer.CustomerId.ToString(), "-- Please select --", "-1");                

                ((Control)repeaterItem.FindControl("row_Recharge_Customer")).Visible = true;

                Control parentRow = ((Control)repeaterItem.FindControl("row_Recharge_Customer"));

                if (isAccountUser)
                {
                    bool isARRecharge = NTExpenseTypeRef.isOtherReceivableRecharge(ntRechargeDetailDef.ExpenseType);
                    ((HtmlTableCell)parentRow.FindControl("row_Intercomm3")).Visible = isARRecharge;
                    ((HtmlTableCell)parentRow.FindControl("row_Intercomm4")).Visible = isARRecharge;
                }
            }
            else
                ((Control)repeaterItem.FindControl("row_Recharge_Customer")).Visible = false;

            //show office drop down for selection only when recharge type = office
            if (rechargeTypeId == NTInvoiceDetailType.OFFICE.Id)
            {
                dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_Office");
                ArrayList arr_OfficeList = WebUtil.getNTRechargeableOfficeList(-1);
                ArrayList arr_CompanyList = CommonUtil.getCompanyList(ddl_Office.selectedValueToInt);
                if (arr_CompanyList.Count == 1)
                {
                    OfficeRef invOffice = null;
                    foreach (OfficeRef tmpOffice in arr_OfficeList)
                    {
                        if (tmpOffice.OfficeId == ddl_Office.selectedValueToInt)
                        {
                            invOffice = tmpOffice;
                            break;
                        }
                    }
                    arr_OfficeList.Remove(invOffice);
                }

                if (ntRechargeDetailDef.Office != null && ntRechargeDetailDef.Company != null
                    && ntRechargeDetailDef.Office.OfficeId == ddl_Office.selectedValueToInt && ntRechargeDetailDef.Company.Id == ddl_BusinessEntity.selectedValueToInt)
                    ntRechargeDetailDef.Office = null;

                dropdown.bindList(arr_OfficeList, "OfficeCode", "OfficeId",
                    (ntRechargeDetailDef.Office == null) ? "-1" : ntRechargeDetailDef.Office.OfficeId.ToString(), "--", "-1");
                ((Control)repeaterItem.FindControl("row_Recharge_Office")).Visible = true;

                dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_BusinessEntity");

                if (ntRechargeDetailDef.Office != null)
                {
                    if (ntRechargeDetailDef.Office.OfficeId == ddl_Office.selectedValueToInt && arr_CompanyList.Count > 1)
                    {
                        CompanyRef invCom = null;
                        foreach (CompanyRef tmpCom in arr_CompanyList)
                        {
                            if (tmpCom.CompanyId == ddl_BusinessEntity.selectedValueToInt)
                            {
                                invCom = tmpCom;
                                break;
                            }
                        }
                        arr_CompanyList.Remove(invCom);
                        dropdown.bindList(arr_CompanyList, "CompanyName", "CompanyId", ntRechargeDetailDef.Company == null ? CompanyType.NEXT_SOURCING.Id.ToString() : ntRechargeDetailDef.Company.Id.ToString());
                    }
                    else
                    {
                        dropdown.bindList(CommonUtil.getCompanyList(ntRechargeDetailDef.Office.OfficeId), "CompanyName", "CompanyId", ntRechargeDetailDef.Company == null ? CompanyType.NEXT_SOURCING.Id.ToString() : ntRechargeDetailDef.Company.Id.ToString());

                    }
                }

                if (!isAccountUser)
                    dropdown.Visible = false;
            }
            else
                ((Control)repeaterItem.FindControl("row_Recharge_Office")).Visible = false;

            if (rechargeTypeId != NTInvoiceDetailType.OFFICE.Id && rechargeTypeId != NTInvoiceDetailType.USER.Id)
            {
                ((Control)repeaterItem.FindControl("row_Recharge_ContactPerson")).Visible = true;
            }
            else
                ((Control)repeaterItem.FindControl("row_Recharge_ContactPerson")).Visible = false;

            selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)repeaterItem.FindControl("txt_User");
            //selectionList.setWidth(305);
            //selectionList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.user);
            if (rechargeTypeId == NTInvoiceDetailType.USER.Id ||
//                (rechargeTypeId == NTInvoiceDetailType.OFFICE.Id && ntRechargeDetailDef.ExpenseType != null && ntRechargeDetailDef.ExpenseType.IsStaffCode == 1) ||
                (ntRechargeDetailDef.ExpenseType != null && ((rechargeTypeId == NTInvoiceDetailType.OFFICE.Id && ntRechargeDetailDef.ExpenseType.IsStaffCode == 1) || 
                ( ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1311303" || ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1311307"))))
            {
                //if (ntRechargeDetailDef.User != null && ntRechargeDetailDef.User.UserId != int.MinValue)
                //    selectionList.UserId = ntRechargeDetailDef.User.UserId;
                ((Control)repeaterItem.FindControl("row_Recharge_User")).Visible = true;
                if (rechargeTypeId == NTInvoiceDetailType.OFFICE.Id && ntRechargeDetailDef.Office != null)
                    if (ntRechargeDetailDef.ExpenseType.SUNAccountCode == "4104104") // mobile phone
                        ((HtmlTableRow)repeaterItem.FindControl("row_Recharge_User")).Attributes.Add("onmouseover", "txt_UserOfficeId.value = '-1';");
                    else
                        ((HtmlTableRow)repeaterItem.FindControl("row_Recharge_User")).Attributes.Add("onmouseover", "txt_UserOfficeId.value = " + ntRechargeDetailDef.Office.OfficeId.ToString());
                else
                    ((HtmlTableRow)repeaterItem.FindControl("row_Recharge_User")).Attributes.Add("onmouseover", "txt_UserOfficeId.value = '-1';");
            }
            else
                ((Control)repeaterItem.FindControl("row_Recharge_User")).Visible = false;

            selectionList = (com.next.isam.webapp.webservices.UclSmartSelection)repeaterItem.FindControl("txt_ProductTeam");
            //selectionList.setWidth(305);
            //selectionList.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
            if (ntRechargeDetailDef.ExpenseType != null && rechargeTypeId == NTInvoiceDetailType.OFFICE.Id)
            {
                if (ntRechargeDetailDef.ExpenseType.IsProductCode == 1 && ntRechargeDetailDef.Office != null)
                //if (ntRechargeDetailDef.ExpenseType.IsProductCode == 1 && ntRechargeDetailDef.ExpenseType.SUNAccountCode != "1452028" && ntRechargeDetailDef.Office != null)
                {
                    ((HtmlTableRow)repeaterItem.FindControl("row_Recharge_ProductTeam")).Visible = true;
                    ((HtmlTableRow)repeaterItem.FindControl("row_Recharge_ProductTeam")).Attributes.Add("onmouseover", "ctl00_ContentPlaceHolder1_ddl_Office.value = " + ntRechargeDetailDef.Office.OfficeId.ToString());
                }
                else
                    ((HtmlTableRow)repeaterItem.FindControl("row_Recharge_ProductTeam")).Visible = false;

                if (ntRechargeDetailDef.ExpenseType.IsDepartmentCode == 1 && ntRechargeDetailDef.Office != null)
                {
                    dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_CostCenter");
                    if (dropdown.SelectedIndex != -1)
                        ntRechargeDetailDef.CostCenter = CommonUtil.getCostCenterByKey(dropdown.selectedValueToInt);
                    else
                        ntRechargeDetailDef.CostCenter = null;

                    dropdown.bindList(com.next.common.appserver.GeneralManager.Instance.getCostCenterListByOffice(ntRechargeDetailDef.Office.OfficeId),
                        "OfficeDescriptionWithCode", "CostCenterId", ntRechargeDetailDef.CostCenter == null ? "-1" : ntRechargeDetailDef.CostCenter.CostCenterId.ToString(), "-- Please select --", "-1");
                    ((Control)repeaterItem.FindControl("row_Recharge_CostCenter")).Visible = true;
                }
                else
                    ((Control)repeaterItem.FindControl("row_Recharge_CostCenter")).Visible = false;

                if (ntRechargeDetailDef.ExpenseType.IsSeasonCode == 1 && ntRechargeDetailDef.ExpenseType.SUNAccountCode != "1452028")
                {
                    dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_Season");
                    if (dropdown.SelectedIndex != -1)
                        ntRechargeDetailDef.Season = CommonUtil.getSeasonByKey(dropdown.selectedValueToInt);
                    else
                        ntRechargeDetailDef.Season = null;

                    dropdown.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", ntRechargeDetailDef.Season == null ? "" : ntRechargeDetailDef.Season.SeasonId.ToString(), "-- Please select --", "-1");
                    ((Control)repeaterItem.FindControl("row_Recharge_Season")).Visible = true;
                }
                else
                    ((Control)repeaterItem.FindControl("row_Recharge_Season")).Visible = false;

                if (ntRechargeDetailDef.ExpenseType.IsItemNo == 1)
                {
                    ((Control)repeaterItem.FindControl("row_Recharge_ItemNo")).Visible = true;
                }
                else
                    ((Control)repeaterItem.FindControl("row_Recharge_ItemNo")).Visible = false;

                if (ntRechargeDetailDef.ExpenseType.IsDevSampleCostType == 1)
                {
                    ((Control)repeaterItem.FindControl("row_Recharge_DevSampleType")).Visible = true;
                    dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_DevSampleType");
                    if (dropdown.SelectedIndex != -1)
                        ntRechargeDetailDef.DevSampleCostTypeId = dropdown.selectedValueToInt;
                    else
                        ntRechargeDetailDef.DevSampleCostTypeId = 0;
                    dropdown.bindList(NTDevSampleCostType.getNTDevSampleCostTypeList(), "Description", "Id", ntRechargeDetailDef.DevSampleCostTypeId == 0 ? "-1" : ntRechargeDetailDef.DevSampleCostTypeId.ToString());
                }
                else
                    ((Control)repeaterItem.FindControl("row_Recharge_DevSampleType")).Visible = false;

                if (ntRechargeDetailDef.ExpenseType.IsQtyRequired == 1)
                {
                    ((Control)repeaterItem.FindControl("row_Recharge_Quantity")).Visible = true;
                }
                else
                    ((Control)repeaterItem.FindControl("row_Recharge_Quantity")).Visible = false;

                if (ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1412101")
                {
                    ((Control)dropdown.NamingContainer.FindControl("row_Recharge_ExpenseNature")).Visible = true;
                    dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_Nature");
                    if (dropdown.SelectedIndex != -1)
                        ntRechargeDetailDef.NatureIdForAccrual = dropdown.selectedValueToInt;
                    else
                        ntRechargeDetailDef.NatureIdForAccrual = 0;

                    dropdown.bindList(NonTradeManager.Instance.getNTExpenseNatureList(txt_SupplierName.NTVendorId), "Description", "NatureId", ntRechargeDetailDef.NatureIdForAccrual == 0 ? "-1" : ntRechargeDetailDef.NatureIdForAccrual.ToString(), "-- Please select --", "-1");
                }
                else
                    ((Control)repeaterItem.FindControl("row_Recharge_ExpenseNature")).Visible = false;

                /* TODO:TRADINGAF */
                ((Control)repeaterItem.FindControl("row_Recharge_TradingAF")).Visible = (ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1452028");

                if (ntRechargeDetailDef.ExpenseType.SUNAccountCode == "1452028")
                {
                    if (ntRechargeDetailDef.SegmentValue7 == null)
                        ((SmartDropDownList)repeaterItem.FindControl("ddl_SegmentValue7")).selectByValue("1"); // U
                    if (ntRechargeDetailDef.SegmentValue8 == null)
                        ((SmartDropDownList)repeaterItem.FindControl("ddl_SegmentValue8")).selectByValue("10"); // G
                }
            }
            else
            {
                ((Control)repeaterItem.FindControl("row_Recharge_Office")).Visible = false;
                ((HtmlTableRow)repeaterItem.FindControl("row_Recharge_ProductTeam")).Visible = false;
                ((Control)repeaterItem.FindControl("row_Recharge_CostCenter")).Visible = false;
                ((Control)repeaterItem.FindControl("row_Recharge_DevSampleType")).Visible = false;
                ((Control)repeaterItem.FindControl("row_Recharge_ItemNo")).Visible = false;
                ((Control)repeaterItem.FindControl("row_Recharge_Season")).Visible = false;
                ((Control)repeaterItem.FindControl("row_Recharge_ExpenseNature")).Visible = false;
                ((Control)repeaterItem.FindControl("row_Recharge_Quantity")).Visible = false;
                ((Control)repeaterItem.FindControl("row_Recharge_TradingAF")).Visible = false;
            }

            if (ntRechargeDetailDef.ExpenseType != null && ntRechargeDetailDef.ExpenseType.IsSegmentValue == 1)
            {
                if ((NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId) &&
                    (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DEPARTMENT_APPROVED.Id || vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_EVALUATING.Id)) ||
                    (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId) &&
                    vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.ACCOUNTS_APPROVED.Id)                    
                    )
                {
                    ((Control)repeaterItem.FindControl("row_Recharge_SegmentField7")).Visible = true;
                    ((Control)repeaterItem.FindControl("row_Recharge_SegmentField8")).Visible = true;

                    dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_SegmentValue7");
                    if (dropdown.SelectedIndex != -1)
                        ntRechargeDetailDef.SegmentValue7 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(int.Parse(dropdown.SelectedValue));
                    dropdown = (SmartDropDownList)repeaterItem.FindControl("ddl_SegmentValue8");
                    if (dropdown.SelectedIndex != -1)
                        ntRechargeDetailDef.SegmentValue8 = NonTradeManager.Instance.getNTEpicorSegmentValueByKey(int.Parse(dropdown.SelectedValue));                    
                }
                else if (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId) ||
                        NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, GeneralCriteria.ALL).Contains(officeId) ||
                        NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SUN_INTERFACE.Id, GeneralCriteria.ALL).Contains(officeId) ||
                        CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelARApprove) ||
                        CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.SecondLevelARApprove))
                {
                    ((Control)repeaterItem.FindControl("row_Recharge_SegmentField7")).Visible = true;
                    ((Control)repeaterItem.FindControl("row_Recharge_SegmentField8")).Visible = true;
                    ((SmartDropDownList)repeaterItem.FindControl("ddl_SegmentValue7")).Enabled = true;
                    ((SmartDropDownList)repeaterItem.FindControl("ddl_SegmentValue8")).Enabled = true;
                }
            }
            else
            {
                ((Control)repeaterItem.FindControl("row_Recharge_SegmentField7")).Visible = false;
                ((Control)repeaterItem.FindControl("row_Recharge_SegmentField8")).Visible = false;
                ntRechargeDetailDef.SegmentValue7 = null;
                ntRechargeDetailDef.SegmentValue8 = null;
            }
        }

        /* todo */
        
        private string validateProcurementRequestNo(out APDS.ProcurementRequestDef requestDef)
        {
            APDS.APDSService svc = new APDS.APDSService();
            requestDef = svc.GetApprovedProcurementRequestDefByRequestNo(this.ddl_Office.selectedValueToInt, this.txtProcurementRequestNo.Text.Trim());
            if (requestDef == null)
                return "Invalid Asset Procurement Request Form No.";
            
            //NTInvoiceDef invoiceDef = NonTradeManager.Instance.getNTInvoiceByProcurementRequestId(requestDef.RequestId);
            //if (invoiceDef != null
            //    && ((vwDomainNTInvoiceDef.NTInvoice == null || vwDomainNTInvoiceDef.NTInvoice.InvoiceId <= 0)
            //    || (vwDomainNTInvoiceDef.NTInvoice.InvoiceId != invoiceDef.InvoiceId)))
            //{
            //    return "The Asset Procurement Request Form No. has been referenced by another Non-Trade Invoice";
            //}
            return string.Empty;
        }

        protected void btnProcurement_Click(object sender, EventArgs e)
        {
            if (this.txtProcurementRequestNo.Text.Trim() == string.Empty)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Asset Procurement", "alert('Please enter Asset Procurement Request Form No.')", true);
                return;
            }

            APDS.ProcurementRequestDef requestDef;

            string msg = validateProcurementRequestNo(out requestDef);
            if (msg != string.Empty)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Asset Procurement", "alert('" + msg + "')", true);
                return;
            }

            string para = SecurityManager.Instance.getAccessibleToken(this.LogonUserId);
            string strLink = ((Button)sender).CommandArgument + para;
            strLink += ("&requestId=" + requestDef.RequestId.ToString());

            com.next.infra.util.ClientScript.windowOpen(strLink, "APDS_Procurement", 1000, 800, false, true, true, false, true, this.Page);
        }

        protected void val_Procurement_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (this.txtProcurementRequestNo.Text.Trim() != string.Empty)
            {
                APDS.ProcurementRequestDef requestDef;
                string msg = validateProcurementRequestNo(out requestDef);
                if (msg != string.Empty)
                {
                    args.IsValid = false;
                    val_Procurement.ErrorMessage = msg;
                    return;
                }
            }

        }

        protected void payByHK() 
        {
            if (NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL).Contains(ddl_Office.selectedValueToInt) ||
                NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.SECOND_APPROVER.Id, GeneralCriteria.ALL).Contains(ddl_Office.selectedValueToInt) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.FirstLevelARApprove) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ntExpenseInvoice.Id, ISAMModule.ntExpenseInvoice.SecondLevelARApprove))
            { 
                ckb_PayByHK.Enabled = true; 
            }

            if (vwDomainNTInvoiceDef.NTInvoice.WorkflowStatus.Id == NTWFS.DRAFT.Id && ddl_Currency.selectedValueToInt != CurrencyId.LKR.Id && ddl_Office.selectedValueToInt == OfficeId.SL.Id)
            {
                ckb_PayByHK.Checked = true;
            }
            else if (ddl_BusinessEntity.selectedValueToInt == CompanyType.NEXT_SOURCING.Id 
                     && (ddl_Office.selectedValueToInt == OfficeId.CA.Id || ddl_Office.selectedValueToInt == OfficeId.TH.Id || ddl_Office.selectedValueToInt == OfficeId.VN.Id 
                        || ddl_Office.selectedValueToInt == OfficeId.PK.Id
                        || ddl_Office.selectedValueToInt == OfficeId.BD.Id))
            {
                ckb_PayByHK.Checked = true;
            }

            if (ddl_BusinessEntity.selectedValueToInt != CompanyType.NEXT_SOURCING.Id)
                ckb_PayByHK.Checked = false;
        }

        protected void ddl_Currency_SelectedIndexChanged(object sender, EventArgs e)
        {
            ckb_PayByHK.Checked = false;
            payByHK();
        }

        protected int getDefaultCompanyByNTVendor()
        {
            int defCompany = -1;
            if (txt_SupplierName.NTVendorId != int.MinValue)
            {
                NTVendorDef ntvendor = WebUtil.getNTVendorByKey(txt_SupplierName.NTVendorId);
                if (ntvendor.CompanyId != int.MinValue)
                {
                    defCompany = ntvendor.CompanyId;
                }
            }
            return defCompany;
        }
        
    }
}
