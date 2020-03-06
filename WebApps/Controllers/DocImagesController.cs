using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebApps.Models;
using WebApps.ViewModel;
using PagedList;


namespace WebApps.Controllers
{
    public class DocImagesController : Controller
    {
        private LHPContext db = new LHPContext();

        private string basepath = "~\\App_Data\\Uploads\\";
        private int CurrentYear = DateTime.Now.Year;
        private string PrepareFolder(Guid lastdir)
        {
            string dirPath = GetDir(lastdir);
            if (!Directory.Exists(Server.MapPath(dirPath)))
            {
                Directory.CreateDirectory(Server.MapPath(dirPath));
            } 
            return dirPath;
        }
        private string SetCurrentFolder(Guid lastdir)
        {
            string dirPath = GetDir(lastdir);
            return dirPath;
        }

        private string GetFolder(int DocID)
        {
            string fm = "000000.##";

            string folder = DocID.ToString(fm);
            string dirPath = Path.Combine(basepath,CurrentYear.ToString(), folder + "\\");
            return dirPath;
        }
        // GET: DocImages
        public ActionResult Index(int? pegawe, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DocNumber = (string.IsNullOrEmpty(sortOrder) || sortOrder == "DocNumber_Desc") ? "DocNumber" : "DocNumber_Desc";
            ViewBag.DocDate = sortOrder == "DocDate" ? "DocDate_Desc" : "DocDate";
            ViewBag.Petugas = sortOrder == "Petugas" ? "Petugas_Desc" : "Petugas";
            //ViewBag.DocWorkDate = sortOrder == "DocWorkDate" ? "DocWorkDate_Desc" : "DocWorkDate";
            //untuk query search
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
            ViewBag.nmPegawe = "Semua Pegawai";

            var documents = db.DocImages
                .Include(i => i.ImgDetail)
                .Include( i => i.Document);

            if (pegawe != null)
            {
                documents = documents.Where(d => d.Document.WorkerID == pegawe);
                string nama = db.Workers.Find(pegawe).WorkerName;
                ViewBag.nmPegawe = nama;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                documents = documents.Where(d => d.nomorKontainer.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "DocNumber_Desc":
                    documents = documents.OrderByDescending(d => d.Document.DocNumber);
                    break;
                case "DocNumber":
                    documents = documents.OrderBy(d => d.Document.DocNumber);
                    break;
                case "DocDate":
                    documents = documents.OrderBy(d => d.Document.DocDate);
                    break;
                case "DocDate_Desc":
                    documents = documents.OrderByDescending(d => d.Document.DocDate);
                    break;
                case "Petugas":
                    documents = documents.OrderBy(d => d.Document.Worker.WorkerName);
                    break;
                case "Petugas_Desc":
                    documents = documents.OrderByDescending(d => d.Document.Worker.WorkerName);
                    break;
                //case "DocWorkDate":
                //    documents = documents.OrderBy(d => d.DocWorkDate);
                //    break;
                //case "DocWorkDate_Desc":
                //    documents = documents.OrderByDescending(d => d.DocWorkDate);
                //    break;
                default:
                    documents = documents.OrderBy(d => d.DocImagesID);
                    break;
            }
            IQueryable<GaleriKontener> gk = from galeri in documents
                                            select new GaleriKontener
                                            {
                                                ID = galeri.DocImagesID,
                                                DocNumber = galeri.Document.DocNumber,
                                                DocDate = galeri.Document.DocDate,
                                                petugas = galeri.Document.Worker.WorkerName,
                                                nomKontener = galeri.nomorKontainer,
                                                direktori = galeri.direktori,
                                                displayedimage = galeri.ImgDetail.FirstOrDefault().FileName
                                            };
            //var docImages = db.DocImages.Include(d => d.Document);
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(gk.ToPagedList(pageNumber, pageSize));
        }

        // GET: DocImages
        public ActionResult LaporanHarian(string waktu)
        {
            DateTime reportDate = DateTime.Now.Date;
            if (waktu != null)
            {
                reportDate = DateTime.Parse(waktu).Date;
            }
          

            var documents = db.DocImages
                .Include(i => i.ImgDetail)
                .Include(i => i.Document);

            IQueryable<GaleriKontener> gk = from galeri in documents.Where(d => d.Document.DocWorkDate == reportDate)
                                            select new GaleriKontener
                                            {
                                                ID = galeri.DocImagesID,
                                                DocNumber = galeri.Document.DocNumber,
                                                DocDate = galeri.Document.DocDate,
                                                petugas = galeri.Document.Worker.WorkerName,
                                                nomKontener = galeri.nomorKontainer,
                                                direktori = galeri.direktori,
                                                displayedimage = galeri.ImgDetail.FirstOrDefault().FileName
                                            };
            //var docImages = db.DocImages.Include(d => d.Document);

            return View(gk.ToList());
        }

        // GET: DocImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DocImages docImage = db.DocImages.Include(s => s.ImgDetail).SingleOrDefault(x => x.DocImagesID == id);

            if (docImage == null)
            {
                return HttpNotFound();
            }

            foreach (var item in docImage.ImgDetail)
            {
                item.FileName = GetPath(docImage.direktori, item.FileName);
            }
 
            return View(docImage);
        }

