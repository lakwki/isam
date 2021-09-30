<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="LCStatusReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.LCStatusReport" Title="LC Status Report"%>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel2" runat="server" SkinID="sectionHeader_Report">L/C Status Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<script type="text/javascript">

     function copyLCBatchNo()
     {        
         document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoFrom").value;        
     }  
     
     function copyLCNo()
     {        
         document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoFrom").value;        
     }  

     
     function isFormValid()
     {
        if (
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoFrom").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoTo").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBillRefNoFrom").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBillRefNoTo").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoFrom").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoTo").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateFrom_txt_LCIssueDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateTo_txt_LCIssueDateTo_textbox").value == "" &&  
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCExpiryDateFrom_txt_LCExpiryDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCExpiryDateTo_txt_LCExpiryDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCPaymentCheckDateFrom_txt_LCPaymentCheckDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCPaymentCheckDateTo_txt_LCPaymentCheckDateTo_textbox").value == "" 
            )
            {
                alert("Please enter search criteria on one of below search criteria.\r\n"
                + " - LC No.\r\n"
                + " - LC Bill Ref. No.\r\n"
                + " - LC Batch No.\r\n"
                + " - LC Batch Date.\r\n" 
                + " - LC Issue Date.\r\n"
                + " - LC Expiry Date.\r\n"
                + " - LC Payment Check Date.\r\n"
                );            
                return false;
            }

            
            if ( ((document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoFrom").value != "" ) 
                 && !isBatchNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoFrom").value)) 
               || ((document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoTo").value != "" )
                 && !isBatchNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoTo").value))) 
            {
                alert("Invalid LC Batch no.");
                return false;
            }


        if (  
              ( document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateFrom_txt_LCIssueDateFrom_textbox").value == "" 
                && document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateTo_txt_LCIssueDateTo_textbox").value != "" )
            ||( document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateFrom_txt_LCIssueDateFrom_textbox").value != "" 
                && document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateTo_txt_LCIssueDateTo_textbox").value == "" )
            ||(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCExpiryDateFrom_txt_LCExpiryDateFrom_textbox").value == "" 
                && document.getElementById("ctl00_ContentPlaceHolder1_txt_LCExpiryDateTo_txt_LCExpiryDateTo_textbox").value != "" )
            ||(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCExpiryDateFrom_txt_LCExpiryDateFrom_textbox").value != "" 
                && document.getElementById("ctl00_ContentPlaceHolder1_txt_LCExpiryDateTo_txt_LCExpiryDateTo_textbox").value == "" )
            ||(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCPaymentCheckDateFrom_txt_LCPaymentCheckDateFrom_textbox").value == "" 
                && document.getElementById("ctl00_ContentPlaceHolder1_txt_LCPaymentCheckDateTo_txt_LCPaymentCheckDateTo_textbox").value != "") 
            ||(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCPaymentCheckDateFrom_txt_LCPaymentCheckDateFrom_textbox").value != "" 
                && document.getElementById("ctl00_ContentPlaceHolder1_txt_LCPaymentCheckDateTo_txt_LCPaymentCheckDateTo_textbox").value == "" )
           )  
        {
           alert ("Please enter date range in pair.");
           return false;
        }
           

        return true;
     }

</script>

<table >
<!--
    <tr>
        <td colspan="2" class="tableHeader">LC Status Report</td>
    </tr>
-->
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel4">LC No.</td>
        <td>
            <asp:TextBox ID="txt_LCNoFrom" runat='server' value='' /> 
            &nbsp;To&nbsp;
            <asp:TextBox ID="txt_LCNoTo" runat="server" value='' />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">LC Bill Ref. No.</td>
        <td>
            <asp:TextBox ID="txt_LCBillRefNoFrom" runat='server' value='' /> 
            &nbsp;To&nbsp;
            <asp:TextBox ID="txt_LCBillRefNoTo" runat="server" value='' />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">LC Batch No.</td>
        <td>
            <asp:TextBox ID="txt_LCBatchNoFrom" runat='server' value=''  /> 
            &nbsp;To&nbsp;
            <asp:TextBox ID="txt_LCBatchNoTo" runat="server" value=''/>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">LC Issue Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_LCIssueDateFrom" runat="server" FromDateControl="txt_LCIssueDateFrom" ToDateControl="txt_LCIssueDateTo" />
            &nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_LCIssueDateTo" runat="server" FromDateControl="txt_LCIssueDateFrom" ToDateControl="txt_LCIssueDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">LC Expiry Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_LCExpiryDateFrom" runat="server" FromDateControl="txt_LCExpiryDateFrom" ToDateControl="txt_LCExpiryDateTo" />
            &nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_LCExpiryDateTo" runat="server" FromDateControl="txt_LCExpiryDateFrom" ToDateControl="txt_LCExpiryDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:135px;">LC Payment Check Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_LCPaymentCheckDateFrom" runat="server" FromDateControl="txt_LCPaymentCheckDateFrom" ToDateControl="txt_LCPaymentCheckDateTo" />
            &nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_LCPaymentCheckDateTo" runat="server" FromDateControl="txt_LCPaymentCheckDateTo" ToDateControl="txt_LCPaymentCheckDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Supplier</td>
        <td><uc1:uclsmartselection id="uclVendor" runat="server"></uc1:uclsmartselection></td>
    </tr>

    <tr>
        <td class="FieldLabel4">GB Test Result</td>
        <td><cc2:SmartDropDownList ID="ddl_GBTestResult" runat ="server" /></td>
    </tr>

    <tr>
        <td class="FieldLabel4">Country Of Origin</td>
        <td><cc2:SmartDropDownList ID="ddl_CO" runat ="server" /></td>
    </tr>

    <tr>
        <td class="FieldLabel4">Office Group</td>
        <td><cc2:SmartDropDownList  ID="ddl_OfficeGroup" runat="server" width="70px" AutoPostBack="True" onSelectedIndexChanged='ddl_OfficeGroup_SelectedIndexChanged'/></td>
    </tr>
    <tr id="tr_HandlingOffice" runat="server" style="display:none;">
        <td class="FieldLabel4">Handling Office</td>
        <td><cc2:SmartDropDownList  ID="ddl_HandlingOffice" runat="server" width="70px" AutoPostBack="false" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Product Team</td>
        <td><cc2:SmartDropDownList  ID="ddl_ProductTeam" runat="server" Width="350px" /></td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Preview" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Preview_Click" OnClientClick="return isFormValid();" />
            <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
        </td>
    </tr>

</table>
</asp:Content>
