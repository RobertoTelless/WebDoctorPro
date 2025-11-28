using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IConvenioAppService : IAppServiceBase<CONVENIO>
    {
        Int32 ValidateCreate(CONVENIO item, USUARIO usuario);
        Int32 ValidateEdit(CONVENIO item, CONVENIO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CONVENIO item, USUARIO usuario);
        Int32 ValidateReativar(CONVENIO item, USUARIO usuario);

        CONVENIO CheckExist(CONVENIO item, Int32 idAss);
        List<CONVENIO> GetAllItens(Int32 idAss);
        CONVENIO GetItemById(Int32 id);
        List<CONVENIO> GetAllItensAdm(Int32 idAss);

    }
}
