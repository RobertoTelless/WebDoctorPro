using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class LogRepository : RepositoryBase<LOG>, ILogRepository
    {
        public LOG GetById(Int32 id)
        {
            IQueryable<LOG> query = Db.LOG.Where(p => p.LOG_CD_ID == id);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public List<LOG> GetAllItens(Int32 idAss)
        {
            DateTime hoje = DateTime.Today.Date;
            DateTime doze = hoje.AddMonths(-12);
            
            IQueryable<LOG> query = Db.LOG.Where(p => p.LOG_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.LOG_IN_SISTEMA == 6);
            query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA).Value <= DbFunctions.TruncateTime(hoje).Value);
            query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA).Value >= DbFunctions.TruncateTime(doze).Value);
            query = query.Include(p => p.USUARIO);
            query = query.OrderByDescending(a => a.LOG_DT_DATA);
            return query.AsNoTracking().ToList();
        }

        public List<LOG> GetAllItensDataCorrente(Int32 idAss)
        {
            IQueryable<LOG> query = Db.LOG.Where(p => p.LOG_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.LOG_IN_SISTEMA == 6);
            query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA) == DbFunctions.TruncateTime(DateTime.Today.Date));
            query = query.OrderByDescending(a => a.LOG_DT_DATA);
            return query.AsNoTracking().ToList();
        }

        public List<LOG> GetLogByFaixa(DateTime inicio, DateTime final, Int32 idAss)
        {
            IQueryable<LOG> query = Db.LOG.Where(p => p.LOG_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.LOG_IN_SISTEMA == 6);
            query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA) >= DbFunctions.TruncateTime(inicio));
            query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA) <= DbFunctions.TruncateTime(final));
            query = query.OrderByDescending(a => a.LOG_DT_DATA);
            return query.AsNoTracking().ToList();
        }

        public List<LOG> GetAllItensMesCorrente(Int32 idAss)
        {
            IQueryable<LOG> query = Db.LOG.Where(p => p.LOG_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA).Value.Month == DbFunctions.TruncateTime(DateTime.Today.Date).Value.Month & DbFunctions.TruncateTime(p.LOG_DT_DATA).Value.Year == DbFunctions.TruncateTime(DateTime.Today.Date).Value.Year);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.LOG_IN_SISTEMA == 6);
            query = query.OrderByDescending(a => a.LOG_DT_DATA);
            return query.AsNoTracking().ToList();
        }

        public List<LOG> GetAllItensMesAnterior(Int32 idAss)
        {
            var currentMonth = DateTime.Today.Month;
            var previousMonth = DateTime.Today.AddMonths(-1).Month;
            var year = DateTime.Today.Year;
            IQueryable<LOG> query = Db.LOG.Where(p => p.LOG_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.LOG_IN_SISTEMA == 6);
            if (currentMonth == 1)
            {
                previousMonth = 12;
                year -= year;
            }
            query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA).Value.Month == previousMonth & DbFunctions.TruncateTime(p.LOG_DT_DATA).Value.Year == year);
            query = query.OrderByDescending(a => a.LOG_DT_DATA);
            return query.AsNoTracking().ToList();
        }

        public List<LOG> GetAllItensUsuario(Int32 id, Int32 idAss)
        {
            IQueryable<LOG> query = Db.LOG.Where(p => p.LOG_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Where(p => p.LOG_IN_SISTEMA == 6);
            query = query.OrderByDescending(a => a.LOG_DT_DATA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<LOG> ExecuteFilter(Int32? usuId, DateTime? dataInicio, DateTime? dataFim, String operacao, Int32 idAss)
        {
            List<LOG> lista = new List<LOG>();
            IQueryable<LOG> query = Db.LOG;
            if (!String.IsNullOrEmpty(operacao))
            {
                query = query.Where(p => p.LOG_NM_OPERACAO.ToUpper().Contains(operacao.ToUpper()));
            }
            if (usuId != 0)
            {
                query = query.Where(p => p.USUARIO.USUA_CD_ID == usuId);
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.LOG_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.LOG_IN_SISTEMA == 6);
                query = query.Include(p => p.USUARIO);
                query = query.OrderByDescending(a => a.LOG_DT_DATA);
                lista = query.AsNoTracking().ToList<LOG>();
            }
            return lista;
        }

    }
}
 