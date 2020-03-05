using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApps.Models
{
    public class Worker
    {
        public Worker()
        {
            Documents = new List<Document>();
        }

        [Key,Required]
        public int WorkerID { get; set; }
        [StringLength(50),Display(Name ="Nama Pegawai")]
        public string WorkerName { get; set; }
        [Display(Name ="Pegawai Aktif?")]
        public bool WorkerActive { get; set; }
        [Display(Name ="Pegawai Cuti?")]
        public bool WorkerFree { get; set; }

        public ICollection<Document> Documents { get; set; }

    }
}