using Microsoft.EntityFrameworkCore;

namespace Agenda.Data
{
    public class AgendaContext : DbContext
    {
        public DbSet<Contato> ContatoSet { get; set; }
        public DbSet<Lembrete> LembreteSet { get; set; }

        public AgendaContext() : base()
        {
            
        }

        public AgendaContext(DbContextOptions<AgendaContext> options) : base(options)
        {

        }
    }
}
