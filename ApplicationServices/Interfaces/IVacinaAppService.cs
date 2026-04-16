using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IVacinaAppService : IAppServiceBase<VACINA>
    {
        Int32 ValidateCreate(VACINA item, USUARIO usuario);
        Int32 ValidateEdit(VACINA item, VACINA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(VACINA item, USUARIO usuario);
        Int32 ValidateReativar(VACINA item, USUARIO usuario);

        VACINA CheckExist(VACINA item, Int32 idAss);
        List<VACINA> GetAllItens(Int32 idAss);
        VACINA GetItemById(Int32 id);

    }
}
