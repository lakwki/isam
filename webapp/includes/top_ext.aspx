<link href="../includes/global.css" type="text/css" rel="stylesheet">
	<script language="javascript">
	function bodyOnLoad() {
		window.history.forward();
	}
	</script>
	<script language="javascript" src="../includes/common.js"></script>
	<script language="javascript" src="../includes/externalCommon.js"></script>
	<script language="javascript" src="../includes/autoCompleteBox.js"></script>
	<script language="javascript" src="../includes/keylauncher.js"></script>
	<script language="JavaScript" src="../includes/overlib.js"><!-- overLIB (c) Erik Bosrup --></script>
	<!--#include file="../includes/logo.aspx"-->
	<asp:TextBox id="Dummy_TextBox1" runat="server" style="display:none"></asp:TextBox>
	<asp:Button id="Dummy_Button1" runat="server" Text="Button" style="display:none"></asp:Button>

	<!--  LMS Working Environment -->
	<!--web sevices-->
	<div id="webService" style="BEHAVIOR: url(../webservices/webservice.htc)"></div>
	<!---->
	<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#D6E7EF" style="table-layout: auto;">
		<tr>
			<td width="171" background="../images/space_navigation.gif" align="left" valign="top"
				nowrap>
				<!--  LMS Navigation Menu -->
				<asp:Repeater id="repMenu" OnItemDataBound="repMenu_ItemDataBound" runat="server" visible="true">
				<ItemTemplate>
						<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#D6E7EF">
							<tr>
								<td background="../images/space_menu.gif">
									<%# DataBinder.Eval(Container, "DataItem.MenuPath") %>
									<asp:Repeater id="repSubMenu" runat="server">
										<HeaderTemplate>
											<table width="171" border="0" cellpadding="0" cellspacing="0">
												<tr>
													<td colspan="3">
														<img src="../images/menu_testrequestbox_01.gif" width="171" alt=""></td>
												</tr>
												<tr>
													<td>
														<img src="../images/menu_testrequestbox_02.gif" width="10" alt=""></td>
													<td width="150" bgcolor="#FFFFFF" valign="top">
										</HeaderTemplate>
										<ItemTemplate>
													&nbsp;&nbsp;<%# DataBinder.Eval(Container, "DataItem.Path") %><br>
										</ItemTemplate>
										<FooterTemplate>
								</td>
								<td>
									<img src="../images/menu_testrequestbox_04.gif" width="11" alt=""></td>
							</tr>
							<tr>
								<td colspan="3">
									<img src="../images/menu_testrequestbox_05.gif" width="171" alt=""></td>
							</tr>
						</table>
										</FooterTemplate>
									</asp:Repeater>
			</td>
		</tr>
		<tr>
			<td align="left"><img src="../images/space_menu.gif"></td>
		</tr>
	</table> 
					</ItemTemplate> 
				</asp:Repeater>
			</td>
			<td align="left" valign="top" bgcolor="#FFFFFF" width="100%">
			<table cellpadding="0" cellspacing="0" border="0" width="100%">
				<tr>
					<td align="left" width="1"><img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" visible="false"></td>
					<td class="columnHeading" align="left"><img src="../images/banner_workplace.gif" runat="server" id="imgHeader" visible="false"></td>
				</tr>
			</table>	
			<br><br>
			<!--  LMS Content -->
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td width="10">&nbsp;</td>
					<td class="title">
