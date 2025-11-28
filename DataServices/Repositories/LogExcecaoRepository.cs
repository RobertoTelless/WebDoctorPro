using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class LogExcecaoRepository : RepositoryBase<LOG_EXCECAO_NOVO>, ILogExcecaoRepository
    {
        public LOG_EXCECAO_NOVO GetItemById(Int32 id)
        {
            IQueryable<LOG_EXCECAO_NOVO> query = Db.LOG_EXCECAO_NOVO;
            query = query.Where(p => p.LOEX_CD_ID == id);
            return query.AsNoTracking().FirstOrDefault();
        }

        public List<LOG_EXCECAO_NOVO> GetAllItens(Int32 idAss)
        {
            IQueryable<LOG_EXCECAO_NOVO> query = Db.LOG_EXCECAO_NOVO.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.LOEX_IN_SISTEMA == 6);
            return query.AsNoTracking().ToList();
        }

        public List<LOG_EXCECAO_NOVO> ExecuteFilter(Int32? usuId, DateTime? data, String gerador, Int32 idAss)
        {
            List<LOG_EXCECAO_NOVO> lista = new List<LOG_EXCECAO_NOVO>();
            IQueryable<LOG_EXCECAO_NOVO> query = Db.LOG_EXCECAO_NOVO;
            if (!String.IsNullOrEmpty(gerador))
            {
                query = query.Where(p => p.LOEX_NM_GERADOR.Contains(gerador));
            }
            if (usuId != null)
            {
                query = query.Where(p => p.USUARIO.USUA_CD_ID == usuId);
            }
            if (data != null & data != DateTime.MinValue)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOEX_DT_DATA) == DbFunctions.TruncateTime(data));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.LOEX_IN_SISTEMA == 2);
                query = query.OrderByDescending(a => a.LOEX_DT_DATA);
                lista = query.AsNoTracking().ToList<LOG_EXCECAO_NOVO>();
            }
            return lista;
        }
    }
}
 