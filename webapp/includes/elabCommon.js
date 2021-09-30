 function openLabTestReport(id){
	windowOpen("../testreport/TestReportDetail.aspx?rid="+id, 770, screen.height*0.88, "testreport" + id, true, false, "status=yes");
 }
 function openExternalLinkage(){
	windowOpen("../testreport/ExternalResultLinkage.aspx", 770, screen.height*0.88, "externallinkage", true, false, "status=yes");
 }
 function openCommercialLabOrder(id){
	windowOpen("../commerciallab/CommercialLabOrderEdit.aspx?cid="+id, 770, screen.height*0.88, "commerciallab" + id, true, false, "status=yes");
 }
  function openCommercialLabOrderView(id){
	windowOpen("../commerciallab/CommercialLabOrderEdit.aspx?cid="+id, 770, screen.height*0.68, "commerciallab" + id, true, false, "status=yes");
 }
 function printCommercialLabOrder(id){
	windowOpen("../reporter/CommercialLabReport.aspx?cid="+id, 770, screen.height*0.88, "commerciallabprint" + id, true, false, "status=yes");
 }
