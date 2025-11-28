using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoVideoService : IServiceBase<TIPO_VIDEO>
    {
        Int32 Create(TIPO_VIDEO perfil, LOG log);
        Int32 Create(TIPO_VIDEO perfil);
        Int32 Edit(TIPO_VIDEO perfil, LOG log);
        Int32 Edit(TIPO_VIDEO perfil);
        Int32 Delete(TIPO_VIDEO perfil, LOG log);

        TIPO_VIDEO CheckExist(TIPO_VIDEO item, Int32 idAss);
        List<TIPO_VIDEO> GetAllItens(Int32 idAss);
        TIPO_VIDEO GetItemById(Int32 id);
        List<TIPO_VIDEO> GetAllItensAdm(Int32 idAss);
    }
}
