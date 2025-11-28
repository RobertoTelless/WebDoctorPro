using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class LinguaRepository : RepositoryBase<LINGUA>, ILinguaRepository
    {
        public LINGUA GetItemById(Int32 id)
        {
            IQueryable<LINGUA> query = Db.LINGUA;
            query = query.Where(p => p.LING_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<LINGUA> GetAllItens()
        {
            IQueryable<LINGUA> query = Db.LINGUA.Where(p => p.LING_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 