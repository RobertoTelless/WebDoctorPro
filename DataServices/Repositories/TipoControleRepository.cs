using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoControleRepository : RepositoryBase<TIPO_CONTROLE>, ITipoControleRepository
    {
        public TIPO_CONTROLE GetItemById(Int32 id)
        {
            IQueryable<TIPO_CONTROLE> query = Db.TIPO_CONTROLE;
            query = query.Where(p => p.TICO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_CONTROLE> GetAllItens()
        {
            IQueryable<TIPO_CONTROLE> query = Db.TIPO_CONTROLE;
            return query.ToList();
        }

    }
}
