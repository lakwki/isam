<%@ Register TagPrefix="cc1" Namespace="com.next.infra.smartwebcontrol" Assembly="com.next.infra.smartwebcontrol" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="UclDepartmentStaff.ascx.cs" Inherits="com.next.ecs.webapp.usercontrol.UclDepartmentStaff" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<cc1:SmartDropDownList id="cbxDepartment" runat="server" AutoPostBack="True" CssClass="cbx" Width="150px" onselectedindexchanged="cbxDepartment_SelectedIndexChanged"></cc1:SmartDropDownList>
<cc1:SmartDropDownList id="cbxStaff" runat="server" CssClass="cbx" Width="150px"></cc1:SmartDropDownList>
