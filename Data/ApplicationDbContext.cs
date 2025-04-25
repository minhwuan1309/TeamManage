using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamManage.Models;

namespace TeamManage.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
        }

        // ========== Main DbSet ==========
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // ========== Workflow ==========
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public DbSet<WorkflowStepApproval> WorkflowStepApprovals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Khóa ngoại từ ProjectMember → ApplicationUser
            builder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMemberships)
                .HasForeignKey(pm => pm.UserId);

            builder.Entity<Module>()
                .HasOne(m => m.AssignedUser)
                .WithMany()
                .HasForeignKey(m => m.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<TaskItem>()
                .HasOne(t => t.AssignedUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Notification>()
                .HasOne(n => n.Receiver)
                .WithMany()
                .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkflowStepApproval>()
                .HasOne(a => a.Approver)
                .WithMany(u => u.Approvals)
                .HasForeignKey(a => a.ApproverId);
        }
    }
}
