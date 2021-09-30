<%@ Control Language="c#" AutoEventWireup="True" Codebehind="UclTestReportIconBar.ascx.cs" Inherits="com.next.isam.webapp.usercontrol.UclTestReportIconBar" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<table cellSpacing="0" cellPadding="0" width="100%" border="0">
	<tr>
		<td vAlign="middle" align="left"><font color="#336699" size="-2"> <IMG src="../images/icon_email.gif">
				Report Informed <IMG src="../images/icon_samplereceived.gif"> Sample Received <IMG src="../images/icon_oversea.gif">
				Offshore Submission <IMG src="../images/icon_external.gif"> External Result
				<asp:Label id="lblUrgent" runat="server" Font-Size="12px" ForeColor="Red" Font-Bold="True">!</asp:Label>
				Urgent Request <span style="DISPLAY: none">
					<asp:Label id="lblExpress" runat="server" Font-Size="12px" ForeColor="Red" Font-Bold="True">+</asp:Label>
					Express Service
					<asp:Label id="lblExpressRejected" runat="server" Font-Size="12px" ForeColor="Red" Font-Bold="True">*</asp:Label>
					Express Request Rejected</span></font>
		</td>
	</tr>
</table>
