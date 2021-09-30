using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.types;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.shipping;
using com.next.isam.webapp.commander;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class NSLSZOrderReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControl();
            }
        }

        void initControl()
        {
            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_Destination.bindList(WebUtil.getCustomerDestinationList(), "DestinationDesc", "CustomerDestinationId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_LoadingPort.bindList(WebUtil.getShipmentPortList(), "ShipmentPortDescription", "ShipmentPortId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_PackingMethod.bindList(WebUtil.getPackingMethodList(), "PackingMethodDescription", "PackingMethodId", "", "--All--", GeneralCriteria.ALL.ToString());

            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);

            cbl_Customer.DataSource = WebUtil.getCustomerList();
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();


            cbl_ShipmentMethod.DataSource = WebUtil.getShipmentMethodList();
            cbl_ShipmentMethod.DataTextField = "ShipmentMethodDescription";
            cbl_ShipmentMethod.DataValueField = "ShipmentMethodId";
            cbl_ShipmentMethod.DataBind();

            foreach (ListItem item in cbl_Customer.Items)
            {
                if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                    item.Selected = true;
            }
            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                item.Selected = true;
            }            
        }

        ReportClass genReport()
        {
            string invoicePrefix = "";
            int invoiceSeqFrom = 0;
            int invoiceSeqTo = 0;
            int invoiceYear = 0;
            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;
            DateTime invoiceUploadDateFrom = DateTime.MinValue;
            DateTime invoiceUploadDateTo = DateTime.MinValue;
            DateTime customerAtWHDateFrom = DateTime.MinValue;
            DateTime customerAtWHDateTo = DateTime.MinValue;
            int officeId = Convert.ToInt32(ddl_Office.SelectedValue);
            int customerDestination = Convert.ToInt32(ddl_Destination.SelectedValue);
            int shipmentPortId = Convert.ToInt32(ddl_LoadingPort.SelectedValue);
            int packingMethodId = Convert.ToInt32(ddl_PackingMethod.SelectedValue);
            int isSZUTOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            int isDualSourcing = Convert.ToInt32(ddl_DualSourcingOrder.SelectedValue);
            int isOPROrder = Convert.ToInt32(ddl_OPROrder.SelectedValue);
            int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);

            if (txt_InvoiceNoFrom.Text.Trim() != "")
            {
                invoicePrefix = WebUtil.getInvoicePrefix(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqFrom = WebUtil.getInvoiceSeq(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqTo = WebUtil.getInvoiceSeq(txt_InvoiceNoTo.Text.Trim());
                invoiceYear = WebUtil.getInvoiceYear(txt_InvoiceNoFrom.Text.Trim());
            }
            if (txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }


            if (txt_InvoiceUploadDateFrom.Text.Trim() != "")
            {
                invoiceUploadDateFrom = DateTimeUtility.getDate(txt_InvoiceUploadDateFrom.Text.Trim());
                if (txt_InvoiceUploadDateTo.Text.Trim() == "")
                    txt_InvoiceUploadDateTo.Text = txt_InvoiceUploadDateFrom.Text;
                invoiceUploadDateTo = DateTimeUtility.getDate(txt_InvoiceUploadDateTo.Text.Trim());
            }

            if (txt_AtWHDateFrom.Text.Trim() != "")
            {
                customerAtWHDateFrom = DateTimeUtility.getDate(txt_AtWHDateFrom.Text.Trim());
                if (txt_AtWHDateTo.Text.Trim() == "")
                    txt_AtWHDateTo.Text = txt_AtWHDateFrom.Text;
                customerAtWHDateTo = DateTimeUtility.getDate(txt_AtWHDateTo.Text.Trim());
            }
            
            TypeCollector productTeamList = TypeCollector.Inclusive;
            if (uclProductTeam.ProductCodeId != int.MinValue)
                productTeamList.append(uclProductTeam.ProductCodeId);

            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (officeId == -1)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                foreach (OfficeRef office in userOfficeList)
                {
                    officeIdList.append(office.OfficeId);

                    if (uclProductTeam.ProductCodeId == int.MinValue)
                    {
                        ArrayList pt = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, office.OfficeId, this.LogonUserId, GeneralCriteria.ALLSTRING);
                        foreach (OfficeStructureRef os in pt)
                        {
                            productTeamList.append(os.OfficeStructureId);
                        }
                    }
                }
            }
            else
            {
                if (uclProductTeam.ProductCodeId == int.MinValue)
                {
                    ArrayList pt = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, officeId, this.LogonUserId, GeneralCriteria.ALLSTRING);
                    foreach (OfficeStructureRef os in pt)
                    {
                        productTeamList.append(os.OfficeStructureId);
                    }
                }
            }

            TypeCollector shipmentStatusList = TypeCollector.Inclusive ;
            if (ddl_ShipmentStatus.SelectedValue == "-1")
            {
                shipmentStatusList.append(ContractWFS.PENDING_FOR_APPROVAL.Id);
                shipmentStatusList.append(ContractWFS.AMEND.Id);
                shipmentStatusList.append(ContractWFS.APPROVED.Id);
                shipmentStatusList.append(ContractWFS.PO_PRINTED.Id);
                shipmentStatusList.append(ContractWFS.INVOICED.Id);
            }
            else if (ddl_ShipmentStatus.SelectedValue == "1")
            {
                shipmentStatusList.append(ContractWFS.INVOICED.Id);
            }
            else
            {
                shipmentStatusList.append(ContractWFS.PENDING_FOR_APPROVAL.Id);
                shipmentStatusList.append(ContractWFS.AMEND.Id);
                shipmentStatusList.append(ContractWFS.APPROVED.Id);
                shipmentStatusList.append(ContractWFS.PO_PRINTED.Id);
            }
            

            TypeCollector customerTypeCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_Customer.Items)
            {
                if (item.Selected)
                    customerTypeCollector.append(Convert.ToInt32(item.Value));
            }

            
            TypeCollector shipmentMethodCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                if (item.Selected)
                    shipmentMethodCollector.append(Convert.ToInt32(item.Value));
            }

            return ShippingReportManager.Instance.getNSLSZOrderReport(customerAtWHDateFrom, customerAtWHDateTo, invoiceDateFrom, invoiceDateTo, invoiceUploadDateFrom,
                invoiceUploadDateTo, invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, officeIdList, productTeamList, shipmentPortId, customerDestination,
                packingMethodId, shipmentMethodCollector, customerTypeCollector, shipmentStatusList, isSZUTOrder, isDualSourcing, isOPROrder,isLDPOrder, this.LogonUserId);            
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "NSLSZOrder");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "NSLSZOrder");
        }
    }
}
