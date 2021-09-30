using System;
using System.Web;

using com.next.infra.util;
using com.next.infra.web;
using com.next.common.domain;
using com.next.common.appserver;

namespace com.next.isam.webapp.commander.settings
{
    /// <summary>
    /// Summary description for ChangePasswordCommander.
    /// </summary>
    public class ChangePasswordCommander : ICommand
    {
        public ChangePasswordCommander()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public void execute(HttpContext context)
        {
            int userid = WebHelper.getLogonUserId(context);

            string newpassword = (string)context.Items["NewPwd"];
            string oldpassword = (string)context.Items["OldPwd"];

            int result = AdminManager.Instance.ChangePassword(userid, oldpassword, newpassword);
            context.Items.Add("result", result);
        }
    }
}
