using Assistant.Desktop.Entities;

using Microsoft.EntityFrameworkCore;

namespace Assistant.Desktop.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
}