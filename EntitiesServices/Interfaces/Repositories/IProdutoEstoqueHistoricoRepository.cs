using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoEstoqueHistoricoRepository : IRepositoryBase<PRODUTO_ESTOQUE_HISTORICO>
    {
        List<PRODUTO_ESTOQUE_HISTORICO> GetAllItens(Int32 idAss);
        PRODUTO_ESTOQUE_HISTORICO GetItemById(Int32 id);
    }
}
