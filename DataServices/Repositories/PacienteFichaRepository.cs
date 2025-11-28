using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PacienteFichaRepository : RepositoryBase<PACIENTE_FICHA>, IPacienteFichaRepository
    {
        public List<PACIENTE_FICHA> GetAllItens()
        {
            return Db.PACIENTE_FICHA.ToList();
        }

        public PACIENTE_FICHA GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_FICHA> query = Db.PACIENTE_FICHA.Where(p => p.PAFC_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 