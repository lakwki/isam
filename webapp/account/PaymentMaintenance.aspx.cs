using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.util;
using com.next.infra.web;
using com.next.isam.appserver.account;
using com.next.isam.domain.order;
using com.next.isam.domain.claim;
using com.next.isam.domain.types;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.webapp.commander.shipment;
using com.next.common.web.commander;
using com.next.common.domain;

namespace com.next.isam.webapp.account
{
    public partial class PaymentMaintenance : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private ArrayList vwSearchResult
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (ArrayList)ViewState["SearchResult"];
            }
        }

        private ArrayList vwDNSearchResult
        {
            set
            {
                ViewState["DNSearchResult"] = value;
            }
            get
            {
                return (ArrayList)ViewState["DNSearchResult"];
            }
        }

        private ArrayList vwCcyDiscrepancyList
        {
            set
            {
                ViewState["CcyDiscrepancyList"] = value;
            }
            get
            {
                return (ArrayList)ViewState["CcyDiscrepancyList"];
            }
        }

        private ArrayList vwDNCcyDiscrepancyList
        {
            set
            {
                ViewState["DNCcyDiscrepancyList"] = value;
            }
            get
            {
                return (ArrayList)ViewState["DNCcyDiscrepancyList"];
            }
        }
        ArrayList updatedList = null;
        /*
        protected string lbl_TotalSalesAmt_id = "lbl_TotalSalesAmt";
        protected string lbl_TotalARAmt_id = "lbl_TotalARAmt";
        protected string lbl_TotalAPAmt_id = "lbl_TotalAPAmt";
        protected string lbl_TotalPurchaseAmt_id = "lbl_TotalPurchaseAmt";
        protected string lbl_TotalCommAmt_id = "lbl_TotalCommAmt";
        protected string lbl_TotalSalesCommAmt_id = "lbl_TotalSalesCommAmt";
        protected string lbl_TotalDNAmt_id = "lbl_TotalDNAmt";
        protected string lbl_TotalDNSettleAmt_id = "lbl_TotalDNSettleAmt";
        */
        protected decimal totalSalesAmt = 0;
        protected decimal totalPurchaseAmt = 0;
        protected decimal totalARAmt = 0;
        protected decimal totalAPAmt = 0;
        protected decimal totalCommAmt = 0;
        protected decimal totalSalesCommAmt = 0;
        protected decimal totalSelectedSalesAmt = 0;
        protected decimal totalSelectedPurchaseAmt = 0;
        protected decimal totalSelectedARAmt = 0;
        protected decimal totalSelectedAPAmt = 0;
        protected decimal totalSelectedCommAmt = 0;
        protected decimal totalSelectedSalesCommAmt = 0;

        protected int selectedCount = 0;
        protected int uploadCount = 0;
        protected string type = "ar";

        protected decimal totalDNAmt = 0;
        protected decimal totalSelectedDNAmt = 0;
        protected decimal totalDNSettleAmt = 0;
        protected decimal totalSelectedDNSettleAmt = 0;
        protected int DNSelectedCount = 0;
        protected int DNUploadCount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            type = ddl_type.SelectedValue;
        }

        #region GridView Event

        protected void PaymentRowDelete(object sender, GridViewDeleteEventArgs arg)
        {
            vwSearchResult.RemoveAt(arg.RowIndex);

            gv_Payment.DataSource = vwSearchResult;
            gv_Payment.DataBind();
        }

        protected void PaymentRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContractShipmentListJDef def = (ContractShipmentListJDef)vwSearchResult[e.Row.RowIndex];
                Label lbl = (Label)e.Row.Cells[4].FindControl("lbl_Currency");
                TextBox txt;
                CheckBox ckb = (CheckBox)e.Row.Cells[0].FindControl("ckb_update");

                if (type == "ap")
                {
                    lbl.Text = def.BuyCurrency.CurrencyCode;
                }
                else
                {
                    lbl.Text = def.SellCurrency.CurrencyCode;
                }

                if (def.SequenceNo == int.MinValue)
                {
                    lbl = (Label)e.Row.Cells[2].FindControl("lbl_SeqNo");
                    lbl.Text = "";
                }

                #region ar
                if (gv_Payment.Columns[9].Visible)
                {
                    if (def.ARDate == DateTime.MinValue)
                    {
                        txt = (TextBox)e.Row.FindControl("txt_ARDate");
                        txt.Text = "";
                    }

                    TextBox txtAmt = (TextBox)e.Row.Cells[8].FindControl("txt_ARAmt");
                    TextBox txtDate = (TextBox)e.Row.Cells[9].FindControl("txt_ARDate");
                    TextBox txtRefNo = (TextBox)e.Row.Cells[10].FindControl("txt_ARRefNo");

                    //hide textbox in ar columns for split shipment
                    if (def.InvoiceNo.Contains("-"))
                    {
                        txtAmt.Visible = false;
                        txtDate.Visible = false;
                        txtRefNo.Visible = false;
                    }

                    if (vwCcyDiscrepancyList != null && vwCcyDiscrepancyList.Contains(def.ShipmentId))
                    {
                        txtAmt.Enabled = false;
                        txtAmt.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);
                        txtDate.Enabled = false;
                        txtDate.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);
                        txtRefNo.Enabled = false;
                        txtRefNo.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);
                        ckb.Checked = false;
                    }

                    if (updatedList != null && updatedList.Contains(def.InvoiceNo))
                    {
                        e.Row.BackColor = System.Drawing.Color.MistyRose;
                        ckb.Checked = false;
                    }
                }
                #endregion

                #region ap
                if (gv_Payment.Columns[13].Visible)
                {
                    if (def.APDate == DateTime.MinValue)
                    {
                        txt = (TextBox)e.Row.Cells[13].FindControl("txt_APDate");
                        txt.Text = "";
                    }
                    if (vwCcyDiscrepancyList != null && vwCcyDiscrepancyList.Contains(def.ShipmentId))
                    {
                        txt = (TextBox)e.Row.Cells[12].FindControl("txt_APAmt");
                        txt.Enabled = false;
                        txt.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        txt = (TextBox)e.Row.Cells[13].FindControl("txt_APDate");
                        txt.Enabled = false;
                        txt.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        txt = (TextBox)e.Row.Cells[14].FindControl("txt_APRefNo");
                        txt.Enabled = false;
                        txt.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);
                        ckb.Checked = false;
                    }
                    if (updatedList != null && updatedList.Contains(def.InvoiceNo))
                    {
                        e.Row.BackColor = System.Drawing.Color.MistyRose;
                        ckb.Checked = false;
                    }
                }
                #endregion

                #region sales commission
                if (gv_Payment.Columns[15].Visible)
                {
                    if (def.NSLCommissionSettlementDate == DateTime.MinValue)
                    {
                        txt = (TextBox)e.Row.FindControl("txt_commDate");
                        txt.Text = "";
                    }
                    if (vwCcyDiscrepancyList != null && vwCcyDiscrepancyList.Contains(def.ShipmentId))
                    {
                        txt = (TextBox)e.Row.FindControl("txt_commAmt");
                        txt.Enabled = false;
                        txt.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        txt = (TextBox)e.Row.FindControl("txt_commDate");
                        txt.Enabled = false;
                        txt.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        txt = (TextBox)e.Row.FindControl("txt_commRefNo");
                        txt.Enabled = false;
                        txt.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);
                        ckb.Checked = false;
                    }
                    if (updatedList != null && updatedList.Contains(def.InvoiceNo))
                    {
                        e.Row.BackColor = System.Drawing.Color.MistyRose;
                        ckb.Checked = false;
                    }
                    if (def.TotalShippedAmount > 0 && (def.NSLCommissionSettlementAmount / def.TotalShippedAmount * 100) >= 16)
                    {
                        e.Row.BackColor = System.Drawing.Color.Aquamarine;
                        ckb.Checked = false;
                    }
                }
                #endregion

                totalSalesAmt += def.TotalShippedAmount;
                totalPurchaseAmt += def.TotalShippedSupplierGarmentAmountAfterDiscount;
                totalARAmt += def.ARAmount;
                totalCommAmt += def.NSLCommissionSettlementAmount;
                totalSalesCommAmt += def.NSLCommissionAmount;

                //if ((type == "ap" || type == "ap2") && gv_Payment.Columns[10].Visible && gv_Payment.Columns[13].Visible) //not uploaded from file
                if ((type == "ap" || type == "ap2" || type == "ap3") && gv_Payment.Columns[13].Visible) //not uploaded from file
                {
                    if (def.APAmount == 0)
                    {   // load default AP amount if it is empty
                        def.APAmount += decimal.Round(def.TotalShippedSupplierGarmentAmountAfterDiscount, 2, MidpointRounding.AwayFromZero)
                                        - Math.Round(def.QACommissionAmount, 2, MidpointRounding.AwayFromZero)
                                        - Math.Round(def.VendorPaymentDiscountAmount, 2, MidpointRounding.AwayFromZero)
                                        - Math.Round(def.LabTestIncomeAmount, 2, MidpointRounding.AwayFromZero);
                        txt = (TextBox)e.Row.Cells[11].FindControl("txt_APAmt");
                        txt.Text = def.APAmount.ToString("#,##0.00");
                    }
                }
                totalAPAmt += def.APAmount;

                if (ckb.Checked)
                {
                    totalSelectedSalesAmt += def.TotalShippedAmount;
                    totalSelectedPurchaseAmt += def.TotalShippedSupplierGarmentAmountAfterDiscount;
                    totalSelectedARAmt += def.ARAmount;
                    totalSelectedAPAmt += def.APAmount;
                    totalSelectedCommAmt += def.NSLCommissionSettlementAmount;
                    totalSelectedSalesCommAmt += def.NSLCommissionAmount;
                    selectedCount += 1;
                }
            }
