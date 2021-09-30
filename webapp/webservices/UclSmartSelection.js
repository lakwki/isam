var divPanel=null;
var globalMethod= new Object();
globalMethod.articleNo = new Object();
globalMethod.articleNo.cbxListOnAfterSelection=cbxListOnAfterSelectionForArticleNo;
globalMethod.garmentVendor = new Object();
globalMethod.garmentVendor.cbxListOnAfterSelection = cbxListOnAfterSelectionForGarmentSupplier;


function getSelectionList(selectionKey, webMethod, callBackResult, parentId, para2, para3, para4)
{
	var iCallID;
	divPanel=getPanel(parentId);
	var service=getService(parentId);
	var txtName=getTxtName(parentId);
	var para1=txtName.value;

	if (txtName.value != "") {
	    service.useService("../webservices/SmartSelectionWebServices.asmx?WSDL", "SmartSelectionWebServices");
		iCallID = service.SmartSelectionWebServices.callService(callBackResult, webMethod, selectionKey, parentId, para1, para2, para3, para4);
    }
}

function callWebServices(webMethod, callBackResult, para1, para2, para3, para4){
	var iCallID;
	//divPanel=getPanel(parentId);
	var service=document.all["webService"];
	//var txtName=getTxtName(parentId);

    service.useService("../webservices/SmartSelectionWebServices.asmx?WSDL","SmartSelectionWebServices");

    if (typeof(para1)!="undefined" && typeof(para2)!="undefined" && typeof(para3)!="undefined" && typeof(para4)!="undefined")
		iCallID = service.SmartSelectionWebServices.callService(callBackResult, webMethod, para1, para2, para3, para4);
	else if (typeof(para1)!="undefined" && typeof(para2)!="undefined" && typeof(para3)!="undefined")
		iCallID = service.SmartSelectionWebServices.callService(callBackResult, webMethod, para1, para2, para3);
	else if (typeof(para1)!="undefined" && typeof(para2)!="undefined")
		iCallID = service.SmartSelectionWebServices.callService(callBackResult, webMethod, para1, para2);
	else if (typeof(para1)!="undefined")
		iCallID = service.SmartSelectionWebServices.callService(callBackResult, webMethod, para1);
	else
		iCallID = service.SmartSelectionWebServices.callService(callBackResult, webMethod);
}

function callBackResultGeneral(result)
{
    // if there is an error, and the call came from the call() in init()
    if(result.error)
    {
		throwException(result);
    }
    // if there was no error
    else
    {
        divPanel.innerHTML = result.value;
    }
}
/*
function callBackResultForArticleNo(result)
{
    // if there is an error, and the call came from the call() in init()
    if(result.error)
    {
		throwException(result);
    }
    // if there was no error
    else
    {
        divPanel.innerHTML=result.value;
        cbxListOnAfterSelectionForArticleNo
    }
}
*/

/*event*/
function txtNameOnKeyDown(selectionKey, parentId){
	var keyCode=event.keyCode;

	if (keyCode==13){
		selectList(selectionKey, parentId);
		return false;
	}

	return true;
}
function txtNameOnKeyUp(selectionKey, parentId, webMethod, callBackResult, para2, para3, para4, para5, para6) {

    var panel = getPanel(parentId);
    var txtName = getTxtName(parentId);
    var cbxList = getCbxList(parentId);
    var keyCode = event.keyCode;
    var divBase = getDivBase(parentId);

    if (keyCode == 13) return false;

    if (keyCode >= 9 && keyCode <= 18)
        return;
    switch (keyCode) {
        case 38: //up
            if (cbxList.selectedIndex > 0) cbxList.selectedIndex--;
            break;
        case 40: //down
            if (cbxList.selectedIndex < cbxList.length - 1) cbxList.selectedIndex++;
            break;
        default:
            getSelectionList(selectionKey, webMethod, callBackResult, parentId, para2, para3, para4, para5, para6);
            panel.style.position = "absolute";
            panel.style.left = txtName.offsetLeft - 5;
            panel.style.posTop = txtName.offsetTop + 18;
            panel.style.display = "block";
            panel.style.zIndex = 999;
            divBase.style.zIndex = 999;
            break;
    }
}


