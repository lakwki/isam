<%@ Register TagPrefix="cc1" Namespace="com.next.infra.smartwebcontrol" Assembly="com.next.infra.smartwebcontrol" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="UclLoginOffice.ascx.cs" Inherits="com.next.common.web.UclLoginOffice" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<cc1:SmartDropDownList id="cbxOffice" runat="server" CssClass="cbx">
	<asp:ListItem Value="@nextsl.com.hk" Selected="True">Hong Kong</asp:ListItem>
	<asp:ListItem Value="@sh.nextsl.com.cn">Shanghai</asp:ListItem>
	<asp:ListItem Value="@nextsl.co.th">Thailand</asp:ListItem>
	<asp:ListItem Value="@nextsl.com.tr">Turkey</asp:ListItem>
	<asp:ListItem Value="@nextsl.com.lk">Sri Lanka</asp:ListItem>
	<asp:ListItem Value="@NEXTmfg.lk">SriLanka NEXT Mfg.</asp:ListItem>
	<asp:ListItem Value="@nextsl.co.in">India</asp:ListItem>
	<asp:ListItem Value="@nextsl.co.pk">Pakistan</asp:ListItem>
	<asp:ListItem Value="@nextsl.co.uk|@next.co.uk">UK</asp:ListItem>
	<asp:ListItem Value="@nextslbd.com">Bangladesh</asp:ListItem>
	<asp:ListItem Value="">Other</asp:ListItem>
</cc1:SmartDropDownList>
