using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApps.Models
{
    public class EventImages
    {
        public EventImages()
        {
            ImgDetail = new List<ImgDetail>();
        }

        [Key,Required]
        public int EventID { get; set;}
        public Guid direktori { get; set; }

        public string deskripsi { get; set; }
        
        public virtual ICollection<ImgDetail> ImgDetail { get; set; }


    }
}