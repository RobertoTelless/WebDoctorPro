using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class AvisoLembreteRepository : RepositoryBase<AVISO_LEMBRETE>, IAvisoLembreteRepository
    {
        public AVISO_LEMBRETE GetItemById(Int32 id)
        {
            IQueryable<AVISO_LEMBRETE> query = Db.AVISO_LEMBRETE;
            query = query.Where(p => p.AVIS_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<AVISO_LEMBRETE> GetAllItens(Int32 idAss)
        {
            IQueryable<AVISO_LEMBRETE> query = Db.AVISO_LEMBRETE.Where(p => p.AVIS_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<AVISO_LEMBRETE> ExecuteFilter(String titulo, DateTime? dataInicio, DateTime? dataFim, Int32? ciente, Int32 idAss)
        {
            List<AVISO_LEMBRETE> lista = new List<AVISO_LEMBRETE>();
            IQueryable<AVISO_LEMBRETE> query = Db.AVISO_LEMBRETE;
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.AVIS_NM_TITULO.Contains(titulo));
            }
            if (ciente != null & ciente > 0)
            {
                query = query.Where(p => p.AVIS_IN_CIENTE == ciente);
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.AVIS_DT_AVISO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.AVIS_DT_AVISO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.AVIS_DT_AVISO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.AVIS_DT_AVISO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.AVIS_IN_ATIVO == 1);
                query = query.OrderBy(a => a.AVIS_DT_AVISO);
                lista = query.AsNoTracking().ToList<AVISO_LEMBRETE>();
            }
            return lista;
        }

    }
}
 