<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="AddContract.aspx.cs" Inherits="com.next.isam.webapp.account.AddContract" Async="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Contract</title>
    <script src="../common/jquery-1.7.min.js" type="text/javascript"></script>
    <script type="text/javascript" >
        var main = window.opener;
        $(document).ready(function () {
            $("#btn_Cancel").click(function () {
                if (confirm("Confirm to close?")) {
                    window.close();
                }
            });

            $("#selectAll").click(function () {
                var checked = this.checked;
                $(".cbSelect input[type=checkbox]").prop("checked", checked);
            });
        });

        function saveContract() {
            if (typeof(main) !== 'undefined' && main != null) {
                main.btnConfirmContractCallback();
            }
        }

        window.onload = function () {
            // This page is just open
            main.popup = window; // Assgin again after opener PostBacked
            main.toClose = true; // Assume yes for window is going to be closed with unload event
        }

        window.onunload = function () {
            if (!window.closed && (typeof (main) !== 'undefined' && main != null)) {
                if (!main.closed && main.toClose === true) { // Main page has been closed, so this popup no long
                    main.contractCloseCallback();
                    window.close();
                }
            }
        };
    </script> 
    <style type="text/css">
        .cbSelect
        {
        }
    </style>  
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:GridView ID="gv_Contractlist" runat="server" EnableModelValidation="True" AutoGenerateColumns="False" 
            onrowdatabound="gv_Contractlist_RowDataBound">
            <Columns>
                <asp:templatefield HeaderText="<input type='checkbox' id='selectAll'/>">
                    <itemtemplate>
                        <asp:checkbox ID="cbSelect" CssClass="cbSelect" runat="server"></asp:checkbox>
                    </itemtemplate>
                </asp:templatefield>
                <asp:templatefield HeaderText="Contract No">
                    <itemtemplate>
                        <asp:Label ID="lbl_ContractNo" runat="server"></asp:Label>
                    </itemtemplate>
                </asp:templatefield>
                <asp:templatefield HeaderText="Item">
                    <itemtemplate>
                        <asp:Label ID="lbl_ItemNo" runat="server"></asp:Label>
                    </itemtemplate>
                </asp:templatefield>
                <asp:templatefield HeaderText="Product Team">
                    <itemtemplate>
                        <asp:Label ID="lbl_ProductTeam" runat="server"></asp:Label>
                    </itemtemplate>                
                </asp:templatefield>
                <asp:templatefield HeaderText="Customer At-Warehouse Date">
                    <itemtemplate>
                        <asp:Label ID="lbl_CustomerAtWarehouseDate" runat="server"></asp:Label>
                    </itemtemplate>
                </asp:templatefield>
                <asp:templatefield HeaderText="Invoice No">
                    <itemtemplate>
                        <asp:Label ID="lbl_InvoiceNo" runat="server"></asp:Label>
                    </itemtemplate>
                </asp:templatefield>
                <asp:templatefield HeaderText="Invoice Date">
                    <itemtemplate>
                        <asp:Label ID="lbl_InvoiceDate" runat="server"></asp:Label>
                    </itemtemplate>
                </asp:templatefield>
                <asp:templatefield HeaderText="Payment Term">
                    <itemtemplate>
                        <asp:Label ID="lbl_PaymentTerm" runat="server"></asp:Label>
                    </itemtemplate>
                </asp:templatefield>
                <asp:templatefield HeaderText="L/C Bill Ref No">
                    <itemtemplate>
                        <asp:Label ID="lbl_LCBillRefNo" runat="server"></asp:Label>
                    </itemtemplate>
                </asp:templatefield>
                <asp:templatefield HeaderText="Workflow Status">
                    <itemtemplate>
                        <asp:Label ID="lbl_WorkflowStatus" runat="server"></asp:Label>
                    </itemtemplate>
                </asp:templatefield>
                
            </Columns>
            <EmptyDataTemplate>No record found.</EmptyDataTemplate>
        </asp:GridView>
        <hr />
        <asp:Button ID="btn_OK" runat="server" Text="OK" OnClientClick="main.toClose=false;" Enabled="false" onclick="btn_OK_Click" SkinID="MButton" />&nbsp;
        <asp:Button ID="btn_Cancel" runat="server" Text="CANCEL" SkinID="MButton" />
        <asp:Button ID="btn_Save" runat="server" Enabled="false" Visible="false" OnClick="btn_Save_Click" />
    </div>
    </form>
</body>
</html>
