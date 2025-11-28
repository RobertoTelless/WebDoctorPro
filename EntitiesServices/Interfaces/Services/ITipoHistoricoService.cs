using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoHistoricoService : IServiceBase<TIPO_HISTORICO>
    {
        Int32 Create(TIPO_HISTORICO perfil, LOG log);
        Int32 Create(TIPO_HISTORICO perfil);
        Int32 Edit(TIPO_HISTORICO perfil, LOG log);
        Int32 Edit(TIPO_HISTORICO perfil);
        Int32 Delete(TIPO_HISTORICO perfil, LOG log);

        TIPO_HISTORICO CheckExist(TIPO_HISTORICO item, Int32 idAss);
        List<TIPO_HISTORICO> GetAllItens(Int32 idAss);
        TIPO_HISTORICO GetItemById(Int32 id);
        List<TIPO_HISTORICO> GetAllItensAdm(Int32 idAss);
    }
}
