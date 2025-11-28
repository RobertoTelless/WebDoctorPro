using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAvisoLembreteAppService : IAppServiceBase<AVISO_LEMBRETE>
    {
        Int32 ValidateCreate(AVISO_LEMBRETE item, USUARIO usuario);
        Int32 ValidateCreate(AVISO_LEMBRETE item);
        Int32 ValidateEdit(AVISO_LEMBRETE item, AVISO_LEMBRETE itemAntes, USUARIO usuario);
        Int32 ValidateDelete(AVISO_LEMBRETE item, USUARIO usuario);
        Int32 ValidateReativar(AVISO_LEMBRETE item, USUARIO usuario);

        List<AVISO_LEMBRETE> GetAllItens(Int32 idAss);
        AVISO_LEMBRETE GetItemById(Int32 id);
        Tuple<Int32, List<AVISO_LEMBRETE>, Boolean> ExecuteFilter(String titulo, DateTime? inicio, DateTime? final, Int32? ciente, Int32 idAss);
    }
}
