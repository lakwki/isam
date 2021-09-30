<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="True" Inherits="com.next.isam.webapp._Default" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>ISAM - Integrated Shipping and Account Management System</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="javascript" src="../includes/keylauncher.js"></script>
		<script lang="javascript">
		function onUnloadPage() {
				if (document.all) {
					if ((window.event.clientX < 0) && (window.event.clientY < 0)) //X button is clicked
					{
						//silent logout
						logout = new Image();
						logout.src="./main/Logout.aspx";
					}
				}
			}
		</script>
	</head>
	<frameset rows="*" frameborder="NO" border="0" framespacing="0" onunload="onUnloadPage();">
		<frame src="main\main.aspx" name="topFrame" scrolling="yes" noresize frameborder="no">
	</frameset>
</html>
