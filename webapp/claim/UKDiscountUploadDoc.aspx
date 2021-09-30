<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="UKDiscountUploadDoc.aspx.cs" Inherits="com.next.isam.webapp.claim.UKDiscountUploadDoc" %>
<%@ Register TagPrefix="uc1" TagName="UclSmartSelection" Src="../webservices/UclSmartSelection.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="com.next.infra.smartwebcontrol" Assembly="com.next.infra.smartwebcontrol" %>
<%@ Register TagPrefix="uc1" TagName="UclSubHeader" Src="../usercontrol/UclSubHeader.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
		<title>Revise DMS Document</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
        <script language="javascript" src="../includes/common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
		<asp:validationsummary id="vs" runat="server"></asp:validationsummary>
        <table cellspacing="2" cellpadding="2" border="0">
            <tr>
                <td colspan="2" class="tableHeader">Attachment Upload</td>
            </tr>
            <tr>
                <td width="100"><b><span style="color:Black;font-size:8pt;">UK Discount Claim D/N No:</span></b></td>
                <td width="250"><asp:Label runat="server" ID="lbl_UKDNNo" /></td>
            </tr>
            <tr>
                <td><b><span style="color:Black;font-size:8pt;">Status:</span></b></td>
                <td><asp:Label ID="lbl_Status" runat="server" /></td>
            </tr>
        </table>
        <p>&nbsp;<asp:Label ID="lblMode" runat="server" ForeColor="DarkBlue" Font-Bold="true"></asp:Label></p>
        <p>
            &nbsp;<asp:FileUpload runat="server" ID="FileUpload1" /><br />
        </p>
		<p>
			<asp:Button id="btnSave" runat="server" CssClass="btn" Text="Save" onclick="btnSave_Click"></asp:Button>&nbsp;&nbsp;&nbsp;
			<asp:Button id="btnCancel" runat="server" CssClass="btn" Text="Cancel" 
                onclick="btnCancel_Click" />
        </p>
		<br/>
		<br/>
    </form>
</body>
</html>