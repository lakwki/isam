<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UKClaimReview.aspx.cs" Inherits="com.next.isam.webapp.claim.UKClaimReview" Title="Next Claim Review" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Next Claim - Review</asp:Panel>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<table>
    <tr>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td></td>
        <td>
            <div id="divMessage" runat="server" style ="display:none;">
                <asp:Label ID="lblMessage" runat="server" style="Font-Size:large">Next Claim Debit Note Cannot Be Found.</asp:Label>
            </div>
        </td>
        </tr>
</table>

</asp:Content>

