using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILocacaoOcorrenciaRepository : IRepositoryBase<LOCACAO_OCORRENCIA>
    {
        List<LOCACAO_OCORRENCIA> GetAllItens(Int32 idAss);
        LOCACAO_OCORRENCIA GetItemById(Int32 id);
    }
}
