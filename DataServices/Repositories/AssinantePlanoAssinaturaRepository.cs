using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class AssinantePlanoAssinaturaRepository : RepositoryBase<ASSINANTE_PLANO_ASSINATURA>, IAssinantePlanoAssinaturaRepository
    {
        public List<ASSINANTE_PLANO_ASSINATURA> GetAllItens(Int32 idAss)
        {
            IQueryable<ASSINANTE_PLANO_ASSINATURA> query = Db.ASSINANTE_PLANO_ASSINATURA.Where(p => p.ASPA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public ASSINANTE_PLANO_ASSINATURA GetItemById(Int32 id)
        {
            IQueryable<ASSINANTE_PLANO_ASSINATURA> query = Db.ASSINANTE_PLANO_ASSINATURA.Where(p => p.ASPA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 