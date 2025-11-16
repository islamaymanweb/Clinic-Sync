 
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace infrastructure.Data
{
    public class DatabaseInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            // Create database if not exists
            await context.Database.MigrateAsync();

            // Create roles
            var roles = new[] { "Patient", "Doctor", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            await CreateSpecialties(context);

            await CreateAdminUser(userManager);

            // ✅ إضافة المريض الجديد قبل الأطباء
            await CreateDefaultPatient(context, userManager);

            await CreateDefaultDoctors(context, userManager);


            await CreateDoctorSchedules(context);

            await context.SaveChangesAsync();
        }

        private static async Task CreateSpecialties(ApplicationDbContext context)
        {
            if (!await context.Specialties.AnyAsync())
            {
                var specialties = new List<Specialty>
                {
                    new Specialty { Id = Guid.NewGuid(), Name = "Cardiology", Description = "Heart and cardiovascular system", IsActive = true },
                    new Specialty { Id = Guid.NewGuid(), Name = "Dermatology", Description = "Skin, hair, and nails", IsActive = true },
                    new Specialty { Id = Guid.NewGuid(), Name = "Pediatrics", Description = "Children's health", IsActive = true },
                    new Specialty { Id = Guid.NewGuid(), Name = "Orthopedics", Description = "Bones and muscles", IsActive = true },
                    new Specialty { Id = Guid.NewGuid(), Name = "Neurology", Description = "Nervous system disorders", IsActive = true }
                };

                await context.Specialties.AddRangeAsync(specialties);
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateAdminUser(UserManager<AppUser> userManager)
        {
            var adminEmail = "admin@healthsync.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new AppUser
                {
                    Id = Guid.NewGuid(),
                    FullName = "System Administrator",
                    UserName = adminEmail,
                    Email = adminEmail,
      
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ProfilePictureUrl = "/assets/images/avatars/admin-avatar.png"
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }

       
        private static async Task CreateDefaultPatient(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            var patientEmail = "patient@healthsync.com";
            var patientUser = await userManager.FindByEmailAsync(patientEmail);

            if (patientUser == null)
            {
                var patient = new AppUser
                {
                    Id = Guid.NewGuid(),
                    FullName = "Ahmed Mohamed",
                    UserName = patientEmail,
                    Email = patientEmail,
                   
                    Role = UserRole.Patient,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ProfilePictureUrl = "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=400&h=400&fit=crop&crop=face"
                };

                var result = await userManager.CreateAsync(patient, "Patient123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(patient, "Patient");

                    // إنشاء الملف الشخصي للمريض
                    var patientProfile = new Patient
                    {
                        Id = Guid.NewGuid(),
                        UserId = patient.Id,
                        DateOfBirth = new DateTime(1990, 5, 15),
                        Gender = Gender.Male,
                        PhoneNumber = "+201234567890",
                        EmergencyContact = "+201098765432"
                    };

                    await context.Patients.AddAsync(patientProfile);
                }
            }
        }

        private static async Task CreateDefaultDoctors(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            var specialties = await context.Specialties.ToListAsync();

            var doctors = new[]
            {
                new
                {
                    Email = "dr.ahmed.hassan@healthsync.com",
                    FullName = "Dr. Ahmed Hassan",
                    SpecialtyName = "Cardiology",
                    LicenseNumber = "CARD123456",
                    YearsOfExperience = 12,
                    ConsultationFee = 250.00m,
                    Bio = "Consultant Cardiologist with over 12 years of experience in treating heart diseases and cardiovascular conditions.",
                    ProfilePictureUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=400&h=400&fit=crop&crop=face"
                },
                new
                {
                    Email = "dr.sara.mohamed@healthsync.com",
                    FullName = "Dr. Sara Mohamed",
                    SpecialtyName = "Dermatology",
                    LicenseNumber = "DERM789012",
                    YearsOfExperience = 8,
                    ConsultationFee = 200.00m,
                    Bio = "Dermatology specialist with expertise in skin diseases, cosmetic dermatology, and laser treatments.",
                    ProfilePictureUrl = "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=400&h=400&fit=crop&crop=face"
                },
                new
                {
                    Email = "dr.mahmoud.ali@healthsync.com",
                    FullName = "Dr. Mahmoud Ali",
                    SpecialtyName = "Pediatrics",
                    LicenseNumber = "PEDIA345678",
                    YearsOfExperience = 15,
                    ConsultationFee = 180.00m,
                    Bio = "Senior Pediatrician with 15 years of experience in child healthcare and childhood diseases prevention.",
                    ProfilePictureUrl = "https://images.unsplash.com/photo-1622253692010-333f2da6031d?w=400&h=400&fit=crop&crop=face"
                },
                new
                {
                    Email = "dr.noura.khalid@healthsync.com",
                    FullName = "Dr. Noura Khalid",
                    SpecialtyName = "Orthopedics",
                    LicenseNumber = "ORTHO901234",
                    YearsOfExperience = 10,
                    ConsultationFee = 300.00m,
                    Bio = "Orthopedic surgeon specializing in joint replacement, sports injuries, and fracture management.",
                    ProfilePictureUrl = "https://images.unsplash.com/photo-1551601651-2a8555f1a136?w=400&h=400&fit=crop&crop=face"
                },
                new
                {
                    Email = "dr.omar.farouk@healthsync.com",
                    FullName = "Dr. Omar Farouk",
                    SpecialtyName = "Neurology",
                    LicenseNumber = "NEURO567890",
                    YearsOfExperience = 14,
                    ConsultationFee = 280.00m,
                    Bio = "Neurologist with extensive experience in treating neurological disorders, epilepsy, and headaches.",
                    ProfilePictureUrl = "https://images.unsplash.com/photo-1582750433449-648ed127bb54?w=400&h=400&fit=crop&crop=face"
                }
            };

            foreach (var doctorInfo in doctors)
            {
                var existingUser = await userManager.FindByEmailAsync(doctorInfo.Email);
                if (existingUser == null)
                {
                    var doctorUser = new AppUser
                    {
                        Id = Guid.NewGuid(),
                        FullName = doctorInfo.FullName,
                        UserName = doctorInfo.Email,
                        Email = doctorInfo.Email,
                      
                        Role = UserRole.Doctor,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        ProfilePictureUrl = doctorInfo.ProfilePictureUrl
                    };

                    var result = await userManager.CreateAsync(doctorUser, "Doctor123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(doctorUser, "Doctor");

                        var specialty = specialties.FirstOrDefault(s => s.Name == doctorInfo.SpecialtyName) ?? specialties.First();

                        var doctorProfile = new Doctor
                        {
                            Id = Guid.NewGuid(),
                            UserId = doctorUser.Id,
                            SpecialtyId = specialty.Id,
                            LicenseNumber = doctorInfo.LicenseNumber,
                            YearsOfExperience = doctorInfo.YearsOfExperience,
                            ConsultationFee = doctorInfo.ConsultationFee,
                            Bio = doctorInfo.Bio,
                            IsApproved = true
                        };

                        await context.Doctors.AddAsync(doctorProfile);
                    }
                }
            }
        }

        private static async Task CreateDoctorSchedules(ApplicationDbContext context)
        {
            if (!await context.DoctorSchedules.AnyAsync())
            {
                var doctors = await context.Doctors.ToListAsync();
                var schedules = new List<DoctorSchedule>();

                foreach (var doctor in doctors)
                {

                    var workDays = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

                    foreach (var day in workDays)
                    {
                        schedules.Add(new DoctorSchedule
                        {
                            Id = Guid.NewGuid(),
                            DoctorId = doctor.Id,
                            DayOfWeek = day,
                            StartTime = new TimeSpan(9, 0, 0),
                            EndTime = new TimeSpan(17, 0, 0),
                            IsActive = true
                        });
                    }
                }

                await context.DoctorSchedules.AddRangeAsync(schedules);
            }
        }
    }
}