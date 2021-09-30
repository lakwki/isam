using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections;
using com.next.common.domain.types;
using com.next.infra.smartwebcontrol;
using com.next.common.domain.industry.types;
using com.next.common.web.commander;
using com.next.common.domain.industry.vendor;
using System.Web.Script.Services;
using com.next.isam.webapp.commander;
using com.next.isam.domain.nontrade;
using com.next.common.domain.nss;
using com.next.common.domain;
using com.next.infra.util;
using com.next.common.domain.industry.fabric;
using System.Web.UI;

namespace com.next.isam.webapp.webservices
{
    /// <summary>
    /// Summary description for SmartSelectionAJAXWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SmartSelectionAJAXWebService : System.Web.Services.WebService
    {

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string[] getFabricVendorList(string prefix)
        {
            return getVendorList(VendorType.FABRIC.Id, prefix);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string[] getNonClothingVendorList(string prefix)
        {
            return getVendorList(VendorType.NONCLOTHING.Id, prefix);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string[] getGarmentVendorList(string prefix)
        {
            return getVendorList(VendorType.GARMENT.Id, prefix);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string[] getSZVendorList(string prefix)
        {
            return getVendorList(VendorType.GARMENT.Id, prefix);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string[] getVendorListForRecharge(string prefix)
        {
            return getVendorList(-1, prefix);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string[] getNTVendorList(string prefix)
        {
            ArrayList list = new ArrayList();

            string[] prefixs = prefix.Split('|');

            list = WebUtil.getNTVendorByName(prefixs[0], int.Parse(prefixs[2]), int.Parse(prefixs[3]));

            return getPopulateArray(2, list);
        }

        public string[] getVendorList(int vendorTypeId, string prefix)
        {
            ArrayList list = new ArrayList();

            string[] prefixs = prefix.Split('|');

            if (vendorTypeId == -1) 
            {
                vendorTypeId = int.Parse(prefixs[1]);
            }

            if (vendorTypeId == VendorType.GARMENT.Id || vendorTypeId == VendorType.NONCLOTHING.Id)
            {
                TypeCollector types = TypeCollector.Inclusive.append(VendorType.GARMENT.Id).append(VendorType.NONCLOTHING.Id);
                list = (ArrayList)IndustryUtil.getVendorListByTypes(types, prefixs[0]);
            }
            else
            {
                list = (ArrayList)IndustryUtil.getVendorList(vendorTypeId, prefixs[0]);
            }

            return getPopulateArray(1, list);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string[] getContractNoList(string prefix)
        {
            string[] prefixs = prefix.Split('|');

            ArrayList list = (ArrayList)CommonUtil.getTop100ContractBaseDefListBySearching(prefixs[0]);

            return getPopulateArray(3, list);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string[] getProductCodeList(string prefix)
        {
            int userId = ConvertUtility.toInt32(Context.Request.ServerVariables["AUTH_USER"]);

            string[] prefixs = prefix.Split('|');

            string productCodeName = prefixs[0];
            int officeId = int.Parse(prefixs[2]);
            int departmentId = int.Parse(prefixs[4]);

            if (officeId == -1)
                return null;

            ArrayList list = null;
            if (HttpContext.Current.Request.UrlReferrer != null && HttpContext.Current.Request.UrlReferrer.AbsolutePath.ToLower().Contains("nontradeinvoice.aspx"))
                list = (ArrayList)CommonUtil.getProductCodeListWithSeasonOfficeStructureByCriteria(departmentId, GeneralCriteria.ALL, officeId, productCodeName);
            else
                list = (ArrayList)CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(departmentId, GeneralCriteria.ALL, officeId, userId, productCodeName);

            return getPopulateArray(4, list);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string[] getArticleNoList(string prefix)
        {
            string[] prefixs = prefix.Split('|');

            string articleNo = prefixs[0];
            int vendorId = int.Parse(prefixs[5]);
            if (vendorId <= 0) vendorId = GeneralCriteria.ALL;

            ArrayList list = IndustryUtil.getFabricInfoList(vendorId, articleNo);

            return getPopulateArray(5, list);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string[] getUserList(string prefix)
        {
            ArrayList list = new ArrayList();
            //list = CommonUtil.getUserSelectionListByCriteria(userName, TypeCollector.Inclusive);

            string[] prefixs = prefix.Split('|');

            string userName = prefixs[0];
            int officeId = int.Parse(prefixs[2]);

            if (officeId == -1)
                list = CommonUtil.getUserSelectionListByCriteria(-1, OfficeStructureType.DEPARTMENT.Type, TypeCollector.Inclusive, userName, -1, "A", DateTime.Now.AddDays(-365));
            else
            {
                ArrayList arr_Department = CommonUtil.getDepartmentList(officeId);
                TypeCollector departmentIds = TypeCollector.Inclusive;
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

            return getPopulateArray(6, list);
        }
        

        public string[] getPopulateArray(int listType, ArrayList list)
        {
            List<string> items = new List<string>(list.Count);

            if (listType == 1)
            {
                foreach (VendorRef vendor in list)
                {
                    //items.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(vendor.Name, vendor.VendorId.ToString()));
                    items.Add(String.Format("{0}^{1}^{2}", vendor.Name, vendor.VendorId, vendor.OfficeId));
                }
            }
            else if (listType == 2)
            {
                foreach (NTVendorDef vendor in list)
                {
                    items.Add(String.Format("{0}^{1}", vendor.VendorNameWithCompany, vendor.NTVendorId));
                }
            }
            else if (listType == 3)
            {
                foreach (ContractBaseDef csDef in list)
                {
                    items.Add(String.Format("{0}^{1}", csDef.ContractNo, csDef.ContractId));
                }
            }
            else if (listType == 4)
            {
                foreach (OfficeStructureRef pcRef in list)
                {
                    items.Add(String.Format("{0}^{1}", pcRef.Code + " - " + pcRef.Description, pcRef.OfficeStructureId));
                }
            }
            else if (listType == 5)
            {
                foreach (FabricInfoReferenceRef fref in list)
                {
                    string s = fref.ArticleNo;

                    if (fref.Vendor != null) s += "   (" + StringUtility.subString(fref.Vendor.Name, 0, 15) + "...)";

                    items.Add(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", fref.ArticleNo + "   (" + StringUtility.subString(fref.Vendor.Name, 0, 15) + "...)", fref.FabricId.ToString(), fref.ArticleNo,fref.Construction,fref.Composition,fref.Width,fref.Weight,fref.Finish));

                    if (StringUtility.isNotEmptyString(fref.Composition))
                    {
                        string composition = "";

                        if (fref.Composition.Length <= 50)
                            composition = "     - " + fref.Composition;
                        else
                            composition = "     - " + StringUtility.subString(fref.Composition, 0, 50) + "...";

                        items.Add(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", composition, fref.FabricId.ToString(), fref.ArticleNo, fref.Construction, fref.Composition, fref.Width, fref.Weight, fref.Finish));
                    }
                }
            }
            else if (listType == 6)
            {
                foreach (UserSelectionRef selection in list)
                {
                    items.Add(String.Format("{0}^{1}", selection.DisplayName, selection.UserId));
                }
            }

            return items.ToArray();
        }

        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID) return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control controlToReturn = FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null) return controlToReturn;
            }
            return null;
        }


    }
}
