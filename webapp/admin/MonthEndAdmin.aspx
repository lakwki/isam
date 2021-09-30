<%@ Page language="c#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" Codebehind="MonthEndAdmin.aspx.cs" AutoEventWireup="True" Inherits="com.next.isam.webapp.admin.MonthEndAdmin" Title="Month End Admin"%>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" alt="workplace banner"/>
-->

<asp:Panel ID="panel1" runat="server" SkinID="sectionHeader_PersonalSettings">Month End Admin</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<br/>
	<span class="header2" style="display:none;">
	    Please use the below panel to freeze / resume the <br />(ILS messages upload & NSS->ISAM data synchronization) which 
		might affect the month end result
	</span>
    <b>Current Period : </b> <asp:Label ID="lbl_FiscalYear" runat="server" Text="" style=" color:Red;font-weight:bold;"></asp:Label>&nbsp;
    <br />
    <b>Automatic Month End Closing : </b> <asp:Label ID="lbl_AutoCutoffTime" runat="server" Text="" style=" color:Red;font-weight:bold;">&nbsp;</asp:Label>&nbsp;
    <asp:Button ID="btnSetReady" runat="server" Visible="false"  Text="Set READY" OnClick="btnSetReady_OnClick" SkinID="MButton"/>
	<br/><br/><br />
	<asp:datagrid id="grdOffice" runat="server" AutoGenerateColumns="False">
		<AlternatingItemStyle CssClass="gridAltItem"></AlternatingItemStyle>
		<ItemStyle CssClass="gridItem"></ItemStyle>
		<HeaderStyle CssClass="gridTitle"></HeaderStyle>
		<Columns>
			<asp:TemplateColumn HeaderText="Office Code" HeaderStyle-Font-Bold="True" HeaderStyle-Width="50px">
				<ItemTemplate>
					<asp:Label id="lblOfficeCode" runat="server" Width="50px"></asp:Label>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Office" HeaderStyle-Font-Bold="True">
				<ItemTemplate>
					<asp:Label id="lblDescription" runat="server" Width="120px"></asp:Label>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Action" HeaderStyle-Font-Bold="True" HeaderStyle-Width="80px" Visible="false">
				<ItemTemplate>
					<asp:Button id="btnChangeStatus" runat="server" CommandName="ChangeStatus" CssClass="btn"></asp:Button>
				</ItemTemplate>
			</asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Month End Status" HeaderStyle-Font-Bold="True">
				<ItemTemplate>
					<asp:Textbox id="txtStatus" runat="server" skinid="TextBox_160" ReadOnly="true"></asp:Textbox>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Month End Action" HeaderStyle-Font-Bold="True" HeaderStyle-HorizontalAlign="Center">
				<ItemTemplate>
                <asp:Table runat="server">
                    <asp:TableRow runat="server">
                        <asp:TableCell>&nbsp;</asp:TableCell>
					    <asp:TableCell id="tdStart" runat="server">
                            <asp:Button id="btnStart" runat="server" CommandName="StartMonthEnd"  SkinID="SButton" text="Start" Enabled="true" Visible="false"></asp:Button>
                            <asp:Button id="btnStartDisabled" runat="server" SkinID="SDisabledButton" text="Start" Enabled="false" Visible="true"></asp:Button>
                        </asp:TableCell>
					    <asp:TableCell id="tdPause" runat="server">
                            <asp:Button id="btnPause" runat="server" CommandName="PauseMonthEnd" SkinID="SButton" text="Pause" Enabled="true" Visible="false"></asp:Button>
					        <asp:Button id="btnPauseDisabled" runat="server" SkinID="SDisabledButton" text="Pause" Enabled="false" Visible="false"></asp:Button>
                        </asp:TableCell>
                        <asp:TableCell>→</asp:TableCell>
					    <asp:TableCell id="tdVerify" runat="server">
                            <asp:Button id="btnVerify" runat="server" CommandName="GenCheckList" SkinID="LButton" Text="Check Discrepancy" Enabled="true" Visible="false"></asp:Button>
					        <asp:Button id="btnVerifyDisabled" runat="server" SkinID="LDisabledButton" Text="Check Discrepancy" Enabled="false" Visible="true"></asp:Button>
                        </asp:TableCell>
                        <asp:TableCell>→</asp:TableCell>
					    <asp:TableCell id="tdReady" runat="server">
                            <asp:Button id="btnReady" runat="server" CommandName="SetMonthEndReady" SkinID="LButton" Text="Ready To Capture" Enabled="true" Visible="false"></asp:Button>
					        <asp:Button id="btnReadyDisabled" runat="server" SkinID="LDisabledButton" Text="Ready To Capture" Enabled="false" Visible="true"></asp:Button>
                        </asp:TableCell>
                        <asp:TableCell>→</asp:TableCell>
                        <asp:TableCell id="tdCapture" runat="server">
                            <asp:Button id="btnCapture" runat="server" CommandName="CaptureSales" SkinID="LButton" Text="Capture Sales" Enabled="true" Visible="false"></asp:Button>
                            <asp:Button id="btnCaptureDisabled" runat="server" SkinID="LDisabledButton" Text="Capture Sales" Enabled="false" Visible="true"></asp:Button>
                        </asp:TableCell>
                        <asp:TableCell>→</asp:TableCell>
                        <asp:TableCell id="tdComplete" runat="server">
                            <asp:Button id="btnComplete" runat="server" CommandName="SetMonthEndCompleted" SkinID="MButton"  text="Complete" Enabled="true" Visible="false"></asp:Button>
                            <asp:Button id="btnCompleteDisabled" runat="server" SkinID="MDisabledButton"  text="Complete" Enabled="false" Visible="true"></asp:Button>
                        </asp:TableCell>
                        <asp:TableCell id="tdFail" runat="server">
                            <asp:Button id="btnFail" runat="server" CommandName="RestartMonthEnd" SkinID="MButton"  text="Restart" Enabled="true" Visible="false"></asp:Button>
                            <asp:Button id="btnFailDisabled" runat="server" SkinID="MDisabledButton"  text="Fail-Restart" Enabled="false" Visible="false"></asp:Button>
                        </asp:TableCell>
                        <asp:TableCell id="tdSendSlippage" runat="server" Visible="true">
                            <asp:Button id="btnSendSlippage" runat="server" CommandName="SendSlippageMail" SkinID="MButton"  text="Send Slippage" Enabled="true" Visible="true"></asp:Button>
                            <asp:Button id="btnSendSlippageDisabled" runat="server" SkinID="MDisabledButton"  text="Send Slippage" Enabled="false" Visible="false"></asp:Button>
                        </asp:TableCell>
                        <asp:TableCell id="tdInterface" runat="server" Visible="true">
                            <asp:Button id="btnInterface" runat="server" CommandName="SubmitInterfaceBatch" SkinID="MButton"  text="Submit Interface Batch" Enabled="true" Visible="true"></asp:Button>
                            <asp:Button id="btnInterfaceDisabled" runat="server" SkinID="MDisabledButton"  text="Submit Interface Batch" Enabled="false" Visible="false"></asp:Button>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
	<br/>
	<br/>
</asp:Content>