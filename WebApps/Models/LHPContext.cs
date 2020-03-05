using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApps.Models
{
    public class LHPContext: DbContext
    {
        public LHPContext() : base("DBLhp")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<LHPContext>());

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<LHPContext, WebApps.Migrations.Configuration>());
        }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<DocImages> DocImages { get; set; }
        public DbSet<ImgDetail> imgDetails { get; set; }
    }
}