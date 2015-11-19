namespace ProjectManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomUserData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ProjectUserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ProjectUserId");
        }
    }
}
