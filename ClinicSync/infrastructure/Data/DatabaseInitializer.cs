//using Core.Entities;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace infrastructure.Data
//{
//    public class DatabaseInitializer
//    {
//        public static async Task Initialize(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
//        {
//            // Create database if not exists
//            await context.Database.MigrateAsync();

//            // Create roles
//            var roles = new[] { "Patient", "Doctor", "Admin" };
//            foreach (var role in roles)
//            {
//                if (!await roleManager.RoleExistsAsync(role))
//                {
//                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
//                }
//            }

//            // Create default admin
//            var adminEmail = "admin@healthsync.com";
//            var adminUser = await userManager.FindByEmailAsync(adminEmail);

//            if (adminUser == null)
//            {
//                var admin = new AppUser
//                {
//                    Id = Guid.NewGuid(),
//                    FullName = "System Administrator",
//                    UserName = adminEmail,
//                    Email = adminEmail,
//                    EmailConfirmed = false,
//                    Role = UserRole.Admin,
//                    IsActive = true,
//                    CreatedAt = DateTime.UtcNow,
//                    ProfilePictureUrl = "/assets/images/avatars/admin-avatar.png" // ✅ صورة المدير
//                };

//                var result = await userManager.CreateAsync(admin, "Admin123!");
//                if (result.Succeeded)
//                {
//                    await userManager.AddToRoleAsync(admin, "Admin");
//                }
//            }

//            // ✅ إضافة 5 أطباء افتراضيين مع صور
//            await CreateDefaultDoctors(context, userManager);
//        }

//        private static async Task CreateDefaultDoctors(ApplicationDbContext context, UserManager<AppUser> userManager)
//        {
//            // التحقق من وجود تخصصات أولاً
//            var specialties = await context.Specialties.ToListAsync();
//            if (!specialties.Any())
//            {
//                // إذا لم توجد تخصصات، إنشائها
//                specialties = new List<Specialty>
//                {
//                    new Specialty { Id = Guid.NewGuid(), Name = "Cardiology", Description = "Heart and cardiovascular system", IsActive = true },
//                    new Specialty { Id = Guid.NewGuid(), Name = "Dermatology", Description = "Skin, hair, and nails", IsActive = true },
//                    new Specialty { Id = Guid.NewGuid(), Name = "Pediatrics", Description = "Children's health", IsActive = true },
//                    new Specialty { Id = Guid.NewGuid(), Name = "Orthopedics", Description = "Bones and muscles", IsActive = true },
//                    new Specialty { Id = Guid.NewGuid(), Name = "Neurology", Description = "Nervous system disorders", IsActive = true }
//                };

