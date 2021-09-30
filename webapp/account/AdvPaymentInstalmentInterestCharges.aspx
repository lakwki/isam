<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="AdvPaymentInstalmentInterestCharges.aspx.cs" Inherits="com.next.isam.webapp.account.AdvPaymentInstalmentInterestCharges" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>
<script src="../common/common.js" type="text/javascript"></script>
<script src="../common/jquery-1.7.min.js" type="text/javascript"></script>
<script src="../webservices/UclSmartSelection.js" type="text/javascript"></script>
<script type="text/javascript">

    function chargesClose(dcNoteNo) {
        if (window.opener != null && !window.opener.closed) {
            var i = window.opener.location['href'].indexOf('&dcNo');
            var s = window.opener.location['href'];
            if (i != -1) {
                s = window.opener.location['href'].substring(0, i);
            }
            window.opener.location = s + "&dcNo=" + dcNoteNo;
            window.close();
        }
    }

    function Confirm() {
        var dcNoteDate = document.getElementById('txtDebitNoteDate_txtDebitNoteDate_textbox').value;
        var fromDate = document.getElementById('txtPaymentDateFrom_txtPaymentDateFrom_textbox').value;
        var toDate = document.getElementById('txtPaymentDateTo_txtPaymentDateTo_textbox').value;
        var remainingTotal = document.getElementById('txtAdvanceRemainingTotal').value;
        var rate = document.getElementById('txtInterestRate').value;

        if (dcNoteDate != '' && fromDate != '' && toDate != '' && remainingTotal != '' && rate != '') {
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
        else {
            if (dcNoteDate == '') {
                alert('Please input DN/CN Date');
            }
            else if (fromDate == '') {
                alert('Please input Covering Period From Date');
            }
            else if (toDate == '') {
                alert('Please input Covering Period To Date');
            }
            else if (remainingTotal == '') {
                alert('Please input Advance Remaining Total Amount');
            }
            else if (rate == '') {
                alert('Please input Interest Rate');
            }
            return;
        }
    }

    function interestDaysChange() {
        var dt = document.getElementById("txtPaymentDateFrom_txtPaymentDateFrom_textbox").value;
        var dtTo = document.getElementById("txtPaymentDateTo_txtPaymentDateTo_textbox").value;
        if (dt != '' && dtTo != '') {
            var lblInterestDays = document.getElementById("lblInterestDays");
            var hidInterestDays = document.getElementById("hidInterestDays");
            var date1, date2;
            var mdy1, mdy2;
            var mdy1 = dt.split('/');
            var mdy2 = dtTo.split('/');
            date1 = new Date(mdy1[2], mdy1[1] - 1, mdy1[0]);
            date2 = new Date(mdy2[2], mdy2[1] - 1, mdy2[0]);

            var oneDay = 24 * 60 * 60 * 1000;

            var diffDays = ((date2 - date1) / oneDay) + 1;

            if (diffDays >= 0) {
                lblInterestDays.innerHTML = diffDays;
                hidInterestDays.value = diffDays;

                var e = document.getElementById("ddlDocType");
                var isDebit = e.options[e.selectedIndex].value == 0 ? true : false;
                var txtAdvanceRemainingTotal = document.getElementById("txtAdvanceRemainingTotal").value.replace(/,(?=.*\.\d+)/g, '');
                var txtInterestRate = document.getElementById("txtInterestRate");
                var lblInterestCharges = document.getElementById("lblInterestCharges");
                var charges = txtAdvanceRemainingTotal * (diffDays / 365) * (txtInterestRate.value / 100);
                if (isDebit)
                    lblInterestCharges.innerText = charges.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                else
                    lblInterestCharges.innerText = (charges * -1).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");

            }
        }
    }

    function isNumberKey(evt, obj) {

        var charCode = (evt.which) ? evt.which : event.keyCode
        var value = obj.value;
        var dotcontains = value.indexOf(".") != -1;
        if (dotcontains)
            if (charCode == 46) return false;
        if (charCode == 46) return true;
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }

</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet" />
    <title>Advance Payment Instalment Interest Charges</title>
    <style type="text/css">
        #p01 {
            color: #005AB5;
            font-size: medium;
        }

        .beta table, .beta th, .beta td {
            border: 1px solid black;
        }

        .gridHeaderNew {
            font-size: x-small;
            font-weight: bold;
        }

        .FieldLabel2 {
            Width: 140px;
        }

        .auto-style1 {
            height: 30px;
        }

        .auto-style2 {
            width: 319px;
            height: 30px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1 id="p01">Advance Payment Instalment Interest Charges</h1>
        <table>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblPN" Text="Advance Payment No" class="FieldLabel2" />
                </td>
                <td>
                    <asp:Label runat="server" ID="lblPaymentNo"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblOffice" Text="Office" class="FieldLabel2" />
                </td>
                <td>
                    <asp:Label runat="server" ID="lblOfficeName"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblVendor" Text="Supplier" class="FieldLabel2" />
                </td>
                <td>
                    <asp:Label runat="server" ID="lblSupplier"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblDocType" Text="Doc Type" class="FieldLabel2" />
                </td>
                <td style="width: 400px">
                    <asp:DropDownList ID="ddlDocType" AutoPostBack="True" AppendDataBoundItems="true" runat="server" Width="200px" OnSelectedIndexChanged="txtInterestRate_TextChanged">
                        <asp:ListItem Text="Debit Note" Value="0" />
                        <asp:ListItem Text="Credit Note" Value="1" />
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel2">Issue DN/CN Date</td>
                <td>
                    <cc2:SmartCalendar ID="txtDebitNoteDate" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="auto-style1">
                    <asp:Label runat="server" ID="lblCoveringPeriod" Text="Covering Period" class="FieldLabel2" /></td>
                <td class="auto-style2">
                    <cc2:SmartCalendar ID="txtPaymentDateFrom" FromDateControl="txtPaymentDateFrom" ToDateControl="txtPaymentDateTo" runat="server" />
                    &nbsp;To&nbsp;
                    <cc2:SmartCalendar ID="txtPaymentDateTo" FromDateControl="txtPaymentDateFrom" ToDateControl="txtPaymentDateTo" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblIntDays" Text="Interest Days" class="FieldLabel2" />
                </td>
                <td>
                    <asp:Label runat="server" ID="lblInterestDays" Text="0"></asp:Label>
                    &nbsp;
                    <asp:Label runat="server" ID="lblDays">Day(s)</asp:Label>
                    <asp:HiddenField ID="hidInterestDays" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblAdvRemTotal" Text="Advance Remaining Total" class="FieldLabel2" />
                </td>
                <td>
                    <asp:Label runat="server" ID="lblCurrency"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:TextBox runat="server" ID="txtAdvanceRemainingTotal" onkeypress="return isNumberKey(event,this)" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblInterestRate" Text="Interest Rate %" class="FieldLabel2" /></td>
                <td>
                    <asp:TextBox runat="server" ID="txtInterestRate" onkeypress="return isNumberKey(event,this)" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblIntChar" Text="Interest Charges" class="FieldLabel2" /></td>
                <td>
                    <asp:Label runat="server" ID="lblCurrency2"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Label runat="server" ID="lblInterestCharges" Text="0.00"></asp:Label>
                </td>
            </tr>

        </table>
        <br />
        <br />
        <asp:Button runat="server" ID="btnConfirm" Text="Issue DN/CN" OnClick="btnConfirm_Click" OnClientClick="Confirm()" SkinID="XLButton" />
    </form>
</body>
<script type="text/javascript">

    $("INPUT").blur(function () {
        interestDaysChange();
    });

</script>
</html>
