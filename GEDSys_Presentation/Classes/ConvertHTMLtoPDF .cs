using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.html.table;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.parser;

namespace ERP_Condominios_Solution.Classes
{
    public class PdfCreator
    {

        public string ConvertHtmlToPdf(string htmlContent, string fileNameWithoutExtension, string filePath, string cssContent = "")
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var fileNameWithPath = Path.Combine(filePath, fileNameWithoutExtension + ".pdf");

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                using (var document = new Document())
                {
                    var writer = PdfWriter.GetInstance(document, stream);
                    document.Open();
                    document.NewPage();
                    document.Add(new Chunk(""));

                    // instantiate custom tag processor and add to `HtmlPipelineContext`.
                    var tagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
                    tagProcessorFactory.AddProcessor(new TableData(), new string[] { HTML.Tag.TD });
                    var htmlPipelineContext = new HtmlPipelineContext(null);
                    htmlPipelineContext.SetTagFactory(tagProcessorFactory);

                    var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
                    var htmlPipeline = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);

                    // get an ICssResolver and add the custom CSS
                    var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                    cssResolver.AddCss(cssContent, "utf-8", true);
                    var cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);

                    var worker = new XMLWorker(cssResolverPipeline, true);
                    var parser = new XMLParser(worker);
                    using (var stringReader = new StringReader(htmlContent))
                    {
                        parser.Parse(stringReader);
                    }
                }
            }
            return fileNameWithPath;
        }
    }
}