declare @FromDate datetime												
declare @ToDate datetime												
set @FromDate = '2021-01-24'												
												
set @ToDate = '2021-01-26'
------------------------------------------------------------------------------------------------------------												
select Title='Shipment within ' + convert(varchar,@FromDate,103) + ' - ' + convert(varchar,@ToDate,103)												
												
drop table #temp_shipment												
drop table #temp_variance												
drop table #temp_result												
												
select iv.ShipmentId, io.OrderRefId, iv.InvoicePrefix, iv.InvoiceSeq, iv.InvoiceYear,												
		iv.InvoiceDate, iv.InvoiceUploadDate, iv.ActualAtWarehouseDate, iv.ILSActualAtWarehouseDate, iv.BookingAtWarehouseDate,										
		OC_ExFtyDate = oc.ExFactoryDate,										
		PL_ExFtyDate = pl.ExFactoryDate, 										
		PL_Qty = pl.TotalPieces, 										
		PL_HandoverDate = pl.HandoverDate, 										
		il.OC_Qty, il.Variance, il.NukOkToMoveDate, il.NukCtsDate										
	into #temp_shipment											
	from invoice as iv with (nolock)											
		inner join Shipment as s with (nolock) on s.ShipmentId=iv.ShipmentId										
		left join ilsOrderRef as io with (nolock) on io.ShipmentId=iv.ShipmentId										
		left join ILSOrderCopy as oc with (nolock) on oc.OrderRefId=io.orderrefid										
		left join ilsPackingList as pl with (nolock) on pl.OrderRefId=io.OrderRefId										
		cross apply (select NukOkToMoveDate, NukCtsDate,										
						oc_Qty = sum(c.qty),						
						Variance = sum(case when abs(convert(decimal(18,10),(d.qty - c.qty)) / convert(decimal(18,10),c.qty)) >= 0.05 						
										then abs(round(convert(decimal(18,10),(d.qty - c.qty)) / convert(decimal(18,10),c.qty) * 100.0,2)) 		
										else 0 end)		
						from (select io.orderRefId, 						
								NukOkToMoveDate = max(case when status=25 then NukExtractDate end),  				
								NukCtsDate = max(case when status=26 then NukExtractDate end)  				
								from ilsMonthEndLog where OrderRefId=io.OrderRefId and Status in (25,26) 				
								) as l				
							left join ilsordercopydetail c with (nolock) on l.orderrefid = c.orderrefid					
							left join ilspackinglistdetail d with (nolock) on c.orderrefid = d.orderrefid and c.optionno = d.optionno					
						group by l.OrderRefId, NukOkToMoveDate, NukCtsDate						
						) as il						
	where s.status=1											
		and s.WorkflowStatusId not in (1,3,5,9)										
		and (s.IsMockShopSample<>1 and s.IsPressSample<>1)										
		and (										
			iv.InvoiceDate between @FromDate and @ToDate 									
			or iv.InvoiceUploadDate between @FromDate and @ToDate 									
												
			or iv.BookingAtWarehouseDate  between @FromDate and @ToDate									
			or iv.ActualAtWarehouseDate  between @FromDate and @ToDate									
			or iv.ILSActualAtWarehouseDate  between @FromDate and @ToDate									
			or s.CustomerAtWarehouseDate  between @FromDate and @ToDate									
			or s.SupplierAtWarehouseDate  between @FromDate and @ToDate									
												
			--or pl.HandoverDate between @FromDate and @ToDate 									
			--or ms.NUKExtractDate between @FromDate and @ToDate 									
		)										
												
												
