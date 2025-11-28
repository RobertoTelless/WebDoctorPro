using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class AssinanteAnotacaoRepository : RepositoryBase<ASSINANTE_ANOTACAO>, IAssinanteAnotacaoRepository
    {
        public List<ASSINANTE_ANOTACAO> GetAllItens()
        {
            return Db.ASSINANTE_ANOTACAO.ToList();
        }

        public ASSINANTE_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<ASSINANTE_ANOTACAO> query = Db.ASSINANTE_ANOTACAO.Where(p => p.ASAT_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 