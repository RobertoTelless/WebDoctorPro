using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAcessoMetodoAppService : IAppServiceBase<ACESSO_METODO>
    {
        Int32 ValidateCreate(ACESSO_METODO item);
        Int32 ValidateEdit(ACESSO_METODO item);

        List<ACESSO_METODO> GetAllItens(Int32 idAss);
        ACESSO_METODO GetItemById(Int32 id);
        Tuple<Int32, List<ACESSO_METODO>, Boolean> ExecuteFilter(Int32? usuario, DateTime? inicio, DateTime? final, String sigla, String entidade, String metodo, Int32 idAss);
    }
}
