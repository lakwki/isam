<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="com.next.isam.webapp.main.ErrorPage" Theme="DefaultTheme"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

			<p>
			<asp:Label  CssClass="AlertHeader" id="lblErrorMessage" runat="server"></asp:Label>
            </p>
			<FONT id="LID1" style="FONT: 8pt/11pt verdana; COLOR: black">
				<P id="LID2">
			</FONT>Please try the following:
			<UL>
				<LI id="instructionsText1">
					Click <a href="javascript: window.history.back();">BACK</a> button to previous 
					page.<br>
					<br>
				<LI id="instructionsText2">
					Click <a href="http://ns-s13/nslhd/default.aspx" target="helpDesk">HERE</a> to go to Help 
					Desk System;<br>
					Or dial Hotline (852) 2376 5433.<br>
					<br>
				<LI id="instructionsText3">
					Contact IT Department for assistance. <FONT color="#ff9900"><STRONG>
							<br>
							Michael Lau : <a href="mailto:michael_lau@nextsl.com.hk">michael_lau@nextsl.com.hk</a><br />
							Cliff Wong : <a href="mailto:cliff_wong@nextsl.com.hk">cliff_wong@nextsl.com.hk</a>
						</STRONG></FONT>
				</LI>
			</UL>
			<br>
			<br>
			<br>
			<br>
			<br>
			<br>
</asp:Content>