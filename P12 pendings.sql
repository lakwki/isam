declare @fromdate datetime = '2020-12-27'
declare @enddate datetime = '2021-01-23'

/*
SELECT b.contractno, a.deliveryno, a.supplieratwarehousedate,a.ShipmentId, b.OfficeId, b.TradingAgencyId, b.IsNextMfgOrder, b.IsPOIssueToNextMfg, a.IsMockShopSample, a.IsStudioSample, c.IsSelfBilledOrder, b.ProductId, 
	b.CustomerId, b.SeasonId, a.SellCurrencyId, a.BuyCurrencyId, a.CustomerDestinationId, a.VendorId, b.ProductTeamId, a.TermOfPurchaseId,
	a.PaymentTermId, b.PackingUnitId, NULL AS InvoicePrefix, 0 AS InvoiceSeq, 0 AS InvoiceYear, NULL AS SequenceNo, NULL AS InvoiceDate, NULL AS InvoiceUploadDate,
	a.CustomerAtWarehouseDate, a.SupplierAtWarehouseDate, c.ActualAtWarehouseDate, c.ILSActualAtWarehouseDate, c.SupplierInvoiceNo, 
	b.PiecesPerPack, b.SetSplitCount, a.QACommissionPercent, a.VendorPaymentDiscountPercent, a.LabTestIncome, b.UKSupplierCode, a.TotalShippedAmt, a.TotalShippedNetFOBAmtAfterDiscount, a.TotalShippedSupplierGmtAmtAfterDiscount,
	ROUND(a.TotalShippedAmt * w.ExchangeRate / y.ExchangeRate, 2) AS TotalShippedAmtInBaseCurrency,
	ROUND(a.TotalShippedNetFOBAmtAfterDiscount * x.ExchangeRate / y.ExchangeRate, 2) AS TotalShippedNetFOBAmtInBaseCurrency, 
	ROUND(a.TotalShippedSupplierGmtAmtAfterDiscount * x.ExchangeRate / y.ExchangeRate, 2) AS TotalShippedSupplierGmtAmtInBaseCurrency, 
	a.TotalShippedQty, ROUND(a.TotalShippedAmt * a.NSLCommissionPercent / 100.0, 2) AS SalesCommission,
	ROUND(ROUND(a.TotalShippedAmt * a.NSLCommissionPercent / 100.0, 2) * w.ExchangeRate / y.ExchangeRate, 2) AS SalesCommissionInBaseCurrency, 
	c.InputVATActualAmt AS UTInputVATAmt,
	c.OutputVATActualAmt AS UTOutputVATAmt,
	c.ImportDutyActualAmt AS UTImportDuty,
	ROUND(ISNULL(c.ImportDutyActualAmt * z.ExchangeRate / y.ExchangeRate, 0),2) AS UTImportDutyInBaseCurrency,
	ROUND(ISNULL(a.TotalShippedAmt * w.ExchangeRate / z.ExchangeRate, 0),2) AS UTTotalShippedAmt,
	ROUND(ISNULL(a.TotalShippedNetFOBAmtAfterDiscount * x.ExchangeRate / z.ExchangeRate, 0),2) AS UTTotalShippedNetFOBAmt,
	ROUND(ISNULL(a.TotalShippedSupplierGmtAmtAfterDiscount * x.ExchangeRate / z.ExchangeRate, 0),2) AS UTTotalShippedSupplierGmtAmt,
	ROUND(ISNULL(a.TotalShippedAmt * a.NSLCommissionPercent / 100.0 * w.ExchangeRate / z.ExchangeRate, 0),2) AS UTSalesCommission,
	1 AS IsAccrual, NULL AS AccruedSince, GETDATE() AS CreatedOn, NULL AS ModifiedOn
FROM Shipment a 
	INNER JOIN Contract b ON a.ContractId = b.ContractId 
		AND a.Status = 1 AND b.Status = 1 AND a.WorkflowStatusId IN (2,4,6,7) --AND b.customerid in (27,28)
		AND a.IsMockShopSample = 0
		AND a.IsStudioSample = 0
		--AND a.CustomerDestinationId IN (4,5)
		AND ((b.CustomerId IN (1, 2, 5, 6, 7) AND a.CustomerDestinationId  IN (1, 2, 3, 6, 7, 69))
		      OR
			  (b.CustomerId in ( 27,28) AND b.UKSupplierCode IN (SELECT UKSupplierCode FROM NSLedSelfBilledSupplierCode WHERE Status = 1)))

	INNER JOIN Invoice c ON a.ShipmentId = c.ShipmentId 
		AND a.supplieratwarehousedate BETWEEN @FromDate AND @EndDate
		--AND ISNULL(c.ActualAtWarehouseDate, c.ILSActualAtWarehouseDate) BETWEEN @FromDate AND @EndDate

	LEFT JOIN ExchangeRate w ON w.CurrencyId = a.SellCurrencyId  AND w.ExchangeRateTypeId = 1 AND @EndDate BETWEEN w.EffectiveDateFrom AND w.EffectiveDateTo
	LEFT JOIN ExchangeRate x ON x.CurrencyId = a.BuyCurrencyId  AND x.ExchangeRateTypeId = 1 AND @EndDate BETWEEN x.EffectiveDateFrom AND x.EffectiveDateTo
	LEFT JOIN ExchangeRate y ON y.CurrencyId = 3 AND y.ExchangeRateTypeId = 1 AND @EndDate BETWEEN y.EffectiveDateFrom AND y.EffectiveDateTo
	LEFT JOIN ExchangeRate z ON z.CurrencyId = 4 AND z.ExchangeRateTypeId = 1 AND @EndDate BETWEEN z.EffectiveDateFrom AND z.EffectiveDateTo
WHERE   (SELECT COUNT(*) FROM SUNExcludeShipment AS cy WITH (NOLOCK) WHERE cy.ShipmentId = a.ShipmentId) = 0
	AND (SELECT COUNT(*) FROM CutOffSales AS cx WITH (NOLOCK) WHERE cx.ShipmentId = a.ShipmentId) = 0
	--and a.SalesForecastSpecialGroupId = 1
	AND a.TermOfPurchaseId NOT IN (5)

*/



