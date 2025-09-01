using ApiLivros.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiLivros.Data;

// DbContext do EF Core usando InMemory. Guarda a lista de livros.
public class AppDbContext : DbContext
{
    public DbSet<Book> Books => Set<Book>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
