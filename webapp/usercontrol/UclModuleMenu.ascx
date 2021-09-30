<%@ Control Language="c#" AutoEventWireup="True" Codebehind="UclModuleMenu.ascx.cs" Inherits="com.next.ecs.webapp.usercontrol.UclModuleMenu" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<FONT face="·s²Ó©úÅé">
	<asp:Repeater id="repMenu" OnItemDataBound="repMenu_ItemDataBound" runat="server">
				<ItemTemplate>
			<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#297a73">
				<tr>
					<td><%# DataBinder.Eval(Container, "DataItem.MenuPath") %>
						<asp:Repeater id="repSubMenu" runat="server">
							<HeaderTemplate>
								<table width="100%" border="0" cellspacing="0" cellpadding="0">
							</HeaderTemplate>
							<ItemTemplate>
								<tr>
									<td width="5" class="MENU"></td>
									<td class="MENU">
										<%# DataBinder.Eval(Container, "DataItem.Path") %>
									</td>
								</tr>
							</ItemTemplate>
							<FooterTemplate>
			</table>
								</FooterTemplate>
</asp:Repeater>
	<br>
	</td> </tr> </table> </ItemTemplate> </asp:Repeater></FONT>
