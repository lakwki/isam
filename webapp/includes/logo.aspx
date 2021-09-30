<table width="100%" border="0" cellspacing="0" cellpadding="0">
	<tr bgcolor="#000000" style="HEIGHT: 25px">
		<td width="150"><img src="../images/banner_logo.gif" border="0" alt="Next Sourcing"></td>
		<td align="right" valign="middle" nowrap><font color="#f0f0f0" align="right"><b>User :</b>
				<asp:Label id="lblDisplayName" runat="server"></asp:Label></font></td>
		<td align="right" valign="middle" nowrap>&nbsp; <font color="#ffffff"><b><a href="../help/Help.aspx" style="COLOR: #ffffff" title="Click to send mail or phone directly to &#13;&#10;Vincent Leung (Ext. 470) &#13;&#10;Man Choy (Ext. 477) &#13;&#10;Cliff Wong (Ext. 230)">
						Help</a></b> | <b><A href="http://nsls10:8080/" style="COLOR: #ffffff" target="helpDesk" title="Hotline: (852) 2376 5433">
						Help Desk</A></b> | <b><a href="../main/logout.aspx" style="COLOR: #ffffff">Logout</a></b></font>&nbsp;&nbsp;</td>
	</tr>
	<tr bgcolor="#647d91">
		<td align="left"><img src="../images/banner_elis.gif" border="0" alt="Laboratory Management System"></td>
		<td align="right" colspan="2" valign="middle" nowrap>
			<asp:Label id="lblSearchReports" runat="server" ForeColor="White">Search Test Report or Item No:</asp:Label>
			<asp:TextBox id="txtLabTestReportKeywordSearch" onKeyPress="getKey();" runat="server" CssClass="formText"></asp:TextBox>
			<asp:ImageButton id="btnLabTestReportSearch" runat="server" ImageUrl="../images/btn_search.gif" ToolTip="Please enter Report No., Description, &amp; Item No."></asp:ImageButton></td>
	</tr>
</table>
<table width="100%" border="0" cellspacing="0" cellpadding="0">
	<tr>
		<td colspan="2"><img src="../images/ruler_orange.gif" border="0" width="100%" height="3"></td>
	</tr>
</table>
