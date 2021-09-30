using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.shipping;
using com.next.common.domain;
using com.next.common.domain.module;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.isam.reporter.shipping;
using com.next.isam.reporter.helper;
using com.next.isam.domain.types;
using com.next.infra.util;
using com.next.common.datafactory.worker.industry;
using com.next.common.datafactory.worker;
using com.next.isam.dataserver.worker;


namespace com.next.isam.webapp.shipping
{
    public partial class Alert : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private bool HasAccessRightsTo_SuperView;
        private bool HasAccessRightsTo_InvUploadFail;
        private bool HasAccessRightsTo_MissingAdvisingBank;
        private bool HasAccessRightsTo_OutstandingBooking;
        private bool HasAccessRightsTo_OutstandingDocument;
        private bool HasAccessRightsTo_OutstandingDocumentOffshore;
        private bool HasAccessRightsTo_OutstandingResubmitDocument;
        private bool HasAccessRightsTo_OutstandingUTOrder;
        private bool HasAccessRightsTo_MissingSunAccountCode;
        private bool HasAccessRightsTo_MissingEpicorSupplierId;
        private bool HasAccessRightsTo_MissingPaymentAdviceEMail;
        private bool HasAccessRightsTo_EziBuyOSPaymentList;
        private bool HasAccessRightsTo_OutstandingNTInvoiceDepartmentApproval;
        private bool HasAccessRightsTo_NTInvoiceAccountsApproved;
        private bool HasAccessRightsTo_OutstandingNTInvoiceAccountLevel1Approval;
        private bool HasAccessRightsTo_OutstandingNTInvoiceAccountLevel2Approval;
        private bool HasAccessRightsTo_NewNTVendorApproval;
        private bool HasAccessRightsTo_NTVendorAmendment;

        private static TypeCollector SelectedOfficeId;
        private static string SelectedDepartmentCode;
        private static int AlertUserId;
        private static ArrayList DepartmentIdList = new ArrayList();
        private static TypeCollector SelectedDepartmentId;
        private static string CountMode = "";
        private static char Delimiter = AlertNotificationWorker.Instance.Delimiter;

        const int   InvUploadFail = 0;
        const int   AdvisingBank = 1;
        const int   OSUTOrder = 2;
        const int   OSBooking = 3;
        const int   OSDoc = 4;
        const int   OSDocOffshore = 5;
        const int   OSResubmitDoc = 6;
        const int   SunAccCode = 7;
        const int   EAdvice = 8;
        const int   EziBuyOSPayment = 9;
        const int   OSNTInvDeptApv = 10;
        const int   OSNTInvAccL1Apv = 11;
        const int   OSNTInvAccL2Apv = 12;
        const int   NewNTVendorApv = 13;
        const int   NTVendorAmendment = 14;
        const int   TotalAlertCount = 15;
        const int   EpicorSupplierId = 16;
        bool isDebuger = false;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            int i;
            //isDebuger = (this.LogonUserId == 574 || this.LogonUserId == 122 || this.LogonUserId == 246);   // Shipping & Accounts user
            //isDebuger = (this.LogonUserId == 574) || (this.LogonUserId == 1098) || (this.LogonUserId == 380);
            isDebuger = (this.LogonUserId == 574);

