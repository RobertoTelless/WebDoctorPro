using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class AcessoMetodoRepository : RepositoryBase<ACESSO_METODO>, IAcessoMetodoRepository
    {
        public ACESSO_METODO GetItemById(Int32 id)
        {
            IQueryable<ACESSO_METODO> query = Db.ACESSO_METODO;
            query = query.Where(p => p.ACES_CD_ID == id);
            return query.AsNoTracking().FirstOrDefault();
        }

        public List<ACESSO_METODO> GetAllItens(Int32 idAss)
        {
            IQueryable<ACESSO_METODO> query = Db.ACESSO_METODO.Where(p => p.ACES_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<ACESSO_METODO> ExecuteFilter(Int32? usuario, DateTime? dataInicio, DateTime? dataFim, String sigla, String entidade, String metodo, Int32 idAss)
        {
            List<ACESSO_METODO> lista = new List<ACESSO_METODO>();
            IQueryable<ACESSO_METODO> query = Db.ACESSO_METODO;
            if (usuario != null & usuario > 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usuario);
            }
            if (dataInicio != null & dataFim == null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.ACES_DT_ACESSO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if (dataInicio == null & dataFim != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.ACES_DT_ACESSO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (dataInicio != null & dataFim != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.ACES_DT_ACESSO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.ACES_DT_ACESSO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (!String.IsNullOrEmpty(sigla))
            {
                query = query.Where(p => p.ACES_SG_ACESSO.ToUpper() == sigla.ToUpper());
            }
            if (!String.IsNullOrEmpty(entidade))
            {
                query = query.Where(p => p.ACES_NM_CONTROLLER.Contains(entidade));
            }
            if (!String.IsNullOrEmpty(metodo))
            {
                query = query.Where(p => p.ACES_NM_METHOD.Contains(metodo));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.ACES_IN_ATIVO == 1);
                query = query.OrderBy(a => a.ACES_IN_SISTEMA == 6);
                lista = query.AsNoTracking().ToList<ACESSO_METODO>();
            }
            return lista;
        }
    }
}
 