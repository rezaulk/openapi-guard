using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenApiGuard.Core.Entities;

namespace OpenApiGuard.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectRepoLink> ProjectRepoLinks => Set<ProjectRepoLink>();
    public DbSet<ApiSpecVersion> ApiSpecVersions => Set<ApiSpecVersion>();
    public DbSet<DiffReport> DiffReports => Set<DiffReport>();
    public DbSet<RuleSet> RuleSets => Set<RuleSet>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(200).IsRequired();
            e.Property(p => p.Description).HasMaxLength(1000);
            e.Property(p => p.OwnerUserId).HasMaxLength(450).IsRequired();
            e.HasIndex(p => p.OwnerUserId);
        });

        builder.Entity<ProjectRepoLink>(e =>
        {
            e.HasKey(r => r.ProjectId);
            e.HasOne(r => r.Project)
                .WithOne(p => p.RepoLink)
                .HasForeignKey<ProjectRepoLink>(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(r => r.Provider).HasMaxLength(50).IsRequired();
            e.Property(r => r.RepoUrl).HasMaxLength(500).IsRequired();
            e.Property(r => r.DefaultBranch).HasMaxLength(200).IsRequired();
            e.Property(r => r.OpenApiSpecHint).HasMaxLength(500);
        });

        builder.Entity<ApiSpecVersion>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasOne(s => s.Project)
                .WithMany(p => p.Specs)
                .HasForeignKey(s => s.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(s => s.Label).HasMaxLength(200).IsRequired();
            e.Property(s => s.Source).HasMaxLength(50).IsRequired();
            e.Property(s => s.Format).HasMaxLength(10).IsRequired();
            e.Property(s => s.OpenApiVersion).HasMaxLength(10).IsRequired();
            e.Property(s => s.UploadedByUserId).HasMaxLength(450).IsRequired();
            e.Property(s => s.RawSpecText).IsRequired();
        });

        builder.Entity<DiffReport>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasOne(d => d.Project)
                .WithMany(p => p.Diffs)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(d => d.SummaryJson).IsRequired();
            e.Property(d => d.CreatedByUserId).HasMaxLength(450).IsRequired();
            e.HasIndex(d => d.ProjectId);
        });

        builder.Entity<RuleSet>(e =>
        {
            e.HasKey(r => r.ProjectId);
            e.HasOne(r => r.Project)
                .WithOne(p => p.RuleSet)
                .HasForeignKey<RuleSet>(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(r => r.JsonConfig).IsRequired();
            e.Property(r => r.UpdatedByUserId).HasMaxLength(450).IsRequired();
        });

        builder.Entity<AuditLog>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.ActorUserId).HasMaxLength(450).IsRequired();
            e.Property(a => a.Action).HasMaxLength(200).IsRequired();
            e.HasIndex(a => a.ProjectId);
            e.HasIndex(a => a.CreatedAt);
        });
    }
}
