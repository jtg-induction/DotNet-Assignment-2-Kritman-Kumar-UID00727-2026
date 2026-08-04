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
            base.OnModelCreating(modelBuilder);
        }

    }
}
