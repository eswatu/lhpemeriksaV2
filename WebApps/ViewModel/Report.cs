using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApps.ViewModel
{
    public class Report
    {
        public string namaPegawai { get; set; }
        public int jmlDoc { get; set; }
        public int jmlDss { get; set; }
        public int jmlDsc { get; set; }
        public int jmlDts { get; set; }
        public double percentTS { get { return Math.Round(((double)jmlDts / jmlDoc)*100,2); }}
    }
}