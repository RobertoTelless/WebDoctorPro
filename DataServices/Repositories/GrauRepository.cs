using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class GrauRepository : RepositoryBase<GRAU_INSTRUCAO>, IGrauRepository
    {
        public GRAU_INSTRUCAO GetItemById(Int32 id)
        {
            IQueryable<GRAU_INSTRUCAO> query = Db.GRAU_INSTRUCAO;
            query = query.Where(p => p.GRAU_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<GRAU_INSTRUCAO> GetAllItens()
        {
            IQueryable<GRAU_INSTRUCAO> query = Db.GRAU_INSTRUCAO;
            query = query.Where(p => p.GRAU_IN_ATIVO == 1);
            return query.AsNoTracking().ToList();
        }

    }
}
