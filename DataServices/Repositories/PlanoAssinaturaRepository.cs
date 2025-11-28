using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PlanoAssinaturaRepository : RepositoryBase<PLANO_ASSINATURA>, IPlanoAssinaturaRepository
    {
        public List<PLANO_ASSINATURA> GetAllItens()
        {
            IQueryable<PLANO_ASSINATURA> query = Db.PLANO_ASSINATURA.Where(p => p.PLAS_IN_ATIVO == 1);
            query = query.Where(p => p.PLAS_IN_SISTEMA == 6);
            return query.ToList();
        }

        public PLANO_ASSINATURA GetItemById(Int32 id)
        {
            IQueryable<PLANO_ASSINATURA> query = Db.PLANO_ASSINATURA.Where(p => p.PLAS_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 