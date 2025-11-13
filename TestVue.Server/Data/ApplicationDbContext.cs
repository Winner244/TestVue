using Microsoft.EntityFrameworkCore;
using TestVue.Server.Models;

namespace TestVue.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FormSubmissionModel> FormSubmissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FormSubmissionModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SubmittedAt).IsRequired();
                entity.Property(e => e.FormData)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, System.Text.Json.JsonElement>()
                    );
            });
        }
    }
}
