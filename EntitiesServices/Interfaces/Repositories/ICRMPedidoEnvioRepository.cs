using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMPedidoEnvioRepository : IRepositoryBase<CRM_PEDIDO_ENVIO>
    {
        List<CRM_PEDIDO_ENVIO> GetAllItens(Int32 idAss);
        CRM_PEDIDO_ENVIO GetItemById(Int32 id);
        CRM_PEDIDO_ENVIO GetByProposta(Int32 prop, Int32 idAss);
    }
}
