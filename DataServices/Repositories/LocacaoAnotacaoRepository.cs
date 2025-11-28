using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class LocacaoAnotacaoRepository : RepositoryBase<LOCACAO_ANOTACAO>, ILocacaoAnotacaoRepository
    {
        public List<LOCACAO_ANOTACAO> GetAllItens()
        {
            return Db.LOCACAO_ANOTACAO.ToList();
        }

        public LOCACAO_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<LOCACAO_ANOTACAO> query = Db.LOCACAO_ANOTACAO.Where(p => p.LOAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 