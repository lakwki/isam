using System;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.common;
using com.next.common.datafactory.worker;
using com.next.infra.persistency.transactions;
using com.next.common.domain.types;

namespace com.next.isam.appserver.common
{
    public class CommonManager
    {
        private static CommonManager _instance;
        private CommonWorker commonWorker;
        private GeneralWorker generalWorker;

        public CommonManager()
        {
            commonWorker = CommonWorker.Instance;
            generalWorker = GeneralWorker.Instance;
        }

        public static CommonManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommonManager();
                }
                return _instance;
            }
        }

        public ICollection getOrderTypeList()
        {

            return commonWorker.getOrderTypeList();
        }

        public ICollection getTermOfPurchaseList()
        {
            return commonWorker.getTermOfPurchaseList();
        }

        public ICollection getCustomerList()
        {
            return commonWorker.getCustomerList(-1);
        }

        public ICollection getCustomerList(int payFlag)
        {
            return commonWorker.getCustomerList(payFlag);
        }

        public ICollection getFinalDestinationList()
        {
            return commonWorker.getCustomerDestinationByCustomerId(GeneralCriteria.ALL.GetHashCode()) ;
        }

        public ArrayList getUKDiscountReasonList()
        {
            ArrayList list = (ArrayList)commonWorker.getUKDiscountReasonList();
            return list;
        }

        public UKDiscountReasonRef getUKDiscountReasonByKey(int key)
        {
            return commonWorker.getUKDiscountReasonByKey(key);
        }

        public ICollection getShipmentCountryList()
        {
            return commonWorker.getShipmentCountryList();
        }

        public ICollection getShipmentPortList()
        {
            return commonWorker.getShipmentPortList();
        }

        public ShipmentCountryRef getShipmentCountryByKey(int key)
        {
            return commonWorker.getShipmentCountryByKey(key);
        }

        public ICollection getPackingUnitList()
        {
            return commonWorker.getPackingUnitList();
        }

        public ICollection getPackingMethodList()
        {
            return commonWorker.getPackingMethodList();
        }

        public ICollection getCountryList()
        {
            return generalWorker.getCountryList();
        }

        public ICollection getQuotaCategoryGroupList()
        {
            return commonWorker.getQuotaCategoryGroupList();
        }

        public ICollection getQuotaCategoryList()
        {
            return commonWorker.getQuotaCategoryList();
        }

        public ICollection getOtherCostTypeList()
        {
            return commonWorker.getOtherCostTypeList();
        }

        public ArrayList getCustomerDestinationList()
        {
            return (ArrayList) commonWorker.getCustomerDestinationList();
        }

        public CustomerDestinationDef getCustomerDestinationByKey(int key)
        {
            return commonWorker.getCustomerDestinationByKey(key);
        }

        public TradingAgencyDef getTradingAgencyByKey(int key)
        {
            return commonWorker.getTradingAgencyByKey(key);
        }

        public PackingMethodRef getPackingMethodByKey(int key)
        {
            return commonWorker.getPackingMethodByKey(key);
        }

        public SystemParameterRef getSystemParameterByKey(int key)
        {
            return commonWorker.getSystemParameterByKey(key);
        }

        public SystemParameterRef getSystemParameterByName(string paramName)
        {
            return commonWorker.getSystemParameterByName(paramName);
        }

        public ICollection getTradingAgencyList()
        {
            return commonWorker.getTradingAgencyList();
        }

        public ICollection getShipmentMethodList()
        {
            return commonWorker.getShipmentMethodList();
        }

        public ArrayList getBudgetYearList()
        {
            return commonWorker.getBudgetYearList();
        }

        public ArrayList getPaymentTermList()
        {
            return (ArrayList) commonWorker.getPaymentTermList();
        }

        public ArrayList getCurrencyListForExchangeRate()
        {
            return (ArrayList) commonWorker.getCurrencyListForExchangeRate();
        }

        public ArrayList getNewCurrencyListForExchangeRate()
        {
            return (ArrayList)commonWorker.getNewCurrencyListForExchangeRate();
        }

        public ArrayList getEffectiveCurrencyList()
        {
            return (ArrayList)commonWorker.getEffectiveCurrencyList();
        }

        public ArrayList getStaffListByName(string name)
        {
            return (ArrayList) commonWorker.getStaffListByName(name);
        }

        public OfficeRef getDGHandlingOffice(int officeId)
        {
            return commonWorker.getDGHandlingOffice(officeId);
            /*
            OfficeRef office;
            office = GeneralWorker.Instance.getOfficeRefByKey(officeId);

            if (officeId == OfficeId.DG.Id)
            {
                //office.OfficeCode = "DG";
                //office.Description = "Dong Gung Office";
                office.OfficeCode = "FB";
                office.Description = "Footwear & Bags";
            }
            return office;
            */
        }

        public ArrayList getDGHandlingOfficeList()
        {
            ArrayList list = new ArrayList();
            list.Add(getDGHandlingOffice(OfficeId.DG.Id));
            list.Add(getDGHandlingOffice(OfficeId.HK.Id));
            list.Add(getDGHandlingOffice(OfficeId.SH.Id));
            list.Add(getDGHandlingOffice(OfficeId.VN.Id));
            return list;
        }

        public ArrayList getDGOfficeGroupIdList()
        {
            ArrayList list = new ArrayList();
            list.Add(OfficeId.DG.Id);
            list.Add(101);
            return list;
        }

        public ArrayList getReportOfficeGroupList()
        {
            return (ArrayList)commonWorker.getReportOfficeGroupList();
        }

        public ReportOfficeGroupRef getReportOfficeGroupByKey(int key)
        {
            return commonWorker.getReportOfficeGroupByKey(key);
        }

        public ArrayList getReportOfficeGroupListByAccessibleOfficeIdList(ArrayList officeList)
        {
            return this.getReportOfficeGroupListByAccessibleOfficeIdList(officeList, false);
        }

        public ArrayList getReportOfficeGroupListByAccessibleOfficeIdList(ArrayList officeList, bool hideHandlingOfficeGroup)
        {
            TypeCollector officeIdCollector = TypeCollector.Exclusive;
            foreach (OfficeRef def in officeList)
            {
                officeIdCollector.append(def.OfficeId);
            }
            return commonWorker.getReportOfficeGroupListByAccessibleOfficeIdList(officeIdCollector, hideHandlingOfficeGroup);
        }

        public ArrayList getOfficeListByReportOfficeGroupId(int officeGroupId)
        {
            return commonWorker.getOfficeListByReportOfficeGroupId(officeGroupId);
        }

        public ArrayList getBankList()
        {
            return (ArrayList)commonWorker.getBankList();
        }

        public BankRef getBankByKey(int key)
        {
            return commonWorker.getBankByKey(key);
        }

        public ArrayList getBankBranchList(int bankId)
        {
            return (ArrayList) commonWorker.getBankBranchListByBankId(bankId);
        }

        public ArrayList getBankBranchListByVendorId(int vendorId)
        {
            return commonWorker.getBankBranchListByVendorId(vendorId);
        }

        public void updateBank(BankRef bankRef, int userId)
        {
            commonWorker.updateBank(bankRef, userId);
        }

        public void updateBankBranch(BankBranchRef branchRef, int userId)
        {
            commonWorker.updateBankBranch(branchRef, userId);
        }

        public void updateFileUpload(FileUploadDef fileDef, int userId)
        {
            commonWorker.updateFileUpload(fileDef, userId);
        }

        public void disableSystemParam(int paramId, bool disabled)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                commonWorker.setParameterStatus(paramId, disabled); 
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public void disableMockShopSunAccountUpload(bool disabled)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                commonWorker.setParameterStatus(12, disabled);
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public decimal getSeasonalExchangeRate(int seasonId, int fromCurrencyId, int toCurrencyId)
        {
            return commonWorker.getSeasonalExchangeRate(seasonId, fromCurrencyId, toCurrencyId);
        }
    }
}
