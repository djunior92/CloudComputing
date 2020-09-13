using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Data
{
    public class LembreteRepository
    {
        private readonly AgendaContext ctx;

        public IQueryable<Lembrete> Source => ctx.LembreteSet;

        public LembreteRepository(AgendaContext context)
        {
            ctx = context;
        }

        public Lembrete CreateNew()
        {
            Lembrete result = new Lembrete();
            return result;
        }

        public async Task<Lembrete> GetAsync(int id)
        {
            Lembrete result = await Source.FirstOrDefaultAsync(r => r.Id == id);
            return result;
        }

        public async Task<List<Lembrete>> ListAsync()
        {
            List<Lembrete> result = await Source
                .AsNoTracking()
                .ToListAsync();

            return result;
        }

        public async Task<Lembrete> SaveAsync(Lembrete model)
        {
            Lembrete result = null;

            bool isnew = (model.Id == 0);

            ctx.Entry(model).State = (isnew) ? EntityState.Added : EntityState.Modified;
            if (await ctx.SaveChangesAsync() > 0)
                result = model;

            return result;
        }        
    }
}
