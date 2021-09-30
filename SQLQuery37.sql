declare @d1 datetime
declare @d2 datetime
set @d1 = '2021-01-14'
set @d2 = '2021-01-18'

select j.officecode, a.contractno, b.deliveryseq as deliveryno, a.currency, sum(t.RevisedQuantity) as qty, sum(t.costprice * t.RevisedQuantity) as amt, bt.importdate, pt.code as productteamcode, 
pt.description as productteamdesc, b.RevisedDeliveryDate, sum(t.costprice * t.RevisedQuantity * ex1.exchangerate / ex2.exchangerate) as amt_usd , cal.budgetyear, cal.period 
from ebookingbatch bt with (nolock) inner join ebookingcontract a with (nolock)  on a.batchno = bt.batchno and convert(datetime,convert(varchar,bt.importdate,112) ) between @d1 and @d2
inner join nsldb..accountfinancialcalender cal on cal.appid = 9 and bt.importdate between cal.startdate and cal.enddate and cal.status = 1


inner join ebookingshipment b with (nolock) on a.batchno = b.batchno and a.transactionno = b.transactionno  and a.stageid = 4 and b.totalqty = 0
inner join contract o with (nolock) on o.contractno = a.contractno and o.status = 1
inner join shipment sh with (nolock) on o.contractid = sh.contractid and sh.deliveryno = b.deliveryseq and sh.status = 1
inner join exchangerate ex1 with (nolock) on ex1.exchangeratetypeid = 1 and ex1.currencyid = sh.sellcurrencyid and ex1.effectivetypeid = 0 and ex1.status = 1
inner join exchangerate ex2 with (nolock) on ex2.exchangeratetypeid = 1 and ex2.currencyid = 3 and ex2.effectivetypeid = 0 and ex2.status = 1

inner join nsldb..officestructure pt with (nolock) on o.productteamid = pt.officestructureid 
inner join nsldb..office j with (nolock) on j.officeid = o.officeid
inner join EBLastBatch lb with (nolock) ON bt.batchno = lb.batchno and a.contractno = lb.contractno and b.deliveryseq = lb.deliveryseq 

inner join ebookingshipment s with (nolock) on s.batchno + s.transactionno = lb.lastbatchno 
inner join ebookingsizeoption t with (nolock) on s.batchno = t.batchno and s.transactionno = t.transactionno and t.deliveryseq = b.deliveryseq 
group by a.contractno, b.deliveryseq, a.currency,j.officecode, bt.importdate, pt.code , pt.description, b.RevisedDeliveryDate,cal.budgetyear, cal.period 
having sum(t.costprice * t.RevisedQuantity) > 0


