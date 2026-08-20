namespace RestaurantServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueMobileIndexToRestaurant : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Restaurants", "MobileNumber", unique: true, name: "IX_Restaurant_MobileNumber");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Restaurants", "IX_Restaurant_MobileNumber");
        }
    }
}
