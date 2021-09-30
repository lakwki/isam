select 'update shipment set ebookinglastuploadedon =  dateadd(d,  1,ebookinglastuploadedon), createdon = dateadd(d,  1,createdon) where shipmentid = ' + convert(varchar, c.shipmentid),
c.shipmentid ,a.contractno, a.deliveryno, c.createdon, convert(varchar, c.createdon, 103) ,c.ebookinglastuploadedon from ebookinglog a inner join contract b on a.contractno = b.contractno and a.batchno = '06347' and a.status = 1
inner join shipment c on b.contractid = c.contractid and a.deliveryno = c.deliveryno and convert(varchar, c.createdon, 103) = '07/09/2021'
order by c.createdon



select a.contractno, a.deliveryno, c.createdon, convert(varchar, c.createdon, 103) ,c.ebookinglastuploadedon from ebookinglog a inner join contract b on a.contractno = b.contractno and a.batchno = '06348' and a.status = 1
inner join shipment c on b.contractid = c.contractid and a.deliveryno = c.deliveryno and convert(varchar, c.createdon, 103) = '08/09/2021'
order by c.createdon

select dateadd(d,  -1,getdate())