SELECT b.ContractNo, a.Deliveryno, a.ShipmentId, o.OfficeCode, cu.CustomerCode ,
       a.CustomerAtWarehouseDate, a.SupplierAtWarehouseDate, c.ActualAtWarehouseDate, c.ILSActualAtWarehouseDate, c.SupplierInvoiceNo, 
       b.UKSupplierCode, 
       ROUND(CASE WHEN b.CustomerId = 8 THEN c.ChoiceOrderTotalShippedAmt ELSE a.TotalShippedAmt END * zs.ExchangeRate / y.ExchangeRate,2) AS TotalSalesAmtInUSD, w.description 
FROM Shipment a INNER JOIN Contract b ON a.ContractId = b.ContractId AND a.Status = 1 --and a.shipmentid = 1125346
		AND b.Status = 1 AND a.WorkflowStatusId IN (8) --AND b.customerid in (27,28) 
		--AND (b.CustomerId IN (27, 28) AND b.UKSupplierCode IN (SELECT UKSupplierCode FROM NSLedSelfBilledSupplierCode WHERE Status = 1))
		AND a.TermOfPurchaseId NOT IN (5)
		--INNER JOIN Product p ON p.ProductId = b.ProductId
		--AND b.CustomerId NOT IN (28)
		-- Remarked by Toby on 2014-10-24 (Shipping will issue CHOICE order invoice)
		-- AND NOT (b.CustomerId = 8 AND b.OfficeId = 1)
		
	--INNER JOIN Invoice c ON a.ShipmentId = c.ShipmentId AND c.InvoiceDate <= @EndDate AND c.InvoiceUploadDate >= @FromDate --Modified by Toby on 2011-11-03
	--INNER JOIN Invoice c ON a.ShipmentId = c.ShipmentId AND c.InvoiceDate <= @EndDate AND c.InvoiceUploadDate >= ( @FromDate - 60 ) --Modified by Toby on 2012-06-06
	INNER JOIN Invoice c ON a.ShipmentId = c.ShipmentId AND c.InvoiceDate <= @EndDate AND c.InvoiceUploadDate >= ( @FromDate - 180 )
	inner join nsldb..office o on o.officeid = b.officeid
	inner join customer cu on cu.customerid = b.customerid 
	inner join workflowstatus w on w.recordtypeid = 1 and w.workflowstatusid = a.workflowstatusid 
	LEFT JOIN ExchangeRate y ON y.CurrencyId = 3 AND y.ExchangeRateTypeId = 1 AND @EndDate BETWEEN y.EffectiveDateFrom AND y.EffectiveDateTo
	LEFT JOIN ExchangeRate z ON z.CurrencyId = 4 AND z.ExchangeRateTypeId = 1 AND @EndDate BETWEEN z.EffectiveDateFrom AND z.EffectiveDateTo
    LEFT JOIN ExchangeRate zs ON zs.CurrencyId = a.SellCurrencyId AND zs.ExchangeRateTypeId = 1 AND @EndDate BETWEEN zs.EffectiveDateFrom AND zs.EffectiveDateTo
	LEFT JOIN ExchangeRate zb ON zb.CurrencyId = a.BuyCurrencyId AND zb.ExchangeRateTypeId = 1 AND @EndDate BETWEEN zb.EffectiveDateFrom AND zb.EffectiveDateTo

