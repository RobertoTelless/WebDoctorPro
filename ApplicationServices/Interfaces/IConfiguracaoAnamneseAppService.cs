using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IConfiguracaoAnamneseAppService : IAppServiceBase<CONFIGURACAO_ANAMNESE>
    {
        Int32 ValidateEdit(CONFIGURACAO_ANAMNESE item, CONFIGURACAO_ANAMNESE itemAntes, USUARIO usuario);
        CONFIGURACAO_ANAMNESE GetItemById(Int32 id);
        Int32 ValidateEdit(CONFIGURACAO_ANAMNESE item);

        List<CONFIGURACAO_ANAMNESE> GetAllItems(Int32 idAss);
        Int32 ValidateCreate(CONFIGURACAO_ANAMNESE item);
    }
}
