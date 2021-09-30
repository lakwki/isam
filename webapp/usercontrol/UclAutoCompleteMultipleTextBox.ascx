<%@ Control Language="c#" AutoEventWireup="True" Codebehind="UclAutoCompleteMultipleTextBox.ascx.cs" Inherits="com.next.isam.webapp.usercontrol.UclAutoCompleteMultipleTextBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="cc1" Namespace="com.next.infra.smartwebcontrol" Assembly="com.next.infra.smartwebcontrol" %>
<asp:panel id="pnlBase" runat="server">
	<asp:TextBox id="txt" runat="server" autocomplete="off" CssClass="formText"></asp:TextBox> <!--input type="button" onclick="actb_tocomplete(1);"-->
	<cc1:SmartDropDownList id="cbx" style="DISPLAY: none" runat="server"></cc1:SmartDropDownList>
	<asp:TextBox id="txtIsMultiple" style="DISPLAY: none" runat="server"></asp:TextBox>
	<asp:CustomValidator id="cVal" runat="server" EnableClientScript="False" ErrorMessage="*" Display="Dynamic"
		Enabled="False" onservervalidate="cVal_ServerValidate">*</asp:CustomValidator>
</asp:panel>
<DIV class="formText" id="divMultiple" style="OVERFLOW-Y: scroll; HEIGHT: 80px" runat="server"><asp:checkboxlist id="chkList" runat="server" RepeatColumns="2" BorderStyle="None" DataTextFormatString="<span style='font-size:9px;'> {0}</span>"
		RepeatDirection="Horizontal" CellPadding="0" CellSpacing="0" Font-Size="XX-Small"></asp:checkboxlist>
</DIV>
<IMG id="imgAddMore" src="../images/icon_add.gif" runat="server" style="CURSOR: hand">
