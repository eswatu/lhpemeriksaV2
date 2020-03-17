using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApps.Models;
using WebApps.ViewModel;
using PagedList;

namespace WebApps.Controllers
{
    public class DocumentController : Controller
    {
        private LHPContext db = new LHPContext();


        // GET: Document
        // GET: Document
        [Authorize]
        public ActionResult Index(int? pegawe, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DocNumber = (string.IsNullOrEmpty(sortOrder) || sortOrder == "DocNumber_Desc") ? "DocNumber" : "DocNumber_Desc";
            ViewBag.DocDate = sortOrder == "DocDate" ? "DocDate_Desc" : "DocDate";
            ViewBag.DocOwner = sortOrder == "DocOwner" ? "DocOwner_Desc" : "DocOwner";
            ViewBag.DocWorkDate = sortOrder == "DocWorkDate" ? "DocWorkDate_Desc" : "DocWorkDate";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.dtpeg = pegawe;
            var documents = db.Documents.Include(d => d.Worker);
            ViewBag.nmPegawe = "Semua Pegawai";
            if (pegawe != null) {
                documents = documents.Where(d => d.WorkerID == pegawe);
                string nama = db.Workers.Find(pegawe).WorkerName;
                ViewBag.nmPegawe = nama;
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                documents = documents.Where(d => d.DocOwner.Contains(searchString) || d.Keterangan.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "DocNumber_Desc":
                    documents = documents.OrderByDescending(d => d.DocNumber);
                    break;
                case "DocNumber":
                    documents = documents.OrderBy(d => d.DocNumber);
                    break;
                case "DocDate":
                    documents = documents.OrderBy(d => d.DocDate);
                    break;
                case "DocDate_Desc":
                    documents = documents.OrderByDescending(d => d.DocDate);
                    break;
                case "DocOwner":
                    documents = documents.OrderBy(d => d.DocOwner);
                    break;
                case "DocOwner_Desc":
                    documents = documents.OrderByDescending(d => d.DocOwner);
                    break;
                case "DocWorkDate":
                    documents = documents.OrderBy(d => d.DocWorkDate);
                    break;
                case "DocWorkDate_Desc":
                    documents = documents.OrderByDescending(d => d.DocWorkDate);
                    break;
                default:
                    documents = documents.OrderBy(d => d.DocNumber);
                    break;
            }
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(documents.ToPagedList(pageNumber, pageSize));
        }

        // GET: Document/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Document/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.WorkerID = new SelectList(db.Workers.Where(w => (w.WorkerActive == true && w.WorkerFree == false)).OrderBy(w => w.WorkerName), "WorkerID", "WorkerName");
            Document document = new Document();
            return View(document);
        }

        // POST: Document/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "DocNumber,DocDate,DocOwner,DocWorkDate,WorkerID,Status,Keterangan")] Document document)
        {
            ViewBag.WorkerID = new SelectList(db.Workers.Where(w => (w.WorkerActive == true && w.WorkerFree == false)).OrderBy(w => w.WorkerName), "WorkerID", "WorkerName");

            if (ModelState.IsValid)
            {
                db.Documents.Add(document);
                db.SaveChanges();
                return RedirectToAction("ReviewHarian");
            }

            return View(document);
        }

        // GET: Document/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            ViewBag.WorkerID = new SelectList(db.Workers.Where(w => (w.WorkerActive == true && w.WorkerFree == false)).OrderBy(w => w.WorkerName), "WorkerID", "WorkerName",document.WorkerID);
            return View(document);
        }

