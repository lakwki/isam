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

namespace com.next.isam.webapp.account
{
    public partial class DCNoteDiscrepancy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected DataSet LoadInvoiceDataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("TradingAgency");
            dt.Columns.Add("InvoiceNumber");
            dt.Columns.Add("InvoiceDate");
            dt.Columns.Add("SuppName");
            dt.Columns.Add("SuppSUNCOA");
            dt.Columns.Add("ProdTeam");
            dt.Columns.Add("SuppInvNo");
            dt.Columns.Add("ContractNo");
            dt.Columns.Add("Ccy");
            dt.Columns.Add("InvAmt");
            dt.Columns.Add("SuppInvAmt");
            dt.Columns.Add("DocRecDateByAcc");
            dt.Columns.Add("ExtractDate");
            dt.Columns.Add("Customer");
            dt.Columns.Add("Office");
            dt.Columns.Add("DlyNo");
            dt.Columns.Add("ARDate");
            dt.Columns.Add("ARAmt");
            dt.Columns.Add("SalesAmt");
            dt.Columns.Add("Diff");
            dt.Columns.Add("DCNo");
            dt.Columns.Add("Status");
            DataRow dr = dt.NewRow();

            dr[0] = "NSL";
            dr[1] = "HKR/21101/2008";
            dr[2] = "21/10/2008";
            dr[3] = "NEXT MANUFACTURING (SHENZHEN) LIMITED";
            dr[4] = "1413201";
            dr[5] = "GJKW";
            dr[6] = "INV1008/01";
            dr[7] = "OA8402587-4";
            dr[8] = "USD";
            dr[9] = "19,410.95";
            dr[10] = "19,410.95";
            dr[13] = "DIR";
            dr[14] = "HK";
            dr[15] = "1";
            dr[16] = "14/12/2008";
            dr[17] = "1100";
            dr[18] = "1000";
            dr[19] = "100";
            dr[20] = "C0249480";
            dr[21] = "APPROVED";
            dt.Rows.Add(dr);

            for (int i = 0; i < 100; i++)
            {
                dr = dt.NewRow();
                dr[0] = "NSL";
                dr[1] = "HKR/00258/2008";
                dr[2] = "31/10/2008";
                dr[3] = "NEXT MANUFACTURING (SHENZHEN) LIMITED";
                dr[4] = "1413201";
                dr[5] = "WT";
                dr[6] = "TB0801079";
                dr[7] = "OA8402587-4";
                dr[8] = "USD";
                dr[9] = "19,410.95";
                dr[10] = "19,410.95";
                dr[13] = "DIR";
                dr[14] = "HK";
                dr[15] = "1";
                dr[16] = "14/12/2008";
                dr[17] = "1100";
                dr[18] = "1000";
                dr[19] = "100";
                dr[20] = "C0249480";
                dr[21] = "APPROVED";

                dt.Rows.Add(dr);

                dr = dt.NewRow();
                dr[0] = "NSL";
                dr[1] = "HKR/00258/2008";
                dr[2] = "31/10/2008";
                dr[3] = "NEXT MANUFACTURING (SHENZHEN) LIMITED";
                dr[4] = "1413201";
                dr[5] = "WT";
                dr[6] = "TB0801079";
                dr[7] = "OA8402587-4";
                dr[8] = "USD";
                dr[9] = "19,410.95";
                dr[10] = "19,410.95";
                dr[13] = "RET";
                dr[14] = "HK";
                dr[15] = "1";
                dr[16] = "14/12/2008";
                dr[17] = "1100";
                dr[18] = "1000";
                dr[19] = "100";
                dr[20] = "C0249480";
                dr[21] = "APPROVED";

                dt.Rows.Add(dr);
                dr = dt.NewRow();
                dr[0] = "NSL";
                dr[1] = "HKR/00258/2008";
                dr[2] = "31/10/2008";
                dr[3] = "NEXT MANUFACTURING (SHENZHEN) LIMITED";
                dr[4] = "1413201";
                dr[5] = "WT";
                dr[6] = "TB0801079";
                dr[7] = "OA8402587-4";
                dr[8] = "USD";
                dr[9] = "19,410.95";
                dr[10] = "19,410.95";
                dr[13] = "DIR";
                dr[14] = "HK";
                dr[15] = "1";
                dr[16] = "14/12/2008";
                dr[17] = "1100";
                dr[18] = "1000";
                dr[19] = "100";
                dr[20] = "C0249480";
                dr[21] = "APPROVED";

                dt.Rows.Add(dr);
                dr = dt.NewRow();
                dr[0] = "NSL";
                dr[1] = "HKR/00258/2008";
                dr[2] = "31/10/2008";
                dr[3] = "NEXT MANUFACTURING (SHENZHEN) LIMITED";
                dr[4] = "1413201";
                dr[5] = "WT";
                dr[6] = "TB0801079";
                dr[7] = "OA8402587-4";
                dr[8] = "USD";
                dr[9] = "19,410.95";
                dr[10] = "19,410.95";
                dr[13] = "RET";
                dr[14] = "HK";
                dr[15] = "1";
                dr[16] = "14/12/2008";
                dr[17] = "1100";
                dr[18] = "1000";
                dr[19] = "100";
                dr[20] = "C0249480";
                dr[21] = "APPROVED";

                dt.Rows.Add(dr);
                dr = dt.NewRow();
                dr[0] = "NSL";
                dr[1] = "HKR/00258/2008";
                dr[2] = "31/10/2008";
                dr[3] = "NEXT MANUFACTURING (SHENZHEN) LIMITED";
                dr[4] = "1413201";
                dr[5] = "WT";
                dr[6] = "TB0801079";
                dr[7] = "OA8402587-4";
                dr[8] = "USD";
                dr[9] = "19,410.95";
                dr[10] = "19,410.95";
                dr[13] = "RET";
                dr[14] = "HK";
                dr[15] = "1";
                dr[16] = "14/12/2008";
                dr[17] = "1100";
                dr[18] = "1000";
                dr[19] = "100";
                dr[20] = "C0249480";
                dr[21] = "APPROVED";

                dt.Rows.Add(dr);
                dr = dt.NewRow();
                dr[0] = "NSL";
                dr[1] = "HKR/00258/2008";
                dr[2] = "31/10/2008";
                dr[3] = "NEXT MANUFACTURING (SHENZHEN) LIMITED";
                dr[4] = "1413201";
                dr[5] = "WT";
                dr[6] = "TB0801079";
                dr[7] = "OA8402587-4";
                dr[8] = "USD";
                dr[9] = "19,410.95";
                dr[10] = "19,410.95";
                dr[13] = "RET";
                dr[14] = "HK";
                dr[15] = "1";
                dr[16] = "14/12/2008";
                dr[17] = "1100";
                dr[18] = "1000";
                dr[19] = "100";
                dr[20] = "C0249480";
                dr[21] = "APPROVED";

                dt.Rows.Add(dr);
            }
            ds.Tables.Add(dt);

            return ds;
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            pnl_Result.Visible = true;
            gv_DC.DataSource = LoadInvoiceDataSet();
            gv_DC.DataBind();
        }

    }
}
