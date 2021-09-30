<table width="100%" border="1" cellspacing="0" cellpadding="0">
	<tr>
		<td width="20%" align="left"><img src="../images/banner_elis.gif" border="0" alt="Laboratory Management System"></td>
		<td align="left"><font color="#FFFFFF">Search Test Report:</font>&nbsp; <input name="txtSearch" type="text" class="formText" size="15">
			<strong><a href="#" style="color:#FFFFFF">Search</a></strong>&nbsp;&nbsp;<a href="#" style="color:#FFFFFF" onClick="expandcontent('AdvancedSearchSection')"><strong>Advanced 
					Search</strong></a></strong>
		</td>
	</tr>
	<tr>
		<td colspan="2"><img src="../images/ruler_orange.gif" border="0" width="1004"></td>
	</tr>
</table>
<!-- ELIS Advanced Search Interface -->
<div name="AdvancedSearchSection" id="AdvancedSearchSection" style="display:none;">
	<table id="Table_AdvancedSearch" width="100%" height="245" border="0" cellpadding="0" cellspacing="0">
		<tr>
			<td colspan="3">
				<img src="../images/elis_search_01.gif" width="1004" height="19" alt=""></td>
		</tr>
		<tr>
			<td>
				<img src="../images/elis_search_02.gif" width="144" height="207" alt=""></td>
			<td width="100%" bgcolor="#FFFFFF" valign="top" align="left" class="title">
				<strong>Test Report Advanced Search</strong><br>
				<br>
				<table width="100%" border="0" cellspacing="2" cellpadding="1">
					<tr>
						<td width="12%">Report No.</td>
						<td><input name="txtReportNo" type="text" size="15" class="formText"></td>
						<td width="12%">Sample Type</td>
						<td>
							<select name="selSampleType" size="1" class="formText">
								<option value="1">-- Please Select --</option>
							</select>
						</td>
						<td width="12%">Test Type</td>
						<td>
							<select name="selTestType" size="1" class="formText">
								<option value="1">-- Please Select --</option>
							</select>
						</td>
					</tr>
					<tr>
						<td width="12%">Range Type</td>
						<td>
							<select name="selRangeType" size="1" class="formText">
								<option value="1">-- Please Select --</option>
								<option value="2">Men</option>
								<option value="3">Women</option>
								<option value="4">Kid</option>
								<option value="5">TSS</option>
							</select>
						</td>
						<td width="12%">Department</td>
						<td>
							<select name="selDepartment" size="1" class="formText">
								<option value="1">-- Please Select --</option>
							</select>
						</td>
						<td width="12%">Season</td>
						<td>
							<select name="selSeason" size="1" class="formText">
								<option value="1">-- Please Select --</option>
							</select>
						</td>
					</tr>
					<tr>
						<td width="12%">Garment Manufacturer</td>
						<td>
							<select name="selGarmentManufacturer" size="1" class="formText">
								<option value="1">-- Please Select --</option>
							</select>
						</td>
						<td width="12%">Fabric Supplier</td>
						<td>
							<select name="selFabricSupplier" size="1" class="formText">
								<option value="1">-- Please Select --</option>
							</select>
						</td>
						<td width="12%">Acc. Supplier</td>
						<td>
							<select name="selAccSupplier" size="1" class="formText">
								<option value="1">-- Please Select --</option>
							</select>
						</td>
					</tr>
					<tr>
						<td width="12%">Style No.</td>
						<td>
							<input name="txtStyleNo" type="text" size="10" value="" class="formText">
						</td>
						<td width="12%">Quality Ref</td>
						<td><input name="txtQualityRef" type="text" size="10" value="" class="formText"></td>
					</tr>
					<tr>
						<td width="12%" valign="top">Date Range</td>
						<td>
							<input name="txtFromDate" type="text" size="11" value="" class="formText">&nbsp;<img src="../images/icon_calendar.gif">&nbsp;
							<input name="txtToDate" type="text" size="11" value="" class="formText">&nbsp;<img src="../images/icon_calendar.gif">&nbsp;
						</td>
						<td width="12%">Validated by</td>
						<td colspan="3">
							<select name="selSampleType" size="1" class="formText">
								<option value="1">-- Please Select --</option>
							</select>
						</td>
					</tr>
				</table>
				<p align="right">
					<button type="button" name="btnSearch" title="Search" style="width:70px">Search</button>&nbsp;
					<button type="button" name="btnSearch" title="Cancel" style="width:70px">Cancel</button>
				</p>
			</td>
			<td>
				<img src="../images/elis_search_04.gif" width="24" height="207" alt=""></td>
		</tr>
		<tr>
			<td colspan="3">
				<img src="../images/elis_search_05.gif" width="1004" height="19" alt=""></td>
		</tr>
	</table>
	<table id="Table_01" width="100%" border="0" cellpadding="0" cellspacing="0">
		<tr>
			<td><img src="../images/ruler_orange.gif"></td>
		</tr>
	</table>
</div>
<script type="text/javascript">

var enablepersist="on" //Enable saving state of content structure using session cookies? (on/off)
var collapseprevious="yes" //Collapse previously open content when opening present? (yes/no)

if (document.getElementById){
document.write('<style type="text/css">')
document.write('.switchcontent{display:none;}')
document.write('</style>')
}

function getElementbyClass(classname){
ccollect=new Array()
var inc=0
var alltags=document.all? document.all : document.getElementsByTagName("*")
for (i=0; i<alltags.length; i++){
if (alltags[i].className==classname)
ccollect[inc++]=alltags[i]
}
}

function contractcontent(omit){
var inc=0
while (ccollect[inc]){
if (ccollect[inc].id!=omit)
ccollect[inc].style.display="none"
inc++
}
}

function expandcontent(cid){
if (typeof ccollect!="undefined"){
if (collapseprevious=="yes")
contractcontent(cid)
document.getElementById(cid).style.display=(document.getElementById(cid).style.display!="block")? "block" : "none"
}
}

function revivecontent(){
contractcontent("omitnothing")
selectedItem=getselectedItem()
selectedComponents=selectedItem.split("|")
for (i=0; i<selectedComponents.length-1; i++)
document.getElementById(selectedComponents[i]).style.display="block"
}

function get_cookie(Name) { 
var search = Name + "="
var returnvalue = "";
if (document.cookie.length > 0) {
offset = document.cookie.indexOf(search)
if (offset != -1) { 
offset += search.length
end = document.cookie.indexOf(";", offset);
if (end == -1) end = document.cookie.length;
returnvalue=unescape(document.cookie.substring(offset, end))
}
}
return returnvalue;
}

function getselectedItem(){
if (get_cookie(window.location.pathname) != ""){
selectedItem=get_cookie(window.location.pathname)
return selectedItem
}
else
return ""
}

function saveswitchstate(){
var inc=0, selectedItem=""
while (ccollect[inc]){
if (ccollect[inc].style.display=="block")
selectedItem+=ccollect[inc].id+"|"
inc++
}

document.cookie=window.location.pathname+"="+selectedItem
}

function do_onload(){
getElementbyClass("switchcontent")
if (enablepersist=="on" && typeof ccollect!="undefined")
revivecontent()
}


if (window.addEventListener)
window.addEventListener("load", do_onload, false)
else if (window.attachEvent)
window.attachEvent("onload", do_onload)
else if (document.getElementById)
window.onload=do_onload

if (enablepersist=="on" && document.getElementById)
window.onunload=saveswitchstate

</script>
