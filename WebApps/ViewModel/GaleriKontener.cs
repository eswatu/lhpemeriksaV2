using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApps.ViewModel
{
    public class GaleriKontener
    {
        public int ID { get; set; }
        public int DocNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime DocDate { get; set; }
        public Guid direktori { get; set; }
        public string petugas { get; set; }
        public string nomKontener { get; set; }
        public string displayedimage { get; set; }
    }
}