using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class RecebimentoAnotacaoRepository : RepositoryBase<RECEBIMENTO_ANOTACAO>, IRecebimentoAnotacaoRepository
    {
        public List<RECEBIMENTO_ANOTACAO> GetAllItens()
        {
            return Db.RECEBIMENTO_ANOTACAO.ToList();
        }

        public RECEBIMENTO_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<RECEBIMENTO_ANOTACAO> query = Db.RECEBIMENTO_ANOTACAO.Where(p => p.REAT_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 