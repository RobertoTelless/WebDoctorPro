using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProspectaRepository : RepositoryBase<PROSPECTA_MAIL>, IProspectaRepository
    {

        public PROSPECTA_MAIL GetItemById(Int32 id)
        {
            IQueryable<PROSPECTA_MAIL> query = Db.PROSPECTA_MAIL;
            query = query.Where(p => p.MAIL_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PROSPECTA_MAIL> GetAllItens()
        {
            IQueryable<PROSPECTA_MAIL> query = Db.PROSPECTA_MAIL;
            query = query.Where(p => p.MAIL_IN_ATIVO == 1);
            return query.AsNoTracking().ToList();
        }

        public List<PROSPECTA_MAIL> ExecuteFilter(DateTime? dataInicio, String cidade, String uf, Int32? enviado)
        {
            List<PROSPECTA_MAIL> lista = new List<PROSPECTA_MAIL>();
            IQueryable<PROSPECTA_MAIL> query = Db.PROSPECTA_MAIL;


            if ((dataInicio !=  null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MAIL_DT_ENTRADA) == DbFunctions.TruncateTime(dataInicio));
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.MAIL_NM_CIDADE.Contains(cidade));
            }
            if (!String.IsNullOrEmpty(uf))
            {
                query = query.Where(p => p.MAIL_NM_UF == uf);
            }
            if (enviado != null)
            {
                if (enviado == 1)
                {
                    query = query.Where(p => p.MAIL_IN_ENVIOS > 0);
                }
                else
                {
                    query = query.Where(p => p.MAIL_IN_ENVIOS == 0);
                }
            }
            if (query != null)
            {
                query = query.Where(p => p.MAIL_IN_ATIVO == 1);
                query = query.OrderBy(a => a.MAIL_NM_UF).ThenBy(p => p.MAIL_NM_CIDADE).ThenBy(p => p.MAIL_NM_NOME);
                lista = query.AsNoTracking().ToList<PROSPECTA_MAIL>();
            }
            return lista;
        }

    }
}
