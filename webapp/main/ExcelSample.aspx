<%@ Page Language="c#" CodeBehind="ExcelSample.aspx.cs" AutoEventWireup="True" ValidateRequest="false" Inherits="com.next.ecs.webapp.main.Default" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>NEXT SOURCING</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			xml String
			<asp:TextBox id="txtXMLFile" runat="server" Rows="10" TextMode="MultiLine" Width="384px" Height="336px">aatest</asp:TextBox>xsl 
			file
			<asp:TextBox id="txtXSLFile" runat="server">ActualExcel</asp:TextBox>
			<asp:Button id="Button1" runat="server" Text="Button" onclick="Button1_Click"></asp:Button>
		</form>
	</body>
</HTML>
