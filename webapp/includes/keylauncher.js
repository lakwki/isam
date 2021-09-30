chooser = "";
keyBuffer = "";
var key  = new Array();
//Define page navigation
key[20] = "../testreport/TestReportList.aspx"; 		//CTRL+SHIFT+T
key[4] = "../debitnote/GenerateDebitNote.aspx";		//CTRL+SHIFT+D
key[16] = "../personal/Filter.aspx";				//CTRL+SHIFT+P
key[12] = "../personal/Label.aspx";					//CTRL+SHIFT+L
function getKey( keyStroke ) {
	isNetscape = ( document.layers );
	chooser    = ( isNetscape )?keyStroke.which:event.keyCode;
	which      = String.fromCharCode( chooser ).toLowerCase();
// Show the keycode
//alert("Key Code: "+chooser);
//	for(var i in key) if (which == i) window.location = key[i];
	for (var i in key) if (chooser == i) window.location = key[i];

	// Search focus - CTRL+SHIFT+F
	if (chooser == 6) {
		window.document.Form1.txtLabTestReportKeywordSearch.focus();

	}
	if (chooser == 13 && window.document.Form1.txtLabTestReportKeywordSearch.value!="") {
//alert("kkk");
		__doPostBack('btnLabTestReportSearch', '');
		//return false;
	}
}
document.onkeypress = getKey;