select 												
		s.ShipmentId, Office=o.OfficeCode, Customer=cs.CustomerCode, 										
		Dept = rtrim(replace(dp.Description,'('+o.OfficeCode+')','')), ProductTeam = pt.Code + ' - ' + pt.Description,										
		p.ItemNo, Supplier = v.Name, 										
		c.ContractNo, DlyNo=s.DeliveryNo, WorkflowStatus = w.Description, 										
		InvoiceNo = dbo.fn_formatInvoiceNo(i.InvoicePrefix,i.InvoiceSeq,i.InvoiceYear),										
		OrgCustomerAwhDate	= isnull(convert(varchar,nss.FirstApprovedCustomerAtWarehouseDate,23),''),									
		CustomerAwhDate		= isnull(convert(varchar,s.CustomerAtwarehouseDate,23),''), 								
		SupplierAwhDate		= isnull(convert(varchar,s.SupplierAtWarehouseDate,23),''), 								
		BookAwhDate			= isnull(convert(varchar,i.BookingAtWarehouseDate,23),''), 							
		InvoiceDate			= isnull(convert(varchar,i.InvoiceDate,23),''), 							
		InvoiceUploadDate	= isnull(convert(varchar,i.InvoiceUploadDate,23),''), 									
		ActualAwhDate		= isnull(convert(varchar,i.ActualAtWarehouseDate,23),''), 								
		IlsActualAwhDate	= isnull(convert(varchar,i.ILSActualAtWarehouseDate,23),''),									
		PL_HandoverDate		= isnull(convert(varchar,i.PL_HandoverDate,23),''),								
		NUKOKToMoveDate		= isnull(convert(varchar,i.NUKOkToMoveDate,23),''),								
		NUKCtsDate			= isnull(convert(varchar,i.NUKCtsDate,23),''),							
		TotalOrderQty		= isnull(convert(varchar,s.TotalOrderQty),''),								
		TotalPoQty			= isnull(convert(varchar,s.TotalPoQty),''),							
		TotalShippedQty		= isnull(convert(varchar,s.TotalShippedQty),''),								
		OC_Qty				= isnull(convert(varchar,i.OC_Qty),''),						
		PL_Qty				= isnull(convert(varchar,i.PL_Qty),''),						
		SizeDiscrepancy		= (case when i.variance<>0 then 'Yes' else 'No' end),								
		OC_ExFtyDate		= isnull(convert(varchar,i.OC_ExFtyDate,23), ''),								
		PL_ExFtyDate		= isnull(convert(varchar,i.PL_ExFtyDate,23), ''),								
		Variance			= isnull(convert(varchar,datediff(d,i.PL_ExFtyDate, i.ILSActualAtWarehouseDate)),''),							
		Currency			= cy.CurrencyCode, 							
		TotalOrderAmtUSD	= isnull(convert(varchar, (case when s.SellCurrencyId=3 then s.TotalOrderAmt else round(s.TotalOrderAmt * r.ExchangeRate / ur.ExchangeRate,2) end)), ''),									
		TotalPoAmtUSD		= isnull(convert(varchar, (case when s.SellCurrencyId=3 then s.TotalPoAmt else round(s.TotalPoAmt * r.ExchangeRate / ur.ExchangeRate,2) end)), ''),								
		TotalShippedAmtUSD	= isnull(convert(varchar, (case when s.SellCurrencyId=3 then s.TotalShippedAmt else round(s.TotalShippedAmt * r.ExchangeRate / ur.ExchangeRate,2) end)), ''),									
		Merchandiser		= u.DisplayName,								
		SlipageRemark		= isnull((select top 1 convert(varchar,sl.FiscalYear)+' P' +convert(varchar,sl.Period) + ' ' 								
								+ (case when AmendInvoiceDate is not null 				
									then ' (Inv. Date ' + isnull(convert(varchar,InvoiceDate,103),'  ') + ' -> ' + convert(varchar,AmendInvoiceDate,103) + ')'			
									else ' (Act.AWH Date ' + isnull(convert(varchar,ActualAtwarehouseDate,103),'  ') + ' -> ' + convert(varchar, AmendAtWarehouseDate ,103) +')' end) 			
								from cutoffslippage as sl where sl.shipmentId=s.ShipmentId),'')				
	into #temp_result											
	from #temp_shipment as i											
		inner join nsldb02.nss.dbo.Shipment as nss on nss.shipmentId=i.ShipmentId										
		inner join shipment as s with (nolock) on s.shipmentid=i.shipmentid										
		inner join Contract as c with (nolock) on c.ContractId=s.ContractId										
		inner join customer as cs with (nolock) on cs.CustomerId=c.CustomerId										
		inner join nsldb..officeStructure as pt with (nolock) on pt.OfficeStructureId=c.ProductTeamId										
		inner join nsldb..officeStructure as dp with (nolock) on dp.OfficeStructureId=c.DeptId										
		inner join product as p with (nolock) on p.productId=c.ProductId										
		inner join currency as cy with (nolock) on cy.currencyId=s.SellCurrencyId										
		inner join office as o with (nolock) on o.officeId=c.OfficeId										
		inner join nsldb..UserInfo as u with (nolock) on u.UserId=c.MerchandiserId										
		inner join nslindustry..vendor as v with (nolock) on v.VendorId=s.VendorId										
		inner join workflowStatus as w with (nolock) on w.WorkflowStatusId=s.WorkflowStatusId and w.RecordTypeId=1										
		left join exchangeRate as r with (nolock) on r.ExchangeRateTypeId=1 and r.CurrencyId=s.SellCurrencyId and isnull(i.InvoiceDate,s.CustomerAtWarehouseDate) between r.EffectiveDateFrom and r.EffectiveDateTo										
		left join exchangeRate as ur with (nolock) on ur.ExchangeRateTypeId=1 and ur.CurrencyId=3 and isnull(i.InvoiceDate,s.CustomerAtWarehouseDate) between ur.EffectiveDateFrom and ur.EffectiveDateTo										
												
--	Size Option Variance											
select a.shipmentid, a.contractno, a.deliveryno, c.optionno, c.optiondesc, oc_qty=c.qty, pl_qty=d.qty,												
		round(convert(decimal(18,10),(d.qty - c.qty)) / convert(decimal(18,10),c.qty) * 100.0,2) as variance										
	into #temp_variance											
	from ilsorderref a with (nolock)											
		inner join ilsordercopy b with (nolock) on a.orderrefid = b.orderrefid										
		inner join (select distinct ShipmentId from #temp_shipment) as t on t.ShipmentId = a.ShipmentId										
		inner join ilsordercopydetail c with (nolock) on b.orderrefid = c.orderrefid										
		inner join ilspackinglistdetail d with (nolock) on c.orderrefid = d.orderrefid and c.optionno = d.optionno										
	where abs(convert(decimal(18,10),(d.qty - c.qty)) / convert(decimal(18,10),c.qty)) >= 0.05											
												
												
select * from #temp_result order by ContractNo,DlyNo												
select * from #temp_variance order by ContractNo,DeliveryNo,OptionNo												



select * into cutoffslippage_20210126 from cutoffslippage where shipmentid = 1197729

--delete from cutoffslippage where shipmentid = 1197729

select * from cutoffsales where shipmentid = 1197729
select * from suninterfacelog where shipmentid = 1197729
select * from nuksales where shipmentid = 1197729

select * from lcapplicationshipment where deducedfabriccost is not null