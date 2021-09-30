/*******************************
!!!!! This page is share between all .net system
!!!!! Please ensure any changes are runable in all other system before check-in

20041104
Kenneth Shum
*******************************/


//var ol_css="FONT-WEIGHT: normal; FONT-SIZE: 11px; COLOR: #000000; FONT-FAMILY: Verdana, Geneva, Arial, Helvetica, sans-serif; BACKGROUND-COLOR: #ffffe8; TEXT-ALIGN: left";
//var ol_css="color: red";
var ol_css = 53;
var ol_fgclass="toolTipTextBg";
var ol_bgclass="toolTipCaptionBg";
var ol_textfontclass="";
var ol_captionfontclass="toolTipCaptionFont";
var ol_closefontclass="";

commonBodyOnLoad();

function commonBodyOnLoad(){
	if (document.body){
		//window.history.forward();
		formatBody();
		initSmartToolTip();
		restoreScrollPosition();
		//document.onreadystatechange=onreadystatechange;
		if (typeof(bodyOnLoad)!='undefined') bodyOnLoad();
	}
	else{
		setTimeout("commonBodyOnLoad();", 10);
	}
}
/*
function onreadystatechange(){
	alert(document.readyState);
}*/
var c=0;
function formatBody(){
	document.body.style.marginTop=0;
	document.body.style.marginBottom=0;
	document.body.style.marginLeft=0;
	document.body.style.marginRight=0;
	//alert(document.body.childNodes[0].childNodes.length);
	//getChildNodes(document.body);
	//alert(c);
}
/*
function getChildNodes(node){
	//alert("hello")
	for (var i=0; i<node.childNodes.length;i++){
		var obj=node.childNodes[i];

//		c=c+1;
alert(obj.DefaultValue)
		if (typeof(obj.DefaultValue)!="undefined"){
			alert();
		}
//			alert(obj.childNodes.length)

		if (obj.childNodes.length>0){
			getChildNodes(obj);
		}
	}
}
*/
function initSmartToolTip(){
		var sHTML="<div id='overDiv' style='position:absolute; visibility:hidden; z-index:1000;'></div>"
		document.body.insertAdjacentHTML("afterBegin",sHTML);

}

function dialogOpen(url, width, height){
	dialog=window.open(url,"dialog","width=" + width + ", height=" + height + ", status=no,toolbar=no,menubar=no,location=no");
	dialog.focus()
}

//isStatus, isToolbar, isMenubar, isLocation
//default setting of windowOpen location=no,menubar=no,status=no,titlebar=yes,toolbar=no,resizable=no,scrollbars=no, toolbar=no, channelmode=no, fullscreen=no
function windowOpen(url, width, height, name, isScrollbars, isResize, extTag){
	var str="left=10, top=10, width=" + width + ", height=" + height + ", location=no,menubar=no,status=no,titlebar=yes,toolbar=no, toolbar=no, channelmode=no, fullscreen=no, resizable=";
	str+=(isResize==true) ? "yes":"no";
	str+=",scrollbars=";
	str+=(isScrollbars==true) ? "yes":"no";
	if (typeof(extTag)!="undefined")
		str+="," + extTag;

	dialog=window.open(url,name, str);
	dialog.focus();
}

function loadSmartCalendar(obj){
	var date=document.all(obj).value;

	if (date=="" && document.all(obj).VisibleDate)
		date=document.all(obj).VisibleDate

	calendar_window=window.open("../common/SmartCalendar.aspx?target=" + obj + "&date=" + date,"calendar_window","width=220,height=190");
	calendar_window.focus()
}

function loadSmartCalendarRange(obj, to){
	var date=document.all(obj).value;
	var dateTo=document.all(to).value;

	if (date=="" && document.all(obj).VisibleDate)
		date=document.all(obj).VisibleDate;

	if (dateTo=="" && document.all(to).VisibleDate)
		dateTo=document.all(to).VisibleDate

	calendar_window=window.open("../common/SmartCalendarRange.aspx?target=" + obj + "&date=" + date + "&targetTo=" + to + "&dateTo=" + dateTo,"calendar_window","width=465,height=190");
	calendar_window.focus()
}

