namespace IntelliCloud.WordStore.Data.Database.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<IntelliCloud.WordStore.Data.Database.Context.WordStoreContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "IntelliCloud.WordStore.Data.Database.Context.WordStoreContext";
        }

        protected override void Seed(IntelliCloud.WordStore.Data.Database.Context.WordStoreContext context)
        {
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IX_Keyword_Value ON Keyword (Value)");
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
