using System.Data.Entity;

namespace LinqExpressionProjection.Test.Model
{
    internal class ProjectsDbContext : DbContext
    {
        static ProjectsDbContext()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ProjectsDbContext>());
        }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Subproject> Subprojects { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
