using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections ;
using CrystalDecisions.Shared;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.industry.vendor;
using com.next.infra.util;
using com.next.common.domain.types;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.account ;
using com.next.isam.reporter.paymentadvice;
using com.next.isam.domain.shipping;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.order;

namespace com.next.isam.appserver.account
{
    public class PaymentAdviceManager
    {
        private static PaymentAdviceManager _instance;

        private PaymentAdviceWorker worker;

        public PaymentAdviceManager()
        {
            worker = PaymentAdviceWorker.Instance;
        }

        public static PaymentAdviceManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PaymentAdviceManager();
                return _instance;
            }
        }

        public void importFile()
        {
            try
            {
                string paymentAdviceFolder = WebConfig.getValue("appSettings", "PAYMENT_ADVICE_UPLOAD_Folder") + @"archive\";

                ArrayList requestList = worker.getGenerateFileRequestList();

                foreach (GenerateFileRequestDef def in requestList)
                {
                    try
                    {
                        System.IO.FileInfo fi = new FileInfo(def.FileName);
                        if (fi.Extension.ToUpper() == ".CSV")
                        {
                            worker.uploadPaymentAdviceFile(def);

                            fi.MoveTo(paymentAdviceFolder + DateTime.Now.ToString("yyyyMMddhhmm_") + fi.Name);

                            MailHelper.sendGeneralMessage("PaymentAdvice Batch Uploaded Successfully (" + def.FileName + ")", "");
                        }
                    }
                    catch (Exception e)
                    {
                        MailHelper.sendErrorAlert(e, "");
                    }
                }
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public void generatePaymentAdvice()
        {
            PaymentAdviceReportDs.PaymentAdviceReportRow r = null;
            VendorRef vendor = null;
            string paymentAdviceOutputFolder = WebConfig.getValue("appSettings", "PAYMENT_ADVICE_OUTPUT_Folder");
            string eAdviceAddr = null;
            NTVendorDef ntVendor = null;

            try
            {
                PaymentAdviceReport rpt = new PaymentAdviceReport();
                PaymentAdviceReportDs ds = new PaymentAdviceReportDs();
                ArrayList paymentAdviceList = worker.getPaymentAdviceList();


                foreach (PaymentAdviceDef paymentAdviceDef in paymentAdviceList)
                {
                    vendor = null;
                    ntVendor = null;
                    ArrayList paymentAdviceDetailList = worker.getPaymentAdviceDetailList(paymentAdviceDef.PaymentAdviceId);
                    /* removed 2018-08-31
                    if (paymentAdviceDef.SUNSupplierId != string.Empty)
                        vendor = VendorWorker.Instance.getVendorBySUNSupplierId(paymentAdviceDef.SUNSupplierId);
                    */
                    if (paymentAdviceDef.EpicorSupplierId != string.Empty)
                    {
                        if (paymentAdviceDef.EpicorSupplierId != string.Empty && paymentAdviceDef.EpicorSupplierId.EndsWith("N"))
                        {
                            ntVendor = NonTradeManager.Instance.getNTVendorByEPVendorCode(paymentAdviceDef.EpicorSupplierId, true);
                            if (ntVendor == null)
                                ntVendor = NonTradeManager.Instance.getNTVendorByEPVendorCode(paymentAdviceDef.EpicorSupplierId, false);
                        }
                        else
                        {
                            TypeCollector vendorTypes = TypeCollector.Inclusive;
                            vendorTypes.append(com.next.common.domain.industry.types.VendorType.FABRIC.Id);
                            vendorTypes.append(com.next.common.domain.industry.types.VendorType.GARMENT.Id);
                            vendorTypes.append(com.next.common.domain.industry.types.VendorType.NONCLOTHING.Id);
                            vendor = VendorWorker.Instance.getVendorByEpicorSupplierId(paymentAdviceDef.EpicorSupplierId, vendorTypes);
                        }
                    }

                    foreach (PaymentAdviceDetailDef detail in paymentAdviceDetailList)
                    {
                        r = ds.PaymentAdviceReport.NewPaymentAdviceReportRow();                        
                        r.PaymentAdviceId = detail.PaymentAdviceId;
                        r.SUNSupplierId = paymentAdviceDef.SUNSupplierId;
                        if (vendor != null && !vendor.Name.ToLower().Contains("do not use"))
                            r.SupplierName = vendor.Name;
                        else
                            r.SupplierName = paymentAdviceDef.SupplierName;
                        r.PayDate = paymentAdviceDef.PayDate;
                        r.BankName = paymentAdviceDef.BankName;
                        r.ManufacturerInvoiceNo = detail.ManufacturerInvoiceNo;
                        r.PONo = detail.PONo;
                        r.Amount = detail.Amount;
                        r.Currency = detail.Currency;
                        r.TransactionDate = detail.TransactionDate;
                        r.RefNo = detail.RefNo;

                        if (detail.RefNo == string.Empty)
                        {
                            if (ShippingWorker.isValidInvoiceNo(detail.PONo))
                            {
                                InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByInvoiceNo(ShippingWorker.getInvoicePrefix(detail.PONo), ShippingWorker.getInvoiceSeq(detail.PONo), ShippingWorker.getInvoiceYear(detail.PONo));
                                ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(invoiceDef.ShipmentId);
                                ContractDef contractDef = OrderSelectWorker.Instance.getContractByShipmentId(invoiceDef.ShipmentId);
                                r.RefNo = contractDef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString();
                            }
                        }

                        ds.PaymentAdviceReport.AddPaymentAdviceReportRow(r);
                    }
                    

                    rpt.SetDataSource(ds);
                    rpt.SetParameterValue("CompanyHeader", "NEXT SOURCING LIMITED");
                    rpt.SetParameterValue("ReportHeader", "Payment Advice");
                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, paymentAdviceOutputFolder + paymentAdviceDef.PaymentAdviceId.ToString() + ".pdf");
                    rpt.Close();

                    if (vendor != null)
                    {
                        eAdviceAddr = string.Empty;

                        if (vendor.eAdviceAddr != null)
                            eAdviceAddr = vendor.eAdviceAddr.Trim();

                        if (eAdviceAddr != string.Empty)
                        {                            
                            NoticeHelper.sendPaymentAdvice(vendor, paymentAdviceOutputFolder + paymentAdviceDef.PaymentAdviceId.ToString() + ".pdf", paymentAdviceDef.IsChecked, paymentAdviceDef.Company, paymentAdviceDef);                            
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("e-Advice mail undefined");
                            NoticeHelper.sendPaymentAdviceMissingSupplierEmail(vendor, paymentAdviceOutputFolder + paymentAdviceDef.PaymentAdviceId.ToString() + ".pdf", paymentAdviceDef.Company);
                        }
                    }
                    else if (paymentAdviceDef.EpicorSupplierId != string.Empty && paymentAdviceDef.EpicorSupplierId.EndsWith("N"))
                    {
                        NoticeHelper.sendNTPaymentAdvice(paymentAdviceDef.SupplierName, paymentAdviceOutputFolder + paymentAdviceDef.PaymentAdviceId.ToString() + ".pdf", paymentAdviceDef.IsChecked, paymentAdviceDef.Company, ntVendor.EAdviceEmail.Trim());
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("vendor not found");
                        NoticeHelper.sendPaymentAdviceMissingSupplierEmail(paymentAdviceDef.SupplierName, paymentAdviceOutputFolder + paymentAdviceDef.PaymentAdviceId.ToString() + ".pdf", paymentAdviceDef.Company);
                    }

                    paymentAdviceDef.Mailed = true;
                    worker.updatePaymentAdvice(paymentAdviceDef);
                    
                    ds.PaymentAdviceReport.Clear();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public ArrayList getGenerateFileRequestList()
        {
            return worker.getGenerateFileRequestList();
        }

        public void markPaymentAdviceAsChecked()
        {
            ArrayList paymentAdviceList = worker.getPaymentAdviceByCriteria(DateTime.Today, "AP-TT");

            foreach (PaymentAdviceDef def in paymentAdviceList)
            {
                if (def.IsChecked == 0 && def.Company.StartsWith("NS"))
                {
                    def.Mailed = false;
                    def.IsChecked = 1;
                    worker.updatePaymentAdvice(def);
                }
            }
        }

        public void updateGenerateFileRequest(GenerateFileRequestDef def)
        {
            worker.updateGenerateFileRequest(def);
        }
    }
}
