using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApps.Models;
using WebApps.ViewModel;
using PagedList;


namespace WebApps.Controllers
{
    public class EventImageController : Controller
    {
        //variabel
        private LHPContext db = new LHPContext();

        private string basepath = "~\\App_Data\\Uploads\\Event\\";
        private int CurrentYear = DateTime.Now.Year;


        //metode

        /// <summary>
        /// return string berupa alamat (path) pada server menggunakan <string> input
        /// </summary>
        /// <param name="tFolder">string</param>
        /// <returns></returns>
        private string PrepareFolder(String tFolder)
        {
            string dirPath = GetDir(tFolder);
            if (!Directory.Exists(Server.MapPath(tFolder)))
            {
                Directory.CreateDirectory(Server.MapPath(dirPath));
            } 
            return dirPath;
        }

        /// <summary>
        /// mencari alamat (path) folder (current) yang digunakan untuk penyimpanan menggunakan alamat akhir folder
        /// </summary>
        /// <param name="tFolder">String</param>
        /// <returns></returns>
        private string SetCurrentFolder(string tFolder)
        {
            string dirPath = GetDir(tFolder);
            return dirPath;
        }

        /// <summary>
        /// Untuk return halaman index pada Event
        /// </summary>
        /// <param name="pegawe"></param>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
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

            var documents = from dbase in db.EventImages select dbase;

            if (!string.IsNullOrEmpty(searchString))
            {
                documents = documents.Where(d => d.deskripsi.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "EventDate_Desc":
                    documents = documents.OrderByDescending(d => d.EventDate);
                    break;
                case "EventDate":
                    documents = documents.OrderBy(d => d.EventDate);
                    break;
                default:
                    documents = documents.OrderBy(d => d.EventDate);
                    break;
            }

            //var docImages = db.DocImages.Include(d => d.Document);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(documents.ToPagedList(pageNumber, pageSize));
        }

        // GET: DocImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EventImages evImg = db.EventImages.SingleOrDefault(x => x.EventID == id);

            if (evImg == null)
            {
                return HttpNotFound();
            }

            foreach (var item in evImg.ImgDetail)
            {
                item.FileName = GetPath(evImg.direktori, item.FileName);
            }
 
            return View(evImg);
        }
        /// <summary>
        /// Membuat event baru
        /// </summary>
        /// <returns>Void</returns>
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(EventImages evImage)
        {
            if (ModelState.IsValid)
            {
                //input waktu sebagai string untuk membuat nama folder
                string cDir = DateTime.Now.ToString("yyyyMMddHHmmss");
                evImage.direktori = cDir;
                //buat folder
                string dirPath = PrepareFolder(cDir);
                //loop setiap file untuk disimpan
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

                evImage.ImgDetail = fileDetails;
                db.EventImages.Add(evImage);
                //bagian save /update dokumen
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(evImage);
        }

        /// <summary>
        /// Edit Event dokumentasi, menggunakan id event (integer)
        /// </summary>
        /// <param name="id">Integer</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventImages evImage = db.EventImages.Include(s => s.ImgDetail)
                .SingleOrDefault(x => x.EventID == id);

            if (evImage == null)
            {
                return HttpNotFound();
            }

            return View(evImage);
        }

        // POST: DocImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(EventImages evImage)
        {
            string dirPath = SetCurrentFolder(evImage.direktori);
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
                            locationPath = Path.Combine(Server.MapPath(dirPath), fileName)
                        };
                        filedetails.Add(fileDetail);
                        var path = Server.MapPath(Path.Combine(dirPath, fileDetail.FileName));
                        file.SaveAs(path);

                        db.Entry(fileDetail).State = EntityState.Added;
                    }
                }
                db.Entry(evImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(evImage);
        }

        // GET: DocImages/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventImages evImage = db.EventImages.Find(id);
            if (evImage == null)
            {
                return HttpNotFound();
            }
            return View(evImage);
        }

        // POST: DocImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            EventImages evImage = db.EventImages.Find(id);
            string dirPath = PrepareFolder(evImage.direktori);

            foreach (var item in evImage.ImgDetail.ToList())
            {
                db.imgDetails.Remove(item);
                db.SaveChanges();
            }

            //cleanup folder penyimpanan
            System.IO.Directory.Delete(Server.MapPath(GetDir(evImage.direktori)),true);
            var document = db.Documents.Find(evImage.EventID);
            db.Entry(document).State = EntityState.Modified;
            db.EventImages.Remove(evImage);
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
        public JsonResult DeleteFile(string id, string kdEvent)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            if (String.IsNullOrEmpty(kdEvent))
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
                EventImages evImages = db.EventImages.Find(int.Parse(kdEvent));
                //Remove from database
                db.imgDetails.Remove(fileDetail);
                db.SaveChanges();

                //Delete file from the file system
                var path = Path.Combine(Server.MapPath(basepath), GetDir(evImages.direktori), fileDetail.FileName);
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

        private ActionResult GetFirstImage(int eventID)
        {
            String lokasiDir = db.EventImages.Find(eventID).direktori;
            string dirPath = GetDir(lokasiDir);
            string[] files = Directory.GetFiles(Server.MapPath(dirPath));
            string selectedImg = Path.Combine(dirPath, files[0]);
            var bytes = System.IO.File.ReadAllBytes(Server.MapPath(selectedImg));
            return File(bytes, "image/jpg");
        }
        /// <summary>
        /// Display gambar menggunakan id Event dan nama file
        /// </summary>
        /// <param name="EVdir">nomor id Event, int</param>
        /// <param name="fileName">nama file gambar</param>
        /// <returns></returns>
        public ActionResult DownImg(string EVdir, string fileName)
        {
            var bytes = System.IO.File.ReadAllBytes(Server.MapPath(GetPath(EVdir, fileName)));
            return File(bytes, "image/jpg");
        }

        /// <summary>
        /// Display gambar menggunakan id Event dan nama file
        /// </summary>
        /// <param name="DocID">nomor id dokumen, int</param>
        /// <param name="fileName">nama file gambar</param>
        /// <returns></returns>
        public ActionResult FirstImg(string evDir, string fileName)
        {
            var bytes = System.IO.File.ReadAllBytes(Server.MapPath(GetPath(evDir, fileName)));
            return File(bytes, "image/jpg");
        }
        /// <summary>
        /// Menghasilkan alamat fisik (direktori) berupa string,
        /// berdasarkan tahun (current year) menghasilkan 6 digit string
        /// </summary>
        /// <param name="byDocID">nomor dokumen, INT</param>
        /// <returns></returns>
        private string GetDir(string tFolder)
        {
            return Path.Combine(basepath,CurrentYear.ToString(), tFolder);
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
        private string GetPath(String g, string filename)
        {
            return Path.Combine(GetDir(g) , filename);
        }
    }
}
