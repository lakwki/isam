<%@ Page Title="Non-Trade Expense - Vendor" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme"  AutoEventWireup="true" CodeBehind="NonTradeVendorSearch.aspx.cs" Inherits="com.next.isam.webapp.nontrade.NonTradeVendorSearch" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Accounts">Non-Trade Expense Vendor Search Engine</asp:Panel>
<script type="text/javascript" >
    function openVendorWindow(ntVendorId) {
        if (ntVendorId == 0)
            window.open('NonTradeVendor.aspx', 'NTInvoice', 'width=900,height=600, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();
        else
            window.open('NonTradeVendor.aspx?NTVendorId=' + ntVendorId, 'NTInvoice', 'width=900,height=600, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();

    }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="800px" cellspacing="2" cellpadding="2">        
        <tr>
            <td>
                <table id="tb_AdvanceSearch" runat="server" width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="5">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2" style="width:100px;">&nbsp;Office&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>
                            <cc2:SmartDropDownList ID="ddl_Office" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_Office_SelectedIndexChange" />
                        </td>
                        <td style="width:1px;">&nbsp;</td>
                        <td class="FieldLabel2" style="width:100px;">&nbsp;Expense Type</td>
                        <td>&nbsp;</td>
                        <td>
                            <cc2:SmartDropDownList ID="ddl_ExpenseType" runat="server" />
                        </td>
                    </tr>                                        
                    <tr>
                        <td class="FieldLabel2">&nbsp;Vendor</td>
                        <td>&nbsp;</td>
                        <td>
                            <uc1:UclSmartSelection ID="txt_SupplierName" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                        <td class="FieldLabel2">&nbsp;Status</td>
                        <td>&nbsp;</td>
                        <td><cc2:SmartDropDownList ID="ddl_Status" runat="server" /></td>
                    </tr>                                 
                    <tr>
                        <td>&nbsp;</td>
                    </tr>       
                    <tr>
                        <td colspan="5">
                            <asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="Search_OnClick"  />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btn_Reset" runat="server" Text="Reset" OnClick="Reset_OnClick"  />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btn_Print" runat="server" Text="Print" OnClick="btn_Print_Click" />&nbsp;&nbsp;&nbsp;
                            <!--<asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="MButton" OnClick="btn_Export_Click" />&nbsp;&nbsp;&nbsp;-->
                            <asp:Button ID="btn_New" runat="server" Text="New" OnClientClick="openVendorWindow(0); return false;" />
                        </td>
                    </tr>
                </table>
            
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
            <asp:Panel ID="pnl_Result" runat="server" Visible="false" >
            <table style="border: 1px solid #B0B0B0;" cellspacing="0" cellpadding="0">
                    <tr>    
                        <td >
                            <asp:GridView ID="gv_NTVendorList" runat="server" >
                                <Columns >
                                    <asp:TemplateField HeaderText="Vendor Name"  ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Left" >
                                        <ItemTemplate >
                                        <a href="#" onclick='openVendorWindow(<%# DataBinder.Eval(Container, "DataItem.NTVendorId")%>); return false;'><%# DataBinder.Eval(Container, "DataItem.VendorName")%></a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Address" ItemStyle-Width="300px"  ItemStyle-HorizontalAlign="Left" >
                                        <ItemTemplate>
                                            <%# DataBinder.Eval(Container, "DataItem.Address")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Country" ItemStyle-Width="100px">
                                        <ItemTemplate >
                                            <%# DataBinder.Eval(Container, "DataItem.Country.Name")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Telephone No." ItemStyle-Width="100px">
                                        <ItemTemplate >
                                        <%# DataBinder.Eval(Container, "DataItem.Telephone")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fax No." ItemStyle-Width="100px">
                                        <ItemTemplate >
                                            <%# DataBinder.Eval(Container, "DataItem.Fax")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px">
                                        <ItemTemplate >
                                            <%# DataBinder.Eval(Container, "DataItem.WorkflowStatus.Name")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found.
                                </EmptyDataTemplate>
                            </asp:GridView>                            
                        </td>              
                    </tr>      
                </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
