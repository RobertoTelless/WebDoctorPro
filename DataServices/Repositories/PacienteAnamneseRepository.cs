using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PacienteAnamneseRepository : RepositoryBase<PACIENTE_ANAMNESE>, IPacienteAnamneseRepository
    {
        public PACIENTE_ANAMNESE GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_ANAMNESE> query = Db.PACIENTE_ANAMNESE;
            query = query.Where(p => p.PAAM_CD_ID == id);
            query = query.Include(p => p.PACIENTE_CONSULTA);
            query = query.Include(p => p.PACIENTE_CONSULTA.VALOR_CONSULTA);
            query = query.Include(p => p.PACIENTE);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_ANAMNESE> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_ANAMNESE> query = Db.PACIENTE_ANAMNESE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

    }
}
