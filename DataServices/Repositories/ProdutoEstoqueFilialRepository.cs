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
    public class ProdutoEstoqueFilialRepository : RepositoryBase<PRODUTO_ESTOQUE_FILIAL>, IProdutoEstoqueFilialRepository
    {
        public List<PRODUTO_ESTOQUE_FILIAL> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO_ESTOQUE_FILIAL> query = Db.PRODUTO_ESTOQUE_FILIAL.Where(p => p.PREF_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PREF_IN_SISTEMA == 6);
            query = query.Include(p => p.PRODUTO);
            return query.ToList();
        }

        public PRODUTO_ESTOQUE_FILIAL GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_ESTOQUE_FILIAL> query = Db.PRODUTO_ESTOQUE_FILIAL.Where(p => p.PREF_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 