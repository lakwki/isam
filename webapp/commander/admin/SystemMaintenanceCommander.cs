using System;
using System.Web;
using System.Collections;
using com.next.infra.web;
using com.next.infra.util;
using com.next.common.domain.types;
using com.next.isam.appserver.common;
using com.next.isam.domain.common;

namespace com.next.isam.webapp.commander.admin
{
    public class SystemMaintenanceCommander : ICommand
    {
        public SystemMaintenanceCommander()
        {
        }

        public void execute(HttpContext context)
        {
            Action action = (Action) context.Items[WebParamNames.COMMAND_ACTION];
            int userId = WebHelper.getLogonUserId(context);
            CommonManager commonManager = CommonManager.Instance;
            if (action == Action.GetLCBank)
            {
                ArrayList bankList = commonManager.getBankList();
                context.Items.Add(Param.resultList, bankList);
            }
            else if (action == Action.UpdateLCBank)
            {
                BankRef bankRef = (BankRef) context.Items[Param.bankRef];
                commonManager.updateBank(bankRef, userId);

                ArrayList bankList = CommonManager.Instance.getBankList();
                context.Items.Add(Param.resultList, bankList);
            }
            else if (action == Action.GetBankBranchList)
            {
                int bankId = Convert.ToInt32(context.Items[Param.bankId]);
                context.Items.Add(Param.bankRef, commonManager.getBankByKey(bankId));
                context.Items.Add(Param.resultList, commonManager.getBankBranchList(bankId));
            }
            else if (action == Action.UpdateBankBranch)
            {
                BankBranchRef branchRef = (BankBranchRef)context.Items[Param.branchRef];
                commonManager.updateBankBranch(branchRef, userId);

                ArrayList branchList = CommonManager.Instance.getBankBranchList(branchRef.BankId);
                context.Items.Add(Param.resultList, branchList);
            }
            else if (action == Action.GetBankBranchListByVendorId)
            {
                int vendorId = Convert.ToInt32(context.Items[Param.vendorId]);

                ArrayList branchList = CommonManager.Instance.getBankBranchListByVendorId(vendorId);
                context.Items.Add(Param.resultList, branchList);
            }
        }

        public enum Action
        {
            GetLCBank,
            UpdateLCBank,
            UpdateBankBranch,
            GetBankBranchList,
            GetBankBranchListByVendorId
        }

        public enum Param
        {
            bankId,
            vendorId,
            bankRef,
            branchRef,
            resultList
        }
    }
}
