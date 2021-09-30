using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.customerreceipt;
using com.next.isam.dataserver.worker;
using com.next.isam.reporter.dataserver;
using com.next.isam.appserver.common;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.common.datafactory.worker;


namespace com.next.isam.webapp.reporter
{
    public partial class CustomerReceiptReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        bool HasAccessRights_SuperView;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                initControl();
            }
        }

        ArrayList vsUserAllProductTeamList
        {
            get { return (ArrayList)ViewState["UserProductTeamList"]; }
            set { ViewState["UserProductTeamList"] = value; }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "Customer Receipt Report");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "Customer Receipt Report");
        }

        void initControl()
        {
            int nUserId;

            HasAccessRights_SuperView = false;
            //HasAccessRights_SuperView = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcApplication.Id, ISAMModule.lcApplication.SuperView);
            //********** For Testing **************************
            //if (this.LogonUserId==574)
            //  HasAccessRights_SuperView = true;
            //********** For Testing **************************
            nUserId = ((HasAccessRights_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);

            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
            ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());
            //ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());

            
            ddl_BaseCurrency.bindList(AccountWorker.Instance.getBaseCurrencyList(), "CurrencyCode", "CurrencyId","");
            foreach (ListItem item in ddl_BaseCurrency.Items) item.Selected = (item.Text == "USD");

            ddl_Currency.bindList(AccountWorker.Instance.getOrderCurrencyList(), "CurrencyCode", "CurrencyId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_PurchaseTerm.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());

            //ddl_ReceiptReferenceNo.bindList(WebUtil.getCustomerDestinationList(), "DestinationDesc", "CustomerDestinationId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_ReceiptReferenceNo.DataSource = ReporterWorker.Instance.GetPaymentReferenceCodeList();
            ddl_ReceiptReferenceNo.DataBind();
            ddl_ReceiptReferenceNo.Items.Insert(0, new ListItem("--All--", ""));

            
            ddl_Season.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_Year.DataSource = WebUtil.getBudgetYearList();
            ddl_Year.DataBind();
            ddl_Year.Items.Insert(0, new ListItem("--All--", "-1"));

            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
            vsUserAllProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralCriteria.ALL, nUserId, GeneralCriteria.ALLSTRING);

            cbl_Customer.DataSource = WebUtil.getCustomerList();
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();

            cbl_TradingAgency.DataSource = WebUtil.getTradingAgencyList();
            cbl_TradingAgency.DataTextField = "ShortName";
            cbl_TradingAgency.DataValueField = "TradingAgencyId";
            cbl_TradingAgency.DataBind();

            ddl_PaymentTerm.bindList(CommonWorker.Instance.getPaymentTermList(), "PaymentTermDescription", "PaymentTermId", "", "--All--", GeneralCriteria.ALL.ToString());

            /*
            cbl_ShipmentMethod.DataSource = WebUtil.getShipmentMethodList();
            cbl_ShipmentMethod.DataTextField = "ShipmentMethodDescription";
            cbl_ShipmentMethod.DataValueField = "ShipmentMethodId";
            cbl_ShipmentMethod.DataBind();
            */

            foreach (ListItem item in cbl_Customer.Items)
            {
                if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                    item.Selected = true;
            }
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                item.Selected = true;
            }

            /*
            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                item.Selected = true;
            }
            */
        }

        private ReportClass genReport()
        {
            int i;

            DateTime receiptDateFrom = DateTime.MinValue;
            DateTime receiptDateTo = DateTime.MinValue;

            if (txt_ReceiptDateFrom.Text.Trim() != "")
            {
                //receiptDateFrom = DateTime.ParseExact(txt_ReceiptDateFrom.Text.Trim(), "dd/MM/yyyy", null);
                //receiptDateTo = DateTime.ParseExact(txt_ReceiptDateTo.Text.Trim(), "dd/MM/yyyy", null);
                DateTime.TryParse(txt_ReceiptDateFrom.Text, out receiptDateFrom);
                if (receiptDateFrom == null)
                    receiptDateFrom = DateTime.MinValue;

                DateTime.TryParse(txt_ReceiptDateTo.Text, out receiptDateTo);
                if (receiptDateTo == null)
                    receiptDateTo = DateTime.MinValue;
            }
            int fiscalYear = Convert.ToInt32(ddl_Year.SelectedValue);
            int periodFrom = Convert.ToInt32(ddl_PeriodFrom.SelectedValue);
            int periodTo = Convert.ToInt32(ddl_PeriodTo.SelectedValue);

            ArrayList customerIdList = new ArrayList();
            string customerName = string.Empty;
            foreach (ListItem item in cbl_Customer.Items)
                if (item.Selected)
                {
                    customerIdList.Add(Convert.ToInt32(item.Value));
                    customerName += (customerName == string.Empty ? "" : ", ") + item.Text;
                }

            int baseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            string baseCurrencyName = ddl_BaseCurrency.selectedText;

            ArrayList currencyIdList = new ArrayList();
            string currencyName = string.Empty;
            if (ddl_Currency.SelectedValue != "-1")
            {
                currencyIdList.Add(Convert.ToInt32(ddl_Currency.SelectedValue));
                currencyName = ddl_Currency.selectedText;
            }
            else
            {
                for (i = 1; i < ddl_Currency.Items.Count; i++)
                {
                    currencyIdList.Add(Convert.ToInt32(ddl_Currency.Items[i].Value));
                    //currencyName += (currencyName == string.Empty ? ddl_Currency.Items[i].Text : "," + ddl_Currency.Items[i].text);
                }
                currencyName = "ALL";
            }

            /*  
            ArrayList receiptReferenceNoList = new ArrayList();
            if (ddl_ReceiptReferenceNo.SelectedValue != "-1")
                receiptReferenceNoList.Add(ddl_ReceiptReferenceNo.selectedText);
            else
                for (i=1; i<ddl_ReceiptReferenceNo.Items.Count; i++)
                    receiptReferenceNoList.Add(ddl_ReceiptReferenceNo.Items[i].Text);
            */
            string receiptReferenceNo;
            receiptReferenceNo = ddl_ReceiptReferenceNo.SelectedValue;

            string officeName = string.Empty;
            string productTeamName = string.Empty;
            /*
            ArrayList officeIdList = new ArrayList();
            if (ddl_Office.SelectedValue != "-1")
            {
                officeIdList.Add(Convert.ToInt32(ddl_Office.SelectedValue));
                //officeName = (ddl_Office.Items[ddl_Office.selecetedItem].Text);
                officeName = (ddl_Office.selectedText);
            }
            else
            {
                for (i = 1; i < ddl_Office.Items.Count; i++)
                {
                    officeIdList.Add(Convert.ToInt32(ddl_Office.Items[i].Value));
                    //officeName += (officeName == string.Empty ? "" : ",") + ddl_Office.Items[i].Text.Replace("Office", "").Trim();
                }
                officeName = "ALL";
            }
            //ArrayList productTeamIdList = new ArrayList();
            //string productTeamName = string.Empty;
            if (uclProductTeam.ProductCodeId != int.MinValue)
            {
                //productTeamIdList.Add(uclProductTeam.ProductCodeId);
                //foreach (OfficeStructureRef row in vsUserAllProductTeamList)
                //{
                //    if (row.OfficeStructureId == uclProductTeam.ProductCodeId)
                //    {
                //        productTeamName = row.Code + " - " + row.Description;
                //        break;
                //    }
                //}
                productTeamName = GeneralWorker.Instance.getProductCodeDefByKey(uclProductTeam.ProductCodeId).CodeDescription;
            }
            else
            {
                //foreach (OfficeStructureRef row in vsUserAllProductTeamList)
                //    productTeamIdList.Add(row.OfficeStructureId);
                //productTeamIdList.Add(-1);
                productTeamName = "ALL";
            }

            
            //int productTeamId = -1;
            //if (uclProductTeam.ProductCodeId != int.MinValue)
            //    productTeamId=uclProductTeam.ProductCodeId;
            
            int departmentId = ddl_Department.SelectedValue == "" ? -1 : Convert.ToInt32(ddl_Department.SelectedValue);
            string departmentName = (departmentId == -1 ? "ALL" : ddl_Department.selectedText);
            */

            int officeId = -1;
            int officeGroupId = Convert.ToInt32(ddl_Office.SelectedValue);
            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (officeGroupId == -1)
            {
                officeName = "ALL";
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                foreach (OfficeRef office in userOfficeList)
                    officeIdList.append(office.OfficeId);
            }
            else
            {
                officeName = (ddl_Office.selectedText);
                if (ddl_Office.selectedText.Contains("+"))
                {
                    ArrayList officeList = CommonWorker.Instance.getOfficeListByReportOfficeGroupId(Convert.ToInt32(officeGroupId));
                    foreach (OfficeRef office in officeList)
                        officeIdList.append(office.OfficeId);
                }
                else
                    officeIdList.append(officeGroupId);
            }

            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);
            if (!officeIdList.contains(OfficeId.DG.Id))
                handlingOfficeId = -1;

            //TypeCollector productTeamList = TypeCollector.Inclusive;
            if (uclProductTeam.ProductCodeId != int.MinValue)
            {
                //productTeamName = GeneralWorker.Instance.getProductCodeDefByKey(uclProductTeam.ProductCodeId).CodeDescription;
                productTeamName = ((TextBox)uclProductTeam.FindControl("txtName")).Text;
            }
            else
            {
                productTeamName = "ALL";
            }

            //departmentId = ddl_Department.SelectedValue == "" ? -1 : Convert.ToInt32(ddl_Department.SelectedValue);
            int departmentId = -1;
            string departmentName = string.Empty;
            TypeCollector departmentIdList = TypeCollector.Inclusive;
            if (ddl_Department.SelectedValue == "" || ddl_Department.SelectedIndex == 0)
            {
                departmentId = -1;
                departmentName = "ALL";
            }
            else
            {
                string[] idList = ddl_Department.SelectedItem.Value.Split(char.Parse("|"));
                if (idList.Length > 0)
                    foreach (string str in idList)
                        departmentIdList.append(Convert.ToInt32(str));
                departmentName = ddl_Department.selectedText;
            }


            ArrayList seasonIdList = new ArrayList();
            string seasonName = string.Empty;
            if (ddl_Season.SelectedValue != "-1")
            {
                seasonIdList.Add(Convert.ToInt32(ddl_Season.SelectedValue));
                seasonName = (ddl_Season.selectedText);
            }
            else
            {
                for (i = 1; i < ddl_Season.Items.Count; i++)
                {
                    seasonIdList.Add(Convert.ToInt32(ddl_Season.Items[i].Value));
                }
                seasonName = "ALL";
            }


            ArrayList tradingAgencyIdList = new ArrayList();
            string tradingAgencyName = string.Empty;
            foreach (ListItem item in cbl_TradingAgency.Items)
                if (item.Selected)
                {
                    tradingAgencyIdList.Add(Convert.ToInt32(item.Value));
                    tradingAgencyName += (tradingAgencyName == string.Empty ? "" : ", ") + item.Text;
                }

            ArrayList purchaseTermIdList = new ArrayList();
            string purchaseTermName = string.Empty;
            if (ddl_PurchaseTerm.SelectedValue != "-1")
            {
                purchaseTermIdList.Add(Convert.ToInt32(ddl_PurchaseTerm.SelectedValue));
                purchaseTermName = ddl_PurchaseTerm.selectedText;
            }
            else
                for (i = 1; i < ddl_PurchaseTerm.Items.Count; i++)
                {
                    purchaseTermIdList.Add(Convert.ToInt32(ddl_PurchaseTerm.Items[i].Value));
                    purchaseTermName += (purchaseTermName == string.Empty ? "" : ", ") + ddl_PurchaseTerm.Items[i].Text;
                }

            ArrayList paymentTermIdList = new ArrayList();
            string paymentTermName = string.Empty;
            if (ddl_PaymentTerm.SelectedValue != "-1")
            {
                paymentTermIdList.Add(Convert.ToInt32(ddl_PaymentTerm.SelectedValue));
                paymentTermName = ddl_PaymentTerm.selectedText;
            }
            else
                for (i = 1; i < ddl_PaymentTerm.Items.Count; i++)
                {
                    paymentTermIdList.Add(Convert.ToInt32(ddl_PaymentTerm.Items[i].Value));
                    paymentTermName += (paymentTermName == string.Empty ? "" : ", ") + ddl_PaymentTerm.Items[i].Text;
                }
            
            /*
            TypeCollector shipmentMethodCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                if (item.Selected)
                    shipmentMethodCollector.append(Convert.ToInt32(item.Value));
            }
            */


            int vendorId = -1;
            string vendorName = "ALL";
            if (txt_Supplier.VendorId != int.MinValue)
            {
                vendorId = txt_Supplier.VendorId;
                vendorName = txt_Supplier.ToString();

            }

            CustomerReceiptRpt rpt = CustomerReceiptReportManager.Instance.getCustomerReceiptReport(
                receiptDateFrom, receiptDateTo, fiscalYear, periodFrom, periodTo, baseCurrencyId, customerIdList, currencyIdList,
                //receiptReferenceNoList, 
                receiptReferenceNo, officeIdList, handlingOfficeId, seasonIdList,
                //departmentId, 
                departmentIdList, vendorId, uclProductTeam.ProductCodeId,
                tradingAgencyIdList, purchaseTermIdList, paymentTermIdList, this.LogonUserId
                , baseCurrencyName, customerName, currencyName, officeName, seasonName,
                departmentName, productTeamName, vendorName, tradingAgencyName, purchaseTermName, paymentTermName,
                int.Parse(this.radDateType.SelectedValue), int.Parse(this.ddl_SampleOrder.SelectedValue), rad_Version.SelectedValue);

            return rpt;
        }

        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_Department.Items.Clear();
            if (ddl_Office.SelectedValue != "-1")
            {
                ArrayList arr_department = WebUtil.getProductDepartmentListByOfficeGroupId(Convert.ToInt32(ddl_Office.SelectedValue), this.LogonUserId);
                ddl_Department.bindList(arr_department, "Description", "ProductDepartmentId", "", "--All--", GeneralCriteria.ALL.ToString());
                WebUtil.convertToDepartmentGroupList(ddl_Department.Items);
            }
        }

    }
}
