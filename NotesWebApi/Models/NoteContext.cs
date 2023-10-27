using Microsoft.EntityFrameworkCore;

namespace NotesWebApi.Models
{
    public class NoteContext : DbContext
    {
        public NoteContext(DbContextOptions<NoteContext> options)
        : base(options)
        {
        }
        public DbSet<Note> Notes { get; set; } = null!;
    }
}
