
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;

namespace SisWBeck.DB
{
    public class SISWBeckContext :DbContext
    {
        
        public DbSet<Pesagens> Pesagens { get; set; }
        public DbSet<Lotes> Lotes { get; set; }

        public SISWBeckContext(DbContextOptions<SISWBeckContext> options) : base(options) 
        {
            SQLitePCL.Batteries_V2.Init();
            this.Database.EnsureCreated();
        }



        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Lotes>().HasKey(l => l.Id);
        //    base.OnModelCreating(modelBuilder);
        //}


    }
}
