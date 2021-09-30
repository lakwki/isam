<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SmartCalendarRange.aspx.cs" Inherits="com.next.common.web.SmartCalendarRange" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Calendar Selector</title>
    <!--@OutputCache Duration="600" VaryByParam="target;targetTo;date;dateTo;__EVENTARGUMENT;__EVENTTARGET;cobMonth;cobYear;cobMonthTo;cobYearTo;myCal;myCalTo"-->
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <script lang="javascript">
        function setDateRange(target, dt, targetTo, dtTo) {
            if (opener && opener.document && opener.document.all[target]) {
                opener.document.all[target].value = dt;
            }

            if (opener && opener.document && opener.document.all[targetTo]) {
                opener.document.all[targetTo].value = dtTo;
            }

            var lblInterestDays = window.opener.document.getElementById("lblInterestDays");
            var hidInterestDays = window.opener.document.getElementById("hidInterestDays");
            if (lblInterestDays != null) {
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

                    var e = window.opener.document.getElementById("ddlDocType");
                    var isDebit = e.options[e.selectedIndex].value == 0 ? true : false;
                    var txtAdvanceRemainingTotal = window.opener.document.getElementById("txtAdvanceRemainingTotal").value.replace(/,(?=.*\.\d+)/g, '');
                    var txtInterestRate = window.opener.document.getElementById("txtInterestRate");
                    var lblInterestCharges = window.opener.document.getElementById("lblInterestCharges");
                    var charges = txtAdvanceRemainingTotal * (diffDays / 365) * (txtInterestRate.value / 100);
                    if (isDebit)
                        lblInterestCharges.innerText = charges.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    else
                        lblInterestCharges.innerText = (charges * -1).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                }
            }

            window.close();
        }

        function windowOnBlur() {
            //window.focus();
        }

        window.onblur = windowOnBlur;
    </script>
    <style>
        A {
            COLOR: teal;
            TEXT-DECORATION: none;
        }
    </style>
</head>
<body>
    <form id="myCalendar" runat="server">
        <asp:DropDownList ID="cobMonth"
            Style="z-index: 100; left: 35px; position: absolute; top: 5px; right: 829px;" runat="server"
            Font-Size="X-Small" AutoPostBack="True"
            OnSelectedIndexChanged="cobMonth_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:DropDownList ID="cobYear"
            Style="z-index: 101; left: 122px; position: absolute; top: 5px" runat="server"
            Font-Size="X-Small" AutoPostBack="True"
            OnSelectedIndexChanged="cobYear_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:Calendar ID="myCal"
            Style="z-index: 2; left: 2px; position: absolute; top: 2px" runat="server"
            Font-Size="Small" Font-Names="Arial" BorderColor="SteelBlue" PrevMonthText="3"
            NextMonthText="4" Width="214px" Height="181px" OnDayRender="myCal_DayRender"
            OnSelectionChanged="myCal_SelectionChanged"
            OnVisibleMonthChanged="myCal_VisibleMonthChanged">
            <TodayDayStyle BackColor="#DEEBF7"></TodayDayStyle>
            <DayStyle Font-Size="Small" BorderWidth="1px" BorderStyle="Solid" BorderColor="SteelBlue"
                BackColor="White"></DayStyle>
            <NextPrevStyle Font-Names="Webdings" Font-Bold="True" ForeColor="White"></NextPrevStyle>
            <DayHeaderStyle Font-Bold="True" ForeColor="SteelBlue" BackColor="#E0E0E0"></DayHeaderStyle>
            <SelectedDayStyle Font-Bold="True"></SelectedDayStyle>
            <TitleStyle Font-Bold="True" ForeColor="SteelBlue" BackColor="SteelBlue"></TitleStyle>
            <WeekendDayStyle ForeColor="SteelBlue"></WeekendDayStyle>
            <OtherMonthDayStyle ForeColor="Silver"></OtherMonthDayStyle>
        </asp:Calendar>
        </asp:label>
    <asp:Label ID="Label2" Style="z-index: 1; left: 128px; position: absolute; top: 168px" runat="server"
        Visible="False">Label
    </asp:Label>
        <asp:Label ID="Label1"
            Style="z-index: 101; left: 218px; position: absolute; top: 77px" runat="server"
            Font-Size="XX-Large" Font-Names="Webdings" ForeColor="SteelBlue">4
        </asp:Label>
        <asp:DropDownList ID="cobMonthTo" Style="z-index: 106; left: 280px; position: absolute; top: 5px"
            runat="server" Font-Size="X-Small" AutoPostBack="True"
            OnSelectedIndexChanged="cobMonthTo_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:DropDownList ID="cobYearTo"
            Style="z-index: 107; left: 368px; position: absolute; top: 5px" runat="server"
            Font-Size="X-Small" AutoPostBack="True"
            OnSelectedIndexChanged="cobYearTo_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:Calendar ID="myCalTo"
            Style="z-index: 5; left: 250px; position: absolute; top: 2px" runat="server"
            Font-Size="Small" Font-Names="Arial" BorderColor="SteelBlue" PrevMonthText="3"
            NextMonthText="4" Width="214px" Height="181px" SelectionMode="None"
            OnDayRender="myCalTo_DayRender"
            OnVisibleMonthChanged="myCalTo_VisibleMonthChanged">
            <TodayDayStyle BackColor="#DEEBF7"></TodayDayStyle>
            <DayStyle Font-Size="Small" BorderWidth="1px" BorderStyle="Solid" BorderColor="SteelBlue"
                BackColor="White"></DayStyle>
            <NextPrevStyle Font-Names="Webdings" Font-Bold="True" ForeColor="White"></NextPrevStyle>
            <DayHeaderStyle Font-Bold="True" ForeColor="SteelBlue" BackColor="#E0E0E0"></DayHeaderStyle>
            <SelectedDayStyle Font-Bold="True"></SelectedDayStyle>
            <TitleStyle Font-Bold="True" ForeColor="SteelBlue" BackColor="SteelBlue"></TitleStyle>
            <WeekendDayStyle ForeColor="SteelBlue"></WeekendDayStyle>
            <OtherMonthDayStyle ForeColor="Silver"></OtherMonthDayStyle>
        </asp:Calendar>

    </form>
</body>
</html>
