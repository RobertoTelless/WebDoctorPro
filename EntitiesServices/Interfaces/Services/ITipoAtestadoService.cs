using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoAtestadoService : IServiceBase<TIPO_ATESTADO>
    {
        Int32 Create(TIPO_ATESTADO perfil, LOG log);
        Int32 Create(TIPO_ATESTADO perfil);
        Int32 Edit(TIPO_ATESTADO perfil, LOG log);
        Int32 Edit(TIPO_ATESTADO perfil);
        Int32 Delete(TIPO_ATESTADO perfil, LOG log);

        TIPO_ATESTADO CheckExist(TIPO_ATESTADO item, Int32 idAss);
        List<TIPO_ATESTADO> GetAllItens(Int32 idAss);
        TIPO_ATESTADO GetItemById(Int32 id);
        List<TIPO_ATESTADO> GetAllItensAdm(Int32 idAss);
    }
}
