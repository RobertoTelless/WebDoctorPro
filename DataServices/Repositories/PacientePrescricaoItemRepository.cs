using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PacientePrescricaoItemRepository : RepositoryBase<PACIENTE_PRESCRICAO_ITEM>, IPacientePrescricaoItemRepository
    {
        public List<PACIENTE_PRESCRICAO_ITEM> GetAllItens(Int32 idUsu)
        {
            IQueryable<PACIENTE_PRESCRICAO_ITEM> query = Db.PACIENTE_PRESCRICAO_ITEM.Where(p => p.PAPI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            query = query.OrderBy(a => a.PAPI_NM_REMEDIO);
            return query.ToList();
        }

        public PACIENTE_PRESCRICAO_ITEM GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_PRESCRICAO_ITEM> query = Db.PACIENTE_PRESCRICAO_ITEM.Where(p => p.PAPI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_PRESCRICAO_ITEM> ExecuteFilter(Int32? forma, String nome, DateTime? dataInicio, DateTime? dataFim, String remedio, String generico, Int32 idAss)
        {
            List<PACIENTE_PRESCRICAO_ITEM> lista = new List<PACIENTE_PRESCRICAO_ITEM>();
            IQueryable<PACIENTE_PRESCRICAO_ITEM> query = Db.PACIENTE_PRESCRICAO_ITEM;
            if (forma != null & forma > 0)
            {
                query = query.Where(p => p.TIFO_CD_ID == forma);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PACIENTE.PACI_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(remedio))
            {
                query = query.Where(p => p.PAPI_NM_REMEDIO.Contains(remedio));
            }
            if (!String.IsNullOrEmpty(generico))
            {
                query = query.Where(p => p.PAPI_NM_GENERICO.Contains(generico));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PACIENTE_PRESCRICAO.PAPR_DT_EMISSAO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PACIENTE_PRESCRICAO.PAPR_DT_EMISSAO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PACIENTE_PRESCRICAO.PAPR_DT_EMISSAO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.PACIENTE_PRESCRICAO.PAPR_DT_EMISSAO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PAPI_IN_ATIVO == 1);
                query = query.OrderBy(a => a.PAPI_NM_REMEDIO);
                lista = query.ToList<PACIENTE_PRESCRICAO_ITEM>();
            }
            return lista;
        }

    }
}
