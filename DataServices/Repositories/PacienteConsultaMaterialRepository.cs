using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PacienteConsultaMaterialRepository : RepositoryBase<PACIENTE_CONSULTA_MATERIAL>, IPacienteConsultaMaterialRepository
    {
        public List<PACIENTE_CONSULTA_MATERIAL> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_CONSULTA_MATERIAL> query = Db.PACIENTE_CONSULTA_MATERIAL.Where(p => p.PCMA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public PACIENTE_CONSULTA_MATERIAL GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_CONSULTA_MATERIAL> query = Db.PACIENTE_CONSULTA_MATERIAL.Where(p => p.PCMA_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 