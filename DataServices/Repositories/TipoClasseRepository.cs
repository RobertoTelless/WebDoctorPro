using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoClasseRepository : RepositoryBase<TIPO_CARTEIRA_CLASSE>, ITipoClasseRepository
    {
        public TIPO_CARTEIRA_CLASSE GetItemById(Int32 id)
        {
            IQueryable<TIPO_CARTEIRA_CLASSE> query = Db.TIPO_CARTEIRA_CLASSE;
            query = query.Where(p => p.TICL_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_CARTEIRA_CLASSE> GetAllItens()
        {
            IQueryable<TIPO_CARTEIRA_CLASSE> query = Db.TIPO_CARTEIRA_CLASSE;
            return query.ToList();
        }

    }
}
