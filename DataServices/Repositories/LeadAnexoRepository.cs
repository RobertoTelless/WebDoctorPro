using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class LeadAnexoRepository : RepositoryBase<LEAD_ANEXO>, ILeadAnexoRepository
    {
        public List<LEAD_ANEXO> GetAllItens()
        {
            return Db.LEAD_ANEXO.ToList();
        }

        public LEAD_ANEXO GetItemById(Int32 id)
        {
            IQueryable<LEAD_ANEXO> query = Db.LEAD_ANEXO.Where(p => p.LEAX_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 