using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class VacinaRepository : RepositoryBase<VACINA>, IVacinaRepository
    {
        public VACINA GetItemById(Int32 id)
        {
            IQueryable<VACINA> query = Db.VACINA;
            query = query.Where(p => p.VACI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<VACINA> GetAllItens(Int32 idAss)
        {
            IQueryable<VACINA> query = Db.VACINA.Where(p => p.VACI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
