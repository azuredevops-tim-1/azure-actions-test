using DevOps.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DevOps.Backend.Data
{
    public sealed class TaskContext : DbContext
    {
        public TaskContext(DbContextOptions<TaskContext> options)
            : base(options)
        {
        }

        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("DevOps");
        }
    }
}