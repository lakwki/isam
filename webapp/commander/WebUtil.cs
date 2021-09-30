using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web;
using com.next.infra.util;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.module;
using com.next.common.web.commander;
using com.next.isam.appserver.user;
using com.next.isam.appserver.common;
using com.next.isam.appserver.shipping;
using com.next.isam.appserver.account;
using com.next.isam.appserver.order;
using com.next.isam.domain.common;
using com.next.isam.domain.nontrade ;
using com.next.isam.dataserver.worker;
using com.next.common.appserver;

namespace com.next.isam.webapp.commander
{
    public class WebUtil
    {
        private WebUtil()
        {
        }

        public static ArrayList getProductCodeList()
        {
            return WebUtil.getProductCodeList(false);
        }

        public static ArrayList getProductCodeList(bool showNewCodeOnly)
        {
            return CommonUtil.getProductCodeListByProductDepartmentId(-1, showNewCodeOnly);
        }

        public static ArrayList getAvailProductDeptIds(com.next.infra.smartwebcontrol.SmartDropDownList cbx)
        {
            ArrayList productDepts = new ArrayList();

            if (cbx.SelectedIndex >= 0)
            {
                if (cbx.SelectedIndex == 0)
                {
                    if (cbx.Items.Count > 1)
                    {
                        for (int i = 1; i < cbx.Items.Count; i++)
                        {
                            productDepts.Add(Convert.ToInt32(cbx.Items[i].Value));
                        }
                    }
                }
                else
                    productDepts.Add(Convert.ToInt32(cbx.SelectedValue));
            }

            return productDepts;
        }


        public static void setAccessOfficeIds(TypeCollector officeIds, ArrayList accessOffices, com.next.infra.smartwebcontrol.SmartDropDownList cbx)
        {
            int selectedOfficeId;

            try
            {
                selectedOfficeId = int.Parse(cbx.SelectedValue);
            }
            catch
            {
                selectedOfficeId = -1;
            }

            if (selectedOfficeId > 0)
            {
                officeIds.append(selectedOfficeId);
            }
            else
            {
                WebUtil.setAccessOfficeIds(officeIds, accessOffices);
            }
        }

        public static void setAccessOfficeIds(TypeCollector officeIds, ArrayList accessOffices)
        {
            foreach (OfficeRef def in accessOffices)
            {
                officeIds.append(def.OfficeId);
            }
        }

        public static ArrayList getAvailProductCodes(ArrayList productDepartmentIds, bool showNewCodeOnly)
        {
            if (productDepartmentIds != null && productDepartmentIds.Count > 0)
            {
                TypeCollector productDepts = TypeCollector.Inclusive;

                foreach (int pDeptId in productDepartmentIds)
                {
                    productDepts.append(pDeptId);
                }

                ArrayList pcList = CommonUtil.getProductCodeListByProductDepartmentId(productDepts, showNewCodeOnly);
                ArrayList resultList = new ArrayList();

                if (pcList != null && pcList.Count > 0)
                {
                    foreach (ProductCodeRef pcDef in pcList)
                    {
                        resultList.Add(CommonUtil.getProductCodeDefByKey(pcDef.ProductCodeId));
                    }
                    resultList.Sort(new ArrayListHelper.Sorter("CodeDescriptionOffice"));
                }

                return resultList;
            }
            else
                return new ArrayList();
        }

        public static ArrayList getAccessOfficeByUserId(int userId)
        {
            return UserAccessManager.Instance.getAccessOfficeByUserId(userId);
        }

        public static UserLocation getUserLocationByUserId(int userId)
        {
            return UserAccessManager.Instance.getUserLocationByUserId(userId);
        }

        public static ArrayList getAccessProductDeptByUserId(int userId, int officeId)
        {
            bool isAdvanceUser = CommonUtil.isAuthenticated(userId, AccessMapper.ELAB.Id, ElabModule.testReportEnquiry.Id);

            return UserAccessManager.Instance.getAccessProductDeptByUserId(userId, officeId, isAdvanceUser);
        }

        public static ICollection getCustomerList()
        {
            return CommonManager.Instance.getCustomerList();
        }

        public static ICollection getCustomerList(int payFlag)
        {
            return CommonManager.Instance.getCustomerList(payFlag);
        }

        public static CustomerDef getCustomerByKey(int key)
        {
            return CommonWorker.Instance.getCustomerByKey(key);
        }

        public static ICollection getOrderTypeList()
        {
            return CommonManager.Instance.getOrderTypeList();
        }

        public static ICollection getTermOfPurchaseList()
        {
            return CommonManager.Instance.getTermOfPurchaseList();
        }

        public static ICollection getFinalDestinationList()
        {
            return CommonManager.Instance.getFinalDestinationList();
        }

        public static string getInvoicePrefix(string invoiceNo)
        {
            return ShipmentManager.getInvoicePrefix(invoiceNo);
        }

