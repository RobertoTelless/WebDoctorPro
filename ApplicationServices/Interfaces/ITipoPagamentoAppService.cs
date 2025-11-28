using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoPagamentoAppService : IAppServiceBase<TIPO_PAGAMENTO>
    {
        Int32 ValidateCreate(TIPO_PAGAMENTO item, USUARIO usuario);
        Int32 ValidateCreate(TIPO_PAGAMENTO item);
        Int32 ValidateEdit(TIPO_PAGAMENTO item, TIPO_PAGAMENTO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_PAGAMENTO item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_PAGAMENTO item, USUARIO usuario);

        TIPO_PAGAMENTO CheckExist(TIPO_PAGAMENTO item, Int32 idAss);
        List<TIPO_PAGAMENTO> GetAllItens(Int32 idAss);
        TIPO_PAGAMENTO GetItemById(Int32 id);
        List<TIPO_PAGAMENTO> GetAllItensAdm(Int32 idAss);

    }
}
