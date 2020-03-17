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

        [Display(Name ="Tanggal Pelaksanaan")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [StringLength(80)]
        [Display(Name = "Nama Kegiatan")]
        public String EventTitle { get; set; }
        //di bawah untuk sistem
        public String direktori { get; set; }

        [Display(Name ="Deskripsi Kegiatan")]
        public string deskripsi { get; set; }
        
        public virtual ICollection<ImgDetail> ImgDetail { get; set; }


    }
}