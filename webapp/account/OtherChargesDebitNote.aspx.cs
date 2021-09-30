using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.infra.web;
using com.next.isam.webapp.commander;
using com.next.isam.appserver.common;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.OleDb;
using com.next.infra.util;
using com.next.isam.domain.account;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.types;
using com.next.common.domain.industry.vendor;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.account;
using com.next.common.datafactory.worker;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.order;

namespace com.next.isam.webapp.account
{
    public partial class OtherChargesDebitNote : com.next.isam.webapp.usercontrol.PageTemplate
    {
        ArrayList vwOfficeExpenseTypeList
        {
            get { return (ArrayList)ViewState["vwOfficeExpenseTypeList"]; }
            set { ViewState["vwOfficeExpenseTypeList"] = value; }
        }

        ArrayList vwNoteList
        {
            get { return (ArrayList)ViewState["NoteList"]; }
            set { ViewState["NoteList"] = value; }
        }

        public class IssueType
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                bool isAccountUser = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.Id, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.ViewForAccountsUser);
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
                this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId");

                IList statuslist = new List<IssueType>()
                {
                 new IssueType(){ ID=1, Name="Accounts Payable - Supplier ID"},
                 new IssueType(){ ID=2, Name="Accounts Receivable - Customer ID"},
                 new IssueType(){ ID=3, Name="General Ledger - GL Account"},
                };
                ddlIssuedTo.DataSource = statuslist;
                ddlIssuedTo.DataTextField = "Name";
                ddlIssuedTo.DataValueField = "ID";
                ddlIssuedTo.DataBind();

