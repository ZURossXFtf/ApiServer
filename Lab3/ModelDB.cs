using Microsoft.EntityFrameworkCore;

namespace Lab3
{
    public class ModelDB:DbContext
    {
        public ModelDB(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Registration>? Registrations { get; set; }
        public DbSet<Assortiment>? Assortiments { get; set; }
        public DbSet<User>? Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assortiment>().HasData(
                new Assortiment { Id = 1, Kod = "001", Name = "бездари", Price = 100, },
                new Assortiment { Id = 2, Kod = "002", Name = "бестолочи", Price = 200, }
                );
            modelBuilder.Entity<Registration>().HasData(
                new Registration
                {
                    Id = 1,
                    Name = "perviy",
                    Weight = 1,
                    Cost = 10,
                    DateConfirm = new DateTime(2003, 12, 8),
                    AssortimentId = 1
                },
                new Registration
                {
                    Id = 2,
                    Name = "vtoroy",
                    Weight = 2,
                    Cost = 20,
                    DateConfirm = new DateTime(2004, 12, 8),
                    AssortimentId = 2
                },
                new Registration
                {
                    Id = 3,
                    Name = "tretiy",
                    Weight = 3,
                    Cost = 30,
                    DateConfirm = new DateTime(2005, 12, 8),
                    AssortimentId = 3
                });

    //        modelBuilder.Entity<User>().HasData(
    //            new User { id = 1, EMail = "kosha@mail.ru", Password = "123456" },
    //new User { id = 1, EMail = "zinovievmaksim@mail.ru", Password = "11111" }
    //            );
        }
    }
}
