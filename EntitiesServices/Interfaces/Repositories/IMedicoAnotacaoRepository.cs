using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMedicoAnotacaoRepository : IRepositoryBase<MEDICOS_ENVIO_ANOTACAO>
    {
        List<MEDICOS_ENVIO_ANOTACAO> GetAllItens();
        MEDICOS_ENVIO_ANOTACAO GetItemById(Int32 id);
    }
}
