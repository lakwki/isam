<%@ Page Language="C#" Theme="DefaultTheme"  MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="DCNoteIndex.aspx.cs" Inherits="com.next.isam.webapp.account.DCNoteIndex" Title="Debit/Credit Note" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinId="sectionHeader_Accounts"></asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table>
    <tr>
        <td><span style="color:Green; font-weight:bold;">General</span></td>
    </tr>
    <tr runat="server" id="tr_Adjustment_Search">
        <td>-&nbsp;<a href="AdjustmentNoteSearch.aspx">Debit / Credit Note Search</a></td>
    </tr>
    <tr><td>&nbsp;</td></tr>
    <tr>
        <td><span style="color:Green; font-weight:bold;">Next Claims</span></td>
    </tr>
    <tr runat="server" id="tr_UKClaim">
        <td>-&nbsp;<a href="../claim/UKClaimSearch.aspx">Next Claim Portal</a></td>
    </tr>
    <tr runat="server" id="tr_COOApproval">
        <td>-&nbsp;<a href="../claim/COOApproval.aspx">Next Claim COO Approval</a></td>
    </tr>
    <tr runat="server" id="tr_UKClaimRecharge">
        <td>-&nbsp;<a href="../claim/RechargeSupplierDCNote.aspx">Next Claim Recharge Debit / Credit Note</a></td>
    </tr>
    <tr runat="server" id="tr_MFRNBatchUpload">
        <td>-&nbsp;<a href="../claim/MFRNBatchUpload.aspx">MFRN Batch Upload</a></td>
    </tr>
    <tr runat="server" id="tr_NextClaimUpload">
        <td>-&nbsp;<a href="../claim/NextClaimUpload.aspx">Next Claim Upload</a></td>
    </tr>
    <tr runat="server" id="tr_UKDiscountClaim">
        <td>-&nbsp;<a href="../claim/UKDiscountClaimSearch.aspx">UK Discount Claim Portal</a></td>
    </tr>
    <tr><td>&nbsp;</td></tr>
    <tr>
        <td><span style="color:Green; font-weight:bold;">Against Customers</span></td>
    </tr>
    <tr runat="server" id="tr_MockShop">
        <td>-&nbsp;<a href="MockShopSampleDebitNote.aspx">Mock Shop Sample Debit Note</a></td>
    </tr>
    <tr runat="server" id="tr_Studio">
        <td>-&nbsp;<a href="StudioSampleDebitNote.aspx">Studio Sample Debit Note</a></td>
    </tr>
    <tr runat="server" id="tr_ILSDiffDCNote">
        <td>-&nbsp;<a href="ILSDiffDCNote.aspx">ILS Price Difference Debit / Credit Note (Not Yet Complete)</a></td>
    </tr>
    <tr runat="server" id="tr_UT">
        <td>-&nbsp;<a href="UTDebitNote.aspx">UT Debit / Credit Note</a></td>
    </tr>
    <tr runat="server" id="tr_AR_Adjustment">
        <td>-&nbsp;<a href="ARAdjustment.aspx">Account Receivable Adjustment</a></td>
    </tr>
    <tr runat="server" id="tr_AR_OtherChargeDCNote">
        <td>-&nbsp;<a href="OtherChargesDebitNote.aspx">Other Charges Debit / Credit Note</a></td>
    </tr>
    <tr runat="server" id="tr_SendThirdPartyCustomerDNToUK">
        <td>-&nbsp;<a href="SendThirdPartyCustomerDNToUK.aspx">Send Third-Party Customer D/N to UK</a></td>
    </tr>
    <!--
    <tr runat="server" id="tr_Sales_Amendment">
        <td>-&nbsp;Sales Amount Amendment Debit / Credit Notes</td>        
    </tr>
    -->
    <tr><td>&nbsp;</td></tr>
    <tr>
        <td><span style="color:Green; font-weight:bold;">Against Suppliers</span></td>
    </tr>
    <tr runat="server" id="tr_AP_Adjustment">
        <td>-&nbsp;<a href="APAdjustment.aspx">Supplier Amount Amendment Debit / Credit Notes</a></td>
    </tr>
    <tr runat="server" id="tr_QA">
        <td>-&nbsp;<a href="QADebitNote.aspx" >QA Commission Debit Note</a></td>
    </tr>
    <tr runat="server" id="tr_AP_OtherChargeDCNote">
        <td>-&nbsp;<a href="OtherChargesDebitNote.aspx">Other Charges Debit / Credit Note</a></td>
    </tr>
</table>
</asp:Content>