            DateTime startTime = DateTime.Now;
            HasAccessRightsTo_SuperView = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.SuperView);
            HasAccessRightsTo_InvUploadFail = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.InvoiceUploadFail);
            HasAccessRightsTo_MissingAdvisingBank = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.MissingAdvisingBank);
            HasAccessRightsTo_OutstandingBooking = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.OutstandingBooking);
            HasAccessRightsTo_OutstandingDocument = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.OutstandingDocument);
            HasAccessRightsTo_OutstandingDocumentOffshore = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.OutstandingDocumentOffshore);
            HasAccessRightsTo_OutstandingResubmitDocument = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.OutstandingResubmitDocument);
            HasAccessRightsTo_OutstandingUTOrder = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.OutstandingUTOrder);
            //HasAccessRightsTo_MissingSunAccountCode = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.MissingSunAccountCode);
            HasAccessRightsTo_MissingSunAccountCode = false;
            HasAccessRightsTo_MissingEpicorSupplierId = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.MissingSunAccountCode);
            HasAccessRightsTo_MissingPaymentAdviceEMail = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.MissingPaymentAdviceEMail);
            HasAccessRightsTo_EziBuyOSPaymentList = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.EziBuyOSPaymentList);
            HasAccessRightsTo_OutstandingNTInvoiceDepartmentApproval = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.OutstandingNTInvoiceDeptApproval);
            HasAccessRightsTo_OutstandingNTInvoiceAccountLevel1Approval = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.OutstandingNTInvoiceAccountLevel1Approval);
            HasAccessRightsTo_OutstandingNTInvoiceAccountLevel2Approval = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.OutstandingNTInvoiceAccountLevel2Approval);
            HasAccessRightsTo_NTInvoiceAccountsApproved = HasAccessRightsTo_OutstandingNTInvoiceAccountLevel1Approval;
            HasAccessRightsTo_NewNTVendorApproval = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.NewNTVendorApproval);
            HasAccessRightsTo_NTVendorAmendment = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.alertAndNotification.Id, ISAMModule.alertAndNotification.NTVendorAmendment);
            startTime = debugShowTime(startTime, isDebuger, "PageLoad - Get Access Rights");

            //********** For Testing **************************
            //if (this.LogonUserId==574)
            //  HasAccessRightsTo_SuperView = true;
            // HasAccessRightsTo_MissingSunAccountCode = true;
            //********** For Testing **************************
            AlertUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);

            if (!IsPostBack)
            {
                this.ddl_Office.bindList(CommonUtil.getOfficeListByUserId(AlertUserId, GeneralCriteria.ALL), "Description", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
                startTime = debugShowTime(startTime, isDebuger, "PageLoad - Get User Office List");

                for (i = 0; i < this.ddl_Office.Items.Count; i++)
                    this.ddl_Office.Items[i].Text = this.ddl_Office.Items[i].Text.Replace("Office", "").Trim();
                if (this.ddl_Office.Items.Count == 2) this.ddl_Office.SelectedIndex = 1;

                //this.ddl_Department.bindList(GetGeneralDepartmentList(AlertUserId), "Description", "Code", "", "--All--", "");
                //this.ddl_Department.bindList(AlertNotificationWorker.Instance.getProductDepartmentList(AlertUserId), "Description", "Code", "", "--All--", "");
                ArrayList allDept = AlertNotificationWorker.Instance.getProductDepartmentList(AlertUserId);
                startTime = debugShowTime(startTime, isDebuger, "PageLoad - Get Product Department List");

                string deptCode = "";
                int idx = ddl_Department.Items.Count;
                DepartmentIdList.Add(TypeCollector.Inclusive);
                if (allDept != null)
                    foreach (ProductDepartmentRef rf in allDept)
                    {
                        if (deptCode != rf.Code)
                        {
                            idx = ddl_Department.Items.Count;
                            deptCode = rf.Code;
                            DepartmentIdList.Add(TypeCollector.Inclusive);

                            ListItem itm = new ListItem();
                            itm.Value = rf.Code;
                            itm.Text = rf.Description;
                            ddl_Department.Items.Add(itm);
                        }
                        ((TypeCollector)DepartmentIdList[idx]).append(rf.ProductDepartmentId);
                    }
                if (this.ddl_Department.Items.Count == 2) this.ddl_Department.SelectedIndex = 1;
                SelectedDepartmentId = (TypeCollector)DepartmentIdList[ddl_Department.SelectedIndex];

                // Set alert filter
                SelectedOfficeId = TypeCollector.Inclusive;
                for (i = 1; i < this.ddl_Office.Items.Count; i++)
                    if (this.ddl_Office.SelectedValue == this.ddl_Office.Items[i].Value || this.ddl_Office.SelectedValue == GeneralCriteria.ALL.ToString())
                        SelectedOfficeId.append(int.Parse(this.ddl_Office.Items[i].Value));
                SelectedDepartmentCode = this.ddl_Department.SelectedValue;

                //vwAlertInvUploadFail = new ArrayList();

                LoadAllAlertCount();
            }
            else
            {
                // Set alert filter
                SelectedOfficeId = TypeCollector.Inclusive;
                for (i = 1; i < this.ddl_Office.Items.Count; i++)
                    if (this.ddl_Office.SelectedValue == this.ddl_Office.Items[i].Value || this.ddl_Office.SelectedValue == GeneralCriteria.ALL.ToString())
                        SelectedOfficeId.append(int.Parse(this.ddl_Office.Items[i].Value));
                SelectedDepartmentCode = this.ddl_Department.SelectedValue;
                SelectedDepartmentId = (TypeCollector)DepartmentIdList[ddl_Department.SelectedIndex];
            }
        }

        /*
        public ArrayList GetGeneralDepartmentList(int userId)
        {
            string[] DepartmentName;
            char[] Seperator = { '(', ')' };
            bool Exist;
            int i, Diff;

            ArrayList DepartmentList = new ArrayList();
            ArrayList UserDepartmentList = new ArrayList();
            UserDepartmentList = CommonUtil.getProductDepartmentAllByUserId(userId);

            //Build the Department array order by the deparment name
            foreach (ProductDepartmentRef rf in UserDepartmentList)
            {
                DepartmentName = rf.Description.Split(Seperator);
                Exist = false;
                i = 0;
                if (DepartmentList.Count > 0)
                    for (i = 0; i < DepartmentList.Count && !Exist; i++)
                    {
                        Diff = string.Compare(((ProductDepartmentRef)DepartmentList[i]).Description, DepartmentName[0]);
                        if (Diff > 0)
                            break;  //Department code does not exist in DepartmentList
                        else
                            if (Diff == 0)
                                Exist = (((ProductDepartmentRef)DepartmentList[i]).Code == rf.Code);
                    }

                if (!Exist)
                {
                    rf.Description = DepartmentName[0];
                    rf.OfficeId = GeneralCriteria.ALL;
                    rf.ProductDepartmentId = GeneralCriteria.ALL;
                    DepartmentList.Insert(i, rf);
                }
            }
            return DepartmentList;
        }
        */
       
        #region View State

        ArrayList vwAlertInvUploadFail
        {
            get { return (ArrayList)ViewState["InvoiceUploadFail"]; }
            set { ViewState["InvoiceUploadFail"] = value; }
        }

        ArrayList vwAlertUTOutstandingInvoice
        {
            get { return (ArrayList)ViewState["UTOutstandingInvoice"]; }
            set { ViewState["UTOutstandingInvoice"] = value; }
        }

        ArrayList vwAlertVendorMissingAdvisingBank
        {
            get { return (ArrayList)ViewState["VendorMissingAdvisingBank"]; }
            set { ViewState["VendorMissingAdvisingBank"] = value; }
        }

        ArrayList vwAlertOutstandingBooking
        {
            get { return (ArrayList)ViewState["OutstandingBooking"]; }
            set { ViewState["OutstandingBooking"] = value; }
        }

        ArrayList vwAlertOutstandingDocument
        {
            get { return (ArrayList)ViewState["OutstandingDocument"]; }
            set { ViewState["OutstandingDocument"] = value; }
        }

        ArrayList vwAlertOutstandingDocumentOffshore
        {
            get { return (ArrayList)ViewState["OutstandingDocumentOffshore"]; }
            set { ViewState["OutstandingDocumentOffshore"] = value; }
        }

        ArrayList vwAlertOutstandingResubmitDocument
        {
            get { return (ArrayList)ViewState["OutstandingResubmitDocument"]; }
            set { ViewState["OutstandingResubmitDocument"] = value; }
        }
        /*
        ArrayList vwMissingSunAccountCode
        {
            get { return (ArrayList)ViewState["MissingSunAccountCode"]; }
            set { ViewState["MissingSunAccountCode"] = value; }
        }
        
        ArrayList vwMissingEpicorSupplierId
        {
            get { return (ArrayList)ViewState["MissingEpicorSupplierId"]; }
            set { ViewState["MissingEpicorSupplierId"] = value; }
        }
        
        ArrayList vwMissingPaymentAdviceEMail
        {
            get { return (ArrayList)ViewState["MissingPaymentAdviceEMail"]; }
            set { ViewState["MissingPaymentAdviceEMail"] = value; }
        }
        */
        AlertTable vwMissingSunAccountCode
        {
            get { return (AlertTable)ViewState["MissingSunAccountCode"]; }
            set { ViewState["MissingSunAccountCode"] = value; }
        }
        AlertTable vwMissingEpicorSupplierId
        {
            get { return (AlertTable)ViewState["MissingEpicorSupplierId"]; }
            set { ViewState["MissingEpicorSupplierId"] = value; }
        }

        AlertTable vwMissingPaymentAdviceEMail
        {
            get { return (AlertTable)ViewState["MissingPaymentAdviceEMail"]; }
            set { ViewState["MissingPaymentAdviceEMail"] = value; }
        }

        ArrayList vwEziBuyOSPaymentList
        {
            get { return (ArrayList)ViewState["EziBuyOSPaymentList"]; }
            set { ViewState["EziBuyOSPaymentList"] = value; }
        }

        ArrayList vwAlertOSNTInvoiceForDeptApproval
        {
            get { return (ArrayList)ViewState["OutstandingNTInvoiceForDeptApproval"]; }
            set { ViewState["OutstandingNTInvoiceForDeptApproval"] = value; }
        }

        ArrayList vwAlertNTInvoiceAccountsApproved
        {
            get { return (ArrayList)ViewState["NTInvoiceAccountsApproved"]; }
            set { ViewState["NTInvoiceAccountsApproved"] = value; }
        }

        ArrayList vwAlertOSNTInvoiceForAccLevel1Approval
        {
            get { return (ArrayList)ViewState["OutstandingNTInvoiceForAccountLevel1Approval"]; }
            set { ViewState["OutstandingNTInvoiceForAccountLevel1Approval"] = value; }
        }

        ArrayList vwAlertOSNTInvoiceForAccLevel2Approval
        {
            get { return (ArrayList)ViewState["OutstandingNTInvoiceForAccountLevel2Approval"]; }
            set { ViewState["OutstandingNTInvoiceForAccountLevel2Approval"] = value; }
        }

        ArrayList vwAlertNewNTVendorApproval
        {
            get { return (ArrayList)ViewState["NewNTVendorApproval"]; }
            set { ViewState["NewNTVendorApproval"] = value; }
        }

        ArrayList vwAlertNTVendorAmendment
        {
            get { return (ArrayList)ViewState["AlertNTVendorAmendment"]; }
            set { ViewState["AlertNTVendorAmendment"] = value; }
        }
                
        ArrayList vwAlertNTVendorGroupDetail
        {
            get { return (ArrayList)ViewState["AlertNTVendorGroupDetail"]; }
            set { ViewState["AlertNTVendorGroupDetail"] = value; }
        }

        ArrayList vwAlertNTVendorGroupCount
        {
            get { return (ArrayList)ViewState["AlertNTVendorGroupCount"]; }
            set { ViewState["AlertNTVendorGroupCount"] = value; }
        }

        ArrayList vwAlertVendorGroupDetail
        {
            get { return (ArrayList)ViewState["AlertVendorGroupDetail"]; }
            set { ViewState["AlertVendorGroupDetail"] = value; }
        }

        AlertTable vwAlertGenericVendorGroupDetail
        {
            get { return (AlertTable)ViewState["AlertGenericVendorGroupDetail"]; }
            set { ViewState["AlertGenericVendorGroupDetail"] = value; }
        }

        ArrayList vwAlertVendorGroupCount
        {
            get { return (ArrayList)ViewState["AlertVendorGroupCount"]; }
            set { ViewState["AlertVendorGroupCount"] = value; }
        }

        ArrayList vwAlertNTInvoiceApprovalGroupCount
        {
            get { return (ArrayList)ViewState["AlertNTInvoiceApprovalGroupCount"]; }
            set { ViewState["AlertNTInvoiceApprovalGroupCount"] = value; }
        }

        ArrayList vwAlertNTInvoiceApprovalGroupDetail
        {
            get { return (ArrayList)ViewState["AlertNTInvoiceApprovalGroupDetail"]; }
            set { ViewState["AlertNTInvoiceApprovalGroupDetail"] = value; }
        }

        

        #endregion


        #region Grid Building

        protected void gv_InvUploadFail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertInvUploadFail[e.Row.RowIndex];
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Text = alert.ContractNo;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Attributes.Add("OnClick", "window.open('../Shipping/ShipmentDetail.aspx?ShipmentId=" + alert.ShipmentId.ToString() + "&DefaultReceiptDate=" + HttpUtility.UrlEncode(DateTimeUtility.getDateString(DateTime.Today)) + "', 'ShipmentDetail', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = alert.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_InvNo")).Text = alert.InvoiceNo;
                ((Label)e.Row.FindControl("lbl_InvDate")).Text = DateTimeUtility.getDateString(alert.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_WorkflowStatus")).Text = alert.WorkflowStatus.Name;
                ((Label)e.Row.FindControl("lbl_MerchName")).Text = "<div style='text-align:left;'>" + alert.Merchandiser.DisplayName + "</div>";
                ((Label)e.Row.FindControl("lbl_FailReason")).Text = "<div style='text-align:left;'>"
                                                                        + alert.ILSInvoiceStatus.AlertText
                    //                                                                        + "  <img src='../images/icon_email.gif' onclick='emailUserToFollowUp(" + Convert.ToChar(34) + Alert.ContractNo + "-" + Alert.DeliveryNo + Convert.ToChar(34) + ", " + Convert.ToChar(34) + Alert.InvoiceNo + Convert.ToChar(34) + ", " + Convert.ToChar(34) + Alert.ILSInvoiceStatus.AlertDescription + Convert.ToChar(34) + ");' style='cursor:hand;' title='Send eMail'/>"
                                                                        + "</div>";
            }
        }

        protected void gv_MissingAdvBankVendor_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertVendorMissingAdvisingBank[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Vendor")).Text = "<div style='text-align:left;'>" + alert.Vendor.Name + "</div>";
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Text = alert.ContractNo;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Attributes.Add("OnClick", "window.open('../Shipping/ShipmentDetail.aspx?ShipmentId=" + alert.ShipmentId.ToString() + "&DefaultReceiptDate=" + HttpUtility.UrlEncode(DateTimeUtility.getDateString(DateTime.Today)) + "', 'ShipmentDetail', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_DeliveryNo")).Text = alert.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = alert.Product.ItemNo;
                ((Label)e.Row.FindControl("lbl_CustDlyDate")).Text = DateTimeUtility.getDateString(alert.CustomerAtWarehouseDate);
                ((Label)e.Row.FindControl("lbl_Status")).Text = alert.WorkflowStatus.Name;
                ((Label)e.Row.FindControl("lbl_MerName")).Text = "<div style='text-align:left;'>" + alert.Merchandiser.DisplayName + "</div>";
            }
        }

        protected void gv_UTOrderOSInv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertUTOutstandingInvoice[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_office")).Text = alert.Office.OfficeCode;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Text = alert.ContractNo;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Attributes.Add("OnClick", "window.open('../Shipping/ShipmentDetail.aspx?ShipmentId=" + alert.ShipmentId.ToString() + "&DefaultReceiptDate=" + HttpUtility.UrlEncode(DateTimeUtility.getDateString(DateTime.Today)) + "', 'ShipmentDetail', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = alert.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_STWDate")).Text = DateTimeUtility.getDateString(alert.ActualAtWarehouseDate);
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = alert.Product.ItemNo;
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = "<div style='text-align:left;'>" + alert.Vendor.Name + "</div>";
                ((Label)e.Row.FindControl("lbl_Status")).Text = alert.WorkflowStatus.Name;
            }
        }

        protected void gv_OSBooking_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertOutstandingBooking[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Text = alert.ContractNo;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Attributes.Add("OnClick", "window.open('../Shipping/ShipmentDetail.aspx?ShipmentId=" + alert.ShipmentId.ToString() + "&DefaultReceiptDate=" + HttpUtility.UrlEncode(DateTimeUtility.getDateString(DateTime.Today)) + "', 'ShipmentDetail', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = alert.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = alert.Product.ItemNo;
                ((Label)e.Row.FindControl("lbl_supplier")).Text = "<div style='text-align:left;'>" + alert.Vendor.Name + "</div>";
                ((Label)e.Row.FindControl("lbl_CustDlyDate")).Text = DateTimeUtility.getDateString(alert.CustomerAtWarehouseDate);
                ((Label)e.Row.FindControl("lbl_Status")).Text = alert.WorkflowStatus.Name;
                ((Label)e.Row.FindControl("lbl_ShipUser")).Text = "<div style='text-align:left;'>" + (alert.InvoiceUploadUser == null ? "" : "<div style='text-align:left;'>" + alert.InvoiceUploadUser.DisplayName) + "</div>";
            }
        }

        protected void gv_OSShipDoc_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertOutstandingDocument[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Text = alert.ContractNo;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Attributes.Add("OnClick", "window.open('../Shipping/ShipmentDetail.aspx?ShipmentId=" + alert.ShipmentId.ToString() + "&DefaultReceiptDate=" + HttpUtility.UrlEncode(DateTimeUtility.getDateString(DateTime.Today)) + "', 'ShipmentDetail', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = alert.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = alert.Product.ItemNo;
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = alert.Vendor.Name;
                ((Label)e.Row.FindControl("lbl_DocRcptDate")).Text = DateTimeUtility.getDateString(alert.ShippingDocReceiptDate);
                ((Label)e.Row.FindControl("lbl_ShipUser")).Text = "<div style='text-align:left;'>" + (alert.InvoiceUploadUser == null ? "" : alert.InvoiceUploadUser.DisplayName) + "</div>";
            }
        }

        protected void gv_OSResubmitDoc_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertOutstandingResubmitDocument[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = alert.Vendor.Name;
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Text = alert.ContractNo;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Attributes.Add("OnClick", "window.open('../Shipping/ShipmentDetail.aspx?ShipmentId=" + alert.ShipmentId.ToString() + "&DefaultReceiptDate=" + HttpUtility.UrlEncode(DateTimeUtility.getDateString(DateTime.Today)) + "', 'ShipmentDetail', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = alert.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = alert.Product.ItemNo;
                ((Label)e.Row.FindControl("lbl_InvNo")).Text = alert.InvoiceNo;

                string[] remark = alert.Remark.Split(Delimiter);
                int rejectReasonId = int.Parse(remark[0]);
                string supInvNo = "";
                for (int i = 1; i < remark.Length; i++)
                    supInvNo += (supInvNo == "" ? "" : Delimiter.ToString()) + remark[i];
                ((Label)e.Row.FindControl("lbl_SupInvNo")).Text = supInvNo;
                ((Label)e.Row.FindControl("lbl_RejectReason")).Text = (rejectReasonId == RejectPaymentReason.NoReason.Id ? "" : RejectPaymentReason.getReason(rejectReasonId).Name);
            }
        }

        protected void gv_OSShipDocOffshore_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AlertNotificationRef Alert = (AlertNotificationRef)vwAlertOutstandingDocumentOffshore[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Office")).Text = Alert.Office.OfficeCode;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Text = Alert.ContractNo;
                ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Attributes.Add("OnClick", "window.open('../Shipping/ShipmentDetail.aspx?ShipmentId=" + Alert.ShipmentId.ToString() + "&DefaultReceiptDate=" + HttpUtility.UrlEncode(DateTime.Today.ToString("dd/MM/yyyy")) + "', 'ShipmentDetail', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = Alert.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = Alert.Product.ItemNo;
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = Alert.Vendor.Name;
                ((Label)e.Row.FindControl("lbl_InvoiceNo")).Text = Alert.InvoiceNo;
                ((Label)e.Row.FindControl("lbl_InvoiceDate")).Text = DateTimeUtility.getDateString(Alert.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_SupplierInvoiceNo")).Text = Alert.SupplierInvoiceNo;
                ((Label)e.Row.FindControl("lbl_DocRcptDate")).Text = DateTimeUtility.getDateString(Alert.ShippingDocReceiptDate);
                ((Label)e.Row.FindControl("lbl_ShipUser")).Text = "<div style='text-align:left;'>" + (Alert.InvoiceUploadUser == null ? "" : Alert.InvoiceUploadUser.DisplayName) + "</div>";
                if (!Alert.IsUploadDMSDocument)
                    ((Image)e.Row.FindControl("img_Doc")).Visible = false;
                else
                    ((Image)e.Row.FindControl("img_Doc")).Attributes.Add("onclick", "openAttachments(this, '" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(Alert.ShipmentId.ToString())) + "');return false;");
                Label lbl = ((Label)e.Row.FindControl("lbl_FollowUp"));
                if (Alert.SupplierInvoiceNo == String.Empty)
                    lbl.Text += ((lbl.Text == String.Empty ? String.Empty : "<br>") + "- Input Supplier Invoice No.");
                if (!Alert.IsUploadDMSDocument)
                    lbl.Text += ((lbl.Text == String.Empty ? String.Empty : "<br>") + "- Upload supporting document into DMS.");
                if (Alert.ShippingDocReceiptDate == DateTime.MinValue)
                    lbl.Text += ((lbl.Text == String.Empty ? String.Empty : "<br>") + "- Input Shipping Doc. Receipt Date.");
                if (Alert.ShippingDocCheckedDate == DateTime.MinValue)
                    lbl.Text += ((lbl.Text == String.Empty ? String.Empty : "<br>") + "- Please tick Shipping Doc. Checked Amount checkbox.");
            }
        }

        protected void gv_MissingSunAccCode_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string[] alert = (string[])vwMissingSunAccountCode.Rows[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert[vwMissingSunAccountCode.ColumnId("Office")];
                ((Label)e.Row.FindControl("lbl_PurchaseTerm")).Text = alert[vwMissingSunAccountCode.ColumnId("Purchase Term")];
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = alert[vwMissingSunAccountCode.ColumnId("Vendor Name")];
            }
            
        }

        protected void gv_MissingEpicorSupplierId_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            /*
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AlertNotificationRef alert = (AlertNotificationRef)vwMissingEpicorSupplierId[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((Label)e.Row.FindControl("lbl_PurchaseTerm")).Text = alert.TermOfPurchase.TermOfPurchaseDescription;
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = alert.Vendor.Name;
            }
            */
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string[] alert = (string[])vwMissingEpicorSupplierId.Rows[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert[vwMissingEpicorSupplierId.ColumnId("Office")];
                ((Label)e.Row.FindControl("lbl_PurchaseTerm")).Text = alert[vwMissingEpicorSupplierId.ColumnId("Purchase Term")];
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = alert[vwMissingEpicorSupplierId.ColumnId("Vendor Name")];
                ((Label)e.Row.FindControl("lbl_SupplierId")).Text = alert[vwMissingEpicorSupplierId.ColumnId("EpicorSupplierId")];
            }
        }

        protected void gv_MissingPayAdvEMail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            /*
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AlertNotificationRef alert = (AlertNotificationRef)vwMissingPaymentAdviceEMail[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = alert.Vendor.Name;
            }
            */
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string[] alert = (string[])vwMissingPaymentAdviceEMail.Rows[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert[vwMissingPaymentAdviceEMail.ColumnId("Office")];
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = alert[vwMissingPaymentAdviceEMail.ColumnId("Vendor Name")];
            }
        }

        protected void gv_EziBuyOSPaymentList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string[] Remark;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AlertNotificationRef alert = (AlertNotificationRef)vwEziBuyOSPaymentList[e.Row.RowIndex];
                Remark = alert.Remark.Split(Delimiter);
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((Label)e.Row.FindControl("lbl_InboundDlyNo")).Text = Remark[0].ToString();
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = alert.Product.ItemNo;
                ((Label)e.Row.FindControl("lbl_CustomerDlydate")).Text = DateTimeUtility.getDateString(alert.CustomerAtWarehouseDate);
                ((Label)e.Row.FindControl("lbl_InvoiceNo")).Text = alert.InvoiceNo;
                ((Label)e.Row.FindControl("lbl_InvoiceAmount")).Text = alert.TotalShippedAmt.ToString("N02");
                ((Label)e.Row.FindControl("lbl_InvoiceDate")).Text = DateTimeUtility.getDateString(alert.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_ActualInWHDate")).Text = DateTimeUtility.getDateString(alert.ActualAtWarehouseDate);
                ((Label)e.Row.FindControl("lbl_InvoiceSentDate")).Text = Remark[1].ToString();
                ((Label)e.Row.FindControl("lbl_SettlementDate")).Text = Remark[2].ToString();
                ((Label)e.Row.FindControl("lbl_BookInWHDate")).Text = Remark[3].ToString();
                ((Label)e.Row.FindControl("lbl_GoldSealApprovalDate")).Text = DateTimeUtility.getDateString(alert.ApprovalDate);
            }
        }

        protected void gv_OSNTInvDeptApv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string AccountNo = string.Empty;
                string NTVendor = string.Empty;
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertOSNTInvoiceForDeptApproval[e.Row.RowIndex];
                string[] remark = alert.Remark.Split(Delimiter);
                if (remark.Length > 1)
                    NTVendor = remark[1];
                if (remark.Length > 0)
                    AccountNo = remark[0];
                NTWFS wfs = NTWFS.getType(alert.WorkflowStatus.Id);
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((LinkButton)e.Row.FindControl("lnk_InvoiceAccountNo")).Text = alert.SupplierInvoiceNo + (string.IsNullOrEmpty(alert.SupplierInvoiceNo) ? "" : "<br>") + AccountNo;
                ((LinkButton)e.Row.FindControl("lnk_InvoiceAccountNo")).Attributes.Add("OnClick", "window.open('../NonTrade/NonTradeInvoice.aspx?InvoiceId=" + alert.DocumentId.ToString() + "', 'NonTradeInvoice', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_InvoiceDate")).Text = DateTimeUtility.getDateString(alert.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_Vendor")).Text = "<div style='text-align:left;'>" + NTVendor + "</div>";
                ((Label)e.Row.FindControl("lbl_DueDate")).Text = DateTimeUtility.getDateString(alert.Date);
                ((Label)e.Row.FindControl("lbl_Currency")).Text = alert.Currency.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_Amount")).Text = alert.Amount.ToString("N02");
                ((Label)e.Row.FindControl("lbl_InvoiceStatus")).Text = wfs.Name;
                ((Label)e.Row.FindControl("lbl_Approver")).Text = alert.User.DisplayName;
            }
        }

        protected void gv_NTInvAccApv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string AccountNo = string.Empty;
                string NTVendor = string.Empty;
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertNTInvoiceAccountsApproved[e.Row.RowIndex];
                string[] remark = alert.Remark.Split(Delimiter);
                if (remark.Length > 1)
                    NTVendor = remark[1];
                if (remark.Length > 0)
                    AccountNo = remark[0];
                NTWFS wfs = NTWFS.getType(alert.WorkflowStatus.Id);
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((LinkButton)e.Row.FindControl("lnk_InvoiceAccountNo")).Text = alert.SupplierInvoiceNo + (string.IsNullOrEmpty(alert.SupplierInvoiceNo) ? "" : "<br>") + AccountNo;
                ((LinkButton)e.Row.FindControl("lnk_InvoiceAccountNo")).Attributes.Add("OnClick", "window.open('../NonTrade/NonTradeInvoice.aspx?InvoiceId=" + alert.DocumentId.ToString() + "', 'NonTradeInvoice', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_InvoiceDate")).Text = DateTimeUtility.getDateString(alert.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_Vendor")).Text = "<div style='text-align:left;'>" + NTVendor + "</div>";
                ((Label)e.Row.FindControl("lbl_DueDate")).Text = DateTimeUtility.getDateString(alert.Date);
                ((Label)e.Row.FindControl("lbl_Currency")).Text = alert.Currency.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_Amount")).Text = alert.Amount.ToString("N02");
                ((Label)e.Row.FindControl("lbl_InvoiceStatus")).Text = wfs.Name;
            }
        }

        protected void gv_OSNTInvAccLvl1Apv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string AccountNo = string.Empty;
                string NTVendor = string.Empty;
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertOSNTInvoiceForAccLevel1Approval[e.Row.RowIndex];
                string[] remark = alert.Remark.Split(Delimiter);
                if (remark.Length > 1)
                    NTVendor = remark[1];
                if (remark.Length > 0)
                    AccountNo = remark[0];
                NTWFS wfs = NTWFS.getType(alert.WorkflowStatus.Id);
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((LinkButton)e.Row.FindControl("lnk_InvoiceAccountNo")).Text = alert.SupplierInvoiceNo + (string.IsNullOrEmpty(alert.SupplierInvoiceNo) ? "" : "<br>") + AccountNo;
                ((LinkButton)e.Row.FindControl("lnk_InvoiceAccountNo")).Attributes.Add("OnClick", "window.open('../NonTrade/NonTradeInvoice.aspx?InvoiceId=" + alert.DocumentId.ToString() + "', 'NonTradeInvoice', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_InvoiceDate")).Text = DateTimeUtility.getDateString(alert.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_Vendor")).Text = "<div style='text-align:left;'>" + NTVendor + "</div>";
                ((Label)e.Row.FindControl("lbl_DueDate")).Text = DateTimeUtility.getDateString(alert.Date);
                ((Label)e.Row.FindControl("lbl_Currency")).Text = alert.Currency.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_Amount")).Text = alert.Amount.ToString("N02");
                ((Label)e.Row.FindControl("lbl_InvoiceStatus")).Text = wfs.Name;
            }
        }

        protected void gv_OSNTInvAccLvl2Apv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string AccountNo = string.Empty;
                string NTVendor = string.Empty;
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertOSNTInvoiceForAccLevel2Approval[e.Row.RowIndex];
                string[] remark = alert.Remark.Split(Delimiter);
                if (remark.Length > 1)
                    NTVendor = remark[1];
                if (remark.Length > 0)
                    AccountNo = remark[0];
                NTWFS wfs = NTWFS.getType(alert.WorkflowStatus.Id);
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((LinkButton)e.Row.FindControl("lnk_InvoiceAccountNo")).Text = alert.SupplierInvoiceNo + (string.IsNullOrEmpty(alert.SupplierInvoiceNo) ? "" : "<br>") + AccountNo;
                ((LinkButton)e.Row.FindControl("lnk_InvoiceAccountNo")).Attributes.Add("OnClick", "window.open('../NonTrade/NonTradeInvoice.aspx?InvoiceId=" + alert.DocumentId.ToString() + "', 'NonTradeInvoice', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_InvoiceDate")).Text = DateTimeUtility.getDateString(alert.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_Vendor")).Text = "<div style='text-align:left;'>" + NTVendor + "</div>";
                ((Label)e.Row.FindControl("lbl_DueDate")).Text = DateTimeUtility.getDateString(alert.Date);
                ((Label)e.Row.FindControl("lbl_Currency")).Text = alert.Currency.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_Amount")).Text = alert.Amount.ToString("N02");
                ((Label)e.Row.FindControl("lbl_InvoiceStatus")).Text = wfs.Name;
            }
        }

        protected void gv_NewNTVendorApproval_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string vendorAddress = string.Empty;
                string vendorName = string.Empty;
                string expenseType = string.Empty;
                AlertNotificationRef alert = (AlertNotificationRef)vwAlertNewNTVendorApproval[e.Row.RowIndex];
                string[] remark = alert.Remark.Split(Delimiter);
                if (remark.Length > 0)
                    vendorAddress = remark[0];
                if (remark.Length > 1)
                    vendorName = remark[1];
                if (remark.Length > 2)
                    expenseType = remark[2];
                NTWFS wfs = NTWFS.getType(alert.WorkflowStatus.Id);
                ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                ((LinkButton)e.Row.FindControl("lnk_NTVendor")).Text = vendorName;
                ((LinkButton)e.Row.FindControl("lnk_NTVendor")).Attributes.Add("OnClick", "window.open('../NonTrade/NonTradeVendor.aspx?NTVendorId=" + alert.DocumentId.ToString() + "', 'NonTradeVendor', 'width=800,height=600,scrollbars=1,resizable=1,status=1'); return false;");
                ((Label)e.Row.FindControl("lbl_NTVendorAddress")).Text = "<div style='text-align:left;'>" + vendorAddress + "</div>";
                ((Label)e.Row.FindControl("lbl_ExpenseType")).Text = "<div style='text-align:left;'>" + expenseType + "</div>";
            }
        }

        protected void gv_NTVendorAmendment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string address = string.Empty;
                    string vendorName = string.Empty;
                    string expenseType = string.Empty;
                    string amendment = string.Empty;
                    AlertNotificationRef alert = (AlertNotificationRef)vwAlertNTVendorAmendment[e.Row.RowIndex];
                    string[] remark = alert.Remark.Split(Delimiter);
                    if (remark.Length > 0)
                        address = remark[0];
                    if (remark.Length > 1)
                        vendorName = remark[1];
                    if (remark.Length > 2)
                        expenseType = remark[2];
                    amendment = alert.Description;
                    NTWFS wfs = NTWFS.getType(alert.WorkflowStatus.Id);
                    ((Label)e.Row.FindControl("lbl_Office")).Text = alert.Office.OfficeCode;
                    ((LinkButton)e.Row.FindControl("lnk_NTVendor")).Text = vendorName;
                    ((LinkButton)e.Row.FindControl("lnk_NTVendor")).Attributes.Add("OnClick", "window.open('../NonTrade/NonTradeVendor.aspx?NTVendorId=" + alert.DocumentId.ToString() + "', 'NonTradeVendor', 'width=800,height=600,scrollbars=1,resizable=1,status=1'); return false;");
                    ((Label)e.Row.FindControl("lbl_NTVendorAmendment")).Text = "<div style='text-align:left;'>" + amendment + "&nbsp;</div>";
                }
            }
        }

        #endregion


        #region Data Extraction

        protected void BuildExtractResultMessage(Label Obj, int RecordCount)
        {
            string Msg;

            if (RecordCount < 0)
                RecordCount = -1;

            if (RecordCount == 0)
                Msg = " (No Record)";
            else
                if (RecordCount == 1)
                    Msg = " (1 Record)";
                else
                    if (RecordCount > 1 && RecordCount < 100)
                        Msg = " (" + RecordCount.ToString() + " Records)";
                    else
                        if (RecordCount >= 100)
                            Msg = " (First 100 Records)";
                        else
                            Msg = "";

            Obj.Text = Msg + "&nbsp;";
            Obj.ForeColor = (RecordCount > 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black);
        }

        protected void LoadInvUploadFail(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = AlertNotificationWorker.Instance.getInvoiceUploadFailCount(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                BuildExtractResultMessage(lbl_InvUploadFail, RecordCount);
                tr_InvUploadFail_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertInvUploadFail = AlertNotificationWorker.Instance.getInvoiceUploadFailList(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                gv_InvUploadFail.DataSource = vwAlertInvUploadFail;
                gv_InvUploadFail.DataBind();
                gv_InvUploadFail.Visible = true;

                BuildExtractResultMessage(lbl_InvUploadFail, vwAlertInvUploadFail.Count);
                lb_InvUploadFail.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadMissingAdvBankVendor(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount;
                RecordCount = loadVendorAlertGroupCount(AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId, AlertType.AdvisingBank);
                BuildExtractResultMessage(lbl_MissingAdvBankVendor, RecordCount);
                tr_MissingAdvBankVendor_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertVendorMissingAdvisingBank = loadVendorAlertGroupDetail(UserId, OfficeIdList, DepartmentCode, DepartmentIdList, AlertType.AdvisingBank);
                gv_MissingAdvBankVendor.DataSource = vwAlertVendorMissingAdvisingBank;
                gv_MissingAdvBankVendor.DataBind();
                gv_MissingAdvBankVendor.Visible = true;

                BuildExtractResultMessage(lbl_MissingAdvBankVendor, vwAlertVendorMissingAdvisingBank.Count);
                lb_MissingAdvBankVendor.Visible = false;        //Allow to click once only
            }
        }

        protected void LoadUTOrderOSInv(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = AlertNotificationWorker.Instance.getUTOrderOutstandingToInvoiceCount(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                BuildExtractResultMessage(lbl_UTOrderOSInv, RecordCount);
                tr_UTOrderOSInv_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertUTOutstandingInvoice = AlertNotificationWorker.Instance.getUTOrderOutstandingToInvoiceList(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                gv_UTOrderOSInv.DataSource = vwAlertUTOutstandingInvoice;
                gv_UTOrderOSInv.DataBind();
                gv_UTOrderOSInv.Visible = true;

                BuildExtractResultMessage(lbl_UTOrderOSInv, vwAlertUTOutstandingInvoice.Count);
                lb_UTOrderOSInv.Visible = false;        //Allow to click once only
            }
        }

        protected void LoadOSBooking(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = AlertNotificationWorker.Instance.getOutstandingBookingCount(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                BuildExtractResultMessage(lbl_OSBooking, RecordCount);
                tr_OSBooking_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertOutstandingBooking = AlertNotificationWorker.Instance.getOutstandingBookingList(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                gv_OSBooking.DataSource = vwAlertOutstandingBooking;
                gv_OSBooking.DataBind();
                gv_OSBooking.Visible = true;

                BuildExtractResultMessage(lbl_OSBooking, vwAlertOutstandingBooking.Count);
                lb_OSBooking.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadOSShipDoc(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = AlertNotificationWorker.Instance.getOutstandingDocumentCount(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                BuildExtractResultMessage(lbl_OSShipDoc, RecordCount);
                tr_OSShipDoc_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertOutstandingDocument = AlertNotificationWorker.Instance.getOutstandingDocumentList(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                gv_OSShipDoc.DataSource = vwAlertOutstandingDocument;
                gv_OSShipDoc.DataBind();
                gv_OSShipDoc.Visible = true;

                BuildExtractResultMessage(lbl_OSShipDoc, vwAlertOutstandingDocument.Count);
                lb_OSShipDoc.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadOSShipDocOffshore(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = AlertNotificationWorker.Instance.getOutstandingDocumentOffshoreCount(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                BuildExtractResultMessage(this.lbl_OSShipDocOffshore, RecordCount);
                tr_OSShipDocOffshore_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertOutstandingDocumentOffshore = AlertNotificationWorker.Instance.getOutstandingDocumentOffshoreList(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                gv_OSShipDocOffshore.DataSource = vwAlertOutstandingDocumentOffshore;
                gv_OSShipDocOffshore.DataBind();
                gv_OSShipDocOffshore.Visible = true;

                BuildExtractResultMessage(lbl_OSShipDocOffshore, vwAlertOutstandingDocumentOffshore.Count);
                lb_OSShipDocOffshore.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadOSResubmitDoc(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = AlertNotificationWorker.Instance.getOutstandingResubmitDocumentCount(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                BuildExtractResultMessage(lbl_OSResubmitDoc, RecordCount);
                tr_OSResubmitDoc_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertOutstandingResubmitDocument = AlertNotificationWorker.Instance.getOutstandingResubmitDocumentList(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                gv_OSResubmitDoc.DataSource = vwAlertOutstandingResubmitDocument;
                gv_OSResubmitDoc.DataBind();
                gv_OSResubmitDoc.Visible = true;

                BuildExtractResultMessage(lbl_OSResubmitDoc, vwAlertOutstandingResubmitDocument.Count);
                lb_OSResubmitDoc.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadMissingSunAccCode(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = loadVendorAlertGroupCount(AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId, AlertType.SunAccCode);
                BuildExtractResultMessage(lbl_MissingSunAccCode, RecordCount);
                tr_MissingSunAccCode_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwMissingSunAccountCode = loadGenericVendorAlertGroupDetail(UserId, OfficeIdList, DepartmentCode, DepartmentIdList, AlertType.SunAccCode);
                gv_MissingSunAccCode.DataSource = vwMissingSunAccountCode.Rows; 
                gv_MissingSunAccCode.DataBind();
                gv_MissingSunAccCode.Visible = true;

                BuildExtractResultMessage(lbl_MissingSunAccCode, vwMissingSunAccountCode.Rows.Count);
                lb_MissingSunAccCode.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadMissingEpicorSupplierId(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                /*
                int RecordCount = loadVendorAlertGroupCount(AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId, AlertType.EpicorSupplierId);
                */ 
                int RecordCount = loadEpicorVendorAlertCount(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                BuildExtractResultMessage(lbl_MissingEpicorSupplierId, RecordCount);
                tr_MissingEpicorSupplierId_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                //vwMissingEpicorSupplierId = loadVendorAlertGroupDetail(UserId, OfficeIdList, DepartmentCode, DepartmentIdList, AlertType.EpicorSupplierId);
                vwMissingEpicorSupplierId = loadEpicorVendorAlertDetail(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                gv_MissingEpicorSupplierId.DataSource = vwMissingEpicorSupplierId.Rows;
                gv_MissingEpicorSupplierId.DataBind();
                gv_MissingEpicorSupplierId.Visible = true;

                BuildExtractResultMessage(lbl_MissingEpicorSupplierId, vwMissingEpicorSupplierId.Rows.Count);
                lb_MissingEpicorSupplierId.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadMissingPayAdvEMail(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = loadVendorAlertGroupCount(AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId, AlertType.EAdvice);
                BuildExtractResultMessage(lbl_MissingPayAdvEMail, RecordCount);
                tr_MissingPayAdvEMail_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                //vwMissingPaymentAdviceEMail = loadVendorAlertGroupDetail(UserId, OfficeIdList, DepartmentCode, DepartmentIdList, AlertType.EAdvice);
                //gv_MissingPayAdvEMail.DataSource = vwMissingPaymentAdviceEMail;
                vwMissingPaymentAdviceEMail = loadGenericVendorAlertGroupDetail(UserId, OfficeIdList, DepartmentCode, DepartmentIdList, AlertType.EAdvice);
                gv_MissingPayAdvEMail.DataSource = vwMissingPaymentAdviceEMail.Rows;
                gv_MissingPayAdvEMail.DataBind();
                gv_MissingPayAdvEMail.Visible = true;

                //BuildExtractResultMessage(lbl_MissingPayAdvEMail, vwMissingPaymentAdviceEMail.Count);
                BuildExtractResultMessage(lbl_MissingPayAdvEMail, vwMissingPaymentAdviceEMail.Rows.Count);
                lb_MissingPayAdvEMail.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadEziBuyOSPaymentList(string Type, int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount;
                if (CountMode == "FAST")
                    RecordCount = AlertNotificationWorker.Instance.getEziBuyOSPaymentListCount_Fast(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                else
                    RecordCount = AlertNotificationWorker.Instance.getEziBuyOSPaymentListCount(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);

                BuildExtractResultMessage(lbl_EziBuyOSPaymentList, RecordCount);
                tr_EziBuyOSPaymentList_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwEziBuyOSPaymentList = AlertNotificationWorker.Instance.getEziBuyOSPaymentList(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                gv_EziBuyOSPaymentList.DataSource = vwEziBuyOSPaymentList;
                gv_EziBuyOSPaymentList.DataBind();
                gv_EziBuyOSPaymentList.Visible = true;

                BuildExtractResultMessage(lbl_EziBuyOSPaymentList, vwEziBuyOSPaymentList.Count);
                lb_EziBuyOSPaymentList.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadOSNTInvDeptApv(string Type, int UserId, TypeCollector OfficeIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = LoadNTInvoiceAlertGroupCount(AlertUserId, SelectedOfficeId, AlertType.OSNTInvDeptApv);
                BuildExtractResultMessage(this.lbl_OSNTInvDeptApv, RecordCount);
                tr_OSNTInvDeptApv_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertOSNTInvoiceForDeptApproval = LoadNTInvoiceAlertDetailList(UserId, OfficeIdList, AlertType.OSNTInvDeptApv);
                gv_OSNTInvDeptApv.DataSource = vwAlertOSNTInvoiceForDeptApproval;
                gv_OSNTInvDeptApv.DataBind();
                gv_OSNTInvDeptApv.Visible = true;

                BuildExtractResultMessage(lbl_OSNTInvDeptApv, vwAlertOSNTInvoiceForDeptApproval.Count);
                lb_OSNTInvDeptApv.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadNTInvAccApv(string Type, int UserId, TypeCollector OfficeIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = LoadNTInvoiceAlertGroupCount(AlertUserId, SelectedOfficeId, AlertType.NTInvAccApv);
                BuildExtractResultMessage(this.lbl_NTInvAccApv, RecordCount);
                tr_NTInvAccApv_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertNTInvoiceAccountsApproved = LoadNTInvoiceAlertDetailList(UserId, OfficeIdList, AlertType.NTInvAccApv);
                gv_NTInvAccApv.DataSource = vwAlertNTInvoiceAccountsApproved;
                gv_NTInvAccApv.DataBind();
                gv_NTInvAccApv.Visible = true;

                BuildExtractResultMessage(lbl_NTInvAccApv, vwAlertNTInvoiceAccountsApproved.Count);
                lb_NTInvAccApv.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadOSNTInvAccLvl1Apv(string Type, int UserId, TypeCollector OfficeIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = LoadNTInvoiceAlertGroupCount(AlertUserId, SelectedOfficeId, AlertType.OSNTInvAccL1Apv);
                BuildExtractResultMessage(this.lbl_OSNTInvAccLvl1Apv, RecordCount);
                tr_OSNTInvAccLvl1Apv_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertOSNTInvoiceForAccLevel1Approval = LoadNTInvoiceAlertDetailList(UserId, OfficeIdList, AlertType.OSNTInvAccL1Apv);
                gv_OSNTInvAccLvl1Apv.DataSource = vwAlertOSNTInvoiceForAccLevel1Approval;
                gv_OSNTInvAccLvl1Apv.DataBind();
                gv_OSNTInvAccLvl1Apv.Visible = true;

                BuildExtractResultMessage(lbl_OSNTInvAccLvl1Apv, vwAlertOSNTInvoiceForAccLevel1Approval.Count);
                lb_OSNTInvAccLvl1Apv.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadOSNTInvAccLvl2Apv(string Type, int UserId, TypeCollector OfficeIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = LoadNTInvoiceAlertGroupCount(AlertUserId, SelectedOfficeId, AlertType.OSNTInvAccL2Apv);
                BuildExtractResultMessage(this.lbl_OSNTInvAccLvl2Apv, RecordCount);
                tr_OSNTInvAccLvl2Apv_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertOSNTInvoiceForAccLevel2Approval = LoadNTInvoiceAlertDetailList(UserId, OfficeIdList, AlertType.OSNTInvAccL2Apv);
                gv_OSNTInvAccLvl2Apv.DataSource = vwAlertOSNTInvoiceForAccLevel2Approval;
                gv_OSNTInvAccLvl2Apv.DataBind();
                gv_OSNTInvAccLvl2Apv.Visible = true;

                BuildExtractResultMessage(lbl_OSNTInvAccLvl2Apv, vwAlertOSNTInvoiceForAccLevel2Approval.Count);
                lb_OSNTInvAccLvl2Apv.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadNewNTVendorApproval(string Type, int UserId, TypeCollector OfficeIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = loadNTVendorAlertGroupCount(UserId, OfficeIdList, AlertType.NewNTVendorApv);
                BuildExtractResultMessage(this.lbl_NewNTVendorApproval, RecordCount);
                tr_NewNTVendorApproval_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertNewNTVendorApproval = loadNTVendorAlertGroupDetail(UserId, OfficeIdList, AlertType.NewNTVendorApv);
                gv_NewNTVendorApproval.DataSource = vwAlertNewNTVendorApproval;
                gv_NewNTVendorApproval.DataBind();
                gv_NewNTVendorApproval.Visible = true;

                BuildExtractResultMessage(lbl_NewNTVendorApproval, vwAlertNewNTVendorApproval.Count);
                lb_NewNTVendorApproval.Visible = false;           //Allow to click once only
            }
        }

        protected void LoadNTVendorAmendment(string Type, int UserId, TypeCollector OfficeIdList)
        {
            if (Type == "COUNT")
            {
                int RecordCount = loadNTVendorAlertGroupCount(AlertUserId, SelectedOfficeId, AlertType.NTVendorAmendment);
                BuildExtractResultMessage(this.lbl_NTVendorAmendment, RecordCount);
                tr_NTVendorAmendment_Header.Visible = true;
            }
            else if (Type == "DETAIL")
            {
                vwAlertNTVendorAmendment = loadNTVendorAlertGroupDetail(UserId, OfficeIdList, AlertType.NTVendorAmendment);
                gv_NTVendorAmendment.DataSource = vwAlertNTVendorAmendment;
                gv_NTVendorAmendment.DataBind();
                gv_NTVendorAmendment.Visible = true;

                BuildExtractResultMessage(lbl_NTVendorAmendment, vwAlertNTVendorAmendment.Count);
                lb_NTVendorAmendment.Visible = false;           //Allow to click once only
            }
        }

        // Loading Count and Detail in group
        protected int LoadNTInvoiceAlertGroupCount(int UserId, TypeCollector OfficeIdList, AlertType alertType)
        {
            int index = -1;
            int count = 0;
            string keyword = string.Empty;
            if (alertType == AlertType.OSNTInvDeptApv)
                index = 0;
            else if (alertType == AlertType.OSNTInvAccL1Apv)
                index = 1;
            else if (alertType == AlertType.OSNTInvAccL2Apv)
                index = 2;
            else if (alertType == AlertType.NTInvAccApv)
                index = 3;

            if (index >= 0)
            {
                if (vwAlertNTInvoiceApprovalGroupCount == null)
                    vwAlertNTInvoiceApprovalGroupCount = AlertNotificationWorker.Instance.getNTInvoiceApprovalGroupCount(UserId, OfficeIdList);

                count = (int)((ArrayList)vwAlertNTInvoiceApprovalGroupCount)[index];
            }
            return count;
        }

        
        protected ArrayList LoadNTInvoiceAlertDetailList(int UserId, TypeCollector OfficeIdList, AlertType alertType)
        {
            ArrayList invList = new ArrayList();
            string keyword = string.Empty;
            int groupId;

            if (alertType == AlertType.OSNTInvDeptApv) groupId = 0;
            else if (alertType == AlertType.OSNTInvAccL1Apv) groupId = 1;
            else if (alertType == AlertType.OSNTInvAccL2Apv) groupId = 2;
            else if (alertType == AlertType.NTInvAccApv) groupId = 3;
            else groupId = -1;

            return AlertNotificationWorker.Instance.getNTInvoiceApprovalDetailList(UserId, OfficeIdList, groupId);
        }
        
        /*
        protected ArrayList LoadNTInvoiceAlertGroupDetailList(int UserId, TypeCollector OfficeIdList, AlertType alertType)
        {
            ArrayList invList = new ArrayList();
            string keyword = string.Empty;
            int groupId;

            if (alertType == AlertType.OSNTInvDeptApv) groupId = 0;
            else if (alertType == AlertType.OSNTInvAccL1Apv) groupId = 1;
            else if (alertType == AlertType.OSNTInvAccL2Apv) groupId = 2;
            else if (alertType == AlertType.NTInvAccApv) groupId = 3;
            else groupId = -1;

            return AlertNotificationWorker.Instance.getNTInvoiceApprovalGroupList(UserId, OfficeIdList, groupId);
        }
        */
        /*
        protected ArrayList LoadNTInvoiceAlertGroupDetailList(int UserId, TypeCollector OfficeIdList, AlertType alertType)
        {
            ArrayList invList = new ArrayList();

            if (vwAlertNTInvoiceApprovalGroupDetail == null)
                vwAlertNTInvoiceApprovalGroupDetail = AlertNotificationWorker.Instance.getNTInvoiceApprovalGroupList(UserId, OfficeIdList);

            foreach (AlertNotificationRef alert in vwAlertNTInvoiceApprovalGroupDetail)
                if ((alertType == AlertType.OSNTInvDeptApv && alert.GroupId % 2 == 1)
                    || (alertType == AlertType.OSNTInvAccL1Apv && (alert.GroupId / 2 % 2) == 1)
                    || (alertType == AlertType.OSNTInvAccL2Apv && (alert.GroupId / 4 % 2) == 1)
                    || (alertType == AlertType.NTInvAccApv && (alert.GroupId / 8 % 2) == 1))
                {
                    invList.Add(alert);
                    if (invList.Count >= 100)
                        break;
                }
            return invList;
        }
        */

        protected int loadVendorAlertGroupCount(int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList, AlertType vendorAlertType)
        {
            ArrayList vendorList = new ArrayList();
            int key=-1;
            int count = 0;
            string keyword = string.Empty;
            if (vendorAlertType == AlertType.SunAccCode)
                key = 0;
            else if (vendorAlertType == AlertType.EAdvice)
                key = 1;
            else if (vendorAlertType == AlertType.AdvisingBank)
                key = 2;
            //else if (vendorAlertType == AlertType.EpicorSupplierId)
            //    key = 3;
            if (key >= 0)
            {
                if (vwAlertVendorGroupCount == null)
                    vwAlertVendorGroupCount = AlertNotificationWorker.Instance.getVendorAlertCount(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);

                count = (int)((ArrayList)vwAlertVendorGroupCount)[key];
            }
            return count;            
        }

        protected ArrayList loadVendorAlertGroupDetail(int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList, AlertType vendorAlertType)
        {
            ArrayList vendorList = new ArrayList();
            string keyword = string.Empty;
            /*
            if (vendorAlertType == AlertType.SunAccCode)
                keyword = "SunAcc";
            else if (vendorAlertType == AlertType.EAdvice)
                keyword = "EAdvAddr";
            else if (vendorAlertType == AlertType.AdvisingBank)
                keyword = "AdvBank";
            else if (vendorAlertType == AlertType.EpicorSupplierId)
                keyword = "EpicorSupplier";
            */
            keyword = "AdvBank";
            if (keyword != string.Empty)
            {
                Hashtable ht = new Hashtable();
                if (vwAlertVendorGroupDetail == null)
                    vwAlertVendorGroupDetail = AlertNotificationWorker.Instance.getVendorAlertDetailList(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                foreach (AlertNotificationRef alert in vwAlertVendorGroupDetail)
                    if (alert.Remark.IndexOf(keyword) >= 0)
                    {
                        if (ht[alert.Vendor.VendorId] == null)
                        {
                            ht.Add(alert.Vendor.VendorId, alert.ShipmentId);
                            vendorList.Add(alert);
                            if (vendorList.Count >= 100)
                                break;
                        }
                    }
            }
            return vendorList;
        }
        
        protected AlertTable loadGenericVendorAlertGroupDetail(int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList, AlertType vendorAlertType)
        {
            AlertTable vendorTable = null;
            string keyword = string.Empty;
            if (vendorAlertType == AlertType.SunAccCode)
                keyword = "SunAcc";
            else if (vendorAlertType == AlertType.EAdvice)
                keyword = "EAdvAddr";
            /*
            else if (vendorAlertType == AlertType.AdvisingBank)
                keyword = "AdvBank";
            else if (vendorAlertType == AlertType.EpicorSupplierId)
                keyword = "EpicorSupplier";
            */
            if (keyword != string.Empty)
            {
                if (vwAlertGenericVendorGroupDetail == null)
                    vwAlertGenericVendorGroupDetail = AlertNotificationWorker.Instance.getGenericVendorAlertDetailList(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
                vendorTable = vwAlertGenericVendorGroupDetail.CopyColumns();
                int remarkColumn = vwAlertGenericVendorGroupDetail.ColumnId("Remark");
                AlertTable table = vwAlertGenericVendorGroupDetail;
                Hashtable ht = new Hashtable();
                foreach (string[] r in table.Rows)
                {
                    if (r[remarkColumn].Contains(keyword))
                    {
                        string vendorId = r[table.ColumnId("VendorId")];
                        string shipmentId = r[table.ColumnId("ShipmentId")];
                        if (ht[vendorId] == null)
                        {
                            ht.Add(vendorId, shipmentId);
                            if (string.IsNullOrEmpty(r[table.ColumnId("Vendor Name")]))
                            {
                                r[table.ColumnId("Vendor Name")] = VendorWorker.Instance.getVendorByKey(int.Parse(r[table.ColumnId("VendorId")])).Name;
                                r[table.ColumnId("Office")] = GeneralWorker.Instance.getOfficeRefByKey(int.Parse(r[table.ColumnId("OfficeId")])).OfficeCode;
                                r[table.ColumnId("Purchase Term")] = CommonWorker.Instance.getTermOfPurchaseByKey(int.Parse(r[table.ColumnId("PurchaseTermId")])).TermOfPurchaseDescription;
                            }
                            if (keyword == "AdvBank")
                                if (string.IsNullOrEmpty(r[table.ColumnId("Office")]))
                                {
                                    r[table.ColumnId("Item No")] = ProductWorker.Instance.getProductByKey(int.Parse(r[table.ColumnId("ProductId")])).ItemNo;
                                    r[table.ColumnId("Merchandiser")] = GeneralWorker.Instance.getUserByKey(int.Parse(r[table.ColumnId("MerchandiserId")])).DisplayName;
                                    r[table.ColumnId("WorkflowStatus")] = ContractWFS.getType(int.Parse(r[table.ColumnId("WorkflowStatusId")])).Name;
                                }

                            vendorTable.Rows.Add(r);
                            if (vendorTable.Rows.Count >= 100)
                                break;
                        }
                    }
                }
            }
            return vendorTable;
        }

        protected AlertTable getEpicorVendorAlertDetail(int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            AlertTable vendorTable = null;
            AlertTable nssVendor = AlertNotificationWorker.Instance.getVendorCompanyList(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
            vendorTable = nssVendor.CopyColumns();
            SortedList epicorVendorList = AccountWorker.Instance.getEpicorVendorByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALLSTRING, GeneralCriteria.ALLSTRING);
            foreach (string[] r in nssVendor.Rows)
            {
                string epicorSupplierId = r[nssVendor.ColumnId("EpicorSupplierId")];
                string epicorComanyId = r[nssVendor.ColumnId("EpicorCompanyId")];
                if (epicorSupplierId == "" || !epicorVendorList.ContainsKey(epicorSupplierId + "|" + epicorComanyId))
                {
                    r[nssVendor.ColumnId("Vendor Name")] = VendorWorker.Instance.getVendorByKey(int.Parse(r[nssVendor.ColumnId("VendorId")])).Name;
                    r[nssVendor.ColumnId("Office")] = GeneralWorker.Instance.getOfficeRefByKey(int.Parse(r[nssVendor.ColumnId("OfficeId")])).OfficeCode;
                    r[nssVendor.ColumnId("Purchase Term")] = CommonWorker.Instance.getTermOfPurchaseByKey(int.Parse(r[nssVendor.ColumnId("PurchaseTermId")])).TermOfPurchaseDescription;
                    vendorTable.Rows.Add(r);
                }
            }
            return vendorTable;
        }

        protected AlertTable loadEpicorVendorAlertDetail(int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            AlertTable vendorTable = getEpicorVendorAlertDetail(UserId, OfficeIdList, DepartmentCode, DepartmentIdList);
            AlertTable top100Vendor = vendorTable.CopyColumns();
            foreach (string[] r in vendorTable.Rows)
            {
                top100Vendor.Rows.Add(r);
                if (top100Vendor.Rows.Count >= 100)
                    break;
            }
            return top100Vendor;
        }

        protected int loadEpicorVendorAlertCount(int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector DepartmentIdList)
        {
            return getEpicorVendorAlertDetail(UserId, OfficeIdList, DepartmentCode, DepartmentIdList).Rows.Count;
        }

        protected int loadNTVendorAlertGroupCount(int UserId, TypeCollector OfficeIdList, AlertType NTVendorAlertType)
        {
            int index = -1;
            int count = 0;
            if (NTVendorAlertType == AlertType.NewNTVendorApv)
                index = 0;
            else if (NTVendorAlertType == AlertType.NTVendorAmendment)
                index = 1;

            if (index >= 0)
            {
                if (vwAlertNTVendorGroupCount == null)
                    vwAlertNTVendorGroupCount = AlertNotificationWorker.Instance.getNTVendorGroupCount(UserId, OfficeIdList);
                count = (int)((ArrayList)vwAlertNTVendorGroupCount)[index];
            }
            return count;
        }

        protected ArrayList loadNTVendorAlertGroupDetail(int UserId, TypeCollector OfficeIdList, AlertType NTVendorAlertType)
        {
            ArrayList vendorList = new ArrayList();
            string keyword = string.Empty;

            if (vwAlertNTVendorGroupDetail == null)
                vwAlertNTVendorGroupDetail = AlertNotificationWorker.Instance.getNTVendorDetailGroupList(UserId, OfficeIdList);
            foreach (AlertNotificationRef alert in vwAlertNTVendorGroupDetail)
                if ((NTVendorAlertType == AlertType.NewNTVendorApv && alert.GroupId % 2 == 1)
                    || (NTVendorAlertType == AlertType.NTVendorAmendment && (alert.GroupId / 2 % 2) == 1))
                {
                    vendorList.Add(alert);
                    if (vendorList.Count >= 100)
                        break;
                }
            return vendorList;
        }

        
        protected void LoadAllAlertCount()
        {
            DateTime StartTime;
            DateTime groupStartTime;

            StartTime = DateTime.Now;
            groupStartTime = StartTime;

            if (HasAccessRightsTo_MissingAdvisingBank || HasAccessRightsTo_SuperView)
            {
                LoadMissingAdvBankVendor("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "Missing Advising Bank");
            }
            /*
            if (HasAccessRightsTo_MissingSunAccountCode || HasAccessRightsTo_SuperView)
            {
                LoadMissingSunAccCode("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "Vendor Missing SUN Account");
            }
            */ 
            if (HasAccessRightsTo_MissingEpicorSupplierId || HasAccessRightsTo_SuperView)
            {
                LoadMissingEpicorSupplierId("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "Vendor Missing Epicor Supplier ");
            }
            if (HasAccessRightsTo_MissingPaymentAdviceEMail || HasAccessRightsTo_SuperView)
            {
                LoadMissingPayAdvEMail("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "Vendor Missing e-Advice Address");
            }
            groupStartTime = debugShowTime(groupStartTime, isDebuger, "Vendor Alerts (SunAccCode/EAdvice/AdvBank/EpicorSupplier)");

            if (HasAccessRightsTo_InvUploadFail || HasAccessRightsTo_SuperView)
            {
                LoadInvUploadFail("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "Invoice Upload Fail");
            }
            if (HasAccessRightsTo_OutstandingUTOrder || HasAccessRightsTo_SuperView)
            {
                LoadUTOrderOSInv("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "O/S UT Order");
            }
            if (HasAccessRightsTo_OutstandingBooking || HasAccessRightsTo_SuperView)
            {
                LoadOSBooking("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "O/S Booking");
            }
            if (HasAccessRightsTo_OutstandingDocument || HasAccessRightsTo_SuperView)
            {
                LoadOSShipDoc("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "O/S Document to Accounts");
            }
            if (HasAccessRightsTo_OutstandingDocumentOffshore || HasAccessRightsTo_SuperView)
            {
                LoadOSShipDocOffshore("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "O/S Document to Accounts (Offshore)");
            }
            if (HasAccessRightsTo_OutstandingResubmitDocument || HasAccessRightsTo_SuperView)
            {
                LoadOSResubmitDoc("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "O/S Doc resubmit to Accounts");
            }
            if (HasAccessRightsTo_EziBuyOSPaymentList || HasAccessRightsTo_SuperView)
            {
                LoadEziBuyOSPaymentList("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                StartTime = debugShowTime(StartTime, isDebuger, "O/S EziBuy Payment");
            }

            groupStartTime = StartTime;
            if (HasAccessRightsTo_OutstandingNTInvoiceDepartmentApproval || HasAccessRightsTo_SuperView)
            {
                LoadOSNTInvDeptApv("COUNT", AlertUserId, SelectedOfficeId);
                StartTime = debugShowTime(StartTime, isDebuger, "O/S NT Invoice Dept Approval");
            }
            if (HasAccessRightsTo_OutstandingNTInvoiceAccountLevel1Approval || HasAccessRightsTo_SuperView)
            {
                LoadOSNTInvAccLvl1Apv("COUNT", AlertUserId, SelectedOfficeId);
                StartTime = debugShowTime(StartTime, isDebuger, "O/S NT Invoice 1st Account Approval");
            }
            if (HasAccessRightsTo_OutstandingNTInvoiceAccountLevel2Approval || HasAccessRightsTo_SuperView)
            {
                LoadOSNTInvAccLvl2Apv("COUNT", AlertUserId, SelectedOfficeId);
                StartTime = debugShowTime(StartTime, isDebuger, "O/S NT Invoice 2nd Account Approval");
            }
            if (HasAccessRightsTo_NTInvoiceAccountsApproved || HasAccessRightsTo_SuperView)
            {
                LoadNTInvAccApv("COUNT", AlertUserId, SelectedOfficeId);
                StartTime = debugShowTime(StartTime, isDebuger, "NT Invoice Accounts Approved");
            }
            groupStartTime = debugShowTime(groupStartTime, isDebuger, "O/S NT Invoice Approval");

            if (HasAccessRightsTo_NewNTVendorApproval || HasAccessRightsTo_SuperView)
            {
                LoadNewNTVendorApproval("COUNT", AlertUserId, SelectedOfficeId);
                StartTime = debugShowTime(StartTime, isDebuger, "New Vendor Approval");
            }
            if (HasAccessRightsTo_NTVendorAmendment || HasAccessRightsTo_SuperView)
            {
                LoadNTVendorAmendment("COUNT", AlertUserId, SelectedOfficeId);
                StartTime = debugShowTime(StartTime, isDebuger, "NTVendorAmendment");
            }
            groupStartTime = debugShowTime(groupStartTime, isDebuger, "NT Vendor Alert (Approval/Amendment)");
        }

        protected DateTime debugShowTime(DateTime startTime, bool enable, string tip)
        {
            if (enable)
            {
                pn_Debug.Visible = enable;
                DateTime endTime = DateTime.Now;
                TextBox tb = new TextBox();
                
                pn_Debug.Controls.Add(tb);
                tb.Text = ((endTime - startTime).TotalSeconds.ToString());
                tb.ToolTip = (string.IsNullOrEmpty(tip) ? startTime.TimeOfDay.ToString() + " - " + endTime.TimeOfDay.ToString() : tip);
                tb.Width = 100;
            }
            return DateTime.Now;
        }

        #endregion


        #region Click Events

        protected void lb_InvUploadFail_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_InvUploadFail || HasAccessRightsTo_SuperView)
            {
                LoadInvUploadFail("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "InvUploadFail";     //Show Detail grid at client side.
            }
        }

        protected void lb_MissingAdvBankVendor_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_MissingAdvisingBank || HasAccessRightsTo_SuperView)
            {
                LoadMissingAdvBankVendor("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "MissingAdvBankVendor";     //Show Detail grid at client side.
            }
        }

        protected void lb_UTOrderOSInv_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_OutstandingUTOrder || HasAccessRightsTo_SuperView)
            {
                LoadUTOrderOSInv("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "UTOrderOSInv";     //Show Detail grid at client side.
            }
        }

        protected void lb_OSBooking_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_OutstandingBooking || HasAccessRightsTo_SuperView)
            {
                LoadOSBooking("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "OSBooking";     //Show Detail grid at client side.
            }
        }

        protected void lb_OSShipDoc_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_OutstandingDocument || HasAccessRightsTo_SuperView)
            {
                LoadOSShipDoc("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "OSShipDoc";     //Show Detail grid at client side.
            }
        }

        protected void lb_OSShipDocOffshore_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_OutstandingDocumentOffshore || HasAccessRightsTo_SuperView)
            {
                LoadOSShipDocOffshore("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "OSShipDocOffshore";     //Show Detail grid at client side.
            }
        }

        protected void lb_OSResubmitDoc_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_OutstandingResubmitDocument || HasAccessRightsTo_SuperView)
            {
                LoadOSResubmitDoc("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "OSResubmitDoc";     //Show Detail grid at client side.
            }
        }

        protected void lb_MissingSunAccCode_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_MissingSunAccountCode || HasAccessRightsTo_SuperView)
            {
                LoadMissingSunAccCode("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "MissingSunAccCode";     //Show Detail grid at client side.
            }
        }

        protected void lb_MissingEpicorSupplierId_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_MissingEpicorSupplierId || HasAccessRightsTo_SuperView)
            {
                LoadMissingEpicorSupplierId("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "MissingEpicorSupplierId";     //Show Detail grid at client side.
            }
        }

        protected void lb_MissingPayAdvEMail_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_MissingPaymentAdviceEMail || HasAccessRightsTo_SuperView)
            {
                LoadMissingPayAdvEMail("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "MissingPayAdvEMail";     //Show Detail grid at client side.
            }
        }

        protected void lb_EziBuyOSPaymentList_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_EziBuyOSPaymentList || HasAccessRightsTo_SuperView)
            {
                LoadEziBuyOSPaymentList("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                txt_AlertList.Text += Delimiter + "EziBuyOSPaymentList";     //Show Detail grid at client side.
            }
        }

        protected void btn_EziBuyOSPaymentListInExcel_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_EziBuyOSPaymentList || HasAccessRightsTo_SuperView)
            {
                ReportHelper.export(ShippingReportManager.Instance.getEziBuyOSPaymentList(AlertUserId, SelectedOfficeId, SelectedDepartmentCode),
                                    HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "EziBuyOutstandingPaymentList");
            }
        }

        protected void btn_EziBuyOSPaymentReportForSendingEMail_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_EziBuyOSPaymentList || HasAccessRightsTo_SuperView)
            {
                ReportHelper.export(ShippingReportManager.Instance.getEziBuyOSPaymentReport_Test("DMY"),
                                    HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "EziBuyOutstandingPaymentReport");
            }
        }

        protected void lb_OSNTInvDeptApv_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_OutstandingNTInvoiceDepartmentApproval || HasAccessRightsTo_SuperView)
            {
                LoadOSNTInvDeptApv("DETAIL", AlertUserId, SelectedOfficeId);
                txt_AlertList.Text += Delimiter + "OSNTInvDeptApv";     //Show Detail grid at client side.
            }
        }

        protected void lb_OSNTInvAccLvl1Apv_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_OutstandingNTInvoiceAccountLevel1Approval || HasAccessRightsTo_SuperView)
            {
                LoadOSNTInvAccLvl1Apv("DETAIL", AlertUserId, SelectedOfficeId);
                txt_AlertList.Text += Delimiter + "OSNTInvAccLvl1Apv";     //Show Detail grid at client side.
            }
        }

        protected void lb_OSNTInvAccLvl2Apv_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_OutstandingNTInvoiceAccountLevel2Approval || HasAccessRightsTo_SuperView)
            {
                LoadOSNTInvAccLvl2Apv("DETAIL", AlertUserId, SelectedOfficeId);
                txt_AlertList.Text += Delimiter + "OSNTInvAccLvl2Apv";     //Show Detail grid at client side.
            }
        }

        protected void lb_NTInvAccApv_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_NTInvoiceAccountsApproved || HasAccessRightsTo_SuperView)
            {
                LoadNTInvAccApv("DETAIL", AlertUserId, SelectedOfficeId);
                txt_AlertList.Text += Delimiter + "NTInvAccApv";     //Show Detail grid at client side.
            }
        }

        protected void lb_NTVendorAmendment_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_NTVendorAmendment || HasAccessRightsTo_SuperView)
            {
                LoadNTVendorAmendment("DETAIL", AlertUserId, SelectedOfficeId);
                txt_AlertList.Text += Delimiter + "NTVendorAmendment";     //Show Detail grid at client side.
            }
        }

        protected void lb_NewNTVendorApproval_click(object sender, EventArgs e)
        {
            if (HasAccessRightsTo_NewNTVendorApproval || HasAccessRightsTo_SuperView)
            {
                LoadNewNTVendorApproval("DETAIL", AlertUserId, SelectedOfficeId);
                txt_AlertList.Text += Delimiter + "NewNTVendorApproval";     //Show Detail grid at client side.
            }
        }

        protected void btn_Apply_Click(object sender, EventArgs e)
        {
            vwAlertVendorGroupDetail = null;
            vwAlertVendorGroupCount = null;
            vwAlertGenericVendorGroupDetail = null;
            vwAlertNTInvoiceApprovalGroupCount = null;
            vwAlertNTInvoiceApprovalGroupDetail = null;
            vwAlertNTVendorGroupCount = null;
            vwAlertNTVendorGroupDetail = null;

            if (HasAccessRightsTo_InvUploadFail || HasAccessRightsTo_SuperView)
            {
                if (vwAlertInvUploadFail == null)
                    LoadInvUploadFail("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadInvUploadFail("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_MissingAdvisingBank || HasAccessRightsTo_SuperView)
            {
                if (vwAlertVendorMissingAdvisingBank == null)
                    LoadMissingAdvBankVendor("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadMissingAdvBankVendor("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_OutstandingUTOrder || HasAccessRightsTo_SuperView)
            {
                if (vwAlertUTOutstandingInvoice == null)
                    LoadUTOrderOSInv("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadUTOrderOSInv("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_OutstandingBooking || HasAccessRightsTo_SuperView)
            {
                if (vwAlertOutstandingBooking == null)
                    LoadOSBooking("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadOSBooking("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_OutstandingDocument || HasAccessRightsTo_SuperView)
            {
                if (vwAlertOutstandingDocument == null)
                    LoadOSShipDoc("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadOSShipDoc("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_OutstandingDocumentOffshore || HasAccessRightsTo_SuperView)
            {
                if (vwAlertOutstandingDocumentOffshore == null)
                    LoadOSShipDocOffshore("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadOSShipDocOffshore("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_OutstandingResubmitDocument || HasAccessRightsTo_SuperView)
            {
                if (vwAlertOutstandingResubmitDocument == null)
                    LoadOSResubmitDoc("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadOSResubmitDoc("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_MissingSunAccountCode || HasAccessRightsTo_SuperView)
            {
                if (vwMissingSunAccountCode == null)
                    LoadMissingSunAccCode("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadMissingSunAccCode("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_MissingEpicorSupplierId || HasAccessRightsTo_SuperView)
            {
                if (vwMissingEpicorSupplierId == null)
                    LoadMissingEpicorSupplierId("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadMissingEpicorSupplierId("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_MissingPaymentAdviceEMail || HasAccessRightsTo_SuperView)
            {
                if (vwMissingPaymentAdviceEMail == null)
                    LoadMissingPayAdvEMail("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadMissingPayAdvEMail("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_EziBuyOSPaymentList || HasAccessRightsTo_SuperView)
            {
                if (vwEziBuyOSPaymentList == null)
                    LoadEziBuyOSPaymentList("COUNT", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
                else
                    LoadEziBuyOSPaymentList("DETAIL", AlertUserId, SelectedOfficeId, SelectedDepartmentCode, SelectedDepartmentId);
            }

            if (HasAccessRightsTo_OutstandingNTInvoiceDepartmentApproval || HasAccessRightsTo_SuperView)
            {
                if (vwAlertOSNTInvoiceForDeptApproval == null)
                    LoadOSNTInvDeptApv("COUNT", AlertUserId, SelectedOfficeId);
                else
                    LoadOSNTInvDeptApv("DETAIL", AlertUserId, SelectedOfficeId);
            }

            if (HasAccessRightsTo_OutstandingNTInvoiceAccountLevel1Approval || HasAccessRightsTo_SuperView)
            {
                if (vwAlertOSNTInvoiceForAccLevel1Approval == null)
                    LoadOSNTInvAccLvl1Apv("COUNT", AlertUserId, SelectedOfficeId);
                else
                    LoadOSNTInvAccLvl1Apv("DETAIL", AlertUserId, SelectedOfficeId);
            }

            if (HasAccessRightsTo_OutstandingNTInvoiceAccountLevel2Approval || HasAccessRightsTo_SuperView)
            {
                if (vwAlertOSNTInvoiceForAccLevel2Approval == null)
                    LoadOSNTInvAccLvl2Apv("COUNT", AlertUserId, SelectedOfficeId);
                else
                    LoadOSNTInvAccLvl2Apv("DETAIL", AlertUserId, SelectedOfficeId);
            }

            if (HasAccessRightsTo_NTInvoiceAccountsApproved || HasAccessRightsTo_SuperView)
            {
                if (vwAlertNTInvoiceAccountsApproved == null)
                    LoadNTInvAccApv("COUNT", AlertUserId, SelectedOfficeId);
                else
                    LoadNTInvAccApv("DETAIL", AlertUserId, SelectedOfficeId);
            }

            if (HasAccessRightsTo_NewNTVendorApproval || HasAccessRightsTo_SuperView)
            {
                if (vwAlertNewNTVendorApproval == null)
                    LoadNewNTVendorApproval("COUNT", AlertUserId, SelectedOfficeId);
                else
                    LoadNewNTVendorApproval("DETAIL", AlertUserId, SelectedOfficeId);
            }

            if (HasAccessRightsTo_NTVendorAmendment || HasAccessRightsTo_SuperView)
            {
                if (vwAlertNTVendorAmendment == null)
                    LoadNTVendorAmendment("COUNT", AlertUserId, SelectedOfficeId);
                else
                    LoadNTVendorAmendment("DETAIL", AlertUserId, SelectedOfficeId);
            }

        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            this.ddl_Office.SelectedValue = GeneralCriteria.ALL.ToString();
            LoadAllAlertCount();
        }

        #endregion
    }

    #region Alert Type class

    [Serializable]
    public class AlertType : DomainData
    {
        public static AlertType InvUploadFail = new AlertType(Code.InvUploadFail);
        public static AlertType AdvisingBank = new AlertType(Code.AdvisingBank);
        public static AlertType OSUTOrder = new AlertType(Code.OSUTOrder);
        public static AlertType OSBooking = new AlertType(Code.OSBooking);
        public static AlertType OSDoc = new AlertType(Code.OSDoc);
        public static AlertType OSDocOffshore = new AlertType(Code.OSDocOffshore);
        public static AlertType OSDocResubmit = new AlertType(Code.OSDocResubmit);
        public static AlertType SunAccCode = new AlertType(Code.SunAccCode);
        public static AlertType EAdvice = new AlertType(Code.EAdvice);
        public static AlertType EziBuyOSPayment = new AlertType(Code.EziBuyOSPayment);
        public static AlertType OSNTInvDeptApv = new AlertType(Code.OSNTInvDeptApv);
        public static AlertType OSNTInvAccL1Apv = new AlertType(Code.OSNTInvAccL1Apv);
        public static AlertType OSNTInvAccL2Apv = new AlertType(Code.OSNTInvAccL2Apv);
        public static AlertType NTInvAccApv = new AlertType(Code.NTInvAccApv);
        public static AlertType NewNTVendorApv = new AlertType(Code.NewNTVendorApv);
        public static AlertType NTVendorAmendment = new AlertType(Code.NTVendorAmendment);
        public static AlertType EpicorSupplierId = new AlertType(Code.EpicorSupplierId);
        public static int TotalNumberOfAlert = 17;

        private Code _code;

        private enum Code
        {
            InvUploadFail = 0,
            AdvisingBank =1,
            OSUTOrder = 2,
            OSBooking = 3,
            OSDoc = 4,
            OSDocOffshore = 5,
            OSDocResubmit = 6,
            SunAccCode = 7,
            EAdvice = 8,
            EziBuyOSPayment = 9,
            OSNTInvDeptApv = 10,
            OSNTInvAccL1Apv = 11,
            OSNTInvAccL2Apv = 12,
            NTInvAccApv = 13,
            NewNTVendorApv = 14,
            NTVendorAmendment = 15,
            EpicorSupplierId = 16
        }

        private AlertType(Code code)
        {
            this._code = code;
        }

        public int Id
        {
            get
            {
                return Convert.ToUInt16(_code.GetHashCode());
            }
        }

        public string Name
        {
            get
            {
                switch (_code)
                {
                    case Code.InvUploadFail:
                        return "ILS Invoice Upload Fail";
                    case Code.AdvisingBank:
                        return "Vendor Missing L/C Advising Bank";
                    case Code.OSUTOrder:
                        return "UT Order Outstanding to Invoice";
                    case Code.OSBooking:
                        return "Outstanding Booking";
                    case Code.OSDoc:
                        return "Outstanding Document to be Presented to Accounts";
                    case Code.OSDocOffshore:
                        return "Outstanding Document to be Presented to Accounts(Offshore)";
                    case Code.OSDocResubmit:
                        return "Outstanding Document to be Re-submitted to Accounts";
                    case Code.SunAccCode:
                        return "Supplier Missing SUN Account Code";
                    case Code.EpicorSupplierId:
                        return "Supplier Missing Epicor Supplier Id";
                    case Code.EAdvice:
                        return "Supplier Missing Payment Advice E-mail Address";
                    case Code.EziBuyOSPayment:
                        return "EziBuy - Outstanding Payment and Cargo Delivery List";
                    case Code.OSNTInvDeptApv:
                        return "Outstanding Non-Trade Expense Invoice Pending for Department Approval";
                    case Code.OSNTInvAccL1Apv:
                        return "Outstanding Non-Trade Expense Invoice Pending for Accounts 1st Level Approval";
                    case Code.OSNTInvAccL2Apv:
                        return "Outstanding Non-Trade Expense Invoice Pending for Accounts 2nd Approval";
                    case Code.NTInvAccApv:
                        return "Outstanding Non-Trade Expense Invoice Pending for Interface Genearteion";
                    case Code.NewNTVendorApv:
                        return "Non-Trade Vendor Pending For Approval";
                    case Code.NTVendorAmendment:
                        return "Non-Trade Vendor Amendment";
                    default:
                        return "ERROR";
                }
            }
        }
        
        public static AlertType getType(int id)
        {
            if (id == Code.InvUploadFail.GetHashCode()) return AlertType.InvUploadFail;
            else if (id == Code.AdvisingBank.GetHashCode()) return AlertType.AdvisingBank;
            else if (id == Code.OSBooking.GetHashCode()) return AlertType.OSBooking;
            else if (id == Code.OSDoc.GetHashCode()) return AlertType.OSDoc;
            else if (id == Code.OSDocOffshore.GetHashCode()) return AlertType.OSDocOffshore;
            else if (id == Code.OSDocResubmit.GetHashCode()) return AlertType.OSDocResubmit;
            else if (id == Code.OSUTOrder.GetHashCode()) return AlertType.OSUTOrder;
            else if (id == Code.OSDocResubmit.GetHashCode()) return AlertType.OSDocResubmit;
            else if (id == Code.SunAccCode.GetHashCode()) return AlertType.SunAccCode;
            else if (id == Code.EpicorSupplierId.GetHashCode()) return AlertType.EpicorSupplierId;
            else if (id == Code.EAdvice.GetHashCode()) return AlertType.EAdvice;
            else if (id == Code.EziBuyOSPayment.GetHashCode()) return AlertType.EziBuyOSPayment;
            else if (id == Code.OSNTInvDeptApv.GetHashCode()) return AlertType.OSNTInvDeptApv;
            else if (id == Code.OSNTInvAccL1Apv.GetHashCode()) return AlertType.OSNTInvAccL1Apv;
            else if (id == Code.OSNTInvAccL2Apv.GetHashCode()) return AlertType.OSNTInvAccL2Apv;
            else if (id == Code.NTInvAccApv.GetHashCode()) return AlertType.NTInvAccApv;
            else if (id == Code.NewNTVendorApv.GetHashCode()) return AlertType.NewNTVendorApv;
            else if (id == Code.NTVendorAmendment.GetHashCode()) return AlertType.NTVendorAmendment;
            else return null;
        }
    }
    #endregion

}
