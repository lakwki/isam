using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Linq;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.infra.web;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.types;
using com.next.isam.appserver.account;
using com.next.common.domain.dms;
using com.next.common.domain.industry.vendor;
using com.next.infra.util;
using System.IO;

namespace com.next.isam.webapp.nontrade
{
    public partial class NonTradeVendor : com.next.isam.webapp.usercontrol.PageTemplate
    {
        NTVendorDef vwNTVendor
        {
            get
            {
                return (NTVendorDef)ViewState["vwNTVendor"];
            }
            set
            {
                ViewState["vwNTVendor"] = value;
            }
        }

        ArrayList vwExpenseTypeMappingList
        {
            get
            {
                return (ArrayList) ViewState["vwExpenseTypeMappingList"];
            }
            set
            {
                ViewState["vwExpenseTypeMappingList"] = value;
            }
        }

        ArrayList vwUpdatedExpenseTypeMappingList
        {
            get
            {
                return (ArrayList)ViewState["vwUpdatedExpenseTypeMappingList"];
            }
            set
            {
                ViewState["vwUpdatedExpenseTypeMappingList"] = value;
            }
        }

        ArrayList vwOfficeExpenseTypeList
        {
            get
            {
                return (ArrayList)ViewState["vwOfficeExpenseTypeList"];
            }
            set
            {
                ViewState["vwOfficeExpenseTypeList"] = value;
            }
        }

        ArrayList vwSimilarVendorList
        {
            get { return (ArrayList)ViewState["vwSimilarVendorList"]; }
            set { ViewState["vwSimilarVendorList"] = value; }
        }

        AttachmentInfoDef vwNTVendorDocumentAttachment
        {
            get
            {
                return (AttachmentInfoDef)ViewState["vwNTVendorDocumentAttachment"];
            }
            set
            {
                ViewState["vwNTVendorDocumentAttachment"] = value;
            }
        }

        bool isEpicorSupplierIdCreator;

        protected void Page_Load(object sender, EventArgs e)
        {
            isEpicorSupplierIdCreator = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntVendor.Id, com.next.common.domain.module.ISAMModule.ntVendor.EpicorSupplierIdCreation);

            if (!Page.IsPostBack)
            {
                int ntVendorId = -1;

                if (Request.Params["NTVendorId"] != null)
                {
                    ntVendorId = Convert.ToInt32(Request.Params["NTVendorId"]);

                    vwNTVendor = NonTradeManager.Instance.getNTVendorByKey(ntVendorId);
                    vwExpenseTypeMappingList = NonTradeManager.Instance.getNTVendorExpenseTypeMappingByNTVendorId(ntVendorId);
                }
                else
                    vwNTVendor = new NTVendorDef();

                bindData();
            }

        }

        private bool isEpicorSupplierIdApprover()
        {
            return (isEpicorSupplierIdCreator || btn_Approve.Visible);
        }

        void bindData()
        {
            bool isAccountUser = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.Id, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.ViewForAccountsUser);
            //bool isEpicorSupplierIdCreator = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntVendor.Id, com.next.common.domain.module.ISAMModule.ntVendor.EpicorSupplierIdCreate);

            string officeId = vwNTVendor.Office == null ? this.LogonUserHomeOffice.OfficeId.ToString() : vwNTVendor.Office.OfficeId.ToString();

            ddl_Office.bindList(WebUtil.getNTOfficeList(this.LogonUserId), "OfficeCode", "OfficeId", officeId);
            vwOfficeExpenseTypeList = WebUtil.getNTExpenseTypeListByOfficeId(ddl_Office.selectedValueToInt);
            ArrayList expenseTypeList = vwOfficeExpenseTypeList;
            if (isAccountUser)
            {
                expenseTypeList.Sort(new NTExpenseTypeRef.ExpenseTypeComparer(NTExpenseTypeRef.ExpenseTypeComparer.CompareType.SUNAccountCode));
            }
            ddl_Country.bindList(CommonUtil.getCountryList(), "Name", "CountryId", vwNTVendor.Country == null ? "" : vwNTVendor.Country.CountryId.ToString(), "-- Please select --", "-1");
            ddl_DefaultCurrency.bindList(WebUtil.getEffectiveCurrencyList(), "CurrencyCode", "CurrencyId", vwNTVendor.Currency == null ? "" : vwNTVendor.Currency.CurrencyId.ToString(), "--", "-1");
            ddl_DefaultCompany.bindList(CommonUtil.getCompanyList(int.Parse(officeId)), "CompanyName", "CompanyId", (vwNTVendor.CompanyId == int.MinValue ? "-1" : vwNTVendor.CompanyId.ToString()), "--- N/A ---", "-1");
            ddl_DefaultPaymentTerm.bindList(NTPaymentMethodRef.getCollectionValues(ddl_Office.selectedValueToInt), "Name", "Id", vwNTVendor.PaymentMethod == null ? "" : vwNTVendor.PaymentMethod.Id.ToString(), "--", "-1");
            ddl_ConsumptionUnit.bindList(ConsumptionType.getCollectionValues(), "Name", "Id", vwNTVendor.ConsumptionUnitId == -1 ? "-1" : vwNTVendor.ConsumptionUnitId.ToString(), "--- N/A ---", "-1");
            ddl_UtilityProviderType.bindList(UtilityProviderType.getCollectionValues(), "Name", "Id", vwNTVendor.UtilityProviderTypeId == -1 ? "-1" : vwNTVendor.UtilityProviderTypeId.ToString(), "--- N/A ---", "-1");
            ddl_UtilityProviderType.Attributes.Add("onchange", "updateConsumptionUnit();");

            this.lnkFile.Visible = false;

