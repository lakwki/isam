select * from ebookinglog where batchno = '06180'
select * from eb1 where batchno = '06180' order by 3
select * from ebookingcontract where batchno = '06180'

insert eb1 select * from ebookinglog where batchno = '06180' and transactionno <> '0000069'


0002015

select * into eb1 from ebookinglog where batchno = '06180'

delete ebookinglog where batchno = '06180'

select * from eb1

sp_import_booking '06180'

select count(*) from ebookinglog where batchno < '05951'

SELECT * INTO EBookingLog_Archive_Before_Jun_2020 from ebookinglog where batchno < '05951'
delete from ebookinglog where batchno < '05951'

select * from ebookingbatch

05951

update statistics ebookingbatch  
update statistics ebookingtransaction  
update statistics ebookingcontract  
update statistics ebookingshipment  
update statistics ebookingparty  
update statistics ebookingsizeoption  
update statistics ebookingmessage  
update statistics ebookinglog  


sp_helptext sp_UpdateEBDBStat

select count(*) from ebookingcontract where batchno < '05951'

SELECT * INTO EBookingContract_Archive_Before_Jun_2020 from ebookingContract where batchno < '05951'
SELECT * INTO EBookingShipment_Archive_Before_Jun_2020 from ebookingShipment where batchno < '05951'
SELECT * INTO EBookingSizeOption_Archive_Before_Jun_2020 from ebookingSizeOption where batchno < '05951'
SELECT * INTO EBookingParty_Archive_Before_Jun_2020 from ebookingParty where batchno < '05951'


SELECT * INTO EBookingContract_X from ebookingContract where batchno >= '05951'
SELECT * INTO EBookingShipment_X from ebookingShipment where batchno >= '05951'
SELECT * INTO EBookingSizeOption_X from ebookingSizeOption where batchno >= '05951'
SELECT * INTO EBookingParty_X from ebookingParty where batchno >= '05951'



delete  from ebookingContract where batchno < '05951'
delete  from ebookingShipment where batchno < '05951'
delete from ebookingSizeOption where batchno < '05951'
delete from ebookingParty where batchno < '05951'

truncate table ebookingparty

insert ebookingcontract select * from ebookingcontract_x

insert ebookingshipment select * from ebookingshipment_x
insert ebookingsizeoption select * from ebookingsizeoption_x
insert ebookingparty select * from ebookingparty_x



select * from eookinglog


select * from ebookingcontract where contractno = 'AJ8435237'
select * from ebookingshipment where batchno = '06152' and transactionno = '0000028'
select * from ebookingsizeoption where batchno = '06179' and transactionno = '0000203'
select * from ebookingparty where batchno = '06180' and transactionno = '0000154'