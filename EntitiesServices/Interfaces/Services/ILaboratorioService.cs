using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ILaboratorioService : IServiceBase<LABORATORIO>
    {
        Int32 Create(LABORATORIO perfil, LOG log);
        Int32 Create(LABORATORIO perfil);
        Int32 Edit(LABORATORIO perfil, LOG log);
        Int32 Edit(LABORATORIO perfil);
        Int32 Delete(LABORATORIO perfil, LOG log);

        LABORATORIO CheckExist(LABORATORIO item, Int32 idAss);
        List<LABORATORIO> GetAllItens(Int32 idAss);
        LABORATORIO GetItemById(Int32 id);
        List<LABORATORIO> GetAllItensAdm(Int32 idAss);

        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);

    }
}
