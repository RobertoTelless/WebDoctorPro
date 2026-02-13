using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class AcessoSiteRepository : RepositoryBase<ACESSO_SITE>, IAcessoSiteRepository
    {
        public ACESSO_SITE GetItemById(Int32 id)
        {
            IQueryable<ACESSO_SITE> query = Db.ACESSO_SITE;
            query = query.Where(p => p.ACST_CD_ID == id);
            return query.AsNoTracking().FirstOrDefault();
        }

        public List<ACESSO_SITE> GetAllItens()
        {
            IQueryable<ACESSO_SITE> query = Db.ACESSO_SITE.Where(p => p.ACST_IN_ATIVO == 1);
            query = query.Where(p => p.ACST_IN_SISTEMA == 6);
            return query.AsNoTracking().ToList();
        }
    }
}
 