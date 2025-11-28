using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class SexoRepository : RepositoryBase<SEXO>, ISexoRepository
    {
        public SEXO GetItemById(Int32 id)
        {
            IQueryable<SEXO> query = Db.SEXO;
            query = query.Where(p => p.SEXO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<SEXO> GetAllItens()
        {
            IQueryable<SEXO> query = Db.SEXO;
            return query.ToList();
        }

    }
}
