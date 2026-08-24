using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Models;

namespace TicketSystem.Database
{
    public class TicketSystemDbContext : IdentityDbContext<IdentityUser>
    {
        public TicketSystemDbContext(DbContextOptions<TicketSystemDbContext> options) : base(options)
        {

        }
        public DbSet<Ticket> Tickets {get; set;}
        public DbSet<Project> Projects { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UploadFile> UploadFiles { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<BlockTicket> BlockTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ticket → Project
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); //  definiere was beim Löschen passiert

            // Ticket → IdentityUser (Ersteller)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.IdentityUser)
                .WithMany() // kein Rückverweis 
                .HasForeignKey(t => t.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket → Bearbeiter (optional)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Bearbeiter)
                .WithMany()
                .HasForeignKey(t => t.BearbeiterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket → Closer (optional)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Closer)
                .WithMany()
                .HasForeignKey(t => t.CloserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Comment → Ticket
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment → IdentityUser
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.IdentityUser)
                .WithMany()
                .HasForeignKey(c => c.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // UploadFile → Ticket
            modelBuilder.Entity<UploadFile>()
                .HasOne(f => f.Ticket)
                .WithMany(t => t.UploadFiles)
                .HasForeignKey(f => f.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // UploadFile → IdentityUser
            modelBuilder.Entity<UploadFile>()
                .HasOne(f => f.IdentityUser)
                .WithMany()
                .HasForeignKey(f => f.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);
            // BlockTicket -> Ticket
            // Vermeidet multiple cascade paths --> nochmal nachgucken
            modelBuilder.Entity<BlockTicket>()
                .HasOne(bt => bt.Ticket)
                .WithMany(t => t.BlockedTickets)
                .HasForeignKey(bt => bt.TicketId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<BlockTicket>()
                .HasOne(bt => bt.BlockedTicket)
                .WithMany()
                .HasForeignKey(bt => bt.BlockedTicketId)
                .OnDelete(DeleteBehavior.NoAction); 

        }       
    }
}
