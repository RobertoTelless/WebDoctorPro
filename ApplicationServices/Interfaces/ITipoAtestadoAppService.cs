using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoAtestadoAppService : IAppServiceBase<TIPO_ATESTADO>
    {
        Int32 ValidateCreate(TIPO_ATESTADO item, USUARIO usuario);
        Int32 ValidateEdit(TIPO_ATESTADO item, TIPO_ATESTADO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_ATESTADO item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_ATESTADO item, USUARIO usuario);

        TIPO_ATESTADO CheckExist(TIPO_ATESTADO item, Int32 idAss);
        List<TIPO_ATESTADO> GetAllItens(Int32 idAss);
        TIPO_ATESTADO GetItemById(Int32 id);
        List<TIPO_ATESTADO> GetAllItensAdm(Int32 idAss);

    }
}
