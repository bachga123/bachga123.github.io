using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoginWebsite.Models;
using System.IO;

namespace LoginWebsite.Models
{
    public class File
    {
            public int FileId { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public byte[] Content { get; set; }
            public FileType FileType { get; set; }
            public int UserId { get; set; }
            public virtual User User { get; set; }
    }
    
}