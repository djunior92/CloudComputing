using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Data
{
    public class ContatoRepository
    {
        private readonly AgendaContext ctx;

        public IQueryable<Contato> Source => ctx.ContatoSet;

        public ContatoRepository(AgendaContext context)
        {
            ctx = context;
        }

        public Contato CreateNew()
        {
            Contato result = new Contato();
            return result;
        }

        public async Task<Contato> GetAsync(int id)
        {
            Contato result = await Source.FirstOrDefaultAsync(r => r.Id == id);
            return result;
        }

        public async Task<List<Contato>> ListAsync()
        {
            List<Contato> result = await Source
                .AsNoTracking()
                .ToListAsync();

            return result;
        }

        public async Task<Contato> SaveAsync(Contato model)
        {
            Contato result = null;

            bool isnew = (model.Id == 0);

            ctx.Entry(model).State = (isnew) ? EntityState.Added : EntityState.Modified;
            if (await ctx.SaveChangesAsync() > 0)
                result = model;

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            bool result = false;

            Contato item = await GetAsync(id);
            if (item == null)
                return result;

            ctx.Entry(item).State = EntityState.Deleted;
            if (await ctx.SaveChangesAsync() > 0)
                result = true;

            return result;
        }
    }
}
