using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class ConfiguracaoRepository : RepositoryBase<CONFIGURACAO>, IConfiguracaoRepository
    {
        public CONFIGURACAO GetItemById(Int32 id)
        {
            IQueryable<CONFIGURACAO> query = Db.CONFIGURACAO;
            query = query.Where(p => p.ASSI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONFIGURACAO> GetAllItems(Int32 idAss)
        {
            IQueryable<CONFIGURACAO> query = Db.CONFIGURACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

    }
}
 