            if (vwNTVendor.NTVendorId != 0)
            {
                txt_vendorId.Text = vwNTVendor.NTVendorId.ToString();
                txt_VendorName.Text = vwNTVendor.VendorName;
                txt_OtherName.Text = vwNTVendor.OtherName;
                txt_Address.Text = vwNTVendor.Address;
                txt_phoneNo.Text = vwNTVendor.Telephone;
                txt_FaxNo.Text = vwNTVendor.Fax;
                txt_Email.Text = vwNTVendor.Email;
                txt_EAdviceEmail.Text = vwNTVendor.EAdviceEmail;
                txt_ContactPerson.Text = vwNTVendor.ContactPerson;
                txt_BankAccountNo.Text = vwNTVendor.BankAccountNo;
                txt_PaymentTermDays.Text = vwNTVendor.PaymentTermDays.ToString();
                txt_SUNAccCode.Text = vwNTVendor.SUNAccountCode;
                txt_Remark.Text = vwNTVendor.Remark;
                ckb_InvoiceNoReq.Checked = vwNTVendor.IsInvoiceNoRequired == 1 ? true : false;
                ckb_CustomerNoReq.Checked = vwNTVendor.IsCustomerNoRequired == 1 ? true : false;
                txt_Status.Text = vwNTVendor.WorkflowStatus.Name;
                if (vwNTVendor.AmendmentDetail != string.Empty)
                {
                    txt_AmendRequest.Text = vwNTVendor.AmendmentDetail;
                }

                ArrayList queryStructs = new ArrayList();

                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Non Trade Vendor Document"));
                queryStructs.Add(new QueryStructDef("Vendor Id", vwNTVendor.NTVendorId.ToString()));

                ArrayList qList = DMSUtil.queryDocument(queryStructs);
                ArrayList attachmentInfoList = new ArrayList();

                this.updVendorDoc = new FileUpload();

                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    foreach (AttachmentInfoDef attDef in docInfoDef.AttachmentInfos)
                    {
                        FieldInfoDef fiDef = (FieldInfoDef)docInfoDef.FieldInfos[1];
                        attDef.Description = fiDef.Content + "|" + docInfoDef.DocumentID.ToString();
                        this.vwNTVendorDocumentAttachment = attDef;
                        this.lnkFile.Visible = true;
                        this.lnkFile.Text = attDef.FileName;
                        break;
                    }
                }

