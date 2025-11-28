using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMovimentacaoEstoqueRepository : IRepositoryBase<MOVIMENTO_ESTOQUE_PRODUTO>
    {
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItens(Int32 idAss);
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensAdm(Int32 idAss);
        MOVIMENTO_ESTOQUE_PRODUTO GetItemById(Int32 id);
        List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilter(Int32? es, Int32? tipo, Int32? resp, DateTime? data, Int32 idAss);

    }
}
