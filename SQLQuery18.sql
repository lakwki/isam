select a.* from ukclaim a inner join ukclaimrefund b on a.claimid = b.claimid  and a.status = 1 and b.status = 1 and b.claimrefundid in (
4674,
4848,
4719,
4682,
4718,
4833,
4736,
4737,
4675,
4676,
4677,
4717,
4849,
4836)


Reject	


select * from ntinvoice where invoiceid = 50311
select * from ntinvoicedetail where invoiceid = 50311


select * from nsldb..officestructure where code = '312' and officeid = 3

select * from nsldb..userseasonofficestructure where userid = 32219 

select * from nsldb..officestructure where officestructureid in (326,324)
326
1868