WHERE (SELECT COUNT(*) FROM CutOffSales AS cx WITH (NOLOCK) WHERE cx.ShipmentId = a.ShipmentId) = 0
	AND ((0 = 1 AND (a.IsMockShopSample = 0 OR a.IsStudioSample = 0))
	    OR (0 = 0 AND (a.IsMockShopSample = 0 AND a.IsStudioSample = 0)))
	AND a.ShipmentId NOT IN (419567, 433887,419568)
    AND (SELECT COUNT(*) FROM SUNExcludeShipment AS cy WITH (NOLOCK) WHERE cy.ShipmentId = a.ShipmentId) = 0
	and c.IsSelfBilledOrder = 1
	AND NOT EXISTS(SELECt * from nuksales where shipmentid = a.shipmentid)
	--AND EXISTS(SELECT * FROM [ns-led]..RangePlan where itemno = p.itemno AND Status = 1)


SELECT b.ContractNo, a.DeliveryNo, a.ShipmentId,  o.OfficeCode, cu.CustomerCode, 
       a.CustomerAtWarehouseDate, a.SupplierAtWarehouseDate, c.ActualAtWarehouseDate, c.ILSActualAtWarehouseDate, c.SupplierInvoiceNo, 
       b.UKSupplierCode, 
       ROUND(a.TotalOrderAmt * zs.ExchangeRate / y.ExchangeRate,2) AS TotalSalesAmtInUSD, w.Description  AS WorkflowStatus
FROM Shipment a INNER JOIN Contract b ON a.ContractId = b.ContractId AND a.Status = 1 --and a.shipmentid = 1125346
		AND b.Status = 1 AND a.WorkflowStatusId not IN (3,5,9,8) --AND b.customerid in (27,28) 
		--AND (b.CustomerId IN (27, 28) AND b.UKSupplierCode IN (SELECT UKSupplierCode FROM NSLedSelfBilledSupplierCode WHERE Status = 1))
		AND a.TermOfPurchaseId NOT IN (5)
		--INNER JOIN Product p ON p.ProductId = b.ProductId
		--AND b.CustomerId NOT IN (28)
		-- Remarked by Toby on 2014-10-24 (Shipping will issue CHOICE order invoice)
		-- AND NOT (b.CustomerId = 8 AND b.OfficeId = 1)
		
	--INNER JOIN Invoice c ON a.ShipmentId = c.ShipmentId AND c.InvoiceDate <= @EndDate AND c.InvoiceUploadDate >= @FromDate --Modified by Toby on 2011-11-03
	--INNER JOIN Invoice c ON a.ShipmentId = c.ShipmentId AND c.InvoiceDate <= @EndDate AND c.InvoiceUploadDate >= ( @FromDate - 60 ) --Modified by Toby on 2012-06-06
	INNER JOIN Invoice c ON a.ShipmentId = c.ShipmentId AND a.supplieratwarehousedate >= ( @FromDate - 180 ) and a.supplieratwarehousedate <= @EndDate
	inner join nsldb..office o on o.officeid = b.officeid
	inner join customer cu on cu.customerid = b.customerid 
	inner join workflowstatus w on w.recordtypeid = 1 and w.workflowstatusid = a.workflowstatusid 

	LEFT JOIN ExchangeRate y ON y.CurrencyId = 3 AND y.ExchangeRateTypeId = 1 AND @EndDate BETWEEN y.EffectiveDateFrom AND y.EffectiveDateTo
	LEFT JOIN ExchangeRate z ON z.CurrencyId = 4 AND z.ExchangeRateTypeId = 1 AND @EndDate BETWEEN z.EffectiveDateFrom AND z.EffectiveDateTo
    LEFT JOIN ExchangeRate zs ON zs.CurrencyId = a.SellCurrencyId AND zs.ExchangeRateTypeId = 1 AND @EndDate BETWEEN zs.EffectiveDateFrom AND zs.EffectiveDateTo
	LEFT JOIN ExchangeRate zb ON zb.CurrencyId = a.BuyCurrencyId AND zb.ExchangeRateTypeId = 1 AND @EndDate BETWEEN zb.EffectiveDateFrom AND zb.EffectiveDateTo

WHERE (SELECT COUNT(*) FROM CutOffSales AS cx WITH (NOLOCK) WHERE cx.ShipmentId = a.ShipmentId) = 0
	AND ((0 = 1 AND (a.IsMockShopSample = 0 OR a.IsStudioSample = 0))
	    OR (0 = 0 AND (a.IsMockShopSample = 0 AND a.IsStudioSample = 0)))
	AND a.ShipmentId NOT IN (419567, 433887,419568)
    AND (SELECT COUNT(*) FROM SUNExcludeShipment AS cy WITH (NOLOCK) WHERE cy.ShipmentId = a.ShipmentId) = 0
	and c.IsSelfBilledOrder = 1
	AND NOT EXISTS(SELECT * from nuksales where shipmentid = a.shipmentid)
	--AND EXISTS(SELECT * FROM [ns-led]..RangePlan where itemno = p.itemno AND Status = 1)

