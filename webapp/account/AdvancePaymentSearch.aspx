<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="AdvancePaymentSearch.aspx.cs" Inherits="com.next.isam.webapp.account.AdvancePaymentSearch" Title="Advance Payment Search" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">Advance Payment Search</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- popBox includes -->
<script src="../includes/popBox1.3.0.min.js" type="text/javascript"></script>
<link href="../includes/popBox1.3.0.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">
    $(document).ready(function () {

    });

</script>
<table>
    <tr>
        <td style="width: 120px">&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:120px;">Office</td>
        <td style="width: 319px"><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="200px"></cc2:smartdropdownlist></td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width: 120px">Payment No.</td>
        <td style="width: 319px">                            
            <asp:TextBox ID="txt_PaymentNo" runat="server"></asp:TextBox>
        </td>   
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:120px;">Payment Date</td>
        <td style="width: 319px">
            <cc2:SmartCalendar ID="txtPaymentDateFrom" FromDateControl="txtPaymentDateFrom" ToDateControl="txtPaymentDateTo" runat="server" />
            &nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txtPaymentDateTo" FromDateControl="txtPaymentDateFrom" ToDateControl="txtPaymentDateTo" runat="server" />  
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width: 120px">Vendor</td>
        <td style="width: 319px">                            
            <uc1:uclsmartselection id="uclVendor" runat="server"></uc1:uclsmartselection>
        </td>   
    </tr>
    <tr>
        <td class="FieldLabel2" style="width: 120px">L/C Bill Ref No.</td>
        <td style="width: 319px">                            
            <asp:TextBox ID="txtLCBillRefNo" runat="server"></asp:TextBox>
        </td>   
    </tr>
    <tr>
        <td class="FieldLabel2" style="width: 120px">Contract No</td>
        <td style="width: 319px">                            
            <asp:TextBox ID="txtContractNo" runat="server"></asp:TextBox>
        </td>   
    </tr>
    <tr>
        <td class="FieldLabel2" style="width: 120px">Payment Status</td>
        <td style="width: 319px">                            
            <asp:DropDownList runat="server" ID="ddlStatus"/></asp:TextBox>
        </td>   
    </tr>
</table>
<table>
    <tr>
        <td >
            <asp:Button ID="btn_Search" runat="server" Text="Search" onclick="btn_Search_Click" SkinID="MButton"/>
            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnReset" runat="server" Text="Reset" onclick="btnReset_Click" SkinID="MButton" />
            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_NewInstalment" runat="server" Text="New [By Instalment]" onclick="btn_NewInstalment_Click" SkinID="XLButton" />
                
        </td>
    </tr>
</table>
<br /><br />
<asp:GridView ID="gv_Request" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_Request_RowDataBound" 
        onrowcommand="gv_Request_RowCommand" AllowSorting="true" 
        onsorting="gv_Request_Sorting">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton ID="lnk_Edit" runat="server" CausesValidation="false" PostBackUrl="" ToolTip="Edit"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Adv. Payment No" SortExpression="PaymentNo" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_PaymentNo" runat="server"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Payment Type" ControlStyle-Width="180px">
            <ItemTemplate>
                <asp:Label ID="lbl_PaymentType" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Office">
            <ItemTemplate>
                <asp:Label ID="lbl_Office" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Vendor" SortExpression="Vendor" HeaderStyle-ForeColor="DarkBlue" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <asp:Label ID="lbl_Vendor" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Currency" SortExpression="Curreny" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_Currency" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Total Payment Amt" ControlStyle-Width="100px" SortExpression="PaymentAmt" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_TotalAmt" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Payment Date" SortExpression="PaymentDate" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_PaymentDate" runat="server"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Created By" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <asp:Label ID="lbl_CreatedBy" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Variance">
            <ItemTemplate>
                <asp:Label ID="lbl_Variance" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Balance">
            <ItemTemplate>
                <asp:Label ID="lbl_Balance" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Balance (No Recovery Plan)">
            <ItemTemplate>
                <asp:Label ID="lbl_NoRecoveryPlanBalance" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Coming Deduction By Instalment">
            <ItemTemplate>
                <asp:Label ID="lbl_ComingDeduction" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton ID="lnk_Delete" runat="server" CausesValidation="false" PostBackUrl="" ToolTip="Delete"/>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>No result found.</EmptyDataTemplate>
</asp:GridView>   

</asp:Content>
