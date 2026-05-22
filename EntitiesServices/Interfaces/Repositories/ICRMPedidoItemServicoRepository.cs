using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMPedidoItemServicoRepository : IRepositoryBase<CRM_PEDIDO_VENDA_ITEM_SERVICO>
    {
        List<CRM_PEDIDO_VENDA_ITEM_SERVICO> GetAllItens();
        CRM_PEDIDO_VENDA_ITEM_SERVICO GetItemById(Int32 id);
    }
}
