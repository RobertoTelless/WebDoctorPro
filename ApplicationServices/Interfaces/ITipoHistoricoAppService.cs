using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoHistoricoAppService : IAppServiceBase<TIPO_HISTORICO>
    {
        Int32 ValidateCreate(TIPO_HISTORICO item, USUARIO usuario);
        Int32 ValidateCreate(TIPO_HISTORICO item);
        Int32 ValidateEdit(TIPO_HISTORICO item, TIPO_HISTORICO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_HISTORICO item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_HISTORICO item, USUARIO usuario);

        TIPO_HISTORICO CheckExist(TIPO_HISTORICO item, Int32 idAss);
        List<TIPO_HISTORICO> GetAllItens(Int32 idAss);
        TIPO_HISTORICO GetItemById(Int32 id);
        List<TIPO_HISTORICO> GetAllItensAdm(Int32 idAss);

    }
}
