using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PagamentoAnotacaoRepository : RepositoryBase<PAGAMENTO_ANOTACAO>, IPagamentoAnotacaoRepository
    {
        public List<PAGAMENTO_ANOTACAO> GetAllItens()
        {
            return Db.PAGAMENTO_ANOTACAO.ToList();
        }

        public PAGAMENTO_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<PAGAMENTO_ANOTACAO> query = Db.PAGAMENTO_ANOTACAO.Where(p => p.PGAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 