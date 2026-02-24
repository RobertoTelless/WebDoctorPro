using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class AreaPacienteRepository : RepositoryBase<AREA_PACIENTE>, IAreaPacienteRepository
    {
        public AREA_PACIENTE GetItemById(Int32 id)
        {
            IQueryable<AREA_PACIENTE> query = Db.AREA_PACIENTE;
            query = query.Where(p => p.AREA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<AREA_PACIENTE> GetAllItens(Int32 idAss)
        {
            IQueryable<AREA_PACIENTE> query = Db.AREA_PACIENTE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.AREA_IN_ATIVO == 1);
            return query.AsNoTracking().ToList();
        }
    }
}