function clearSmartSelection(selectionKey, parentId, customClientMethod) {
    alert(selectionKey);
	var txtName = getTxtName(parentId);
	var txtOldName = getTxtOldName(parentId);
	var txtId = getTxtId(parentId);

	txtName.value = "";
	txtName.disabled = false;
	txtOldName.value = "";
	txtId.value = "";

	if (parentId=="uclArticleNo"){
		uclArticleNoOnClear();
	}

	txtName.fireEvent("onchange");
	txtId.fireEvent("onchange");

	if (selectionKey == "garmentVendor") {
	    var contentPlaceHolder1_ddl_Office = document.getElementById("ctl00_ContentPlaceHolder1_ddl_Office");
	    if (typeof (contentPlaceHolder1_ddl_Office) !== 'undefined' && contentPlaceHolder1_ddl_Office != null) {
	        contentPlaceHolder1_ddl_Office.value = "-1";

	    }
	}
}

/*temp solution by kenneth*/
function uclArticleNoOnClear(){
        if(document.all.lblFabricFabricName)
			document.all.lblFabricFabricName.innerText="";
		if(document.all.uclFabricSupplier_txtName)
			document.all.uclFabricSupplier_txtName.innerText="";
		if(document.all.lblFabricConstruction)
			document.all.lblFabricConstruction.innerText="";
        if(document.all.lblFabricComposition)
			document.all.lblFabricComposition.innerText="";
        if(document.all.lblFabricWidth)
			document.all.lblFabricWidth.innerText="";
        if(document.all.lblFabricBeforeWashWeight)
			document.all.lblFabricBeforeWashWeight.innerText="";
        if(document.all.lblFabricFinish)
			document.all.lblFabricFinish.innerText="";
}

function txtNameOnFocus(selectionKey, parentId){
	var txtName = getTxtName(parentId);
	var txtOldName = getTxtOldName(parentId);

	if (txtName.value==txtOldName.value){
		txtName.value="";
	}
}

var mouseOverCbxList = false;

function txtNameOnBlur(selectionKey, parentId) {
    var txtName = getTxtName(parentId);
    var txtOldName = getTxtOldName(parentId);
    var cbxList = getCbxList(parentId);
    var panel = getPanel(parentId);
    var divBase = getDivBase(parentId);

    txtName.value = txtOldName.value;
    panel.style.zIndex = 100;
    divBase.style.zIndex = 100;
    //cbxList.style.zIndex=100;

    if (typeof (cbxList) != "undefined" && cbxList.selectedIndex >= 0) {
        selectList(selectionKey, parentId);
    }
    else {
        var olIe4 = (document.all) ? true : false;
        var olIe7 = false;

        if (olIe4) {
            var agent = navigator.userAgent;
            if (/MSIE/.test(agent)) {
                var versNum = parseFloat(agent.match(/MSIE[ ](\d\.\d+)\.*/i)[1]);
                if (versNum >= 7)
                    olIe7 = true;
            }
        }

        if (olIe7) {
            if (typeof (cbxList) == "undefined") {
                panel.style.display = "none";
            }
            else {
                if (!mouseOverCbxList) panel.style.display = "none";
            }
        }
        else {
            panel.style.display = "none";
        }

        //panel.innerHTML="";
    }
}

var isChangedValue;
/*end*/
function selectList(selectionKey, parentId) {
    var txtName = getTxtName(parentId);
    var txtOldName = getTxtOldName(parentId);
    var txtId = getTxtId(parentId);
    var panel = getPanel(parentId);
    var cbxList = getCbxList(parentId);
    var divBase = getDivBase(parentId);

    if (cbxList && cbxList.selectedIndex >= 0) {
        var displayText;

        if (typeof (cbxList.item(cbxList.selectedIndex).DisplayText) != "undefined") {
            displayText = cbxList.item(cbxList.selectedIndex).DisplayText;
        }
        else {
            displayText = cbxList.item(cbxList.selectedIndex).text
        }

        if (displayText == txtOldName.value)
            isChangedValue = false
        else
            isChangedValue = true

        txtName.value = displayText;
        txtOldName.value = displayText;
        txtId.value = cbxList.item(cbxList.selectedIndex).value;

        // Added by Billy on 20-Jan-2009
        panel.style.zIndex = 100;
        divBase.style.zIndex = 100;
        //cbxList.style.zIndex=100;

        panel.style.display = "none";

        if (globalMethod[selectionKey] && globalMethod[selectionKey].cbxListOnAfterSelection)
            globalMethod[selectionKey].cbxListOnAfterSelection(parentId);
        txtName.fireEvent("onchange");
        txtId.fireEvent("onchange");
    }
    else {
        if (!mouseOverCbxList) panel.style.display = "none";
    }
}



