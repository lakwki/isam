using System;
using com.next.common.domain;
using System.Drawing;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using com.next.common.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class NSLedRangePlanDef : DomainData
    {
        public int RangePlanId { get; set; }
        public string ItemNo { get; set; }
        public int OfficeId { get; set; }
        public string Description { get; set; }
        public string ProductTeamName { get; set; }
        public string UKDepartment { get; set; }
        public int SeasonId { get; set; }
        public string ActualSaleSeason { get; set; }
        public string ExpectedSaleSeason { get; set; }
        public int TotalQty { get; set; }
        public decimal TotalFOBCost { get; set; }
        public decimal TotalFOBCostInUSD { get; set; }
        public decimal USDExchangeRate { get; set; }
        public decimal GBPExchangeRate { get; set; }
        public decimal SupplierCurrencyExchangeRate { get; set; }
        public int SupplierCurrencyId { get; set; }
        public int CustomerId { get; set; }
        public decimal MinRetailSellingPrice { get; set; }
        public decimal MaxRetailSellingPrice { get; set; }
        public decimal TotalRetailAmt { get; set; }
        public decimal TotalComm { get; set; }
        public decimal TotalFreight { get; set; }
        public decimal ActualFreightUSD { get; set; }
        public decimal ActualFreightUnitCostUSD { get; set; }
        public decimal DutyPercent { get; set; }
        public byte[] Picture { get; set; }
        public DateTime FirstInvoiceDate { get; set; }
        public int FirstShipmentId { get; set; }
        public DateTime ActualLaunchedDate { get; set; }
        public DateTime PlannedLaunchedDate { get; set; }
        public string Comment { get; set; }
        public string SellThruRemark { get; set; }
        public int IsDuty { get; set; }
        public int ShippedPeriod { get; set; }
        public int CountryOfOriginId { get; set; }

        public static string getCustomerText(int customerId)
        {
            if (customerId == 1)
                return "Online Only";
            else if (customerId == 2)
                return "Retail + Online";
            else if (customerId == 3)
                return "Retail Only";
            else
                return "N/A";
        }

        public static byte[] ResizeImage(byte[] ary, int maxWidth, int maxHeight)
        {
            System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(ary, 0, ary.Length));

            if (img.Height < maxHeight && img.Width < maxWidth)
            {
                MemoryStream ms = new MemoryStream();
                img.Save(ms, img.RawFormat);
                return ms.ToArray();
            }
            using (img)
            {
                Double xRatio = (double)img.Width / maxWidth;
                Double yRatio = (double)img.Height / maxHeight;
                Double ratio = Math.Max(xRatio, yRatio);
                int nnx = (int)Math.Floor(img.Width / ratio);
                int nny = (int)Math.Floor(img.Height / ratio);
                Bitmap cpy = new Bitmap(nnx, nny, PixelFormat.Format32bppArgb);
                using (Graphics gr = Graphics.FromImage(cpy))
                {
                    gr.Clear(Color.Transparent);

                    // This is said to give best quality when resizing images
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    gr.DrawImage(img,
                        new Rectangle(0, 0, nnx, nny),
                        new Rectangle(0, 0, img.Width, img.Height),
                        GraphicsUnit.Pixel);
                }
                MemoryStream ms = new MemoryStream();
                cpy.Save(ms, img.RawFormat);
                return ms.ToArray();
            }

        }

    }
}
