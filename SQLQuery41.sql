select b.* from advancepayment a inner join shipmentdeduction b on a.paymentno = b.docno and b.status = 1 where a.paymentno in (
'ADV20096',
'ADV20105',
'ADV20109',
'ADV20113',
'ADV20129',
'ADV20147',
'ADV20150',
'ADV20161',
'ADV20162',
'ADV20163',
'ADV20166',
'ADV20172',
'ADV20175',
'ADV20176',
'ADV20178',
'ADV20181',
'ADV20182',
'ADV20183',
'ADV20184',
'ADV20196',
'ADV20197',
'ADV20207',
'ADV20208',
'ADV20209',
'ADV20211',
'ADV20214',
'ADV20216',
'ADV20218',
'ADV21219',
'ADV21220',
'ADV21226',
'ADV21227',
'ADV21248',
'ADV21249',
'ADV21250',
'ADV21251',
'ADV21252',
'ADV21253',
'ADV21266',
'ADV21267',
'ADV21271',
'ADV21272',
'ADV21273',
'ADV21274',
'ADV21275') 