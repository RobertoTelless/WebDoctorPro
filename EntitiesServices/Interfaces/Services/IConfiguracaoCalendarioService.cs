using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IConfiguracaoCalendarioService : IServiceBase<CONFIGURACAO_CALENDARIO>
    {
        CONFIGURACAO_CALENDARIO GetItemById(Int32 id);
        List<CONFIGURACAO_CALENDARIO> GetAllItems(Int32 idAss);

        Int32 Edit(CONFIGURACAO_CALENDARIO item, LOG log);
        Int32 Edit(CONFIGURACAO_CALENDARIO item);
        Int32 Create(CONFIGURACAO_CALENDARIO item);

        CONFIGURACAO_CALENDARIO_BLOQUEIO GetBloqueioById(Int32 id);
        Int32 EditBloqueio(CONFIGURACAO_CALENDARIO_BLOQUEIO item);
        Int32 CreateBloqueio(CONFIGURACAO_CALENDARIO_BLOQUEIO item);
        List<CONFIGURACAO_CALENDARIO_BLOQUEIO> GetAllBloqueio(Int32 idAss);

    }
}