        // GET: DocImages/Create
        [Authorize]
        public ActionResult Create(int? docid)
        {

            var dateCriteria = DateTime.Now.Date.AddDays(-4);

            if (docid != null)
            {
                ViewBag.DocID = new SelectList(db.Documents.Where(d => (d.DocWorkDate > dateCriteria)), "DocID", "DocNumber",docid);
            }
            else {
                ViewBag.DocID = new SelectList(db.Documents.Where(d => (d.DocWorkDate > dateCriteria)), "DocID", "DocNumber");

            }
            return View();
        }

        // POST: DocImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(DocImages docImage)
        {
            if (ModelState.IsValid)
            {
                Guid fpath = Guid.NewGuid();
                string dirPath = PrepareFolder(fpath);

                List<ImgDetail> fileDetails = new List<ImgDetail>();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        ImgDetail imgDetail = new ImgDetail()
                        {
                            FileName = fileName,
                            Extension = Path.GetExtension(fileName),
                            Id = Guid.NewGuid(),
                            locationPath = Path.Combine(dirPath, fileName)
                        };
                        fileDetails.Add(imgDetail);
                        var path = Server.MapPath(Path.Combine(dirPath,imgDetail.FileName));
                        file.SaveAs(path);
                    }

                }

                docImage.ImgDetail = fileDetails;
                db.DocImages.Add(docImage);
                docImage.direktori = fpath;
                //bagian save /update dokumen
                var selectDoc = db.Documents.Find(docImage.DocID);
                db.Entry(selectDoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var dateCriteria = DateTime.Now.Date.AddDays(-4);
            ViewBag.DocID = new SelectList(db.Documents.Where(d => (d.DocWorkDate > dateCriteria)), "DocID", "DocNumber");
            return View(docImage);
        }

        // GET: DocImages/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocImages docImage = db.DocImages.Include(s => s.ImgDetail)
                .Include(t => t.Document)
                .SingleOrDefault(x => x.DocImagesID == id);

            if (docImage == null)
            {
                return HttpNotFound();
            }
            var dateCriteria = DateTime.Now.Date.AddDays(-4);
            ViewBag.DocID = new SelectList(db.Documents.Where(d => (d.DocWorkDate > dateCriteria)), "DocID", "DocNumber",docImage.DocID);
            return View(docImage);
        }

        // POST: DocImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(DocImages docImage)
        {
            string dirPath = SetCurrentFolder(docImage.direktori);
            if (ModelState.IsValid)
            {
                List<ImgDetail> filedetails = new List<ImgDetail>();
                //New Files
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        ImgDetail fileDetail = new ImgDetail()
                        {
                            FileName = fileName,
                            Extension = Path.GetExtension(fileName),
                            Id = Guid.NewGuid(),
                            docImagesID = docImage.DocImagesID,
                            locationPath = Path.Combine(Server.MapPath(dirPath), fileName)
                        };
                        filedetails.Add(fileDetail);
                        var path = Server.MapPath(Path.Combine(dirPath, fileDetail.FileName));
                        file.SaveAs(path);

                        db.Entry(fileDetail).State = EntityState.Added;
                    }
                }
                db.Entry(docImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(docImage);
        }