        // POST: Document/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "DocID,DocNumber,DocDate,DocOwner,DocWorkDate,WorkerID,Status,Keterangan")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ReviewHarian");
            }
            return View(document);
        }

        // GET: Document/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Document/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Document document = db.Documents.Find(id);
            db.Documents.Remove(document);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //sesi laporan

        //GET: Review Harian Dokumen
        public ActionResult ReviewHarian()
        {
            var tabelDoc = db.Documents.Where(doc => doc.DocWorkDate == DateTime.Today).ToList();
            return View(tabelDoc);
        }
        //GET: Lihat dokumen dengan tanggal spesifik
        public ActionResult LaporanHarian(string tgl)
        {
            DateTime reportDate;
            if (String.IsNullOrEmpty(tgl))
            {
                reportDate = DateTime.Now;
            }
            else
            {
                reportDate = DateTime.Parse(tgl);
            }
            var tabelDoc = db.Documents.Where(doc => doc.DocWorkDate == reportDate).ToList();
            return View(tabelDoc);
        }

        // GET: Document laporan per bulan (int)
        public ActionResult LaporanBulan(int? bulan, int? tahun)
        {
            if (bulan == null)
            {
                bulan = DateTime.Now.Month;
            }
            if (tahun == null || tahun == 0)
            {
                tahun = DateTime.Now.Year;
            }
            string kodebulan;
            switch (bulan)
            {
                case 1:
                    kodebulan = "Januari " + tahun.ToString();
                    break;
                case 2:
                    kodebulan = "Februari " + tahun.ToString();
                    break;
                case 3:
                    kodebulan = "Maret " + tahun.ToString();
                    break;
                case 4:
                    kodebulan = "April " + tahun.ToString();
                    break;
                case 5:
                    kodebulan = "Mei " + tahun.ToString();
                    break;
                case 6:
                    kodebulan = "Juni " + tahun.ToString();
                    break;
                case 7:
                    kodebulan = "Juli " + tahun.ToString();
                    break;
                case 8:
                    kodebulan = "Agustus " + tahun.ToString();
                    break;
                case 9:
                    kodebulan = "September " + tahun.ToString();
                    break;
                case 10:
                    kodebulan = "Oktober " + tahun.ToString();
                    break;
                case 11:
                    kodebulan = "Nopember " + tahun.ToString();
                    break;
                case 12:
                    kodebulan = "Desember " + tahun.ToString();
                    break;
                default:
                    kodebulan = "-";
                    break;
            }
            ViewBag.nama = kodebulan;
            var docs = db.Documents.Where(d => d.DocWorkDate.Month == bulan && d.DocWorkDate.Year == tahun);

            var docAll = from mthrep in docs
                         group mthrep by mthrep.Worker.WorkerName into dp
                         orderby dp.Key ascending
                         select new Report
                         {
                             namaPegawai = dp.Key,
                             jmlDoc = dp.Count(),
                             jmlDts = dp.Count(d => d.Status == docStatus.tidak_sesuai),
                             jmlDsc = dp.Count(d => d.Status == docStatus.sesuai_catatan),
                             jmlDss = dp.Count(d => d.Status == docStatus.sesuai)
                         };
            return View(docAll.ToList());
        }
        public ActionResult LaporanTriwulan(int? tw, int? tahun)
        {

            if (tahun == null || tahun == 0)
            {
                tahun = DateTime.Now.Year;
            }
            int[] daftar = new int[2];
            string kodetriwulan;
            switch (tw)
            {
                case 1:
                    kodetriwulan = "Triwulan I tahun " + tahun.ToString();
                    daftar = new int[] { 1, 3 };
                    break;
                case 2:
                    kodetriwulan = "Triwulan II tahun " + tahun.ToString();
                    daftar = new int[] { 4, 6 };
                    break;
                case 3:
                    kodetriwulan = "Triwulan III tahun " + tahun.ToString();
                    daftar = new int[] { 7, 9 };
                    break;
                case 4:
                    kodetriwulan = "Triwulan IV tahun " + tahun.ToString();
                    daftar = new int[] { 10, 12 };
                    break;
                default:
                    kodetriwulan = "-";
                    daftar = new int[] { 1, 3 };
                    break;
            }
            ViewBag.nama = kodetriwulan;
            int a = daftar[0];
            int b = daftar[1];
            var docs = db.Documents.Where(d => d.DocWorkDate.Month >= a && d.DocWorkDate.Month <= b && d.DocWorkDate.Year == tahun);

            var docAll = from mthrep in docs
                         group mthrep by mthrep.Worker.WorkerName into dp
                         orderby dp.Key ascending
                         select new Report
                         {
                             namaPegawai = dp.Key,
                             jmlDoc = dp.Count(),
                             jmlDts = dp.Count(d => d.Status == docStatus.tidak_sesuai),
                             jmlDsc = dp.Count(d => d.Status == docStatus.sesuai_catatan),
                             jmlDss = dp.Count(d => d.Status == docStatus.sesuai)
                         };
            return View(docAll.ToList());
        }
        public ActionResult LaporanSemester(int? smt, int? tahun)
        {
            if (smt == null)
            {
                smt = 1;
            }
            if (tahun == null || tahun == 0)
            {
                tahun = DateTime.Now.Year;
            }
            string kodesmt;
            int[] daftar = new int[2];
            switch (smt)
            {
                case 1:
                    kodesmt = "Semester I tahun " + tahun.ToString();
                    daftar = new int[] { 1, 6 };
                    break;
                case 2:
                    kodesmt = "Semester II tahun " + tahun.ToString();
                    daftar = new int[] { 7, 12 };
                    break;
                default:
                    kodesmt = "-";
                    daftar = new int[] { 1, 6 };
                    break;
            }
            ViewBag.nama = kodesmt;
            int a = daftar[0];
            int b = daftar[1];
            var docs = db.Documents.Where(d => d.DocWorkDate.Month >= a && d.DocWorkDate.Month <= b && d.DocWorkDate.Year == tahun);

            var docAll = from mthrep in docs
                         group mthrep by mthrep.Worker.WorkerName into dp
                         orderby dp.Key ascending
                         select new Report
                         {
                             namaPegawai = dp.Key,
                             jmlDoc = dp.Count(),
                             jmlDts = dp.Count(d => d.Status == docStatus.tidak_sesuai),
                             jmlDsc = dp.Count(d => d.Status == docStatus.sesuai_catatan),
                             jmlDss = dp.Count(d => d.Status == docStatus.sesuai)
                         };
            return View(docAll.ToList());
        }
        public ActionResult LaporanTahun(int? tahun)
        {
            if (tahun == null || tahun == 0)
            {
                tahun = DateTime.Now.Year;
            }
            string kodetahun = "Laporan Per Pemeriksa Tahun ";
            ViewBag.nama = kodetahun;

            var docs = db.Documents.Where(d => d.DocWorkDate.Year == tahun);

            var docAll = from mthrep in docs
                         group mthrep by mthrep.Worker.WorkerName into dp
                         orderby dp.Key ascending
                         select new Report
                         {
                             namaPegawai = dp.Key,
                             jmlDoc = dp.Count(),
                             jmlDts = dp.Count(d => d.Status == docStatus.tidak_sesuai),
                             jmlDsc = dp.Count(d => d.Status == docStatus.sesuai_catatan),
                             jmlDss = dp.Count(d => d.Status == docStatus.sesuai)
                         };
            return View(docAll.ToList());
        }

        //GET Tahunan, untuk default di tampilan utama
        public ActionResult YearlyIndex()
        {
            IQueryable<Yearly> tabelDoc = from docs in db.Documents.Where(d => d.DocWorkDate.Year == DateTime.Now.Year)
                                          group docs by docs.DocWorkDate.Month into dp
                                          orderby dp.Key ascending
                                          select new Yearly
                                          {
                                              nmrBulan = dp.Key,
                                              jmlDoc = dp.Count(),
                                              jmlDts = dp.Count(d => d.Status == docStatus.tidak_sesuai),
                                              jmlDsc = dp.Count(d => d.Status == docStatus.sesuai_catatan),
                                              jmlDss = dp.Count(d => d.Status == docStatus.sesuai)
                                          };
            return View(tabelDoc);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
