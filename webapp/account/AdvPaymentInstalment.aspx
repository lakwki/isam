<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="AdvPaymentInstalment.aspx.cs" Inherits="com.next.isam.webapp.account.AdvPaymentInstalment" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<script src="../common/common.js" type="text/javascript" ></script>  
<script src="../webservices/UclSmartSelection.js" type="text/javascript" ></script>  

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet"/>
    <title>Advance Payment (By Instalment) Data Maintenance</title>
    <style type="text/css">
        #p01 {
            color: #005AB5;
            font-size:medium;
        } 
        .beta table, .beta th, .beta td 
        {
            border: 1px solid black;
        } 
        .gridHeaderNew
        {
            font-size:x-small;
            font-weight:bold;
        }
        .FieldLabel2
        {
            Width:140px;
        }
        input[type=text]:disabled {
          color: #000000;
        }

        select:disabled {
          color: #000000;
        }        
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <h1 id="p01">Advance Payment (By Instalment)</h1>
    <table>
        <tr><td><asp:Label runat="server" ID="lblPN" Text="Advance Payment No" class="FieldLabel2" /></td><td><asp:Label runat="server" ID="lblPaymentNo"></asp:Label></td></tr>
        <tr><td><asp:Label runat="server" ID="lblOffice" Text="Office" class="FieldLabel2" /></td><td><cc2:smartdropdownlist id="ddlOffice" runat="server"></cc2:smartdropdownlist></td></tr>
        <tr><td><asp:Label runat="server" ID="lblVendor" Text="Supplier" class="FieldLabel2" /></td><td><uc1:UclSmartSelection  ID="txt_Supplier" runat="server" width="300px"/></td></tr>
        <tr><td><asp:Label runat="server" ID="lblPaymentDate" Text="Payment Date" class="FieldLabel2"/></td><td><cc2:SmartCalendar ID="txtOverallDate" runat="server"/></td></tr>
        <tr>
            <td class="FieldLabel2" style="width:120px;">Payable To</td>
            <td>
                <asp:TextBox ID="txt_PayableTo" runat="server" style="width:300px;"></asp:TextBox>
            </td>
        </tr>
        <tr><td><asp:Label runat="server" ID="Label4" Text="Total Payment Amt" class="FieldLabel2" /></td><td><asp:TextBox runat="server" ID="txtTotalPaymentAmt" /></td></tr>
        <tr><td><asp:Label runat="server" ID="lblTotal" Text="Total Amt (Included Interest Amt)" class="FieldLabel2"/></td><td><cc2:smartdropdownlist id="ddlCurrency" runat="server"></cc2:smartdropdownlist>&nbsp;<b><asp:Label runat="server" ID="lblTotalAmt" Text="0" /></b></td></tr>
        <tr><td><asp:Label runat="server" ID="Label1" Text="Interest Amt" class="FieldLabel2" /></td><td><asp:TextBox runat="server" ID="txtInterestChargedAmt" Text="0" /></td></tr>
        <tr><td><asp:Label runat="server" ID="Label2" Text="Interest Rate %" class="FieldLabel2" /></td><td><asp:TextBox runat="server" ID="txtInterestRate" Text="0" /></td></tr>

        <tr><td><asp:Label runat="server" ID="Label3" Text="Initiated By" class="FieldLabel2"/></td><td><uc1:UclSmartSelection ID="txt_InitiatedBy" runat="server" /></td></tr>
        <tr><td valign="top"><asp:Label runat="server" ID="lblRemark" Text="Remark" class="FieldLabel2" /></td><td><asp:TextBox runat="server" ID="txtRemark" TextMode="MultiLine" style="width:300px;height:140px; overflow-x:auto;"  /></td></tr>

        <tr>
            <td class="FieldLabel2" style="vertical-align:top ; background-position : top;">
                Balance of Adv. Payment</td>
            <td>
                <asp:Label ID="lbl_balDiff" runat="server"></asp:Label>
            </td>
        </tr>

    </table>
    <br/>
    <table>
        <tr>
            <td colspan="2" rowspan="2">           
            <asp:Repeater ID="rep_InstalmentDetail" runat="server" OnItemDataBound="rep_InstalmentDetail_DataBound">
                <HeaderTemplate>
                    <table style="width:900px;" cellspacing="0" cellpadding="0" class="beta">
                        <tr>
                            <td class="gridHeader">
                                <asp:ImageButton ID="btn_AddInstalment" runat="server" ImageUrl="~/images/icon_s_add.gif" OnClick="btn_AddInstalment_Click" CausesValidation="false"  />
                            </td>
                            <td class="gridHeaderNew" style="width:80px;">Seq</td>
                            <td class="gridHeaderNew">Expected Date</td>
                            <td class="gridHeaderNew"><b>Expected Amt<b></td>
                            <td class="gridHeaderNew"><b>Actual Deduction Amt<b></td>
                            <td class="gridHeaderNew"><b>Particular<b></td>
                            <td class="gridHeaderNew">Deduction Date</td>
                        </tr>                    
                </HeaderTemplate>
                <ItemTemplate>
                    <tr >
                        <td  style="width:20px; text-align :center;">
                            <asp:ImageButton ID="btn_RemoveInstalment" runat="server" ImageUrl="~/images/icon_remove.gif" CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' OnClick="btn_RemoveInstalment_Click" CausesValidation="false"  />
                        </td>
                        <td style="width:80px; text-align :center;">
                            <asp:Label ID="lbl_Seq" runat="server" />
                        </td>
                        <td  style="width:200px; text-align :center ;">
                            <cc2:SmartCalendar ID="txtPayDate" runat="server" />
                        </td>
                        <td style="text-align :center ;"><asp:TextBox runat="server" ID="txtExpectedAmt" Text='<%# DataBinder.Eval(Container, "DataItem.ExpectedAmount")%>'/></td>
                        <td style="text-align :center ;"><asp:TextBox runat="server" ID="txtPayAmt" Text='<%# DataBinder.Eval(Container, "DataItem.PaymentAmount")%>'/></td>
                        <td style="width:300px; text-align :left ;"><asp:TextBox style="width:280px;" runat="server" ID="txtRemark" Text='<%# DataBinder.Eval(Container, "DataItem.Remark")%>'/></td>
                        <td style="width:200px; text-align :center ;">
                            <cc2:SmartCalendar ID="txtSettleDate" runat="server" ReadOnly="false" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
       </td>
        </tr>
    </table>
    <br/>
    <asp:Button runat="server" ID="btnSave" Text="Save" onclick="btnSave_Click" />&nbsp;&nbsp;&nbsp;
    <asp:Button runat="server" ID="btnAttachment" Text="View Attachment" onclick="btnAttachment_Click" SkinID="LButton"/>&nbsp;&nbsp;&nbsp;
    <asp:Button runat="server" ID="btnLog" Text="View Log" CausesValidation="false" SkinID="LButton"/>&nbsp;&nbsp;&nbsp;
    <asp:Button runat="server" ID="btnCalculation" Text="Calculate Interest" CausesValidation="false" SkinID="XXLButton" OnClick="btnCalculation_Click" />&nbsp;&nbsp;&nbsp;
    </form>
</body>
</html>
