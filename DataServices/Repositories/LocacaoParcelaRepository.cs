using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class LocacaoParcelaRepository : RepositoryBase<LOCACAO_PARCELA>, ILocacaoParcelaRepository
    {
        public List<LOCACAO_PARCELA> GetAllItens(Int32 idAss)
        {
            IQueryable<LOCACAO_PARCELA> query = Db.LOCACAO_PARCELA.Where(p => p.LOPA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public LOCACAO_PARCELA GetItemById(Int32 id)
        {
            IQueryable<LOCACAO_PARCELA> query = Db.LOCACAO_PARCELA.Where(p => p.LOPA_CD_ID == id);
            query = query.Include(p => p.LOCACAO);
            return query.FirstOrDefault();
        }

        public List<LOCACAO_PARCELA> ExecuteFilter(Int32? locacao, Int32? paci, DateTime? dataInicio, DateTime? dataFim, String descricao, Int32 idAss)
        {
            List<LOCACAO_PARCELA> lista = new List<LOCACAO_PARCELA>();
            IQueryable<LOCACAO_PARCELA> query = Db.LOCACAO_PARCELA;
            if (locacao != null & locacao > 0)
            {
                query = query.Where(p => p.LOCA_CD_ID == locacao);
            }
            if (paci != null & paci > 0)
            {
                query = query.Where(p => p.LOCACAO.PACI_CD_ID == paci);
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.LOPA_NM_PARCELAS.Contains(descricao));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOPA_DT_VENCIMENTO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOPA_DT_VENCIMENTO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOPA_DT_VENCIMENTO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.LOPA_DT_VENCIMENTO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.LOPA_DT_VENCIMENTO);
                lista = query.ToList<LOCACAO_PARCELA>();
            }
            return lista;
        }

    }
}
 