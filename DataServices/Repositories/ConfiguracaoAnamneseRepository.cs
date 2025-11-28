using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class ConfiguracaoAnamneseRepository : RepositoryBase<CONFIGURACAO_ANAMNESE>, IConfiguracaoAnamneseRepository
    {
        public CONFIGURACAO_ANAMNESE GetItemById(Int32 id)
        {
            IQueryable<CONFIGURACAO_ANAMNESE> query = Db.CONFIGURACAO_ANAMNESE;
            query = query.Where(p => p.ASSI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONFIGURACAO_ANAMNESE> GetAllItems(Int32 idAss)
        {
            IQueryable<CONFIGURACAO_ANAMNESE> query = Db.CONFIGURACAO_ANAMNESE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 