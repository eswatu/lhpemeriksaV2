using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApps.Models
{
    public class Document
    {
        public Document() {
                    DocDate = DateTime.Today;
                    DocImages = new List<DocImages>();
                }

        [Key]
        public int DocID { get; set; }

        [Display(Name ="Nomor Dokumen")]
        public int DocNumber { get; set; }

        [Column(TypeName = "datetime2"), Display(Name ="Tanggal Dokumen")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DocDate { get; set; }
        [Display(Name ="Nama Importir")]
        public string DocOwner { get; set; }

        [Column(TypeName = "datetime2"), Display(Name ="Tanggal Pelaporan")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DocWorkDate { get; set; }
        public docStatus Status { get; set; }
        public string Keterangan { get; set; }


        [Display(Name ="Petugas")]
        public int WorkerID { get; set; }
        public virtual Worker Worker { get; set; }

        public virtual ICollection<DocImages> DocImages { get; set; }

    }

    public enum docStatus 
    {
        belum_laporan = 0,
        sesuai,
        sesuai_catatan,
        tidak_sesuai
    }

}