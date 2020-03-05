using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApps.Models
{
    public class DocImages
    {
        public DocImages()
        {
            ImgDetail = new List<ImgDetail>();
        }

        [Key,Required]
        public int DocImagesID { get; set;}
        public Guid direktori { get; set; }

        [Required, StringLength(11,ErrorMessage ="nomor kontainer Tidak Valid")]
        public string nomorKontainer { get; set; }

        [Required]
        public int DocID { get; set; }
        public virtual Document Document { get; set; }

        public virtual ICollection<ImgDetail> ImgDetail { get; set; }


    }
}