using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class LeadAnotacaoRepository : RepositoryBase<LEAD_ANOTACAO>, ILeadAnotacaoRepository
    {
        public List<LEAD_ANOTACAO> GetAllItens()
        {
            return Db.LEAD_ANOTACAO.ToList();
        }

        public LEAD_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<LEAD_ANOTACAO> query = Db.LEAD_ANOTACAO.Where(p => p.LEAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 