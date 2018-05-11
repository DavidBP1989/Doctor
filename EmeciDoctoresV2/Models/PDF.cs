using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Web;

namespace EmeciDoctoresV2.Models
{
    public class PDF
    {
        public PDF()
        {

        }

        public byte[] Img_access(string fileImg, string msj)
        {
            Bitmap bitImg = new Bitmap(fileImg);
            Graphics gImg = Graphics.FromImage(bitImg);

            gImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            gImg.DrawString(msj, new Font("Arial", 20, FontStyle.Bold), SystemBrushes.WindowText, new Point(50, 175));

            var ms = new System.IO.MemoryStream();
            bitImg.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }
}