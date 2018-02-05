namespace ShopPlatform
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ShopContext : DbContext
    {
        public ShopContext()
            : base("name=ShopContext")
        {
        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Food> Food { get; set; }
        public virtual DbSet<FoodType> FoodType { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<Shop> Shop { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<FoodDictionary> FoodDictionary { get; set; }
        public virtual DbSet<FoodTypeDictionary> FoodTypeDictionary { get; set; }
        public virtual DbSet<ShopStore> ShopStore { get; set; }
        public virtual DbSet<UnitDictionary> UnitDictionary { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Food>()
                .HasMany(e => e.FoodDictionary)
                .WithRequired(e => e.Food)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Food>()
                .HasMany(e => e.ShopStore)
                .WithRequired(e => e.Food)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FoodType>()
                .HasMany(e => e.FoodTypeDictionary)
                .WithRequired(e => e.FoodType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Language>()
                .HasMany(e => e.FoodDictionary)
                .WithRequired(e => e.Language)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Language>()
                .HasMany(e => e.FoodTypeDictionary)
                .WithRequired(e => e.Language)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Language>()
                .HasMany(e => e.UnitDictionary)
                .WithRequired(e => e.Language)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Shop>()
                .HasMany(e => e.ShopStore)
                .WithRequired(e => e.Shop)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Unit>()
                .HasMany(e => e.ShopStore)
                .WithRequired(e => e.Unit)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Unit>()
                .HasMany(e => e.UnitDictionary)
                .WithRequired(e => e.Unit)
                .WillCascadeOnDelete(false);
        }
    }
}
