using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class LocacaoHistoricoRepository : RepositoryBase<LOCACAO_HISTORICO>, ILocacaoHistoricoRepository
    {
        public LOCACAO_HISTORICO GetItemById(Int32 id)
        {
            IQueryable<LOCACAO_HISTORICO> query = Db.LOCACAO_HISTORICO;
            query = query.Where(p => p.LOHI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<LOCACAO_HISTORICO> GetAllItens(Int32 idAss)
        {
            IQueryable<LOCACAO_HISTORICO> query = Db.LOCACAO_HISTORICO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<LOCACAO_HISTORICO> ExecuteFilter(Int32? tipo, Int32? paci, DateTime? dataInicio, DateTime? dataFim, String descricao, Int32 idAss)
        {
            List<LOCACAO_HISTORICO> lista = new List<LOCACAO_HISTORICO>();
            IQueryable<LOCACAO_HISTORICO> query = Db.LOCACAO_HISTORICO;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.LOCA_CD_ID == tipo);
            }
            if (paci != null & paci > 0)
            {
                query = query.Where(p => p.LOCACAO.PACI_CD_ID == paci);
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.LOHI_NM_OPERACAO.Contains(descricao));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOHI_DT_HISTORICO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOHI_DT_HISTORICO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOHI_DT_HISTORICO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.LOHI_DT_HISTORICO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.LOHI_DT_HISTORICO);
                lista = query.ToList<LOCACAO_HISTORICO>();
            }
            return lista;
        }

    }
}
