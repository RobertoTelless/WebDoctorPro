using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PagamentoAnexoRepository : RepositoryBase<PAGAMENTO_ANEXO>, IPagamentoAnexoRepository
    {
        public List<PAGAMENTO_ANEXO> GetAllItens()
        {
            return Db.PAGAMENTO_ANEXO.ToList();
        }

        public PAGAMENTO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<PAGAMENTO_ANEXO> query = Db.PAGAMENTO_ANEXO.Where(p => p.PAAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 