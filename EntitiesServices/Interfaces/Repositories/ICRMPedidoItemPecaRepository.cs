using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMPedidoItemPecaRepository : IRepositoryBase<CRM_PEDIDO_VENDA_ITEM_PECA>
    {
        List<CRM_PEDIDO_VENDA_ITEM_PECA> GetAllItens();
        CRM_PEDIDO_VENDA_ITEM_PECA GetItemById(Int32 id);
    }
}
