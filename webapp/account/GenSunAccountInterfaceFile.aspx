<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="GenSunAccountInterfaceFile.aspx.cs" Inherits="com.next.isam.webapp.account.GenSunAccountInterfaceFile" Title="Generate SunAccount Interface File" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--    
    <img src="../images/banner_account_sun.gif" runat="server" id="imgHeaderText" />
    <img src="../images/banner_workplace.gif" runat="server" id="img1" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Epicor Interface File Generation</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" language="javascript">
    function updateControl() {
        if ((parseInt(document.all.<%= ddl_Office.ClientID %>.value) == 3 || parseInt(document.all.<%= ddl_Office.ClientID %>.value) == 14) && document.all.<%= hidSuper.ClientID %>.value == 'Y')
        {
            document.all.<%= chkMacro.ClientID %>.disabled = false;
        }
        else
        {
            document.all.<%= chkMacro.ClientID %>.disabled = false; 
            //document.all.<%= chkMacro.ClientID %>.checked = false;
        }
    }
</script>
<table width="800px">
    <!---
    <tr>
        <td></td>
        <td>
            <asp:HyperLink Text=">> Consolidated Interface" runat="server" ID="lnkConsolidatedInterface" NavigateUrl="GenConsolidatedSunInterfaceFile.aspx"></asp:HyperLink>
        </td>
    </tr>
    --->
    <tr>
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:120px;">Office Group</td>
        <td><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="250px"></cc2:smartdropdownlist>
    </td>
    </tr>
    <tr>
        <td class="FieldLabel2">File Type</td>
        <td><cc2:smartdropdownlist id="ddl_SunInterfaceType" runat="server" Width="250px" 
                AutoPostBack="True" 
                onselectedindexchanged="ddl_SunInterfaceType_SelectedIndexChanged"></cc2:smartdropdownlist>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">Phase</td>
        <td><cc2:smartdropdownlist id="ddl_Phase" runat="server" Width="250px" 
                AutoPostBack="True" onselectedindexchanged="ddl_Phase_SelectedIndexChanged"></cc2:smartdropdownlist></td>
    </tr>
    <tr runat="server" id="trPurchaseTerm">
        <td class="FieldLabel2">Purchase Term</td>
        <td><cc2:smartdropdownlist id="ddl_PurchaseTerm" runat="server" Width="250px">
                    <asp:ListItem Text="ALL" Selected="True" Value="0" />
                    <asp:ListItem Text="FOB" Value="1" />
                    <asp:ListItem Text="VM" Value="2" />
            </cc2:smartdropdownlist>
        </td>
    </tr>
    <tr runat="server" id="trUTurn">
        <td class="FieldLabel2">U-Turn</td>
        <td><cc2:smartdropdownlist id="ddl_UTurn" runat="server" Width="250px">
                    <asp:ListItem Text="ALL" Selected="True" Value="0" />
                    <asp:ListItem Text="U-Turn" Value="1" />
                    <asp:ListItem Text="Non U-Turn" Value="2" />
            </cc2:smartdropdownlist>
        </td>
    </tr>
    <tr runat="server" id="trFiscalYear">
        <td class="FieldLabel2">Fiscal Year</td>
        <td>
            <cc2:smartdropdownlist id="ddl_Year" runat="server" Width="250px"></cc2:smartdropdownlist>
        </td>
    </tr>
    <tr runat="server" id="trPeriod">
        <td class="FieldLabel2">Period</td>
        <td>
            <cc2:smartdropdownlist id="ddl_Period" runat="server" Width="250px">
                <asp:ListItem Text="Period 1" Value="1" Selected="True" />
                <asp:ListItem Text="Period 2" Value="2" />
                <asp:ListItem Text="Period 3" Value="3" />
                <asp:ListItem Text="Period 4" Value="4" />
                <asp:ListItem Text="Period 5" Value="5" />
                <asp:ListItem Text="Period 6" Value="6" />
                <asp:ListItem Text="Period 7" Value="7" />
                <asp:ListItem Text="Period 8" Value="8" />
                <asp:ListItem Text="Period 9" Value="9" />
                <asp:ListItem Text="Period 10" Value="10" />
                <asp:ListItem Text="Period 11" Value="11" />
                <asp:ListItem Text="Period 12" Value="12" />
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
        <asp:TemplateField HeaderText="Purchase<br>Term" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <asp:Label ID="lbl_PurchaseTerm" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="U-Turn" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <asp:Label ID="lbl_UTurn" runat="server" />
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
    </Columns>
</asp:GridView>   

        <!---
        <asp:TemplateField HeaderText="Journal No" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate >
                <asp:Label ID="lbl_JournalNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        --->

<script type="text/javascript" language="javascript">
    updateControl();
</script>
</asp:Content>
