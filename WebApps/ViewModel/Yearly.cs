using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApps.ViewModel
{
    public class Yearly
    {
        public int nmrBulan { get; set; }
        public int jmlDoc { get; set; }
        public int jmlDss { get; set; }
        public int jmlDsc { get; set; }
        public int jmlDts { get; set; }
        public double percentTS { get { return Math.Round(((double)jmlDts / jmlDoc)*100,2); }}
    }
}