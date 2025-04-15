namespace OnlineLearningPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        InstructorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Instructors", t => t.InstructorId, cascadeDelete: true)
                .Index(t => t.InstructorId);
            
            CreateTable(
                "dbo.Enrollments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        StudentId = c.Int(nullable: false),
                        EnrollmentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .Index(t => t.CourseId)
                .Index(t => t.StudentId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        BirthDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Instructors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Office = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "InstructorId", "dbo.Instructors");
            DropForeignKey("dbo.Enrollments", "StudentId", "dbo.Students");
            DropForeignKey("dbo.Enrollments", "CourseId", "dbo.Courses");
            DropIndex("dbo.Enrollments", new[] { "StudentId" });
            DropIndex("dbo.Enrollments", new[] { "CourseId" });
            DropIndex("dbo.Courses", new[] { "InstructorId" });
            DropTable("dbo.Instructors");
            DropTable("dbo.Students");
            DropTable("dbo.Enrollments");
            DropTable("dbo.Courses");
        }
    }
}
