using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMedicoEnvioRepository : IRepositoryBase<MEDICOS_ENVIO>
    {
        List<MEDICOS_ENVIO> GetAllItens(Int32 idAss);
        MEDICOS_ENVIO GetItemById(Int32 id);
    }
}
