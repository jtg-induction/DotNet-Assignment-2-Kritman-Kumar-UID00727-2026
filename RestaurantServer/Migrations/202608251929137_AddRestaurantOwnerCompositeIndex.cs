namespace RestaurantServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRestaurantOwnerCompositeIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RestaurantOwners", new[] { "RestaurantId" });
            DropIndex("dbo.RestaurantOwners", new[] { "UserId" });
            CreateIndex("dbo.RestaurantOwners", new[] { "RestaurantId", "UserId" }, unique: true, name: "IX_RestaurantOwner_RestaurantId_UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RestaurantOwners", "IX_RestaurantOwner_RestaurantId_UserId");
            CreateIndex("dbo.RestaurantOwners", "UserId");
            CreateIndex("dbo.RestaurantOwners", "RestaurantId");
        }
    }
}
