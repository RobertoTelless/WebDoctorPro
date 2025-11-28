using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoLogRepository : IRepositoryBase<PRODUTO_LOG>
    {
        List<PRODUTO_LOG> GetAllItens(Int32 idAss);
        PRODUTO_LOG GetItemById(Int32 id);
    }
}