/*call back for garment supplier*/
function cbxListOnAfterSelectionForGarmentSupplier(parentId){

	var iCallID;
	var txtId = getTxtId(parentId);
	var service = getService(parentId);

	service.useService("../webservices/SmartSelectionWebServices.asmx?WSDL", "SmartSelectionWebServices");
    iCallID = service.SmartSelectionWebServices.callService(callBackResultForGetGarmentSupplierContact, "getVendorByKey", txtId.value);

}

function callBackResultForGetGarmentSupplierContact(result){
    if(result.error)
    {
		throwException(result);
    }
    else
    {
        var xmlDoc = new ActiveXObject("microsoft.xmldom");
        xmlDoc.loadXML(result.value);
        var node = xmlDoc.selectSingleNode("VendorRef");
        if (document.all.ctl00_ContentPlaceHolder1_chk_HasUKDN) {
            document.all.ctl00_ContentPlaceHolder1_ddl_Office.value = node.selectSingleNode("OfficeId").text;
            if (node.selectSingleNode("VendorId").text == "353")
                document.all.ctl00_ContentPlaceHolder1_ddl_HandlingOffice.value = "17";
            else 
                document.all.ctl00_ContentPlaceHolder1_ddl_HandlingOffice.value = node.selectSingleNode("OfficeId").text;
        }
    }
}

/*call back for article no*/
function cbxListOnAfterSelectionForArticleNo(parentId){
	var iCallID;
	var txtId = getTxtId(parentId);
	var service = getService(parentId);

    service.useService("../webservices/SmartSelectionWebServices.asmx?WSDL","SmartSelectionWebServices");
    iCallID = service.SmartSelectionWebServices.callService(callBackResultForGetFabricInfoReference, "getFabricInfoReferenceByKey", txtId.value);
}


function callBackResultForGetFabricInfoReference(result){

    if(result.error)
    {
		throwException(result);
    }
    // if there was no error
    else
    {
        var xmlDoc = new ActiveXObject("microsoft.xmldom");

        xmlDoc.loadXML(result.value);

        var node = xmlDoc.selectSingleNode("FabricInfoReferenceRef");

		if(document.all.lblFabricConstruction)
			document.all.lblFabricConstruction.innerText=node.selectSingleNode("Construction").text;
		if(document.all.lblFabricComposition)
			document.all.lblFabricComposition.innerText=node.selectSingleNode("Composition").text;
		if(document.all.lblFabricWidth)
			document.all.lblFabricWidth.innerText=node.selectSingleNode("Width").text;
		if(document.all.lblFabricBeforeWashWeight)
			document.all.lblFabricBeforeWashWeight.innerText=node.selectSingleNode("Weight").text;
		if(document.all.lblFabricFinish)
			document.all.lblFabricFinish.innerText=node.selectSingleNode("Finish").text;
    }
}


/*unitity*/
function getTxtName(parentId) {
    //alert(parentId);
	return document.all[parentId + "_txtName"];
}

function getTxtOldName(parentId) {
    //alert(parentId);
	return document.all[parentId + "_txtOldName"];
}

function getTxtId(parentId) {
    //alert(parentId);
	return document.all[parentId + "_txtId"];
}

function getPanel(parentId){
	return document.all[parentId + "_divPanel"];
}

function getCbxList(parentId){
	return document.all[parentId + "_cbxList"];
}

function getService(parentId){
	return document.all[parentId + "_service"];
}

function getDivBase(parentId) {
    return document.all[parentId + "_divBase"];
}

function getUseValue(parentId) {
    return document.all[parentId + "_txtUseValue"];
}

function getImgEraser(parentId) {
    return document.all[parentId + "_imgEraser"];
}

function enableSmartSelection(parentId) {
    var txtName = getTxtName(parentId);
    var imgEraser = getImgEraser(parentId);
    txtName.disabled = false;
    imgEraser.disabled = false;
    imgEraser.style.cursor = "hand";
}

