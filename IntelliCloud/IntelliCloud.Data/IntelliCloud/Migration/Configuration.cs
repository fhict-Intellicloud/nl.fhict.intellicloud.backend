using System.Data.Entity.Migrations;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;

namespace nl.fhict.IntelliCloud.Data.IntelliCloud.Migration
{
    /// <summary>
    /// Configuration class for the <see cref="IntelliCloudContext"/> model.
    /// </summary>
    internal sealed class Configuration : DbMigrationsConfiguration<IntelliCloudContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
        }

        /// <summary>
        /// Seeds the specified context.
        /// </summary>
        /// <param name="context">The context that is to be seeded.</param>
        protected override void Seed(IntelliCloudContext context)
        {
            context.CreateIndex(
                name: "IX_Source_Unique",
                table: "Source",
                columns: new[] { "Value", "SourceDefinition_Id" },
                unique: true);
        }        
    }
}
