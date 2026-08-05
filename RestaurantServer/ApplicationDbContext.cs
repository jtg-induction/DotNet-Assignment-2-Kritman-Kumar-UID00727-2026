using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using RestaurantServer.Models;

namespace RestaurantServer
{
    public class ApplicationDbContext:DbContext
    {

        public ApplicationDbContext() : base("RestaurantDB")
        {
        
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders {  get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Restaurant>Restaurants { get; set; }
        public DbSet<RestaurantOwner> RestaurantOwners { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Restaurant -> Items
            modelBuilder.Entity<Item>()
                .HasRequired(i => i.Restaurant)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.RestaurantId)
                .WillCascadeOnDelete(false);

            // Restaurant -> Orders
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantId)
                .WillCascadeOnDelete(false);

            // User -> Orders
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(false);

            // Order -> OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .WillCascadeOnDelete(false);

            // Item -> OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Item)
                .WithMany(i => i.OrderItems)
                .HasForeignKey(oi => oi.ItemId)
                .WillCascadeOnDelete(false);

            // Restaurant -> RestaurantOwners
            modelBuilder.Entity<RestaurantOwner>()
                .HasRequired(ro => ro.Restaurant)
                .WithMany(r => r.RestaurantOwners)
                .HasForeignKey(ro => ro.RestaurantId)
                .WillCascadeOnDelete(false);

            // User -> RestaurantOwners
            modelBuilder.Entity<RestaurantOwner>()
                .HasRequired(ro => ro.User)
                .WithMany(u => u.RestaurantOwners)
                .HasForeignKey(ro => ro.UserId)
                .WillCascadeOnDelete(false);

            // Restaurant -> CreatedBy User
            modelBuilder.Entity<Restaurant>()
                .HasRequired(r => r.CreatedByUser)
                .WithMany()
                .HasForeignKey(r => r.CreatedBy)
                .WillCascadeOnDelete(false);

            // Restaurant -> UpdatedBy User
            modelBuilder.Entity<Restaurant>()
                .HasRequired(r => r.UpdatedByUser)
                .WithMany()
                .HasForeignKey(r => r.UpdatedBy)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

    }
}
