<%@ Page language="c#" Codebehind="ToolBar.aspx.cs" AutoEventWireup="True" Inherits="com.next.ecs.webapp.ToolBar" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>NEXT SOURCING</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<style>.MENU {
	FONT-SIZE: 11px; COLOR: #ffffff; FONT-FAMILY: Verdana, Geneva, Arial, Helvetica, sans-serif
}
		</style>
	</HEAD>
	<body leftMargin="0" topMargin="0" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="100%" bgColor="#000000" border="0">
				<tr>
					<td>
						<table cellSpacing="0" cellPadding="0" width="780" border="0">
							<tr>
								<td><a href="../myapplication/ApplicationStatus.aspx" target="bodyFrame"><IMG alt="NEXT SOURCING - Travel &amp; Expense Management System" src="../menu/title_ecs.gif"
											border="0"></a></td>
								<td align="right" width="100%" class="menu">
									<!--<a href="../myapplication/ApplicationStatus.aspx" target="bodyFrame"><img src="../menu/btn_home.gif" border="0" alt="Go to Application Status..." align="absMiddle"></a>-->
									<a href="../help/tems_usermanual.pdf" target="_blank" onmouseover="if (parent && parent.bodyFrame && parent.bodyFrame.overlib) return parent.bodyFrame.overlib('Click here to view TEMS online help.');"
										onmouseout="if (parent && parent.bodyFrame && parent.bodyFrame.overlib) return parent.bodyFrame.nd();">
										<img src="../menu/btn_help.gif" border="0" align="absMiddle"></a> <a href="mailto:simon_so@NextSL.com.hk;kenneth_shum@NextSL.com.hk"
										target="bodyFrame" onmouseover="if (parent && parent.bodyFrame && parent.bodyFrame.overlib) return parent.bodyFrame.overlib('Click to send mail or phone directly to <br> Simon So (Ext. 447)  <br> Kenneth Shum (Ext. 477)');"
										onmouseout="if (parent && parent.bodyFrame && parent.bodyFrame.overlib) return parent.bodyFrame.nd();">
										<img src="../menu/btn_support.gif" border="0" align="absMiddle"></a> <a href="../main/Logout.aspx" target="_parent" onmouseover="if (parent && parent.bodyFrame && parent.bodyFrame.overlib) return parent.bodyFrame.overlib('Click here to logout TEMS');"
										onmouseout="if (parent && parent.bodyFrame && parent.bodyFrame.overlib) return parent.bodyFrame.nd();">
										<img src="../menu/btn_logout.gif" border="0" align="absMiddle"></a> | <img src="../menu/img_user.gif" border="0" align="absMiddle">
									<asp:Label id="lblDisplayName" runat="server"></asp:Label>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td><img src="../menu/corner.gif"></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