        public static int getInvoiceSeq(string invoiceNo)
        {
            return ShipmentManager.getInvoiceSeq(invoiceNo);
        }

        public static int getInvoiceYear(string invoiceNo)
        {
            return ShipmentManager.getInvoiceYear(invoiceNo);
        }

        public static void sendErrorAlert(Exception exp, UserRef userRef)
        {
            string username = "";
            if (userRef != null)
                username = userRef.DisplayName;
            NoticeHelper.sendErrorAlert(exp, username);
        }

        public static ArrayList getUKDiscountReasonList()
        {
            return CommonManager.Instance.getUKDiscountReasonList();
        }

        public static ArrayList getSunDbList()
        {
            return SunManager.Instance.getSunDBList();
        }

        public static ICollection getShipmentCountryList()
        {
            return CommonManager.Instance.getShipmentCountryList();
        }

        public static ICollection getShipmentPortList()
        {
            return CommonManager.Instance.getShipmentPortList();
        }

        public static ICollection getPackingUnitList()
        {
            return CommonManager.Instance.getPackingUnitList();
        }

        public static ICollection getPackingMethodList()
        {
            return CommonManager.Instance.getPackingMethodList();
        }

        #region ReportOfficeGroup

        public static ArrayList getReportOfficeGroupList()
        {
            return CommonManager.Instance.getReportOfficeGroupList();
        }

        public static ReportOfficeGroupRef getReportOfficeGroupByKey(int key)
        {
            return CommonManager.Instance.getReportOfficeGroupByKey(key);
        }

        public static void convertToDepartmentGroupList(ListItemCollection officeDepartmentList)
        { // Multiple Office
            string deptName;
            string deptId;

            ArrayList newDeptGroup = new ArrayList();
            foreach (ListItem deptItem in officeDepartmentList)
                if (deptItem.Text.IndexOf("All") >= 0)
                {
                    ListItem newDept = new ListItem();
                    newDept.Value = deptItem.Value;
                    newDept.Text = deptItem.Text;
                    newDeptGroup.Add(newDept);
                    break;
                }
            foreach (ListItem deptItem in officeDepartmentList)
            {
                bool found;
                if (deptItem.Text.IndexOf("(") > 0)
                {
                    deptName = deptItem.Text.Substring(0, deptItem.Text.IndexOf("(") - 1);
                    deptId = deptItem.Value;
                    found = false;
                    for (int i = 0; i < newDeptGroup.Count; i++)
                    {
                        ListItem itm = ((ListItem)newDeptGroup[i]);
                        if (itm.Text.IndexOf("(") >= 0)
                            if (itm.Text.Substring(0, itm.Text.IndexOf("(") - 1) == deptName)
                            {
                                itm.Value += "|" + deptId;
                                found = true;
                                break;
                            }
                    }
                    if (!found)
                    {
                        ListItem newDept = new ListItem();
                        newDept.Value = deptItem.Value;
                        newDept.Text = deptItem.Text;
                        //deptItem.Text = deptName;
                        newDeptGroup.Add(newDept);
                    }
                }
            }
            officeDepartmentList.Clear();
            for (int i = 0; i < newDeptGroup.Count; i++)
            {
                ListItem itm = (ListItem)newDeptGroup[i];
                if (itm.Value.IndexOf("|") > 0)
                {
                    itm.Text = itm.Text.Substring(0, itm.Text.IndexOf("(") - 1) + " (All)";
                }
                officeDepartmentList.Add(itm);
            }
        }

        public static ArrayList getProductDepartmentListByOfficeGroupId(int officeGroupId, int userId)
        {
            ArrayList arr_department = null;
            ArrayList officeList = CommonWorker.Instance.getOfficeListByReportOfficeGroupId(officeGroupId);
            foreach (OfficeRef office in officeList)
            {
                if (arr_department == null)
                    arr_department = CommonUtil.getProductDepartmentListByCriteria(userId, Convert.ToInt32(office.OfficeId), GeneralCriteria.ALL);
                else
                {
                    ArrayList dept = CommonUtil.getProductDepartmentListByCriteria(userId, Convert.ToInt32(office.OfficeId), GeneralCriteria.ALL);
                    foreach (ProductDepartmentRef rf in dept)
                        arr_department.Add(rf);
                }
            }
            return arr_department;
        }

        #endregion

        public static ICollection getCountryList()
        {
            return CommonManager.Instance.getCountryList();
        }

        public static ICollection getQuotaCategoryGroupList()
        {
            return CommonManager.Instance.getQuotaCategoryGroupList();
        }

        public static ICollection getQuotaCategoryList()
        {
            return CommonManager.Instance.getQuotaCategoryList();
        }

        public static ArrayList getShipmentListByContractNo(string contractNo)
        {
            return OrderManager.Instance.getShipmentListByContractNo(contractNo);
        }

