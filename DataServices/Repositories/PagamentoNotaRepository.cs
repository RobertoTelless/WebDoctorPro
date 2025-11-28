using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PagamentoNotaRepository : RepositoryBase<PAGAMENTO_NOTA_FISCAL>, IPagamentoNotaRepository
    {
        public List<PAGAMENTO_NOTA_FISCAL> GetAllItens()
        {
            return Db.PAGAMENTO_NOTA_FISCAL.ToList();
        }

        public PAGAMENTO_NOTA_FISCAL GetItemById(Int32 id)
        {
            IQueryable<PAGAMENTO_NOTA_FISCAL> query = Db.PAGAMENTO_NOTA_FISCAL.Where(p => p.PANF_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 