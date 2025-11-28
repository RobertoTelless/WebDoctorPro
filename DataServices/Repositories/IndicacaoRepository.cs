using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class IndicacaoRepository : RepositoryBase<INDICACAO>, IIndicacaoRepository
    {
        public INDICACAO GetItemById(Int32 id)
        {
            IQueryable<INDICACAO> query = Db.INDICACAO;
            query = query.Where(p => p.INDI_CD_ID == id);
            query = query.Include(p => p.INDICACAO_ACAO);
            return query.FirstOrDefault();
        }

        public List<INDICACAO> GetAllItens(Int32 idAss)
        {
            IQueryable<INDICACAO> query = Db.INDICACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.INDI_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<INDICACAO> GetAll()
        {
            IQueryable<INDICACAO> query = Db.INDICACAO;
            return query.ToList();
        }

        public List<INDICACAO> ExecuteFilter(Int32? autor, String nome, DateTime? dataInicio, DateTime? dataFim, String email, Int32? status, Int32 idAss)
        {
            List<INDICACAO> lista = new List<INDICACAO>();
            IQueryable<INDICACAO> query = Db.INDICACAO;
            if (autor != null & autor > 0)
            {
                query = query.Where(p => p.USUA_CD_ID == autor);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.INDI_NM_INDICADO.Contains(nome));
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.INDI_NM_EMAIL.Contains(email));
            }
            if (status != null & status > 0)
            {
                query = query.Where(p => p.INDI_IN_STATUS == status);
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.INDI_DT_DATA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.INDI_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.INDI_DT_DATA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.INDI_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.INDI_IN_ATIVO == 1);
                query = query.OrderBy(a => a.INDI_DT_DATA);
                lista = query.ToList<INDICACAO>();
            }
            return lista;
        }

    }
}
