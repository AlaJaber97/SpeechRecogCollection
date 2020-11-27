using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Sql.Models;

namespace SpeechRecg.Api
{
    public class DBcon : IdentityDbContext<BLL.Sql.Models.User>
    {
        public DBcon(DbContextOptions<DBcon> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransData>()
            .HasIndex(p => new { p.UserID, p.AudioFileID, p.TransTextID })
            .IsUnique(true);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<BLL.Sql.Models.AudioFile> AudioFile { get; set; }
        public DbSet<BLL.Sql.Models.TransText> TransText { get; set; }
        public DbSet<BLL.Sql.Models.TransData> TransData { get; set; }
    }
}
