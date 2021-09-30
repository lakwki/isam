<%@ Control Language="c#" AutoEventWireup="True" Codebehind="UclSmartSelection.ascx.cs" Inherits="com.next.isam.webapp.webservices.UclSmartSelection" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script language="JavaScript">

</script>
<span style="POSITION: relative" runat="server" id="divBase">
	<div id="service" style="BEHAVIOR: url(../webservices/webservice.htc)" runat="server"></div>
	<div id="divPanel" style="DISPLAY: none; POSITION: relative; z-index:100;" runat="server">Loading...
		<select id="cbxList" style="DISPLAY: none; document: 'none'" multiple name="cbxList" runat="server">
		</select></div>
	<asp:textbox id="txtName" runat="server" CssClass="formText" Width="200px" autocomplete="off"></asp:textbox><asp:Label id="lblClear" runat="server"></asp:Label>
	<asp:textbox id="txtId" runat="server" style="DISPLAY: none" OnTextChanged="txtIdChange"></asp:textbox>
	<asp:textbox id="txtOldName" runat="server" style="DISPLAY: none"></asp:textbox>
</span>
