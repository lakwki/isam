<%@ Register TagPrefix="uc1" TagName="UclAdvanceSearch" Src="../usercontrol/UclAdvanceSearch.ascx" %>
	<link href="../includes/global.css" type="text/css" rel="stylesheet">
	<script language="javascript" src="../includes/common.js"></script>
	<script language="javascript" src="../includes/elabCommon.js"></script>
	<script language="javascript" src="../includes/autoCompleteBox.js"></script>
	<script language="javascript" src="../includes/keylauncher.js"></script>
	<script language="JavaScript" src="../includes/overlib.js"><!-- overLIB (c) Erik Bosrup --></script>
	<!--#include file="../includes/logo_ext.aspx"-->
	<asp:TextBox id="Dummy_TextBox1" runat="server" style="display:none"></asp:TextBox>
	<asp:Button id="Dummy_Button1" runat="server" Text="Button" style="display:none"></asp:Button>
	<!--  LMS Working Environment -->
	<!--web sevices-->
	<div id="webService" style="BEHAVIOR: url(../webservices/webservice.htc)"></div>
	<!---->	
	<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#D6E7EF" style="table-layout: fixed;">
		<tr>
			<td width="171" background="../images/space_navigation.gif" align="left" valign="top"
				nowrap>
				<!--  ELIS Navigation Menu -->
<!--
						<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#D6E7EF">
							<tr>
								<td align="left"><img src="../images/space_menu.gif"></td>
							</tr>

							<tr>
								<td align="left"><a href="Main.aspx"><img src="../images/menu_workplace.gif" border="0" alt="My Workplace"></a></td>
							</tr>
						
							<tr>
								<td align="left"><a href="TestResult.aspx"><img src="../images/menu_newtestresult.gif" border="0" alt="New Test Result"></a></td>
							</tr>
							<tr>
								<td align="left"><a href="HistoricalTestResult.aspx"><img src="../images/menu_historicaltestresult.gif" border="0" alt="Historical Test Result"></a></td>
							</tr>
	


						</table> 
-->	
			</td>
			<td align="left" valign="top" bgcolor="#FFFFFF" width="832">
						<img src="../images/banner_workplace.gif" runat="server" id="imgHeader" visible="false">
			<!--  ELIS Content -->
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td width="11">&nbsp;</td>
					<td class="title">

