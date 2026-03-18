using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PacienteVacinaRepository : RepositoryBase<PACIENTE_VACINA>, IPacienteVacinaRepository
    {
        public PACIENTE_VACINA GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_VACINA> query = Db.PACIENTE_VACINA;
            query = query.Where(p => p.PAVI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_VACINA> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_VACINA> query = Db.PACIENTE_VACINA.Where(p => p.PAVI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

    }
}
