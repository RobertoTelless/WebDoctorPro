using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoPrecoVendaRepository : IRepositoryBase<PRODUTO_PRECO_VENDA>
    {
        PRODUTO_PRECO_VENDA CheckExist(PRODUTO_PRECO_VENDA item, Int32 idAss);
        List<PRODUTO_PRECO_VENDA> GetAllItens(Int32 idAss);
        PRODUTO_PRECO_VENDA GetItemById(Int32 id);
    }
}
