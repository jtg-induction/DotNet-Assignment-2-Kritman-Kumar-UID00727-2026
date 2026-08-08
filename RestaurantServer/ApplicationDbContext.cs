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
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Restaurant -> Items
            modelBuilder.Entity<Item>()
                .HasRequired(item => item.Restaurant)
                .WithMany(restaurant => restaurant.Items)
                .HasForeignKey(item => item.RestaurantId)
                .WillCascadeOnDelete(false);

            // Restaurant -> Orders
            modelBuilder.Entity<Order>()
                .HasRequired(order => order.Restaurant)
                .WithMany(restaurant => restaurant.Orders)
                .HasForeignKey(order => order.RestaurantId)
                .WillCascadeOnDelete(false);

            // User -> Orders
            modelBuilder.Entity<Order>()
                .HasRequired(order => order.User)
                .WithMany(user => user.Orders)
                .HasForeignKey(order => order.UserId)
                .WillCascadeOnDelete(false);

            // Order -> OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasRequired(orderItem => orderItem.Order)
                .WithMany(order => order.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId)
                .WillCascadeOnDelete(false);

            // Item -> OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasRequired(orderItem => orderItem.Item)
                .WithMany(item => item.OrderItems)
                .HasForeignKey(orderItem => orderItem.ItemId)
                .WillCascadeOnDelete(false);

            // Restaurant -> RestaurantOwners
            modelBuilder.Entity<RestaurantOwner>()
                .HasRequired(restaurantOwner => restaurantOwner.Restaurant)
                .WithMany(restaurant => restaurant.RestaurantOwners)
                .HasForeignKey(ro => ro.RestaurantId)
                .WillCascadeOnDelete(false);

            // User -> RestaurantOwners
            modelBuilder.Entity<RestaurantOwner>()
                .HasRequired(restaurantOwner => restaurantOwner.User)
                .WithMany(u => u.RestaurantOwners)
                .HasForeignKey(restaurantOwner => restaurantOwner.UserId)
                .WillCascadeOnDelete(false);

            // Restaurant -> CreatedBy User
            modelBuilder.Entity<Restaurant>()
                .HasRequired(restaurant => restaurant.CreatedByUser)
                .WithMany()
                .HasForeignKey(restaurant => restaurant.CreatedBy)
                .WillCascadeOnDelete(false);

            // Restaurant -> UpdatedBy User
            modelBuilder.Entity<Restaurant>()
                .HasRequired(restaurant => restaurant.UpdatedByUser)
                .WithMany()
                .HasForeignKey(restaurant => restaurant.UpdatedBy)
                .WillCascadeOnDelete(false);

            // user -> RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .HasRequired(refreshToken => refreshToken.User)
                .WithMany(user => user.RefreshTokens)
                .HasForeignKey(refreshToken => refreshToken.UserId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

    }
}