function disableSmartSelection(parentId) {
    var txtName = getTxtName(parentId);
    var imgEraser = getImgEraser(parentId);
    txtName.disabled = true;
    imgEraser.disabled = true;
    imgEraser.style.cursor = "default";
}


function throwException(result){
        // Pull the error information from the event.result.errorDetail properties
        var xfaultcode   = result.errorDetail.code;
        var xfaultstring = result.errorDetail.string;
        var xfaultsoap   = result.errorDetail.raw;
		alert("Code: " + xfaultcode + "\nFault: " + xfaultstring + "\nSoap: " + xfaultsoap);

        // Add code to handle specific error codes here
}


/*For SmartTextbox*/
var globalMethod_TextOnly = new Object();

globalMethod.pdsproddesignrefno = new Object();
//globalMethod.pdsproddesignrefno.cbxListOnAfterSelection=SmartTextbox_cbxListOnAfterSelection;
globalMethod.pdsproddesignrefno.cbxListOnAfterSelection = cbxListOnAfterSelectionForPdsProdDesignRefNo;



globalMethod.txtarticleNo = new Object();
globalMethod.txtarticleNo.cbxListOnAfterSelection = SmartTextbox_cbxListOnAfterSelectionForArticleNo;


globalMethod_TextOnly.pdsproddesignrefno = new Object();
globalMethod_TextOnly.pdsproddesignrefno.cbxListOnAfterSelection = cbxListOnAfterSelectionForPdsProdDesignRefNo_TextOnly;


function SmartTextbox_txtNameOnKeyDown(selectionKey, parentId) {
    var keyCode = event.keyCode;

    if (keyCode == 13) {
        SmartTextbox_selectList(selectionKey, parentId);
        return false;
    }

    return true;
}


// Modified by Billy on 20-Jan-2009
function SmartTextbox_txtNameOnBlur(selectionKey, parentId) {

    var txtName = getTxtName(parentId);
    var cbxList = getCbxList(parentId);
    var panel = getPanel(parentId);
    var divBase = getDivBase(parentId);

    //alert(selectionKey);
    //panel.style.display="none";
    /*
    if (typeof(cbxList)!="undefined" && cbxList.selectedIndex>=0)
    {
    SmartTextbox_selectList(selectionKey, parentId);
    }
    else if (selectionKey == "pdsproddesignrefno")
    {
    SmartTextbox_selectList(selectionKey, parentId);
    }
    else
    {
    panel.style.display="none";
    }
    */


    if (typeof (cbxList) != "undefined" && cbxList.selectedIndex >= 0) {
        SmartTextbox_selectList(selectionKey, parentId);
    }
    else if (selectionKey == "pdsproddesignrefno") {
        SmartTextbox_selectList(selectionKey, parentId);
    }
    else {
        var olIe4 = (document.all) ? true : false;
        var olIe7 = false;

        if (olIe4) {
            var agent = navigator.userAgent;
            if (/MSIE/.test(agent)) {
                var versNum = parseFloat(agent.match(/MSIE[ ](\d\.\d+)\.*/i)[1]);
                if (versNum >= 7)
                    olIe7 = true;
            }
        }

        if (olIe7) {
            if (typeof (cbxList) == "undefined") {
                panel.style.display = "none";
            }
            else {
                if (!mouseOverCbxList) panel.style.display = "none";
            }
        }
        else {
            panel.style.display = "none";
        }
    }


}




// Modified by Billy on 20-Jan-2009
function SmartTextbox_txtNameOnBlur_Restricted(selectionKey, parentId) {

    var txtName = getTxtName(parentId);
    var cbxList = getCbxList(parentId);
    var panel = getPanel(parentId);
    var txtOldName = getTxtOldName(parentId);
    var divBase = getDivBase(parentId);

    if (txtName.value == "")
        txtOldName.value = "";
    else
        txtName.value = txtOldName.value;

    //panel.style.display="none";

    if (typeof (cbxList) != "undefined" && cbxList.selectedIndex >= 0)
        SmartTextbox_selectList(selectionKey, parentId);
    else
        panel.style.display = "none";
}



function SmartTextbox_callBackResultGeneral(result) {
    // if there is an error, and the call came from the call() in init()
    if (result.error) {
        throwException(result);
    }
    // if there was no error
    else {
        if (result.value != "No record found.") {
            divPanel.innerHTML = result.value;
        }
        else
            divPanel.innerHTML = "";
    }
}


