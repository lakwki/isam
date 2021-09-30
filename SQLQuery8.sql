DECLARE @CurrentFiscalYear int 
DECLARE @CurrentFiscalPeriod int 
SELECT @CurrentFiscalYear = BudgetYear, @CurrentFiscalPeriod = Period 
FROM nsldb..AccountFinancialCalender 
WHERE getdate() BETWEEN StartDate AND EndDate AND AppId=13 


SELECT FiscalYear=fc.BudgetYear, fc.Period, fc.StartDate, fc.EndDate, r.ExchangeRateTypeId, r.CurrencyId, r.ExchangeRate, BaseExchangeRate=br.ExchangeRate 
INTO #PeriodExchangeRate 
FROM nsldb..AccountFinancialCalender AS fc 

INNER JOIN exchangeRate AS br ON fc.EndDate between br.EffectiveDateFrom and br.EffectiveDateTo and br.currencyId=@BaseCurrencyId and fc.appid=13 and fc.status = 1 
INNER JOIN exchangeRate AS r ON br.EffectiveDateTo=r.EffectiveDateTo and br.ExchangeRateTypeId=r.ExchangeRateTypeId and br.status=1 and r.status=1 

SELECT iv.*
FROM Invoice as iv 
INNER JOIN Shipment as sh WITH (NOLOCK) ON sh.ShipmentId=iv.ShipmentId 
LEFT JOIN CutoffSales as co WITH (NOLOCK) ON co.ShipmentId=iv.ShipmentId 
INNER JOIN (SELECT DateType=1, ReceiptDate='Sales' UNION SELECT DateType=2, ReceiptDate='Commission') as dt ON (@DateType=-1 OR @DateType=dt.DateType) 
INNER JOIN NSLDB..AccountFinancialCalender rp WITH (NOLOCK) ON rp.AppId = 13 AND ((dt.DateType=1 AND iv.ARDate between rp.StartDate and rp.EndDate) OR (dt.DateTYpe=2 AND iv.NSLCommissionSettlementDate between rp.StartDate and rp.EndDate)) 
INNER JOIN NSLDB..AccountFinancialCalender ip WITH (NOLOCK) ON ip.AppId = 13 AND iv.InvoiceDate between ip.StartDate and ip.EndDate 
LEFT JOIN SunInterfaceLog l WITH (NOLOCK) ON l.ShipmentId = iv.ShipmentId AND 1 = 1 
WHERE (@FiscalYear IS NULL OR (co.InvoiceDate IS NOT NULL AND co.FiscalYear = @FiscalYear AND co.Period BETWEEN @PeriodFrom AND @PeriodTo)) 
AND ((@ReceiptDateFrom IS NULL AND @ReceiptDateTo IS NULL) 
OR (dt.DateType=1 AND iv.ARDate BETWEEN '2021-01-30 00:00:00' AND '2021-01-30 00:00:00') 
OR (dt.DateType=2 AND iv.NSLCommissionSettlementDate BETWEEN '2021-01-30 00:00:00' AND '2021-01-30 00:00:00')) 
GROUP BY dt.DateType, iv.ShipmentId, sh.SellCurrencyId, iv.ARDate, iv.ArRefNo, iv.ARAmt, sh.TotalShippedAmt, 
iv.NSLCommissionSettlementDate, iv.NSLCommissionRefNo, iv.NSLCommissionAmt,iv.NSLCommissionSettlementAmt 
) as x 
INNER JOIN Shipment AS s WITH (NOLOCK) ON s.ShipmentId=x.ShipmentId AND s.WorkflowStatusId = 8 AND s.Status = 1  and s.shipmentid = 1245025
INNER JOIN Invoice AS i WITH (NOLOCK) ON s.ShipmentId = i.ShipmentId AND x.ReceiveDate IS NOT NULL AND i.Status = 1 
INNER JOIN Contract AS c WITH (NOLOCK) ON c.ContractId=s.ContractId 
INNER JOIN #PeriodExchangeRate as ir on (ir.currencyId=x.CurrencyId and ir.ExchangeRateTypeId=1 and (ir.FiscalYear=x.InvFiscalYear and ir.Period=x.InvFiscalPeriod)) 
INNER JOIN #PeriodExchangeRate as rr on (rr.currencyId=x.CurrencyId and rr.ExchangeRateTypeId=2 and (rr.FiscalYear=x.RcpFiscalYear and rr.Period=x.RcpFiscalPeriod)) 
INNER JOIN Currency as cy WITH (NOLOCK) ON cy.CurrencyId = x.CurrencyId 
INNER JOIN Currency as bc WITH (NOLOCK) ON bc.CurrencyId=@BaseCurrencyId 
INNER JOIN Office as o WITH (NOLOCK) ON o.OfficeId = c.OfficeId 
INNER JOIN NSLDB..Season as se WITH (NOLOCK) ON se.SeasonId = c.SeasonId 
INNER JOIN NSLIndustry..Vendor as v WITH (NOLOCK) ON v.VendorId = s.VendorId 
INNER JOIN PackingUnit as pu WITH (NOLOCK) ON pu.PackingUnitId = c.PackingUnitId 
INNER JOIN Customer as cu WITH (NOLOCK) ON cu.CustomerId = c.CustomerId 
INNER JOIN TradingAgency AS ta WITH (NOLOCK) ON ta.TradingAgencyId = c.TradingAgencyId 
INNER JOIN product AS p WITH (NOLOCK) ON p.ProductId = c.ProductId 
INNER JOIN (SELECT Distinct ProductTeamId=o.officeStructureId, DeptId=o.ParentId, o.Code, o.Description 
FROM NSLDB..UserSeasonOfficeStructure AS u WITH (NOLOCK) 
INNER JOIN NSLDB..OfficeStructure AS o WITH (NOLOCK) on (o.ParentId=u.OfficeStructureId or o.OfficeStructureId=u.OfficeStructureId) and o.officeStructureTypeId=50 
WHERE u.Status=1 AND o.Status=1 
AND u.UserId = @UserId 
) AS pt ON pt.ProductTeamId=c.ProductTeamId 
WHERE c.Status = 1 
AND c.OfficeId IN (3) 
AND s.SellCurrencyId IN (4,23,12,22,2,1,3) 
AND (NULL is NULL or NULL = x.ReceiveRefNo) 
AND s.TermOfPurchaseId IN (3,2,1,5,4) 
AND s.PaymentTermId IN (2,1) 
AND c.SeasonId IN (42,41,40,39,38,37,36,35,34,33,32,31,30,29,28,27,26,25,24,23,22,21,20,19,18,17,16,14,12,11,10,9,8,7,6,5) 
AND (-1 = -1 OR c.OfficeId <> 17 OR SalesForecastSpecialGroupId = -1) 
AND ((-1 = -1 or c.ProductTeamId = -1)) 
AND c.CustomerId IN (1,2,3,4,5,6,7,8,9,10,11,12,13,15,16,17,18,19,21,22,23,26,29,30,32,33,36,37,38,39,40,41) 
AND c.TradingAgencyId IN (1,2,3,4) 
AND (-1 = -1 OR s.VendorId = -1) 
AND ((0 = -1) 
OR (0 = 0 AND (s.IsMockShopSample<>1 AND s.IsPressSample<>1 AND s.IsStudioSample<>1)) 
OR (0 = 1 AND (s.IsMockShopSample=1 OR s.IsPressSample=1 OR s.IsStudioSample=1))) 
AND (pt.DeptId NOT IN (-100)) 
ORDER BY InvoiceNo, SequenceNo 
