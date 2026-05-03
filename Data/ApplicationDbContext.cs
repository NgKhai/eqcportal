using Microsoft.EntityFrameworkCore;
using eqcportal.Models;

namespace eqcportal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Global query filters for soft delete ──────────────────────────
            modelBuilder.Entity<Department>()
                .HasQueryFilter(d => !d.IsDeleted);

            modelBuilder.Entity<Position>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<Employee>()
                .HasQueryFilter(e => !e.IsDeleted);

            // ── Relationships ─────────────────────────────────────────────────
            // Employee → Department (restrict delete so you can't delete a dept with employees)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee → Position (restrict delete)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            // LeaveRequest → Employee (cascade delete)
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attendance → Employee (cascade delete)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // PerformanceReview → Employee (cascade delete)
            modelBuilder.Entity<PerformanceReview>()
                .HasOne(r => r.Employee)
                .WithMany(e => e.PerformanceReviews)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Unique constraint: one Attendance record per Employee per Date ─
            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.EmployeeId, a.Date })
                .IsUnique();

            // ── Seed Data ─────────────────────────────────────────────────────
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Engineering",       Description = "Software development and infrastructure",       IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) },
                new Department { Id = 2, Name = "Human Resources",   Description = "Recruitment, onboarding, and employee welfare", IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) },
                new Department { Id = 3, Name = "Sales & Marketing", Description = "Business development and client relations",      IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) },
                new Department { Id = 4, Name = "Finance",           Description = "Accounting, budgeting, and financial planning", IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) },
                new Department { Id = 5, Name = "Operations",        Description = "Day-to-day business operations and logistics",  IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) }
            );

            // Seed Positions
            modelBuilder.Entity<Position>().HasData(
                new Position { Id = 1, Title = "Software Engineer",    Description = "Develops and maintains software applications", IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) },
                new Position { Id = 2, Title = "HR Manager",          Description = "Manages HR operations and employee relations", IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) },
                new Position { Id = 3, Title = "Sales Executive",     Description = "Handles client relationships and sales",       IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) },
                new Position { Id = 4, Title = "Accountant",          Description = "Manages financial records and reporting",      IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) },
                new Position { Id = 5, Title = "Operations Manager",  Description = "Oversees daily operations and logistics",     IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) },
                new Position { Id = 6, Title = "Junior Developer",    Description = "Entry-level software development role",       IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) },
                new Position { Id = 7, Title = "Team Lead",           Description = "Leads a team of developers or specialists",   IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1) }
            );

            // Seed Employees
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1, FullName = "Nguyen Van An",    Email = "an.nguyen@eqc.com",   Phone = "0901234567",
                    DateOfBirth = new DateTime(1990, 3, 15), Gender = "Male",   Address = "123 Le Loi, Ho Chi Minh City",
                    HireDate = new DateTime(2024, 2, 1), Salary = 25000000, IsActive = true,
                    DepartmentId = 1, PositionId = 1, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1)
                },
                new Employee
                {
                    Id = 2, FullName = "Tran Thi Bich",   Email = "bich.tran@eqc.com",   Phone = "0912345678",
                    DateOfBirth = new DateTime(1992, 7, 22), Gender = "Female", Address = "456 Nguyen Hue, Hanoi",
                    HireDate = new DateTime(2024, 3, 15), Salary = 18000000, IsActive = true,
                    DepartmentId = 2, PositionId = 2, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1)
                },
                new Employee
                {
                    Id = 3, FullName = "Le Hoang Cuong",  Email = "cuong.le@eqc.com",    Phone = "0923456789",
                    DateOfBirth = new DateTime(1988, 11, 5), Gender = "Male",   Address = "789 Tran Phu, Da Nang",
                    HireDate = new DateTime(2023, 6, 1), Salary = 22000000, IsActive = true,
                    DepartmentId = 3, PositionId = 3, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1)
                },
                new Employee
                {
                    Id = 4, FullName = "Pham Thi Dung",   Email = "dung.pham@eqc.com",   Phone = "0934567890",
                    DateOfBirth = new DateTime(1995, 5, 30), Gender = "Female", Address = "321 Hai Ba Trung, Ho Chi Minh City",
                    HireDate = new DateTime(2025, 1, 10), Salary = 16000000, IsActive = true,
                    DepartmentId = 4, PositionId = 4, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1)
                },
                new Employee
                {
                    Id = 5, FullName = "Hoang Minh Duc",  Email = "duc.hoang@eqc.com",   Phone = "0945678901",
                    DateOfBirth = new DateTime(1985, 9, 12), Gender = "Male",   Address = "654 Ly Thuong Kiet, Hanoi",
                    HireDate = new DateTime(2023, 1, 5), Salary = 30000000, IsActive = true,
                    DepartmentId = 5, PositionId = 5, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1)
                },
                new Employee
                {
                    Id = 6, FullName = "Vo Thi Em",       Email = "em.vo@eqc.com",       Phone = "0956789012",
                    DateOfBirth = new DateTime(2000, 1, 18), Gender = "Female", Address = "987 Nam Ky Khoi Nghia, Ho Chi Minh City",
                    HireDate = new DateTime(2025, 7, 1), Salary = 12000000, IsActive = true,
                    DepartmentId = 1, PositionId = 6, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1)
                },
                new Employee
                {
                    Id = 7, FullName = "Do Van Phuc",     Email = "phuc.do@eqc.com",     Phone = "0967890123",
                    DateOfBirth = new DateTime(1987, 4, 8), Gender = "Male",   Address = "135 Dien Bien Phu, Da Nang",
                    HireDate = new DateTime(2022, 9, 1), Salary = 35000000, IsActive = true,
                    DepartmentId = 1, PositionId = 7, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1)
                },
                new Employee
                {
                    Id = 8, FullName = "Nguyen Thi Giang", Email = "giang.nguyen@eqc.com", Phone = "0978901234",
                    DateOfBirth = new DateTime(1993, 12, 25), Gender = "Female", Address = "246 Cach Mang Thang 8, Ho Chi Minh City",
                    HireDate = new DateTime(2024, 11, 1), Salary = 14000000, IsActive = false,
                    DepartmentId = 3, PositionId = 3, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1)
                }
            );
        }
    }
}
