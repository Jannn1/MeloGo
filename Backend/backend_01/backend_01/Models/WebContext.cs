using backend_01.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace backend_01.Model
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    public class WebContext : DbContext
    {
        public DbSet<Ertekeles> Ertekeles { get; set; }
        public DbSet<Feladat> Feladat { get; set; }
        public DbSet<FeladatKategoria> FeladatKategoria { get; set; }
        public DbSet<Felhasznalo> Felhasznalo { get; set; }
        public DbSet<Jelentkezesek> Jelentkezesek { get; set; }
        public DbSet<Kategoria> Kategoria { get; set; }
        public DbSet<Mentes> Mentes { get; set; }

        public WebContext() : base("name=WebCon") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Feladat>()
                .HasRequired(f => f.Felhasznalo)
                .WithMany(u => u.Feladatok)
                .HasForeignKey(f => f.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FeladatKategoria>()
                .HasRequired(fk => fk.Feladat)
                .WithMany(f => f.FeladatKategoriak)
                .HasForeignKey(fk => fk.Task_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FeladatKategoria>()
            .HasKey(fk => new { fk.Task_Id, fk.Kat_Id });


            modelBuilder.Entity<Jelentkezesek>()
                .HasRequired(j => j.Feladat)
                .WithMany(f => f.Jelentkezesek)
                .HasForeignKey(j => j.Task_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Jelentkezesek>()
                .HasRequired(j => j.Felhasznalo)
                .WithMany(u => u.Jelentkezesek)
                .HasForeignKey(j => j.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Jelentkezesek>()
                .HasKey(j => new { j.User_Id, j.Task_Id });

            modelBuilder.Entity<Ertekeles>()
            .HasRequired(e => e.Ertekelo)
            .WithMany()
            .HasForeignKey(e => e.Ertekelo_Id)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ertekeles>()
                .HasRequired(e => e.Ertekelt)
                .WithMany(u => u.Ertekelesek)
                .HasForeignKey(e => e.Ertekelt_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mentes>()
                .HasRequired(m => m.Felhasznalo)
                .WithMany(u => u.Mentesek)
                .HasForeignKey(m => m.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mentes>()
                .HasRequired(m => m.Feladat)
                .WithMany(f => f.Mentesek)
                .HasForeignKey(m => m.Task_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mentes>()
                .HasKey(m => new { m.User_Id, m.Task_Id });

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

    }
}
