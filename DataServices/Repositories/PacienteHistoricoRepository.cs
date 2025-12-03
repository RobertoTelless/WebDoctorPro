using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PacienteHistoricoRepository : RepositoryBase<PACIENTE_HISTORICO>, IPacienteHistoricoRepository
    {
        public PACIENTE_HISTORICO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_HISTORICO> query = Db.PACIENTE_HISTORICO;
            query = query.Where(p => p.PAHI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_HISTORICO> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_HISTORICO> query = Db.PACIENTE_HISTORICO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<PACIENTE_HISTORICO> ExecuteFilter(Int32? tipo, String operacao, DateTime? dataInicio, DateTime? dataFim, String descricao, Int32 idAss)
        {
            List<PACIENTE_HISTORICO> lista = new List<PACIENTE_HISTORICO>();
            IQueryable<PACIENTE_HISTORICO> query = Db.PACIENTE_HISTORICO;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.PAHI_IN_TIPO == tipo);
            }
            if (!String.IsNullOrEmpty(operacao))
            {
                query = query.Where(p => p.PAHI_NM_OPERACAO.Contains(operacao));
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.PAHI_DS_DESCRICAO.Contains(descricao));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAHI_DT_DATA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAHI_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAHI_DT_DATA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.PAHI_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.PAHI_DT_DATA);
                lista = query.AsNoTracking().ToList<PACIENTE_HISTORICO>();
            }
            return lista;
        }

        public List<PACIENTE_HISTORICO> ExecuteFilterGeral(Int32? tipo, String operacao, DateTime? dataInicio, DateTime? dataFim, Int32? paciente, Int32 idAss)
        {
            List<PACIENTE_HISTORICO> lista = new List<PACIENTE_HISTORICO>();
            IQueryable<PACIENTE_HISTORICO> query = Db.PACIENTE_HISTORICO;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.PAHI_IN_TIPO == tipo);
            }
            if (!String.IsNullOrEmpty(operacao))
            {
                query = query.Where(p => p.PAHI_NM_OPERACAO.Contains(operacao));
            }
            if (paciente != null & paciente > 0)
            {
                query = query.Where(p => p.PACI_CD_ID == paciente);
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAHI_DT_DATA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAHI_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAHI_DT_DATA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.PAHI_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.PAHI_DT_DATA);
                lista = query.AsNoTracking().ToList<PACIENTE_HISTORICO>();
            }
            return lista;
        }

    }
}
