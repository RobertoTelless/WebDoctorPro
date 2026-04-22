using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using QRCoder;
using System.Drawing;
using System.IO;

namespace ERP_Condominios_Solution.Classes
{
    public class QrCodeHelper
    {
        public static string GenerateQrCodeAndSave(String url, String filePath)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                // Create QR code data from the URL
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

                // Create QR code from data
                using (var qrCode = new QRCode(qrCodeData))
                {
                    // Render the QR code as an image
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                    {
                        // Save the image to the specified file path as PNG
                        qrCodeImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                        return filePath;
                    }
                }
            }
        }

        // Novo método para retornar os bytes da imagem
        public static byte[] GenerateQrCodeBytes(string url)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            return ms.ToArray();
                        }
                    }
                }
            }
        }
    }
}