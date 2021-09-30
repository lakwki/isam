<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="GenSunAccountInterfaceFile.aspx.cs" Inherits="com.next.isam.webapp.nontrade.GenSunAccountInterfaceFile" Title="Generate SunAccount Interface File For Non-Trade Expense" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--    
    <img src="../images/banner_account_sun.gif" runat="server" id="imgHeaderText" />
    <img src="../images/banner_workplace.gif" runat="server" id="img1" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Epicor Interface File Generation For Non-Trade Expense</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
    function updateControls() {
        document.all.<%= chkMacro.ClientID %>.disabled=(document.all.<%=ddl_SunInterfaceType.ClientID %>.value == '1004');
        if (document.all.<%=ddl_SunInterfaceType.ClientID %>value == '1004') 
            document.all.<%= chkMacro.ClientID %>.checked=false;
    }
</script>
<table width="800px">
    <tr>
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:120px;">Office</td>
        <td><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="250px" 
                onselectedindexchanged="ddl_Office_SelectedIndexChanged" AutoPostBack="True"></cc2:smartdropdownlist>
    </td>
    </tr>
    <tr>
        <td class="FieldLabel2">File Type</td>
        <td><cc2:smartdropdownlist id="ddl_SunInterfaceType" runat="server" Width="250px" 
                AutoPostBack="True" 
                onselectedindexchanged="ddl_SunInterfaceType_SelectedIndexChanged"></cc2:smartdropdownlist>
        </td>
    </tr>
    <tr runat="server" id="trFiscalYear">
        <td class="FieldLabel2">Fiscal Year</td>
        <td>
            <cc2:smartdropdownlist id="ddl_Year" runat="server" Width="250px" 
                AutoPostBack="True" onselectedindexchanged="ddl_Year_SelectedIndexChanged"></cc2:smartdropdownlist>
        </td>
    </tr>
    <tr runat="server" id="trPeriod">
        <td class="FieldLabel2">Period</td>
        <td>
            <cc2:smartdropdownlist id="ddl_Period" runat="server" Width="250px">
            </cc2:smartdropdownlist>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">Auto-Upload to Epicor</td>
        <td>
            <asp:CheckBox runat="server" ID="chkMacro"/>
            <input type="hidden" runat="server" id="hidSuper" />
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>
            <asp:Button ID="btn_Process" runat="server" Text="Process" 
                onclick="btn_Process_Click" />
            <asp:CustomValidator ID="valSubmission" runat="server" Display="None" 
                ErrorMessage="Daily Purchase Interface request is not allowed during month-end process" 
                onservervalidate="valSubmission_ServerValidate"></asp:CustomValidator>
        </td>
    </tr>
</table>
<asp:GridView ID="gv_Request" runat="server" AutoGenerateColumns="false" 
        AllowPaging="true" PageSize="100" 
        onrowdatabound="gv_Request_RowDataBound" 
        OnPageIndexChanging="gv_Request_PageIndexChanging">
        <PagerSettings Mode="Numeric" Position="TopAndBottom"/>
        <PagerStyle Font-Bold="true" HorizontalAlign="Right" />
    <Columns>
        <asp:TemplateField HeaderText="Request Id">
            <ItemTemplate>
                <asp:Label ID="lbl_RequestId" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="File Type" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <asp:Label ID="lbl_SunInterfaceType" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Office Group">
            <ItemTemplate>
                <asp:Label ID="lbl_Office" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Year/<br>Period">
            <ItemTemplate>
                <asp:Label ID="lbl_Period" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Phase" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <asp:Label ID="lbl_Phase" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Requester" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <asp:Label ID="lbl_User" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Request Time" >
            <ItemTemplate >
                <asp:Label ID="lbl_RequestTime" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Completed Time" >
            <ItemTemplate >
                <asp:Label ID="lbl_CompletedTime" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Macro<br>Enabled" >
            <ItemTemplate >
                <asp:Label ID="lbl_MacroEnabled" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Journal No" ItemStyle-HorizontalAlign="Left" Visible="false">
            <ItemTemplate >
                <asp:Label ID="lbl_JournalNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>   
<script type="text/javascript">
    updateControls();
</script>
</asp:Content>
