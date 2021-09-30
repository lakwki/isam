<%@ Register TagPrefix="cc1" Namespace="com.next.infra.smartwebcontrol" Assembly="com.next.infra.smartwebcontrol" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="UclAutoCompleteTextBox.ascx.cs" Inherits="com.next.isam.webapp.usercontrol.UclAutoCompleteTextBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<span id="pnlBase" runat="server">
	<asp:TextBox id="txt" runat="server" autocomplete="off" CssClass="formText" Width="190px"></asp:TextBox> <!--input type="button" onclick="actb_tocomplete(1);"-->
	<cc1:SmartDropDownList id="cbx" style="DISPLAY: none" runat="server"></cc1:SmartDropDownList>
	<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" EnableClientScript="False" Display="Dynamic"
		ControlToValidate="cbx" Enabled="False">*</asp:RequiredFieldValidator>
</span>
