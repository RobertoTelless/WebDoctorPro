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
    public class ProdutoLogRepository : RepositoryBase<PRODUTO_LOG>, IProdutoLogRepository
    {
        public List<PRODUTO_LOG> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO_LOG> query = Db.PRODUTO_LOG.Where(p => p.PRLG_IN_ATIVO == 1);
            query = query.Where(p => p.PRLG_IN_SISTEMA == 6);
            return query.AsNoTracking().ToList();
        }

        public PRODUTO_LOG GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_LOG> query = Db.PRODUTO_LOG.Where(p => p.PRLG_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 