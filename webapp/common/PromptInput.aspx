<%@ Page Language="C#" AutoEventWireup="true"  Theme="DefaultTheme" CodeBehind="PromptInput.aspx.cs" Inherits="com.next.common.web.PromptInput" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet"/>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ISAM - Prompt & Input
    </title>
</head>

<script type="text/javascript">
    //debugger;
    window.focus();

    function IsNumeric(str) {
        return !isNaN(parseFloat(str)) && isFinite(str)
    }

    function IsDate(str) {
        if (!isNaN(Date.parse(str))) {
            var dt = str.split("/");
            return (dt[2] >= 0 && dt[1] >= 1 && dt[1] <= 12 && dt[0] > 0 && dt[0] <= (new Date(dt[2] + (dt[1] == 12 ? 1 : 0), (dt[1] == 12 ? 0 : dt[1]), 0)).getDate());
        }
        return false;
    }

    function returnInput() {
        window.returnValue = getInputs();
        window.close();
    }

    function confirmInput() {
        if (isInputValid())
            returnInput();
    }

    function cancelInput() {
        window.close();
    }

    function getInputs() {
        var tab = document.getElementById("tab_Input");
        var inputs = tab.getElementsByTagName("Input");
        var values = new Array();
        for (i = 0; i < inputs.length; i++) 
            values.push(inputs[i].value);
        return values;
    }

    function isInputValid() {
        //debugger;
        var tab = document.getElementById("tab_Input");
        var inputs = tab.getElementsByTagName("Input");
        var val = "";
        var i;
        var isValid = true;
        var lbl, display, inputValue;
        for (i = 0; i < inputs.length; i++) {
            lbl = inputs[i].parentElement.parentElement.getElementsByTagName("SPAN")[0];
            display = "none";
            if (inputs[i].DataType == "DECIMAL") {
                inputValue = inputs[i].value.replace(",", "");
                if (!IsNumeric(inputValue)) {
                    display = "block";
                    isValid = false;
                }
            }
            else if (inputs[i].DataType == "DATE") {
                if (!IsDate(inputs[i].value)) {
                    display = "block";
                    isValid = false;
                }
            }
            lbl.style.display = display;
        }
        return isValid;
    }

</script>

<body>
    <form id="form1" runat="server">
    <div>
       <table>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr style="display:none;">
                <td colspan="3">
                    <asp:panel ID="Panel1" SkinID="sectionHeader2" runat="server" Font-Size="Medium" Font-Bold="true" Visible="false"></asp:panel>
                    <asp:Label ID="lblPrompt" runat="server" class="FieldLabel2"  /><asp:textbox ID="txtInput" runat="server"/>
                    <asp:Calendar ID="cal_date" runat="server"></asp:Calendar>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Table  ID="tab_Input" runat="server"></asp:Table>
                </td>
            </tr>
        </table>
        <table width="100%">
            <tr>
                <td align="right">
                    <asp:Button ID="btnComfirm" runat="server" text="Confirm" OnClientClick="confirmInput();return false;"/>&nbsp;&nbsp;
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="cancelInput();return false;" />&nbsp;&nbsp;
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
