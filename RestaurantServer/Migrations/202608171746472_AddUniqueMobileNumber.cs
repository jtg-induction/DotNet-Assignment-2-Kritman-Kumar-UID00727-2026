namespace RestaurantServer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddUniqueMobileNumber : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                CREATE UNIQUE INDEX IX_User_MobileNumber
                ON dbo.Users (MobileNumber)
                WHERE MobileNumber IS NOT NULL
            ");
        }

        public override void Down()
        {
            Sql(@"
                DROP INDEX IX_User_MobileNumber
                ON dbo.Users
            ");
        }
    }
}
