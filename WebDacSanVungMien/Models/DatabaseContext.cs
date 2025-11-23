using Microsoft.EntityFrameworkCore;

namespace WebDacSanVungMien.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        // ----------------------------------------------------
        // 1. Khai báo các DbSet (Tương ứng với các Bảng trong SQL)
        // ----------------------------------------------------
        public DbSet<Region> Regions { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        // ----------------------------------------------------
        // 2. Cấu hình Model và Mối quan hệ (Fluent API)
        // ----------------------------------------------------
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Bảng UserFavorite (Bảng liên kết n-nhiều)
            modelBuilder.Entity<UserFavorite>()
                .HasKey(uf => uf.FavoriteID); // Thiết lập khóa chính đơn giản

            // Mối quan hệ giữa User và UserFavorite (1-nhiều)
            modelBuilder.Entity<UserFavorite>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.UserFavorites)
                .HasForeignKey(uf => uf.UserID);

            // Mối quan hệ giữa Specialty và UserFavorite (1-nhiều)
            modelBuilder.Entity<UserFavorite>()
                .HasOne(uf => uf.Specialty)
                .WithMany(s => s.UserFavorites)
                .HasForeignKey(uf => uf.SpecialtyID);

            // Cấu hình Bảng Rating (Bổ sung)
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserID);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Specialty)
                .WithMany(s => s.Ratings)
                .HasForeignKey(r => r.SpecialtyID);

            // Cấu hình các ràng buộc duy nhất (Unique Constraints)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Cấu hình dữ liệu khởi tạo (Seeding Data) cho bảng Role
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleID = 1, RoleName = "Admin" },
                new Role { RoleID = 2, RoleName = "Member" }
            );
        }
    }
}
