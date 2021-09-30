<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UTDebitNote.aspx.cs" Theme="DefaultTheme" Inherits="com.next.isam.webapp.account.UTDebitNote" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">UT Debit / Credit Note</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
    function isValid() {
        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_DebitNoteDate_txt_DebitNoteDate_textbox").value == "") {
            alert("Please enter debit note date.");
            return false;
        }
        return true;
    }
     
</script>
<br />
<table width="600px">
    <tr>
        <td class="FieldLabel2">Office</td>
        <td><cc1:SmartDropDownList ID="ddl_Office" runat="server" Width="147" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2">Fiscal Period</td>
        <td>Year&nbsp;&nbsp;<asp:DropDownList ID="ddl_Year" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
            Period&nbsp;&nbsp;<asp:DropDownList ID="ddl_Period" runat="server">
                <asp:ListItem Text ="1" Value="1" />
                <asp:ListItem Text="2" Value ="2" />
                <asp:ListItem Text="3" Value ="3" />
                <asp:ListItem Text="4" Value ="4" />
                <asp:ListItem Text="5" Value ="5" />
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
    <tr>
        <td class="FieldLabel2">Debit Note Date</td>
        <td><cc1:SmartCalendar ID="txt_DebitNoteDate" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2">Document Type</td>
        <td>
            <asp:RadioButtonList ID="rad_DocType" runat="server"  RepeatDirection ="Horizontal" >
                <asp:ListItem Text="Draft" Value="0" Selected="True" />
                <asp:ListItem Text="Official" Value="1" />
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2"><asp:Button ID="btn_Submit" runat="server" Text="Submit" OnClientClick="return isValid();"
                onclick="btn_Submit_Click" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="LButton"
                OnClientClick="return isValid();" onclick="btn_Export_Click" />
        </td>
    </tr>
</table>
</asp:Content>
