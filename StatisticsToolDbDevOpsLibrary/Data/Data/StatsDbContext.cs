using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StatisticsToolDbDevOpsLibrary.Data.DataModels;

namespace StatisticsToolDbDevOpsLibrary.Data.Data;

public partial class StatsDbContext : DbContext
{
    public StatsDbContext()
    {
    }

    public StatsDbContext(DbContextOptions<StatsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<WorkItemStatistic> WorkItemStatistics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=StatisticsToolDb;User ID=sa;Password=sql@exp2000!;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkItemStatistic>(entity =>
        {
            entity.HasKey(e => e.WorkItemStatisticsId).HasName("PK__WorkItem__E70715892A5F44B0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
