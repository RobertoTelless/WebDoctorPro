using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoPagamentoRepository : IRepositoryBase<TIPO_PAGAMENTO>
    {
        TIPO_PAGAMENTO CheckExist(TIPO_PAGAMENTO item, Int32 idAss);
        List<TIPO_PAGAMENTO> GetAllItens(Int32 idAss);
        TIPO_PAGAMENTO GetItemById(Int32 id);
        List<TIPO_PAGAMENTO> GetAllItensAdm(Int32 idAss);
    }
}
