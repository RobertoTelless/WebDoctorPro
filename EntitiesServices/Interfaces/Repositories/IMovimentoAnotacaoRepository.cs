using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMovimentoAnotacaoRepository : IRepositoryBase<MOVIMENTO_ANOTACAO>
    {
        List<MOVIMENTO_ANOTACAO> GetAllItens();
        MOVIMENTO_ANOTACAO GetItemById(Int32 id);
    }
}
