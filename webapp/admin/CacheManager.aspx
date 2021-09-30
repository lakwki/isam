<%@ Page language="c#" Codebehind="CacheManager.aspx.cs" AutoEventWireup="True" Inherits="com.next.isam.webapp.admin.CacheManager" %>
<%@ Page language="c#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" Codebehind="CacheManager.aspx.cs" AutoEventWireup="True" Inherits="com.next.account.webapp.admin.CacheManager" Title="Cache Manager"%>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:Repeater id="Repeater1" runat="server">
		<ItemTemplate>
			<div>Data:
				<asp:DataGrid id="grdData" runat="server"></asp:DataGrid></div>
		</ItemTemplate>
	</asp:Repeater>
</asp:Content>