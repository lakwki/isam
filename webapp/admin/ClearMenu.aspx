<%@ Page language="c#" Codebehind="ClearMenu.aspx.cs" AutoEventWireup="True" Inherits="com.next.ecs.webapp.admin.ClearMenu" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>NEXT SOURCING</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<!--#include virtual="../includes/top.aspx"-->
			<P>
				<asp:Button id="Button1" runat="server" Text="Clear Menu &amp; Access Right Cache" DESIGNTIMEDRAGDROP="26"
					CssClass="btn" Width="336px" onclick="Button1_Click"></asp:Button></P>
			<P style="DISPLAY: none">
				<asp:Button id="Button2" runat="server" Text="Clear User Action Cache" CssClass="btn" Width="336px"
					Visible="False" onclick="Button2_Click"></asp:Button></P>
			<P>
				<asp:Button id="Button3" runat="server" Width="336px" CssClass="btn" Text="Clear General Cache" onclick="Button3_Click"></asp:Button></P>
			<!--#include virtual="../includes/bottom.aspx"-->
		</form>
	</body>
</HTML>
