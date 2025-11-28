using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class MensagemEnviadaSistemaRepository : RepositoryBase<MENSAGENS_ENVIADAS_SISTEMA>, IMensagemEnviadaSistemaRepository
    {
        public List<MENSAGENS_ENVIADAS_SISTEMA> GetByDate(DateTime data, Int32 idAss)
        {
            List<MENSAGENS_ENVIADAS_SISTEMA> lista = new List<MENSAGENS_ENVIADAS_SISTEMA>();
            IQueryable<MENSAGENS_ENVIADAS_SISTEMA> query = Db.MENSAGENS_ENVIADAS_SISTEMA;
            query = query.Where(p => DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO) == DbFunctions.TruncateTime(DateTime.Today.Date));
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MEEN_IN_SISTEMA == 6);
            lista = query.AsNoTracking().ToList<MENSAGENS_ENVIADAS_SISTEMA>();
            return lista;
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> GetByMonth(DateTime data, Int32 idAss)
        {
            List<MENSAGENS_ENVIADAS_SISTEMA> lista = new List<MENSAGENS_ENVIADAS_SISTEMA>();
            IQueryable<MENSAGENS_ENVIADAS_SISTEMA> query = Db.MENSAGENS_ENVIADAS_SISTEMA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MEEN_IN_SISTEMA == 6);
            query = query.Where(p => DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO).Value.Month >= DbFunctions.TruncateTime(data).Value.Month);
            query = query.Where(p => DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO).Value.Year >= DbFunctions.TruncateTime(data).Value.Year);
            lista = query.AsNoTracking().ToList<MENSAGENS_ENVIADAS_SISTEMA>();
            return lista;
        }

        public MENSAGENS_ENVIADAS_SISTEMA GetItemById(Int32 id)
        {
            IQueryable<MENSAGENS_ENVIADAS_SISTEMA> query = Db.MENSAGENS_ENVIADAS_SISTEMA;
            query = query.Where(p => p.MEEN_CD_ID == id);
            return query.AsNoTracking().FirstOrDefault();
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> GetAllItens(Int32 idAss)
        {
            IQueryable<MENSAGENS_ENVIADAS_SISTEMA> query = Db.MENSAGENS_ENVIADAS_SISTEMA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MEEN_IN_SISTEMA == 6);
            query = query.OrderBy(a => a.MEEN_DT_DATA_ENVIO);
            return query.AsNoTracking().ToList();
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> ExecuteFilter(Int32? escopo, Int32? tipo, DateTime? dataInicio, DateTime? dataFim, String email, String celular, String destino, Int32 idAss)
        {
            List<MENSAGENS_ENVIADAS_SISTEMA> lista = new List<MENSAGENS_ENVIADAS_SISTEMA>();
            IQueryable<MENSAGENS_ENVIADAS_SISTEMA> query = Db.MENSAGENS_ENVIADAS_SISTEMA;
            if (dataInicio != null & dataFim == null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if (dataInicio == null & dataFim != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (dataInicio != null & dataFim != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (tipo > 0)
            {
                query = query.Where(p => p.MEEN_IN_TIPO == tipo);
            }
            if (escopo > 0)
            {
                query = query.Where(p => p.MEEN_IN_ESCOPO == escopo);
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.MEEN_EM_EMAIL_DESTINO.Contains(email));
            }
            if (!String.IsNullOrEmpty(celular))
            {
                query = query.Where(p => p.MEEN_NR_CELULAR_DESTINO.Contains(celular));
            }
            if (!String.IsNullOrEmpty(destino))
            {
                query = query.Where(p => p.MEEN_NM_ORIGEM.Contains(destino));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.MEEN_IN_SISTEMA == 6);
                query = query.OrderBy(a => a.MEEN_DT_DATA_ENVIO);
                lista = query.AsNoTracking().ToList<MENSAGENS_ENVIADAS_SISTEMA>();
            }
            return lista;
        }

    }
}
 