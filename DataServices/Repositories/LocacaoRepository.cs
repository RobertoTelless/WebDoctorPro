using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class LocacaoRepository : RepositoryBase<LOCACAO>, ILocacaoRepository
    {
        public LOCACAO CheckExist(LOCACAO item, Int32 idAss)
        {
            IQueryable<LOCACAO> query = Db.LOCACAO;
            query = query.Where(p => p.PACI_CD_ID == item.PACI_CD_ID);
            query = query.Where(p => p.PROD_CD_ID == item.PROD_CD_ID);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.LOCA_IN_ATIVO == 1);
            query = query.Where(p => p.LOCA_IN_STATUS == 0 || p.LOCA_IN_STATUS == 1 || p.LOCA_IN_STATUS == 3);
            return query.AsNoTracking().FirstOrDefault();
        }

        public LOCACAO GetItemById(Int32 id)
        {
            IQueryable<LOCACAO> query = Db.LOCACAO;
            query = query.Where(p => p.LOCA_CD_ID == id);
            query = query.Include(p => p.LOCACAO_PARCELA);
            query = query.Include(p => p.LOCACAO_HISTORICO);
            query = query.Include(p => p.LOCACAO_OCORRENCIA);
            query = query.Include(p => p.LOCACAO_ANEXO);
            query = query.Include(p => p.LOCACAO_ANOTACAO);
            return query.FirstOrDefault();
        }

        public List<LOCACAO> GetByCPF(String cpf)
        {
            IQueryable<LOCACAO> query = Db.LOCACAO.Where(p => p.LOCA_IN_ATIVO == 1);
            query = query.Where(p => p.PACIENTE.PACI_NR_CPF == cpf);
            return query.ToList();
        }

        public List<LOCACAO> GetAllItens(Int32 idAss)
        {
            IQueryable<LOCACAO> query = Db.LOCACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.LOCA_IN_ATIVO == 1);
            return query.AsNoTracking().ToList();
        }

        public List<LOCACAO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<LOCACAO> query = Db.LOCACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<LOCACAO> ExecuteFilter(String paciente, String prod, DateTime? dataInicio, DateTime? dataFim, Int32? status, String numero, Int32 idAss)
        {
            List<LOCACAO> lista = new List<LOCACAO>();
            IQueryable<LOCACAO> query = Db.LOCACAO;
            if (!String.IsNullOrEmpty(paciente))
            {
                query = query.Where(p => p.PACIENTE.PACI_NM_NOME.Contains(paciente));
            }
            if (!String.IsNullOrEmpty(prod))
            {
                query = query.Where(p => p.PRODUTO.PROD_NM_NOME.Contains(prod));
            }
            if (status != null & status > 0)
            {
                query = query.Where(p => p.LOCA_IN_STATUS == status);
            }
            if (!String.IsNullOrEmpty(numero))
            {
                query = query.Where(p => p.LOCA_NR_NUMERO.Contains(numero));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOCA_DT_INICIO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOCA_DT_INICIO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOCA_DT_INICIO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.LOCA_DT_INICIO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.LOCA_IN_ATIVO == 1);
                query = query.OrderBy(a => a.LOCA_DT_INICIO);
                lista = query.AsNoTracking().ToList<LOCACAO>();
            }
            return lista;
        }

    }
}
