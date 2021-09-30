using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using com.next.infra.util;
using com.next.infra.smartwebcontrol;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.nss;
using com.next.common.domain.industry.vendor;
using com.next.common.domain.industry.fabric;
using com.next.common.domain.industry.types;
using com.next.isam.domain.types;
using com.next.isam.webapp.commander;

namespace com.next.isam.webapp.webservices
{
	public class SmartSelectionWebServices : System.Web.Services.WebService
	{
		public SmartSelectionWebServices()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code

		//Required by the Web Services Designer
		private IContainer components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		[WebMethod]
		public string getFabricInfoReferenceByKey(int fabricId)
		{
			FabricInfoReferenceRef fref = IndustryUtil.getFabricInfoReferenceByKey(fabricId);
			StringWriter writer = new StringWriter();
			XmlSerializer serializer;

			serializer = 	new XmlSerializer(fref.GetType());
			serializer.Serialize(writer, fref);

			return writer.ToString();
		}

		[WebMethod]
		public string getNonClothingVendorList(string selectionKey, string parentId, string vendorName)
		{
			return getVendorList(selectionKey, parentId, VendorType.NONCLOTHING.Id, vendorName);
		}

		[WebMethod]
		public string getFabricVendorList(string selectionKey, string parentId, string vendorName)
		{
			return getVendorList(selectionKey, parentId, VendorType.FABRIC.Id, vendorName);
		}

		[WebMethod]
		public string getGarmentVendorList(string selectionKey, string parentId, string vendorName)
		{
			return getVendorList(selectionKey, parentId, VendorType.GARMENT.Id, vendorName);
		}

        [WebMethod]
        public string getSZVendorList(string selectionKey, string parentId, string vendorName)
        {
            return getVendorList(selectionKey, parentId, VendorType.GARMENT.Id, vendorName);
        }

        [WebMethod]
        public string getVendorListForRecharge(string selectionKey, string parentId, string vendorName, string vendorTypeId)
        {

            return getVendorList(selectionKey, parentId, int.Parse(vendorTypeId), vendorName);
        }

        [WebMethod]
		public string getContractNoList(string selectionKey, string parentId, string contractNo)
		{
			ArrayList list = (ArrayList) CommonUtil.getTop100ContractBaseDefListBySearching(contractNo);
			string cbxListClientId = getCbxListClientId(parentId);
			SmartListBox cbxList = new SmartListBox();

			cbxList.Attributes.Add("onclick", "selectList('" + selectionKey  + "', '" + parentId + "');");
			cbxList.ID=cbxListClientId;
			foreach (ContractBaseDef csDef in list)
			{
				ListItem item = new ListItem();
				item.Value=csDef.ContractId.ToString();
				item.Text=csDef.ContractNo.ToString();
				cbxList.Items.Add(item);
			}

			return this.getListBoxRenderedString(cbxList);
		}

		[WebMethod]
		public string getProductCodeList(string selectionKey, string parentId, string productCodeName, string officeId, string userId, string departmentId)
		{
            if (officeId == "-1")
                return "";

            ArrayList list = null;
            if (HttpContext.Current.Request.UrlReferrer != null && HttpContext.Current.Request.UrlReferrer.AbsolutePath.ToLower().Contains("nontradeinvoice.aspx"))
                list = (ArrayList)CommonUtil.getProductCodeListWithSeasonOfficeStructureByCriteria(Convert.ToInt32(departmentId), GeneralCriteria.ALL, int.Parse(officeId), productCodeName);
            else
                list = (ArrayList)CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(Convert.ToInt32(departmentId), GeneralCriteria.ALL, int.Parse(officeId), int.Parse(userId), productCodeName);
            /*
            ArrayList list = (ArrayList)CommonUtil.getProductCodeListByCodeAndOfficeId(productCodeName, int.Parse(officeId));
			ArrayList list = (ArrayList) CommonUtil.getProductCodeListByCode(productCodeName);
            */

			string cbxListClientId = getCbxListClientId(parentId);
			SmartListBox cbxList = new SmartListBox();

			cbxList.Attributes.Add("onclick", "selectList('" + selectionKey  + "', '" + parentId + "');");
			cbxList.ID=cbxListClientId;
			foreach (OfficeStructureRef pcRef in list)
			{
				ListItem item = new ListItem();
				item.Value=pcRef.OfficeStructureId.ToString();
				item.Text=pcRef.Code+ " - " + pcRef.Description;
				cbxList.Items.Add(item);
			}

			return this.getListBoxRenderedString(cbxList);
		}

		[WebMethod]
		public string getArticleNoList(string selectionKey, string parentId, string articleNo, string vendorIdStr)
		{
			int vendorId=ConvertUtility.toInt32(vendorIdStr);
			if (vendorId<=0) vendorId = GeneralCriteria.ALL;

			ArrayList list = IndustryUtil.getFabricInfoList(vendorId, articleNo);
			string cbxListClientId = getCbxListClientId(parentId);

			SmartListBox cbxList = new SmartListBox();
			cbxList.Attributes.Add("onclick", "selectList('" + selectionKey  + "', '" + parentId + "');");
			cbxList.ID=cbxListClientId;

			foreach(FabricInfoReferenceRef fref in list)
			{
				string s= fref.ArticleNo;

				if (fref.Vendor!=null)	s+="   (" + StringUtility.subString(fref.Vendor.Name, 0, 15) + "...)";

				cbxList.Items.Add(new ListItem(fref.ArticleNo+"   (" + StringUtility.subString(fref.Vendor.Name, 0, 15) + "...)", fref.FabricId.ToString()));

				if (StringUtility.isNotEmptyString(fref.Composition))
				{
					ListItem item = new ListItem();

					if (fref.Composition.Length<=50)
						item.Text="     - " + fref.Composition;
					else
						item.Text="     - " + StringUtility.subString(fref.Composition, 0, 50) + "...";

					item.Value=fref.FabricId.ToString();
					item.Attributes.Add("DisplayText", fref.ArticleNo);
					cbxList.Items.Add(item);
				}
			}

			return this.getListBoxRenderedString(cbxList);
		}

