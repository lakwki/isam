using System;
using com.next.isam.appserver.shipping;

namespace com.next.isam.webapp.account
{
    public partial class ReplacementInvoiceEnquiry : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            //lblReplacementInvoiceNo.Text = ShipmentManager.Instance.getReplacementInvoiceNo(txtILSInvoiceNo.Text);
            string[] msg = ShipmentManager.Instance.getReplacementInvoiceNo(txtILSInvoiceNo.Text);
            lblReplacementInvoiceNo.Text = msg[0];
            lblShipment.Text = msg[1];
        }

    }
}
