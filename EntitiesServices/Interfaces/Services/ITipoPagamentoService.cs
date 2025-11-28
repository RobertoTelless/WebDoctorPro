using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoPagamentoService : IServiceBase<TIPO_PAGAMENTO>
    {
        Int32 Create(TIPO_PAGAMENTO item, LOG log);
        Int32 Create(TIPO_PAGAMENTO item);
        Int32 Edit(TIPO_PAGAMENTO item, LOG log);
        Int32 Edit(TIPO_PAGAMENTO item);
        Int32 Delete(TIPO_PAGAMENTO item, LOG log);

        TIPO_PAGAMENTO CheckExist(TIPO_PAGAMENTO item, Int32 idAss);
        List<TIPO_PAGAMENTO> GetAllItens(Int32 idAss);
        TIPO_PAGAMENTO GetItemById(Int32 id);
        List<TIPO_PAGAMENTO> GetAllItensAdm(Int32 idAss);
    }
}