function blankTextOnFocus(obj){
	if (obj.value==obj.DefaultValue){
		obj.value="";
		obj.className="editText";
	}
}

function blankTextOnBlur(obj){
	if (obj.value==""){
		obj.value=obj.DefaultValue;
		obj.className="editTextToolTip";
	}
}

function limitMaxLength(obj)
{
	if (obj && obj.value && obj.maxlength)
		return (obj.value.length < obj.maxlength);
	else
		return true;
}

function setLimitMaxLength(obj)
{
	if (obj && obj.value && obj.maxlength && obj.value.length > obj.maxlength)
		obj.value = obj.value.substring(0, obj.maxlength);
}

/*smartnavigation*/
function restoreScrollPosition() {
//temp remark by kenneth 20050126
	if (typeof(document)!="unknown" && document.forms &&
	document.forms[0] &&
	document.forms[0].StaticPostBackScrollHorizontalPosition &&
	document.forms[0].StaticPostBackScrollVerticalPosition){
	//alert(document.forms[0].StaticPostBackScrollVerticalPosition.value)
		scrollTo(document.forms[0].StaticPostBackScrollHorizontalPosition.value, document.forms[0].StaticPostBackScrollVerticalPosition.value);
		saveScrollPositions();
		//setTimeout("saveScrollPositions()", 100);
	}
	else{
		setTimeout("restoreScrollPosition();", 10);
	}
}

/*smartnavigation*/
function saveScrollPositions() {
	if (typeof(document)!="unknown" && document.forms && document.forms[0] && document.forms[0].StaticPostBackScrollVerticalPosition)
		document.forms[0].StaticPostBackScrollVerticalPosition.value = 	(navigator.appName == 'Netscape') ? document.pageYOffset : document.body.scrollTop;
	if (typeof(document)!="unknown" && document.forms && document.forms[0] && document.forms[0].StaticPostBackScrollHorizontalPosition)
		document.forms[0].StaticPostBackScrollHorizontalPosition.value = (navigator.appName == 'Netscape') ? document.pageXOffset : document.body.scrollLeft;
	setTimeout('saveScrollPositions()', 100);
}

/*Progress Bar*/
function showProgressBar() {
	var left=window.screen.availWidth/3-100;
	var top=window.screen.availHeight/3-100

	waitDialog=window.showModelessDialog("../common/ProgressBar.htm",window,"dialogHeight: 30px; dialogWidth: 250px; center:yes; edge: sunken; center: Yes; help: No; resizable: No; status: No; scroll: No; dialogLeft:" + left + "px; dialogTop:" + top + "px;");

//	waitDialog=window.showModelessDialog("/ecs/tools/ProgressBar.htm",window,"dialogHeight: 50px; dialogWidth: 250px; dialogTop: 150px; dialogLeft: 150px; edge: sunken; center: Yes; help: No; resizable: No; status: No; scroll: No;");
}

function showDMSProgressBar() {
    var left = window.screen.availWidth / 3 - 100;
    var top = window.screen.availHeight / 3 - 100
    // mike 
    waitDialog = window.showModelessDialog("../common/DMSProgressBar.htm", window, "dialogHeight: 30px; dialogWidth: 450px; center:yes; edge: sunken; center: Yes; help: No; resizable: No; status: No; scroll: No; dialogLeft:" + left + "px; dialogTop:" + top + "px;");
}

function onClickScript(obj){
	if (obj.onClick0 && obj.onClick1){
		return (eval(obj.onClick0) && eval(obj.onClick1));
	 }
	if (obj.onClick0){
		return eval(obj.onClick0);
	 }
	return false;
}

/*Text Area*/
function textAreaOnKeyUp(obj){
	var width = obj.style.pixelWidth;
	var height = obj.style.pixelHeight;
	var textLength = obj.value.length;

	if (textLength>0){
		var line = textLength*15/width +1;
		obj.style.pixelHeight = obj.pixelHeight * line;
	}
}

