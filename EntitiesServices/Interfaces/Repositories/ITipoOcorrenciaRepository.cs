using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoOcorrenciaRepository : IRepositoryBase<TIPO_OCORRENCIA>
    {
        List<TIPO_OCORRENCIA> GetAllItens(Int32 id);
        TIPO_OCORRENCIA GetItemById(Int32 id);

    }
}
