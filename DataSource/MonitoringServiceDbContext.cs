
using Microsoft.Data.Entity;
using Microsoft.Data.Sqlite;
using System;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace MonitoringService.DataSource
{
    class MonitoringServiceDbContext : DbContext
    { 
        public DbSet<CheckResult> CheckResults { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "ResultsDb.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }
    }
}
