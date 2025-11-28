using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class ConfiguracaoCalendarioBloqueioRepository : RepositoryBase<CONFIGURACAO_CALENDARIO_BLOQUEIO>, IConfiguracaoCalendarioBloqueioRepository
    {
        public CONFIGURACAO_CALENDARIO_BLOQUEIO GetItemById(Int32 id)
        {
            IQueryable<CONFIGURACAO_CALENDARIO_BLOQUEIO> query = Db.CONFIGURACAO_CALENDARIO_BLOQUEIO;
            query = query.Where(p => p.COCB_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONFIGURACAO_CALENDARIO_BLOQUEIO> GetAllItems(Int32 idAss)
        {
            IQueryable<CONFIGURACAO_CALENDARIO_BLOQUEIO> query = Db.CONFIGURACAO_CALENDARIO_BLOQUEIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 