using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.account;
using com.next.isam.domain.common;
using com.next.infra.util;
using System.Collections;
using com.next.common.domain;
using com.next.isam.domain.order;

namespace com.next.isam.reporter.invoice.BeneficiaryCert
{
    public class BeneficiaryCertManager
    {
        private static BeneficiaryCertManager _instance;

        public BeneficiaryCertManager()
		{

		}

        public static BeneficiaryCertManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BeneficiaryCertManager();
                }
                return _instance;
            }
        }

        public BeneficiaryCertRpt GetBeneficiaryCertReport(InvoiceDef invoiceDef, ShipmentDef shipmentDef, ContractDef contractDef)
        {
            BeneficiaryCertRpt rpt = new BeneficiaryCertRpt();
            BeneficiaryCertDs ds = new BeneficiaryCertDs();
            try
            {
                BeneficiaryCertDs.BeneficiaryCertRow dr = ds.BeneficiaryCert.NewBeneficiaryCertRow();
                string contractNo = contractDef.ContractNo + "-" + shipmentDef.DeliveryNo;
                string poNo = "";
                switch (contractNo)
                {
                    case "MI8484055-1":
                        poNo = "WHPO55624";
                        break;
                    case "MI8484055-2":
                        poNo = "WHPO55625";
                        break;
                    case "MI8484068-1":
                        poNo = "WHPO55626";
                        break;
                    case "MI8484068-2":
                        poNo = "WHPO55627";
                        break;
                    case "ME8451266-1":
                        poNo = "WHPO55628";
                        break;
                    case "ME8451279-1":
                        poNo = "WHPO55629";
                        break;
                }
                dr.SetField("ContractNo", contractNo);
                dr.SetField("CustomerPONo", poNo);
                dr.SetField("InvoiceDate", invoiceDef.InvoiceDate);
                dr.SetField("InvoiceAmt", shipmentDef.TotalShippedAmountAfterDiscount);
                ds.BeneficiaryCert.Rows.Add(dr);
                rpt.SetDataSource(ds);

                var cust = contractDef.Customer;
                rpt.SetParameterValue("CustomerName", cust.CustomerDescription);
                rpt.SetParameterValue("CustomerAddress1", cust.Address1);
                rpt.SetParameterValue("CustomerAddress2", cust.Address2 + ", " + cust.Address3);
                rpt.SetParameterValue("CustomerAddress3", cust.Address4);

                if (cust.CustomerId == CustomerDef.Id.smithbrooks.GetHashCode())
                    rpt.SetParameterValue("AttnName", "Chelsie Hallam");

                rpt.SetParameterValue("Currency", shipmentDef.SellCurrency.CurrencyCode);
                SettlementBankDetailDef settlementBank = AccountWorker.Instance.getSettlementBankDetail(contractDef.Office.OfficeId, shipmentDef.SellCurrency.CurrencyId, contractDef.TradingAgencyId);
                if (settlementBank != null)
                {
                    rpt.SetParameterValue("BeneficiaryName", settlementBank.AccountName);
                    rpt.SetParameterValue("AccountNo", settlementBank.AccountNo);
                    rpt.SetParameterValue("SwiftCode", settlementBank.SwiftCode);
                    rpt.SetParameterValue("BankName", settlementBank.BankName);
                    rpt.SetParameterValue("BankAddress", settlementBank.BankAddress);
                }
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, "Beneficiary Cert Print Error - Invoice No : " + invoiceDef.InvoiceNo);
                throw e;
            }
            return rpt;
        }
    }
}
