using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoFalhaRepository : IRepositoryBase<PRODUTO_FALHA>
    {
        List<PRODUTO_FALHA> GetAllItens();
        PRODUTO_FALHA GetItemById(Int32 id);

    }
}
