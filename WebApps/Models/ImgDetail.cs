using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApps.Models
{
    public class ImgDetail
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string locationPath { get; set; }
        public int docImagesID { get; set; }
        public virtual DocImages DocImages { get; set; }
        
    }
}