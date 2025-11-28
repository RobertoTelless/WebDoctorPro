using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPerfilAppService : IAppServiceBase<PERFIL>
    {
        Int32 ValidateCreate(PERFIL perfil, USUARIO usuario);
        Int32 ValidateCreate(PERFIL perfil);
        Int32 ValidateEdit(PERFIL perfil, PERFIL perfilAntes, USUARIO usuario);
        Int32 ValidateDelete(PERFIL perfil, USUARIO usuario);

        PERFIL CheckExist(PERFIL item, Int32? idAss);
        PERFIL GetByName(String nome, Int32? idAss);
        USUARIO GetUserProfile(PERFIL perfil);
        List<PERFIL> GetAllItens(Int32? idAss);
        PERFIL GetItemById(Int32? id);
    }
}