SELECT b.ContractNo, a.DeliveryNo, a.ShipmentId,  o.OfficeCode, cu.CustomerCode, 
       a.CustomerAtWarehouseDate, a.SupplierAtWarehouseDate, c.ActualAtWarehouseDate, c.ILSActualAtWarehouseDate, c.SupplierInvoiceNo, 
b.UKSupplierCode, 
       ROUND(a.TotalOrderAmt * zs.ExchangeRate / y.ExchangeRate,2) AS TotalSalesAmtInUSD, w.Description as WorkflowStatus 
FROM Shipment a INNER JOIN Contract b ON a.ContractId = b.ContractId AND a.Status = 1 --and a.shipmentid = 1125346
		AND b.Status = 1 AND 	a.workflowstatusid not in (3,5,9, 8) --AND b.customerid in (27,28) 
		--AND (b.CustomerId IN (27, 28) AND b.UKSupplierCode IN (SELECT UKSupplierCode FROM NSLedSelfBilledSupplierCode WHERE Status = 1))
		AND a.TermOfPurchaseId NOT IN (5)
		--INNER JOIN Product p ON p.ProductId = b.ProductId
		--AND b.CustomerId NOT IN (28)
		-- Remarked by Toby on 2014-10-24 (Shipping will issue CHOICE order invoice)
		-- AND NOT (b.CustomerId = 8 AND b.OfficeId = 1)
		
	--INNER JOIN Invoice c ON a.ShipmentId = c.ShipmentId AND c.InvoiceDate <= @EndDate AND c.InvoiceUploadDate >= @FromDate --Modified by Toby on 2011-11-03
	--INNER JOIN Invoice c ON a.ShipmentId = c.ShipmentId AND c.InvoiceDate <= @EndDate AND c.InvoiceUploadDate >= ( @FromDate - 60 ) --Modified by Toby on 2012-06-06
	INNER JOIN Invoice c ON a.ShipmentId = c.ShipmentId AND a.supplieratwarehousedate >= ( @FromDate - 180 ) and a.supplieratwarehousedate <= @EndDate
				and c.IsSelfBilledOrder = 0
	inner join nsldb..office o on o.officeid = b.officeid
	inner join customer cu on cu.customerid = b.customerid 
	inner join workflowstatus w on w.recordtypeid = 1 and w.workflowstatusid = a.workflowstatusid 
	LEFT JOIN ExchangeRate y ON y.CurrencyId = 3 AND y.ExchangeRateTypeId = 1 AND @EndDate BETWEEN y.EffectiveDateFrom AND y.EffectiveDateTo
	LEFT JOIN ExchangeRate z ON z.CurrencyId = 4 AND z.ExchangeRateTypeId = 1 AND @EndDate BETWEEN z.EffectiveDateFrom AND z.EffectiveDateTo
    LEFT JOIN ExchangeRate zs ON zs.CurrencyId = a.SellCurrencyId AND zs.ExchangeRateTypeId = 1 AND @EndDate BETWEEN zs.EffectiveDateFrom AND zs.EffectiveDateTo
	LEFT JOIN ExchangeRate zb ON zb.CurrencyId = a.BuyCurrencyId AND zb.ExchangeRateTypeId = 1 AND @EndDate BETWEEN zb.EffectiveDateFrom AND zb.EffectiveDateTo

WHERE (SELECT COUNT(*) FROM CutOffSales AS cx WITH (NOLOCK) WHERE cx.ShipmentId = a.ShipmentId) = 0
	AND ((0 = 1 AND (a.IsMockShopSample = 0 OR a.IsStudioSample = 0))
	    OR (0 = 0 AND (a.IsMockShopSample = 0 AND a.IsStudioSample = 0)))
	AND a.ShipmentId NOT IN (419567, 433887,419568)
    AND (SELECT COUNT(*) FROM SUNExcludeShipment AS cy WITH (NOLOCK) WHERE cy.ShipmentId = a.ShipmentId) = 0

	--AND NOT EXISTS(SELECt * from nuksales where shipmentid = a.shipmentid)
	--AND EXISTS(SELECT * FROM [ns-led]..RangePlan where itemno = p.itemno AND Status = 1)


	--select * from nsldb..AccountFinancialCalender where appid = 13 and period = 12 and budgetyear = 2020

/*
	select b.contractno, a.deliveryno, a.NSLCommissionPercent, b.customerid from shipment a inner join contract b on a.contractid = b.contractid where a.shipmentid in (
1269290,
1269296,
1269962)

sp_help shipment
*/

