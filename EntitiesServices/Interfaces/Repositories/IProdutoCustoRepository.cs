using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoCustoRepository : IRepositoryBase<PRODUTO_CUSTO>
    {
        PRODUTO_CUSTO CheckExist(PRODUTO_CUSTO item, Int32 idAss);
        List<PRODUTO_CUSTO> GetAllItens(Int32 idAss);
        PRODUTO_CUSTO GetItemById(Int32 id);
    }
}