        [WebMethod]
        public string getVendorByKey(int vendorId)
        {
            VendorRef def = IndustryUtil.getVendorByKey(vendorId);
            StringWriter writer = new StringWriter();
            XmlSerializer serializer;

            serializer = new XmlSerializer(def.GetType());
            serializer.Serialize(writer, def);

            return writer.ToString();
        }


		public string getVendorList(string selectionKey, string parentId, int vendorTypeId, string vendorName)
		{
			ArrayList list = new ArrayList();

			if (vendorTypeId == VendorType.GARMENT.Id || vendorTypeId == VendorType.NONCLOTHING.Id)
			{
				TypeCollector types = TypeCollector.Inclusive.append(VendorType.GARMENT.Id).append(VendorType.NONCLOTHING.Id);
				list = (ArrayList) IndustryUtil.getVendorListByTypes(types, vendorName);
			}
			else
			{
				list = (ArrayList) IndustryUtil.getVendorList(vendorTypeId, vendorName);
			}
			string cbxListClientId = getCbxListClientId(parentId);

			SmartListBox cbxList = new SmartListBox();
			cbxList.Attributes.Add("onclick", "selectList('" + selectionKey  + "', '" + parentId + "');");
			cbxList.ID=cbxListClientId;
			cbxList.bindList(list, "Name", "VendorId");

			return this.getListBoxRenderedString(cbxList);
		}

        [WebMethod]
        public string getNTVendorList(string selectionKey, string parentId, string vendorName, string officeId, string workflowStatusId)
        {
            ArrayList list = new ArrayList();
            list = WebUtil.getNTVendorByName(vendorName, int.Parse(officeId), int.Parse(workflowStatusId));

            string cbxListClientId = getCbxListClientId(parentId);

            SmartListBox cbxList = new SmartListBox();
            cbxList.Attributes.Add("onclick", "selectList('" + selectionKey + "', '" + parentId + "');");
            cbxList.ID = cbxListClientId;
            cbxList.bindList(list, "VendorNameWithCompany", "NTVendorId");

            return this.getListBoxRenderedString(cbxList);
        }

        [WebMethod]
        public string getUserList(string selectionKey, string parentId, string userName, string officeId)
        {
            ArrayList list = new ArrayList();
            //list = CommonUtil.getUserSelectionListByCriteria(userName, TypeCollector.Inclusive);

            if (officeId == "-1")
                list = CommonUtil.getUserSelectionListByCriteria(-1, OfficeStructureType.DEPARTMENT.Type, TypeCollector.Inclusive, userName, -1, "A", DateTime.Now.AddDays(-365));
            else
            {
                ArrayList arr_Department = CommonUtil.getDepartmentList(int.Parse(officeId));
                TypeCollector departmentIds = TypeCollector.Inclusive ;
                foreach (DepartmentRef dept in arr_Department)
                {
                    departmentIds.append(dept.DepartmentId);
                }

                list = CommonUtil.getUserSelectionListByCriteria(-1, OfficeStructureType.DEPARTMENT.Type, departmentIds, userName, -1, "A", DateTime.Now.AddDays(-365));
            }

            if ("unclassified".Contains(userName.ToLower()))
            {
                UserSelectionRef selection = new UserSelectionRef();
                selection.DisplayName = "UNCLASSIFIED";
                selection.UserId = -1;
                list.Add(selection);
            }

            string cbxListClientId = getCbxListClientId(parentId);

            SmartListBox cbxList = new SmartListBox();
            cbxList.Attributes.Add("onclick", "selectList('" + selectionKey + "', '" + parentId + "');");
            cbxList.ID = cbxListClientId;
            cbxList.bindList(list, "DisplayName", "UserId");

            return this.getListBoxRenderedString(cbxList);
        }

		private string getListBoxRenderedString(SmartListBox cbxList)
		{
			string result="";

			if (cbxList.Items.Count>0)
			{
				cbxList.CssClass="formText";
				cbxList.SelectedIndex=-1;
				cbxList.Height=Unit.Pixel(100);

                cbxList.Attributes.Add("onmouseover", "mouseOverCbxList=true;");
                cbxList.Attributes.Add("onmouseout", "mouseOverCbxList=false");
                cbxList.Attributes.Add("onblur", "txtNameOnBlur('', '" + cbxList.ClientID.Replace("_cbxList", "") + "');");

				StringWriter sw = new StringWriter();
				HtmlTextWriter hw = new HtmlTextWriter(sw);
				cbxList.RenderControl(hw);

				result = sw.ToString();

				foreach(ListItem item in cbxList.Items)
				{
					if (item.Attributes["DisplayText"]!=null)
					{
						string str = "<option value=\""+ item.Value+ "\">" + item.Text +"</option>";
						string replaceStr = "<OPTION value=\""+ item.Value+ "\" DisplayText=\"" + item.Attributes["DisplayText"].ToString() + "\">" + item.Text +"</OPTION>";
						result=result.Replace(str, replaceStr);
					}
				}
			}
			else
			{
				result="No record found.";
			}

			return result;
		}

		private string getCbxListClientId(string parentId)
		{
            return parentId + "_cbxList";
		}
	}
}
