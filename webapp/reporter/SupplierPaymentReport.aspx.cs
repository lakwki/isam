using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.webapp.commander;
using com.next.isam.domain.types;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.invoice;
using com.next.isam.reporter.dataserver;
using com.next.isam.reporter.supplierpayment;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.common;

using com.next.common.datafactory.worker;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;

namespace com.next.isam.webapp.reporter
{
    public partial class SupplierPaymentReport : com.next.isam.webapp.usercontrol.PageTemplate
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
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "Supplier Payment Report");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "Supplier Payment Report");
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

            ddl_PaymentReferenceNo.DataSource = ReporterWorker.Instance.GetPaymentReferenceCodeList();
            ddl_PaymentReferenceNo.DataBind();
            ddl_PaymentReferenceNo.Items.Insert(0, new ListItem("--All--", ""));

            
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


            foreach (ListItem item in cbl_Customer.Items)
            {
                if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                    item.Selected = true;
            }
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                item.Selected = true;
            }
        }

        private ReportClass genReport()
        {
            int i;
            
            DateTime receiptDateFrom = DateTime.MinValue;
            DateTime receiptDateTo = DateTime.MinValue;

            if (txt_PaymentDateFrom.Text.Trim() != "")
            {
                DateTime.TryParse(txt_PaymentDateFrom.Text, out receiptDateFrom);
                if (receiptDateFrom == null)
                    receiptDateFrom = DateTime.MinValue;

                DateTime.TryParse(txt_PaymentDateTo.Text, out receiptDateTo);
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
                }
                currencyName = "ALL";
            }

            string paymentReferenceNo;
            paymentReferenceNo = ddl_PaymentReferenceNo.SelectedValue;


            ArrayList seasonIdList = new ArrayList();
            string seasonName = string.Empty;
            if (ddl_Season.SelectedValue != "-1")
            {
                seasonIdList.Add(Convert.ToInt32(ddl_Season.SelectedValue));
                //seasonName = (ddl_Season.Items[ddl_Season.SelectedItem].Text);
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


            //int productTeamId = -1;
            //if (uclProductTeam.ProductCodeId != int.MinValue)
            //    productTeamId=uclProductTeam.ProductCodeId;
            /*
            ArrayList officeIdList = new ArrayList();
            string officeName = string.Empty;
            if (ddl_Office.SelectedValue != "-1")
            {
                officeIdList.Add(Convert.ToInt32(ddl_Office.SelectedValue));
                officeName = (ddl_Office.selectedText);
            }
            else
            {
                for (i = 1; i < ddl_Office.Items.Count; i++)
                {
                    officeIdList.Add(Convert.ToInt32(ddl_Office.Items[i].Value));
                }
                officeName = "ALL";
            }

            ArrayList productTeamIdList = new ArrayList();
            string productTeamName = string.Empty;
            if (uclProductTeam.ProductCodeId != int.MinValue)
            {
                productTeamIdList.Add(uclProductTeam.ProductCodeId);
                foreach (OfficeStructureRef row in vsUserAllProductTeamList)
                {
                    if (row.OfficeStructureId == uclProductTeam.ProductCodeId)
                    {
                        productTeamName = row.Code + " - " + row.Description;
                        break;
                    }
                }
            }
            else
            {
                foreach (OfficeStructureRef row in vsUserAllProductTeamList)
                    productTeamIdList.Add(row.OfficeStructureId);
                //productTeamIdList.Add(-1);
                productTeamName = "ALL";
            }
            int departmentId = ddl_Department.SelectedValue == "" ? -1 : Convert.ToInt32(ddl_Department.SelectedValue);
            string departmentName = (ddl_Department.SelectedValue == "" ? "ALL" : ddl_Department.selectedText);

            */
            string officeName = string.Empty;
            string productTeamName = string.Empty;

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

            TypeCollector productTeamList = TypeCollector.Inclusive;
            if (uclProductTeam.ProductCodeId != int.MinValue)
            {
                productTeamList.append(uclProductTeam.ProductCodeId);
                productTeamName = ((TextBox)uclProductTeam.FindControl("txtName")).Text;
            }
            else
            {
                foreach (int Id in officeIdList.Values)
                {
                    ArrayList pt = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, Id, this.LogonUserId, GeneralCriteria.ALLSTRING);
                    foreach (OfficeStructureRef os in pt)
                        productTeamList.append(os.OfficeStructureId);
                }
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

            int vendorId = -1;
            string vendorName = "ALL";
            if (txt_Supplier.VendorId != int.MinValue)
            {
                vendorId = txt_Supplier.VendorId;
                vendorName = txt_Supplier.ToString();

            }            

            SupplierPaymentRpt rpt = SupplierPaymentReportManager.Instance.getSupplierPaymentReport(
                receiptDateFrom, receiptDateTo, fiscalYear, periodFrom, periodTo, baseCurrencyId, customerIdList, currencyIdList,
                paymentReferenceNo, officeIdList,  handlingOfficeId, seasonIdList,
                productTeamList,  tradingAgencyIdList, purchaseTermIdList, paymentTermIdList, this.LogonUserId, departmentIdList, vendorId,
                baseCurrencyName, customerName, currencyName, officeName, seasonName,
                productTeamName, tradingAgencyName, purchaseTermName, paymentTermName, departmentName, vendorName, rad_Version.SelectedValue);

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
