using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PacienteSolicitacaoRepository : RepositoryBase<PACIENTE_SOLICITACAO>, IPacienteSolicitacaoRepository
    {
        public PACIENTE_SOLICITACAO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_SOLICITACAO> query = Db.PACIENTE_SOLICITACAO;
            query = query.Where(p => p.PASO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_SOLICITACAO> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_SOLICITACAO> query = Db.PACIENTE_SOLICITACAO.Where(p => p.PASO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<PACIENTE_SOLICITACAO> GetAll()
        {
            IQueryable<PACIENTE_SOLICITACAO> query = Db.PACIENTE_SOLICITACAO;
            return query.AsNoTracking().ToList();
        }

        public List<PACIENTE_SOLICITACAO> GetByCPF(String cpf)
        {
            IQueryable<PACIENTE_SOLICITACAO> query = Db.PACIENTE_SOLICITACAO.Where(p => p.PASO_IN_ATIVO == 1);
            query = query.Where(p => p.PACIENTE.PACI_NR_CPF == cpf);
            return query.AsNoTracking().ToList();
        }

        public List<PACIENTE_SOLICITACAO> ExecuteFilter(Int32? tipo, String nome, DateTime? dataInicio, DateTime? dataFim, String titulo, String descricao, Int32 idAss)
        {
            List<PACIENTE_SOLICITACAO> lista = new List<PACIENTE_SOLICITACAO>();
            IQueryable<PACIENTE_SOLICITACAO> query = Db.PACIENTE_SOLICITACAO;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.TIEX_CD_ID == tipo);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PACIENTE.PACI_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.PASO_NM_TITULO.Contains(titulo));
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.PASO_TX_TEXTO.Contains(descricao));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PASO_DT_EMISSAO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PASO_DT_EMISSAO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PASO_DT_EMISSAO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.PASO_DT_EMISSAO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PASO_IN_ATIVO == 1);
                query = query.OrderByDescending(a => a.PASO_DT_EMISSAO);
                lista = query.AsNoTracking().ToList<PACIENTE_SOLICITACAO>();
            }
            return lista;
        }

    }
}
