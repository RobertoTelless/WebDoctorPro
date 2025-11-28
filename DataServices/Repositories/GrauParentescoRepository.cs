using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
namespace DataServices.Repositories
{
    public class GrauParentescoRepository : RepositoryBase<GRAU_PARENTESCO>, IGrauParentescoRepository
    {
        public GRAU_PARENTESCO GetItemById(Int32 id)
        {
            IQueryable<GRAU_PARENTESCO> query = Db.GRAU_PARENTESCO;
            query = query.Where(p => p.GRPA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<GRAU_PARENTESCO> GetAllItens()
        {
            IQueryable<GRAU_PARENTESCO> query = Db.GRAU_PARENTESCO;
            return query.ToList();
        }

    }
}
