using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class SolicitacaoRepository : RepositoryBase<SOLICITACAO>, ISolicitacaoRepository
    {
        public SOLICITACAO GetItemById(Int32 id)
        {
            IQueryable<SOLICITACAO> query = Db.SOLICITACAO;
            query = query.Where(p => p.SOLI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<SOLICITACAO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<SOLICITACAO> query = Db.SOLICITACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<SOLICITACAO> GetAllItens(Int32 idAss)
        {
            IQueryable<SOLICITACAO> query = Db.SOLICITACAO.Where(p => p.SOLI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public SOLICITACAO CheckExist(SOLICITACAO item, Int32 idAss)
        {
            IQueryable<SOLICITACAO> query = Db.SOLICITACAO;
            query = query.Where(p => p.SOLI_NM_TITULO == item.SOLI_NM_TITULO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.SOLI_IN_ATIVO == 1);
            query = query.Where(p => p.TIEX_CD_ID == item.TIEX_CD_ID);
            return query.AsNoTracking().FirstOrDefault();
        }

        public List<SOLICITACAO> ExecuteFilter(Int32? tipo, String titulo, String descricao, Int32 idAss)
        {
            List<SOLICITACAO> lista = new List<SOLICITACAO>();
            IQueryable<SOLICITACAO> query = Db.SOLICITACAO;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.TIEX_CD_ID == tipo);
            }
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.SOLI_NM_TITULO.Contains(titulo));
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.SOLI_DS_DESCRICAO.Contains(descricao));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.SOLI_IN_ATIVO == 1);
                lista = query.AsNoTracking().ToList<SOLICITACAO>();
            }
            return lista;
        }
    }
}
 