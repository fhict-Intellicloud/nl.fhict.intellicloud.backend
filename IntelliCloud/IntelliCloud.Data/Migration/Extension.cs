using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Data.Migration
{
    /// <summary>
    /// An internal class containing extension methods for the Entity Framework.
    /// </summary>
    internal static class Extension
    {
        /// <summary>
        /// Creates a new index on the table. If an index with the same name already exists, the new one will override it.
        /// </summary>
        /// <param name="db">The database the index is added to.</param>
        /// <param name="name">The name of the index.</param>
        /// <param name="table">The table for which the index should be created.</param>
        /// <param name="columns">The columns that are indexed.</param>
        /// <param name="unique">If set to <c>true</c>, the index is unique.</param>
        /// <param name="filter">The filter of the index.</param>
        public static void CreateIndex(
            this DbContext db, string name, string table, string[] columns, bool unique = false, string filter = null)
        {
            const string query = "EXISTS (SELECT name FROM sys.indexes WHERE name = '{0}')";
            const string createIndex = "CREATE {0} NONCLUSTERED INDEX {1} on {2}({3}) {4} WITH (DROP_EXISTING = {5})";
            const string combined = "IF {0} {1} ELSE {2}";

            string index = string.Format(
                createIndex,
                unique ? "UNIQUE" : string.Empty,
                name,
                table,
                string.Join(", ", columns),
                filter == null ? string.Empty : "where " + filter,
                "{0}");

            string command = string.Format(
                combined, string.Format(query, name), string.Format(index, "ON"), string.Format(index, "OFF"));

            db.Database.ExecuteSqlCommand(command);
        }
    }
}
