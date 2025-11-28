using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoAnotacaoRepository : IRepositoryBase<PRODUTO_ANOTACAO>
    {
        List<PRODUTO_ANOTACAO> GetAllItens();
        PRODUTO_ANOTACAO GetItemById(Int32 id);
    }
}
