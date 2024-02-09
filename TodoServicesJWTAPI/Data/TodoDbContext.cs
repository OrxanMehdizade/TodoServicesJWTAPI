using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoServicesJWTAPI.Models.Entities;

namespace TodoServicesJWTAPI.Data
{
    public class TodoDbContext : IdentityDbContext<AppUser>
    {
        public TodoDbContext(DbContextOptions options) : base(options) { }

        public DbSet<TodoItem> TodoItems => Set<TodoItem>();
        public DbSet<ProductItem> Products => Set<ProductItem>();
        public DbSet<Category> Categories => Set<Category>();

    }
}
