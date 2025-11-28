using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class PacienteExameFisicoRepository : RepositoryBase<PACIENTE_EXAME_FISICOS>, IPacienteExameFisicoRepository
    {
        public PACIENTE_EXAME_FISICOS GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_EXAME_FISICOS> query = Db.PACIENTE_EXAME_FISICOS;
            query = query.Where(p => p.PAEF_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_EXAME_FISICOS> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_EXAME_FISICOS> query = Db.PACIENTE_EXAME_FISICOS;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
