using Microsoft.EntityFrameworkCore;
using BookApp.Models;

namespace BookApp
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; } = null!;
    }
}
