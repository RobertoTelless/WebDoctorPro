using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using QRCoder;
using System.Drawing;
using System.IO;
using iTextSharp.text;           // Para as classes Document e Element
using iTextSharp.text.pdf;

namespace ERP_Condominios_Solution.Classes
{
    public class WatermarkEvent : PdfPageEventHelper
    {
        private string _text;
        public WatermarkEvent(string text) { _text = text; }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            // Define a fonte (suave e grande)
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            PdfContentByte cb = writer.DirectContentUnder; // Desenha por baixo do conteúdo principal

            cb.SaveState();
            cb.SetColorFill(new BaseColor(210, 210, 210)); // Cinza claro para ser suave
            cb.SetFontAndSize(bf, 45); // Tamanho grande

            // Desenha o texto centralizado e rotacionado (45 graus)
            cb.BeginText();
            float x = (document.PageSize.Left + document.PageSize.Right) / 2;
            float y = (document.PageSize.Top + document.PageSize.Bottom) / 2;
            cb.ShowTextAligned(Element.ALIGN_CENTER, _text, x, y, 45);
            cb.EndText();

            cb.RestoreState();
        }
    }
}