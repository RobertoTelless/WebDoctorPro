using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class AssinantePagamentoRepository : RepositoryBase<ASSINANTE_PAGAMENTO>, IAssinantePagamentoRepository
    {
        public List<ASSINANTE_PAGAMENTO> GetAllItens()
        {
            IQueryable<ASSINANTE_PAGAMENTO> query = Db.ASSINANTE_PAGAMENTO.Where(p => p.ASPA_IN_ATIVO == 1);
            return query.ToList();
        }

        public ASSINANTE_PAGAMENTO GetItemById(Int32 id)
        {
            IQueryable<ASSINANTE_PAGAMENTO> query = Db.ASSINANTE_PAGAMENTO.Where(p => p.ASPA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 