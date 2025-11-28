using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRecebimentoAnotacaoRepository : IRepositoryBase<RECEBIMENTO_ANOTACAO>
    {
        List<RECEBIMENTO_ANOTACAO> GetAllItens();
        RECEBIMENTO_ANOTACAO GetItemById(Int32 id);
    }
}
