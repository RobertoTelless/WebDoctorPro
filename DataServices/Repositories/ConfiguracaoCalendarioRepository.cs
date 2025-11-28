using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class ConfiguracaoCalendarioRepository : RepositoryBase<CONFIGURACAO_CALENDARIO>, IConfiguracaoCalendarioRepository
    {
        public CONFIGURACAO_CALENDARIO GetItemById(Int32 id)
        {
            IQueryable<CONFIGURACAO_CALENDARIO> query = Db.CONFIGURACAO_CALENDARIO;
            query = query.Where(p => p.ASSI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONFIGURACAO_CALENDARIO> GetAllItems(Int32 idAss)
        {
            IQueryable<CONFIGURACAO_CALENDARIO> query = Db.CONFIGURACAO_CALENDARIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 