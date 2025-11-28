using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoEstoqueFilialRepository : IRepositoryBase<PRODUTO_ESTOQUE_FILIAL>
    {
        List<PRODUTO_ESTOQUE_FILIAL> GetAllItens(Int32 idAss);
        PRODUTO_ESTOQUE_FILIAL GetItemById(Int32 id);
    }
}
