using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAcessoMetodoService : IServiceBase<ACESSO_METODO>
    {
        Int32 Create(ACESSO_METODO item);
        Int32 Edit(ACESSO_METODO item);

        List<ACESSO_METODO> GetAllItens(Int32 idAss);
        ACESSO_METODO GetItemById(Int32 id);
        List<ACESSO_METODO> ExecuteFilter(Int32? usuario, DateTime? inicio, DateTime? final, String sigla, String entidade, String metodo, Int32 idAss);
    }
}
