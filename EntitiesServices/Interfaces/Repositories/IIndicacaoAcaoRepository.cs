using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IIndicacaoAcaoRepository : IRepositoryBase<INDICACAO_ACAO>
    {
        List<INDICACAO_ACAO> GetAllItens(Int32 idAss);
        INDICACAO_ACAO GetItemById(Int32 id);
    }
}
