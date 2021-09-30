<%@OutputCache Duration="600" VaryByParam="target;date;__EVENTARGUMENT;__EVENTTARGET;cobMonth;cobYear"%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SmartCalendar.aspx.cs" Inherits="com.next.common.web.SmartCalendar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Calendar Selector</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script lang="javascript">
		function setDate(target, dt){
			if (opener && opener.document && opener.document.all[target]){
				opener.document.all[target].value=dt;
				opener.document.all[target].focus();
				window.close();
			}
		}

		function windowOnBlur(){
			//window.focus();
		}

		window.onblur=windowOnBlur;
		</script>
</head>
<body>
    <form id="myCalendar" runat="server">
        <asp:dropdownlist id="cobMonth" 
            style="Z-INDEX: 100; LEFT: 35px; POSITION: absolute; TOP: 5px" runat="server"
			Font-Size="X-Small" AutoPostBack="True" 
            onselectedindexchanged="cobMonth_SelectedIndexChanged">
		</asp:dropdownlist>
		<asp:dropdownlist id="cobYear" 
            style="Z-INDEX: 101; LEFT: 122px; POSITION: absolute; TOP: 5px" runat="server"
			Font-Size="X-Small" AutoPostBack="True" 
            onselectedindexchanged="cobYear_SelectedIndexChanged">
		</asp:dropdownlist>
        <asp:calendar id="myCal" 
            style="Z-INDEX: 3; LEFT: 1px; POSITION: absolute; TOP: 1px" runat="server"
				Font-Size="Small" SelectionMode="None" Height="181px" Width="214px" 
            NextMonthText="4" PrevMonthText="3" BorderColor="SteelBlue" 
            Font-Names="Arial" ondayrender="myCal_DayRender" 
            onvisiblemonthchanged="myCal_VisibleMonthChanged">
				<TodayDayStyle BackColor="#DEEBF7"></TodayDayStyle>
				<DayStyle Font-Size="Small" BorderWidth="1px" BorderStyle="Solid" BorderColor="SteelBlue"
					BackColor="White"></DayStyle>
				<NextPrevStyle Font-Names="Webdings" Font-Bold="True" ForeColor="White"></NextPrevStyle>
				<DayHeaderStyle Font-Bold="True" ForeColor="SteelBlue" BackColor="#E0E0E0"></DayHeaderStyle>
				<SelectedDayStyle Font-Bold="True"></SelectedDayStyle>
				<TitleStyle Font-Bold="True" ForeColor="SteelBlue" BackColor="SteelBlue"></TitleStyle>
				<WeekendDayStyle ForeColor="SteelBlue"></WeekendDayStyle>
				<OtherMonthDayStyle ForeColor="Silver"></OtherMonthDayStyle>
		</asp:calendar>
        
    </form>
</body>
</html>
