<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="DebitNoteVoid.aspx.cs" Inherits="com.next.isam.webapp.account.DCNoteVoid" Async="true" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Void Next Claim - <%= vwDCNoteNo %></title>
    <link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet"/>
    <script src="../common/jquery-1.7.min.js" type="text/javascript" ></script>   
    <script src="../includes/popBox1.3.0.min.js" type="text/javascript"></script>
    <link href="../includes/popBox1.3.0.css" rel="stylesheet" type="text/css" />
    <script src="../common/common.js" type="text/javascript" ></script>  
    <script src="../webservices/UclSmartSelection.js" type="text/javascript" ></script>
    <script type="text/javascript">
        var main = window.opener;
        var ddl_VoidType;
        //var lb_OldSupplierName;
        var uclVendorCurrentdName = "";
        var uclVendorCurrentdId = "";
        var cv_NewSupplier;
        var tr_Supplier;

        $(document).ready(function () {
            $("#btn_Cancel").click(function () {
                if (confirm("Confirm to close?")) {
                    window.close();
                } else {
                    return false;
                }
            });
        });

        window.onload = function () {
            tr_Supplier = document.getElementById('tr_Supplier');
            var uclVendor = document.getElementById('<%# uclVendor.ClientID %>_divBase'); //uclVendor_divBase
            //lb_OldSupplierName = document.getElementById('<%# lb_OldSupplierName.ClientID %>');
            uclVendorCurrentdName = document.getElementById('<%# uclVendor.ClientID %>:txtOldName').value; //uclVendor:txtOldName
            uclVendorCurrentdId = document.getElementById('<%# uclVendor.ClientID %>:txtId').value; //uclVendor:txtId

            cv_NewSupplier = document.getElementById('<%# cv_NewSupplier.ClientID %>');

            ddl_VoidType = document.getElementById("ddl_VoidType");

            ddl_VoidType.onchange = function () {
                cv_NewSupplier.style.display = "none";
                setUclSupplierList(this.value, uclVendor);
            };

            setUclSupplierList(ddl_VoidType.value, uclVendor);
            var tb_Remarks = document.getElementById("tb_Remarks");
        }

        function setUclSupplierList(selectedValue, uclVendor) {
            switch (selectedValue) {
                case 'SUPPLIER':
                    tr_Supplier.style.display = "";
                    uclVendor.style.display = "";
                    //lb_OldSupplierName.style.display = "";
                    break;
                default:
                    tr_Supplier.style.display = "none";
                    uclVendor.style.display = "none";
                    //lb_OldSupplierName.style.display = "none";
                    break;
            }
        }

        function submitVoid() {
            var oldSupplierId = '<%# vwOldSupplierId %>';
            var confirmed = true;
            uclVendorCurrentdName = document.getElementById('uclVendor:txtOldName').value; //uclVendor:txtOldName
            uclVendorCurrentdId = document.getElementById('uclVendor:txtId').value; //uclVendor:txtId

            if (ddl_VoidType.value == "NSPROVISION")
                confirmed = confirm("Confirm to void");
            if (ddl_VoidType.value == "SUPPLIER") {
                if (!isNaN(uclVendorCurrentdId) && uclVendorCurrentdId !== "") {
                    if (oldSupplierId != uclVendorCurrentdId && uclVendorCurrentdId >= 0) {
                        confirmed = confirm("Confirm to void and change the supplier to:\n\r" + uclVendorCurrentdName + "?");
                    }
                }
            }
            if (confirmed) {
                cv_NewSupplier.style.display = "";
            } else {
                cv_NewSupplier.style.display = "none";
            }
            
            return confirmed;
        }        
    </script>
    <style type="text/css">
        #p01 {
            color: #005AB5;
            font-size:medium;
        }
        
        #ddl_VoidType
        {
            width: 150px !important;
        }
        #uclVendor_txtName
        {
            width: 250px !important;
        }
        .FieldLabel2
        {
            vertical-align:top;
            background-position: top;
            width: 120px;
        }        
        
    </style>
</head>
<body>
    <form id="form1" runat="server" method="post">
    <div>
        <h1 id="p01">Void Next Claim - <%= vwDCNoteNo %></h1>
        <table>
            <tr>
                <td class="FieldLabel2">Void Type: </td>
                <td><asp:DropDownList ID="ddl_VoidType" runat="server" ValidationGroup="Submit" ></asp:DropDownList></td>
            </tr>
            <tr id="tr_Supplier">
                <td class="FieldLabel2">Supplier: </td>
                <td><uc1:uclsmartselection ID="uclVendor" runat="server"></uc1:uclsmartselection>
                    <asp:CustomValidator ID="cv_NewSupplier" runat="server" ValidationGroup="Submit" onservervalidate="cv_NewSupplier_ServerValidate"></asp:CustomValidator>
                    <br/><asp:Label ID="lb_OldSupplierName" runat="server" Text="" ></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:Button ID="btn_Save" runat="server" Text="Submit" UseSubmitBehavior="True" OnClientClick="return submitVoid();" onclick="btn_Save_Click" SkinID="MButton" CausesValidation="True" ValidationGroup="Submit" />&nbsp;
        <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" SkinID="MButton"  />

        <br />
    </div>
    </form>
</body>
</html>
