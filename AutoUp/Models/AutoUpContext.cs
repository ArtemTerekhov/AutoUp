using Microsoft.EntityFrameworkCore;

namespace AutoUp.Models
{
    public class AutoUpContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<ForumLink> ForumLinks { get; set; }
        public DbSet<ForumTime> ForumTimes { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<Order> Orders { get; set; }

        public AutoUpContext(DbContextOptions<AutoUpContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminEmail = "admin@mail.ru";
            string adminLogin = adminRoleName;
            string adminPassword = "123456";
            string adminTelegram = "testTelegram@test.info";
            string adminJabber = "testJabber@test.info";

            // добавляем роли
            Role adminRole = new Role { RoleId = 1, Name = adminRoleName };
            Role userRole = new Role { RoleId = 2, Name = userRoleName };
            User adminUser = new User
            {
                UserId = 1,
                Email = adminEmail,
                Password = adminPassword,
                RoleId = adminRole.RoleId,
                Login = adminLogin,
                Telegram = adminTelegram,
                Jabber = adminJabber
            };
            Parameter values = new Parameter
            {
                Id = 1,
                PriceCurrency = "EUR",
                ReceiveCurrency = "BTC",
                Title = "Оплата сервиса",
                Description = "",
                CallbackUrl = "http://autoup.top/payments/coingatecallback",
                SuccessUrl = "http://autoup.top/payments/done",
                CancelUrl = "http://autoup.top/payments/done",
                Token = "pQ7sX4uX7jP5kI8y",
                AuthToken = "KWTqc5y3MrfE777ewVboTeDU38W8PPvyooR6KN6a"
            };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            modelBuilder.Entity<Parameter>().HasData(new Parameter[] { values });

            base.OnModelCreating(modelBuilder);
        }
    }
}
