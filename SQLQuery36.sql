				SELECT od.*
				FROM Shipment as s  WITH (NOLOCK)
					INNER JOIN Contract as c WITH (NOLOCK) on c.ContractId = s.ContractId and c.Status = 1
					INNER JOIN Invoice as i  WITH (NOLOCK) on i.ShipmentId = s.ShipmentId and i.Status = 1 and s.shipmentid = 1255048
					INNER JOIN Product as p WITH (NOLOCK) on p.ProductId = c.ProductId
					LEFT JOIN LCApplicationShipment as ls WITH (NOLOCK) on ls.ShipmentId = s.ShipmentId and ls.Status = 1 AND ls.WorkflowStatusId NOT IN (5)		-- Not Rejected
					LEFT JOIN LCApplication as la WITH (NOLOCK) on la.LCApplicationId = ls.LCApplicationId and la.Status = 1
					LEFT JOIN LCBatch as lb WITH (NOLOCK) on lb.LCBatchId = ls.LCBatchId and lb.Status = 1
					LEFT JOIN vw_NSS_AdvancePaymentOrderDetail as nod on nod.ShipmentId = s.ShipmentId and nod.Status = 1
					LEFT JOIN vw_NSS_AdvancePayment as nap on nap.PaymentId = nod.PaymentId and nap.Status = 1 
					LEFT JOIN AdvancePaymentOrderDetail as od on od.shipmentId=s.shipmentId and od.status=1 and (od.PaymentId = nod.PaymentId or nod.paymentid is null)
					LEFT JOIN AdvancePayment as ap on ap.PaymentId=isnull(od.PaymentId,nap.PaymentId) and ap.status = 1
				WHERE s.Status = 1 and ap.paymentno = 'ADV20180'
        
		drop table #LCAdvPay

        --AND isnull(nap.isc19, ap.isc19) =1 
					--AND apod.ActualDeductAmt>0

					AND (@VendorId IS NULL OR s.VendorId = @VendorId)
					AND (@ItemNo IS NULL OR p.ItemNo = @ItemNo)
					AND (@ContractNo IS NULL OR c.ContractNo = @ContractNo)
					AND (c.OfficeId @OfficeIdList)
					AND (c.ProductTeamId @ProductTeamIdList)
					AND (@FromAtWarehouseDate IS NULL OR s.SupplierAtWarehouseDate >= @FromAtWarehouseDate)
					AND (@ToAtWarehouseDate IS NULL OR s.SupplierAtWarehouseDate <= @ToAtWarehouseDate)
					AND (@FromLCIssueDate IS NULL OR i.LCIssueDate >= @FromLCIssueDate)
					AND (@ToLCIssueDate IS NULL OR i.LCIssueDate <= @ToLCIssueDate)
					AND (@FromLCNo IS NULL OR i.LCNo >= @FromLCNo)
					AND (@ToLCNo   IS NULL OR i.LCNo <= @ToLCNo  )
					AND (@FromLCBatchNo IS NULL OR lb.LCBatchNo >= @FromLCBatchNo)
					AND (@ToLCBatchNo IS NULL OR lb.LCBatchNo <= @ToLCBatchNo)
					AND ((@FromAdvancePaymentNo IS NULL AND @ToAdvancePaymentNo IS NULL) 
						OR (ap.PaymentNo BETWEEN ISNULL(@FromAdvancePaymentNo,@ToAdvancePaymentNo) AND ISNULL(@ToAdvancePaymentNo,@FromAdvancePaymentNo)) 
						OR (nap.PaymentNo BETWEEN ISNULL(@FromAdvancePaymentNo,@ToAdvancePaymentNo) AND ISNULL(@ToAdvancePaymentNo,@FromAdvancePaymentNo)))
					AND ((@FromNSLRefNo IS NULL AND @ToNSLRefNo IS NULL) 
						OR (nap.NSLRefNo BETWEEN ISNULL(@FromNSLRefNo,@ToNSLRefNo) AND ISNULL(@ToNSLRefNo,@FromNSLRefNO))
						OR (ap.FLRefNo BETWEEN ISNULL(@FromNSLRefNo,@ToNSLRefNo) AND ISNULL(@ToNSLRefNo,@FromNSLRefNO)))
					AND (@FromApprovalDate IS NULL OR ap.ApprovedDate >= @FromApprovalDate)
					AND (@ToApprovalDate IS NULL OR ap.ApprovedDate <= @ToApprovalDate)

				SELECT y.* 
				FROM (SELECT DISTINCT ShipmentId FROM #LCAdvPay) AS x
				CROSS APPLY (SELECT TOP 1 * 
								FROM #LcAdvPay WHERE ShipmentId = x.ShipmentId 
								ORDER BY LCApplicationNo DESC, AdvancePaymentCreationDate DESC, NSSOrderDetailCreationDate DESC) as y
				ORDER BY convert(date,ApprovedDate), PaymentNo, ContractNo, DeliveryNo