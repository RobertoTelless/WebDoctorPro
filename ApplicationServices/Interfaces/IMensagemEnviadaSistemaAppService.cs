using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMensagemEnviadaSistemaAppService : IAppServiceBase<MENSAGENS_ENVIADAS_SISTEMA>
    {
        Int32 ValidateCreate(MENSAGENS_ENVIADAS_SISTEMA perfil);

        MENSAGENS_ENVIADAS_SISTEMA GetItemById(Int32 id);
        List<MENSAGENS_ENVIADAS_SISTEMA> GetByDate(DateTime data, Int32 idAss);
        List<MENSAGENS_ENVIADAS_SISTEMA> GetByMonth(DateTime data, Int32 idAss);
        List<MENSAGENS_ENVIADAS_SISTEMA> GetAllItens(Int32 idAss);
        Tuple<Int32, List<MENSAGENS_ENVIADAS_SISTEMA>, Boolean> ExecuteFilterTuple(Int32? escopo, Int32? tipo, DateTime? inicio, DateTime? final, String email, String celular, String destino, Int32 idAss);
    }
}
