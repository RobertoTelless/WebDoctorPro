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
    public class ProdutoCustoRepository : RepositoryBase<PRODUTO_CUSTO>, IProdutoCustoRepository
    {
        public PRODUTO_CUSTO CheckExist(PRODUTO_CUSTO conta, Int32 idAss)
        {
            IQueryable<PRODUTO_CUSTO> query = Db.PRODUTO_CUSTO;
            query = query.Where(p => p.PRCU_DT_CUSTO == conta.PRCU_DT_CUSTO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PRCU_IN_ATIVO == 1);
            query = query.Where(p => p.PRCU_IN_SISTEMA == 6);
            return query.AsNoTracking().FirstOrDefault();
        }

        public List<PRODUTO_CUSTO> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO_CUSTO> query = Db.PRODUTO_CUSTO.Where(p => p.PRCU_IN_ATIVO == 1);
            query = query.Where(p => p.PRCU_IN_SISTEMA == 6);
            return query.AsNoTracking().ToList();
        }

        public PRODUTO_CUSTO GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_CUSTO> query = Db.PRODUTO_CUSTO.Where(p => p.PRCU_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 