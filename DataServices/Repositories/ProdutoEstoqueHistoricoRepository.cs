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
    public class ProdutoEstoqueHistoricoRepository : RepositoryBase<PRODUTO_ESTOQUE_HISTORICO>, IProdutoEstoqueHistoricoRepository
    {
        public List<PRODUTO_ESTOQUE_HISTORICO> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO_ESTOQUE_HISTORICO> query = Db.PRODUTO_ESTOQUE_HISTORICO.Where(p => p.PREH_IN_ATIVO == 1);
            query = query.Where(p => p.PREH_IN_SISTEMA == 6);
            return query.AsNoTracking().ToList();
        }

        public PRODUTO_ESTOQUE_HISTORICO GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_ESTOQUE_HISTORICO> query = Db.PRODUTO_ESTOQUE_HISTORICO.Where(p => p.PREH_IN_ATIVO == id);
            return query.FirstOrDefault();
        }
    }
}
 