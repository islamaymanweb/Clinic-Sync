using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<ScheduleException> ScheduleExceptions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // AppUser configuration
            builder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();

                // Relationships
                entity.HasOne(u => u.Patient)
                      .WithOne(p => p.User)
                      .HasForeignKey<Patient>(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.Doctor)
                      .WithOne(d => d.User)
                      .HasForeignKey<Doctor>(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Patient configuration
            builder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();

                entity.HasIndex(p => p.UserId).IsUnique();
            });

            // Doctor configuration
            builder.Entity<Doctor>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Id).ValueGeneratedOnAdd();

                entity.HasIndex(d => d.UserId).IsUnique();
                entity.HasIndex(d => d.LicenseNumber).IsUnique();

                entity.HasOne(d => d.Specialty)
                      .WithMany(s => s.Doctors)
                      .HasForeignKey(d => d.SpecialtyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Specialty configuration
            builder.Entity<Specialty>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();

                entity.HasIndex(s => s.Name).IsUnique();
            });

            // Appointment configuration
            builder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).ValueGeneratedOnAdd();

                entity.HasIndex(a => a.ReferenceNumber).IsUnique();

                // Relationships
                entity.HasOne(a => a.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(a => a.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                      .WithMany(d => d.Appointments)
                      .HasForeignKey(a => a.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes for performance
                entity.HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.Status });
                entity.HasIndex(a => new { a.PatientId, a.AppointmentDate });
                entity.HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.StartTime, a.EndTime });

                // Property configurations
                entity.Property(a => a.ReferenceNumber)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(a => a.ReasonForVisit)
                      .HasMaxLength(500);

                entity.Property(a => a.Notes)
                      .HasMaxLength(1000);

                entity.Property(a => a.CancellationReason)
                      .HasMaxLength(500);

                entity.Property(a => a.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);
            });

            // DoctorSchedule configuration
            builder.Entity<DoctorSchedule>(entity =>
            {
                entity.HasKey(ds => ds.Id);
                entity.Property(ds => ds.Id).ValueGeneratedOnAdd();

                // Relationships
                entity.HasOne(ds => ds.Doctor)
                      .WithMany(d => d.Schedules)
                      .HasForeignKey(ds => ds.DoctorId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint - one schedule per doctor per day
                entity.HasIndex(ds => new { ds.DoctorId, ds.DayOfWeek })
                      .IsUnique();

                // Property configurations
                entity.Property(ds => ds.DayOfWeek)
                      .IsRequired();

                entity.Property(ds => ds.StartTime)
                      .IsRequired();

                entity.Property(ds => ds.EndTime)
                      .IsRequired();
            });

            // ScheduleException configuration
            builder.Entity<ScheduleException>(entity =>
            {
                entity.HasKey(se => se.Id);
                entity.Property(se => se.Id).ValueGeneratedOnAdd();

                // Relationships
                entity.HasOne(se => se.Doctor)
                      .WithMany(d => d.ScheduleExceptions)
                      .HasForeignKey(se => se.DoctorId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Indexes for performance
                entity.HasIndex(se => new { se.DoctorId, se.ExceptionDate });
                entity.HasIndex(se => se.ExceptionDate);

                // Property configurations
                entity.Property(se => se.Reason)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(se => se.Type)
                      .HasConversion<string>()
                      .HasMaxLength(20);
            });
        }
    }
}
