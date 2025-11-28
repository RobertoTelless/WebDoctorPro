using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoEnvioRepository : IRepositoryBase<TIPO_ENVIO>
    {
        TIPO_ENVIO CheckExist(TIPO_ENVIO item, Int32 idAss);
        List<TIPO_ENVIO> GetAllItens(Int32 idAss);
        TIPO_ENVIO GetItemById(Int32 id);
        List<TIPO_ENVIO> GetAllItensAdm(Int32 idAss);
    }
}
