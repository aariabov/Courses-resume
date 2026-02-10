using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Common;
using Devpull.Controllers;
using Devpull.Course;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Devpull.DbModels;

public partial class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Step> Steps { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Exercise> Exercises { get; set; }

    public virtual DbSet<RunExercise> RunExercises { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<ExerciseLevel> ExerciseLevels { get; set; }

    public virtual DbSet<ExerciseExample> ExerciseExamples { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql("Name=ConnectionStrings:Devpull");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<RefreshToken>()
            .HasIndex(r => new { r.UserId, r.DeviceFingerprint })
            .IsUnique();

        modelBuilder.Entity<Step>().HasIndex(r => new { r.CourseId, r.Ord }).IsUnique();
        modelBuilder.Entity<Lesson>().HasIndex(r => new { r.StepId, r.Ord }).IsUnique();
        modelBuilder.Entity<Subscription>().HasIndex(r => r.PaymentId).IsUnique();
        modelBuilder.Entity<Tag>().HasIndex(e => e.Name).IsUnique();
        modelBuilder.Entity<ExerciseLevel>().HasIndex(e => e.Name).IsUnique();
        modelBuilder.Entity<Exercise>().HasIndex(e => e.Url).IsUnique();

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Url).IsUnique();
        });

        modelBuilder.Entity<Payment>().Property(u => u.CreateJson).HasColumnType("jsonb");
        modelBuilder.Entity<Payment>().Property(o => o.Status).HasConversion<string>();
        modelBuilder.Entity<Payment>().Property(o => o.Type).HasConversion<string>();

        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Number).IsUnique();
            entity.Property(e => e.Number).ValueGeneratedOnAdd();
        });

        modelBuilder
            .Entity<Exercise>()
            .HasData(
                new Exercise
                {
                    Id = Guid.Parse(ExerciseConst.AddExerciseId),
                    ExerciseLevelId = 1,
                    Url = "",
                    ShortName = "",
                    Description = "",
                    Template =
                        @"int Add(int a, int b)
{
    // Ваш код
}"
                },
                new Exercise
                {
                    Id = Guid.Parse(ExerciseConst.MinExerciseId),
                    ExerciseLevelId = 1,
                    Url = "",
                    ShortName = "",
                    Description = "",
                    Template =
                        @"int Min(int a, int b)
{
    // Ваш код
}"
                }
            );

        modelBuilder.ConvertToSnakeCase(); // должно быть в конце, иначе не применяется
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

public static partial class SnakeCaseExtensions
{
    public static void ConvertToSnakeCase(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            // Replace table names
            entity.SetTableName(
                (
                    entity.GetTableName()
                    ?? throw new InvalidOperationException("Missing entity table name")
                ).ToSnakeCase()
            );

            // Replace column names
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToSnakeCase());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(
                    (
                        key.GetName()
                        ?? throw new InvalidOperationException("Missing entity key name")
                    ).ToSnakeCase()
                );
            }

            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(
                    (
                        key.GetConstraintName()
                        ?? throw new InvalidOperationException("Missing entity fk name")
                    ).ToSnakeCase()
                );
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(
                    (
                        index.GetDatabaseName()
                        ?? throw new InvalidOperationException("Missing entity database name")
                    ).ToSnakeCase()
                );
            }
        }
    }

    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var startUnderscores = StartUnderscoreRegex().Match(input);
        return startUnderscores + SnakeCaseRegex().Replace(input, "$1_$2").ToLower();
    }

    [GeneratedRegex(@"^_+")]
    private static partial Regex StartUnderscoreRegex();

    [GeneratedRegex(@"([a-z0-9])([A-Z])")]
    private static partial Regex SnakeCaseRegex();
}
