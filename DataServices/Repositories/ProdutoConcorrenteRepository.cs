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
    public class ProdutoConcorrenteRepository : RepositoryBase<PRODUTO_CONCORRENTE>, IProdutoConcorrenteRepository
    {
        public List<PRODUTO_CONCORRENTE> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO_CONCORRENTE> query = Db.PRODUTO_CONCORRENTE.Where(p => p.PRPF_IN_ATIVO == 1);
            query = query.Where(p => p.PRPF_IN_SISTEMA == 6);
            return query.ToList();
        }

        public PRODUTO_CONCORRENTE GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_CONCORRENTE> query = Db.PRODUTO_CONCORRENTE.Where(p => p.PRPF_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 