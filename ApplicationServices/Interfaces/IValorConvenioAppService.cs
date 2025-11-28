using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IValorConvenioAppService : IAppServiceBase<VALOR_CONVENIO>
    {
        Int32 ValidateCreate(VALOR_CONVENIO item, USUARIO usuario);
        Int32 ValidateEdit(VALOR_CONVENIO item, VALOR_CONVENIO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(VALOR_CONVENIO item, USUARIO usuario);
        Int32 ValidateReativar(VALOR_CONVENIO item, USUARIO usuario);

        VALOR_CONVENIO CheckExist(VALOR_CONVENIO item, Int32 idAss);
        List<VALOR_CONVENIO> GetAllItens(Int32 idAss);
        VALOR_CONVENIO GetItemById(Int32 id);
        List<VALOR_CONVENIO> GetAllItensAdm(Int32 idAss);

        List<CONVENIO> GetAllConvenios(Int32 idAss);
    }
}
