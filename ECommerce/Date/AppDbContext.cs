using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Api.Date
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().HasMany(u => u.Orders).WithOne(o => o.User).HasForeignKey(o => o.UserId);
            builder.Entity<ApplicationUser>().HasMany(u => u.Payments).WithOne(p => p.User).HasForeignKey(p => p.UserId);
            builder.Entity<ApplicationUser>().HasOne(u => u.Cart).WithOne(c => c.User).HasForeignKey<Cart>(c => c.UserId);
            builder.Entity<Product>().HasMany(p => p.OrderItems).WithOne(oi => oi.Product).HasForeignKey(oi => oi.ProductId);
            builder.Entity<Product>().HasMany(p => p.CartItems).WithOne(ci => ci.Product).HasForeignKey(ci => ci.ProductId);
            builder.Entity<Cart>().HasMany(c => c.CartItems).WithOne(ci => ci.Cart).HasForeignKey(ci => ci.CartId);
            builder.Entity<Order>().HasMany(o => o.OrderItems).WithOne(oi => oi.Order).HasForeignKey(oi => oi.OrderId);
            builder.Entity<Category>().HasMany(c => c.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId);
            builder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Entity<CartItem>().Property(ci => ci.Price).HasColumnType("decimal(18,2)");
            builder.Entity<OrderItem>().Property(oi => oi.Price).HasColumnType("decimal(18,2)");
            builder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
        }

    }

    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedDate { get; set; }

        public Cart Cart { get; set; }

        public List<Order> Orders { get; set; }

        public List<Payment> Payments { get; set; }
    }


}
