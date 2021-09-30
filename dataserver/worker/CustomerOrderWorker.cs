using System;
using com.next.infra.persistency.dataaccess;
using com.next.common.datafactory.worker;
using com.next.isam.dataserver.model.nss;
using com.next.isam.domain.order;

namespace com.next.isam.dataserver.worker
{
    public class CustomerOrderWorker : Worker
    {
        private static CustomerOrderWorker _instance;
        private GeneralWorker generalWorker;

        public CustomerOrderWorker()
        {
            generalWorker = GeneralWorker.Instance;
        }

        public static CustomerOrderWorker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CustomerOrderWorker();
                }
                return _instance;
            }
        }

        public CustomerOrderCopyDef getCustomerOrderCopy(int shipmentId, ContractDef contract)
        {
            CustomerOrderCopyDef order = null;
            IDataSetAdapter ad = getDataSetAdapter("EzibuyOrderCopySummaryApt", "GetEzibuyOrderCopySummaryByContractNo");
            ad.SelectCommand.Parameters["@ContractNo"].Value = contract.ContractNo;
            ad.SelectCommand.Parameters["@BookingRefNo"].Value = contract.BookingRefNo;
            EzibuyOrderCopySummaryDs dataset = new EzibuyOrderCopySummaryDs();
            int recordsAffected = ad.Fill(dataset);
            if (recordsAffected > 0)
            {   
                // output Ezibuy order copy
                order = new CustomerOrderCopyDef();
                EzibuyOrderCopySummaryDs.EzibuyOrderCopySummaryRow row = dataset.EzibuyOrderCopySummary[0];
                
                order.OrderSource = "EZIBUY";
                order.OrderRefId = int.MinValue;
                order.ContractNo = row.PurchaseOrder;
                order.DeliveryNo = string.Empty;
                order.ItemNo = string.Empty; ;
                order.ItemDesc = string.Empty; ;
                order.TransportMode = row.Mode;
                order.CountryOfOrigin = string.Empty; ;
                order.DepartCountry = (row.IsPortLoad_codeNull() ? string.Empty : row.PortLoad_code.Substring(0, 2));
                order.ExFactoryDate = (row.IsExWorksNull() ? DateTime.MinValue : row.ExWorks);
                order.InWarehouseDate = DateTime.MinValue;
                order.SupplierCode = string.Empty; ;
                order.SupplierName = string.Empty; ;
                order.HangBox = string.Empty; ;
                order.BuyingTerms = (row.IsIncoTermsNull() ? string.Empty : row.IncoTerms);
                if (row.IsPortDischarge_codeNull())
                    order.FinalDestination = string.Empty;
                else
                    order.FinalDestination = (row.PortDischarge_code.Length >= 2 ? row.PortDischarge_code.Substring(0, 2) : string.Empty);
                order.Currency = (row.IsCurrencyNull() ? string.Empty : row.Currency);
                order.NextFreightPercent = 0;
                order.SupplierFreightPercent = 0;
                order.ArrivalPort = (row.IsPortDischarge_nameNull() ? string.Empty : row.PortDischarge_name);
                order.FranchisePartnerCode = string.Empty; 
                order.Refurb = string.Empty; 
                order.FileNo = string.Empty; 
                if (row.IsDateTime_CreatedNull())
                    order.ImportDate = DateTime.MinValue;
                else
                {
                    DateTime importDate;
                    if (!DateTime.TryParse(row.DateTime_Created, out importDate))
                        order.ImportDate = importDate;
                    else
                        order.ImportDate = DateTime.MinValue;
                }
                order.LastSentOfficeCode = string.Empty; 
                order.LastSentQuota = string.Empty; 
                order.LastSentDocType = string.Empty; 
                order.IsValid = true;

                order.Forwarder = (row.IsForwarderNull() ? string.Empty : row.Forwarder);
                order.ShipFrom = (row.IsPortLoad_codeNull() ? string.Empty : row.PortLoad_code);
                order.ScheduledDeliveryDate = (row.IsScheduledDeliveryNull() ? DateTime.MinValue : row.ScheduledDelivery);
                order.PromotionStartDate = (row.IsPromotionStartDateNull() ? DateTime.MinValue : row.PromotionStartDate);
                order.TotalOrderAmount = (row.IsTotalOrderAmountNull() ? 0 : row.TotalOrderAmount);
                order.TotalOrderQuantity = (row.IsTotalOrderQuantityNull() ? 0 : row.TotalOrderQuantity);
            }
            else
            {
                // Order Copy from other customer
            }
            return order;
        }
 
        public CustomerOrderCopyDef getEzibuyOrderCopyByShipmentId(int shipmentId)
        {
            CustomerOrderCopyDef def = new CustomerOrderCopyDef();
            return def;
        }

        public CustomerOrderCopyDef getEzibuyOrderCopyDetailList(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("CustomerOrderCopyApt", "GetEzibuyOrderCopyDetailByOrderRefId");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;

            CustomerOrderCopyDef def = new CustomerOrderCopyDef();
            return def;
        }
    }
}
