namespace WebApplication3.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class dm150321db : DbContext
    {
        public dm150321db()
            : base("name=dm150321db1")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Auction> Auction { get; set; }
        public virtual DbSet<Bid> Bid { get; set; }
        public virtual DbSet<BuildVersion> BuildVersion { get; set; }
        public virtual DbSet<ErrorLog> ErrorLog { get; set; }
        public virtual DbSet<InformationsForAdministrator> InformationsForAdministrator { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserClaim> UserClaim { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>()
                .Property(e => e.AuctionRowVersion)
                .IsFixedLength();

            modelBuilder.Entity<InformationsForAdministrator>()
                .Property(e => e.ValueToken)
                .HasPrecision(10, 3);

            modelBuilder.Entity<Order>()
                .Property(e => e.RealPrice)
                .HasPrecision(10, 3);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Role1)
                .Map(m => m.ToTable("UserRole").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<User>()
                .HasMany(e => e.AuctionOwner)
                .WithOptional(e => e.Owner)
                .HasForeignKey(e => e.IdOwner);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AuctionWon)
                .WithOptional(e => e.Winner)
                .HasForeignKey(e => e.IdWinner);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Bid)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.IdUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Order)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);
        }
    }
}
