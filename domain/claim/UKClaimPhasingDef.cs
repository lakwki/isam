using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKClaimPhasingDef : DomainData
    {
        public UKClaimPhasingDef()
        {

        }

        public int OfficeId { get; set; }
        public int VendorId { get; set; }
        public int CurrencyId { get; set; }
        public int ClaimTypeId { get; set; }
        public string ClaimReason { get; set; }
        public string Name { get; set; }
        public decimal P01Amount { get; set; }
        public decimal P02Amount { get; set; }
        public decimal P03Amount { get; set; }
        public decimal P04Amount { get; set; }
        public decimal P05Amount { get; set; }
        public decimal P06Amount { get; set; }
        public decimal P07Amount { get; set; }
        public decimal P08Amount { get; set; }
        public decimal P09Amount { get; set; }
        public decimal P10Amount { get; set; }
        public decimal P11Amount { get; set; }
        public decimal P12Amount { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal P01NSAmount { get; set; }
        public decimal P02NSAmount { get; set; }
        public decimal P03NSAmount { get; set; }
        public decimal P04NSAmount { get; set; }
        public decimal P05NSAmount { get; set; }
        public decimal P06NSAmount { get; set; }
        public decimal P07NSAmount { get; set; }
        public decimal P08NSAmount { get; set; }
        public decimal P09NSAmount { get; set; }
        public decimal P10NSAmount { get; set; }
        public decimal P11NSAmount { get; set; }
        public decimal P12NSAmount { get; set; }
        public decimal TotalNSAmount { get; set; }

        public decimal P01VendorAmount { get; set; }
        public decimal P02VendorAmount { get; set; }
        public decimal P03VendorAmount { get; set; }
        public decimal P04VendorAmount { get; set; }
        public decimal P05VendorAmount { get; set; }
        public decimal P06VendorAmount { get; set; }
        public decimal P07VendorAmount { get; set; }
        public decimal P08VendorAmount { get; set; }
        public decimal P09VendorAmount { get; set; }
        public decimal P10VendorAmount { get; set; }
        public decimal P11VendorAmount { get; set; }
        public decimal P12VendorAmount { get; set; }
        public decimal TotalVendorAmount { get; set; }

        public decimal P01AmountInUSD { get; set; }
        public decimal P02AmountInUSD { get; set; }
        public decimal P03AmountInUSD { get; set; }
        public decimal P04AmountInUSD { get; set; }
        public decimal P05AmountInUSD { get; set; }
        public decimal P06AmountInUSD { get; set; }
        public decimal P07AmountInUSD { get; set; }
        public decimal P08AmountInUSD { get; set; }
        public decimal P09AmountInUSD { get; set; }
        public decimal P10AmountInUSD { get; set; }
        public decimal P11AmountInUSD { get; set; }
        public decimal P12AmountInUSD { get; set; }
        public decimal TotalAmountInUSD { get; set; }

        public decimal P01NSAmountInUSD { get; set; }
        public decimal P02NSAmountInUSD { get; set; }
        public decimal P03NSAmountInUSD { get; set; }
        public decimal P04NSAmountInUSD { get; set; }
        public decimal P05NSAmountInUSD { get; set; }
        public decimal P06NSAmountInUSD { get; set; }
        public decimal P07NSAmountInUSD { get; set; }
        public decimal P08NSAmountInUSD { get; set; }
        public decimal P09NSAmountInUSD { get; set; }
        public decimal P10NSAmountInUSD { get; set; }
        public decimal P11NSAmountInUSD { get; set; }
        public decimal P12NSAmountInUSD { get; set; }
        public decimal TotalNSAmountInUSD { get; set; }

        public decimal P01VendorAmountInUSD { get; set; }
        public decimal P02VendorAmountInUSD { get; set; }
        public decimal P03VendorAmountInUSD { get; set; }
        public decimal P04VendorAmountInUSD { get; set; }
        public decimal P05VendorAmountInUSD { get; set; }
        public decimal P06VendorAmountInUSD { get; set; }
        public decimal P07VendorAmountInUSD { get; set; }
        public decimal P08VendorAmountInUSD { get; set; }
        public decimal P09VendorAmountInUSD { get; set; }
        public decimal P10VendorAmountInUSD { get; set; }
        public decimal P11VendorAmountInUSD { get; set; }
        public decimal P12VendorAmountInUSD { get; set; }
        public decimal TotalVendorAmountInUSD { get; set; }

        public decimal LYP01AmountInUSD { get; set; }
        public decimal LYP02AmountInUSD { get; set; }
        public decimal LYP03AmountInUSD { get; set; }
        public decimal LYP04AmountInUSD { get; set; }
        public decimal LYP05AmountInUSD { get; set; }
        public decimal LYP06AmountInUSD { get; set; }
        public decimal LYP07AmountInUSD { get; set; }
        public decimal LYP08AmountInUSD { get; set; }
        public decimal LYP09AmountInUSD { get; set; }
        public decimal LYP10AmountInUSD { get; set; }
        public decimal LYP11AmountInUSD { get; set; }
        public decimal LYP12AmountInUSD { get; set; }
        public decimal LYTotalAmountInUSD { get; set; }

        public decimal LYP01NSAmountInUSD { get; set; }
        public decimal LYP02NSAmountInUSD { get; set; }
        public decimal LYP03NSAmountInUSD { get; set; }
        public decimal LYP04NSAmountInUSD { get; set; }
        public decimal LYP05NSAmountInUSD { get; set; }
        public decimal LYP06NSAmountInUSD { get; set; }
        public decimal LYP07NSAmountInUSD { get; set; }
        public decimal LYP08NSAmountInUSD { get; set; }
        public decimal LYP09NSAmountInUSD { get; set; }
        public decimal LYP10NSAmountInUSD { get; set; }
        public decimal LYP11NSAmountInUSD { get; set; }
        public decimal LYP12NSAmountInUSD { get; set; }
        public decimal LYTotalNSAmountInUSD { get; set; }

        public decimal LYP01VendorAmountInUSD { get; set; }
        public decimal LYP02VendorAmountInUSD { get; set; }
        public decimal LYP03VendorAmountInUSD { get; set; }
        public decimal LYP04VendorAmountInUSD { get; set; }
        public decimal LYP05VendorAmountInUSD { get; set; }
        public decimal LYP06VendorAmountInUSD { get; set; }
        public decimal LYP07VendorAmountInUSD { get; set; }
        public decimal LYP08VendorAmountInUSD { get; set; }
        public decimal LYP09VendorAmountInUSD { get; set; }
        public decimal LYP10VendorAmountInUSD { get; set; }
        public decimal LYP11VendorAmountInUSD { get; set; }
        public decimal LYP12VendorAmountInUSD { get; set; }
        public decimal LYTotalVendorAmountInUSD { get; set; }

        public DateTime LatestShipmentDate { get; set; }

    }
}
