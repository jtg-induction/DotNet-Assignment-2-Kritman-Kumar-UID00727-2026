namespace RestaurantServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Items",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 150),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    RestaurantId = c.Long(nullable: false),
                    Stock = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Restaurants", t => t.RestaurantId)
                .Index(t => t.RestaurantId);

            CreateTable(
                "dbo.OrderItems",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 150),
                    PriceAtPurchase = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Quantity = c.Int(nullable: false),
                    ItemId = c.Long(nullable: false),
                    OrderId = c.Long(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.ItemId)
                .Index(t => t.OrderId);

            CreateTable(
                "dbo.Orders",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    RestaurantId = c.Long(nullable: false),
                    UserId = c.Long(nullable: false),
                    Status = c.Int(nullable: false),
                    TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    AddressLine1 = c.String(),
                    AddressLine2 = c.String(),
                    City = c.String(),
                    PostalCode = c.String(),
                    Country = c.String(),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Restaurants", t => t.RestaurantId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.RestaurantId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Restaurants",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    RestaurantName = c.String(nullable: false, maxLength: 150),
                    Description = c.String(maxLength: 500),
                    MobileNumber = c.String(maxLength: 20),
                    AddressLine1 = c.String(maxLength: 200),
                    AddressLine2 = c.String(maxLength: 200),
                    City = c.String(maxLength: 100),
                    PostalCode = c.String(maxLength: 20),
                    Country = c.String(maxLength: 100),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.RestaurantOwners",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    RestaurantId = c.Long(nullable: false),
                    UserId = c.Long(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Restaurants", t => t.RestaurantId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.RestaurantId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100),
                    Email = c.String(nullable: false, maxLength: 150),
                    PasswordHash = c.String(nullable: false, maxLength: 500),
                    Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Role = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    MobileNumber = c.String(maxLength: 20),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.RestaurantOwners", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.RestaurantOwners", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.Orders", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.Items", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "ItemId", "dbo.Items");
            DropIndex("dbo.RestaurantOwners", new[] { "UserId" });
            DropIndex("dbo.RestaurantOwners", new[] { "RestaurantId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Orders", new[] { "RestaurantId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.OrderItems", new[] { "ItemId" });
            DropIndex("dbo.Items", new[] { "RestaurantId" });
            DropTable("dbo.Users");
            DropTable("dbo.RestaurantOwners");
            DropTable("dbo.Restaurants");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Items");
        }
    }
}
