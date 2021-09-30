using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using com.next.infra.web;
using com.next.infra.util;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.module;
using com.next.common.security;
using com.next.common.web.commander;

namespace ISAM.MasterPage
{
    public partial class MainMaster : System.Web.UI.MasterPage
    {
        protected readonly int APPID = AccessMapper.ISAM.Id;

        protected int LogonUserId
        {
            get { return ConvertUtility.toInt32(Context.Request.ServerVariables["AUTH_USER"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int logonUserId = LogonUserId;

                if (repMenu != null)
                {
                    ArrayList menus = CommonUtil.getAccessibleMenu(APPID, logonUserId);
                    this.repMenu.DataSource = menus;
                    this.repMenu.DataBind();

                    UserRef userRef = CommonUtil.getUserByKey(logonUserId);

                    if (userRef != null)
                        this.lblDisplayName.Text = userRef.DisplayName;
                }
            }
        }

        protected void repMenu_ItemDataBound(object source, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (WebHelper.isRepeaterNormalItemType(e))
            {
                MenuStruct ms = (MenuStruct)e.Item.DataItem;
                Repeater repSubMenu = (Repeater)e.Item.FindControl("repSubMenu");

                if (ms.MenuItem != null && ms.MenuItem.Count > 0)
                {
                    //((ArrayList)ms.MenuItem).Sort(new ArrayListHelper.Sorter("SeqNo", true));
                    //repSubMenu.DataSource = ms.MenuItem;
                    ArrayList nonEmptyModule = new ArrayList();
                    foreach (ModuleProperty module in ms.MenuItem)
                        if (!string.IsNullOrEmpty(module.Path))
                            nonEmptyModule.Add(module);
                    (nonEmptyModule).Sort(new ArrayListHelper.Sorter("SeqNo", true));
                    repSubMenu.DataSource = nonEmptyModule;
                    repSubMenu.DataBind();
                }
                else
                    repSubMenu.Visible = false;
            }
        }

        protected void ApplicationLinkbutton_Click(object sender, System.EventArgs e)
        {
            string para = SecurityManager.Instance.getAccessibleToken(this.LogonUserId);
            string strLink = ((LinkButton)sender).CommandArgument + para;

            com.next.infra.util.ClientScript.windowOpen(strLink, this.Page);
        }

        protected void btnIntranet_Click(object sender, ImageClickEventArgs e)
        {
            string para = SecurityManager.Instance.getAccessibleToken(this.LogonUserId);
            string strLink = ((ImageButton)sender).CommandArgument + para;

            com.next.infra.util.ClientScript.windowOpen(strLink, this.Page);
        }
    }
}
