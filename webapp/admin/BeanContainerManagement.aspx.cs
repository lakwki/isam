using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using com.next.infra.util;
using com.next.infra.web;
using com.next.infra.configuration;
using com.next.infra.configuration.appserver;
using com.next.isam.webapp.usercontrol;
using com.next.common.domain.module;
using com.next.isam.appserver.common;

namespace com.next.isam.webapp.admin
{
	public partial class BeanContainerManagement : PageTemplate
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
            PageModuleId = AccessMapper.ISAM.SystemAdmin.Id;

            StartupLoaderFactory.Instance.Initialize();

			if (!Page.IsPostBack)
			{
				object[] aHandlers = ((ArrayList) StartupLoaderFactory.Instance.getHandlerNames()).ToArray();
				Array.Sort(aHandlers);
				ArrayList handlers = new ArrayList(aHandlers);
				this.grdBeanContainer.DataSource=handlers;
				this.grdBeanContainer.DataBind();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grdBeanContainer.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdBeanContainer_ItemCommand);
			this.grdBeanContainer.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.grdBeanContainer_ItemDataBound);

		}
		#endregion

		private void grdBeanContainer_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (WebHelper.isGridNormalItemType(e))
			{
				string handlerName = (string)e.Item.DataItem;
                if (handlerName == "SunInterfaceSubmissionHandler")
                {
                    if (CommonManager.Instance.getSystemParameterByKey(11).ParameterValue == "Y" && StartupLoaderFactory.Instance.isHandlerRunning(handlerName))
                        StartupLoaderFactory.Instance.stopHandler(handlerName);
                    if (CommonManager.Instance.getSystemParameterByKey(11).ParameterValue == "N" && !StartupLoaderFactory.Instance.isHandlerRunning(handlerName))
                        StartupLoaderFactory.Instance.startHandler(handlerName);
                }
                if (handlerName == "MockShopSunAccountUploadHandler")
                {
                    if (CommonManager.Instance.getSystemParameterByKey(12).ParameterValue == "Y" && StartupLoaderFactory.Instance.isHandlerRunning(handlerName))
                        StartupLoaderFactory.Instance.stopHandler(handlerName);
                    if (CommonManager.Instance.getSystemParameterByKey(12).ParameterValue == "N" && !StartupLoaderFactory.Instance.isHandlerRunning(handlerName))
                        StartupLoaderFactory.Instance.startHandler(handlerName);
                }
                if (handlerName == "SunInterfaceProcessHandler" || handlerName == "NTSunInterfaceProcessHandler")
                {
                    if (CommonManager.Instance.getSystemParameterByKey(220).ParameterValue == "Y" && StartupLoaderFactory.Instance.isHandlerRunning(handlerName))
                        StartupLoaderFactory.Instance.stopHandler(handlerName);
                    if (CommonManager.Instance.getSystemParameterByKey(220).ParameterValue == "N" && !StartupLoaderFactory.Instance.isHandlerRunning(handlerName))
                        StartupLoaderFactory.Instance.startHandler(handlerName);
                }

                string statusText = (StartupLoaderFactory.Instance.isHandlerRunning(handlerName)) ? "Started" : "Stopped";
				e.Item.Cells[0].Text="Handler";
				e.Item.Cells[1].Text=handlerName;
				e.Item.Cells[2].Text= statusText;
                e.Item.Cells[2].Font.Bold = true;
                e.Item.Cells[2].HorizontalAlign = HorizontalAlign.Center;
                if (statusText == "Started")
                {
                    e.Item.Cells[2].BackColor = Color.LightGreen;
                    e.Item.Cells[2].ForeColor = Color.Black;
                }
                else
                {
                    e.Item.Cells[2].BackColor = Color.Red;
                    e.Item.Cells[2].ForeColor = Color.Black;
                }
				((Button)e.Item.Cells[3].FindControl("btnAction")).Text=(StartupLoaderFactory.Instance.isHandlerRunning(handlerName))?"Stop":"Start";
				this.vwHandlers.Add(handlerName);
			}
		}

		private void grdBeanContainer_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName=="doAction")
			{
				string handlername = (string)this.vwHandlers[e.Item.ItemIndex];

				if (StartupLoaderFactory.Instance.isHandlerRunning(handlername))
				{
					StartupLoaderFactory.Instance.stopHandler(handlername);
                    if (handlername == "SunInterfaceSubmissionHandler")
                        CommonManager.Instance.disableSystemParam(11, true);
                    if (handlername == "MockShopSunAccountUploadHandler")
                        CommonManager.Instance.disableSystemParam(12, true);
				}
				else
				{
					StartupLoaderFactory.Instance.startHandler(handlername);
                    if (handlername == "SunInterfaceSubmissionHandler")
                        CommonManager.Instance.disableSystemParam(11, false);
                    if (handlername == "MockShopSunAccountUploadHandler")
                        CommonManager.Instance.disableSystemParam(12, false);
				}
			}
			else if (e.CommandName=="runNow")
			{
				string handlername = (string)this.vwHandlers[e.Item.ItemIndex];
				StartupLoaderFactory.Instance.GetHandler(handlername).runNow();
			}
			Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, "admin.BeanContainerManagement");
			HttpHandler handler = new HttpHandler();
			handler.ProcessRequest(Context);
		}

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			StartupLoaderFactory.Instance.ReStart();
			Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, "admin.BeanContainerManagement");
			HttpHandler handler = new HttpHandler();
			handler.ProcessRequest(Context);
		}

		private ArrayList vwHandlers
		{
			get
			{
				if (ViewState["vwHandlers"]==null)
				{
					ViewState["vwHandlers"] = new ArrayList();
				}
				return (ArrayList) ViewState["vwHandlers"];
			}
			set{ ViewState["vwHandlers"]=value;}
		}
	}
}
