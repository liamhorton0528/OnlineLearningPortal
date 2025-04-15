namespace OnlineLearningPortal.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OnlineLearningPortal.Models.OnlineLearningPortalContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "OnlineLearningPortal.Models.OnlineLearningPortalContext";
        }

        protected override void Seed(OnlineLearningPortal.Models.OnlineLearningPortalContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
