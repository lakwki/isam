using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using com.next.infra.util;
using com.next.infra.configuration.appserver;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.webapp.commander;
using System.Diagnostics;
using System.Reflection;

namespace com.next.isam
{
	public class Global : System.Web.HttpApplication
	{
		private System.ComponentModel.IContainer components = null;
		private StartupLoaderFactory starter = StartupLoaderFactory.Instance;

		public Global()
		{
			InitializeComponent();
		}

		protected void Application_Start(Object sender, EventArgs e)
		{
			starter.Initialize();
		}

		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
            /*
            // Fires at the beginning of each request
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Methods", "GET,PUT,POST,DELETE,OPTIONS");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Headers", "Content-Type,Authorization");
            */
		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{
            if (Request.Path.Contains("reporter"))
            {
                HttpContext.Current.SkipAuthorization = true;
            }
            if (Request.Path.Contains("webservices"))
            {
                HttpContext.Current.SkipAuthorization = true;
            }   
		}

		protected void Application_Error(Object sender, EventArgs e)
		{
			/***
			 * send msg for error alert
			 ***/
			int userId=0;

			try
			{
				userId=Convert.ToInt16(Context.Request.ServerVariables["AUTH_USER"]);
			}
			finally
			{
			}

			UserRef userref=CommonUtil.getUserByKey(userId);
			Exception exp = Context.Error;// Server.GetLastError();

			WebUtil.sendErrorAlert(exp, userref);

			string message="";
			string err="";

			err=exp.Message;

			if (exp.InnerException!=null) err+="<div>"+exp.InnerException.Message+"</div>";

			message="Sorry, we cannot process your request at this moment." + "<div style='font-size: 10px;color: #c6c6c6;'>" + err + "</div>";

			Context.Items.Add("result", message);

			if (!Convert.ToBoolean(WebConfig.getValue("appSettings", "debug")))
				Server.Transfer("../main/ErrorPage.aspx");

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{
            HttpRuntime runtime = (HttpRuntime)typeof(System.Web.HttpRuntime).InvokeMember("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);

            if (runtime == null)
                return;

            string shutDownMessage = (string)runtime.GetType().InvokeMember("_shutDownMessage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);

            string shutDownStack = (string)runtime.GetType().InvokeMember("_shutDownStack", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);

            if (!EventLog.SourceExists(".NET Runtime"))
            {
                EventLog.CreateEventSource(".NET Runtime", "Application");
            }

            EventLog log = new EventLog();

            log.Source = ".NET Runtime";

            log.WriteEntry(String.Format("\r\n\r\n_shutDownMessage={0}\r\n\r\n_shutDownStack={1}", shutDownMessage, shutDownStack), EventLogEntryType.Error);
		}

		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}

