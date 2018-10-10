namespace MyVeggieStore.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class LocationModels : DbContext
    {
        public LocationModels()
            : base("name=LocationModels")
        {
        }

        public virtual DbSet<tblLocation> tblLocations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblLocation>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<tblLocation>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<tblLocation>()
                .Property(e => e.Latitude)
                .HasPrecision(10, 8);

            modelBuilder.Entity<tblLocation>()
                .Property(e => e.Longitude)
                .HasPrecision(11, 8);
        }
    }
}
