using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class NacionalidadeRepository : RepositoryBase<NACIONALIDADE>, INacionalidadeRepository
    {
        public NACIONALIDADE GetItemById(Int32 id)
        {
            IQueryable<NACIONALIDADE> query = Db.NACIONALIDADE;
            query = query.Where(p => p.NACI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<NACIONALIDADE> GetAllItens()
        {
            IQueryable<NACIONALIDADE> query = Db.NACIONALIDADE.Where(p => p.NACI_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 