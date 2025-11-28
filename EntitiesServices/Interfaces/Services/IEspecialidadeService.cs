using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IEspecialidadeService : IServiceBase<ESPECIALIDADE>
    {
        Int32 Create(ESPECIALIDADE perfil, LOG log);
        Int32 Create(ESPECIALIDADE perfil);
        Int32 Edit(ESPECIALIDADE perfil, LOG log);
        Int32 Edit(ESPECIALIDADE perfil);
        Int32 Delete(ESPECIALIDADE perfil, LOG log);

        ESPECIALIDADE CheckExist(ESPECIALIDADE item, Int32 idAss);
        List<ESPECIALIDADE> GetAllItens(Int32 idAss);
        ESPECIALIDADE GetItemById(Int32 id);
        List<ESPECIALIDADE> GetAllItensAdm(Int32 idAss);
    }
}
