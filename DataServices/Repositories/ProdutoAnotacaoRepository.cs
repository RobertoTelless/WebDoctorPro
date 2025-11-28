using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoAnotacaoRepository : RepositoryBase<PRODUTO_ANOTACAO>, IProdutoAnotacaoRepository
    {
        public List<PRODUTO_ANOTACAO> GetAllItens()
        {
            return Db.PRODUTO_ANOTACAO.ToList();
        }

        public PRODUTO_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_ANOTACAO> query = Db.PRODUTO_ANOTACAO.Where(p => p.PRAT_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 