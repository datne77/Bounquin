using BookReadingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BookReadingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Chapter>()
                .HasOne(c => c.Book)
                .WithMany(b => b.Chapters)  // ✅ Một Book có nhiều Chapters
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Khi xóa Book, xóa luôn Chapters của nó

            // ✅ Khi xóa Chapter, tự động xóa tất cả Comment bên trong
            builder.Entity<Chapter>()
                .HasMany(c => c.Comments)
                .WithOne(c => c.Chapter)
                .HasForeignKey(c => c.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Khi xóa User, tất cả Comment của User sẽ bị xóa
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập quan hệ một-nhiều giữa Category và Book
            builder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Nếu xóa thể loại, xóa luôn sách thuộc thể loại đó

            builder.Entity<Book>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa sách khi xóa User
                                                    // Thiết lập giá trị mặc định cho CreatedDate trong Book

            // ✅ Thiết lập quan hệ giữa Report và ApplicationUser
            builder.Entity<Report>()
                .HasOne(r => r.Reporter)
                .WithMany()
                .HasForeignKey(r => r.ReporterId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa báo cáo khi xóa User

            // ✅ Thiết lập quan hệ giữa Report và Book
            builder.Entity<Report>()
                .HasOne(r => r.Book)
                .WithMany()
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa báo cáo khi xóa Book
            builder.Entity<Book>()
                .Property(b => b.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            // Seed dữ liệu quyền (Roles)
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "A1234567-B89C-123D-E456-426614174000", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "B1234567-B89C-123D-E456-426614174001", Name = "User", NormalizedName = "USER" }
            );
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BannerSlide> BannerSlides { get; set; }
        public DbSet<ReadingHistory> ReadingHistories { get; set; }
        public DbSet<ClassicBookEntry> ClassicBookEntries { get; set; }


    }
}
