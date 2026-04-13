using Microsoft.EntityFrameworkCore;
using PerformanceAnalysis.Domain.Auth;

namespace PerformanceAnalysis.Infrastructure.Auth;

/// <summary>
/// DbContext для аутентификации (Database-First, Практика 46).
/// Сгенерирован командой EF Core Scaffold, при необходимости доработан вручную.
/// </summary>
public partial class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public virtual DbSet<Domain.Auth.Group> Groups { get; set; }
    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }
    public virtual DbSet<Domain.Auth.Student> Students { get; set; }
    public virtual DbSet<Domain.Auth.User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Auth.Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_groups");
            entity.ToTable("groups");
            entity.HasIndex(e => e.Courseid, "ix_groups_courseid");
            entity.HasIndex(e => e.Directionid, "ix_groups_directionid");
            entity.HasIndex(e => e.Name, "ix_groups_name").IsUnique();
            entity.HasIndex(e => e.Projectid, "ix_groups_projectid");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Courseid).HasColumnName("courseid");
            entity.Property(e => e.Directionid).HasColumnName("directionid");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Projectid).HasColumnName("projectid");

            entity.HasMany(d => d.Students).WithMany(p => p.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentGroup",
                    r => r.HasOne<Domain.Auth.Student>().WithMany()
                        .HasForeignKey("Studentsid")
                        .HasConstraintName("fk_student_groups_students_studentsid"),
                    l => l.HasOne<Domain.Auth.Group>().WithMany()
                        .HasForeignKey("Groupsid")
                        .HasConstraintName("fk_student_groups_groups_groupsid"),
                    j =>
                    {
                        j.HasKey("Groupsid", "Studentsid").HasName("pk_student_groups");
                        j.ToTable("student_groups");
                        j.HasIndex(new[] { "Studentsid" }, "ix_student_groups_studentsid");
                        j.IndexerProperty<int>("Groupsid").HasColumnName("groupsid");
                        j.IndexerProperty<int>("Studentsid").HasColumnName("studentsid");
                    });
        });

        modelBuilder.Entity<Refreshtoken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_refreshtokens");
            entity.ToTable("refreshtokens");
            entity.HasIndex(e => e.Tokenhash, "ix_refreshtokens_tokenhash");
            entity.HasIndex(e => e.Userid, "ix_refreshtokens_userid");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat).HasColumnName("createdat");
            entity.Property(e => e.Expiresat).HasColumnName("expiresat");
            entity.Property(e => e.Revokedat).HasColumnName("revokedat");
            entity.Property(e => e.Tokenhash).HasColumnName("tokenhash");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Refreshtokens)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("fk_refreshtokens_users_userid");
        });

        modelBuilder.Entity<Domain.Auth.Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_students");
            entity.ToTable("students");
            entity.HasIndex(e => e.Userid, "ix_students_userid").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatarpath).HasColumnName("avatarpath");
            entity.Property(e => e.Phone).HasMaxLength(30).HasColumnName("phone");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Vkprofilelink).HasColumnName("vkprofilelink");

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Domain.Auth.Student>(d => d.Userid)
                .HasConstraintName("fk_students_users_userid");
        });

        modelBuilder.Entity<Domain.Auth.User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_users");
            entity.ToTable("users");
            entity.HasIndex(e => e.Email, "ix_users_email").IsUnique();
            entity.HasIndex(e => e.Login, "ix_users_login").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("createdat");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Firstname).HasColumnName("firstname");
            entity.Property(e => e.Lastname).HasColumnName("lastname");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Middlename).HasColumnName("middlename");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Role).HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
