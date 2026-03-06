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

        public List<AREA_PACIENTE> ExecuteFilter(String paciente, DateTime? dataInicio, DateTime? dataFim, Int32? tipo, Int32 idAss)
        {
            List<AREA_PACIENTE> lista = new List<AREA_PACIENTE>();
            IQueryable<AREA_PACIENTE> query = Db.AREA_PACIENTE;
            if (!String.IsNullOrEmpty(paciente))
            {
                query = query.Where(p => p.PACIENTE.PACI_NM_NOME.Contains(paciente));
            }
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.AREA_IN_TIPO == tipo);
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.AREA_DT_ENTRADA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.AREA_DT_ENTRADA) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.AREA_DT_ENTRADA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.AREA_DT_ENTRADA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.AREA_IN_ATIVO == 1);
                query = query.OrderBy(a => a.AREA_DT_ENTRADA);
                lista = query.AsNoTracking().ToList<AREA_PACIENTE>();
            }
            return lista;
        }

    }
}
