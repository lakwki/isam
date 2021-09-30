using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.util;
using CrystalDecisions.CrystalReports.Engine;

using com.next.infra.web;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.module;
using com.next.common.web.commander;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.shipping;
using com.next.isam.domain.types;
using com.next.isam.domain.order;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.shipment;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.lcreport;
using com.next.isam.domain.shipping;
using com.next.common.appserver;

namespace com.next.isam.webapp.shipping
{

    public partial class ShippingDocumentControl : com.next.isam.webapp.usercontrol.PageTemplate
    {
        //private bool HasAccessRightsTo_SuperView;
        private bool HasAccessRightsTo_Update;
        private bool HasAccessRightsTo_Print;
        private bool HasAccessRightsTo_View;
        private bool HasAccessRightsTo_AccessLC;
        private ArrayList userOfficeIdList;
        private ArrayList userDepartmentList;
        //private ArrayList userProductTeamList;
        private ArrayList COIdList;

        //private ArrayList userOfficeList;

        // Pre-NOW version
        private string decryptParameter(string param) { return (string.IsNullOrEmpty(param) ? string.Empty : param); }
        private string encryptParameter(string param) { return (string.IsNullOrEmpty(param) ? string.Empty : param); }
        private string openPopupWindowFunction = "window.open";
        //NOW version
        //private string decryptParameter(string param) { return WebUtil.DecryptParameter(param); }
        //private string encryptParameter(string param) { return WebUtil.EncryptParameter(param); }
        //private string openPopupWindowFunction = "openPopupWindow";

