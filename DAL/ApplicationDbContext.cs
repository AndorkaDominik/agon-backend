using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using DAL.Entities;
namespace DAL;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole, string>(options)
{
    public DbSet<Athlete> Athletes { get; set; }
    public DbSet<AthleteProgress> AthleteProgresses { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
    public DbSet<WorkoutSession> WorkoutSessions { get; set; }
    public DbSet<PlanExercise> PlanExercises { get; set; }
    public DbSet<SessionExercise> SessionExercises { get; set; }
    public DbSet<AssignedPlan> AssignedPlans { get; set; }
    public DbSet<CoachingRequest> CoachingRequests { get; set; }
    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<DiscussionMessage> DiscussionMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Athlete>()
            .Property(a => a.ThemeColor)
            .HasMaxLength(16)
            .HasDefaultValue("grass");

        modelBuilder.Entity<User>()
            .HasOne(u => u.Athlete)
            .WithOne(a => a.User)
            .HasForeignKey<Athlete>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Athlete>()
            .HasOne(a => a.AthleteProgress)
            .WithOne(ap => ap.Athlete)
            .HasForeignKey<AthleteProgress>(ap => ap.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Trainer)
            .WithOne(t => t.User)
            .HasForeignKey<Trainer>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CoachingRequest>()
            .HasOne(cr => cr.Athlete)
            .WithMany()
            .HasForeignKey(cr => cr.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CoachingRequest>()
            .HasOne(cr => cr.Trainer)
            .WithMany()
            .HasForeignKey(cr => cr.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Discussion>()
            .HasOne(d => d.Athlete)
            .WithMany()
            .HasForeignKey(d => d.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Discussion>()
            .HasOne(d => d.Trainer)
            .WithMany()
            .HasForeignKey(d => d.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DiscussionMessage>()
            .HasOne(dm => dm.Discussion)
            .WithMany(d => d.Messages)
            .HasForeignKey(dm => dm.DiscussionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DiscussionMessage>()
            .HasOne(dm => dm.Sender)
            .WithMany()
            .HasForeignKey(dm => dm.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkoutSession>()
            .HasMany(s => s.Exercises)
            .WithOne()
            .HasForeignKey(e => e.WorkoutSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkoutPlan>()
            .HasMany(wp => wp.Exercises)
            .WithOne()
            .HasForeignKey(pe => pe.WorkoutPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AssignedPlan>()
            .HasKey(ap => new { ap.AthleteId, ap.WorkoutPlanId });

        modelBuilder.Entity<AssignedPlan>()
            .HasOne(ap => ap.Athlete)
            .WithMany(a => a.AssignedPlans)
            .HasForeignKey(ap => ap.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AssignedPlan>()
            .HasOne(ap => ap.WorkoutPlan)
            .WithMany(wp => wp.SharedWithAthletes)
            .HasForeignKey(ap => ap.WorkoutPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Athlete", NormalizedName = "ATHLETE", ConcurrencyStamp = "1" },
            new IdentityRole { Id = "2", Name = "Trainer", NormalizedName = "TRAINER", ConcurrencyStamp = "2" }
        );
    }
}
