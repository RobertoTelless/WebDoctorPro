using System;
using System.Collections.Generic;

namespace CrossCutting
{
    public class AttachmentModel
    {
        public string PATH { get; set; }
        public string ATTACHMENT_NAME { get; set; }
        public string CONTENT_TYPE { get; set; }
        public string ContentBytes { get; set; }
        public List<AttachmentModel> Attachments { get; set; }
    }
}
