492792  - ND
M05388 - LK
724603  – LK
751329 


select a.* from rangeplan a inner join rangeplanlatestversionid b on a.rangeplanid = b.rangeplanid and a.versionid = b.versionid and a.status =1
and a.itemno in (
'122950',
'122950',
'224749',
'224749',
'228661',
'228661',
'233964',
'233964',
'348620',
'380510',
'380510',
'495228',
'495228',
'746826',
'746826',
'790098',
'862902'
)


