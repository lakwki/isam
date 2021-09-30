using System;
using System.Web;
using System.Collections;
using com.next.infra.web;
using com.next.infra.util;
using com.next.common.appserver;
using com.next.common.domain;
using com.next.isam.appserver.account;

namespace com.next.isam.webapp.commander.admin
{
	public class MonthEndCommander : ICommand
	{
		public MonthEndCommander()
		{
		}
		
		public void execute(HttpContext context)
		{
			GeneralManager generalManager = GeneralManager.Instance;
			Action action = (Action) context.Items[WebParamNames.COMMAND_ACTION];
			int userId = WebHelper.getLogonUserId(context);						
			
			if (action == Action.changeStatus)
			{
				OfficeRef def = (OfficeRef) context.Items["OfficeRef"];
				generalManager.updateOfficeList(ConvertUtility.createArrayList(def), userId);
			}
            else if (action == Action.updateMonthEndStatus)
            {
                OfficeRef office = (OfficeRef)context.Items["OfficeRef"];
                string fiscalPeriod = (string)context.Items["FiscalPeriod"];
                AccountManager.Instance.updateMonthEndStatus(office, fiscalPeriod, userId);
            }
            else if (action == Action.resetMonthEndStatus)
            {
                ArrayList list = (ArrayList)context.Items["OfficeList"];
                string fiscalPeriod = (string)context.Items["FiscalPeriod"];
                AccountManager.Instance.resetMonthEndStatus(list, fiscalPeriod, userId);
            }
            else if (action == Action.salesCutoff)
            {
                OfficeRef office = (OfficeRef)context.Items["OfficeRef"];
                int fiscalYear = (int)context.Items["FiscalYear"];
                int period = (int)context.Items["Period"];
                AccountManager.Instance.salesCutoff(office, fiscalYear, period, userId);
            }
            else if (action == Action.sendSlippageMail)
            {
                OfficeRef office = (OfficeRef)context.Items["OfficeRef"];
                int fiscalYear = (int)context.Items["FiscalYear"];
                int period = (int)context.Items["Period"];
                AccountManager.Instance.SendSlippageMail(office, fiscalYear, period, userId);
            }
            else if (action == Action.submitInterfaceBatch)
            {
                OfficeRef office = (OfficeRef)context.Items["OfficeRef"];
                int fiscalYear = (int)context.Items["FiscalYear"];
                int period = (int)context.Items["Period"];
                AccountManager.Instance.SubmitInterfaceBatch(office, fiscalYear, period, userId);
            }

        }
		
		public enum Action
		{
			changeStatus,
            updateMonthEndStatus,
            resetMonthEndStatus,
            salesCutoff,
            sendSlippageMail,
            submitInterfaceBatch
		}

		public enum Param
		{
			officeList
		}
	}
}
