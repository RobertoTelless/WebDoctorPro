using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class PacienteContatoRepository : RepositoryBase<PACIENTE_CONTATO>, IPacienteContatoRepository
    {
        public PACIENTE_CONTATO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_CONTATO> query = Db.PACIENTE_CONTATO;
            query = query.Where(p => p.PACO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_CONTATO> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_CONTATO> query = Db.PACIENTE_CONTATO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
