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
    public class ProdutoKitRepository : RepositoryBase<PRODUTO_KIT>, IProdutoKitRepository
    {

        public List<PRODUTO_KIT> GetAllItens(Int32 idAss)
        {
            return Db.PRODUTO_KIT.ToList();
        }

        public PRODUTO_KIT GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_KIT> query = Db.PRODUTO_KIT.Where(p => p.PRKI_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 