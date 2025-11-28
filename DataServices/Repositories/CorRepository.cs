using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
namespace DataServices.Repositories
{
    public class CorRepository : RepositoryBase<COR>, ICorRepository
    {
        public COR GetItemById(Int32 id)
        {
            IQueryable<COR> query = Db.COR;
            query = query.Where(p => p.COR1_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<COR> GetAllItens()
        {
            IQueryable<COR> query = Db.COR;
            return query.ToList();
        }

    }
}
