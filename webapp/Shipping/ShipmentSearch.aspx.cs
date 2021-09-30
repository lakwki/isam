using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using com.next.common.domain;
using com.next.common.appserver;
using com.next.isam.domain.types;
using com.next.common.datafactory;
using com.next.common.web;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.isam.appserver.common;
using com.next.isam.appserver.order;
using com.next.infra.web;
using com.next.infra.util;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.shipment;
using com.next.common.web.commander;
using com.next.common.domain.types;
using com.next.common.domain.module;
using com.next.isam.domain.common;
using com.next.isam.domain.order;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.invoice;
using com.next.isam.appserver.shipping;
using com.next.isam.appserver.account;
using com.next.isam.domain.account;

namespace com.next.isam.webapp.shipping
{
    public partial class ShipmentSearch : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private ArrayList vwSearchResult
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (ArrayList)ViewState["SearchResult"];
            }
        }

        private string sortExpression
        {
            get
            {
                return (string)ViewState["SortExpression"];
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }
        private SortDirection sortDirection
        {
            get { return (SortDirection)ViewState["SortDirection"]; }
            set { ViewState["SortDirection"] = value; }

        }

        private ArrayList userOfficeList;
        private bool isAuditViewer = true;

        // Pre-NOW version
        private string decryptParameter(string param) { return (string.IsNullOrEmpty(param) ? string.Empty : param); }
        private string encryptParameter(string param) { return (string.IsNullOrEmpty(param) ? string.Empty : param); }
        private string openPopupWindowFunction = "window.open";
        //NOW version
        //private string decryptParameter(string param) { return WebUtil.DecryptParameter(param); }
        //private string encryptParameter(string param) { return WebUtil.EncryptParameter(param); }
        //private string openPopupWindowFunction = "openPopupWindow";


        protected void Page_Load(object sender, EventArgs e)
        {
            userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

            if (!Page.IsPostBack)
            {
                //this.Form.DefaultButton = this.btn_QuickSearch.UniqueID;
                this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                if (userOfficeList.Count == 1)
                {
                    OfficeRef oref = (OfficeRef)userOfficeList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                }
                this.ddl_OPRType.bindList((ArrayList)OPRFabricType.getCollectionValueForReport(), "OPRFabricReportSelectionName", "OPRFabricTypeId");
                this.ddl_Customer.bindList(WebUtil.getCustomerList(), "CustomerCode", "CustomerId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                this.ddl_OrderType.bindList(WebUtil.getOrderTypeList(), "Name", "Code", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                ddl_TermOfPurchase.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                ddl_FinalDestination.bindList(WebUtil.getFinalDestinationList(), "DestinationDesc", "CustomerDestinationId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                //ddl_WorkflowStatus.bindList(ContractWFS.getCollectionValue(), "Name", "Id" , "", "-- ALL --", GeneralCriteria.ALL.ToString()) ;
                ddl_CO.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "-- ALL --", GeneralCriteria.ALL.ToString());

                txt_SupplierName.setWidth(305);
                uclProductTeam.setWidth(305);
                uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
                txt_SupplierName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

                ddl_Office.Attributes.Add("onchange", "cbxOfficeOnChange();");

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.ShippingAdvancedSearch))
                {
                    tb_DefaultReceiptDate.Visible = true;
                    txt_DefaultDocReceiptDate.Text = DateTimeUtility.getDateString(DateTime.Today);

                    ddl_ShippingUser.Visible = true;
                    TypeCollector officeType = TypeCollector.Inclusive;
                    foreach (OfficeRef def in userOfficeList)
                    {
                        officeType.append(def.OfficeId);
                    }

                    TypeCollector jobNatureType = TypeCollector.Inclusive;
                    jobNatureType.append(JobNatureId.SHIPPING.Id);
                    ArrayList shippingUserList = CommonUtil.getUserSelectionListByOfficeJobNature(officeType, jobNatureType, "A");

                    ddl_ShippingUser.bindList(shippingUserList, "DisplayName", "UserId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                }

            }
            /*
            else
            {
                this.Form.DefaultButton = this.tb_QuickSearch.Visible ? this.btn_QuickSearch.UniqueID : this.btnSearch.UniqueID;
            }
            */

            isAuditViewer = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.AuditView);
            if (isAuditViewer)
                btn_Advance.Visible = false;
            else
                btn_Advance.Visible = true;
        }



        #region gvInvoice Events
        protected void InvoiceDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContractShipmentListJDef contractShipmentDef = (ContractShipmentListJDef)vwSearchResult[e.Row.RowIndex];
                Label lbl = null;


                //ImageButton btn = (ImageButton)e.Row.Cells[0].FindControl("btnView");
                ImageButton btn = (ImageButton)e.Row.FindControl("btnView");
                
                string shipmentIdList = string.Empty;
                foreach (ContractShipmentListJDef def in vwSearchResult)
                    shipmentIdList += (string.IsNullOrEmpty(shipmentIdList) ? "" : "|") + def.ShipmentId.ToString();
                string param = "ShipmentId=" + encryptParameter(contractShipmentDef.ShipmentId.ToString());
                param += "&DefaultReceiptDate=" + encryptParameter(txt_DefaultDocReceiptDate.Text);
                param += "&ShipmentIdList=" + encryptParameter(shipmentIdList);
                
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.View))
                    btn.Attributes.Add("onclick", "openShipment('" + param + "');return false;");
                else
                    btn.Visible = false;

                //((HiddenField)e.Row.Cells[0].FindControl("hid_ShipmentId")).Value = contractShipmentDef.ShipmentId.ToString();
                //HtmlInputCheckBox ckb = (HtmlInputCheckBox)e.Row.Cells[0].FindControl("ckb_Print");
                ((HiddenField)e.Row.FindControl("hid_ShipmentId")).Value = contractShipmentDef.ShipmentId.ToString();
                HtmlInputCheckBox ckb = (HtmlInputCheckBox)e.Row.FindControl("ckb_Print");
                //CheckBox ckb = (CheckBox)e.Row.Cells[0].FindControl("ckb_Print");
                ckb.Attributes.Add("ShipmentId", contractShipmentDef.ShipmentId.ToString());
                ckb.Attributes.Add("CurrencyCode", contractShipmentDef.SellCurrency.CurrencyCode);
                ckb.Attributes.Add("CustomerDestination", contractShipmentDef.CustomerDestination.DestinationCode.ToString());
                if (contractShipmentDef.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
                {
                    ckb.Visible = true;
                    if ((contractShipmentDef.ShippingUser != null && contractShipmentDef.ShippingUser.UserId == this.LogonUserId && CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.PrintInvoice)) ||
                        CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.PrintAllInvoice))
                    {
                        ckb.Disabled = false;
                        //ckb.Enabled = true;
                        ckb.Checked = true;
                    }

                    if (contractShipmentDef.Customer.CustomerType.CustomerTypeId == CustomerTypeRef.Type.lipsy.GetHashCode()
                            || contractShipmentDef.Customer.CustomerType.CustomerTypeId == CustomerTypeRef.Type.daysgroup.GetHashCode())
                        ckb.Attributes.Add("alt", contractShipmentDef.InvoiceNo);

                    Label lbl_InvoiceNo = (Label)e.Row.FindControl("lbl_InvoiceNo");
                    lbl_InvoiceNo.ToolTip = AccountManager.Instance.getCutOffStatus(contractShipmentDef.ShipmentId).ToToolTipText;
                }
                else if ((contractShipmentDef.WorkflowStatus.Id == ContractWFS.PO_PRINTED.Id ||
                    contractShipmentDef.WorkflowStatus.Id == ContractWFS.APPROVED.Id) &&
                    (contractShipmentDef.Customer.CustomerType.CustomerTypeId == CustomerTypeRef.Type.lipsy.GetHashCode() ||
                    contractShipmentDef.Customer.CustomerType.CustomerTypeId == CustomerTypeRef.Type.daysgroup.GetHashCode()))
                {
                    //show checkbox to enable edit mulitple shipment, for lipsy and daysgroup only
                    ckb.Visible = true;
                    ckb.Disabled = false;
                    ckb.Checked = true;
                }
                else if (contractShipmentDef.WorkflowStatus.Id == ContractWFS.CANCELLED.Id ||
                    contractShipmentDef.WorkflowStatus.Id == ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Id ||
                    contractShipmentDef.WorkflowStatus.Id == ContractWFS.REJECTED.Id)
                {
                    e.Row.BackColor = System.Drawing.Color.FromArgb(187, 187, 187);
                }
                else if (contractShipmentDef.WorkflowStatus.Id == ContractWFS.ENQUIRY.Id ||
                    contractShipmentDef.WorkflowStatus.Id == ContractWFS.AMEND.Id ||
                    contractShipmentDef.WorkflowStatus.Id == ContractWFS.PENDING_FOR_APPROVAL.Id)
                {
                    e.Row.BackColor = System.Drawing.Color.PowderBlue;//.FromArgb(155, 188, 252);
                }


                //113 227 130
                Image img;
                // Read DMS Doc.
                img = (Image)e.Row.FindControl("img_ReadDmsDoc");
                img.Visible = true;
                if (contractShipmentDef.IsUploadDMSDocument && !isAuditViewer)
                    //img.Attributes.Add("onclick", openPopupWindowFunction+"('../account/AttachmentList.aspx?ShipmentId=" + encryptParameter(contractShipmentDef.ShipmentId.ToString()) + "', 'attachmentlist', 'width=400,height=300,scrollbars=1,status=0');return false;");
                    img.Attributes.Add("onclick", openPopupWindowFunction + "('../account/AttachmentList.aspx?ShipmentId=" + WebUtil.EncryptParameter(contractShipmentDef.ShipmentId.ToString()) + "', 'attachmentlist', 'width=400,height=300,scrollbars=1,status=0');return false;");
                else
                    img.Visible = false;


                // Legend
                //img = (Image)e.Row.Cells[3].Controls[1]; //img_dualSourcing
                img = (Image)e.Row.FindControl("img_dualSourcing");
                if (contractShipmentDef.IsDualSourcingOrder == 1)
                    img.Visible = true;

                //img = (Image)e.Row.Cells[3].Controls[2]; //img_SZOrder
                img = (Image)e.Row.FindControl("img_SZOrder");
                if (contractShipmentDef.IsNextMfgOrder == 1)
                    img.Visible = true;

                //img = (Image)e.Row.Cells[3].Controls[3]; //img_discount
                img = (Image)e.Row.FindControl("img_discount");
                if (contractShipmentDef.IsUKDiscount == 1)
                    img.Visible = true;

                //img = (Image)e.Row.Cells[3].Controls[4];
                img = (Image)e.Row.FindControl("img_OPRFabric");
                if (contractShipmentDef.WithOPRFabric != 0)
                    img.Visible = true;

                if (contractShipmentDef.WorkflowStatus.Id == ContractWFS.INVOICED.Id && (!contractShipmentDef.EditLock || !contractShipmentDef.PaymentLock))
                {
                    //img = (Image)e.Row.Cells[3].Controls[5]; //-->img_releaseLock
                    img = (Image)e.Row.FindControl("img_releaseLock");
                    img.Visible = true;
                }

                //if (((contractShipmentDef.Customer.CustomerId == 1 || contractShipmentDef.Customer.CustomerId == 2) &&
                //    contractShipmentDef.CustomerDestination.DestinationCode == "CN")
                //    || (contractShipmentDef.Customer.CustomerId == CustomerDef.Id.hempel.GetHashCode())
                //    )
                if (contractShipmentDef.TermOfPurchaseId==TermOfPurchaseRef.Id.FOB_UT.GetHashCode())
                {
                    //img = (Image)e.Row.Cells[3].Controls[6]; //-->img_UTurn
                    img = (Image)e.Row.FindControl("img_UTurn");
                    img.Visible = true;
                }

                if (contractShipmentDef.IsMockShopSample == 1)
                {
                    //img = (Image)e.Row.Cells[3].Controls[7];
                    img = (Image)e.Row.FindControl("img_MockShop");
                    img.Visible = true;
                }

                if (contractShipmentDef.IsPressSample == 1)
                {
                    //img = (Image)e.Row.Cells[3].Controls[8];
                    img = (Image)e.Row.FindControl("img_PressSample");
                    img.Visible = true;
                }

                if (contractShipmentDef.IsStudioSample == 1)
                {
                    img = (Image)e.Row.FindControl("img_StudioSample");
                    img.Visible = true;
                }

                if (contractShipmentDef.IsLDPOrder == 1)
                {
                    //img = (Image)e.Row.Cells[3].Controls[9];
                    img = (Image)e.Row.FindControl("img_LDP");
                    img.Visible = true;
                }

                if (contractShipmentDef.WithQCCharge == 1)
                {
                    //img = (Image)e.Row.Cells[3].Controls[10];
                    img = (Image)e.Row.FindControl("img_WithQCCharge");
                    img.Visible = true;
                }

                ShipmentDef shipment = ShipmentManager.Instance.getShipmentById(contractShipmentDef.ShipmentId);
                if (shipment.IsTradingAirFreight && shipment.TradingAirFreightTypeId == 1)
                {
                    img = (Image)e.Row.FindControl("img_TradingAirFreight");
                    img.Visible = (shipment.IsTradingAirFreight);
                }
                else
                {
                    if (contractShipmentDef.ShipmentMethod.ShipmentMethodId == ShipmentMethodRef.Method.AIR.GetHashCode()
                        || contractShipmentDef.ShipmentMethod.ShipmentMethodId == ShipmentMethodRef.Method.SEAorAIR.GetHashCode()
                        || contractShipmentDef.ShipmentMethod.ShipmentMethodId == ShipmentMethodRef.Method.ECOAIR.GetHashCode())
                    {
                        img = (Image)e.Row.FindControl("img_AirShipment");
                        img.Visible = (shipment.NSLAirFreightPaymentPercent == 0);
                        img = (Image)e.Row.FindControl("img_AirFreightWithPay");
                        img.Visible = (shipment.NSLAirFreightPaymentPercent > 0);
                    }
                }

                if (contractShipmentDef.SpecialOrderTypeId == 1)
                {
                    img = (Image)e.Row.FindControl("img_ReprocessGoods");
                    img.Visible = true;
                }

                if (contractShipmentDef.IsChinaGBTestRequired)
                {
                    img = (Image)e.Row.FindControl("img_GBTestRequired");
                    img.Visible = true;
                    int testResult = GeneralManager.Instance.getChinaGBTestResult(contractShipmentDef.ProductId, contractShipmentDef.Vendor.VendorId);
                    if (testResult == 1)
                    {
                        img = (Image)e.Row.FindControl("img_GBTestPassed");
                        img.Visible = true;
                    }
                    else if (testResult == 0)
                    {
                        img = (Image)e.Row.FindControl("img_GBTestFailedHold");
                        img.Visible = true;
                    }
                    else if (testResult == 2)
                    {
                        img = (Image)e.Row.FindControl("img_GBTestFailedRelease");
                        img.Visible = true;
                    }
                    else if (testResult == 9)
                    {
                        img = (Image)e.Row.FindControl("img_GBTestFailedCannotRelease");
                        img.Visible = true;
                    }
                }

                /*
                if (ShipmentManager.Instance.isViaCambodiaQCC(contractShipmentDef.ShipmentId))
                {
                    img = (Image)e.Row.FindControl("img_QCCInspection");
                    img.Visible = true;
                }
                */
                
                //lbl = (Label) e.Row.Cells[15].Controls[1];
                lbl = (Label)e.Row.FindControl("lbl_InvoiceDate");
                if (lbl.Text == DateTime.MinValue.Date.ToShortDateString())
                    lbl.Text = "";
            }

        }

        protected void InvoiceRowCommand(object sender, GridViewCommandEventArgs arg)
        {
            if (arg.CommandName == "cmdPrint")
            {
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.PrintInvoice);
                Context.Items.Add(ShipmentCommander.Param.shipmentId, Convert.ToInt32(arg.CommandArgument));

                forwardToScreen(null);

            }
        }

        protected void gvInvoiceOnSort(object sender, GridViewSortEventArgs e)
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

            ContractShipmentListJDef.ContractShipmentComparer.CompareType compareType;

            if (sortExpression == "ContractNo")
                compareType = ContractShipmentListJDef.ContractShipmentComparer.CompareType.ContractNo;
            else if (sortExpression == "DeliveryNo")
                compareType = ContractShipmentListJDef.ContractShipmentComparer.CompareType.DlyNo;
            else if (sortExpression == "Supplier")
                compareType = ContractShipmentListJDef.ContractShipmentComparer.CompareType.Supplier;
            else if (sortExpression == "ItemNo")
                compareType = ContractShipmentListJDef.ContractShipmentComparer.CompareType.ItemNo;
            else if (sortExpression == "InvoiceNo")
                compareType = ContractShipmentListJDef.ContractShipmentComparer.CompareType.InvoiceNo;
            else if (sortExpression == "ShippingUser")
                compareType = ContractShipmentListJDef.ContractShipmentComparer.CompareType.ShippingUser;
            else
                compareType = ContractShipmentListJDef.ContractShipmentComparer.CompareType.CustomerDlyDate;

            vwSearchResult.Sort(new ContractShipmentListJDef.ContractShipmentComparer(compareType, sortDirection));
            gvInvoice.DataSource = vwSearchResult;
            gvInvoice.DataBind();
        }

        protected void gvInvoicePageIndexChanged(object sender, GridViewPageEventArgs arg)
        {
            gvInvoice.PageIndex = arg.NewPageIndex;
            gvInvoice.DataSource = vwSearchResult;
            gvInvoice.DataBind();
        }

        #endregion

        #region button events

        protected void btn_Advance_Click(object sender, EventArgs e)
        {
            //this.Form.DefaultButton = this.btnSearch.UniqueID;
            tb_AdvanceSearch.Visible = true;
            tb_QuickSearch.Visible = false;

        }

        protected void btn_Quick_Click(object sender, EventArgs e)
        {
            //this.Form.DefaultButton = this.btn_QuickSearch.UniqueID;
            tb_QuickSearch.Visible = true;
            tb_AdvanceSearch.Visible = false;
        }

        protected void btn_QuickSearch_Click(object sender, EventArgs e)
        {
            pnl_Result.Visible = true;


            if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.SearchForNextMfg))
            {
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.GetShipmentList);

                Context.Items.Add(ShipmentCommander.Param.contractNo, txtContractNo_q.Text);
                Context.Items.Add(ShipmentCommander.Param.invoiceNo, txt_InvoiceNo_q.Text);
                Context.Items.Add(ShipmentCommander.Param.itemNo, txtItemNo_q.Text);
                Context.Items.Add(ShipmentCommander.Param.officeList, userOfficeList);

                forwardToScreen(null);

                ArrayList list = (ArrayList)Context.Items[ShipmentCommander.Param.shipmentList];

                this.vwSearchResult = list;
            }
            else
            {
                string invoicePrefix = GeneralCriteria.ALLSTRING;
                int invoiceSeq = GeneralCriteria.ALL;
                int invoiceYear = GeneralCriteria.ALL;
                if (txt_InvoiceNo_q.Text.Trim() != "")
                {
                    invoicePrefix = WebUtil.getInvoicePrefix(txt_InvoiceNo_q.Text.Trim());
                    invoiceSeq = WebUtil.getInvoiceSeq(txt_InvoiceNo_q.Text.Trim());
                    invoiceYear = WebUtil.getInvoiceYear(txt_InvoiceNo_q.Text.Trim());
                }
                TypeCollector officeList = TypeCollector.createNew(OfficeId.SL.Id);

                search(txtContractNo_q.Text, GeneralCriteria.ALL, invoicePrefix, invoiceSeq, invoiceSeq, invoiceYear, GeneralCriteria.ALLSTRING,
                    3154, GeneralCriteria.ALL, GeneralCriteria.ALLSTRING, GeneralCriteria.ALLSTRING, officeList, DateTime.MinValue, DateTime.MinValue,
                    GeneralCriteria.ALL, GeneralCriteria.ALLSTRING, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue,
                    DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, 0, 0, 0, 0, 0, 0, 0, 0, 0, GeneralCriteria.ALL,
                    GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralCriteria.ALLSTRING, GeneralCriteria.ALL,
                    TypeCollector.Inclusive, TypeCollector.Inclusive);
            }

            gvInvoice.DataSource = this.vwSearchResult;
            gvInvoice.DataBind();

            if (this.vwSearchResult != null)
            {
                if (this.vwSearchResult.Count < 100)
                    lbl_RowCount.Text = string.Format("Total {0} records.", this.vwSearchResult.Count.ToString());
                else
                    lbl_RowCount.Text = "There are more than 100 shipment matching your search criteria.<br />" +
                        "Only the first 100 search result are shown.";
            }
            btn_Print.Visible = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.PrintInvoice);
            btn_Edit.Visible = false;
            btn_Send.Visible = false;
        }


        protected void btn_QuickReset_Click(object sender, EventArgs e)
        {
            txt_InvoiceNo_q.Text = "";
            txtContractNo_q.Text = "";
            txtItemNo_q.Text = "";

            pnl_Result.Visible = false;
            vwSearchResult = null;
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtContractNo.Text = "";
            txtDeliveryNo.Text = "";
            txtItemNo.Text = "";
            txtNSLInvoiceNoFrom.Text = "";
            txtNSLInvoiceNoTo.Text = "";
            txt_FiscalYear.Text = "";
            ddl_Customer.SelectedIndex = 0;
            txtSupplierInvoiceNoFrom.Text = "";
            txtSupplierInvoiceNoTo.Text = "";
            ddl_FinalDestination.SelectedIndex = 0;
            txtInvoiceDateFrom.Text = "";
            txtInvoiceDateTo.Text = "";
            txtAtWHDateFrom.Text = "";
            txtAtWHDateTo.Text = "";
            txt_ILSInWHDateFrom.Text = "";
            txt_ILSInWHDateTo.Text = "";
            ddl_OPRType.SelectedIndex = 0;
            txtInvoiceUploadDateFrom.Text = "";
            txtInvoiceUploadDateTo.Text = "";
            ddl_Office.SelectedIndex = 0;
            ddl_OrderType.SelectedIndex = 0;
            ddl_TermOfPurchase.SelectedIndex = 0;
            ddl_CO.SelectedIndex = 0;
            txt_DocNo.Text = "";
            // ddl_WorkflowStatus.SelectedIndex = 0;
            ckbAir.Checked = true;
            ckbEcoAir.Checked = true;
            ckbSea.Checked = true;
            ckbSeaAir.Checked = true;
            ckbTruck.Checked = true;
            ckbSplitContractOnly.Checked = false;

            ckb_Status_Enquiry.Checked = true;
            ckb_Status_Draft.Checked = true;
            ckb_Status_pend4Appraval.Checked = true;
            ckb_Status_Pend4Cancel.Checked = true;
            ckb_Status_AMENDED.Checked = true;
            ckb_Status_Reject.Checked = true;
            ckb_Status_Approved.Checked = true;
            ckb_Status_POGen.Checked = true;
            ckb_Status_Invoiced.Checked = true;
            ckb_Status_Cancelled.Checked = true;

            txt_SupplierName.clear();
            uclProductTeam.clear();

            //txt_DefaultDocReceiptDate.Text = "";

            pnl_Result.Visible = false;
            vwSearchResult = null;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            bool isNextMfgUser = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.SearchForNextMfg);

            pnl_Result.Visible = true;
            string invoicePrefix = GeneralCriteria.ALLSTRING;
            int invoiceSeqFrom = GeneralCriteria.ALL;
            int invoiceSeqTo = GeneralCriteria.ALL;
            int invoiceYear = GeneralCriteria.ALL;
            int vendorId = GeneralCriteria.ALL;
            string supplierInvoiceNoFrom = GeneralCriteria.ALLSTRING;
            string supplierInvoiceNoTo = GeneralCriteria.ALLSTRING;
            DateTime invoiceDateFrom, invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo,
                customerAtWHDateFrom, customerAtWHDateTo, ilsAtWHDateFrom, ilsAtWHDateTo,
                invoiceSentDateFrom, invoiceSentDateTo;
            invoiceDateFrom = invoiceDateTo = invoiceUploadDateFrom = invoiceUploadDateTo =
                customerAtWHDateFrom = customerAtWHDateTo = ilsAtWHDateFrom = ilsAtWHDateTo =
                invoiceSentDateFrom = invoiceSentDateTo = DateTime.MinValue;

            if (txtNSLInvoiceNoFrom.Text.Trim() != "")
            {
                invoicePrefix = WebUtil.getInvoicePrefix(txtNSLInvoiceNoFrom.Text.Trim());
                invoiceSeqFrom = WebUtil.getInvoiceSeq(txtNSLInvoiceNoFrom.Text.Trim());
                invoiceSeqTo = WebUtil.getInvoiceSeq(txtNSLInvoiceNoTo.Text.Trim());
                invoiceYear = Convert.ToInt32(txt_FiscalYear.Text.Trim());
            }

            if (txtSupplierInvoiceNoFrom.Text.Trim() != "")
            {
                supplierInvoiceNoFrom = txtSupplierInvoiceNoFrom.Text.Trim();
                supplierInvoiceNoTo = txtSupplierInvoiceNoTo.Text.Trim();
            }

            TypeCollector officeList = TypeCollector.Inclusive;
            if (isNextMfgUser)
            {
                vendorId = 3154;
                officeList.append(OfficeId.SL.Id);
            }
            else
            {
                vendorId = txt_SupplierName.VendorId == int.MinValue ? GeneralCriteria.ALL : txt_SupplierName.VendorId;
                if (ddl_Office.SelectedValue == GeneralCriteria.ALL.ToString())
                {
                    foreach (OfficeRef office in userOfficeList)
                    {
                        officeList.append(office.OfficeId);
                    }
                }
                else
                {
                    officeList.append(int.Parse(ddl_Office.SelectedValue));
                }
            }

            if (txtInvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = Convert.ToDateTime(txtInvoiceDateFrom.Text.Trim());
                if (txtInvoiceDateTo.Text.Trim() == "")
                    invoiceDateTo = invoiceDateFrom;
                else
                    invoiceDateTo = Convert.ToDateTime(txtInvoiceDateTo.Text.Trim());
            }

            if (txtInvoiceUploadDateFrom.Text.Trim() != "")
            {
                invoiceUploadDateFrom = Convert.ToDateTime(txtInvoiceUploadDateFrom.Text.Trim());
                if (txtInvoiceUploadDateTo.Text.Trim() == "")
                    invoiceUploadDateTo = invoiceUploadDateFrom;
                else
                    invoiceUploadDateTo = Convert.ToDateTime(txtInvoiceUploadDateTo.Text.Trim());
            }

            if (txtAtWHDateFrom.Text.Trim() != "")
            {
                customerAtWHDateFrom = Convert.ToDateTime(txtAtWHDateFrom.Text);
                if (txtAtWHDateTo.Text.Trim() == "")
                    customerAtWHDateTo = customerAtWHDateFrom;
                else
                    customerAtWHDateTo = Convert.ToDateTime(txtAtWHDateTo.Text);
            }

            if (txt_ILSInWHDateFrom.Text.Trim() != "")
            {
                ilsAtWHDateFrom = Convert.ToDateTime(txt_ILSInWHDateFrom.Text);
                if (txt_ILSInWHDateTo.Text.Trim() == "")
                    ilsAtWHDateTo = ilsAtWHDateFrom;
                else
                    ilsAtWHDateTo = Convert.ToDateTime(txt_ILSInWHDateTo.Text);
            }

            if (txt_InvoiceSentDateFrom.Text.Trim() != "")
            {
                invoiceSentDateFrom = Convert.ToDateTime(txt_InvoiceSentDateFrom.Text);
                if (txt_InvoiceSentDateTo.Text.Trim() == "")
                    invoiceSentDateTo = invoiceSentDateFrom;
                else
                    invoiceSentDateTo = Convert.ToDateTime(txt_InvoiceSentDateTo.Text.Trim());
            }
            TypeCollector shipmentMethodList = TypeCollector.Inclusive;
            if (ckbAir.Checked)
                shipmentMethodList.append(ShipmentMethodRef.Method.AIR.GetHashCode());
            if (ckbSea.Checked)
                shipmentMethodList.append(ShipmentMethodRef.Method.SEA.GetHashCode());
            if (ckbSeaAir.Checked)
                shipmentMethodList.append(ShipmentMethodRef.Method.SEAorAIR.GetHashCode());
            if (ckbEcoAir.Checked)
                shipmentMethodList.append(ShipmentMethodRef.Method.ECOAIR.GetHashCode());
            if (ckbTruck.Checked)
                shipmentMethodList.append(ShipmentMethodRef.Method.TRUCK.GetHashCode());

            TypeCollector workflowStatusList = TypeCollector.Inclusive;
            if (ckb_Status_Enquiry.Checked)
                workflowStatusList.append(ContractWFS.ENQUIRY.Id);
            if (ckb_Status_Draft.Checked)
                workflowStatusList.append(ContractWFS.PENDING_FOR_SUBMIT.Id);
            if (ckb_Status_pend4Appraval.Checked)
                workflowStatusList.append(ContractWFS.PENDING_FOR_APPROVAL.Id);
            if (ckb_Status_Pend4Cancel.Checked)
                workflowStatusList.append(ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Id);
            if (ckb_Status_AMENDED.Checked)
                workflowStatusList.append(ContractWFS.AMEND.Id);
            if (ckb_Status_Reject.Checked)
                workflowStatusList.append(ContractWFS.REJECTED.Id);
            if (ckb_Status_Approved.Checked)
                workflowStatusList.append(ContractWFS.APPROVED.Id);
            if (ckb_Status_POGen.Checked)
                workflowStatusList.append(ContractWFS.PO_PRINTED.Id);
            if (ckb_Status_Invoiced.Checked)
                workflowStatusList.append(ContractWFS.INVOICED.Id);
            if (ckb_Status_Cancelled.Checked)
                workflowStatusList.append(ContractWFS.CANCELLED.Id);

            search(txtContractNo.Text.Trim(),
                txtDeliveryNo.Text.Trim() == "" ? GeneralCriteria.ALL : Convert.ToInt32(txtDeliveryNo.Text),
                invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear,
                txtItemNo.Text.Trim(),
                vendorId,
                Convert.ToInt32(ddl_Customer.SelectedValue),
                supplierInvoiceNoFrom, supplierInvoiceNoTo,
                officeList,
                invoiceDateFrom, invoiceDateTo,
                uclProductTeam.ProductCodeId == int.MinValue ? GeneralCriteria.ALL : uclProductTeam.ProductCodeId,
                //ddl_OrderType.SelectedValue == "-1" ? GeneralCriteria.ALLSTRING : ddl_OrderType.SelectedValue,
                ddl_OrderType.SelectedValue == "-1" ? GeneralCriteria.ALLSTRING : (ddl_OrderType.SelectedValue == "F" ? "FX" : ddl_OrderType.SelectedValue),        // F -> FOB + FOB_UT
                invoiceUploadDateFrom, invoiceUploadDateTo,
                customerAtWHDateFrom, customerAtWHDateTo,
                ilsAtWHDateFrom, ilsAtWHDateTo,
                invoiceSentDateFrom, invoiceSentDateTo,
                ckbSplitContractOnly.Checked ? 1 : 0,
                ckb_IsSZOrder.Checked ? 1 : 0,
                ckb_IsSample.Checked ? 1 : 0,
                ckb_LDP.Checked ? 1 : 0,
                ckb_QCCharge.Checked ? 1 : 0,
                ckb_ReprocessGoods.Checked ? 1 : 0,
                ckb_GBTest.Checked ? 1 : 0,

                //ckb_QCCInspection.Checked ? 1 : 0,
                0,
                /* TODO:TRADINGAF*/
                ckb_IsTradingAF.Checked ? 1 : 0,
                
                //0,



                ddl_ShippingUser.SelectedValue == "" ? GeneralCriteria.ALL : Convert.ToInt32(ddl_ShippingUser.SelectedValue),
                Convert.ToInt32(ddl_OPRType.SelectedValue),
                Convert.ToInt32(ddl_TermOfPurchase.SelectedValue),
                Convert.ToInt32(ddl_FinalDestination.SelectedValue),
                txt_DocNo.Text.Trim(),
                Convert.ToInt32(ddl_CO.SelectedValue),
                shipmentMethodList,
                workflowStatusList);

            gvInvoice.DataSource = this.vwSearchResult;
            gvInvoice.DataBind();

            if (this.vwSearchResult != null)
            {
                if (this.vwSearchResult.Count < 100)
                    lbl_RowCount.Text = string.Format("Total {0} records.", this.vwSearchResult.Count.ToString());
                else
                    lbl_RowCount.Text = "There are more than 100 shipment matching your search criteria.<br />" +
                        "Only the first 100 search result are shown.";
            }
            btn_Print.Visible = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.PrintInvoice);

            //display the edit button for generating invoice for multiple shipment
            if (ddl_Customer.SelectedValue == "9" || ddl_Customer.SelectedValue == "11")
            {
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.GenInvoiceForMultiShipment))
                    btn_Edit.Visible = true;
                else
                    btn_Edit.Visible = false;
                btn_Send.Visible = false;
            }
            else if (ddl_Customer.SelectedValue == "13")
            {
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.SendEzibuyInvoice))
                    btn_Send.Visible = true;
                else
                    btn_Send.Visible = false;
                btn_Edit.Visible = false;
            }
            else
            {
                btn_Edit.Visible = false;
                btn_Send.Visible = false;
            }

        }

        protected void search(string contractNo, int deliveryNo, string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear,
            string itemNo, int vendorId, int customerId, string supplierInvoiceNoFrom, string supplierInvoiceNoTo, TypeCollector officeList,
            DateTime invoiceDateFrom, DateTime invoiceDateTo, int productTeamId, string orderType, DateTime invoiceUploadDateFrom,
            DateTime invoiceUploadDateTo, DateTime customerAtWHDateFrom, DateTime CustomerAtWHDateTo, DateTime ilsAtWHDateFrom,
            DateTime ilsAtWHDateTo, DateTime invoiceSentDateFrom, DateTime invoiceSentDateTo, int splitOnly, int szOrderOnly, int sampleOnly,
            int ldpOnly, int withQCCCharge, int isReprocessGoods, int isGBTestRequired, int isQCCInspection, int isTradingAF, int invoiceUploadUserId, int oprTypeId, int termOfPurchaseId, int customerDestinationId, string docNo,
            int countryOfOriginId, TypeCollector shipmentMethodList, TypeCollector workflowStatusList)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.GetShipmentListForAdvancedSearch);

            Context.Items.Add(ShipmentCommander.Param.contractNo, contractNo);
            Context.Items.Add(ShipmentCommander.Param.deliveryNo, deliveryNo);

            Context.Items.Add(ShipmentCommander.Param.invoicePrefix, invoicePrefix);
            Context.Items.Add(ShipmentCommander.Param.invoiceSeqFrom, invoiceSeqFrom);
            Context.Items.Add(ShipmentCommander.Param.invoiceSeqTo, invoiceSeqTo);
            Context.Items.Add(ShipmentCommander.Param.invoiceYear, invoiceYear);

            Context.Items.Add(ShipmentCommander.Param.itemNo, itemNo);
            Context.Items.Add(ShipmentCommander.Param.vendorId, vendorId);
            Context.Items.Add(ShipmentCommander.Param.customerId, customerId);

            Context.Items.Add(ShipmentCommander.Param.supplierInvoiceNoFrom, supplierInvoiceNoFrom);
            Context.Items.Add(ShipmentCommander.Param.supplierInvoiceNoTo, supplierInvoiceNoTo);

            Context.Items.Add(ShipmentCommander.Param.officeList, officeList);

            Context.Items.Add(ShipmentCommander.Param.invoiceDateFrom, invoiceDateFrom);
            Context.Items.Add(ShipmentCommander.Param.invoiceDateTo, invoiceDateTo);

            Context.Items.Add(ShipmentCommander.Param.productTeamId, productTeamId);
            Context.Items.Add(ShipmentCommander.Param.orderType, orderType);

            Context.Items.Add(ShipmentCommander.Param.invoiceUploadDateFrom, invoiceUploadDateFrom);
            Context.Items.Add(ShipmentCommander.Param.invoiceUploadDateTo, invoiceUploadDateTo);

            Context.Items.Add(ShipmentCommander.Param.customerAgreedAtWHDateFrom, customerAtWHDateFrom);
            Context.Items.Add(ShipmentCommander.Param.customerAgreedAtWHDateTo, CustomerAtWHDateTo);

            Context.Items.Add(ShipmentCommander.Param.ILSActualAtWHDateFrom, ilsAtWHDateFrom);
            Context.Items.Add(ShipmentCommander.Param.ILSActualAtWHDateTo, ilsAtWHDateTo);

            Context.Items.Add(ShipmentCommander.Param.invoiceSentDateFrom, invoiceSentDateFrom);
            Context.Items.Add(ShipmentCommander.Param.invoiceSentDateTo, invoiceSentDateTo);

            Context.Items.Add(ShipmentCommander.Param.splitOnly, splitOnly);
            Context.Items.Add(ShipmentCommander.Param.szOrderOnly, szOrderOnly);
            Context.Items.Add(ShipmentCommander.Param.sampleOnly, sampleOnly);
            Context.Items.Add(ShipmentCommander.Param.ldpOrder, ldpOnly);
            Context.Items.Add(ShipmentCommander.Param.withQCCharge, withQCCCharge);
            Context.Items.Add(ShipmentCommander.Param.isReprocessGoods, isReprocessGoods);
            Context.Items.Add(ShipmentCommander.Param.isChinaGBTestRequired, isGBTestRequired);
            //Context.Items.Add(ShipmentCommander.Param.isQccInspection, isQCCInspection);
            Context.Items.Add(ShipmentCommander.Param.isTradingAirFreight, isTradingAF);

            Context.Items.Add(ShipmentCommander.Param.invoiceUploadUserId, invoiceUploadUserId);

            Context.Items.Add(ShipmentCommander.Param.oprTypeId, oprTypeId);
            Context.Items.Add(ShipmentCommander.Param.termOfPurchaseId, termOfPurchaseId);
            Context.Items.Add(ShipmentCommander.Param.customerDestinationId, customerDestinationId);
            Context.Items.Add(ShipmentCommander.Param.docNo, docNo);
            Context.Items.Add(ShipmentCommander.Param.countryOfOriginId, countryOfOriginId);

            Context.Items.Add(ShipmentCommander.Param.shipmentMethodList, shipmentMethodList);
            Context.Items.Add(ShipmentCommander.Param.workflowStatusList, workflowStatusList);

            forwardToScreen(null);
            ArrayList list = (ArrayList)Context.Items[ShipmentCommander.Param.shipmentList];

            this.vwSearchResult = list;
        }

        protected void btn_Print_Click(object sender, EventArgs e)
        {
            HtmlInputCheckBox ckb = null;
            ContractShipmentListJDef def;
            ArrayList shipmentIdList = new ArrayList();

            foreach (GridViewRow row in gvInvoice.Rows)
            {
                ckb = (HtmlInputCheckBox)row.Cells[0].FindControl("ckb_Print");
                if (ckb != null && ckb.Checked)
                {
                    def = (ContractShipmentListJDef)vwSearchResult[row.RowIndex];
                    if (def.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
                    {
                        if (def.Customer.CustomerType.CustomerTypeId != CustomerTypeRef.Type.daysgroup.GetHashCode() &&
                            def.Customer.CustomerType.CustomerTypeId != CustomerTypeRef.Type.lipsy.GetHashCode())
                        {
                            shipmentIdList.Add(def.ShipmentId);
                        }
                        else if ((def.ShippingUser != null && def.ShippingUser.UserId == this.LogonUserId &&
                            CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.PrintInvoice)) ||
                        CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.searchEngine.Id, ISAMModule.searchEngine.PrintAllInvoice))
                        {
                            shipmentIdList.Add(def.ShipmentId);
                        }
                    }
                }
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.PrintInvoice);
            Context.Items.Add(ShipmentCommander.Param.shipmentList, shipmentIdList);

            forwardToScreen(null);
        }

        protected void btn_Send_Click(object sender, EventArgs e)
        {
            HtmlInputCheckBox ckb = null;
            ContractShipmentListJDef def;
            ArrayList shipmentIdList = new ArrayList();

            foreach (GridViewRow row in gvInvoice.Rows)
            {
                ckb = (HtmlInputCheckBox)row.Cells[0].FindControl("ckb_Print");
                if (ckb != null && ckb.Checked)
                {
                    def = (ContractShipmentListJDef)vwSearchResult[row.RowIndex];
                    if (def.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
                    {
                        if (def.Customer.CustomerType.CustomerTypeId == CustomerTypeRef.Type.ezibuy.GetHashCode())
                        {
                            shipmentIdList.Add(def.ShipmentId);
                        }
                    }
                }
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.SendEzibuyInvoice);
            Context.Items.Add(ShipmentCommander.Param.shipmentList, shipmentIdList);

            forwardToScreen(null);

        }

        protected void btn_Edit_Click(object sender, EventArgs e)
        {
            HtmlInputCheckBox ckb = null;
            ContractShipmentListJDef def;
            string shipmentIdList = "";
            int currencyId = -1;

            foreach (GridViewRow row in gvInvoice.Rows)
            {
                ckb = (HtmlInputCheckBox)row.Cells[0].FindControl("ckb_Print");
                if (ckb != null && ckb.Checked)
                {
                    def = (ContractShipmentListJDef)vwSearchResult[row.RowIndex];
                    if (currencyId != -1 && currencyId != def.SellCurrency.CurrencyId)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ccy",
                            "alert('For editing multiple shipment, all selected shipment must have the same currency.');", true);
                        break;
                    }
                    currencyId = def.SellCurrency.CurrencyId;
                    shipmentIdList += shipmentIdList == "" ? def.ShipmentId.ToString() : "|" + def.ShipmentId.ToString();

                }
            }

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "...", "EditMultipleShipment('ShipmentId=" + encryptParameter(shipmentIdList) + "')", true);

        }

        #endregion
        protected void ddl_OrderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_TermOfPurchase.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());

            if (ddl_OrderType.SelectedValue == "F")
            {
                ddl_TermOfPurchase.Items.Remove(ddl_TermOfPurchase.Items.FindByValue(TermOfPurchaseRef.Id.CM.GetHashCode().ToString()));
                ddl_TermOfPurchase.Items.Remove(ddl_TermOfPurchase.Items.FindByValue(TermOfPurchaseRef.Id.CMT.GetHashCode().ToString()));
                ddl_TermOfPurchase.Items.Remove(ddl_TermOfPurchase.Items.FindByValue(TermOfPurchaseRef.Id.VMTrading.GetHashCode().ToString()));
            }
            else if (ddl_OrderType.SelectedValue == "V")
            {
                ddl_TermOfPurchase.Items.Remove(ddl_TermOfPurchase.Items.FindByValue(TermOfPurchaseRef.Id.FOB.GetHashCode().ToString()));
                ddl_TermOfPurchase.Items.Remove(ddl_TermOfPurchase.Items.FindByValue(TermOfPurchaseRef.Id.FOB_UT.GetHashCode().ToString()));
            }
        }

        protected bool isNextMfgUser(int userId)
        {
            UserRef user = CommonUtil.getUserByKey(userId);
            return user.EmailAddress.Contains("nextmfg");
        }

     }
}
