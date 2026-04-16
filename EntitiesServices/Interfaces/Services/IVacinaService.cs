using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IVacinaService : IServiceBase<VACINA>
    {
        Int32 Create(VACINA perfil, LOG log);
        Int32 Create(VACINA perfil);
        Int32 Edit(VACINA perfil, LOG log);
        Int32 Edit(VACINA perfil);
        Int32 Delete(VACINA perfil, LOG log);

        VACINA CheckExist(VACINA item, Int32 idAss);
        List<VACINA> GetAllItens(Int32 idAss);
        VACINA GetItemById(Int32 id);
    }
}
