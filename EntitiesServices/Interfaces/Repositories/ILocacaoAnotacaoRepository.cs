using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILocacaoAnotacaoRepository : IRepositoryBase<LOCACAO_ANOTACAO>
    {
        List<LOCACAO_ANOTACAO> GetAllItens();
        LOCACAO_ANOTACAO GetItemById(Int32 id);
    }
}