/*
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                Label lbl = (Label)e.Row.FindControl("lbl_TotalSalesAmt");
                lbl_TotalSalesAmt_id = lbl.ClientID;
                lbl.Text = totalSelectedSalesAmt.ToString();

                lbl = (Label)e.Row.FindControl("lbl_TotalPurchaseAmt");
                lbl_TotalPurchaseAmt_id = lbl.ClientID;
                lbl.Text = totalSelectedPurchaseAmt.ToString();

                lbl = (Label)e.Row.FindControl("lbl_TotalARAmt");
                lbl_TotalARAmt_id = lbl.ClientID;
                lbl.Text = totalSelectedARAmt.ToString();

                lbl = (Label)e.Row.FindControl("lbl_TotalAPAmt");
                lbl_TotalAPAmt_id = lbl.ClientID;
                lbl.Text = totalSelectedAPAmt.ToString();

                lbl = (Label)e.Row.FindControl("lbl_TotalCommAmt");
                lbl_TotalCommAmt_id = lbl.ClientID;
                lbl.Text = totalSelectedCommAmt.ToString();

                lbl = (Label)e.Row.FindControl("lbl_TotalSalesCommAmt");
                lbl_TotalSalesCommAmt_id = lbl.ClientID;
                lbl.Text = totalSelectedSalesCommAmt.ToString();
            }
 */ 
        }

        protected void DNPaymentRowDelete(object sender, GridViewDeleteEventArgs arg)
        {
            vwSearchResult.RemoveAt(arg.RowIndex);

            gv_DNPayment.DataSource = vwSearchResult;
            gv_DNPayment.DataBind();
        }

        protected void DNPaymentRowDataBound(object sender, GridViewRowEventArgs e)
        {
            int colCheckBox = 0;
            int colDNNo = 1;
            int colCurrency = 2;
            int colDNAmt = 3;
            int colSettleAmt = 4;
            int colSettleDate = 5;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TextBox txt;
                UKClaimDCNoteDef def = (UKClaimDCNoteDef)vwDNSearchResult[e.Row.RowIndex];
                CheckBox ckb = (CheckBox)e.Row.Cells[colCheckBox].FindControl("ckb_update");
                Label lbl = (Label)e.Row.Cells[colCurrency].FindControl("lbl_Currency");
                lbl.Text = CommonUtil.getCurrencyByKey(def.CurrencyId).CurrencyCode;

                if (gv_DNPayment.Columns[colSettleDate].Visible)
                {
                    if (def.SettlementDate == DateTime.MinValue)
                    {
                        txt = (TextBox)e.Row.Cells[colSettleDate].FindControl("txt_Date");
                        txt.Text = "";
                    }
                    if (vwCcyDiscrepancyList != null && vwCcyDiscrepancyList.Contains(def.DCNoteId * -1))
                    {
                        lbl.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        //txt = (TextBox)e.Row.Cells[colSettleAmt].FindControl("txt_DNSettleAmt");
                        //txt.Enabled = false;
                        //txt.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        //lbl = (Label)e.Row.Cells[colSettleAmt].FindControl("lbl_DNSettleAmt");
                        //lbl.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        txt = (TextBox)e.Row.Cells[colSettleDate].FindControl("txt_Date");
                        txt.Enabled = false;
                        txt.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        ckb.Checked = false;
                    }
                }

                totalDNAmt += def.TotalAmount;
                totalDNSettleAmt += def.SettledAmount;

                if (ckb.Checked)
                {
                    totalSelectedDNAmt += def.TotalAmount;
                    totalSelectedDNSettleAmt += def.SettledAmount;
                    DNSelectedCount += 1;
                }
            }
