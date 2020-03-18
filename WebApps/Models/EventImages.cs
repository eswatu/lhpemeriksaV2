using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column(TypeName = "datetime2"), Display(Name = "Tanggal Kegiatan")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
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