using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using com.next.isam.domain.types;
using com.next.infra.util;

namespace com.next.isam.webapp.account
{
    public partial class GetRejectReason : System.Web.UI.Page  //com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ArrayList reasonList = RejectPaymentReason.getCollectionValues();
                reasonList.Sort(new ArrayListHelper.Sorter("Name", true));
                ddl_Reason.Items.Add(new ListItem("", "-1"));
                foreach (RejectPaymentReason reason in reasonList)
                    ddl_Reason.Items.Add(new ListItem(reason.Name, reason.Id.ToString()));
            }
        }

        protected void btnComfirm_Click(object sender, EventArgs e)
        {

        }
    }
}
