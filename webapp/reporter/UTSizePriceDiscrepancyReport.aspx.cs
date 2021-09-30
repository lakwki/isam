using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.util;
using com.next.isam.reporter.helper;
using com.next.common.domain.types;
using System.Collections;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.reporter.shipping;

namespace com.next.isam.webapp.reporter
{
    public partial class UTSizePriceDiscrepancyReport : System.Web.UI.Page
    {
        public int LogonUserId
        {
            get { return ConvertUtility.toInt32(Context.Request.ServerVariables["AUTH_USER"]); }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) 
            {
                initControl();
            }
        }

        protected void initControl()
        {
            uclOfficeSelection.UserId = this.LogonUserId;

            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!checking(sender))
            {
                return;
            }

            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "UTDiscrepancyReport");

        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!checking(sender))
            {
                return;
            }

            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "UTDiscrepancyReport");
        }

        ReportClass genReport()
        {
            TypeCollector officeList = uclOfficeSelection.getOfficeList();
            DateTime custAtWHDateFrom = DateTime.MinValue;
            DateTime custAtWHDateTo = DateTime.MinValue;
            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;
            int vendorId = -1;

            if (txt_CustomerAtWHDateFrom.Text.Trim() != "")
            {
                custAtWHDateFrom = DateTimeUtility.getDate(txt_CustomerAtWHDateFrom.Text.Trim());
                if (txt_CustomerAtWHDateTo.Text.Trim() == "")
                    txt_CustomerAtWHDateTo.Text = txt_CustomerAtWHDateFrom.Text;
                custAtWHDateTo = DateTimeUtility.getDate(txt_CustomerAtWHDateTo.Text.Trim());
            }
            if (txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            if (txt_Supplier.VendorId != int.MinValue)
                vendorId = txt_Supplier.VendorId;

            return ShippingReportManager.Instance.getUTDiscrepancyReport(officeList, custAtWHDateFrom, custAtWHDateTo, invoiceDateFrom, invoiceDateTo, vendorId, this.LogonUserId);
        }

        private bool checking(object sender)
        {
            //Please enter search criteria on one of below search criteria.
            int totalOffice = 0;
            TypeCollector officeList = uclOfficeSelection.getOfficeList();
            ICollection ids = officeList.Values;
            foreach (int id in ids)
            {
                totalOffice++;
            }
            if (txt_CustomerAtWHDateFrom.Text.Trim() == "" && txt_CustomerAtWHDateTo.Text.Trim() == "" && txt_InvoiceDateFrom.Text.Trim() == "" && txt_InvoiceDateTo.Text.Trim() == "" && txt_Supplier.KeyTextBox.Text.Trim() == "" && totalOffice == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Please enter at least 1 searching criteria.');", true);
                return false;
            }
            if (totalOffice == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Please choose at least one or more Office');", true);
                return false;
            }
            if ((txt_CustomerAtWHDateFrom.Text != "" && txt_CustomerAtWHDateTo.Text == "") || (txt_CustomerAtWHDateTo.Text != "" && txt_CustomerAtWHDateFrom.Text == ""))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Invalid Customer At-WareHouse Date Range');", true);
                return false;
            }
            if ((txt_InvoiceDateFrom.Text != "" && txt_InvoiceDateTo.Text == "") || (txt_InvoiceDateTo.Text != "" && txt_InvoiceDateFrom.Text == ""))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Invalid Invoice Date Range');", true);
                return false;
            }
            /*if (txt_CustomerAtWHDateFrom.Text.Trim()=="" && txt_CustomerAtWHDateTo.Text.Trim() == "" && txt_InvoiceDateFrom.Text.Trim() == "" && txt_InvoiceDateTo.Text.Trim() == "" && txt_Supplier.KeyTextBox.Text.Trim() == "" && totalOffice == 0 && totalcountry==0 && totalport == 0 && totalshipment == 0) 
            {
                correct = false;
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Please enter at least 1 searching criteria.');", true);
            }*/
            return true;
        }
    }
}