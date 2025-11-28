using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IConfiguracaoCalendarioAppService : IAppServiceBase<CONFIGURACAO_CALENDARIO>
    {
        Int32 ValidateEdit(CONFIGURACAO_CALENDARIO item, CONFIGURACAO_CALENDARIO itemAntes, USUARIO usuario);
        CONFIGURACAO_CALENDARIO GetItemById(Int32 id);
        Int32 ValidateEdit(CONFIGURACAO_CALENDARIO item);

        List<CONFIGURACAO_CALENDARIO> GetAllItems(Int32 idAss);
        Int32 ValidateCreate(CONFIGURACAO_CALENDARIO item);

        CONFIGURACAO_CALENDARIO_BLOQUEIO GetBloqueioById(Int32 id);
        Int32 ValidateEditBloqueio(CONFIGURACAO_CALENDARIO_BLOQUEIO item);
        Int32 ValidateCreateBloqueio(CONFIGURACAO_CALENDARIO_BLOQUEIO item);
        List<CONFIGURACAO_CALENDARIO_BLOQUEIO> GetAllBloqueio(Int32 idAss);
    }
}
