select a.contractno, b.deliveryno, b.customeratwarehousedate, dbo.fn_formatinvoiceno(i.invoiceprefix, i.invoiceseq, i.invoiceyear) as invoiceno, d.invoicedate, d.invoiceno as selfbilled_invoiceno, 
case when datepart(hh,d.processeddate)=0 and datepart(mi,d.processeddate)=0 and datepart(ss,d.processeddate)=0 then 'MANUAL INVOICE' else 'SYSTEM INVOICE' END as upload_mode, w.description as workflow_status
from contract a inner join shipment b on a.contractid = b.contractid and a.status = 1 and b.status = 1 and a.contractno + '-' + convert(varchar, b.deliveryno) in (
'VA8479154-1'
)
left join ilsorderref c on c.shipmentid = b.shipmentid left join ilsinvoice d on c.orderrefid = d.orderrefid 
inner join workflowstatus w on b.workflowstatusid = w.workflowstatusid and w.recordtypeid = 1
inner join invoice i on i.shipmentid = b.shipmentid 
order by 4
