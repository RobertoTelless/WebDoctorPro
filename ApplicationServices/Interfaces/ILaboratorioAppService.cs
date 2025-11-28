using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ILaboratorioAppService : IAppServiceBase<LABORATORIO>
    {
        Int32 ValidateCreate(LABORATORIO item, USUARIO usuario);
        Int32 ValidateEdit(LABORATORIO item, LABORATORIO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(LABORATORIO item, USUARIO usuario);
        Int32 ValidateReativar(LABORATORIO item, USUARIO usuario);

        LABORATORIO CheckExist(LABORATORIO item, Int32 idAss);
        List<LABORATORIO> GetAllItens(Int32 idAss);
        LABORATORIO GetItemById(Int32 id);
        List<LABORATORIO> GetAllItensAdm(Int32 idAss);

        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);

    }
}
