<%@ Page Title="LG Hold Payment Report" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="LGHoldPaymentReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.LGHoldPaymentReport" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Accounts">LG Hold Payment Report</asp:Panel>
    <%--<script type="text/javascript" >
    function openVendorWindow(ntVendorId) {
        if (ntVendorId == 0)
            window.open('NonTradeVendor.aspx', 'NTInvoice', 'width=900,height=600, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();
        else
            window.open('NonTradeVendor.aspx?NTVendorId=' + ntVendorId, 'NTInvoice', 'width=900,height=600, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();

    }

</script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="800px" cellspacing="2" cellpadding="2">
        <tr>
            <td>
                <table id="tb_AdvanceSearch" runat="server" width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td class="FieldLabel4" style="width: 80px">Office</td>
                        <td style="width: 319px">
                            <cc2:SmartDropDownList ID="ddlOffice" runat="server"></cc2:SmartDropDownList></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel4" style="width: 80px">LG No.</td>
                        <td>
                            <asp:TextBox ID="txt_lgNo" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel4" style="width: 80px">Vendor</td>
                        <td style="width: 350px">
                            <uc1:UclSmartSelection ID="txt_Supplier" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel4" style="width: 80px">Item No.</td>
                        <td>
                            <asp:TextBox ID="txt_itemNo" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel4" style="width: 80px">Contract No.</td>
                        <td>
                            <asp:TextBox ID="txt_contractNo" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2" style="width: 80px">Payment Status</td>
                        <td style="width: 319px">
                            <asp:DropDownList runat="server" ID="ddlStatus" />
                        </td>
                    </tr>
                </table>

            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
    </table>
         <asp:Button ID="btn_Print" runat="server" Text="Print" OnClick="btn_Print_Click" />
</asp:Content>
