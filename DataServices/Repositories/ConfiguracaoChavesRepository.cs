using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class ConfiguracaoChavesRepository : RepositoryBase<CONFIGURACAO_CHAVES>, IConfiguracaoChavesRepository
    {
        public CONFIGURACAO_CHAVES GetItemById(Int32 id)
        {
            IQueryable<CONFIGURACAO_CHAVES> query = Db.CONFIGURACAO_CHAVES;
            query = query.Where(p => p.CFBA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONFIGURACAO_CHAVES> GetAllItems()
        {
            IQueryable<CONFIGURACAO_CHAVES> query = Db.CONFIGURACAO_CHAVES;
            return query.ToList();
        }

    }
}
 