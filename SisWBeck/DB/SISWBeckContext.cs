﻿
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;

namespace SisWBeck.DB
{
    public class SISWBeckContext :DbContext
    {
        private static Config config;
        public Config Config
        {
            get
            {
                if (config==null)
                    config = new Config();
                return config;
            }
        }
        
        public DbSet<Pesagens> Pesagens { get; set; }
        public DbSet<Lotes> Lotes { get; set; }

        public SISWBeckContext(DbContextOptions<SISWBeckContext> options) : base(options) 
        {
            SQLitePCL.Batteries_V2.Init();
            this.Database.EnsureCreated();
        }

        public void Remove(Lotes lote)
        {
            if (lote != null)
            {
                var transaction = Database.BeginTransaction();
                try
                {
                    if (lote.Pesagens?.Any() ?? false)
                    {
                        Pesagens.RemoveRange(lote.Pesagens);
                    }
                    Lotes.Remove(lote);
                    SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async void Remove(List<Pesagens> pesagens)
        {
            if (pesagens?.Any() ?? false)
            {
                Pesagens.RemoveRange(pesagens);
                await SaveChangesAsync();
            }
        }

        public async void Remove(Pesagens Pesagem)
        {
            if (Pesagem != null)
            {
                Pesagens.Remove(Pesagem);
                await SaveChangesAsync();
            }
        }

        public async void Add(Pesagens Pesagem)
        {
            if (Pesagem != null)
            {
                Pesagens.Add(Pesagem);
                await SaveChangesAsync();
            }
        }

        public async void Add(List<Pesagens> pesagens)
        {

            if (pesagens?.Any() ?? false)
            {
                Pesagens.AddRange(pesagens);
                await SaveChangesAsync();
            }
        }

        public void Add(Lotes lote)
        {
            if (lote != null)
            {
                var transaction = Database.BeginTransaction();
                try
                {
                    Lotes.Add(lote);
                    if (lote.Pesagens?.Any() ?? false)
                    {
                        Pesagens.AddRange(lote.Pesagens);
                    }
                    SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Lotes>().HasKey(l => l.Id);
        //    base.OnModelCreating(modelBuilder);
        //}


    }
}
