using Microsoft.EntityFrameworkCore;
using TagNameMapper.Models.TableMetadatas;

namespace TagNameMapper.Models.EFCore.DbContexts;

public class TagNameMapperDbContext :DbContext
{
    public TagNameMapperDbContext(DbContextOptions<TagNameMapperDbContext> options) : base(options)
    {
    }

    // DbSet 属性
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagGroup> TagGroups { get; set; }
    public DbSet<PollGroup> PollGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 TagGroup 自引用关系
        modelBuilder.Entity<TagGroup>()
            .HasOne(g => g.ParentGroup)
            .WithMany(g => g.ChildGroups)
            .HasForeignKey(g => g.ParentGroupId)
            .OnDelete(DeleteBehavior.Restrict); // 防止级联删除导致的问题

        // 配置 Tag 与 TagGroup 的关系
        modelBuilder.Entity<Tag>()
            .HasOne(t => t.TagGroup)
            .WithMany(g => g.Tags)
            .HasForeignKey(t => t.TagGroupId)
            .OnDelete(DeleteBehavior.SetNull); // 删除组时，标签的组ID设为null

        // 配置 Tag 与 PollGroup 的关系
        modelBuilder.Entity<Tag>()
            .HasOne(t => t.PollGroup)
            .WithMany()
            .HasForeignKey(t => t.PollGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        // 配置唯一索引 - 确保 Name 在各自表内唯一
        modelBuilder.Entity<TagGroup>()
            .HasIndex(g => new { g.ParentGroupId, g.GroupType, g.Name })
            .IsUnique()
            .HasDatabaseName("IX_TagGroup_ParentGroupId_GroupType_Name_Unique");

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique()
            .HasDatabaseName("IX_Tags_Name_Unique");

        modelBuilder.Entity<PollGroup>()
            .HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("IX_PollGroups_Name_Unique");

        // 配置其他索引
        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Address)
            .HasDatabaseName("IX_Tags_Address");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // 默认 SQLite 连接字符串
            optionsBuilder.UseSqlite("Data Source=Data/DataBase.sqlite");
        }
    }
}