                ddl_PaymentOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "SAME AS OFFICE", GeneralCriteria.ALL.ToString());

                vwOfficeExpenseTypeList = WebUtil.getNTExpenseTypeListByOfficeId(ddl_Office.selectedValueToInt);
                ArrayList expenseTypeList = vwOfficeExpenseTypeList;
                ddlRechargeType.bindList(expenseTypeList, (isAccountUser ? "SimpleAccountCodeDescription" : "Description"), "ExpenseTypeId", "-1", "-- Please select --", GeneralCriteria.ALL.ToString());
            }
        }

        protected void btn_upload(object sender, EventArgs e)
        {
            if (uplFile.HasFile)
            {
                if (txt_DebitNoteDate.DateTime == DateTime.MinValue)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DCNoteDate", "alert('Please enter debit / credit note date');", true);
                    return;
                }

                int issuedTo = Convert.ToInt32(ddlIssuedTo.SelectedValue);
                string path = Path.GetFileName(uplFile.FileName);
                string indicator = ddlDocType.SelectedValue == "0" ? "D" : "C";
                path = path.Replace(" ", "");
                uplFile.SaveAs(WebConfig.getValue("appSettings", "UT_DN_FOLDER") + path);
                String ExcelPath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + path;
                OleDbConnection mycon = new OleDbConnection("Provider = Microsoft.ACE.OLEDB.12.0; Data Source = " + ExcelPath + "; Extended Properties=Excel 8.0; Persist Security Info = False");
                mycon.Open();

                DataTable dtExcelSchema;
                dtExcelSchema = mycon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                OleDbCommand cmd = new OleDbCommand("select * from [" + SheetName + "]", mycon);
                OleDbDataAdapter da = new OleDbDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    lbl_errorMsg.Text = "Data not found! Please check your excel file.";
                    return;
                }

                this.vwNoteList = new ArrayList();
                OtherChargesImportFileDef def = new OtherChargesImportFileDef();
                bool vaild = true;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        string autoUpload = ds.Tables[0].Rows[i][6].ToString() == "DN" ? "D" : "C";
                        if (string.IsNullOrEmpty(ds.Tables[0].Rows[i][j].ToString()))
                        {
                            vaild = false;
                        }
                        if (autoUpload != indicator)
                        {
                            vaild = false;
                        }
                    }
                    if (vaild)
                    {
                        def = new OtherChargesImportFileDef();
                        def.ID = ds.Tables[0].Rows[i][0].ToString();
                        def.Name = ds.Tables[0].Rows[i][1].ToString();
                        def.Description = ds.Tables[0].Rows[i][2].ToString();
                        def.OriCurrencyCode = ds.Tables[0].Rows[i][3].ToString();

                        double value = double.Parse(ds.Tables[0].Rows[i][4].ToString());
                        string result = value.ToString("f2");
                        def.Amount = Math.Round(decimal.Parse(result), 2);

                        def.BilCurrencyCode = ds.Tables[0].Rows[i][5].ToString();
                        def.DebitCreditIndicator = ds.Tables[0].Rows[i][6].ToString() == "DN" ? "D" : "C";
                        def.ExchangeRate = Math.Round(CommonWorker.Instance.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, GeneralWorker.Instance.getCurrencyByCurrencyCode(def.OriCurrencyCode).CurrencyId, txt_DebitNoteDate.DateTime)
                         / CommonWorker.Instance.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, GeneralWorker.Instance.getCurrencyByCurrencyCode(def.BilCurrencyCode).CurrencyId, DateTime.Today), 6);
                        def.BilAmount = Math.Round(Math.Round(CommonWorker.Instance.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, GeneralWorker.Instance.getCurrencyByCurrencyCode(def.OriCurrencyCode).CurrencyId, txt_DebitNoteDate.DateTime)
                         / CommonWorker.Instance.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, GeneralWorker.Instance.getCurrencyByCurrencyCode(def.BilCurrencyCode).CurrencyId, DateTime.Today), 6) * def.Amount, 2);

                        if (issuedTo == 1)
                        {
                            TypeCollector vendorTypes = TypeCollector.Exclusive;
                            VendorRef vendorDef = VendorWorker.Instance.getVendorByEpicorSupplierId(def.ID, vendorTypes);
                            NTVendorDef ntVendorDef = NonTradeManager.Instance.getNTVendorByEPVendorCode(def.ID, false);
                            if (vendorDef == null && ntVendorDef == null)
                            {
                                def.ErrorMessage = def.ID + " Not found!";
                            }
                            else if (vendorDef != null)
                            {
                                if (ddl_Office.selectedValueToInt != vendorDef.OfficeId)
                                {
                                    ArrayList contractDef = OrderSelectWorker.Instance.getContractListByCriteria(ddl_Office.selectedValueToInt, GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralCriteria.ALL, vendorDef.VendorId, GeneralCriteria.ALL, vendorTypes);
                                    if (contractDef.Count > 0)
                                    {
                                        def.VPSName = vendorDef.Name;
                                        def.VendorOffice = GeneralWorker.Instance.getOfficeRefByKey(vendorDef.OfficeId).OfficeCode;
                                    }
                                    else
                                        def.ErrorMessage = vendorDef.Name + " is not " + ddl_Office.selectedText + " office vendor";
                                }
                                else
                                {
                                    def.VPSName = vendorDef.Name;
                                    def.VendorOffice = GeneralWorker.Instance.getOfficeRefByKey(vendorDef.OfficeId).OfficeCode;
                                }
                            }
                            else if (ntVendorDef != null)
                            {
                                def.VPSName = ntVendorDef.VendorName;
                                def.VendorOffice = ntVendorDef.Office.OfficeCode;
                            }
                            /*
                            else if (vendorDef.Name != def.Name)
                            {
                                def.ErrorMessage = def.ID + " is not match with " + def.Name;
                            }
                            */
                        }
                        else if (issuedTo == 2)
                        {
                            ARCustomerDef arDef = AccountManager.Instance.getARCustomerByCode(def.ID);
                            if (arDef == null)
                            {
                                def.ErrorMessage = def.ID + " Not found!";
                            }
                            else
                            {
                                def.VPSName = arDef.Name;
                            }
                            /*
                            else if (arDef.Name != def.Name)
                            {
                                def.ErrorMessage = def.ID + " is not match with " + def.Name;
                            }
                            */
                        }
                        else if (issuedTo == 3)
                        {
                            if (!GenericDCNoteDef.getGLCodeList().Contains(def.ID))
                            {
                                def.ErrorMessage = "Invalid GL Account Id";
                            }
                            def.VPSName = def.Name;
                        }
                        vwNoteList.Add(def);
                    }
                    vaild = true;
                }

                string msg = string.Empty;
                lbl_errorMsg.Text = string.Empty;
                for (int p = 0; p < vwNoteList.Count; p++)
                {
                    OtherChargesImportFileDef tem = (OtherChargesImportFileDef)vwNoteList[p];
                    if (!string.IsNullOrEmpty(tem.ErrorMessage))
                        msg += tem.ErrorMessage + "<br/>";
                }

                if (!string.IsNullOrEmpty(msg))
                {
                    lbl_errorMsg.Text = msg + "Please check and upload again!";
                    gv_excel.DataSource = "";
                    gv_excel.DataBind();
                    btn_Submit.Visible = false;
                }
                else
                {
                    gv_excel.DataSource = vwNoteList;
                    gv_excel.DataBind();
                    btn_Submit.Visible = true;
                    checkboxVisible();
                }
                mycon.Close();
            }
            else
            {
                lbl_errorMsg.Text = "Please upload Excel File!";
                btn_Submit.Visible = false;
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            int RechargeType = Convert.ToInt32(ddlRechargeType.SelectedValue);
            bool autoUpload = chbEpicor.Checked;
            NTExpenseTypeRef type = ddlRechargeType.SelectedValue == "-1" ? null : WebUtil.getNTExpenseTypeByKey(int.Parse(ddlRechargeType.SelectedValue));
            int issuedTo = Convert.ToInt32(ddlIssuedTo.SelectedValue);
            List<GenericDCNoteDef> noteList = new List<GenericDCNoteDef>();

            List<OtherChargesImportFileDef> selectedList = new List<OtherChargesImportFileDef>();

            //update description
            for (int j = 0; j < gv_excel.Rows.Count; j++)
            {
                decimal amt;
                bool isNumeric = decimal.TryParse(((TextBox)gv_excel.Rows[j].FindControl("txt_BillingAmt")).Text, out amt);
                if (!isNumeric)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Billing Amt", "alert('Billing Amt must be number only!');", true);
                    return;
                }

                if (((CheckBox)gv_excel.Rows[j].FindControl("chb_check")).Checked == true)
                {
                    OtherChargesImportFileDef gv = (OtherChargesImportFileDef)vwNoteList[j];
                    gv.Description = ((TextBox)gv_excel.Rows[j].FindControl("txt_Description")).Text;
                    gv.Attn = ((TextBox)gv_excel.Rows[j].FindControl("txt_Attention")).Text;
                    gv.BilAmount = decimal.Parse(((TextBox)gv_excel.Rows[j].FindControl("txt_BillingAmt")).Text);
                    selectedList.Add(gv);
                }
            }

            if (selectedList.Count <= 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "alert('No Row Selected!');", true);
                return;
            }

            if (issuedTo == 1)
            {
                noteList = new List<GenericDCNoteDef>();

                foreach (OtherChargesImportFileDef gv in selectedList)

                {
                    GenericDCNoteDef def = new GenericDCNoteDef();

                    VendorRef vendor = VendorWorker.Instance.getVendorByEpicorSupplierId(gv.ID, TypeCollector.Exclusive);
                    NTVendorDef ntVendor = NonTradeWorker.Instance.getNTVendorByEPVendorCode(gv.ID, false);

                    def.DCNoteDate = txt_DebitNoteDate.DateTime;
                    def.IssueTypeId = issuedTo;
                    def.OfficeId = int.Parse(ddl_Office.SelectedValue);
                    def.VendorId = (vendor != null ? vendor.VendorId : -1);
                    def.NTVendorId = (ntVendor != null ? ntVendor.NTVendorId : -1);
                    def.OriginalCurrencyId = GeneralWorker.Instance.getCurrencyByCurrencyCode(gv.OriCurrencyCode).CurrencyId;
                    def.OriginalAmount = gv.Amount;
                    def.BillingCurrencyId = GeneralWorker.Instance.getCurrencyByCurrencyCode(gv.BilCurrencyCode).CurrencyId;
                    def.Amount = gv.BilAmount;
                    def.DebitCreditIndicator = ddlDocType.SelectedValue == "0" ? "D" : "C";
                    def.PartyName = (vendor != null ? vendor.Name : ntVendor.VendorName);
                    def.PartyAddress1 = (vendor != null ? vendor.Address0 : ntVendor.Address);
                    def.PartyAddress2 = (vendor != null ? vendor.Address1 : string.Empty);
                    def.PartyAddress3 = (vendor != null ? vendor.Address2 : string.Empty);
                    def.PartyAddress4 = (vendor != null ? vendor.Address3 : string.Empty);
                    def.Description = gv.Description;
                    def.COA = type.EpicorCode;
                    def.Remark = string.Empty;
                    def.IsInterfaced = false;
                    def.MailStatus = 0;
                    def.SettlementDate = DateTime.MinValue;
                    def.PaymentOfficeId = ddl_PaymentOffice.SelectedValue == "-1" ? int.Parse(ddl_Office.SelectedValue) : int.Parse(ddl_PaymentOffice.SelectedValue);
                    def.Status = 1;
                    def.Attn = gv.Attn;

                    noteList.Add(def);
                }
            }
            else if (issuedTo == 2)
            {
                noteList = new List<GenericDCNoteDef>();

                foreach (OtherChargesImportFileDef gv in selectedList)
                {
                    GenericDCNoteDef def = new GenericDCNoteDef();
                    ARCustomerDef arDef = AccountManager.Instance.getARCustomerByCode(gv.ID);

                    def.DCNoteDate = txt_DebitNoteDate.DateTime;
                    def.IssueTypeId = issuedTo;
                    def.OfficeId = int.Parse(ddl_Office.SelectedValue);
                    def.CustomerCode = arDef.EpicorCode;
                    def.OriginalCurrencyId = GeneralWorker.Instance.getCurrencyByCurrencyCode(gv.OriCurrencyCode).CurrencyId;
                    def.OriginalAmount = gv.Amount;
                    def.BillingCurrencyId = GeneralWorker.Instance.getCurrencyByCurrencyCode(gv.BilCurrencyCode).CurrencyId;
                    def.Amount = gv.BilAmount;
                    def.DebitCreditIndicator = ddlDocType.SelectedValue == "0" ? "D" : "C";
                    def.PartyName = arDef.Name;
                    def.PartyAddress1 = arDef.Addr1;
                    def.PartyAddress2 = arDef.Addr2;
                    def.PartyAddress3 = arDef.Addr3;
                    def.PartyAddress4 = arDef.Addr4;
                    def.Description = gv.Description;
                    def.COA = type.EpicorCode;
                    def.Remark = string.Empty;
                    def.IsInterfaced = false;
                    def.MailStatus = 0;
                    def.SettlementDate = DateTime.MinValue;
                    def.PaymentOfficeId = ddl_PaymentOffice.SelectedValue == "-1" ? int.Parse(ddl_Office.SelectedValue) : int.Parse(ddl_PaymentOffice.SelectedValue);
                    def.Status = 1;
                    def.Attn = gv.Attn;

                    noteList.Add(def);
                }
            }
            else if (issuedTo == 3)
            {
                noteList = new List<GenericDCNoteDef>();

                foreach (OtherChargesImportFileDef gv in selectedList)
                {
                    GenericDCNoteDef def = new GenericDCNoteDef();

                    def.DCNoteDate = txt_DebitNoteDate.DateTime;
                    def.IssueTypeId = issuedTo;
                    def.OfficeId = int.Parse(ddl_Office.SelectedValue);
                    def.GLCode = gv.ID;
                    def.OriginalCurrencyId = GeneralWorker.Instance.getCurrencyByCurrencyCode(gv.OriCurrencyCode).CurrencyId;
                    def.OriginalAmount = gv.Amount;
                    def.BillingCurrencyId = GeneralWorker.Instance.getCurrencyByCurrencyCode(gv.BilCurrencyCode).CurrencyId;
                    def.Amount = gv.BilAmount;
                    def.DebitCreditIndicator = ddlDocType.SelectedValue == "0" ? "D" : "C";
                    def.PartyName = "NEXT MANUFACTURING (PVT) LTD-TESTING";
                    def.PartyAddress1 = "WORLD TRADE CENTER CEAST TOWER";
                    def.PartyAddress2 = "17/F FLOOR";
                    def.PartyAddress3 = "ECHELON SQUARE, COLOMBO 01";
                    def.PartyAddress4 = "SRI LANKA"; def.Description = gv.Description;
                    def.Description = gv.Description;
                    def.COA = type.EpicorCode;
                    def.Remark = string.Empty;
                    def.IsInterfaced = false;
                    def.MailStatus = 0;
                    def.SettlementDate = DateTime.MinValue;
                    def.PaymentOfficeId = ddl_PaymentOffice.SelectedValue == "-1" ? int.Parse(ddl_Office.SelectedValue) : int.Parse(ddl_PaymentOffice.SelectedValue);
                    def.Status = 1;
                    def.Attn = gv.Attn;

                    noteList.Add(def);
                }
            }

            AccountFinancialCalenderDef calenderDef = GeneralWorker.Instance.getAccountPeriodByDate(AppId.ISAM.Code, txt_DebitNoteDate.DateTime);

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GenerateOtherChargesDCNote);

            Context.Items.Add(AccountCommander.Param.officeId, int.Parse(ddl_Office.SelectedValue));
            Context.Items.Add(AccountCommander.Param.fiscalYear, calenderDef.BudgetYear);
            Context.Items.Add(AccountCommander.Param.period, calenderDef.Period);
            Context.Items.Add(AccountCommander.Param.genericDCNoteList, noteList);
            Context.Items.Add(AccountCommander.Param.autoUpload, autoUpload);

            forwardToScreen(null);

            string fileName = (string)Context.Items[AccountCommander.Param.file];
            WebHelper.outputFileAsHttpRespone(Response, fileName, true);
        }

        protected void gv_excel_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int issuedTo = Convert.ToInt32(ddlIssuedTo.SelectedValue);

            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (issuedTo == 1)
                {
                    e.Row.Cells[1].Text = "Supplier Id";
                    e.Row.Cells[2].Text = "Supplier Name";
                    //e.Row.Cells[4].Text = "Supplier Name (System)";
                    e.Row.Cells[11].Visible = true;
                }
                else if (issuedTo == 2)
                {
                    e.Row.Cells[1].Text = "Customer Id";
                    e.Row.Cells[2].Text = "Customer Name";
                    //e.Row.Cells[4].Text = "Customer Name (System)";
                    e.Row.Cells[11].Visible = false;
                }
                else
                {
                    e.Row.Cells[1].Text = "G/L A/C";
                    e.Row.Cells[2].Text = "Account Name";
                    //e.Row.Cells[4].Text = "Account Name (System)";
                    e.Row.Cells[11].Visible = false;
                }
            }

        }

        protected void checkboxVisible()
        {
            for (int i = 0; i < gv_excel.Rows.Count; i++)
            {
                OtherChargesImportFileDef gv = (OtherChargesImportFileDef)vwNoteList[i];
                if (ddl_Office.selectedText != gv.VendorOffice && int.Parse(ddlIssuedTo.SelectedValue) == 1)
                {
                    /*
                    ((CheckBox)gv_excel.Rows[i].FindControl("chb_check")).Visible = false;
                    ((CheckBox)gv_excel.Rows[i].FindControl("chb_check")).Checked = false;
                    */
                    gv_excel.Rows[i].Cells[11].BackColor = System.Drawing.Color.Yellow;
                }
            }
        }

    }
}
