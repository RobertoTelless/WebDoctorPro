using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoFormaRepository : RepositoryBase<TIPO_FORMA>, ITipoFormaRepository
    {
        public TIPO_FORMA GetItemById(Int32 id)
        {
            IQueryable<TIPO_FORMA> query = Db.TIPO_FORMA;
            query = query.Where(p => p.TIFO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_FORMA> GetAllItens()
        {
            IQueryable<TIPO_FORMA> query = Db.TIPO_FORMA;
            return query.ToList();
        }

    }
}