//                await context.Specialties.AddRangeAsync(specialties);
//                await context.SaveChangesAsync();
//            }
//            var doctors = new[]
//             {
//        new
//        {
//            Email = "dr.ahmed.hassan@healthsync.com",
//            FullName = "Dr. Ahmed Hassan",
//            SpecialtyName = "Cardiology",
//            LicenseNumber = "CARD123456",
//            YearsOfExperience = 12,
//            ConsultationFee = 250.00m,
//            Bio = "Consultant Cardiologist with over 12 years of experience in treating heart diseases and cardiovascular conditions. Specialized in interventional cardiology and cardiac rehabilitation.",
//            ProfilePictureUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=400&h=400&fit=crop&crop=face"
//        },
//        new
//        {
//            Email = "dr.sara.mohamed@healthsync.com",
//            FullName = "Dr. Sara Mohamed",
//            SpecialtyName = "Dermatology",
//            LicenseNumber = "DERM789012",
//            YearsOfExperience = 8,
//            ConsultationFee = 200.00m,
//            Bio = "Dermatology specialist with expertise in skin diseases, cosmetic dermatology, and laser treatments. Committed to providing personalized care for all skin types.",
//            ProfilePictureUrl = "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=400&h=400&fit=crop&crop=face"
//        },
//        new
//        {
//            Email = "dr.mahmoud.ali@healthsync.com",
//            FullName = "Dr. Mahmoud Ali",
//            SpecialtyName = "Pediatrics",
//            LicenseNumber = "PEDIA345678",
//            YearsOfExperience = 15,
//            ConsultationFee = 180.00m,
//            Bio = "Senior Pediatrician with 15 years of experience in child healthcare. Specialized in neonatal care, vaccination, and childhood diseases prevention.",
//            ProfilePictureUrl = "https://images.unsplash.com/photo-1622253692010-333f2da6031d?w=400&h=400&fit=crop&crop=face"
//        },
//        new
//        {
//            Email = "dr.noura.khalid@healthsync.com",
//            FullName = "Dr. Noura Khalid",
//            SpecialtyName = "Orthopedics",
//            LicenseNumber = "ORTHO901234",
//            YearsOfExperience = 10,
//            ConsultationFee = 300.00m,
//            Bio = "Orthopedic surgeon specializing in joint replacement, sports injuries, and fracture management. Dedicated to restoring mobility and improving quality of life.",
//            ProfilePictureUrl = "https://images.unsplash.com/photo-1551601651-2a8555f1a136?w=400&h=400&fit=crop&crop=face"
//        },
//        new
//        {
//            Email = "dr.omar.farouk@healthsync.com",
//            FullName = "Dr. Omar Farouk",
//            SpecialtyName = "Neurology",
//            LicenseNumber = "NEURO567890",
//            YearsOfExperience = 14,
//            ConsultationFee = 280.00m,
//            Bio = "Neurologist with extensive experience in treating neurological disorders, epilepsy, and headaches. Focused on comprehensive neurological care and patient education.",
//            ProfilePictureUrl = "https://images.unsplash.com/photo-1582750433449-648ed127bb54?w=400&h=400&fit=crop&crop=face"
//        }
//    };


//            foreach (var doctorInfo in doctors)
//            {
//                var existingDoctor = await userManager.FindByEmailAsync(doctorInfo.Email);
//                if (existingDoctor == null)
//                {
//                    // إنشاء حساب المستخدم للطبيب
//                    var doctorUser = new AppUser
//                    {
//                        Id = Guid.NewGuid(),
//                        FullName = doctorInfo.FullName,
//                        UserName = doctorInfo.Email,
//                        Email = doctorInfo.Email,
//                        EmailConfirmed = false,
//                        Role = UserRole.Doctor,
//                        IsActive = true,
//                        CreatedAt = DateTime.UtcNow,
//                        ProfilePictureUrl = doctorInfo.ProfilePictureUrl 
//                    };


//                    var password = "Doctor123!";

//                    var result = await userManager.CreateAsync(doctorUser, password);
//                    if (result.Succeeded)
//                    {
//                        await userManager.AddToRoleAsync(doctorUser, "Doctor");

//                        // الحصول على التخصص المناسب
//                        var specialty = specialties.FirstOrDefault(s => s.Name == doctorInfo.SpecialtyName);
//                        if (specialty == null)
//                        {
//                            // إذا لم يوجد التخصص، استخدم أول تخصص متاح
//                            specialty = specialties.First();
//                        }

//                        // إنشاء الملف الطبي للدكتور
//                        var doctorProfile = new Doctor
//                        {
//                            Id = Guid.NewGuid(),
//                            UserId = doctorUser.Id,
//                            SpecialtyId = specialty.Id,
//                            LicenseNumber = doctorInfo.LicenseNumber,
//                            YearsOfExperience = doctorInfo.YearsOfExperience,
//                            ConsultationFee = doctorInfo.ConsultationFee,
//                            Bio = doctorInfo.Bio,
//                            IsApproved = true  
//                        };

//                        await context.Doctors.AddAsync(doctorProfile);
//                    }
//                }
//            }

//            await context.SaveChangesAsync();
//        }
//    }
//}

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
                    EmailConfirmed = true, // ✅ تأكيد البريد للمدير
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
                        EmailConfirmed = false, // ✅ تأكيد البريد للأطباء
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