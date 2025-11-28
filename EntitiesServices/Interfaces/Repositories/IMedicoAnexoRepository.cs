using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMedicoAnexoRepository : IRepositoryBase<MEDICOS_ENVIO_ANEXO>
    {
        List<MEDICOS_ENVIO_ANEXO> GetAllItens();
        MEDICOS_ENVIO_ANEXO GetItemById(Int32 id);
    }
}
