using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IEspecialidadeAppService : IAppServiceBase<ESPECIALIDADE>
    {
        Int32 ValidateCreate(ESPECIALIDADE item, USUARIO usuario);
        Int32 ValidateCreate(ESPECIALIDADE item);
        Int32 ValidateEdit(ESPECIALIDADE item, ESPECIALIDADE itemAntes, USUARIO usuario);
        Int32 ValidateDelete(ESPECIALIDADE item, USUARIO usuario);
        Int32 ValidateReativar(ESPECIALIDADE item, USUARIO usuario);

        ESPECIALIDADE CheckExist(ESPECIALIDADE item, Int32 idAss);
        List<ESPECIALIDADE> GetAllItens(Int32 idAss);
        ESPECIALIDADE GetItemById(Int32 id);
        List<ESPECIALIDADE> GetAllItensAdm(Int32 idAss);
    }
}