function SmartTextbox_selectList(selectionKey, parentId) {
    var txtName = getTxtName(parentId);
    var txtOldName = getTxtOldName(parentId);
    var txtId = getTxtId(parentId);
    var panel = getPanel(parentId);
    var cbxList = getCbxList(parentId);
    var divBase = getDivBase(parentId);

    var txtUseValue = getUseValue(parentId);

    if (cbxList && cbxList.selectedIndex >= 0) {

        if (txtUseValue.value == "Y") {
            var valueText;

            if (typeof (cbxList.item(cbxList.selectedIndex).value) != "undefined") {
                valueText = cbxList.item(cbxList.selectedIndex).value;
            }
            else {
                displayText = cbxList.item(cbxList.selectedIndex).text;
            }

            txtName.value = valueText;
            txtOldName.value = valueText;
        }
        else {
            var displayText;

            if (typeof (cbxList.item(cbxList.selectedIndex).DisplayText) != "undefined") {
                displayText = cbxList.item(cbxList.selectedIndex).DisplayText;
            }
            else {
                displayText = cbxList.item(cbxList.selectedIndex).text;
            }

            txtName.value = displayText;
            txtOldName.value = displayText;
        }

        txtId.value = cbxList.item(cbxList.selectedIndex).value;

        // Added by Billy on 20-Jan-2009
        panel.style.zIndex = 100;
        divBase.style.zIndex = 100;

        panel.style.display = "none";

        if (globalMethod[selectionKey] && globalMethod[selectionKey].cbxListOnAfterSelection)
            globalMethod[selectionKey].cbxListOnAfterSelection(parentId);

    }
    else if (txtName.values != "") {
        txtOldName.value = txtName.value;

        if (globalMethod_TextOnly[selectionKey] && globalMethod_TextOnly[selectionKey].cbxListOnAfterSelection)
            globalMethod_TextOnly[selectionKey].cbxListOnAfterSelection(parentId);

    }

}


function SmartTextbox_cbxListOnAfterSelection(parentId) {
    var iCallID;
    var service = getService(parentId);
    service.useService("../webservices/SmartSelectionWebServices.asmx?WSDL", "SmartSelectionWebServices");
    //iCallID = service.SmartSelectionWebServices.callService(callBackResultForColor, "getVendorInfoReferenceByKey", "0");    
}




function cbxListOnAfterSelectionForPdsProdDesignRefNo(parentId) {
    var iCallID;
    var txtId = getTxtId(parentId);
    var service = getService(parentId);

    service.useService("../webservices/SmartSelectionWebServices.asmx?WSDL", "SmartSelectionWebServices");
    //alert (txtId.value);
    iCallID = service.SmartSelectionWebServices.callService(callBackResultForGetPdsProdDesignRefNo, "getPdsDesignRefNoByKey", txtId.value);
}


function cbxListOnAfterSelectionForPdsProdDesignRefNo_TextOnly(parentId) {
    var iCallID;
    //var txtId = getTxtId(parentId);

    var officeId = -1;
    if (typeof (document.all["cbxOffice"]) != 'undefined') {
        officeId = parseInt(document.all.item("cbxOffice").value);
    }

    var txtName = getTxtName(parentId);
    var service = getService(parentId);

    service.useService("../webservices/SmartSelectionWebServices.asmx?WSDL", "SmartSelectionWebServices");
    //alert (txtId.value);
    iCallID = service.SmartSelectionWebServices.callService(callBackResultForGetPdsProdDesignRefNo, "getPdsDesignRefNoByDesignRefNo", txtName.value, officeId);
}





var uclTxtArticleNo_ControlId;

function SmartTextbox_cbxListOnAfterSelectionForArticleNo(parentId) {
    //alert("a");	
    var iCallID;
    var txtId = getTxtId(parentId);
    var service = getService(parentId);
    uclTxtArticleNo_ControlId = parentId;
    service.useService("../webservices/SmartSelectionWebServices.asmx?WSDL", "SmartSelectionWebServices");
    iCallID = service.SmartSelectionWebServices.callService(SmartTextbox_callBackResultForGetFabricInfoReference, "getFabricInfoReferenceByKey", txtId.value);
}





