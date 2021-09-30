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
using System.Xml.Xsl;
using System.Xml;
using com.next.common.web.commander;

namespace com.next.ecs.webapp.main
{
	/// <summary>
	/// Summary description for _Default.
	/// </summary>
	public partial class Default : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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

		}
		#endregion

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			Response.ContentType = "application/vnd.ms-excel";
			Response.Charset = "";

			/*
			DataSet ds = new DataSet();
			ds.ReadXml(Server.MapPath(this.txtXMLFile.Text+".xml"));
			XmlDataDocument xdd = new XmlDataDocument(ds);
			*/

			System.IO.StringReader sreader = new System.IO.StringReader(this.txtXMLFile.Text);
			System.Xml.XPath.XPathDocument xd = new System.Xml.XPath.XPathDocument(sreader);

			XslTransform  xt = new XslTransform();
			xt.Load(Server.MapPath(this.txtXSLFile.Text+".xsl"));
//			xt.Transform(xdd, null, Response.OutputStream);
//			xt.Transform(xd, null, Response.OutputStream);
			Response.End();
		}
	}
}