                gv_ActionHistory.DataSource = NonTradeManager.Instance.getNTActionHistoryList(-1, vwNTVendor.NTVendorId);
                gv_ActionHistory.DataBind();
            }
            else
            {
                vwExpenseTypeMappingList = new ArrayList();
                vwUpdatedExpenseTypeMappingList = new ArrayList();

                foreach (NTExpenseTypeRef rf in expenseTypeList)
                    if (rf.ExpenseType == "ACCRUAL" || rf.ExpenseType == "PREPAYMENTS" || rf.SUNAccountCode == "1311001" || rf.SUNAccountCode == "1412101")
                    {
                        NTVendorExpenseTypeMappingDef mappingDef = new NTVendorExpenseTypeMappingDef(0, rf);
                        vwExpenseTypeMappingList.Add(mappingDef);
                        vwUpdatedExpenseTypeMappingList.Add(mappingDef);
                    }
            }
            ddl_DefaultExpenseType.bindList(expenseTypeList, (isAccountUser ? "AccountCodeDescription" : "Description"), "ExpenseTypeId", vwNTVendor.ExpenseType == null ? "-1" : vwNTVendor.ExpenseType.ExpenseTypeId.ToString(), "-- Please select --", GeneralCriteria.ALL.ToString());
            ddl_ExpenseType.bindList(expenseTypeList, (isAccountUser ? "AccountCodeDescription" : "Description"), "ExpenseTypeId", "", "-- Please select --", GeneralCriteria.ALL.ToString());
            this.btn_CancelVendor.Visible = false;

            if (vwNTVendor.WorkflowStatus.Id == NTVendorWFS.DRAFT.Id || vwNTVendor.WorkflowStatus.Id == NTVendorWFS.REJECTED.Id)
            {
                btn_Submit.Visible = true;
                btn_Approve.Visible = false;
                btn_Reject.Visible = false;
                if (vwNTVendor.NTVendorId != 0 && NonTradeManager.Instance.getNTInvoiceListByVendorId(vwNTVendor.NTVendorId).Count == 0)
                    this.btn_CancelVendor.Visible = true;
            }
            else if (vwNTVendor.WorkflowStatus.Id == NTVendorWFS.APPROVED.Id)
            {
                btn_Save.Visible = false;
                btn_Submit.Visible = false;
                tb_AmendRequest.Visible = true;
            }
            else if (vwNTVendor.WorkflowStatus.Id == NTVendorWFS.CANCELLED.Id)
            {
                btn_Save.Visible = false;
                btn_Cancel.Visible = false;
            }

            /*
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntVendor.Id, com.next.common.domain.module.ISAMModule.ntVendor.AccountEdit))
            */
            ArrayList accountEditOffice = NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.VENDOR_ACCOUNT_EDIT_USER.Id, GeneralCriteria.ALL);
            if (accountEditOffice.Contains(ddl_Office.selectedValueToInt))
            {
                txt_SUNAccCode.ReadOnly = false;
                if (vwNTVendor.WorkflowStatus.Id == NTVendorWFS.PENDING.Id)
                    btn_Submit.Visible = false;
                else if (vwNTVendor.WorkflowStatus.Id == NTVendorWFS.APPROVED.Id)
                    btn_Submit.Visible = true;
            }
            else if (vwNTVendor.WorkflowStatus.Id == NTVendorWFS.PENDING.Id)
            {
                btn_Submit.Visible = false;
            }

            if (vwNTVendor.WorkflowStatus.Id == NTVendorWFS.PENDING.Id)
            {
                ArrayList approvalOfficeIdList = NonTradeManager.Instance.getNTUserOfficeIdList(this.LogonUserId, NTRoleType.VENDOR_APPROVER.Id, GeneralCriteria.ALL);
                bool enable = approvalOfficeIdList.Contains(ddl_Office.selectedValueToInt);
                btn_Approve.Visible = enable;
                btn_Reject.Visible = enable;
            }
            else
            {
                btn_Approve.Visible = false;
                btn_Reject.Visible = false;
            }

            rep_ExpenseTypeMapping.DataSource = vwExpenseTypeMappingList;
            rep_ExpenseTypeMapping.DataBind();

            // Epicor Supplier ID section
            string vType = getVendorType(vwNTVendor.VendorTypeId);
            string displayMode = (isEpicorSupplierIdApprover() ? "block" : "none");
            tr_VendorType.Style.Add(HtmlTextWriterStyle.Display, displayMode);
            tr_SimilarSupplier.Style.Add(HtmlTextWriterStyle.Display, displayMode);
            //tr_SimilarSupplier.Disabled = (vType == "N" && !string.IsNullOrEmpty(vwNTVendor.EPVendorCode));
            txt_EpicorSupplierId.Text = vwNTVendor.EPVendorCode.Trim();

            //string vType=getVendorType(vwNTVendor.EPVendorCode);
            rad_NonTradeVendor.Checked = (vType == "N");
            rad_BulkVendor.Checked = (vType == "T");
            txt_EpicorSupplierId.ReadOnly = (vType == "N" || !isEpicorSupplierIdApprover());
            if (!string.IsNullOrEmpty(txt_VendorName.Text)) 
                refreshSimilarVendorList();
            //generateSimilarVendorList(txt_VendorName.Text, vType, txt_EpicorSupplierId.Text);
        }

        protected void btn_AddExpenseType_Click(Object sender, EventArgs e)
        {
            NTExpenseTypeRef expenseType = WebUtil.getNTExpenseTypeByKey(int.Parse(ddl_ExpenseType.SelectedValue));
            AddExpenseTypeMapping(expenseType);
        }

        protected void DefaultExpenseType_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (ddl_DefaultExpenseType.SelectedValue != "-1")
            {
                NTExpenseTypeRef expenseType = WebUtil.getNTExpenseTypeByKey(int.Parse(ddl_DefaultExpenseType.SelectedValue));
                AddExpenseTypeMapping(expenseType);
            }
        }

        protected void Office_SelectedIndexChanged(Object sender, EventArgs e)
        {
            vwOfficeExpenseTypeList = WebUtil.getNTExpenseTypeListByOfficeId(ddl_Office.selectedValueToInt);
            RegenerateExpenseTypeMappingList(ddl_Office.selectedValueToInt);
            rep_ExpenseTypeMapping.DataSource = vwExpenseTypeMappingList;
            rep_ExpenseTypeMapping.DataBind();
            ddl_DefaultCompany.bindList(CommonUtil.getCompanyList(ddl_Office.selectedValueToInt), "CompanyName", "CompanyId", "-1", "--- N/A ---", "-1");
        }

        private void RegenerateExpenseTypeMappingList(int officeId)
        {
            NTExpenseTypeRef defaultExpenseType = null;
            ArrayList initialOfficeAccountCodeList = new ArrayList();
            ArrayList initialExpenseTypeMappingList = new ArrayList();
            ArrayList selectedAccountCodeList = new ArrayList();
            ArrayList selectedExpenseTypeMappingList = new ArrayList();

            ArrayList expenseTypeList = vwOfficeExpenseTypeList;  
            foreach(NTExpenseTypeRef rf in expenseTypeList)
                if (rf.Description == ddl_DefaultExpenseType.selectedText)
                {
                    defaultExpenseType = rf;
                    break;
                }
            selectedExpenseTypeMappingList = vwExpenseTypeMappingList;
            foreach (NTVendorExpenseTypeMappingDef mapping in selectedExpenseTypeMappingList)
                selectedAccountCodeList.Add(mapping.ExpenseType.SUNAccountCode);

            vwExpenseTypeMappingList = new ArrayList();
            if (vwUpdatedExpenseTypeMappingList == null)
                vwUpdatedExpenseTypeMappingList = new ArrayList();
            else
                vwUpdatedExpenseTypeMappingList.Clear();

            initialExpenseTypeMappingList = NonTradeManager.Instance.getNTVendorExpenseTypeMappingByNTVendorId(vwNTVendor.NTVendorId) ;
            foreach (NTVendorExpenseTypeMappingDef initialMapping in initialExpenseTypeMappingList)
            {
                initialOfficeAccountCodeList.Add(OfficeId.getName(initialMapping.ExpenseType.OfficeId) + "-" + initialMapping.ExpenseType.SUNAccountCode);
                if (!selectedAccountCodeList.Contains(initialMapping.ExpenseType.SUNAccountCode) || initialMapping.ExpenseType.OfficeId != officeId)
                {
                    initialMapping.Status = 0;
                    vwUpdatedExpenseTypeMappingList.Add(initialMapping);
                }
            }
            foreach (NTVendorExpenseTypeMappingDef mapping in selectedExpenseTypeMappingList)
                foreach (NTExpenseTypeRef rf in expenseTypeList)
                    if (rf.ExpenseTypeId == mapping.ExpenseType.ExpenseTypeId)
                    {
                        NTVendorExpenseTypeMappingDef newMapping;
                        if ((newMapping = new NTVendorExpenseTypeMappingDef(vwNTVendor.NTVendorId, rf)) != null)
                        {
                            vwExpenseTypeMappingList.Add(newMapping);
                            if (!initialOfficeAccountCodeList.Contains(OfficeId.getName(mapping.ExpenseType.OfficeId) + "-" + mapping.ExpenseType.SUNAccountCode))
                                vwUpdatedExpenseTypeMappingList.Add(newMapping);
                        }
                        break;
                    }

            bool isAccountUser = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.Id, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.ViewForAccountsUser);
            if (isAccountUser)
                    expenseTypeList.Sort(new NTExpenseTypeRef.ExpenseTypeComparer(NTExpenseTypeRef.ExpenseTypeComparer.CompareType.SUNAccountCode));
            ddl_DefaultExpenseType.bindList(expenseTypeList, (isAccountUser ? "AccountCodeDescription" : "Description"), "ExpenseTypeId",  (defaultExpenseType==null?"-1":defaultExpenseType.ExpenseTypeId.ToString()) , "-- Please select --", GeneralCriteria.ALL.ToString());
            ddl_ExpenseType.bindList(expenseTypeList, (isAccountUser ? "AccountCodeDescription" : "Description"), "ExpenseTypeId", "", "-- Please select --", GeneralCriteria.ALL.ToString());
        }
        
        private void AddExpenseTypeMapping(NTExpenseTypeRef expenseType)
        {
            NTVendorExpenseTypeMappingDef vendorExpTypeMapping = new NTVendorExpenseTypeMappingDef(vwNTVendor.NTVendorId, expenseType);

            if (vwExpenseTypeMappingList == null)
                vwExpenseTypeMappingList = new ArrayList();
            else
            {
                foreach (NTVendorExpenseTypeMappingDef mapping in vwExpenseTypeMappingList)
                {
                    if (mapping.ExpenseType.ExpenseTypeId == vendorExpTypeMapping.ExpenseType.ExpenseTypeId)
                        return;
                }
            }

            vwExpenseTypeMappingList.Add(vendorExpTypeMapping);

            if (vwUpdatedExpenseTypeMappingList == null)
                vwUpdatedExpenseTypeMappingList = new ArrayList();
            vwUpdatedExpenseTypeMappingList.Add(vendorExpTypeMapping);

            rep_ExpenseTypeMapping.DataSource = vwExpenseTypeMappingList;
            rep_ExpenseTypeMapping.DataBind();

        }

        protected void ExpenseTypeMapping_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                NTVendorExpenseTypeMappingDef mappingDef = (NTVendorExpenseTypeMappingDef)vwExpenseTypeMappingList[Convert.ToInt32(e.CommandArgument)];
                mappingDef.Status = 0;
                vwExpenseTypeMappingList.Remove(mappingDef);
                if (vwUpdatedExpenseTypeMappingList == null)
                    vwUpdatedExpenseTypeMappingList = new ArrayList();
                vwUpdatedExpenseTypeMappingList.Add(mappingDef);

                rep_ExpenseTypeMapping.DataSource = vwExpenseTypeMappingList;
                rep_ExpenseTypeMapping.DataBind();

                if (ddl_DefaultExpenseType.selectedValueToInt == mappingDef.ExpenseType.ExpenseTypeId)
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "document.all." + ddl_DefaultExpenseType.ClientID + ".selectedIndex=0;", true);
            }
        }

        protected void ExpenseTypeMapping_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (!btn_AddExpenseType.Enabled)
                {
                    ((ImageButton)e.Item.FindControl("btn_remove")).Visible = false;
                }
            }
        }

        protected void Save_OnClick(Object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (int.Parse(ddl_Office.SelectedValue) == OfficeId.PK.Id)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('New vendor creation under PK office is not allowed');", true);
                    return;
                }

                vwNTVendor.VendorName = txt_VendorName.Text.Trim().ToUpper();
                vwNTVendor.OtherName = txt_OtherName.Text.Trim();
                vwNTVendor.ContactPerson = txt_ContactPerson.Text.Trim();
                vwNTVendor.Email = txt_Email.Text.Trim();
                vwNTVendor.EAdviceEmail = txt_EAdviceEmail.Text.Trim();
                vwNTVendor.BankAccountNo = txt_BankAccountNo.Text.Trim();
                vwNTVendor.Office = CommonUtil.getOfficeRefByKey(int.Parse(ddl_Office.SelectedValue));
                vwNTVendor.Address = txt_Address.Text.Trim();
                vwNTVendor.Country = CommonUtil.getCountryByKey(int.Parse(ddl_Country.SelectedValue));
                vwNTVendor.Telephone = txt_phoneNo.Text.Trim();
                vwNTVendor.Fax = txt_FaxNo.Text.Trim();
                vwNTVendor.IsInvoiceNoRequired = ckb_InvoiceNoReq.Checked ? 1 : 0;
                vwNTVendor.IsCustomerNoRequired = ckb_CustomerNoReq.Checked ? 1 : 0;
                vwNTVendor.ExpenseType = ddl_DefaultExpenseType.SelectedValue == "-1" ? null : WebUtil.getNTExpenseTypeByKey(int.Parse(ddl_DefaultExpenseType.SelectedValue));
                vwNTVendor.Currency = CommonUtil.getCurrencyByKey(int.Parse(ddl_DefaultCurrency.SelectedValue));
                vwNTVendor.CompanyId = int.Parse(ddl_DefaultCompany.SelectedValue);
                vwNTVendor.ConsumptionUnitId = int.Parse(ddl_ConsumptionUnit.SelectedValue);
                vwNTVendor.UtilityProviderTypeId = int.Parse(ddl_UtilityProviderType.SelectedValue);
                vwNTVendor.PaymentMethod = NTPaymentMethodRef.getType(int.Parse(ddl_DefaultPaymentTerm.SelectedValue));
                vwNTVendor.PaymentTermDays = txt_PaymentTermDays.Text.Trim() == string.Empty ? 0 : int.Parse(txt_PaymentTermDays.Text);
                vwNTVendor.SUNAccountCode = txt_SUNAccCode.Text.Trim();
                vwNTVendor.Remark = txt_Remark.Text.Trim();
                //vwNTVendor.EPVendorCode = txt_EpicorSupplierId.Text.Trim();
                /*
                if (isEpicorSupplierIdApprover() && string.IsNullOrEmpty(vwNTVendor.EPVendorCode) && rad_NonTradeVendor.Checked)
                {
                    if (ddl_SimilarVendor.SelectedValue == "0")
                        vwNTVendor.EPVendorCode = NonTradeManager.Instance.getNewEpicorVendorCode(getVendorNamePrefix(txt_VendorName.Text), "N");
                    else
                    {
                        NTVendorDef vendor = NonTradeManager.Instance.getNTVendorByKey(ddl_SimilarVendor.selectedValueToInt);
                        vwNTVendor.EPVendorCode = vendor.EPVendorCode;
                    }
                }
                else
                    if (!string.IsNullOrEmpty(txt_EpicorSupplierId.Text))
                        vwNTVendor.EPVendorCode = txt_EpicorSupplierId.Text.Trim();
                */
                if (rad_NonTradeVendor.Checked)
                    vwNTVendor.VendorTypeId = 20;
                else if (rad_BulkVendor.Checked)
                    vwNTVendor.VendorTypeId = 10;
                else
                    vwNTVendor.VendorTypeId = 0;

                RegenerateExpenseTypeMappingList(vwNTVendor.Office.OfficeId);

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateNTVendor);

                Context.Items.Add(AccountCommander.Param.ntVendor, vwNTVendor);
                Context.Items.Add(AccountCommander.Param.expenseTypeList, vwUpdatedExpenseTypeMappingList);                

                forwardToScreen(null);

                this.uploadDoc();
                vwUpdatedExpenseTypeMappingList = null;

                bindData();
            }
        }

        protected void Review_OnClick(Object sender, EventArgs e)
        {
            vwNTVendor.ReviewedOn = DateTime.Now;
            vwNTVendor.ReviewedBy = CommonUtil.getUserByKey(this.LogonUserId);

            Save_OnClick(sender, e);

        }

        protected void Approve_OnClick(Object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int i = 0;

                if (isEpicorSupplierIdApprover())
                {
                    if (rad_BulkVendor.Checked)
                        if (string.IsNullOrEmpty(txt_EpicorSupplierId.Text))
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('Please provide Epicor Supplier ID .');", true);
                            return;
                        }
                        else
                        {
                            string vendorCode = txt_EpicorSupplierId.Text;
                            if (vendorCode.Length == 8)
                            {
                                if (!(char.IsLetter(vendorCode[0]) && char.IsLetter(vendorCode[1]) && char.IsLetter(vendorCode[7]) && int.TryParse(vendorCode.Substring(2, 5), out i)))
                                {
                                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('Invalid Epicor Supplier ID.');", true);
                                    return;
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('Epicor Supplier ID must be 8 digits long.');", true);
                                return;
                            }
                        }
                    else
                        if (rad_NonTradeVendor.Checked)
                        {
                            if (ddl_SimilarVendor.SelectedValue == "-1" && string.IsNullOrEmpty(txt_EpicorSupplierId.Text))
                            {
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('Please select a vendor from the similar vendor list.');", true);
                                return;
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('Please select a vendor type.');", true);
                            return;
                        }

                    if (string.IsNullOrEmpty(txt_EpicorSupplierId.Text) && rad_NonTradeVendor.Checked)
                    {
                        if (ddl_SimilarVendor.SelectedValue == "0")
                            //vwNTVendor.EPVendorCode = NonTradeManager.Instance.getNewEpicorVendorCode(getVendorNamePrefix(txt_VendorName.Text), "N");
                            txt_EpicorSupplierId.Text = NonTradeManager.Instance.getNewEpicorVendorCode(getVendorNamePrefix(txt_VendorName.Text), "N");
                        else
                        {
                            NTVendorDef vendor = NonTradeManager.Instance.getNTVendorByKey(ddl_SimilarVendor.selectedValueToInt);
                            //vwNTVendor.EPVendorCode = vendor.EPVendorCode;
                            txt_EpicorSupplierId.Text = vendor.EPVendorCode;
                        }
                    }
                    vwNTVendor.EPVendorCode = txt_EpicorSupplierId.Text.Trim();
                }
                vwNTVendor.WorkflowStatus = NTVendorWFS.APPROVED;
                vwNTVendor.AmendmentDetail = string.Empty;
                txt_AmendRequest.Text = string.Empty;

                Save_OnClick(sender, e);
            }
        }

        protected void Reject_OnClick(Object sender, EventArgs e)
        {
            vwNTVendor.WorkflowStatus = NTVendorWFS.REJECTED;
            Save_OnClick(sender, e);
        }

        protected void Submit_OnClick(Object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (int.Parse(ddl_Office.SelectedValue) == OfficeId.PK.Id)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('New vendor creation under PK office is not allowed');", true);
                    return;
                }

                if ((vwNTVendor.NTVendorId == 0 || ((vwNTVendor.CreatedOn == DateTime.MinValue || vwNTVendor.CreatedOn > ConvertUtility.toDateTime("29/07/2013")) && vwNTVendor.NTVendorId > 90000)) && vwNTVendorDocumentAttachment == null && !this.updVendorDoc.HasFile)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('Please attach vendor document');", true);
                    return;
                }
                
                if (int.Parse(this.ddl_DefaultExpenseType.SelectedValue) == -1)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('Please select default expense type');", true);
                    return;
                }
                
                vwNTVendor.WorkflowStatus = NTVendorWFS.PENDING;
                Save_OnClick(sender, e);
            }
        }

        protected void Cancel_OnClick(Object sender, EventArgs e)
        {
            if (vwNTVendor.NTVendorId == 0)
            {
                vwNTVendor = new NTVendorDef();
                vwExpenseTypeMappingList = new ArrayList();
                vwUpdatedExpenseTypeMappingList = new ArrayList();
            }
            else
            {
                vwNTVendor = NonTradeManager.Instance.getNTVendorByKey(vwNTVendor.NTVendorId);
                vwExpenseTypeMappingList = NonTradeManager.Instance.getNTVendorExpenseTypeMappingByNTVendorId(vwNTVendor.NTVendorId);
                vwUpdatedExpenseTypeMappingList = new ArrayList();
            }

            bindData();
        }


        protected void ActionHistoryDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NTActionHistoryDef historyDef = (NTActionHistoryDef) e.Row.DataItem;

                ((Label)e.Row.FindControl("lbl_ActionBy")).Text = CommonUtil.getUserByKey(historyDef.ActionBy).DisplayName;
            }
        }

        protected void val_Vendor_Validate(object sender, ServerValidateEventArgs e)
        {
            val_Vendor.ErrorMessage = "";

            if (!ckb_CustomerNoReq.Checked && !ckb_InvoiceNoReq.Checked)
            {
                Page.Validators.Add(new ValidationError("Please select Invoice No. and/or Customer No."));
                /*
                e.IsValid = false;
                val_Vendor.ErrorMessage = "Please select Invoice No. and/or Customer No.";
                */
            }

            if (ddl_DefaultExpenseType.SelectedValue == "-1")
            {
                Page.Validators.Add(new ValidationError("Please select Default Expense Type."));
                /*
                e.IsValid = false;
                val_Vendor.ErrorMessage = "Please select Default Expense Type.";
                */
            }
            else
            {
                string sunCode = NonTradeManager.Instance.getNTExpenseTypeByKey(int.Parse(ddl_DefaultExpenseType.SelectedValue)).SUNAccountCode;

                if (sunCode == "4102301" || sunCode == "4102303" || sunCode == "4104310" || sunCode == "4104312" || sunCode == "4104336")
                {
                    if (this.ddl_UtilityProviderType.SelectedValue == "-1")
                    {
                        Page.Validators.Add(new ValidationError("Please select Service Provide Type"));
                        /*
                        e.IsValid = false;
                        val_Vendor.ErrorMessage = "Please select Service Provide Type";
                        */
                    }
                    else if (int.Parse(this.ddl_UtilityProviderType.SelectedValue) != UtilityProviderType.PARKING.Id && this.ddl_ConsumptionUnit.SelectedValue == "-1")
                    {
                        Page.Validators.Add(new ValidationError("Please select Consumption Unit"));
                        /*
                        e.IsValid = false;
                        val_Vendor.ErrorMessage = "Please select Consumption Unit";
                        */
                    }
                }

            }

            if (ddl_DefaultCurrency.SelectedValue == "-1")
            {
                Page.Validators.Add(new ValidationError("Please select Default Currency."));
                /*
                e.IsValid = false;
                val_Vendor.ErrorMessage = "Please select Default Currency.";
                */
            }

            if (ddl_Country.SelectedValue == "-1")
            {
                Page.Validators.Add(new ValidationError("Please select Country."));
                /*
                e.IsValid = false;
                val_Vendor.ErrorMessage = "Please select Country.";
                */
            }
            if (ddl_DefaultPaymentTerm.SelectedValue == "-1")
            {
                Page.Validators.Add(new ValidationError("Please select Default Payment Method."));
                /*
                e.IsValid = false;
                val_Vendor.ErrorMessage = "Please select Default Payment Method.";
                */
            }

            int days = 0;
            if (!int.TryParse(txt_PaymentTermDays.Text, out days))
            {
                Page.Validators.Add(new ValidationError("Invalid Payment Term Days."));
                /*
                e.IsValid = false;
                val_Vendor.ErrorMessage = "Invalid Payment Term Days.";
                */
            }
            else if (days < 0)
            {
                Page.Validators.Add(new ValidationError("Payment Term Days should be larger than zero."));
                /*
                e.IsValid = false;
                val_Vendor.ErrorMessage = "Payment Term Days should be larger than zero.";
                */
            }

            if (this.ddl_UtilityProviderType.selectedValueToInt == UtilityProviderType.CAR_FUEL.Id && this.ddl_ConsumptionUnit.selectedValueToInt != ConsumptionType.MILE.Id && this.ddl_ConsumptionUnit.selectedValueToInt != ConsumptionType.KILOMETER.Id && this.ddl_ConsumptionUnit.selectedValueToInt != ConsumptionType.LITRE.Id)
            {
                Page.Validators.Add(new ValidationError("Invalid consumption unit for car fuel provider type."));
            }

        }

        protected void btn_SendRequest_Click(Object sender, EventArgs e)
        {
            if (txt_AmendRequest.Text.Trim() == string.Empty)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('Please provide the amendment detail.');", true);
                return;
            }

            string amendmentDetail = txt_AmendRequest.Text.Trim();

            NonTradeManager.Instance.sendNTVendorAmendmentRequest(vwNTVendor, amendmentDetail, this.LogonUserId);
            
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NTVendor", "alert('Request sent out successfully.');", true);
        }

        protected void lnkFile_Click(object sender, EventArgs e)
        {

            AttachmentInfoDef def = this.vwNTVendorDocumentAttachment;

            Context.Items.Clear();
            string[] s = def.Description.Split('|');
            Context.Items.Add("docId", s[1]);
            Context.Items.Add("attId", def.AttachmentID.ToString());
            Context.Items.Add("majorId", def.MajorVerion.ToString());
            Context.Items.Add("minorId", def.MinorVerion.ToString());
            Context.Items.Add("buildId", def.Build.ToString());
            forwardToScreen("dms.Attachment");
        }

        private void reviseDocument()
        {
            string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputFolder = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + tsFolderName + "\\";
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);

            ArrayList queryStructs = new ArrayList();

            DocumentInfoDef docInfoDef = DMSUtil.queryDocumentByID(int.Parse(this.vwNTVendorDocumentAttachment.Description.Split('|')[1]));

            foreach (FieldInfoDef fiDef in docInfoDef.FieldInfos)
                queryStructs.Add(new QueryStructDef(fiDef.FieldName, fiDef.Content));

            ArrayList attachmentList = new ArrayList();

            if (docInfoDef != null)
            {
                DMSUtil.DeleteSingleAttachment(int.Parse(this.vwNTVendorDocumentAttachment.Description.Split('|')[1]), queryStructs, this.vwNTVendorDocumentAttachment.FileName);

                string filename = outputFolder + Path.GetFileNameWithoutExtension(this.updVendorDoc.FileName) + Path.GetExtension(this.updVendorDoc.FileName).ToLower();
                this.updVendorDoc.SaveAs(filename);
                attachmentList.Add(filename);
                string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docInfoDef.DocumentID, queryStructs, attachmentList);
                if (strReturn.Trim() == string.Empty)
                {
                    NTActionHistoryDef logDef = new NTActionHistoryDef(-1, this.vwNTVendor.NTVendorId, "Revise document : " + Path.GetFileNameWithoutExtension(this.updVendorDoc.FileName) + Path.GetExtension(this.updVendorDoc.FileName).ToLower(), this.LogonUserId);
                    NonTradeManager.Instance.updateNTActionHistory(logDef);
                }

            }
            FileUtility.clearFolder(outputFolder, false);
        }

        private void uploadDoc()
        {
            if (this.updVendorDoc.HasFile)
            {
                if (this.vwNTVendorDocumentAttachment != null)
                    this.reviseDocument();
                else
                {
                    string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string outputFolder = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + tsFolderName + "\\";
                    if (!System.IO.Directory.Exists(outputFolder))
                        System.IO.Directory.CreateDirectory(outputFolder);

                    FileUpload fu = this.updVendorDoc;

                    string filename = string.Empty;
                    if (fu.HasFile)
                    {
                        filename = Path.GetFileNameWithoutExtension(fu.FileName);
                        ArrayList queryStructs = new ArrayList();
                        queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Non Trade Vendor Document"));
                        queryStructs.Add(new QueryStructDef("Vendor Id", this.vwNTVendor.NTVendorId.ToString()));
                        queryStructs.Add(new QueryStructDef("Vendor Name", this.txt_VendorName.Text.Trim()));

                        ArrayList qList = DMSUtil.queryDocument(queryStructs);
                        long docId = 0;
                        ArrayList attachmentList = new ArrayList();

                        foreach (DocumentInfoDef docInfoDef in qList)
                        {
                            docId = docInfoDef.DocumentID;
                        }

                        if (docId > 0)
                        {
                            string path = outputFolder + filename + Path.GetExtension(fu.FileName).ToLower();
                            fu.SaveAs(path);
                            attachmentList.Add(path);
                            string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                            if (strReturn.Trim() == string.Empty)
                            {
                                NTActionHistoryDef logDef = new NTActionHistoryDef(-1, this.vwNTVendor.NTVendorId, "Revise document : " + filename + Path.GetExtension(fu.FileName).ToLower(), this.LogonUserId);
                                NonTradeManager.Instance.updateNTActionHistory(logDef);
                            }
                        }
                        else
                        {
                            string path = outputFolder + filename + Path.GetExtension(fu.FileName).ToLower();
                            fu.SaveAs(path);
                            attachmentList.Add(path);
                            string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Account\\Non Trade Expense Vendor\\", this.txt_VendorName.Text.Trim(), "Non Trade Vendor Document", queryStructs, attachmentList);
                            if (msg.Trim() == string.Empty)
                            {
                                NTActionHistoryDef logDef = new NTActionHistoryDef(-1, this.vwNTVendor.NTVendorId, "Upload document : " + filename + Path.GetExtension(fu.FileName).ToLower(), this.LogonUserId);
                                NonTradeManager.Instance.updateNTActionHistory(logDef);
                            }
                        }
                    }
                    FileUtility.clearFolder(outputFolder, false);
                }
            }
        }

        protected void btn_CancelVendor_Click(object sender, EventArgs e)
        {
            vwNTVendor.WorkflowStatus = NTVendorWFS.CANCELLED;

            Save_OnClick(sender, e);
        }

        protected void txt_VendorName_OnChanged(object sender, EventArgs e)
        {
            if (isEpicorSupplierIdApprover())
                refreshSimilarVendorList();
        }

        protected void SimilarVendor_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (ddl_SimilarVendor.selectedValueToInt > 0)
            {
                string vendorType = getVendorType();
                string EPVendorCode = string.Empty;
                if (vendorType == "N")
                {
                    NTVendorDef vendor = NonTradeManager.Instance.getNTVendorByKey(ddl_SimilarVendor.selectedValueToInt);
                    EPVendorCode = (vendor == null ? string.Empty : vendor.EPVendorCode);
                }
                else if (vendorType == "T")
                {
                    VendorRef vendor = IndustryUtil.getVendorByKey(ddl_SimilarVendor.selectedValueToInt);
                    EPVendorCode = (vendor == null ? string.Empty : vendor.EpicorSupplierId);
                }
                //vwNTVendor.EPVendorCode = EPVendorCode;
                txt_EpicorSupplierId.Text = EPVendorCode;
            }
            else
            {
                //vwNTVendor.EPVendorCode = string.Empty;
                txt_EpicorSupplierId.Text = string.Empty;
            }

        }
        
        protected void rad_NonTradeVendor_OnCheckedChanged(Object sender, EventArgs e)
        {
            refreshSimilarVendorList();
            if (rad_NonTradeVendor.Checked)
            {
                txt_EpicorSupplierId.Text = (string.IsNullOrEmpty(vwNTVendor.EPVendorCode) ? string.Empty : vwNTVendor.EPVendorCode);
                txt_EpicorSupplierId.ReadOnly = true;
                //tr_SimilarSupplier.Disabled = !string.IsNullOrEmpty(vwNTVendor.EPVendorCode);
            }
        }

        protected void rad_BulkVendor_OnCheckedChanged(Object sender, EventArgs e)
        {
            refreshSimilarVendorList();
            if (rad_BulkVendor.Checked)
            {
                txt_EpicorSupplierId.Text = (string.IsNullOrEmpty(vwNTVendor.EPVendorCode) ? string.Empty : vwNTVendor.EPVendorCode);
                txt_EpicorSupplierId.ReadOnly = false;
                //tr_SimilarSupplier.Disabled = false;
                
            }
        }

        private string getVendorType(string vendorCode)
        {
            //int seq;
            string vendorType = string.Empty;
            if (!string.IsNullOrEmpty(vendorCode))
                if (vendorCode.Length == 8)
                    //if (char.IsLetter(vendorCode, 0) && char.IsLetter(vendorCode, 1) && char.IsLetter(vendorCode, 7))
                    //    if (int.TryParse(vendorCode.Substring(2, 5), out seq))
                            vendorType = vendorCode.Substring(7, 1);
            return (vendorType == "N" || vendorType == "T" ? vendorType : string.Empty);
        }

        private string getVendorType()
        {
            string vendorType=string.Empty;
            if (rad_BulkVendor.Checked)
                vendorType = "T";
            else if (rad_NonTradeVendor.Checked)
                vendorType = "N";
            return vendorType;
        }

        private string getVendorType(int vendorTypeId)
        {
            return (vendorTypeId == 10 ? "T" : (vendorTypeId == 20 ? "N" : string.Empty));
        }

        private string extractLetter(string text)
        {
            return extractLetter(text, false);
        }

        private string extractLetter(string text, bool includeNumber)
        {
            string filteredText = string.Empty;
            char prevChar = ' ';
            char thisChar = ' ';
            for (int i = 0; i < text.Length; i++)
            {
                thisChar = text[i];
                if (!(thisChar == ' ' && prevChar == ' '))
                {
                    if (char.IsLetter(thisChar) || (includeNumber && char.IsNumber(thisChar)))
                        filteredText += thisChar;
                    else if (!filteredText.EndsWith(" "))
                        filteredText += " ";
                }
                prevChar = thisChar;
            }
            return filteredText;
        }

        private string getFirstTwoWord(string name)
        {
            string[] words = extractLetter(name, true).Split(' ');
            return (words.Length == 0 ? string.Empty : (words.Length == 1 ? words[0] : words[0] + " " + words[1]));
        }

        private string getVendorNamePrefix(string vendorName)
        {
            string prefix = string.Empty;
            if (!string.IsNullOrEmpty(vendorName))
            {
                string[] name = extractLetter(vendorName).Trim().Split(' ');
                if (name.Length == 1)
                    prefix = ((name[0].Length >= 2 ? name[0] : vendorName.Trim()) + "__").Substring(0, 2);
                else
                    if (name.Length > 1)
                        prefix = name[0].Substring(0, 1) + name[1].Substring(0, 1);
            }
            return prefix.ToUpper();
        }
        
        private ArrayList getSimilarVendorList(string vendorName, string vendorType)
        {
            ArrayList vendorIdList = new ArrayList();
            if (!string.IsNullOrEmpty(vendorName) && !string.IsNullOrEmpty(vendorType))
            {
                if (vendorType == "N")
                {   // Non-Trade Vendor
                    ArrayList ntVendorList = WebUtil.getNTVendorByName(vendorName, -1, -1);
                    if (ntVendorList.Count > 0)
                        foreach (NTVendorDef v in ntVendorList)
                            if (!string.IsNullOrEmpty(v.EPVendorCode) && ("|N|T|".Contains("|"+vendorType+"|") && v.EPVendorCode.EndsWith(vendorType)))
                            {
                                ListItem itm = new ListItem();
                                itm.Value = v.NTVendorId.ToString();
                                itm.Text = v.VendorName + " - (" + v.Office.OfficeCode + ") - [" + v.EPVendorCode + "]";
                                vendorIdList.Add(itm);
                            }
                }

                if (vendorType == "T")
                {   // Trade Vendor
                    ArrayList tradeVendorList = (ArrayList)IndustryUtil.getVendorList(-1, vendorName);
                    if (tradeVendorList.Count > 0)
                        foreach (VendorRef v in tradeVendorList)
                            if (!string.IsNullOrEmpty(v.EpicorSupplierId) && v.IsDelete == "N" && ("|N|T|".Contains("|"+vendorType+"|") && v.EpicorSupplierId.EndsWith(vendorType)))
                            {
                                ListItem itm = new ListItem();
                                itm.Value = v.VendorId.ToString();
                                OfficeRef office = CommonUtil.getOfficeRefByKey(v.OfficeId);
                                itm.Text = v.Name + (office != null ? " - (" + office.OfficeCode + ")" : "") + " - [" + v.EpicorSupplierId + "]";
                                vendorIdList.Add(itm);
                            }
                }
            }
            ListItem noMatch = new ListItem();
            noMatch.Value = "0";
            noMatch.Text = "-- No Match --";
            vendorIdList.Add(noMatch);

            return vendorIdList;
        }

        private void refreshSimilarVendorList()
        {
            string vendorName = getFirstTwoWord(txt_VendorName.Text).Trim();
            string vendorType = getVendorType();
            generateSimilarVendorList(vendorName, vendorType, null);
        }

        private void generateSimilarVendorList(string vendorName, string vendorType, string vendorCode)
        {
            ListItem defaultVendor = null;
            vendorCode = (vendorCode == null ? "" : vendorCode);
            ArrayList similarVendorList = getSimilarVendorList(vendorName, vendorType);
            if (!string.IsNullOrEmpty(vendorName))
                foreach(ListItem v in similarVendorList)
                    if (v.Text.Contains(txt_VendorName.Text) || v.Text.Contains("[" + vendorCode + "]"))
                    {
                        defaultVendor = v;
                        break;
                    }
            ddl_SimilarVendor.bindList(similarVendorList, "Text", "Value", (defaultVendor != null ? defaultVendor.Value : GeneralCriteria.ALL.ToString()), "-- Please select --", GeneralCriteria.ALL.ToString());
        }

    }
}