        public static ArrayList getDocumentListByShipmentId(int shipmentId)
        {
            return ShipmentManager.Instance.getDocumentListByShipmentId(shipmentId);
        }

        public static ICollection getOtherCostTypeList()
        {
            return CommonManager.Instance.getOtherCostTypeList();
        }

        public static ArrayList getCustomerDestinationList()
        {
            return CommonManager.Instance.getCustomerDestinationList();
        }

        public static CustomerDestinationDef getCustomerDestinationByKey(int key)
        {
            return CommonManager.Instance.getCustomerDestinationByKey(key);
        }

        public static ICollection getTradingAgencyList()
        {
            return CommonManager.Instance.getTradingAgencyList();
        }

        public static PackingMethodRef getPackingMethodByKey(int key)
        {
            return CommonManager.Instance.getPackingMethodByKey(key);
        }

        public static ICollection getShipmentMethodList()
        {
            return CommonManager.Instance.getShipmentMethodList();
        }

        public static ArrayList getBudgetYearList()
        {
            return CommonManager.Instance.getBudgetYearList();
        }

        public static ArrayList getPaymentReferenceCodeList()
        {
            return AccountManager.Instance.getPaymentReferenceCodeList();
        }

        public static ArrayList getPaymentTermList()
        {
            return CommonManager.Instance.getPaymentTermList();
        }

        public static ArrayList getCurrencyListForExchangeRate()
        {
            return CommonManager.Instance.getCurrencyListForExchangeRate();
        }

        public static ArrayList getNewCurrencyListForExchangeRate()
        {
            return CommonManager.Instance.getNewCurrencyListForExchangeRate();
        }

        public static ArrayList getEffectiveCurrencyList()
        {
            return CommonManager.Instance.getEffectiveCurrencyList();
        }

        public static ArrayList getSplitShipmentByShipmentId(int ShipmentId)
        {
            return (ArrayList)OrderSelectWorker.Instance.getSplitShipmentByShipmentId(ShipmentId);
        }

        public static BankRef getBankByKey(int bankId)
        {
            return CommonManager.Instance.getBankByKey(bankId);
        }

        public static CartonTypeRef getCartonTypeByKey(int cartonTypeId)
        {
            return CommonWorker.Instance.getCartonTypeByKey(cartonTypeId);
        }

        public static ArrayList getNTExpenseTypeList()
        {
            return NonTradeManager.Instance.getNTExpenseTypeList();
        }

        public static NTExpenseTypeRef getNTExpenseTypeByKey(int expenseTypeId)
        {
            return NonTradeManager.Instance.getNTExpenseTypeByKey(expenseTypeId);
        }

        public static ArrayList getNTExpenseTypeByNTVendorId(int NTVendorId)
        {
            return NonTradeManager.Instance.getNTExpenseTypeByNTVendorId(NTVendorId);
        }

        public static NTVendorDef getNTVendorByKey(int vendorId)
        {
            return NonTradeManager.Instance.getNTVendorByKey(vendorId);
        }

        public static ArrayList getNTVendorByName(string name, int officeId, int workflowStatusId)
        {
            return NonTradeManager.Instance.getNTVendorByName(name, officeId, workflowStatusId);
        }

        public static ArrayList getCurrentNTMonthEndStatus(ArrayList officeList)
        {
            return NonTradeManager.Instance.getCurrentNTMonthEndStatus(officeList);
        }

        public static ArrayList getNTExpenseTypeListByOfficeId(int officeId)
        {
            return NonTradeManager.Instance.getNTExpenseTypeListByOfficeId(officeId);
        }

        public static ArrayList getNTOfficeList(int userId)
        {
            return NonTradeManager.Instance.getNTOfficeList(userId);
        }

        public static ArrayList getNTRechargeableOfficeList(int userId)
        {
            return NonTradeManager.Instance.getNTRechargeableOfficeList(userId);
        }

        public static CompanyRef getCompanyByKey(int companyId)
        {
            return GeneralManager.Instance.getCompanyByKey(companyId);
        }

        public static string filterSpecialCharacter(string str)
        {
            return FileUploadManager.Instance.filterSpecialCharacter(str);
        }

        #region Parameter Encryption/Decryption function
        private static string DecryptParam(string parameter)
        {
            try { return (parameter != null ? EncryptionUtility.DecryptParam(parameter) : string.Empty); }
            catch { return null; }
        }
        public static string DecryptParameter(string parameter)
        {
            string param = null;
            if ((param = DecryptParam(parameter)) == null)
                param = DecryptParam(HttpUtility.UrlDecode(parameter));
            return param;
        }
        public static string EncryptParameter(string parameter)
        {
            string param = EncryptionUtility.EncryptParam(parameter);
            return HttpUtility.UrlEncode(param);
        }
        #endregion
    }
}