<%@ Page Title="Send Third-Party Customer D/N to UK" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true"
    CodeBehind="SendThirdPartyCustomerDNToUK.aspx.cs" Inherits="com.next.isam.webapp.account.SendThirdPartyCustomerDNToUK" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Send Third-Party Customer D/N to UK</asp:Panel>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        function Confirm() {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
            if (confirm("Are you sure to proceed?")) {
                confirm_value.value = "Yes";
            } else {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }
    </script>

    <table width="600px">
        <tr>
            <td class="FieldLabel2">Fiscal Period</td>
            <td>Year&nbsp;&nbsp;<asp:DropDownList ID="ddl_Year" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
            Period&nbsp;&nbsp;<asp:DropDownList ID="ddl_Period" runat="server">
                <asp:ListItem Text="1" Value="1" />
                <asp:ListItem Text="2" Value="2" />
                <asp:ListItem Text="3" Value="3" />
                <asp:ListItem Text="4" Value="4" />
                <asp:ListItem Text="5" Value="5" />
                <asp:ListItem Text="6" Value="6" />
                <asp:ListItem Text="7" Value="7" />
                <asp:ListItem Text="8" Value="8" />
                <asp:ListItem Text="9" Value="9" />
                <asp:ListItem Text="10" Value="10" />
                <asp:ListItem Text="11" Value="11" />
                <asp:ListItem Text="12" Value="12" />
            </asp:DropDownList>
            </td>
        </tr>
    </table>
    <br />
    <asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="btn_Search_Click" />&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_Send" runat="server" Text="Send" OnClick="btn_Send_Click" OnClientClick="Confirm()" Visible="false" />
    <br />
    <br />
    <asp:GridView ID="gv_sales" runat="server" AllowPaging="True" PageSize="20" OnPageIndexChanging="gv_sales_PageIndexChanging">
        <PagerSettings Mode="Numeric" Position="TopAndBottom" />
        <PagerStyle Font-Bold="true" HorizontalAlign="Right" />
        <Columns>

            <asp:TemplateField HeaderText="Contract No" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container, "DataItem.ContractNo")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Delivery No" ControlStyle-Width="120px" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container, "DataItem.DeliveryNo")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Invoice No" ControlStyle-Width="120px" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container, "DataItem.InvoiceNo")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Invoice Date" ControlStyle-Width="120px" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container, "DataItem.InvoiceDate", "{0:dd/MM/yyyy}")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Currency" ControlStyle-Width="120px" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container, "DataItem.Currency")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Invoice Amount" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container, "DataItem.InvoiceAmt")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Commission Amount" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container, "DataItem.CommissionAmt")%>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </asp:GridView>
    <br />

</asp:Content>
