using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PagamentoRepository : RepositoryBase<CONSULTA_PAGAMENTO>, IPagamentoRepository
    {
        public CONSULTA_PAGAMENTO GetItemById(Int32 id)
        {
            IQueryable<CONSULTA_PAGAMENTO> query = Db.CONSULTA_PAGAMENTO;
            query = query.Where(p => p.COPA_CD_ID == id);
            query = query.Include(p => p.TIPO_PAGAMENTO);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public List<CONSULTA_PAGAMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<CONSULTA_PAGAMENTO> query = Db.CONSULTA_PAGAMENTO.Where(p => p.COPA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.TIPO_PAGAMENTO);
            query = query.Include(p => p.USUARIO);
            return query.AsNoTracking().ToList();
        }

        public List<CONSULTA_PAGAMENTO> ExecuteFilter(Int32? tipo, String nome, String favorecido, DateTime? dataInicio, DateTime? dataFim, Int32? quitado, Int32 idAss)
        {
            List<CONSULTA_PAGAMENTO> lista = new List<CONSULTA_PAGAMENTO>();
            IQueryable<CONSULTA_PAGAMENTO> query = Db.CONSULTA_PAGAMENTO;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.TIPA_CD_ID == tipo);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.COPA_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(favorecido))
            {
                query = query.Where(p => p.COPA_NM_FAVORECIDO.Contains(favorecido));
            }
            if (quitado != null)
            {
                query = query.Where(p => p.COPA_IN_PAGO == quitado);
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.COPA_DT_VENCIMENTO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.COPA_DT_VENCIMENTO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.COPA_DT_VENCIMENTO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.COPA_DT_VENCIMENTO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.COPA_IN_ATIVO == 1);
                query = query.OrderBy(a => a.COPA_DT_VENCIMENTO);
                lista = query.AsNoTracking().ToList<CONSULTA_PAGAMENTO>();
            }
            return lista;
        }

    }
}
