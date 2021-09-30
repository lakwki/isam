<%@ Page language="c#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" Codebehind="BeanContainerManagement.aspx.cs" AutoEventWireup="True" Inherits="com.next.isam.webapp.admin.BeanContainerManagement" Title="DMX"%>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="panel1" runat="server" SkinID="sectionHeader_PersonalSettings">DMX</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<br/>
<asp:DataGrid id="grdBeanContainer" runat="server" AutoGenerateColumns="False" Width="70%">
	<Columns>
		<asp:BoundColumn HeaderText="Type" HeaderStyle-Font-Bold="True"></asp:BoundColumn>
		<asp:BoundColumn HeaderText="Name" HeaderStyle-Font-Bold="True"></asp:BoundColumn>
		<asp:BoundColumn HeaderText="Current Status" HeaderStyle-Font-Bold="True"></asp:BoundColumn>
		<asp:TemplateColumn ItemStyle-HorizontalAlign="Center">
			<ItemTemplate>
			    &nbsp;
				<asp:Button id="btnAction" runat="server" CommandName="doAction" CssClass="btn"></asp:Button>&nbsp;
				<asp:Button id="btnRunNow" runat="server" Text="Run Now" CommandName="runNow" CssClass="btn"></asp:Button>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</asp:DataGrid>
<br>
<asp:Button id="Button1" runat="server" Text="Restart All Bean" SkinId="LButton" onclick="Button1_Click"></asp:Button><br>
</asp:Content>