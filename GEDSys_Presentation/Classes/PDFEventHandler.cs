    using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using QRCoder;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.Classes
{
    public class CustomPageEventHelper : PdfPageEventHelper
    {
        private readonly PdfPTable _headerTable;
        private readonly PdfPTable _footerTable;

        public CustomPageEventHelper(PdfPTable headerTable, PdfPTable footerTable)
        {
            _headerTable = headerTable;
            _footerTable = footerTable;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {

            //// Add page number to the footer
            //PdfPCell pageNumberCell = new PdfPCell(new Phrase($"Page {writer.PageNumber}", new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL)));
            //pageNumberCell.Border = PdfPCell.NO_BORDER;
            //pageNumberCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //_headerTable.AddCell(pageNumberCell);

            // Write the header
            _headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            _headerTable.WidthPercentage = 100;
            _headerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - document.TopMargin + _headerTable.TotalHeight, writer.DirectContent);


            // Write the footer
            _footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            _footerTable.WidthPercentage = 100;
            _footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin - 10, writer.DirectContent);
        }
    }
}