function callBackResultForGetPdsProdDesignRefNo(result) {

    //alert (result.value);

    if (result.value != "") {
        if (result.error) {
            throwException(result);
        }
        // if there was no error
        else {
            var xmlDoc = new ActiveXObject("microsoft.xmldom");

            xmlDoc.loadXML(result.value);
            var node = xmlDoc.selectSingleNode("PdsDesignDef");

            var designId = node.selectSingleNode("DesignId").text;
            var designSourceId = node.selectSingleNode("DesignSourceId").text;
            var designerId = node.selectSingleNode("DesignedBy").text;
            var designerName = node.selectSingleNode("DesignerName").text;

            if (designSourceId != "" && designSourceId != "0") {
                if (typeof (document.all["cbxDesignSource"]) != 'undefined') {
                    //alert(designSourceId);
                    document.all.item("cbxDesignSource").value = designSourceId;
                }
            }

            if (designerId != "" && designerId != "0") {
                if (typeof (document.all["uclDesigner_txtName"]) != 'undefined')
                    document.all.item("uclDesigner_txtName").value = designerName;

                if (typeof (document.all["uclDesigner_txtId"]) != 'undefined')
                    document.all.item("uclDesigner_txtId").value = designerId;

                if (typeof (document.all["uclDesigner_txtOldName"]) != 'undefined')
                    document.all.item("uclDesigner_txtOldName").value = designerName;

                if (typeof (document.all["uclDesigner_cbxList"]) != 'undefined')
                    document.all.item("uclDesigner_cbxList").value = designerId;
            }

        }
    }
}





function SmartTextbox_callBackResultForGetFabricInfoReference(result) {
    if (result.error) {
        throwException(result);
    }
    else {
        var xmlDoc = new ActiveXObject("microsoft.xmldom");

        xmlDoc.loadXML(result.value);
        var node = xmlDoc.selectSingleNode("FabricInfoReferenceRef");

        var tmpControl_Prefix = replaceChars(uclTxtArticleNo_ControlId, "uclTxtArticleNo", "")

        if (typeof (document.all[tmpControl_Prefix + "txtDescription"]) != 'undefined') {
            var strFabrication = "";

            //if (node.selectSingleNode("FabricTypeId").text == "1")
            //	strFabrication = strFabrication + "(Knit)";
            //else if (node.selectSingleNode("FabricTypeId").text == "2")
            //	strFabrication = strFabrication + "(Woven)";

            strFabrication = replaceChars(replaceChars(replaceChars(node.selectSingleNode("Composition").text, "||", ";"), "|", ""), ".00", "")
								+ ", " + replaceChars(replaceChars(node.selectSingleNode("Construction").text, "||", ";"), "|", "");


            if (node.selectSingleNode("FabricTypeId").text == "2") {
                if (node.selectSingleNode("Width").text != "" && node.selectSingleNode("FabricWidth").text != "0") {
                    strFabrication = strFabrication + ", " + replaceChars(node.selectSingleNode("Width").text, "|", "");
                }
            }
            else {
                if (node.selectSingleNode("Weight").text != "" && node.selectSingleNode("FabricWeight").text != "0") {
                    strFabrication = strFabrication + ", " + replaceChars(node.selectSingleNode("Weight").text, "|", "");
                }
            }

            document.all.item(tmpControl_Prefix + "txtDescription").innerText = strFabrication;

        }


        if (typeof (document.all[tmpControl_Prefix + "uclFabricTrimVendor_txtName"]) != 'undefined')
            document.all.item(tmpControl_Prefix + "uclFabricTrimVendor_txtName").value = node.selectSingleNode("Vendor/Name").text;

        if (typeof (document.all[tmpControl_Prefix + "uclFabricTrimVendor_txtId"]) != 'undefined')
            document.all.item(tmpControl_Prefix + "uclFabricTrimVendor_txtId").value = node.selectSingleNode("VendorId").text;

        if (typeof (document.all[tmpControl_Prefix + "uclFabricTrimVendor_txtOldName"]) != 'undefined')
            document.all.item(tmpControl_Prefix + "uclFabricTrimVendor_txtOldName").value = node.selectSingleNode("Vendor/Name").text;

        if (typeof (document.all[tmpControl_Prefix + "uclFabricTrimVendor_cbxList"]) != 'undefined')
            document.all.item(tmpControl_Prefix + "uclFabricTrimVendor_cbxList").value = node.selectSingleNode("VendorId").text;




    }
}