/*            
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                Label lbl;
                lbl = (Label)e.Row.FindControl("lbl_TotalDNAmt");
                lbl_TotalDNAmt_id = lbl.ClientID;
                lbl.Text = totalSelectedDNAmt.ToString();

                lbl = (Label)e.Row.FindControl("lbl_TotalDNSettleAmt");
                lbl_TotalDNSettleAmt_id = lbl.ClientID;
                lbl.Text = totalSelectedDNSettleAmt.ToString();
            }
 */ 
        }

        #endregion

        private string copyFile()
        {
            string destinationPath = "";

            if (file_ARAP.HasFile)
            {
                string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + file_ARAP.FileName;

                if (type == "ar")
                    destinationPath = WebConfig.getValue("appSettings", "UPLOAD_AR_Folder");
                else
                    destinationPath = WebConfig.getValue("appSettings", "UPLOAD_AP_Folder");

                try
                {
                    destinationPath += fileName;
                    file_ARAP.PostedFile.SaveAs(destinationPath);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return destinationPath;
        }

        private void UpdateARAP()
        {
            ContractShipmentListJDef def;
            TextBox txt;
            ArrayList tempList = new ArrayList();
            CheckBox ckb;
            foreach (GridViewRow row in gv_Payment.Rows)
            {
                def = (ContractShipmentListJDef)vwSearchResult[row.RowIndex];
                ckb = (CheckBox)row.Cells[0].FindControl("ckb_update");
                if (!ckb.Checked)
                    continue;
                if (vwCcyDiscrepancyList != null && vwCcyDiscrepancyList.Contains(def.ShipmentId))
                {
                    continue;
                }
                #region ar
                if (gv_Payment.Columns[9].Visible)
                {
                    txt = (TextBox)row.Cells[9].FindControl("txt_ARAmt");
                    if (txt.Visible)
                    {
                        if (txt.Text.Trim() != "")
                            def.ARAmount = Convert.ToDecimal(txt.Text);
                        else
                            def.ARAmount = 0;

                        txt = (TextBox)row.Cells[9].FindControl("txt_ARDate");
                        if (txt.Text.Trim() != "")
                            def.ARDate = DateTimeUtility.getDate(txt.Text);
                        else
                            def.ARDate = DateTime.MinValue;

                        txt = (TextBox)row.Cells[10].FindControl("txt_ARRefNo");
                        def.ARRefNo = txt.Text.Trim();
                    }
                }
                #endregion

                #region ap
                if (gv_Payment.Columns[12].Visible)
                {
                    txt = (TextBox)row.Cells[12].FindControl("txt_APAmt");
                    if (txt.Text.Trim() != "")
                        def.APAmount = Convert.ToDecimal(txt.Text);
                    else
                        def.APAmount = 0;

                    txt = (TextBox)row.Cells[12].FindControl("txt_APDate");
                    if (txt.Text.Trim() != "")
                        def.APDate = DateTimeUtility.getDate(txt.Text);
                    else
                        def.APDate = DateTime.MinValue;

                    txt = (TextBox)row.Cells[13].FindControl("txt_APRefNo");
                    def.APRefNo = txt.Text.Trim();
                }
                #endregion

                #region sales commission
                if (gv_Payment.Columns[15].Visible)
                {
                    txt = (TextBox)row.Cells[15].FindControl("txt_commAmt");
                    if (txt.Text.Trim() != "")
                        def.NSLCommissionSettlementAmount = Convert.ToDecimal(txt.Text);
                    else
                        def.NSLCommissionSettlementAmount = 0;

                    txt = (TextBox)row.Cells[16].FindControl("txt_commDate");
                    if (txt.Text.Trim() != "")
                        def.NSLCommissionSettlementDate = DateTimeUtility.getDate(txt.Text);
                    else
                        def.NSLCommissionSettlementDate = DateTime.MinValue;

                    txt = (TextBox)row.Cells[17].FindControl("txt_commRefNo");
                    def.NSLCommissionSettlementRefNo = txt.Text.Trim();
                }
                #endregion

                tempList.Add(def);
            }
            vwSearchResult = tempList;
        }

        private void UpdateDCNote()
        {
            int colCheckBox = 0;
            int colDNNo = 1;
            int colCurrency = 2;
            int colDNAmt = 3;
            int colSettleAmt = 4;
            int colSettleDate = 5;

            UKClaimDCNoteDef def;
            TextBox txt;
            ArrayList tempList = new ArrayList();
            CheckBox ckb;
            foreach (GridViewRow row in gv_DNPayment.Rows)
            {
                def = (UKClaimDCNoteDef)vwDNSearchResult[row.RowIndex];
                ckb = (CheckBox)row.Cells[colCheckBox].FindControl("ckb_update");
                if (!ckb.Checked)
                    continue;
                if (vwDNCcyDiscrepancyList != null && vwDNCcyDiscrepancyList.Contains(def.DCNoteId))
                {
                    continue;
                }

                if (gv_Payment.Columns[colSettleDate].Visible)
                {

                    txt = (TextBox)row.Cells[colSettleDate].FindControl("txt_Date");
                    if (txt.Text.Trim() != "")
                        def.SettlementDate = DateTimeUtility.getDate(txt.Text);
                    else
                        def.SettlementDate = DateTime.MinValue;
                }
                tempList.Add(def);
            }
            vwDNSearchResult = tempList;
        }

        protected void val_payment_validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            // args.IsValid = true;
            TextBox txt;
            decimal num;
            DateTime date;

            foreach (GridViewRow row in gv_Payment.Rows)
            {
                if (gv_Payment.Columns[9].Visible)
                {
                    num = 0;
                    date = DateTime.MinValue;
                    txt = (TextBox)row.Cells[9].FindControl("txt_ARAmt");
                    if (txt.Visible)
                    {
                        if (txt.Text != "" && !decimal.TryParse(txt.Text, out num))
                        {
                            txt.BorderColor = System.Drawing.Color.Red;
                            args.IsValid = false;
                        }
                        txt = (TextBox)row.Cells[10].FindControl("txt_ARDate");
                        if ((num != 0 || txt.Text != "") && !DateTime.TryParse(txt.Text, null, System.Globalization.DateTimeStyles.None, out date))
                        {
                            txt.BorderColor = System.Drawing.Color.Red;
                            args.IsValid = false;
                        }
                        if (date != DateTime.MinValue)
                        {
                            txt = (TextBox)row.Cells[11].FindControl("txt_ARRefNo");
                            if (txt.Text.Trim() == "")
                            {
                                txt.BorderColor = System.Drawing.Color.Red;
                                args.IsValid = false;
                            }
                        }
                    }

                    if (gv_Payment.Columns[12].Visible)
                    {
                        num = 0;
                        date = DateTime.MinValue;
                        txt = (TextBox)row.Cells[12].FindControl("txt_APAmt");
                        if (txt.Text != "" && !decimal.TryParse(txt.Text, out num))
                        {
                            txt.BorderColor = System.Drawing.Color.Red;
                            args.IsValid = false;
                        }
                        txt = (TextBox)row.Cells[13].FindControl("txt_APDate");
                        if ((num != 0 || txt.Text != "") && !DateTime.TryParse(txt.Text, null, System.Globalization.DateTimeStyles.None, out date))
                        {
                            txt.BorderColor = System.Drawing.Color.Red;
                            args.IsValid = false;
                        }
                        if (date != DateTime.MinValue)
                        {
                            txt = (TextBox)row.Cells[14].FindControl("txt_APRefNo");
                            if (txt.Text.Trim() == "")
                            {
                                txt.BorderColor = System.Drawing.Color.Red;
                                args.IsValid = false;
                            }
                        }
                    }

                    if (gv_Payment.Columns[15].Visible)
                    {
                        num = 0;
                        date = DateTime.MinValue;
                        txt = (TextBox)row.FindControl("txt_commAmt");
                        if (txt.Text != "" && !decimal.TryParse(txt.Text, out num))
                        {
                            txt.BorderColor = System.Drawing.Color.Red;
                            args.IsValid = false;
                        }
                        txt = (TextBox)row.FindControl("txt_commDate");
                        if ((num != 0 || txt.Text != "") && !DateTime.TryParse(txt.Text, null, System.Globalization.DateTimeStyles.None, out date))
                        {
                            txt.BorderColor = System.Drawing.Color.Red;
                            args.IsValid = false;
                        }
                        if (date != DateTime.MinValue)
                        {
                            txt = (TextBox)row.FindControl("txt_commRefNo");
                            if (txt.Text.Trim() == "")
                            {
                                txt.BorderColor = System.Drawing.Color.Red;
                                args.IsValid = false;
                            }
                        }
                    }
                }
            }
        }

        #region button events

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            if (vwSearchResult != null)
                vwSearchResult.Clear();
            gv_Payment.DataSource = vwSearchResult;
            gv_Payment.DataBind();

            if (vwDNSearchResult!=null)
                vwDNSearchResult.Clear();
            gv_DNPayment.DataSource = vwDNSearchResult;
            gv_DNPayment.DataBind();

            pnl_Result.Visible = false;
            lbl_Msg.Visible = false;
            lbl_Error.Visible = false;
            pnl_TotalSelected.Visible = false;
            pnl_TotalDNSelected.Visible = false;

            txt_NSLInvNo.Text = string.Empty;
            txt_LCBillRefNo.Text = string.Empty;
        }

        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            ArrayList uploadList = new ArrayList();

            if (ddl_type.SelectedIndex <= 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "FileTypeError", "alert('Please select file type.');", true);
                return;
            }
            if (!file_ARAP.HasFile)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "FileTypeError", "alert('Please select a file.');", true);
                return;
            }
            pnl_Result.Visible = true;
            string filePath = copyFile();

            if (filePath == "")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploaderror", "alert('Please select a file.');", true);
                return;
            }

            ArrayList ccyDiscrepancyList;
            bool isFileSplit = false;

            try
            {
                uploadList = AccountManager.Instance.getUploadedARAPData(this.LogonUserId, filePath, ddl_type.SelectedValue, out ccyDiscrepancyList, out updatedList, out isFileSplit);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploaderror", string.Format("alert('{0}');", ex.Message), true);
                return;
            }
            vwCcyDiscrepancyList = ccyDiscrepancyList;

            if (uploadList != null)
            {
                if (type == "ar")
                {
                    gv_Payment.Columns[9].Visible = true;
                    gv_Payment.Columns[10].Visible = true;
                    gv_Payment.Columns[11].Visible = true;
                    gv_Payment.Columns[12].Visible = false;
                    gv_Payment.Columns[13].Visible = false;
                    gv_Payment.Columns[14].Visible = false;
                    gv_Payment.Columns[15].Visible = false;
                    gv_Payment.Columns[16].Visible = false;
                    gv_Payment.Columns[17].Visible = false;
                }
                else if (type == "ap" || type == "ap2" || type == "ap3")
                {
                    gv_Payment.Columns[9].Visible = false;
                    gv_Payment.Columns[10].Visible = false;
                    gv_Payment.Columns[11].Visible = false;
                    gv_Payment.Columns[12].Visible = true;
                    gv_Payment.Columns[13].Visible = true;
                    gv_Payment.Columns[14].Visible = true;
                    gv_Payment.Columns[15].Visible = false;
                    gv_Payment.Columns[16].Visible = false;
                    gv_Payment.Columns[17].Visible = false;
                }
                else
                {
                    gv_Payment.Columns[9].Visible = false;
                    gv_Payment.Columns[10].Visible = false;
                    gv_Payment.Columns[11].Visible = false;
                    gv_Payment.Columns[12].Visible = false;
                    gv_Payment.Columns[13].Visible = false;
                    gv_Payment.Columns[14].Visible = false;
                    gv_Payment.Columns[15].Visible = true;
                    gv_Payment.Columns[16].Visible = true;
                    gv_Payment.Columns[17].Visible = true;
                }

                lbl_Msg.Visible = false;
                lbl_Error.Visible = false;

                ArrayList arapList = new ArrayList();
                ArrayList dnList = new ArrayList();
                for (int i = 0; i < uploadList.Count; i++)
                    if (uploadList[i].GetType() == typeof(ContractShipmentListJDef))
                        arapList.Add(uploadList[i]);
                    else if (uploadList[i].GetType() == typeof(UKClaimDCNoteDef))
                        dnList.Add(uploadList[i]);
                vwSearchResult = arapList;
                vwDNSearchResult = dnList;

                uploadCount = vwSearchResult.Count;
                gv_Payment.DataSource = vwSearchResult;
                gv_Payment.DataBind();
                lbl_TotalSelected.Text = selectedCount.ToString();
                pnl_TotalSelected.Visible = (uploadCount > 0);
                pnl_Payment.Visible = (vwSearchResult.Count > 0);

                DNUploadCount = vwDNSearchResult.Count;
                gv_DNPayment.DataSource = vwDNSearchResult;
                gv_DNPayment.DataBind();
                lbl_TotalDNSelected.Text = DNSelectedCount.ToString();
                pnl_TotalDNSelected.Visible = (DNUploadCount > 0);
                pnl_DNPayment.Visible = (vwDNSearchResult.Count > 0);

                if (isFileSplit)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadContextExceedLimit",
                        "alert('File content exceeded system limit. Only the first 1000 entries are uploaded.');", true);
                }
            }

        }

        protected void btn_Save_Click(object sender, EventArgs e)
        {
            //val_payment.Enabled = true;

            if (Page.IsValid)
            {
                UpdateARAP();
                UpdateDCNote();

                ArrayList list = new ArrayList();
                foreach (ContractShipmentListJDef def in vwSearchResult) list.Add(def);
                foreach (UKClaimDCNoteDef def in vwDNSearchResult) list.Add(def);

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateARAPDN);

                Context.Items.Add(AccountCommander.Param.invoiceDebitNoteList, list);

                forwardToScreen(null);

                ArrayList updateFailedList = (ArrayList)Context.Items[AccountCommander.Param.result];

                if (updateFailedList.Count > 0)
                {
                    lbl_Error.Visible = true;
                    lbl_Error.Text = "The following record are failed to update : <br />";
                    for (int i = 0; i < updateFailedList.Count; i++)
                        lbl_Error.Text += updateFailedList[i].ToString() + "<br />";
                }
                lbl_Msg.Text = (list.Count - updateFailedList.Count).ToString() + " records updated.";

                vwSearchResult = null;
                gv_Payment.DataSource = vwSearchResult;
                gv_Payment.DataBind();
                pnl_Payment.Visible = false;
                pnl_TotalSelected.Visible = false;

                vwDNSearchResult = null;
                gv_Payment.DataSource = vwDNSearchResult;
                gv_Payment.DataBind();
                pnl_DNPayment.Visible = false;
                pnl_TotalDNSelected.Visible = false;
                
                pnl_Result.Visible = false;
                lbl_Msg.Visible = true;
            }
        }

        protected void btn_Add_Click(object sender, EventArgs e)
        {
            if (txt_NSLInvNo.Text.Trim() != string.Empty)
                AddPaymentRecordByInvoiceNo();
            if (txt_LCBillRefNo.Text.Trim() != string.Empty)
                AddPaymentRecordByLCBillRefNo();
        }

        protected void AddPaymentRecordByInvoiceNo()
        {
            //val_payment.Enabled = false;
            pnl_Result.Visible = true;
            string input = txt_NSLInvNo.Text.Trim();
            string invoiceNo = string.Empty;
            string dbNoteNo = string.Empty;
            ArrayList list = null;
            txt_NSLInvNo.Text = "";

            bool isInvoiceNo = input.Contains("/");
            bool isSplitOrder = input.Contains("-");
            if (isInvoiceNo || isSplitOrder)
                invoiceNo = input;
            else
                dbNoteNo = input;

            if (isInvoiceNo)
            {
                int sequenceNo = -1;
                if (invoiceNo.Contains("-"))
                    int.TryParse(invoiceNo.Substring(invoiceNo.IndexOf('-') + 1), out sequenceNo);

                //get invoice..
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetInvoiceListByInvoiceNo);
                Context.Items.Add(AccountCommander.Param.invoicePrefix, WebUtil.getInvoicePrefix(invoiceNo));
                Context.Items.Add(AccountCommander.Param.invoiceSeq, WebUtil.getInvoiceSeq(invoiceNo));
                Context.Items.Add(AccountCommander.Param.invoiceYear, WebUtil.getInvoiceYear(invoiceNo));
                Context.Items.Add(AccountCommander.Param.sequenceNo, sequenceNo);
                Context.Items.Add(AccountCommander.Param.officeList, CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type));

                forwardToScreen(null);

                list = (ArrayList)Context.Items[AccountCommander.Param.invoiceList];
            }
            else if (isSplitOrder)
            {
                //get split shipment
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.GetSplitShipmentInvoice);
                Context.Items.Add(ShipmentCommander.Param.poNo, invoiceNo);

                forwardToScreen(null);

                list = (ArrayList)Context.Items[ShipmentCommander.Param.shipmentList];
            }
            else if (type == "ap")
            {   // get Next Claim DB Note
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetUKClaimDCNoteByDCNoteNo);
                Context.Items.Add(AccountCommander.Param.dcNoteNo, dbNoteNo);

                forwardToScreen(null);

                UKClaimDCNoteDef def = (UKClaimDCNoteDef)Context.Items[AccountCommander.Param.dcNote];
                list = new ArrayList();
                if (def != null)
                    list.Add(def);
            }

            if (list == null || list.Count == 0)
            {
                //invoice not found..
                if (type == "ap" && !isInvoiceNo && !isSplitOrder)
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invoiceNotFound", "alert('Debit Note not found.')", true);
                else
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invoiceNotFound", "alert('Invoice/split shipment not found.')", true);
                return;
            }
            else
            {
                //ContractShipmentListJDef def = (ContractShipmentListJDef) list[0];
                if (list[0].GetType() == typeof(ContractShipmentListJDef))
                {
                    foreach (ContractShipmentListJDef def in list)
                    {
                        if (!isInvoiceNo)
                            def.InvoiceNo = invoiceNo;

                        if (vwSearchResult != null)
                        {
                            //check for duplicate entry..
                            foreach (ContractShipmentListJDef temp in vwSearchResult)
                            {
                                if (temp.InvoiceNo == def.InvoiceNo && temp.SequenceNo == def.SequenceNo)
                                {
                                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DuplicateInvoice", "alert('Invoice already exists.')", true);
                                    return;
                                }
                            }
                        }
                        insertPaymentResult(def);
                    }

                    gv_Payment.DataSource = vwSearchResult;
                    gv_Payment.DataBind();
                    uploadCount = vwSearchResult.Count;
                    lbl_TotalSelected.Text = selectedCount.ToString();
                    pnl_TotalSelected.Visible = (uploadCount > 0);
                    pnl_Payment.Visible = true;
                }
                else if (list[0].GetType() == typeof(UKClaimDCNoteDef))
                {   // Next Claim debit note
                    if (vwDNSearchResult == null)
                        vwDNSearchResult = new ArrayList();
                    vwDNSearchResult.Insert(0, list[0]);
                    gv_DNPayment.DataSource = vwDNSearchResult;
                    gv_DNPayment.DataBind();
                    DNUploadCount = vwDNSearchResult.Count;
                    lbl_TotalDNSelected.Text = DNSelectedCount.ToString();
                    pnl_TotalDNSelected.Visible = (DNUploadCount > 0);
                    pnl_DNPayment.Visible = true;
                }
            }
            lbl_Msg.Visible = false;
            lbl_Error.Visible = false;
            txt_NSLInvNo.Focus();
        }

        protected void AddPaymentRecordByLCBillRefNo()
        {
            pnl_Result.Visible = true;

            int recordCount = 0;
            string lcBillRefNo = txt_LCBillRefNo.Text;
            ArrayList list =new ArrayList();
            if (txt_LCBillRefNo.Text.Trim() != string.Empty)
            {
                //get L/C shipment
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetInvoiceListByLcBillRefNo);
                Context.Items.Add(AccountCommander.Param.workflowStatusId, ContractWFS.INVOICED.Id);
                Context.Items.Add(AccountCommander.Param.lcBillRefNo, lcBillRefNo);
                forwardToScreen(null);
                if ((list = (ArrayList)Context.Items[AccountCommander.Param.invoiceList]) != null)
                    recordCount += list.Count;
            }

            if (list == null || list.Count == 0)
            {
                //invoice not found..
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invoiceNotFound", "alert('Invoice not found.')", true);
                return;
            }
            else
            {
                foreach (ContractShipmentListJDef def in list)
                {
                    bool recordExist = false;
                    if (vwSearchResult != null)
                    {
                        //check for duplicate entry..
                        foreach (ContractShipmentListJDef temp in vwSearchResult)
                            if (recordExist=(temp.InvoiceNo == def.InvoiceNo && temp.SequenceNo == def.SequenceNo))
                                break;
                    }
                    if (!recordExist)
                        insertPaymentResult(def);
                }
                gv_Payment.DataSource = vwSearchResult;
                gv_Payment.DataBind();
                uploadCount = vwSearchResult.Count;
                lbl_TotalSelected.Text = selectedCount.ToString();
                pnl_TotalSelected.Visible = (uploadCount > 0);
                pnl_Payment.Visible = true;
            }
            lbl_Msg.Visible = false;
            lbl_Error.Visible = false;
            if (txt_LCBillRefNo.Text.Trim() != string.Empty)
                txt_LCBillRefNo.Focus();
            else
                txt_NSLInvNo.Focus();
            txt_LCBillRefNo.Text = string.Empty;
            txt_NSLInvNo.Text = string.Empty;
        }

        protected void insertPaymentResult(ContractShipmentListJDef def)
        {
            if (gv_Payment.Columns[9].Visible && gv_Payment.Columns[12].Visible)
            {
                if (vwSearchResult == null)
                    vwSearchResult = new ArrayList();
                vwSearchResult.Insert(0, def);
            }
            else
            {
                gv_Payment.Columns[9].Visible = true;
                gv_Payment.Columns[10].Visible = true;
                gv_Payment.Columns[11].Visible = true;
                gv_Payment.Columns[12].Visible = true;
                gv_Payment.Columns[13].Visible = true;
                gv_Payment.Columns[14].Visible = true;
                gv_Payment.Columns[15].Visible = true;
                gv_Payment.Columns[16].Visible = true;
                gv_Payment.Columns[17].Visible = true;

                vwSearchResult.Clear();
                vwSearchResult.Insert(0, def);
            }
        }

        #endregion
    }
}