<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="UKDiscountClaimRefundEdit.aspx.cs" Inherits="com.next.isam.webapp.claim.UKDiscountClaimRefundEdit" Title="Next Discount Claim Refund Edit" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" onContextMenu="return true;" >
<head runat="server">
    <title></title>
    <link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet"/>
    <script src="../common/common.js" type="text/javascript" ></script>   

</head>
<script type="text/javascript">
    window.focus();

    function returnRefund() {
        var val;
        var oAmt = document.all.txt_Amt;
        var oRcvDate = document.all.txt_ReceivedDate;
        var oRmk = document.all.txt_Remark;
        val = "\t" + oRcvDate.value + "\t" + oAmt.value + "\t" + oRmk.value.trim() + "\t";

        alert(val);
        window.returnValue = val;
        window.close();
    }

    
</script>
    <form id="form1" runat="server">

    

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
     <ContentTemplate>
    <div  style="width:450px;overflow:hidden;" >
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts" >Next Discount Claim Refund - Edit</asp:Panel>
    </div>
   
 
      <div>
        <table width="450px">
            <tr>
                <td><b><asp:validationsummary id="vs" runat="server"></asp:validationsummary></b></td>
            </tr>
            <tr>
                <td>
                    <table style="margin-left:10px;" cellpadding="5" cellspacing="0" >
                        <tr>
                            <td class="FieldLabel2">Received Date</td>
                            <td>
                                <asp:TextBox ID="txt_ReceivedDate" runat="server" SkinID="DateTextBox" onblur="formatDateString(this);" />
                                <asp:ImageButton id="btn_cal_ReceivedDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" TabIndex="-1" />
                                <cc1:CalendarExtender ID="ce_ReceivedDate" runat="server" Format="dd/MM/yyyy" PopupButtonID="btn_cal_ReceivedDate"
                                    FirstDayOfWeek="Sunday" TargetControlID="txt_ReceivedDate">
                                </cc1:CalendarExtender>
                            </td>
                            <td class="FieldLabel2" >Amount</td>
                            <td>
                                <asp:TextBox ID="txt_Amt" runat="server" style="width:70px;"/>
                            </td>          
                        </tr>
                        <tr>
                            <td class="FieldLabel2">Remark</td>
                            <td colspan="3">
                                <asp:TextBox Rows="3" ID="txt_Remark" runat="server" style="width:285px;"/>
                            </td>
                        </tr>                     
                   </table>            
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;&nbsp;
                    <asp:Button ID="btn_OK" runat="server" Text="OK" onclick="btn_OK_Click"  CausesValidation="false" />&nbsp;&nbsp; 
                    <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" onclick="btn_Cancel_Click" CausesValidation="false" />&nbsp;&nbsp;
                    <asp:Button ID="But_Exit" runat="server" Text="Exit" OnClientClick="window.close();" CausesValidation="false" visible="false"/>&nbsp;&nbsp;
                </td>
            </tr>
        </table>
        <asp:CustomValidator ID="valcustom" runat="server" Display="None" onservervalidate="valCustom_ServerValidate"></asp:CustomValidator>
     </div>
     </ContentTemplate>
    </asp:UpdatePanel>
    </form>

</html>
