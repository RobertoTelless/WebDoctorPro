using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PacienteLoginRepository : RepositoryBase<PACIENTE_LOGIN>, IPacienteLoginRepository
    {
        public List<PACIENTE_LOGIN> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_LOGIN> query = Db.PACIENTE_LOGIN.Where(p => p.PALO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public PACIENTE_LOGIN GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_LOGIN> query = Db.PACIENTE_LOGIN.Where(p => p.PALO_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 