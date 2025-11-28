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
    public class ProdutoFalhaRepository : RepositoryBase<PRODUTO_FALHA>, IProdutoFalhaRepository
    {
        public List<PRODUTO_FALHA> GetAllItens()
        {
            return Db.PRODUTO_FALHA.ToList();
        }

        public PRODUTO_FALHA GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_FALHA> query = Db.PRODUTO_FALHA.Where(p => p.PRFA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 