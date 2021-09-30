<table width="100%" border="0" cellspacing="0" cellpadding="0">
	<tr>
		<td bgcolor="#000000" width="55%"><img src="../images/banner_logo.gif" border="0" alt="Next Sourcing Limited"></td>
		<td bgcolor="#000000" align="right"><font color="#ffffff"><b>User : </b>
				<asp:Label id="lblDisplayName" runat="server">So Kwok Choi, Simon</asp:Label></font></td>
		<td bgcolor="#000000" align="right"><strong><a href="help.asp" style="COLOR:#ffffff">Help</a></strong>
			<font color="#ffffff">|</font> <strong><A href="http://nsls10:8080/" style="COLOR:#ffffff" target="helpDesk" title="Hotline: (852) 2376 5433">
					Help Desk</A></strong> <font color="#ffffff">|</font> <strong><a href="#" style="COLOR:#ffffff">
					Logout</a>&nbsp;</strong></td>
	</tr>
</table>
<table width="100%" border="0" cellspacing="0" cellpadding="0">
	<tr>
		<td align="left"><img src="../images/banner_elis.gif" border="0" alt="Laboratory Management System"></td>
		<td align="right" valign="middle"><font color="#ffffff">Search Test Report:</font>&nbsp;<asp:TextBox id="txtLabTestReportKeywordSearch" onKeyPress="getKey();" runat="server" CssClass="formText"></asp:TextBox>
			<asp:ImageButton id="btnLabTestReportSearch" runat="server" ImageUrl="../images/btn_search.gif" ToolTip="Please enter Report No., Description, and Style No."></asp:ImageButton></td>
	</tr>
</table>
<table width="100%" border="0" cellspacing="0" cellpadding="0">
	<tr>
		<td colspan="2"><img src="../images/ruler_orange.gif" border="0" width="100%" height="3"></td>
	</tr>
</table>
<uc1:UclAdvanceSearch id="UclAdvanceSearch1" runat="server"></uc1:UclAdvanceSearch>
<!--no user now !!!include file="../includes/banner.aspx"-->
