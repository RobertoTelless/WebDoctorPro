using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class RacaRepository : RepositoryBase<RACA>, IRacaRepository
    {
        public RACA GetItemById(Int32 id)
        {
            IQueryable<RACA> query = Db.RACA;
            query = query.Where(p => p.RACA_CD_ID == id);
            return query.AsNoTracking().FirstOrDefault();
        }

        public List<RACA> GetAllItens()
        {
            IQueryable<RACA> query = Db.RACA;
            return query.AsNoTracking().ToList();
        }

    }
}