        // GET: DocImages/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocImages docImage = db.DocImages.Find(id);
            if (docImage == null)
            {
                return HttpNotFound();
            }
            return View(docImage);
        }

        // POST: DocImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            DocImages docImage = db.DocImages.Find(id);
            string dirPath = PrepareFolder(docImage.direktori);

            foreach (var item in docImage.ImgDetail.ToList())
            {
                //string path = Server.MapPath(Path.Combine(dirPath, item.FileName));

                //if (System.IO.File.Exists(path))
                //{
                //    System.IO.File.Delete(path);
                //}
                db.imgDetails.Remove(item);
                db.SaveChanges();
            }

            //cleanup folder penyimpanan
            System.IO.Directory.Delete(Server.MapPath(GetDir(docImage.direktori)),true);
            var document = db.Documents.Find(docImage.DocID);
            db.Entry(document).State = EntityState.Modified;
            db.DocImages.Remove(docImage);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
  
        /// <summary>
        /// Delete File di menu edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult DeleteFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                Guid guid = new Guid(id);
                ImgDetail fileDetail = db.imgDetails.Find(guid);
                if (fileDetail == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }

                //Remove from database
                db.imgDetails.Remove(fileDetail);
                db.SaveChanges();

                //Delete file from the file system
                var path = Path.Combine(Server.MapPath(basepath), GetDir(fileDetail.DocImages.direktori), fileDetail.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Return berupa file gambar , menggunakan string imagePath (lengkap/full)
        /// </summary>
        /// <param name="imagePath">String, alamat lengkap di server</param>
        public ActionResult GetImg(string pathImg)
        {
            var bytes = System.IO.File.ReadAllBytes(Server.MapPath(pathImg));
            return File(bytes, "image/jpg");
        }

        private ActionResult GetFirstImage(int docImageId)
        {
            Guid direktori = db.DocImages.Find(docImageId).direktori;
            string dirPath = GetDir(direktori);
            string[] files = Directory.GetFiles(Server.MapPath(dirPath));
            string selectedImg = Path.Combine(dirPath, files[0]);
            var bytes = System.IO.File.ReadAllBytes(Server.MapPath(selectedImg));
            return File(bytes, "image/jpg");
        }
        /// <summary>
        /// Display gambar menggunakan id dokumen dan nama file
        /// </summary>
        /// <param name="DocID">nomor id dokumen, int</param>
        /// <param name="fileName">nama file gambar</param>
        /// <returns></returns>
        public ActionResult DownImg(Guid DocDir, string fileName)
        {
            var bytes = System.IO.File.ReadAllBytes(Server.MapPath(GetPath(DocDir, fileName)));
            return File(bytes, "image/jpg");
        }

        /// <summary>
        /// Display gambar menggunakan id dokumen dan nama file
        /// </summary>
        /// <param name="DocID">nomor id dokumen, int</param>
        /// <param name="fileName">nama file gambar</param>
        /// <returns></returns>
        public ActionResult FirstImg(Guid DocDir, string fileName)
        {
            var bytes = System.IO.File.ReadAllBytes(Server.MapPath(GetPath(DocDir, fileName)));
            return File(bytes, "image/jpg");
        }
        /// <summary>
        /// Menghasilkan alamat fisik (direktori) berupa string,
        /// berdasarkan tahun (current year) menghasilkan 6 digit string
        /// 
        /// </summary>
        /// <param name="byDocID">nomor dokumen, INT</param>
        /// <returns></returns>
        private string GetDir(Guid lastdir)
        {
            string f = lastdir.ToString();
            return Path.Combine(basepath,CurrentYear.ToString(), f);
        }
        private string GetDirSTR(string lastdir)
        {
            return Path.Combine(basepath, CurrentYear.ToString(), lastdir);
        }
        /// <summary>
        /// Menghasilkan alamat file (fisik) berupa string dengan folder/namafile.ekstensi
        /// </summary>
        /// <param name="byDocID">nomor dokumen, int</param>
        /// <param name="filename">nama file dengan ekstensinya, string</param>
        /// <returns></returns>
        private string GetPath(Guid g, string filename)
        {
            return Path.Combine(GetDir(g) , filename);
        }
    }
}
