<%@ Page Title="Change Password" Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="com.next.isam.webapp.personal.ChangePassword"  %>

<%@ Register TagPrefix="uc1" TagName="UclSubHeader" Src="../usercontrol/UclSubHeader.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_PersonalSettings">Change Password</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <!--
                <LINK href="../includes/stylesheet.css" type="text/css" rel="stylesheet">

				<TABLE class="tbldata" id="Table1" cellSpacing="0" cellPadding="3" width="100%" border="0">
					<TR>
						<TD class="columnHeading" style="HEIGHT: 25px" vAlign="middle" colSpan="4"><B>Change 
								Password</B></TD>
					</TR>
				</TABLE>
        -->
			</P>
			<asp:Panel id="panelChangePassword" runat="server" Width="400">
				<TABLE cellSpacing="1" width="100%"  border="0">
					<TR>
						<TD colSpan="3">Please input all fields and click on Change 
							Password button:<BR>
							<BR>
							<asp:Label id="lblMessage" runat="server" Visible="False" ForeColor="Red"></asp:Label>
							<asp:Panel id="panelNewLogin" runat="server" Visible="False" BackColor="#FFFFC0">
								<asp:Label id="lblNewLoginMsg" runat="server" BackColor="#FFFFC0"><img src="../images/icon_sp.gif" border="0">&nbsp;The system has detected your password is a default password.<br>For security purpose, please change your password other than the default one.</asp:Label>
								<BR>
							</asp:Panel><BR>
						</TD>
					</TR>
					<TR>
						<TD class="FieldLabel2">Current Password:</TD>
						<TD >
							<asp:TextBox id="txtCurrentPwd" runat="server" MaxLength="15" TextMode="Password" CssClass="editText"></asp:TextBox>
							<asp:Label id="lblNoCurrentPassword" runat="server" Visible="False" ForeColor="Red">*</asp:Label></TD>
					</TR>
					<TR>
						<TD class="FieldLabel2">New Password:</TD>
						<TD >
							<asp:TextBox id="txtNewPwd" runat="server" MaxLength="15" TextMode="Password" CssClass="editText"></asp:TextBox>
							<asp:Label id="lblNoNewPassword" runat="server" Visible="False" ForeColor="Red">*</asp:Label></TD>
					</TR>
					<TR>
						<TD class="FieldLabel2">Re-Type New Password</TD>
						<TD >
							<asp:TextBox id="txtVerifyNewPwd" runat="server" MaxLength="15" TextMode="Password" CssClass="editText"></asp:TextBox>
							<asp:Label id="lblNoVerifyPassword" runat="server" Visible="False" ForeColor="Red">*</asp:Label></TD>
					</TR>
					<TR>
						<TD  colSpan="3">
							<asp:Button id="btnChangePassword" runat="server"  Text="Change Password" onclick="btnChangePassword_Click" skinid="LButton" ></asp:Button></TD>
					</TR>
				</TABLE>
			</asp:Panel>
			<br>
			<br>
			<br>
			<br>
			<br>
			<br>
</asp:Content>