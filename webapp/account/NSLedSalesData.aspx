<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="NSLedSalesData.aspx.cs" Inherits="com.next.isam.webapp.account.NSLedSalesData" Title="NS - LED Sales Data" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">NS - LED Sales Data</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="height: 140px;">
        <tr>
            <td colspan="2">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            </td>
        </tr>
        <tr id="tr1">
            <td class="FieldLabel4" width="100">Office</td>
            <td>
                <cc2:SmartDropDownList  ID="ddl_Office" runat="server" width="70px" AutoPostBack="false" />
            </td>
        </tr>
        <tr id="trInvoiceNo">
            <td class="FieldLabel4">NUK Invoice No</td>
            <td>
                <asp:TextBox ID="txt_InvoiceNo" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr id="trScope">
            <td class="FieldLabel4">Scope</td>
            <td>
                <asp:RadioButton ID="rad_all" runat="server" AutoPostBack="True" Checked="True" GroupName="scope" Text="All" OnCheckedChanged="rad_all_CheckedChanged" />
            </td>
        </tr>
        <tr id="trScope2">
            <td></td>
            <td>
                <asp:RadioButton ID="rad_weekly" AutoPostBack="True" runat="server" GroupName="scope" Text="Weekly: " OnCheckedChanged="rad_weekly_CheckedChanged" />
                <cc2:SmartDropDownList ID="ddl_Year" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_Year_SelectedIndexChanged" Enabled="False"></cc2:SmartDropDownList>
                <cc2:SmartDropDownList ID="ddl_Week" runat="server" AutoPostBack="True" Enabled="False"></cc2:SmartDropDownList>
            </td>
        </tr>
        <tr id="trItemNo">
            <td class="FieldLabel4">Item No</td>
            <td>
                <asp:TextBox ID="txt_ItemNo" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr id="trCountryOfSale">
            <td class="FieldLabel4">Country Of Sale</td>
            <td>
                <cc2:SmartDropDownList ID="ddl_Country" runat="server" Width="150px" AutoPostBack="True"></cc2:SmartDropDownList>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="btn_Search_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_Reset" runat="server" Text="Reset" OnClick="btn_Reset_Click" />
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td colspan="2">
                <asp:GridView ID="gv_NSLedSales" runat="server" EnableModelValidation="True" OnPageIndexChanging="gv_NSLedSales_PageIndexChanging" PageSize="250" AllowPaging="True" Height="406px" OnRowDataBound="gv_NSLedSales_RowDataBound">
                    <PagerSettings Mode="Numeric" Position="TopAndBottom" />
                    <PagerStyle Font-Bold="true" HorizontalAlign="Right" />
                    <Columns>
                        <asp:TemplateField HeaderText="Item No." ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ItemNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Option<br>No." ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_OptionNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Size" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Size" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Description" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Description" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Despatch<br>Qty" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Despatch_Qty" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Despatch<br>Value" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Despatch_Value" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Returns<br>Qty" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Returns_Qty" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Returns<br>Value" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Returns_Value" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Net<br>Qty" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Net_Qty" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Country of Sale" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Country_Of_Sale" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Net<br>Value" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Net_Value" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="VAT %" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_VAT_Percent" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="VAT" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_VAT" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="NSVE" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_NSVE" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Commission" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Commission" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="USD<br>Ex Rate" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="lbl_USDExRate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>