        ArrayList vsShipmentList
        {
            get { return (ArrayList)ViewState["ShipmentList"]; }
            set { ViewState["ShipmentList"] = value; }
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


        protected void Page_Load(object sender, EventArgs e)
        {
            ArrayList userOfficeList;
            //ArrayList coList;
            int UserId;

            userOfficeIdList = new ArrayList();
            userDepartmentList = new ArrayList();
            //userProductTeamList = new ArrayList();
            COIdList = new ArrayList();

            //HasAccessRightsTo_SuperView = (this.LogonUserId == 574 ? true : false);
            HasAccessRightsTo_View = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shippingDocumentControl.Id, ISAMModule.shippingDocumentControl.View);
            HasAccessRightsTo_Update = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shippingDocumentControl.Id, ISAMModule.shippingDocumentControl.UpdateShipment);
            HasAccessRightsTo_Print = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shippingDocumentControl.Id, ISAMModule.shippingDocumentControl.PrintReport);
            HasAccessRightsTo_AccessLC = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shippingDocumentControl.Id, ISAMModule.shippingDocumentControl.AccessLC);

            pnl_PageBody.Visible = (HasAccessRightsTo_View || HasAccessRightsTo_Update || HasAccessRightsTo_Print);
            //lbl_view.Visible = HasAccessRightsTo_View;
            //lbl_Update.Visible = HasAccessRightsTo_Update;
            //lbl_Print.Visible = HasAccessRightsTo_Print;

            //********** For Testing **************************
            //if (this.LogonUserId == 574)
            //    HasAccessRightsTo_SuperView = true;
            //********** For Testing **************************
            //UserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);
            UserId = this.LogonUserId;

            if (!Page.IsPostBack)
            {

                txt_VendorName.setWidth(300);
                txt_VendorName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

                //if (HasAccessRightsTo_SuperView)
                //    userOfficeList = CommonUtil.getOfficeRefListByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralStatus.ACTIVE.Code);
                //else
                TypeCollector officeType = TypeCollector.Inclusive;
                userOfficeList = (ArrayList)CommonUtil.getOfficeListByUserId(UserId, OfficeStructureType.PRODUCTCODE.Type);
                foreach (OfficeRef office in userOfficeList)
                {
                    office.Description.Replace("Office", "").Trim();
                    userOfficeIdList.Add(office.OfficeId);
                    officeType.append(office.OfficeId);
                }
                this.ddl_Office.bindList(userOfficeList, "Description", "OfficeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                if (userOfficeList.Count == 1)
                {
                    OfficeRef oref = (OfficeRef)userOfficeList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                }
                TypeCollector jobNatureType = TypeCollector.Inclusive;
                jobNatureType.append(JobNatureId.SHIPPING.Id);
                ArrayList userList = CommonUtil.getUserSelectionListByOfficeJobNature(officeType, jobNatureType, "A");
                ArrayList shippingUserList = new ArrayList();
                shippingUserList.Add(userList[0]);
                for (int i = 1, j = 0; i < userList.Count; i++)
                {
                    for (j = 0; j < shippingUserList.Count; j++)
                        if (string.Compare(((UserSelectionRef)userList[i]).DisplayName, ((UserSelectionRef)shippingUserList[j]).DisplayName) < 0)
                            break;
                    shippingUserList.Insert(j, userList[i]);
                }
                UserSelectionRef NssAdmin = CommonUtil.getUserSelectionByKey(99999);
                shippingUserList.Add(NssAdmin);
                ddl_ShippingUser.bindList(shippingUserList, "DisplayName", "UserId", "", "-- ALL --", GeneralCriteria.ALL.ToString());

                cbl_Customer.DataSource = WebUtil.getCustomerList();
                cbl_Customer.DataTextField = "CustomerCode";
                cbl_Customer.DataValueField = "CustomerId";
                cbl_Customer.DataBind();

                //userProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralCriteria.ALL, UserId, GeneralCriteria.ALLSTRING);
                //vsUserProductTeamList = userProductTeamList;
                //ddl_ProductTeam.bindList(userProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
                //ddl_ProductTeam_Refresh();

                //coList = CommonUtil.getCountryOfOriginList();
                //foreach (CountryOfOriginRef CO in coList)
                //{
                //    COIdList.Add(CO.CountryOfOriginId);
                //}

                //this.ddl_ProductTeam.bindList(userProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
                //if (userProductTeamList.Count == 1) this.ddl_ProductTeam.SelectedIndex = 1;

                //this.ddl_ProductTeam_Refresh();

                //ddl_Office.Attributes.Add("onchange", "cbxOfficeOnChange();");
                //this.ddl_CO.bindList(coList, "Name", "CountryOfOriginId", "", "--All--", GeneralCriteria.ALL.ToString());

                setDefaultCriteria();

                this.btn_Update.Enabled = HasAccessRightsTo_Update;
                this.btn_Search.Enabled = (HasAccessRightsTo_View || HasAccessRightsTo_Update);
            }

            RefreshReportingButton();

        }


        protected void initControl()
        {
        }


        protected void btn_Search_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            pnl_SearchResult.Visible = true;
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.GetShipmentListForMassUpdate);

            Context.Items.Add(ShipmentCommander.Param.contractNo, txt_ContractNo.Text.Trim());
            Context.Items.Add(ShipmentCommander.Param.itemNo, txt_ItemNo.Text.Trim());
            Context.Items.Add(ShipmentCommander.Param.vendorId, txt_VendorName.VendorId == int.MinValue ? GeneralCriteria.ALL : txt_VendorName.VendorId);

            if (txt_LCNoFrom.Text.Trim() != "" || txt_LCNoTo.Text.Trim() != "")
            {
                Context.Items.Add(ShipmentCommander.Param.lcNoFrom, (string.IsNullOrEmpty(txt_LCNoFrom.Text) ? txt_LCNoTo.Text : txt_LCNoFrom.Text.Trim()));
                Context.Items.Add(ShipmentCommander.Param.lcNoTo, (string.IsNullOrEmpty(txt_LCNoTo.Text) ? txt_LCNoFrom.Text : txt_LCNoTo.Text.Trim()));
            }

            TypeCollector selectedCustomerList = TypeCollector.Inclusive;
            for (int i = 0; i < cbl_Customer.Items.Count; i++)
            {
                if (cbl_Customer.Items[i].Selected)
                    selectedCustomerList.append(Int32.Parse(cbl_Customer.Items[i].Value));
            }
            //Context.Items.Add(ShipmentCommander.Param.customerId, ddl_Customer.SelectedValue);
            if (selectedCustomerList.Values.Count == 0)
                selectedCustomerList.append(-1);
            Context.Items.Add(ShipmentCommander.Param.customerList, selectedCustomerList);

            int DocReceiptStatus, LCPaymentStatus;
            if (cbx_DocReceiptDateUpdated.Checked)
                DocReceiptStatus = (cbx_DocReceiptDateNotUpdated.Checked ? GeneralCriteria.ALL : 1);
            else
                DocReceiptStatus = (cbx_DocReceiptDateNotUpdated.Checked ? 0 : int.MinValue);
            Context.Items.Add(ShipmentCommander.Param.ShippingDocumentReceiptStatus, DocReceiptStatus);

            if (cbx_LCPaymentCheckedUpdated.Checked)
                LCPaymentStatus = (cbx_LCPaymentCheckedNotUpdated.Checked ? GeneralCriteria.ALL : 1);
            else
                LCPaymentStatus = (cbx_LCPaymentCheckedNotUpdated.Checked ? 0 : int.MinValue);
            Context.Items.Add(ShipmentCommander.Param.LcPaymentCheckStatus, LCPaymentStatus);

            if (txt_ActualAtWHDateFrom.Text.Trim() != "")
            {
                Context.Items.Add(ShipmentCommander.Param.actualAtWHDateFrom, Convert.ToDateTime(txt_ActualAtWHDateFrom.Text.Trim()));
                if (txt_ActualAtWHDateTo.Text.Trim() == "")
                    txt_ActualAtWHDateTo.Text = txt_ActualAtWHDateFrom.Text;
                Context.Items.Add(ShipmentCommander.Param.actualAtWHDateTo, Convert.ToDateTime(txt_ActualAtWHDateTo.Text.Trim()));
            }

            if (txt_ILSActualAtWHDateFrom.Text.Trim() != "")
            {
                Context.Items.Add(ShipmentCommander.Param.ILSActualAtWHDateFrom, Convert.ToDateTime(txt_ILSActualAtWHDateFrom.Text.Trim()));
                if (txt_ILSActualAtWHDateTo.Text.Trim() == "")
                    txt_ILSActualAtWHDateTo.Text = txt_ILSActualAtWHDateFrom.Text;
                Context.Items.Add(ShipmentCommander.Param.ILSActualAtWHDateTo, Convert.ToDateTime(txt_ILSActualAtWHDateTo.Text.Trim()));
            }

            TypeCollector workflowStatusList = TypeCollector.Inclusive;
            workflowStatusList.append(ContractWFS.INVOICED.Id);
            Context.Items.Add(ShipmentCommander.Param.workflowStatusList, workflowStatusList);

            TypeCollector officeList = TypeCollector.Inclusive;
            if (ddl_Office.SelectedValue == GeneralCriteria.ALL.ToString())
            {
                //foreach (int officeId in userOfficeIdList)
                for (int i = 0; i < ddl_Office.Items.Count; i++)
                    officeList.append(Int32.Parse(ddl_Office.Items[i].Value));
            }
            else
            {
                officeList.append(Int32.Parse(ddl_Office.SelectedValue));
            }
            Context.Items.Add(ShipmentCommander.Param.officeList, officeList);

            TypeCollector shippingUserList = TypeCollector.Inclusive;
            if (ddl_ShippingUser.SelectedValue == GeneralCriteria.ALL.ToString())
                for (int i = 0; i < ddl_ShippingUser.Items.Count; i++)
                    shippingUserList.append(Int32.Parse(ddl_ShippingUser.Items[i].Value));
            else
                shippingUserList.append(Int32.Parse(ddl_ShippingUser.SelectedValue));
            Context.Items.Add(ShipmentCommander.Param.shippingUserList, shippingUserList);

            Context.Items.Add(ShipmentCommander.Param.sortingOrder, ucl_SortingOrder.SortingField);
            //Context.Items.Add(ShipmentCommander.Param.productTeamId,  GeneralCriteria.ALL);


            forwardToScreen(null);

            ArrayList list = (ArrayList)Context.Items[ShipmentCommander.Param.shipmentList];

            int issuingBankId = int.MinValue;
            LCBatchRef lcBatch;
            foreach (ContractShipmentListJDef shipment in list)
                if ((lcBatch = LCWorker.Instance.getLCBatchByShipmentId(shipment.ShipmentId)) != null)
                    if (issuingBankId != lcBatch.IssuingBankId)
                        issuingBankId = (issuingBankId == int.MinValue ? lcBatch.IssuingBankId : -1);
            
            string defaultLCNo = string.Empty;
            if (txt_LCNoFrom.Text == txt_LCNoTo.Text || string.IsNullOrEmpty(txt_LCNoFrom.Text) || string.IsNullOrEmpty(txt_LCNoTo.Text))
                defaultLCNo = (string.IsNullOrEmpty(txt_LCNoFrom.Text) ? txt_LCNoTo.Text : txt_LCNoFrom.Text).Trim();
            if (issuingBankId == IssuingBank.HSBC.Id)
            {
                txt_LCBillRefNoSequence.Text = string.Empty;
                txt_LCBillRefNoSequence.Width = 40;
                txt_LCBillRefNoSequence.MaxLength = 6;

                txt_LCBillRefNoPrefix.Text = "BR NEX";
                txt_LCBillRefNoPrefix.Width = 35;
                txt_LCBillRefNoPrefix.MaxLength = 6;
                txt_LCBillRefNoPrefix.Style.Add(HtmlTextWriterStyle.Display, "block");

                txt_LCBillRefNoSuffix.Text = "HKH";
                txt_LCBillRefNoSuffix.Width = 20;
                txt_LCBillRefNoSuffix.MaxLength = 3;
                txt_LCBillRefNoSuffix.Style.Add(HtmlTextWriterStyle.Display, "block");
            }
            else if (issuingBankId == IssuingBank.SCB.Id)
            {
                txt_LCBillRefNoSequence.Text = string.Empty;
                txt_LCBillRefNoSequence.Width = 20;
                txt_LCBillRefNoSequence.MaxLength = 3;

                if (defaultLCNo.Length > 2)
                    txt_LCBillRefNoPrefix.Text = (defaultLCNo.Substring(defaultLCNo.Length - 2, 2).ToUpper() == "-G" ? defaultLCNo.Substring(0, defaultLCNo.Length - 2) : defaultLCNo) + "PAY";
                else
                    txt_LCBillRefNoPrefix.Text = (defaultLCNo != string.Empty ? defaultLCNo + "PAY" : string.Empty);
                txt_LCBillRefNoPrefix.Width = 95;
                txt_LCBillRefNoPrefix.MaxLength = 15;
                txt_LCBillRefNoPrefix.Style.Add(HtmlTextWriterStyle.Display, "block");

                txt_LCBillRefNoSuffix.Text = string.Empty;
                txt_LCBillRefNoSuffix.MaxLength = 0;
                txt_LCBillRefNoSuffix.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
            else
            {
                txt_LCBillRefNoSequence.Text = string.Empty;
                txt_LCBillRefNoSequence.Width = 100;
                txt_LCBillRefNoSequence.MaxLength = 20;

                txt_LCBillRefNoPrefix.Text = string.Empty;
                txt_LCBillRefNoPrefix.Style.Add(HtmlTextWriterStyle.Display, "none");

                txt_LCBillRefNoSuffix.Text = string.Empty;
                txt_LCBillRefNoSuffix.Style.Add(HtmlTextWriterStyle.Display, "none");
            }

            this.vsShipmentList = list;
            gv_Shipment.DataSource = list;
            gv_Shipment.DataBind();

            if (list != null)
            {
                if (list.Count < 100)
                    lbl_RowCount.Text = string.Format("Total {0} records.", list.Count.ToString());
                else
                    lbl_RowCount.Text = "There are more than 100 shipment matching your search criteria.<br />" +
                        "Only the first 100 search result are shown.";
                pnl_Update.Visible = HasAccessRightsTo_Update;
                pnl_SearchResult.Visible = (HasAccessRightsTo_View || HasAccessRightsTo_Update);
            }
            else
            {
                pnl_Update.Visible = false;
                pnl_SearchResult.Visible = false;
            }

            btn_Print.Style.Add(HtmlTextWriterStyle.Display, "none");
            btn_Export.Style.Add(HtmlTextWriterStyle.Display, "none");
        }


        protected void gv_Shipment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContractShipmentListJDef shipment = (ContractShipmentListJDef)this.vsShipmentList[e.Row.RowIndex];
                ArrayList deductions = ShipmentManager.Instance.getShipmentDeductionList(shipment.ShipmentId);
                decimal SupplierInvoiceNetAmount = shipment.TotalShippedSupplierGarmentAmountAfterDiscount - shipment.QACommissionAmount - shipment.VendorPaymentDiscountAmount
                                                - ShipmentManager.Instance.calcShipmentDeductionTotal(deductions);
                decimal NSL_NUK_Amt = shipment.TotalShippedAmount;
                decimal Supplier_NSL_amt = shipment.TotalShippedSupplierGarmentAmountAfterDiscount;
                
                //((CheckBox)e.Row.FindControl("ckb_Selected")).Enabled = true;
                bool checkBoxEnable = true;
                if (ShipmentManager.Instance.isReadyForDMS(shipment))
                {
                    string remark = string.Empty;
                    if (shipment.ShippingDocWFS.Id == ShippingDocWFS.NOT_READY.Id || shipment.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                    {
                        checkBoxEnable = true;
                        if (!string.IsNullOrEmpty(shipment.SupplierInvoiceNo) && isShipDocInfoValidInSplitShipment(shipment.ShipmentId))
                            remark = "";
                        else
                            remark = "Supplier Invoice No. in the shipment is not complete yet.\n"
                                    + "Shipping Document Status will not be updated completely.";
                    }
                    else
                    {   // Status is not NOT_READY and not REJECTED
                        remark = "Shipping document status is [" + ShippingDocWFS.getType(shipment.ShippingDocWFS.Id).Name + "].\n";
                        checkBoxEnable = false;
                        if (shipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode() && !shipment.IsLCPaymentChecked)
                        {   // Allow to update the LC Payment Check Date
                            remark += "You can only update the L/C Payment Checked Date.";
                            checkBoxEnable = true;
                        }
                        else // Open Account shipmnt or LC Payment Checked
                            remark += "You are not allow to update this record.";
                    }
                    ((CheckBox)e.Row.FindControl("ckb_Selected")).ToolTip = remark;
                    ((Panel)e.Row.FindControl("pnl_RowRemark")).ToolTip = remark;
                    ((CheckBox)e.Row.FindControl("ckb_Selected")).Enabled = checkBoxEnable;
                    ((Panel)e.Row.FindControl("pnl_RowRemark")).Style.Add(HtmlTextWriterStyle.Display, (remark == "" ? "none" : "block"));


                    ((TextBox)e.Row.FindControl("txt_ShipDocReadyToCheck")).Enabled = true;
                    if (!string.IsNullOrEmpty(shipment.SupplierInvoiceNo)) // && shipment.PiecesPerDeliveryUnit > 0) // && shipment.ShippingDocReceiptDate != DateTime.MinValue)
                        ((TextBox)e.Row.FindControl("txt_ShipDocReadyToCheck")).Text = "READY";
                    else
                        ((TextBox)e.Row.FindControl("txt_ShipDocReadyToCheck")).Text = "";
                }
                else
                {   // not ready for DMS
                    ((TextBox)e.Row.FindControl("txt_ShipDocReadyToCheck")).Enabled = false;
                    ((TextBox)e.Row.FindControl("txt_ShipDocReadyToCheck")).Text = "";
                }


                ((HiddenField)e.Row.FindControl("hid_ShipmentId")).Value = shipment.ShipmentId.ToString();

                //((Label)e.Row.FindControl("lbl_ContractNo")).Text = shipment.ContractNo;
                ((LinkButton)e.Row.FindControl("btn_ContractNo")).Text = shipment.ContractNo;
                //((LinkButton)e.Row.FindControl("btn_ContractNo")).Attributes.Add("OnClick", "window.open('../Shipping/ShipmentDetail.aspx?ShipmentId=" + shipment.ShipmentId.ToString() + "&DefaultReceiptDate=" + HttpUtility.UrlEncode(DateTimeUtility.getDateString(DateTime.Today)) + "', 'ShipmentDetail', 'width=800,height=600,scrollbars=1,resizable=1,status=1'); return false;");
                ((LinkButton)e.Row.FindControl("btn_ContractNo")).Attributes.Add("OnClick", openPopupWindowFunction + "('../Shipping/ShipmentDetail.aspx?ShipmentId=" + encryptParameter(shipment.ShipmentId.ToString()) + "&DefaultReceiptDate=" + encryptParameter(DateTimeUtility.getDateString(DateTime.Today)) + "', 'ShipmentDetail', 'width=800,height=600,scrollbars=1,resizable=1,status=1'); return false;");

                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = shipment.DeliveryNo.ToString();

                Image img = (Image)e.Row.FindControl("img_ReadDmsDoc");
                if (shipment.IsUploadDMSDocument)
                    //img.Attributes.Add("onclick", "window.open('../account/AttachmentList.aspx?ShipmentId=" + shipment.ShipmentId.ToString() + "', 'DMS_Uploaded_Documents', 'width=400,height=300,scrollbars=1,status=0');return false;");
                    img.Attributes.Add("onclick", openPopupWindowFunction+"('../account/AttachmentList.aspx?ShipmentId=" + WebUtil.EncryptParameter(shipment.ShipmentId.ToString()) + "', 'DMS_Uploaded_Documents', 'width=400,height=300,scrollbars=1,status=0');return false;");
                else
                    img.Visible = false;

                /*
                TestResult = 2 : 
                TestResult = 1 : Pass 
                TestResult = 0 : Fail GB and waiting UK approval
                TestResult = 9 : Fail Both GB and UK standard
                 */

                if (shipment.IsChinaGBTestRequired)
                {
                    int testResult = getChinaGBTestResult(shipment);
                    ((Image)e.Row.FindControl("img_GBTestRequire")).Visible = true;                         // (testResult == -1); // Test Result not yet update
                    ((Image)e.Row.FindControl("img_GBTestPass")).Visible = (testResult == 1);               // Pass
                    ((Image)e.Row.FindControl("img_GBTestFailRelease")).Visible = (testResult == 2);        // Fail GB but pass UK standard and approved by NUK
                    ((Image)e.Row.FindControl("img_GBTestFailHold")).Visible = (testResult == 0);           // Fail GB and waiting UK approval
                    ((Image)e.Row.FindControl("img_GBTestFailNotRelease")).Visible = (testResult == 9);     // Fail Both GB and UK standard
                }
                if (ShipmentManager.Instance.isViaCambodiaQCC(shipment.ShipmentId))
                    ((Image)e.Row.FindControl("img_QccInspection")).Visible = true;

                ((Label)e.Row.FindControl("lbl_Supplier")).Text = shipment.Vendor.Name;
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = shipment.ItemNo;
                ((Label)e.Row.FindControl("lbl_Destination")).Text = (shipment.CustomerDestination == null ? "" : shipment.CustomerDestination.DestinationCode);
                ((Label)e.Row.FindControl("lbl_Customer")).Text = shipment.Customer.CustomerCode;
                ((Label)e.Row.FindControl("lbl_TotalShippedQuantity")).Text = shipment.TotalShippedQuantity.ToString("N00");
                ((Label)e.Row.FindControl("lbl_Currency")).Text = shipment.SellCurrency.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_SupplierInvoiceNetAmount")).Text = SupplierInvoiceNetAmount.ToString("N02");
                if (NSL_NUK_Amt != Supplier_NSL_amt)
                    ((Label)e.Row.FindControl("lbl_SupplierInvoiceNetAmount")).ForeColor = System.Drawing.Color.Green;

                ((Label)e.Row.FindControl("lbl_SupplierInvoiceNo")).Text = shipment.SupplierInvoiceNo;
                ((Label)e.Row.FindControl("lbl_InvoiceNo")).Text = shipment.InvoiceNo;
                ((Label)e.Row.FindControl("lbl_InvoiceDate")).Text = DateTimeUtility.getDateString(shipment.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_NoOfSplit")).Text = shipment.SplitCount.ToString("N00");
                ((Label)e.Row.FindControl("lbl_ShippingUser")).Text = (shipment.ShippingUser == null ? "" : shipment.ShippingUser.DisplayName);
                ((Label)e.Row.FindControl("lbl_ShipmentWorkflowStatus")).Text = shipment.WorkflowStatus.Name;

                //e.Row.Cells[18].BackColor = System.Drawing.Color.Gray;
                //e.Row.Cells[18].Width = 1;
                //e.Row.Cells[18].BorderStyle = BorderStyle.None;
                //e.Row.Cells[18].BorderWidth = 2;
                //e.Row.Cells[18].BorderColor = System.Drawing.Color.Red;

            }
            //else
            //    if (e.Row.RowType == DataControlRowType.Separator)
            //    {
            //        e.Row.Cells[18].BackColor = System.Drawing.Color.Yellow;
            //        e.Row.Cells[18].Width = 1;
            //        e.Row.Cells[18].BorderStyle = BorderStyle.Double;
            //        e.Row.Cells[18].BorderWidth = 1;
            //        e.Row.Cells[18].BorderColor = System.Drawing.Color.Red;
            //        e.Row.Cells[18].Height = 20;
            //        e.Row.Cells[18].Visible = false;
            //    }
        }

        protected int getChinaGBTestResult(ContractShipmentListJDef contractShipment)
        {
            return GeneralManager.Instance.getChinaGBTestResult(contractShipment.ProductId, contractShipment.Vendor.VendorId);
        }
        /*
        protected bool isChinaGBTestRequired(ContractShipmentListJDef contractShipment)
        {
            return GeneralManager.Instance.isChinaGBTestRequired(contractShipment.Customer.CustomerId, contractShipment.Season.SeasonId, contractShipment.ProductTeam.ProductCodeId, contractShipment.Vendor.VendorId);
        }
        */
        #region ButtonClick



        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            setDefaultCriteria();
            RefreshReportingButton();

        }

        protected void setDefaultCriteria()
        {
            string CustomerId;

            txt_ContractNo.Text = "";
            txt_ItemNo.Text = "";
            txt_LCNoFrom.Text = "";
            txt_LCNoTo.Text = "";
            txt_ActualAtWHDateFrom.Text = "";
            txt_ActualAtWHDateTo.Text = "";
            txt_ILSActualAtWHDateFrom.Text = "";
            txt_ILSActualAtWHDateTo.Text = "";
            ddl_Office.SelectedIndex = 0;
            ddl_ShippingUser.SelectedIndex = 0;

            cbx_DocReceiptDateNotUpdated.Checked = true;
            cbx_DocReceiptDateUpdated.Checked = false;
            if (HasAccessRightsTo_AccessLC)
            {
                cbx_LCPaymentCheckedNotUpdated.Checked = true;
                cbx_LCPaymentCheckedUpdated.Checked = false;
            }
            else
            {
                cbx_LCPaymentCheckedNotUpdated.Checked = true;
                cbx_LCPaymentCheckedUpdated.Checked = true;
            }
            tr_LCPaymentChecked.Style.Add(HtmlTextWriterStyle.Display, (HasAccessRightsTo_AccessLC ? "" : "none"));
            td_LcBillRefNo.Style.Add(HtmlTextWriterStyle.Display, (HasAccessRightsTo_AccessLC ? "" : "none"));
            td_PayDocCheckDateTitle.Style.Add(HtmlTextWriterStyle.Display, (HasAccessRightsTo_AccessLC ? "" : "none"));
            td_PayDocCheckDateInput.Style.Add(HtmlTextWriterStyle.Display, (HasAccessRightsTo_AccessLC ? "" : "none"));

            txt_ShippingDocReceiptDate.Text = "";
            txt_LcPaymentCheckDate.Text = "";

            txt_VendorName.clear();
            //uclProductTeam.clear();

            pnl_SearchResult.Visible = false;
            pnl_Update.Visible = false;
            vsShipmentList = null;

            for (int i = 0; i < cbl_Customer.Items.Count; i++)
            {
                CustomerId = cbl_Customer.Items[i].Value;
                if (CustomerId == "1" || CustomerId == "2" || CustomerId == "6" || CustomerId == "7")
                {   // DIRECTORY, RETAIL, NTN, LIME are default selected
                    cbl_Customer.Items[i].Selected = true;
                }
                else
                    cbl_Customer.Items[i].Selected = false;
            }

            ucl_SortingOrder.clearAllItems();
            ucl_SortingOrder.addItem("Contract No. (Ascending)", "c.ContractNo, s.DeliveryNo", true);
            ucl_SortingOrder.addItem("Contract No. (Descending)", "c.ContractNo desc, s.DeliveryNo desc", false);
            ucl_SortingOrder.addItem("Invoice Date (Ascending)", "i.InvoiceDate", false);
            ucl_SortingOrder.addItem("Invoice Date (Descending)", "i.InvoiceDate desc", false);
            ucl_SortingOrder.addItem("Shipping User (Ascending)", "u.DisplayName", false);
            ucl_SortingOrder.addItem("Shipping User (Descending)", "u.DisplayName desc", false);
            ucl_SortingOrder.addItem("Supplier Inv. No. (Ascending)", "i.SupplierInvoiceNo", false);
            ucl_SortingOrder.addItem("Supplier Inv. No. (Descending)", "i.SupplierInvoiceNo desc", false);

        }


        protected void btn_Update_Click(object sender, EventArgs e)
        {
            ArrayList selectedShipment = new ArrayList();
            //for(int i=0;i<gv_Shipment.Rows.Count;i++)
            int invalidCount = 0;
            foreach (GridViewRow r in gv_Shipment.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    if (((CheckBox)r.FindControl("ckb_Selected")).Checked)
                    {
                        bool valid = true;
                        //if (ckb_ShipDocCheck.Checked)
                        //{
                            if (((TextBox)r.FindControl("txt_ShipDocReadyToCheck")).Enabled // ready for DMS
                                && (
                                    //((TextBox)r.FindControl("txt_ShipDocReadyToCheck")).Text != "READY" ||
                                    (((TextBox)r.FindControl("txt_ShipDocReadyToCheck")).Text == "READY" && txt_ShippingDocReceiptDate.Text == "")
                                    )
                                )
                            {   // This shipment is not ready for update the Shipping Doc Status

                                // allow to update (LC Payment checked date) even if the DMS info is not ready
                                //valid = false;    
                            }
                        //}
                        if (valid)
                            selectedShipment.Add(((ContractShipmentListJDef)this.vsShipmentList[r.RowIndex]).ShipmentId);
                        else
                            invalidCount++;
                    }
                }
            }
            if (invalidCount > 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UpdateAborted", "alert('" + invalidCount.ToString() + " shipment" + (invalidCount > 1 ? "s are" : " is") + " not ready to change the document status.\\nUpdate operation is aborted');", true);
                return;
            }
            RefreshReportingButton();

            if (selectedShipment.Count > 0)
            {
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
                //Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.GetShipmentListForAdvancedSearch);
                Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.ShipmentListMassUpdate);

                Context.Items.Add(ShipmentCommander.Param.shipmentList, selectedShipment);
                if (txt_ShippingDocReceiptDate.Text.Trim() != "")
                    Context.Items.Add(ShipmentCommander.Param.shippingDocumentReceiptDate, txt_ShippingDocReceiptDate.Text);

                if (txt_LcPaymentCheckDate.Text.Trim() != "")
                    Context.Items.Add(ShipmentCommander.Param.lcPaymentCheckDate, txt_LcPaymentCheckDate.Text);

                //Context.Items.Add(ShipmentCommander.Param.UpdateShippingDocStatus, (ckb_ShipDocCheck.Checked));

                string LCBillRefNo = txt_LCBillRefNoPrefix.Text + txt_LCBillRefNoSequence.Text + txt_LCBillRefNoSuffix.Text;
                if (LCBillRefNo != "")
                    Context.Items.Add(ShipmentCommander.Param.lcBillRefNo, LCBillRefNo);

                forwardToScreen(null);

            }

        }


        protected void btn_Print_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "LC Status Report");
        }


        protected void btn_Export_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "LC Status Report");
        }


        #endregion

        private ReportClass genReport()
        {
            int vendorId;
            string lcNoFrom, lcNoTo;
            string lcBillRefNo;
            LCStatusRpt rpt = null;

            //TypeCollector selectedShipment;
            //selectedShipment =TypeCollector.Inclusive;
            //selectedShipment.append(-1);
            //for (int i=0; i<gv_Shipment.Rows.Count;i++)
            //{
            //    if (((CheckBox)gv_Shipment.Rows[i].FindControl("ckb_Selected")).Checked)
            //        selectedShipment.append(Int32.Parse(((HiddenField)gv_Shipment.Rows[i].FindControl("hid_ShipmentId")).Value));
            //}

            lcNoFrom = (string.IsNullOrEmpty(txt_LCNoFrom.Text) ? txt_LCNoTo.Text : txt_LCNoFrom.Text);
            lcNoTo = (string.IsNullOrEmpty(txt_LCNoTo.Text) ? txt_LCNoFrom.Text : txt_LCNoTo.Text);
            lcBillRefNo = txt_LCBillRefNoPrefix.Text.Trim() + txt_LCBillRefNoSequence.Text.Trim() + txt_LCBillRefNoSuffix.Text.Trim();

            if (txt_VendorName.VendorId <= 0)
            {
                vendorId = int.MinValue;
            }
            else
            {
                vendorId = txt_VendorName.VendorId;
            }
            rpt = LCReportManager.Instance.getLCStatusReport(vendorId, lcNoFrom, lcNoTo, lcBillRefNo, this.LogonUserId);
            return rpt;
        }


        protected void RefreshReportingButton()
        {
            if ((txt_LCNoFrom.Text == "" && txt_LCNoTo.Text == "") || txt_LcPaymentCheckDate.Text == "" ) //|| ((TextBox)txt_VendorName.FindControl("txtName")).Text == ""
            {
                btn_Print.Enabled = true; // false;
                btn_Print.ToolTip = "To print L/C Status Report, please specify the L/C No.";
                btn_Print.Style.Add(HtmlTextWriterStyle.Display, "none");

                btn_Export.Enabled = true; // false;
                btn_Export.ToolTip = "To export L/C Status Report, please specify the L/C No.";
                btn_Export.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
            else
            {
                btn_Print.Enabled = true;
                btn_Print.ToolTip = "Print L/C Status Report";
                btn_Print.Style.Add(HtmlTextWriterStyle.Display, "block");

                btn_Export.Enabled = true;
                btn_Export.ToolTip = "Export L/C Status Report";
                btn_Export.Style.Add(HtmlTextWriterStyle.Display, "block");
            }
        }

        private bool isShipDocInfoValidInSplitShipment(int shipmentId)
        {
            bool valid = true;
            ArrayList splitList = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentByShipmentId(shipmentId);
            if (splitList != null)
                foreach (SplitShipmentDef split in splitList)
                    if (split.IsVirtualSetSplit == 0 && (string.IsNullOrEmpty(split.SupplierInvoiceNo)))//|| split.ShippingDocReceiptDate == DateTime.MinValue)
                    {
                        valid = false;
                        break;
                    }
            return valid;
        }


    }

}
